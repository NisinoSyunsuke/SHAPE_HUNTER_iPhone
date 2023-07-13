using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using DG.Tweening;
using UnityEngine.UI;
using SkillDataBase;
using LanguageData;
using UnityEngine.SceneManagement;
using WaveDataBase;
using StateManager;

public class CanvasManager : MonoBehaviour
{
    GameManager gameManager; LanguageManager languageManager;
    Camera _camera;
    [System.NonSerialized] public RectTransform canvasRect;

    public IEnumerator StartProcess()
    {
        gameManager = GameManager.gameManager;
        languageManager = gameManager.languageManager;
        _camera = Camera.main;
        canvasRect = GetComponent<RectTransform>();

        yield return null;

        RegisterSwipeUI();
    }
    #region//GUI
    [System.Serializable]
    public class GUIData
    {
        public List<GameObject> GUIList = new List<GameObject>();
    }
    public GUIData gUIData;
    #endregion

    #region//スワイプUI関連
    [System.Serializable] public class SwipeUIData
    {
        //public Animator swipeAnimator;
        [System.NonSerialized] public bool trigger; [System.NonSerialized] public float swipeLengh;
        [System.NonSerialized] public Tween swipePointerTransTween;[System.NonSerialized] public Coroutine swipeCoroutine;
        public RectTransform swipePointer;
        [System.NonSerialized] public Tween swipePointerImageTween; public Image swipePointerImage;
        public RectTransform swipeCircle;
        [System.NonSerialized] public Tween swipeCircleImageTween; public Image swipeCircleImage;
        [System.NonSerialized] public bool swipeLineRendererBool; public LineRenderer SwipeLineRenderer;
        [System.NonSerialized] public Vector2 defaultPointerPos;
        

        //public RectTransform swipeTouchUI;
        public Transform x0y0Pointer;
        public Transform x1y0Pointer;
        public Transform x0y1Pointer;
    }
    #region//スワイプ許可論理回路
    public int SwipePermissionBoolInt
    {
        get; set;
    }
    #endregion
    public SwipeUIData swipeUIData;
    void RegisterSwipeUI() //どの画面サイズでもタッチ画面を合わせる ※取り消されているのはOverlayの仕様
    {
        //float width = RectTransformUtility.WorldToScreenPoint(_camera, swipeUIData.x1y0Pointer.transform.position).x- RectTransformUtility.WorldToScreenPoint(_camera, swipeUIData.x0y0Pointer.transform.position).x;
        //float height = RectTransformUtility.WorldToScreenPoint(_camera, swipeUIData.x0y0Pointer.transform.position).y - RectTransformUtility.WorldToScreenPoint(_camera, swipeUIData.x0y1Pointer.transform.position).y;

        Vector2 viewportDeltaX = _camera.WorldToViewportPoint(swipeUIData.x1y0Pointer.transform.position) - _camera.WorldToViewportPoint(swipeUIData.x0y0Pointer.transform.position);
        //Vector2 viewportPositionY = _camera.WorldToViewportPoint(swipeUIData.x0y0Pointer.transform.position - swipeUIData.x0y1Pointer.transform.position);
        Vector2 viewportDeltaY = _camera.WorldToViewportPoint(swipeUIData.x0y0Pointer.transform.position) - _camera.WorldToViewportPoint(swipeUIData.x0y1Pointer.transform.position);
        Vector2 worldObjectScreenPosition = new Vector2(
            ((viewportDeltaX.x * canvasRect.sizeDelta.x)),
            ((viewportDeltaY.y * canvasRect.sizeDelta.y)));

        //swipeUIData.swipeTouchUI.sizeDelta = new Vector2(width, height) / transform.localScale;
        //swipeUIData.swipeTouchUI.sizeDelta = worldObjectScreenPosition;
    }
    public void SwipeStart(bool trigger)
    {
        if (gameManager.playerChip.CurrentWeponState == PlayerChip.WeponState.Spear && gameManager.hpManager.EnergyCheck()) return;
        swipeUIData.trigger = trigger;
        if (trigger)
        {
            if (SwipePermissionBoolInt > 0) { swipeUIData.trigger = false; return; }
            swipeUIData.swipePointerTransTween.Kill();
            swipeUIData.swipePointerImageTween.Kill();
            swipeUIData.swipeCircleImageTween.Kill();
            swipeUIData.swipePointerImage.DOFade(endValue: 0.6f, 0.1f); //swipeUIData.swipePointerImage.color = new Color(255, 255, 255, 100);
            swipeUIData.swipeCircleImage.DOFade(endValue: 0.2f, 0.1f);
            StartCoroutine(SwipeLineRenderer());
            if (swipeUIData.swipeCoroutine != null) StopCoroutine(swipeUIData.swipeCoroutine);
        }
        else
        {
            float speedTime = 0.75f - (0.5f * swipeUIData.swipeLengh);
            swipeUIData.swipePointerTransTween = swipeUIData.swipePointer.DOAnchorPos(swipeUIData.defaultPointerPos, speedTime).SetEase(Ease.OutBack);
            swipeUIData.swipeCoroutine = StartCoroutine(SwipeCoroutine(speedTime));
            if(gameManager.playerChip.CurrentWeponState == PlayerChip.WeponState.Spear)gameManager.hpManager.EnergyFluctuation(-swipeUIData.swipeLengh);
        }
    }
    public void PointerCoordinate(Vector2 coordinate, float lengh)
    {
        swipeUIData.swipeCircle.anchoredPosition = swipeUIData.defaultPointerPos;

        Vector2 worldObjectScreenPosition = new Vector2(
            ((coordinate.x * canvasRect.sizeDelta.x) - (canvasRect.sizeDelta.x * 0.5f)),
            ((coordinate.y * canvasRect.sizeDelta.y) - (canvasRect.sizeDelta.y * 0.5f)));

        swipeUIData.swipePointer.anchoredPosition = worldObjectScreenPosition;
        swipeUIData.swipeLengh = lengh;
        swipeUIData.SwipeLineRenderer.SetPosition(0, swipeUIData.defaultPointerPos);
        swipeUIData.SwipeLineRenderer.SetPosition(1, worldObjectScreenPosition);
        swipeUIData.SwipeLineRenderer.endWidth = lengh * 1f;

        //swipeUIData.swipeAnimator.SetFloat("state", lengh);
    }
    IEnumerator SwipeCoroutine(float speedTime)
    {
        swipeUIData.SwipeLineRenderer.endColor = new Color32(255, 255, 255, 0);
        swipeUIData.swipeLineRendererBool = true;

        yield return new WaitForSeconds(speedTime);
        //swipeUIData.swipeAnimator.SetFloat("state", 0);
        swipeUIData.swipePointerImageTween = swipeUIData.swipePointerImage.DOFade(endValue: 0f, 0.5f);
        swipeUIData.swipeCircleImageTween = swipeUIData.swipeCircleImage.DOFade(endValue: 0f, 0.5f);
    }

    IEnumerator SwipeLineRenderer()
    {
        swipeUIData.swipeLineRendererBool = false; 
        float fadeValue = 0;
        while (!swipeUIData.swipeLineRendererBool && fadeValue <= 0.5f)
        {
            fadeValue += 0.1f; 
            swipeUIData.SwipeLineRenderer.endColor = new Color(1, 1, 1, fadeValue);
            yield return new WaitForSeconds(0.02f);
        }
    }
    #endregion

    #region//エナジーゲージ
    [System.Serializable] public class EnergyUIData {
        public Slider energySlider;
        public Image fill;
        public GameObject energyGage;
        public Coroutine energyScaleCoroutine;
        public Tween energyScaleTween;
        public Coroutine energyScaleActiveCoroutine;
    }
    public EnergyUIData energyUIData;
    public void EnergyCounter(float energy, float maxEnergy)
    {
        energyUIData.energySlider.value = energy / maxEnergy;
    }
    public void EnergyBarScale(float scale, bool activeProcess = false)
    {
        if(!activeProcess)energyUIData.energyScaleCoroutine = StartCoroutine(EnergyBarScaleCoroutine(scale));
        //else energyUIData.energyScaleTween = energyUIData.energyGage.transform.DOScale(scale, 0.3f).SetEase(Ease.OutQuint);
    }
    IEnumerator EnergyBarScaleCoroutine(float scale)
    {
        float duration = 0.2f;
        energyUIData.energyScaleTween = energyUIData.energyGage.transform.DOScale(scale, duration).SetEase(Ease.OutQuint);
        yield return new WaitForSeconds(duration + 0.05f);
        energyUIData.energyScaleTween = energyUIData.energyGage.transform.DOScale(1f, duration);
    }
    #endregion

    #region//ヒットポイント関連
    [System.Serializable] class HPUIData { public Slider hpSlider; public Text hpText; public Text maxHpText; }
    [SerializeField] HPUIData hpUIData;
    public void HPCounter(int hp, int maxhp)
    {
        hpUIData.hpText.text = hp.ToString(); hpUIData.maxHpText.text = maxhp.ToString();
        hpUIData.hpSlider.value = (float)hp / (float)maxhp;
    }
    #endregion

    #region//武器変更UI
    [System.Serializable]
    public class WeponUI
    {
        //[Header("")]
        public GameObject Frame;
        public Sprite spear; public Sprite bow;
        public Image weponImage;
        public Transform backFrameTrans;
        [HideInInspector] public Vector2 recordFramePos;

    }
    public WeponUI weponUI = new WeponUI();
    bool isWeponChangeCoroutineRunning;
    public void WeponChangeButton()
    {
        if (isWeponChangeCoroutineRunning /*&& その他の条件を入れるスワイプ中など*/) return;
        StartCoroutine(WeponChangeCoroutine());
    }
    IEnumerator WeponChangeCoroutine()
    {
        isWeponChangeCoroutineRunning = true;
        ++SwipePermissionBoolInt;
        
        float delayTime = 0.5f;
        float changeSpeed = 0.65f * gameManager.skillManager.SpeedChange(); /*sスキル_SpeedChange*/

        /*weponUI.recordFramePos = weponUI.Frame.transform.position;
        weponUI.Frame.transform.DOMove(weponUI.recordFramePos, 0.4f * changeSpeed).SetDelay(0.4f * changeSpeed);
        weponUI.Frame.transform.DOMove(weponUI.backFrameTrans.position, 0.4f * changeSpeed);*/
        

        Sprite sprite = default;
        PlayerChip.WeponState weponState = default;
        //gameManager.soundManager.Play("whips_2", 1.25f, 0.2f);
        switch (gameManager.playerChip.CurrentWeponState)
        {
            case PlayerChip.WeponState.Bow:
                weponUI.Frame.transform.DORotate(new Vector3(0, 0, 90), delayTime * changeSpeed).SetEase(Ease.OutBack);
                sprite = weponUI.spear;
                weponState = PlayerChip.WeponState.Spear;
                EnergyBarScale(1f, activeProcess: true);
                break;
            case PlayerChip.WeponState.Spear:
                weponUI.Frame.transform.DORotate(new Vector3(0, 0, 0), delayTime * changeSpeed).SetEase(Ease.OutBack);
                sprite = weponUI.bow;
                weponState = PlayerChip.WeponState.Bow;
                EnergyBarScale(0f, activeProcess: true);
                break;
        }
        weponUI.weponImage.transform.DOScale(Vector3.zero, 0.2f * changeSpeed);

        yield return new WaitForSeconds(0.4f * changeSpeed);

        gameManager.soundManager.Play("wepon_1", 2.5f, 0.3f);
        weponUI.weponImage.transform.DOScale(Vector3.one, 0.2f * changeSpeed);
        weponUI.weponImage.sprite = sprite;
        gameManager.playerChip.ChangeWepon(weponState);

        --SwipePermissionBoolInt;
        isWeponChangeCoroutineRunning = false;
    }
    #endregion

    #region//スコア関連
    [System.Serializable] class ScoreUIData { public Text scoreText; }
    [SerializeField] ScoreUIData scoreUIData;
    public void ScoreCounter(int score)
    {
        scoreUIData.scoreText.text = "score　" + score;
    }
    #endregion

    #region//ウェーブ関連
    [System.Serializable] class WaveUIData { public Text waveCountText; }
    [SerializeField] WaveUIData waveData;
    public void WaveCounter(int waveCount)
    {
        waveData.waveCountText.text = waveCount.ToString();
    }
    #endregion

    #region//スキルUI関連
    [System.Serializable] public class SkillUIData { public Transform SkillUIZone; }
    public SkillUIData skillUIData;
    #endregion

    #region//操作不能関連
    [System.Serializable] class InoperableUIData { public GameObject inoperablePanel; }
    [SerializeField] InoperableUIData inoperableUIData;
    public void InoperableSwitch(bool _switch)
    {
        if (_switch)
        {
            inoperableUIData.inoperablePanel.SetActive(true);
        }
        else
        {
            inoperableUIData.inoperablePanel.SetActive(false);
        }
    }
    #endregion

    #region//ゲームの状態
    public enum GameState { playing, reward, gameover }
    GameState gameState;
    public void GameStateChange(GameState _gameState)
    {
        if (gameState == GameState.gameover) return;
        switch (_gameState)
        {
            case GameState.playing:
                break;
            case GameState.reward:
                RewardStart();
                break;
            case GameState.gameover:
                StartCoroutine(GameOverCorutine());
                break;
        }
        //Debug.Log(_gameState);
        gameState = _gameState;
    }
    #endregion

    #region//報酬プロセス
    [System.Serializable] public class RewardUIData
    {
        public GameObject RewardScreen;
        public List<SkilItemSelect> skilItemSelects = new List<SkilItemSelect>();
        [System.NonSerialized] public Image recordSkilItemSelectImage;
        [System.NonSerialized] public SkilItemSelect recordSkilItemSelect;
        [System.NonSerialized] public int skillItemSelectCount;
        [System.NonSerialized] public SkillEnum selectSkill;
        public GridLayoutGroup gridLayoutGroup;
        public Text nameText; public Text explanationText;
        public RewardUIData()
        {
            skillItemSelectCount = 3;
        }
        #region//スキル発動_マジシャン
        [System.Serializable] public class MagicianButton
        {
            public GameObject magicianButton;
            public Image skillImage;
            public Image selected;
        }
        public MagicianButton magicianButton;
        #endregion
    }
    public RewardUIData rewardUIData = new RewardUIData();
    //報酬画面スタート
    public void RewardStart()
    {
        rewardUIData.RewardScreen.SetActive(true);
        SelectSkill(SkillEnum.none);
        gameManager.skillManager.SkillSelection(rewardUIData.skillItemSelectCount);

        #region//スキル発動
        gameManager.skillManager.MagicianReset();
        #endregion
    }
    //スキル選択
    public void SelectSkill(SkillEnum skillEnum, SkilItemSelect skilItemSelect = default)
    {
        rewardUIData.selectSkill = skillEnum;
        //スキルの説明文章を追加
        if (skillEnum != SkillEnum.none) rewardUIData.nameText.text = languageManager.RetrunText(skillEnum.ToString(), TextGroup.skillNameText);
        else rewardUIData.nameText.text = "";
        rewardUIData.explanationText.text = languageManager.RetrunText(skillEnum.ToString(), TextGroup.skillExplaiontext);
        //セレクトエフェクト
        if(rewardUIData.recordSkilItemSelect != null) rewardUIData.recordSkilItemSelect.Selected = false;
        if (skilItemSelect != default)
        {
            skilItemSelect.Selected = true;
            rewardUIData.recordSkilItemSelect = skilItemSelect;
            gameManager.soundManager.Play("select_1", 1f, 0.1f);
        }
    }
    //報酬画面終了
    public void RewardEnd()
    {
        if (rewardUIData.selectSkill == SkillEnum.none) { return; }
        else { gameManager.skillManager.SkillItemGet(rewardUIData.selectSkill); }
        rewardUIData.RewardScreen.SetActive(false);
        gameManager.waveManager.WaveStart();
        gameManager.soundManager.Play("select_2", 1f, 0.1f);
        GameStateChange(GameState.playing);
    }
    #region//スキル発動
    //スキルアイテム数変更
    //ギャンブラー
    public void IncreaseSkillItemSelectCount(int value = 1)
    {
        rewardUIData.skillItemSelectCount += value;
        switch (rewardUIData.skillItemSelectCount)
        {
            case 3: rewardUIData.gridLayoutGroup.spacing = new Vector2(125f, 0); break;
            case 4: rewardUIData.gridLayoutGroup.spacing = new Vector2(60f, 0); rewardUIData.skilItemSelects[4 - 1].gameObject.SetActive(true); break;
            case 5: rewardUIData.gridLayoutGroup.spacing = new Vector2(37.5f, 0); rewardUIData.skilItemSelects[5 - 1].gameObject.SetActive(true); break;
                //5が上限
                //3の場合幅125 4の場合幅60　5の場合幅37.5
        }
    }
    //マジシャン
    public void Magician()
    {
        if (!gameManager.skillManager.Magician()) return;
        gameManager.skillManager.SkillSelection(rewardUIData.skillItemSelectCount);
        gameManager.soundManager.Play("select_3", 1f, 0.1f);
        SelectSkill(SkillEnum.none);
    }
    #endregion
    #endregion

    #region//ゲームオーバープロセス
    [System.Serializable]
    public class GameOverUIData
    {
        [Header("ゲームオーバー")]
        public GameObject GameOverScreen;
        public List<Text> MonsterClassValue = new List<Text>();
        public Text WaveValue;
        public Text BonusValue;
        public Text ScoreValue;
        public GameObject ScoreText;

        [Header("ランキング登録")]
        public GameObject rankingRegisterStrikethrough;
        public GameObject RankingRegisterScreen;
        public Text playerNameDisplayText;
        public InputField playerNameDisplayInputField;
        public Text errorTextDisplay;
        public Text rankingRegisterText;
        public Text highScoreText;

        public float resultSkipFloat = 1f;
        public bool resultSkipBool = false;
    }
    public GameOverUIData gameOverUIData = new GameOverUIData();
    public class GameScoreData
    {
        public List<int> monsterClassValues = new List<int>();
        public int waveValue;
        public int bonusValue;
        public int scoreVlue;
    } 
    IEnumerator GameOverCorutine()
    {

        yield return new WaitForSeconds(1f);
        gameOverUIData.resultSkipBool = true;
        gameOverUIData.GameOverScreen.SetActive(true);
        RankingButtonActiveProcess(false);
        //音楽STOP
        //gameManager.soundManager.BGMStop();
        gameManager.soundManager.BgmPlay(0.1f, 0.75f);

        yield return new WaitForSeconds(0.25f);
        GameScoreData gameScoreData = gameManager.scoreManager.FinalScoreRegister(gameManager.waveManager.WaveCount());
        
        //モンスタースコア
        for (int count = 0; count < gameOverUIData.MonsterClassValue.Count; ++count)
        {
            yield return new WaitForSeconds(0.5f * gameOverUIData.resultSkipFloat);
            gameOverUIData.MonsterClassValue[count].text = gameScoreData.monsterClassValues[count].ToString();
        }

        //WAVEスコア
        yield return new WaitForSeconds(0.5f * gameOverUIData.resultSkipFloat);
        gameOverUIData.WaveValue.text = gameScoreData.waveValue.ToString();

        //ボーナススコア
        yield return new WaitForSeconds(0.5f * gameOverUIData.resultSkipFloat);
        gameOverUIData.BonusValue.text = gameScoreData.bonusValue.ToString();

        //最終スコア
        yield return new WaitForSeconds(0.75f * gameOverUIData.resultSkipFloat);
        gameOverUIData.ScoreValue.text = gameScoreData.scoreVlue.ToString();
        gameOverUIData.ScoreText.SetActive(true);

        //ハイスコア更新
        if(DataBase.GetThisSeasonHighScore() < gameScoreData.scoreVlue)
        {
            //ハイスコアランキング登録するボタンを押せるようにする処理
            RankingButtonActiveProcess(true);
            //ハイスコア仮登録
            preScoreValue = gameScoreData.scoreVlue;
        }

        gameManager.canvasManager.InoperableSwitch(false);
    }
    //タッチでリザルトスキップ
    public void ResultSkip()
    {
        if (!gameOverUIData.resultSkipBool) return;
        TouchManager touchManager = new TouchManager();
        // タッチ状態更新
        touchManager.UpdateProcess();
        // タッチ取得
        TouchManager touchState = touchManager.GetTouch();
        if (!touchState._touch_flag) return;
        switch (touchState._touch_phase)
        {
            case TouchPhase.Began:
                gameOverUIData.resultSkipFloat = 0.1f;
                break;
        }
    }

    #region//タイトルに戻る
    public void BackTitleStart()
    {
        StartCoroutine(BackTitleStartCroutine());
    }
    #endregion
    #region//リトライ
    public void RetryStart()
    {
        DataBase.onRetry = true;
        StartCoroutine(BackTitleStartCroutine());
    }
    IEnumerator BackTitleStartCroutine()
    {
        ButtonPushSound();
        InoperableSwitch(true);  
        gameManager.titleManager.TransitionUI.DOFade(1, 1);

        yield return new WaitForSeconds(1f);

        gameManager.googleAdMobManager.InterStitialStart();
        yield return new WaitUntil(() => !gameManager.googleAdMobManager.InterstitialShowing);

        SceneManager.LoadScene(SceneManager.GetActiveScene().name);
    }
    #endregion

    #region//ランキング
    public void RankingRegisterStart()
    {
        ButtonPushSound();
        gameOverUIData.RankingRegisterScreen.SetActive(true);
        gameOverUIData.GameOverScreen.SetActive(false);
    }
    
    public void RankingRegisterEnd()
    {
        ButtonPushSound();
        nowRankingRegiCoroutine = false;
        RegisterTextDisplay("", Color.white);
        gameOverUIData.RankingRegisterScreen.SetActive(false);
        gameOverUIData.GameOverScreen.SetActive(true);
    }
    #region//ランキング登録
    public void RankingRegister()
    {
        if (nowRankingRegiCoroutine) return;
        ButtonPushSound();
        rankingRegisterCoroutine = StartCoroutine(RankingRegisterCoroutine());
    }

    Coroutine rankingRegisterCoroutine;
    Coroutine rankingGetTimeOut;
    bool nowRankingRegiCoroutine;
    int preScoreValue;
    IEnumerator RankingRegisterCoroutine()
    {
        InoperableSwitch(true); nowRankingRegiCoroutine = true;

        //タイムアウト処理
        Coroutine rankingGetTimeOut = StartCoroutine(RankingRegiCancel(languageManager.RetrunText("Timed out", TextGroup.uiText), 15f));

        yield return StartCoroutine(gameManager.rankingManager.RankingRegister(gameOverUIData.playerNameDisplayText.text, preScoreValue));

        //タイムアウト処理
        StopCoroutine(rankingGetTimeOut);

        RegisterTextDisplay(languageManager.RetrunText("Ranking registration completed", TextGroup.uiText), Color.white);

        //ハイスコアをセーブする処理
        DataBase.SetThisSeasonHighScore(preScoreValue);
        gameManager.saveDataManager.saveData.PlayerHighScoreSeson2 = DataBase.GetThisSeasonHighScore();
        gameManager.saveDataManager.Save();

        yield return new WaitForSeconds(3f);

        RankingButtonActiveProcess(false);
        RankingRegisterEnd();

        InoperableSwitch(false);
    }
    bool nowRankingRegiCanceling;
    public IEnumerator RankingRegiCancel(string cancelReasonText, float timeOutTime)
    {
        nowRankingRegiCanceling = true;
        yield return new WaitForSeconds(timeOutTime);

        if (!nowRankingRegiCanceling) yield break;
        nowRankingRegiCanceling = false;

        StopCoroutine(rankingRegisterCoroutine);
        rankingRegisterCoroutine = default;

        //通信エラーみたいな警告文を出す
        RegisterTextDisplay(cancelReasonText, Color.red);

        yield return new WaitForSeconds(3f);

        InoperableSwitch(false);
        RankingRegisterEnd();
    }

    void RegisterTextDisplay(string text, Color color)
    {
        //languagemanagerを通す
        gameOverUIData.errorTextDisplay.color = color;
        gameOverUIData.errorTextDisplay.text = text;
    }
    //ランキングボタンアクティブ処理
    void RankingButtonActiveProcess(bool active)
    {
        if (active)
        {
            gameOverUIData.rankingRegisterStrikethrough.SetActive(false);
            gameOverUIData.highScoreText.gameObject.SetActive(true);
            gameOverUIData.rankingRegisterText.color = Color.white;
        }
        else
        {
            gameOverUIData.rankingRegisterStrikethrough.SetActive(true);
            gameOverUIData.highScoreText.gameObject.SetActive(false);
            gameOverUIData.rankingRegisterText.color = Color.grey;
        }
    }
    #endregion
    
    #region//ユーザID登録
    public void StringInput()
    {
        gameOverUIData.playerNameDisplayText.text = gameOverUIData.playerNameDisplayInputField.text;
    }
    #endregion
    #region//効果音
    public void ButtonPushSound()
    {
        gameManager.soundManager.Play("select_1", 0.7f, 0.05f);
    }
    #endregion
    #endregion
    #endregion
}
