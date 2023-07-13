using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using StateManager;
using DG.Tweening;
using EffectData;
using HPDataBase;

public class PlayerChip : MonoBehaviour
{
    GameManager gameManager;
    TouchManager touchManager;
    Camera _camera; Animator anim; Animator armAnim;
    [System.NonSerialized] public Rigidbody2D rb2D;
    //[System.NonSerialized] public Collider2D hitBox; //M_attackParticleに登録する必要がある
    public GameObject attackCollider;
    [SerializeField] GroundCollider[] groundCollider; bool[] isGround = new bool[1];
    [SerializeField] PointerScript pointerScript;

    #region//スキル関連
    [System.Serializable]
    public class PlayerChipSkillClass
    {
        public GameObject DoubleEdgedSword;
        public List<TagDetection> wallDitection = new List<TagDetection>();

        public List<Collider2D> M_AttackParticleTriggerColliders = new List<Collider2D>(); //M_attackParticleに登録する必要がある
    }
    public PlayerChipSkillClass playerChipskillClass;
    #endregion

    public void StartProcess()
    {
        gameManager = GameManager.gameManager;
        touchManager = new TouchManager();
        _camera = Camera.main;
        rb2D = GetComponent<Rigidbody2D>(); //hitBox = disableObjData.delateObjs[0].GetComponent<Collider2D>();
        anim = GetComponent<Animator>(); animScale = stickmanBody.localScale;
        armAnim = transform.Find("stickman/armPointer").GetComponent<Animator>();
        //mdData.arrowPointerScale = mdData.arrowPointer.localScale;
        recordSpearScale = spearTrans.localScale;

        pointerScript.StartProcess();
    }

    public void UpdateProcess()
    {
        // タッチ状態更新
        touchManager.UpdateProcess();
        // タッチ取得
        TouchManager touchState = touchManager.GetTouch();
        //
        pointerScript.InfoRegister(default, default, 1f);
        MouseDrag(touchState);
        //
        PlayerDire();
    }
    public void FixedUpdateProcess()
    {
        FirstProcess();

        #region//スキル発動
        //スキル_クライマー
        gameManager.skillManager.Climber(rb2D, playerChipskillClass.wallDitection);
        //スキル_トライデント
        
        #endregion
        //if (isGroundTrigger) EnergyGroundRecovery();
        EnergyRecovery();

        LastProcess();
    }
    #region//フレーム最初処理
    void FirstProcess()
    {
        for (int a = 0; a < groundCollider.Length; ++a) { isGround[a] = groundCollider[a].IsGround(); }
    }
    #endregion
    #region//フレーム最終処理
    void LastProcess()
    {
        if (isGround[0]) Idle();
        else UpJump();

        
        
        DeltaPos();
        switch (CurrentWeponState)
        {
            case WeponState.Spear:
                ParabolaArrow();
                SpearArmTrans();
                break;
            case WeponState.Bow:
                IdleBow();
                PullBow();
                break;
        }
        

        StateCheck();
    }
    #endregion

    #region//武器変更
    public enum WeponState
    {
        Spear, Bow
    }
    public WeponState CurrentWeponState
    {
        protected set; get;
    }
    float gravityScale = 1f;
    [SerializeField] GameObject Spear;
    [SerializeField] GameObject Bow;
    public void ChangeWepon(WeponState weponState)
    {
        CurrentWeponState = weponState;
        switch (CurrentWeponState)
        {
            case WeponState.Spear:
                //スキル_トライデント
                gravityScale = gameManager.skillManager.Trident(rb2D.gravityScale);
                Spear.SetActive(true); Bow.SetActive(false);
                break;
            case WeponState.Bow:
                //gravityScale = 0.75f; //スキル等で設定できるように
                //スキル_トライデント
                gameManager.skillManager.Trident(false, rb2D);
                //スキル_聖なる矢
                gravityScale = gameManager.skillManager.HollyArrow(0.75f/*←基準値*/);
                Spear.SetActive(false); Bow.SetActive(true);
                break;
        }
    }
    //スキル_トライデントand聖なる矢
    public void ChangeWepon(SkillDataBase.SkillEnum skillEnum)
    {
        switch (CurrentWeponState)
        {
            case WeponState.Spear:
                //スキル_トライデント
                if(skillEnum == SkillDataBase.SkillEnum.Trident) gravityScale = gameManager.skillManager.Trident(rb2D.gravityScale);
                break;
            case WeponState.Bow:
                //スキル_聖なる矢
                if(skillEnum == SkillDataBase.SkillEnum.HollyArrow) gravityScale = gameManager.skillManager.HollyArrow(0.75f/*←基準値*/);
                break;
        }
    }
    #endregion

    #region//スワイプ処理
    [System.Serializable] class MouseDragData
    {
        [System.NonSerialized] public Vector2 StartPos;[System.NonSerialized] public Vector2 EndPos;
        //public Transform arrowPointer; [System.NonSerialized] public Vector2 arrowPointerScale;
        public float upperLimit; //ベクトル上限値
    }
    [SerializeField] MouseDragData mdData = new MouseDragData();
    #region//スワイプ許可論理型
    //int swipePermissionBoolInt;
    /*public bool SwipePermissionBool
    {
        set {
            if (value) { ++swipePermissionBoolInt; }
            else { --swipePermissionBoolInt; }
        }
        protected get { return SwipePermissionBool; }
    }*/
    #endregion
    void MouseDrag(TouchManager touchManager)
    {
        if (!gameManager.canvasManager.swipeUIData.trigger) return;
        if (!touchManager._touch_flag) return;
        if (CurrentWeponState == WeponState.Spear && gameManager.hpManager.EnergyCheck()) return;
        //if (gameManager.hpManager.EnergyCheck()) { gameManager.canvasManager.SwipeStart(false); return; }

        switch (touchManager._touch_phase)
        {
            case TouchPhase.Began:
                //↓共有プログラム（武器関係）
                mdData.StartPos = _camera.ScreenToWorldPoint(touchManager._touch_position);

                RectTransform canvasRect = gameManager.canvasManager.canvasRect;
                Vector2 viewportPosition = _camera.WorldToViewportPoint(mdData.StartPos);
                gameManager.canvasManager.swipeUIData.defaultPointerPos = new Vector2(
                    ((viewportPosition.x * canvasRect.sizeDelta.x) - (canvasRect.sizeDelta.x * 0.5f)),
                    ((viewportPosition.y * canvasRect.sizeDelta.y) - (canvasRect.sizeDelta.y * 0.5f)));

                //テスト_ラインの色
                pointerScript.FadeLine(false);
                //スキル_イージスシールド
                gameManager.skillManager.AegisShieldFade(false);
                //↑共有プログラム（武器関係）
                break;
            case TouchPhase.Moved:
                //↓共有プログラム（武器関係）
                Vector2 middleDiff = mdData.StartPos - new Vector2(_camera.ScreenToWorldPoint(touchManager._touch_position).x, _camera.ScreenToWorldPoint(touchManager._touch_position).y);
                Vector2 worldPointerPos = _camera.WorldToViewportPoint(_camera.ScreenToWorldPoint(touchManager._touch_position));
                //角度
                float degree = DataBase.GetAngle(middleDiff); 
                //上限値を超えれば座標上書き
                if (middleDiff.magnitude >= mdData.upperLimit){
                    middleDiff = DataBase.CoordinateUpperLimit(degree, mdData.upperLimit); //Debug.Log("ベクトル大きさ:" + middleDiff.magnitude);
                    worldPointerPos = _camera.WorldToViewportPoint(mdData.StartPos - middleDiff);
                } 
                //ポインター描写
                stickmanArm.rotation = Quaternion.Euler(new Vector3(0, 0, degree - 90));
                //UI描写
                float swipeLengh = middleDiff.magnitude / mdData.upperLimit;
                gameManager.canvasManager.PointerCoordinate(worldPointerPos, swipeLengh);
                //スキル_ゼログラビティ
                gameManager.skillManager.ZeroGravity(rb2D);
                //スキル_イージスシールド
                gameManager.skillManager.aegisShieldData.playerArmDegree = degree;
                //スキル_トライデント
                gameManager.skillManager.TridentDegree = degree;
                //ポインター
                Vector2 pointerVec = middleDiff * 4f;
                #region//スキル発動
                pointerVec *= gameManager.skillManager.HighSpeed()/*スキル_ハイスピード*/ * gameManager.skillManager.DragonShot()/*スキル_ドラゴンショット*/;
                #endregion
                pointerScript.InfoRegister(pointerVec, swipeLengh, gravityScale);
                pointerScript.FixedUpdateProcess();
                //↑共有プログラム（武器関係）
                //武器別anim
                switch (CurrentWeponState)
                {
                    case WeponState.Spear:
                        pointerScript.setOffsetPos = Vector3.zero;
                        break;
                    case WeponState.Bow:
                        playerArmState = PlayerArmState.holdTheBow;
                        armAnim.SetFloat("motionTime", swipeLengh);
                        pointerScript.setOffsetPos = ReturnArrowPos();
                        break;
                }
                break;
            case TouchPhase.Ended:
                //↓共有プログラム（武器関係）
                mdData.EndPos = _camera.ScreenToWorldPoint(touchManager._touch_position);
                Vector2 UltmateDiff = (mdData.StartPos - mdData.EndPos); //Debug.Log("始点:" + mdData.StartPos + "終点:" + mdData.EndPos);
                //上限値を超えれば座標上書き
                if (UltmateDiff.magnitude >= mdData.upperLimit) UltmateDiff = DataBase.CoordinateUpperLimit(DataBase.GetAngle(UltmateDiff), mdData.upperLimit); 
                Vector2 vector = UltmateDiff * 4f;
                #region//スキル発動
                vector *= gameManager.skillManager.HighSpeed(); //Debug.Log(vector.magnitude+":"+ gameManager.skillManager.HighSpeed());
                #endregion
                //↑共有プログラム（武器関係）
                switch (CurrentWeponState)
                {
                    case WeponState.Spear:
                        rb2D.velocity = default;
                        ForceSpeed(vector);
                        gameManager.soundManager.Play("whips_2", 0.75f, 0.2f);
                        //スキル_トライデント
                        gameManager.skillManager.Trident(true, rb2D);
                        break;
                    case WeponState.Bow:
                        ArrowType arrowType = ArrowType.simpleArrow;
                        //スキル_聖なる矢
                        arrowType = gameManager.skillManager.HollyArrow(arrowType);
                        //スキル_ドラゴンショット
                        vector *= gameManager.skillManager.DragonShot();
                        //スキル_ダブルショット
                        int doubleShotInt = gameManager.skillManager.DoubleShot();
                        for (int cnt = 0; cnt < doubleShotInt; ++cnt)
                        {
                            //スキル_ダブルショット↓
                            float setDeltaDegree = default;
                            float multiple = 50f;
                            //奇数か偶数か
                            if(doubleShotInt % 2 == 1 && cnt != 0)
                            {
                                if (cnt % 2 == 1) setDeltaDegree += cnt * multiple;
                                else setDeltaDegree -= (cnt - 1) * multiple;
                            }
                            else if(doubleShotInt % 2 == 0)
                            {
                                if (cnt % 2 == 1) setDeltaDegree += cnt * multiple * 0.5f;
                                else setDeltaDegree -= (cnt + 1) * multiple * 0.5f;
                            }
                            //スキル_ダブルショット↑
                            
                            gameManager.effectManager.ArrowEffect(arrowType, (Vector2)transform.position + ReturnArrowPos(setDeltaDegree),
                                //スキル_巨大
                                gameManager.skillManager.Giant2(),
                                vector);
                            gameManager.soundManager.Play("whips_1", 1.2f, 0.2f);
                        }
                        //スキル_ミニショット
                        for(int miniCnt = 0; miniCnt < gameManager.skillManager.MiniShot(); ++miniCnt)
                        {
                            gameManager.effectManager.ArrowEffect(ArrowType.miniArrow, (Vector2)transform.position + ReturnArrowPos(),
                                //スキル_巨大
                                gameManager.skillManager.Giant2() * 0.25f,
                                vector);
                        }
                        ForceSpeed(-vector * 0.25f, true);
                        StartCoroutine(ArrowCooldown());
                        break;
                }
                //スキル_イージスシールド
                gameManager.skillManager.AegisShieldFade(true);
                //トリガーをオフ
                gameManager.canvasManager.SwipeStart(false);
                //テスト_ラインの色
                pointerScript.FadeLine(true);
                break;
        }
    }
    #endregion
    #region//フォース
    void ForceSpeed(Vector2 vector2, bool defaultVect = false)
    {
        if (defaultVect) rb2D.velocity = default;
        rb2D.AddForce(vector2, ForceMode2D.Impulse);
        paradolaDelay = 0;
    }
    #endregion
    #region//接地状態(エナジー回復)
    bool isGroundTrigger;
    string groundTag = "ground";

    void EnergyRecovery()
    {
        if (isGroundTrigger)
        {
            gameManager.hpManager.EnergyFluctuation(0.015f * 
            //スキル_エネルギータンク
            gameManager.skillManager.EnergyTank()
            ); //スキル_クライマーにも同様の定数で設定している
        }
        else
        {
            gameManager.hpManager.EnergyFluctuation(0.005f * 
            //スキル_エネルギータンク
            gameManager.skillManager.EnergyTank()
            ); //スキル_クライマーにも同様の定数で設定している
        }

    }

    private void OnTriggerEnter2D(Collider2D collision)
    {
        if(collision.tag == groundTag)
        {
            //gameManager.hpManager.EnergyFluctuation(0.75f);
            isGroundTrigger = true;
            //コンボリセット
            //gameManager.scoreManager.ResetCombo();
            
            if( playerArmState == PlayerArmState.pointingTheSpearDown) rb2D.velocity = default;
        }
        if (collision.tag == "monster")
        {

        }
    }
    private void OnTriggerStay2D(Collider2D collision)
    {
        if (collision.tag == groundTag)
        {
            isGroundTrigger = true;
            //コンボリセット
            //gameManager.scoreManager.ResetCombo();
        }
    }
    private void OnTriggerExit2D(Collider2D collision)
    {
        if (collision.tag == groundTag)
        {
            isGroundTrigger = false;
        }
    }
    #endregion
    #region//オブジェクト無効化し、見えなくする
    [System.Serializable] class DisableObjData
    {
        public List<SpriteRenderer> spriteRenderers;
        public GameObject playerColliderTag;
        public List<GameObject> delateObjs = new List<GameObject>();
        [System.NonSerialized] public bool onDisable;
    }
    [SerializeField] DisableObjData disableObjData;
    public void DisableSwitch(bool _switch)
    {
        if (_switch)
        {
            disableObjData.onDisable = true;
            disableObjData.playerColliderTag.tag = "Untagged";
            disableObjData.playerColliderTag.layer = LayerMask.NameToLayer("groundOnly"); 
            foreach(var renderer in disableObjData.spriteRenderers)
            {
                renderer.enabled = false;
            }
            foreach (var obj in disableObjData.delateObjs)
            {
                obj.SetActive(false);
            }
        }
        else
        {
            disableObjData.onDisable = false;
            disableObjData.playerColliderTag.tag = "Player";
            disableObjData.playerColliderTag.layer = LayerMask.NameToLayer("Default");
            foreach (var renderer in disableObjData.spriteRenderers)
            {
                renderer.enabled = true;
            }
            foreach (var obj in disableObjData.delateObjs)
            {
                obj.SetActive(true);
            }
        }
    }
    #endregion
    #region//槍が放物線を描く
    Vector3 latestPos; float paradolaDelay;
    [SerializeField] Transform stickmanArm;
    void ParabolaArrow()
    {
        if (isEntry) return;

        paradolaDelay += Time.deltaTime;
        if (paradolaDelay < 0.1f) return;

        Vector3 diff = transform.position - latestPos;   //前回からどこに進んだかをベクトルで取得
        latestPos = transform.position;  //前回のPositionの更新

        //ベクトルの大きさが0.01以上の時に向きを変える処理をする
        if (diff.magnitude > 0.01f)
        {
            Quaternion rotate = Quaternion.FromToRotation(Vector3.up, diff);
            stickmanArm.rotation = rotate;
            //スキル_トライデント
            gameManager.skillManager.TridentDegree = stickmanArm.eulerAngles.z + 90;
        }
    }
    #endregion
    #region//弓のアイドル状態
    //float recordArmRotate;
    void IdleBow()
    {
        if (isEntry) return;
        if (touchManager._touch_phase == TouchPhase.Moved) return;

        playerArmState = PlayerArmState.idleTheBow;
        float setRotate = default;
        if(stickmanArm.localScale.x > 0)
        {
            if (stickmanArm.rotation.eulerAngles.z > 87 && stickmanArm.rotation.eulerAngles.z < 93) return;
            setRotate = 90;
        }
        else
        {
            if (stickmanArm.rotation.eulerAngles.z > 267 && stickmanArm.rotation.eulerAngles.z < 273) return;
            setRotate = 270;
        }
        if (stickmanArm.rotation.eulerAngles.z > setRotate) stickmanArm.rotation = Quaternion.Euler(0, 0, stickmanArm.rotation.eulerAngles.z - 1);
        else stickmanArm.rotation = Quaternion.Euler(0, 0, stickmanArm.rotation.eulerAngles.z + 1);

        //recordArmRotate = stickmanArm.rotation.eulerAngles.z;
    }
    #endregion
    #region//弓を引くエフェクト
    [System.Serializable]
    class ArrowData
    {
        public LineRenderer arrowLineRenderer;
        public Transform arrowLineTrans;

        public Transform aroowPointerLeading;
        public float ArrowRange { get;} = 0.3f; //弓の発射位置
    }
    [SerializeField] ArrowData arrowData;
    void PullBow()
    {
        arrowData.arrowLineRenderer.SetPosition(1, arrowData.arrowLineTrans.localPosition);
    }
    #endregion
    #region//弓のクールダウン
    IEnumerator ArrowCooldown()
    {
        ++gameManager.canvasManager.SwipePermissionBoolInt;
        yield return new WaitForSeconds(0.3f * gameManager.skillManager.RapidFire()/*スキル_RapidFire*/);
        --gameManager.canvasManager.SwipePermissionBoolInt;
    }
    #endregion
    #region//槍の大きさ
    Vector3 recordSpearScale; 
    [SerializeField] Transform spearTrans;
    public void SpearLengh(float value)
    {
        spearTrans.localScale = recordSpearScale * value; 
    }
    #endregion
    #region//矢の発射位置
    [SerializeField]public Transform testpointer;
    Vector2 ReturnArrowPos(float setDeltaDegree = default)
    {
        float degree = default;
        if (setDeltaDegree == default) degree = stickmanArm.rotation.eulerAngles.z + 90;
        else degree = stickmanArm.rotation.eulerAngles.z + 90 + setDeltaDegree;

        Vector2 coordinate = DataBase.CoordinateUpperLimit(degree, arrowData.ArrowRange);
        return coordinate;
    }
    #endregion
    #region//移動量計算
    Vector3 recordPos; Vector2 deltaValue;
    void DeltaPos()
    {
        deltaValue = transform.position - recordPos;
        recordPos = transform.position;
    }
    #endregion
    #region//プレイヤーanimation
    enum PlayerBodyState
    {
        idle, run, upJump, landing
    }
    PlayerBodyState playerBodyState; PlayerBodyState recordPlayerBodyState;
    enum PlayerArmState
    {
        entry, holdTheSpear, pointingTheSpearDown, holdTheBow, idleTheBow
    }
    PlayerArmState playerArmState; PlayerArmState recordPlayerArmState;
    #region//スプライトの向き
    Vector2 animScale; [SerializeField] Transform stickmanBody;
    void PlayerDire()
    {
        if (isEntry) return;

        if (stickmanArm.rotation.eulerAngles.z < 1 && stickmanArm.rotation.eulerAngles.z > -1) return;

        if (stickmanArm.rotation.eulerAngles.z < 180) {
            stickmanBody.localScale = new Vector2(animScale.x, animScale.y);
            stickmanArm.localScale = new Vector3(1, 1, 1);
        }
        if (stickmanArm.rotation.eulerAngles.z > 180){
            stickmanBody.localScale = new Vector2(animScale.x * -1, animScale.y);
            stickmanArm.localScale = new Vector3(-1, 1, 1);
        }
    }
    #endregion
    #region//プレイヤーSpearArmTrans
    [SerializeField] Transform pointerTrans; [SerializeField] Transform armTrans;
    void SpearArmTrans()
    {
        if (touchManager._touch_phase == TouchPhase.Moved) return;

        armTrans.position = pointerTrans.position;

        float rotateZ = stickmanArm.rotation.eulerAngles.z;
        float motionTIme = default;
        if (rotateZ <= 180) motionTIme = rotateZ / 180;
        else motionTIme = 1 - ((rotateZ - 180) / 180);
        armAnim.SetFloat("motionTime", motionTIme);

        if (isEntry) return;
        if (touchManager._touch_flag) { playerArmState = PlayerArmState.holdTheSpear; return; }

        if (stickmanArm.rotation.eulerAngles.z < 182 && stickmanArm.rotation.eulerAngles.z > 178) playerArmState = PlayerArmState.pointingTheSpearDown;
        else playerArmState = PlayerArmState.holdTheSpear; 

        if (deltaValue == default && isGround[0] && playerArmState == PlayerArmState.pointingTheSpearDown) {
            playerArmState = PlayerArmState.holdTheSpear;
        }   
    }
    #region//プレイヤーエントリー
    [System.Serializable]
    public class EntryData
    {
        public AnimationCurve gravityCurve;
    }
    [SerializeField] EntryData entryData;
    bool isEntry;
    public IEnumerator EntryArmAnim()
    {
        isEntry = true;
        armAnim.CrossFadeInFixedTime("entry", 0.0f, layer: 0);
        playerArmState = PlayerArmState.entry;

        gameManager.effectManager.EffectPlay("hitEffect", transform.position, effectType: EffectType.particle);
        gameManager.effectManager.EffectPlay("hitEffect_2", transform.position, effectType: EffectType.particle);
        gameManager.soundManager.Play("collision_1", 0.9f, 0.1f);

        stickmanBody.localScale = new Vector2(animScale.x, animScale.y);
        stickmanArm.localScale = new Vector3(1, 1, 1);
        stickmanArm.rotation = Quaternion.Euler(new Vector3(0, 0, 90));
        StartCoroutine(EntryGravity());

        yield return new WaitForSeconds(1f);

        //armAnim.CrossFadeInFixedTime("holdTheSpaer", 0.5f, layer: 0);
        playerArmState = PlayerArmState.holdTheSpear; transtionDuration = 0.75f;
        stickmanArm.DORotate(new Vector3(0, 0, 0), 0.5f);

        yield return new WaitForSeconds(0.4f);

        isEntry = false;
    }
    IEnumerator EntryGravity()
    {
        rb2D.gravityScale = 0f;
        float time = 0;
        while (rb2D.gravityScale <= 1f)
        {
            rb2D.gravityScale += 1 * (entryData.gravityCurve.Evaluate(time) * 0.08f);
            time += 0.05f;
            yield return new WaitForSeconds(0.05f);
        }
        rb2D.gravityScale = 1f;
    }
    #endregion
    #endregion
    #region//アニメーター
    float groundTime;
    void Idle()
    {
        groundTime += Time.deltaTime;
        if(groundTime < 0.2f)
        {
            playerBodyState = PlayerBodyState.landing;
        }
        else if(Mathf.Abs(deltaValue.x) > 0.05f)
        {
            playerBodyState = PlayerBodyState.run;
        }
        else
        {
            playerBodyState = PlayerBodyState.idle;
        }
    }
    void UpJump()
    {
        groundTime = 0;
        playerBodyState = PlayerBodyState.upJump;
    }
    float transtionDuration;
    void StateCheck()
    {
        if (transtionDuration == default) transtionDuration = 0.1f;
        if (playerBodyState != recordPlayerBodyState)
        {
            string state_name = playerBodyState.ToString();
            anim.CrossFadeInFixedTime(state_name, transtionDuration, layer: 0);
        }
        recordPlayerBodyState = playerBodyState;

        if(playerArmState != recordPlayerArmState)
        {
            string state_name = playerArmState.ToString();
            if (!disableObjData.onDisable) armAnim.CrossFadeInFixedTime(state_name, transtionDuration, layer: 0);
        }
        recordPlayerArmState = playerArmState;
        transtionDuration = 0;
    }
    #endregion
    #region//衝突
    private void OnCollisionEnter2D(Collision2D collision)
    {
        //スキル_トライデント
        gameManager.skillManager.Trident(false, rb2D);
    }
    #endregion
    #endregion
}
