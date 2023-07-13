using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using HPDataBase;
using EffectData;
using DG.Tweening;

public class HPManager : MonoBehaviour
{
    GameManager gameManager;

    public int maxHp; int hp;

    public void StartProcess()
    {
        gameManager = GameManager.gameManager;

        hp = maxHp;
        gameManager.canvasManager.HPCounter(hp, maxHp);

        energy = maxEnergy; initialMaxEnergy = maxEnergy;
        gameManager.canvasManager.EnergyCounter(energy, maxEnergy);

        invincibleImage = gameManager.player.transform.Find("SpriteZone/InvincibleImage").gameObject;
    }
    #region//HP増減
    public void HPFluctuation(int value, Vector2 pos, TakeDamageType takeDamageType)
    {
        //無敵時間
        if (value <= 0 && isInvicible) {
            gameManager.effectManager.TextEffectPlay("ZERO", pos, /*effectTextType: EffectTextType.floatUp, */ activeOutline: true, multiple: 0.75f);
            gameManager.soundManager.Play("sword_2", 2f, 0.1f);
            return;
        }

        hp += value;

        if(value <= 0)
        {
            gameManager.effectManager.EffectPlay("damageEffect", pos, effectType: EffectType.particle);
            gameManager.effectManager.TextEffectPlay(value.ToString(), pos, color: Color.red, activeOutline: true);
            gameManager.soundManager.Play("hit_1", 1f, 0.1f);
            if (takeDamageType == TakeDamageType.normal)
            {
                //コンボリセット //継続ダメージではリセットされないように
                gameManager.scoreManager.ResetCombo();
            }
        }
        else
        {
            if (hp >= maxHp) hp = maxHp;
            if(takeDamageType == TakeDamageType.recovery)
            {
                if (pos == default){
                    pos = gameManager.player.transform.position;
                    pos = new Vector2(pos.x + Random.Range(-0.3f, 0.3f), pos.y + Random.Range(-0.3f, 0.3f));
                }
                gameManager.effectManager.TextEffectPlay("+" + value.ToString(), pos, color: Color.green, effectTextType: EffectTextType.floatUp, activeOutline: true);
                gameManager.soundManager.Play("recovery_1", 1f, 0.1f);
            }
        }

        if(hp <= 0)
        {
            hp = 0;
            //死亡処理
            gameManager.HorsDeCombat = true;
            gameManager.playerChip.DisableSwitch(true);
            gameManager.canvasManager.InoperableSwitch(true);
            gameManager.effectManager.EffectPlay("breakEffect", gameManager.player.transform.position, effectType: EffectType.particle);
            #region//スキル発動
            if (gameManager.skillManager.Resurrection()) {
                gameManager.canvasManager.HPCounter(hp, maxHp);
                return; }
            //ゲームオーバー
            gameManager.canvasManager.GameStateChange(CanvasManager.GameState.gameover);
            #endregion
        }

        gameManager.canvasManager.HPCounter(hp, maxHp);
    }
    #endregion

    #region//最大HP更新
    public void MaxHPFluctuation(int value)
    {
        //MAXHP上限時処理
        if (maxHp - 99 == 0) {
            Vector2 pos1 = gameManager.player.transform.position;
            pos1 = new Vector2(pos1.x + Random.Range(-0.5f, 0.5f), pos1.y + Random.Range(-0.5f, 0.5f));
            gameManager.effectManager.TextEffectPlay("MAX HP LIMIT", pos1, effectTextType: EffectTextType.floatUp, activeOutline: true);
            return;
        }
        //
        maxHp += value;

        //MAXHP上限時処理
        if (maxHp >= 99) maxHp = 99;
        //
        gameManager.canvasManager.HPCounter(hp, maxHp);

        Vector2 pos = gameManager.player.transform.position;
        pos = new Vector2(pos.x + Random.Range(-0.5f, 0.5f), pos.y + Random.Range(-0.5f, 0.5f));
        gameManager.effectManager.TextEffectPlay("MAX HP +" + value.ToString(), pos, effectTextType: EffectTextType.floatUp, activeOutline: true);
    }
    #endregion

    #region//エナジーゲージ
    [SerializeField] float maxEnergy; float energy;
    [System.NonSerialized] public float initialMaxEnergy; //Eの初期値
    public void EnergyFluctuation(float value)
    {
        energy += value;

        if (value < 0)
        {
            if (value < -0.1f) gameManager.canvasManager.EnergyBarScale(0.9f);
        }
        else
        {
            if (value > 0.1f) gameManager.canvasManager.EnergyBarScale(1.1f);
        }

        if(energy <= 0)
        {
            energy = 0;
        }
        else if (energy >= maxEnergy)
        {
            energy = maxEnergy;
        }

        gameManager.canvasManager.EnergyCounter(energy, maxEnergy);

        if (energy >= 0.25f) {
            gameManager.canvasManager.energyUIData.fill.color = Color.blue;
        }
        else
        {
            gameManager.canvasManager.energyUIData.fill.color = new Color(0.5f, 0.5f, 1f, 0.25f);
        }
    }

    public bool EnergyCheck()
    {
        bool check = energy <= 0.25f;
        if (check)
        {
            //energyなしの時の処理
            gameManager.soundManager.Play("select_5", 0.75f, 0.05f);
            gameManager.canvasManager.EnergyBarScale(0.95f);
        }
        return check;
    }
    #endregion
    #region//最大E更新
    public void MaxEnergyFluctuation(float value)
    {
        maxEnergy += value;
        gameManager.canvasManager.EnergyCounter(energy, maxEnergy);
    }
    #endregion
    #region//無敵時間
    bool isInvicible = false;
    GameObject invincibleImage;
    Coroutine inInvicibleTime;
    public void InvincibleTime(float time, GameObject image = default)
    {
        image = invincibleImage;

        StartCoroutine(InvincibleTimeCoroutine(time, image));

        if(inInvicibleTime != default)StopCoroutine(inInvicibleTime);
        inInvicibleTime = StartCoroutine(InvincibleImageEffect(image));
    }
    IEnumerator InvincibleTimeCoroutine(float time, GameObject image)
    {
        isInvicible = true;
        image.SetActive(true);

        yield return new WaitForSeconds(time);

        isInvicible = false;
        invincibleImage.SetActive(false);
    }
    IEnumerator InvincibleImageEffect(GameObject image)
    {
        while (true)
        {
            image.transform.DOScale(new Vector3(2.00f, 2.00f, 2.00f), 0.5f);
            yield return new WaitForSeconds(0.5f);
            image.transform.DOScale(new Vector3(1.75f, 1.75f, 1.75f), 0.5f);
            yield return new WaitForSeconds(0.5f);
            if (!isInvicible) yield break;
        }
    }
    #endregion
}
