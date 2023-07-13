using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using SkillDataBase;
using HPDataBase;
using EffectData;
using DG.Tweening;
using WaveDataBase;
using LanguageData;

public class SkillManager : MonoBehaviour
{
    GameManager gameManager;

    [SerializeField] List<SkillData> skillDatas = new List<SkillData>();
    public Dictionary<SkillEnum, SkillData> skillDataDic = new Dictionary<SkillEnum, SkillData>();
    public Dictionary<SkillRarity, List<SkillEnum>> skillRarityDic = new Dictionary<SkillRarity, List<SkillEnum>>();
    public Dictionary<SkillRarity, Color> rarityColorDic = new Dictionary<SkillRarity, Color>();
    #region//スタート処理
    public void StartProcess()
    {
        gameManager = GameManager.gameManager;

        //レアリティリスト
        List<SkillEnum> commonList = new List<SkillEnum>();
        List<SkillEnum> rareList = new List<SkillEnum>();
        List<SkillEnum> epicList = new List<SkillEnum>();
        List<SkillEnum> legendaryList = new List<SkillEnum>();
        //Data登録
        foreach (var data in skillDatas)
        {
            skillDataDic.Add(data.skillEnum, data);
            switch (data.skillRarity)
            {
                case SkillRarity.common: commonList.Add(data.skillEnum); break;
                case SkillRarity.rare: rareList.Add(data.skillEnum); break;
                case SkillRarity.epic: epicList.Add(data.skillEnum); break;
                case SkillRarity.legendary: legendaryList.Add(data.skillEnum); break;
            }
        }
        //レアリティDic登録
        skillRarityDic.Add(SkillRarity.common, commonList);
        skillRarityDic.Add(SkillRarity.rare, rareList);
        skillRarityDic.Add(SkillRarity.epic, epicList);
        skillRarityDic.Add(SkillRarity.legendary, legendaryList);
          //Debug.Log("common:" + commonList.Count + " rare:" + rareList.Count + " epic:" + epicList.Count + " legendary:" + legendaryList.Count);
        //レアリティ色登録
        rarityColorDic.Add(SkillRarity.common, Color.white);
        ColorUtility.TryParseHtmlString("#8A80F1", out Color rareColor); rarityColorDic.Add(SkillRarity.rare, rareColor);
        ColorUtility.TryParseHtmlString("#7229E0", out Color epicColor); rarityColorDic.Add(SkillRarity.epic, epicColor);
        ColorUtility.TryParseHtmlString("#DD9D27", out Color legendaryColor); rarityColorDic.Add(SkillRarity.legendary, legendaryColor);
    }
    #endregion
    #region//スキルアイテム追加(報酬)
    public WaveRarityRate waveRarityRate { set; protected get; }
    public void SkillSelection(int loopCount)
    {
        Dictionary<SkillRarity, List<SkillEnum>> skillRarityDicData = new Dictionary<SkillRarity, List<SkillEnum>>(); //コンストラクタを使った方法で「参照渡し」ではなく「値渡し」にしている
        skillRarityDicData.Add(SkillRarity.common, new List<SkillEnum>(skillRarityDic[SkillRarity.common]));
        skillRarityDicData.Add(SkillRarity.rare, new List<SkillEnum>(skillRarityDic[SkillRarity.rare]));
        skillRarityDicData.Add(SkillRarity.epic, new List<SkillEnum>(skillRarityDic[SkillRarity.epic]));
        skillRarityDicData.Add(SkillRarity.legendary, new List<SkillEnum>(skillRarityDic[SkillRarity.legendary]));
        //確率定義
        float rareRate = default; float epicRate = default; float legendaryRate = default;
        switch (waveRarityRate)
        {
            case WaveRarityRate.commonRate:
                rareRate = 0.2f; epicRate = 0.075f; legendaryRate = 0.025f;
                break;
            case WaveRarityRate.rareRate:
                rareRate = 1f; epicRate = 0.2f; legendaryRate = 0.075f;
                break;
            case WaveRarityRate.epicRate:
                rareRate = 1f; epicRate = 1f; legendaryRate = 0.2f;
                break;
        }

        for (int number = 0; number < loopCount; ++ number)
        {
            //確率選択
            SkillRarity selectRarity = SkillRarity.common;
            if (rareRate > Random.value) selectRarity = SkillRarity.rare;
            if (epicRate > Random.value) selectRarity = SkillRarity.epic;
            if (legendaryRate > Random.value) selectRarity = SkillRarity.legendary;
            //スキルアイテム選択
              //Debug.Log(selectRarity+":"+skillRarityDicData[selectRarity].Count);
            SkillEnum selectSkill = skillRarityDicData[selectRarity][Random.Range(0, skillRarityDicData[selectRarity].Count)];
            //continueで選択したアイテムが上限だった場合リロード
            if(skillDataDic[selectSkill].upperLimit!= 0 && skillDataDic[selectSkill].count >= skillDataDic[selectSkill].upperLimit)
            {
                Debug.Log("上限値に達した:" + selectSkill); --number; continue;
            }
            //スキルアイテム登録
            gameManager.canvasManager.rewardUIData.skilItemSelects[number].SkillRegister(selectSkill);
            skillRarityDicData[selectRarity].Remove(selectSkill);
        }
    }
    #endregion
    #region//スキルアイテム追加(ドロップ)
    public void SkillDrop(SkillRarity skillRarity, Vector2 pos)
    {
        Dictionary<SkillRarity, List<SkillEnum>> skillRarityDicData = new Dictionary<SkillRarity, List<SkillEnum>>(); //コンストラクタを使った方法で「参照渡し」ではなく「値渡し」にしている
        skillRarityDicData.Add(SkillRarity.common, new List<SkillEnum>(skillRarityDic[SkillRarity.common]));
        skillRarityDicData.Add(SkillRarity.rare, new List<SkillEnum>(skillRarityDic[SkillRarity.rare]));
        skillRarityDicData.Add(SkillRarity.epic, new List<SkillEnum>(skillRarityDic[SkillRarity.epic]));
        skillRarityDicData.Add(SkillRarity.legendary, new List<SkillEnum>(skillRarityDic[SkillRarity.legendary]));

        for (int number = 0; number < 1; ++number)
        {
            SkillEnum dropSkill = skillRarityDicData[skillRarity][Random.Range(0, skillRarityDicData[skillRarity].Count)];
            //continueで選択したアイテムが上限だった場合リロード
            if (skillDataDic[dropSkill].upperLimit != 0 && skillDataDic[dropSkill].count >= skillDataDic[dropSkill].upperLimit)
            {
                Debug.Log("上限値に達した:" + dropSkill); --number; continue;
            }

            SkillItemGet(dropSkill);
            gameManager.effectManager.TextEffectPlay("SKILL DROP \n" + gameManager.languageManager.RetrunText(dropSkill.ToString(), TextGroup.skillNameText),
                pos, effectTextType: EffectTextType.floatUp, activeOutline: true, multiple: 0.75f);
            gameManager.soundManager.Play("coin_1", 1.2f, 0.15f);
        }
    }
    #endregion
    #region//スキルアイテムゲット
    [SerializeField] SkillItemUI skillItemUI;
    public void SkillItemGet(SkillEnum skillEnum)
    {
        switch (skillDataDic[skillEnum].type)
        {
            case SkillItemType.item:
                #region//消費型の処理
                switch (skillEnum)
                {
                    case SkillEnum.GreenApple:
                    case SkillEnum.Apple:
                        Apple(skillEnum);
                        break;
                    case SkillEnum.Potion:
                    case SkillEnum.HighPotion:
                        Potion(skillEnum);
                        break;
                }
                #endregion
                break;
            case SkillItemType.skill:
                if(skillDataDic[skillEnum].count == 0)
                {
                    var _skillItemUI = Instantiate(skillItemUI, new Vector2(0, 0), Quaternion.identity, gameManager.canvasManager.skillUIData.SkillUIZone);
                    skillDataDic[skillEnum].skillItemUI = _skillItemUI;
                }
                skillDataDic[skillEnum].count += 1;
                skillDataDic[skillEnum].skillItemUI.SkillRegister(skillEnum);
                #region//取得時に適用
                switch (skillEnum)
                {
                    case SkillEnum.LowGravity: LowGravity(gameManager.playerChip.rb2D); break;
                    case SkillEnum.AthleteLv1:
                    case SkillEnum.AthleteLv2:
                    case SkillEnum.AthleteLv3:
                        AtthleteLaunch(skillEnum); break;
                    case SkillEnum.IronArmorLv1:
                    case SkillEnum.IronArmorLv2:
                    case SkillEnum.IronArmorLv3:
                        IronArmor(skillEnum); break;
                    case SkillEnum.LifeFragmentLv1:
                    case SkillEnum.LifeFragmentLv2:
                        LifeFragment(skillEnum); break;
                    case SkillEnum.InotiShizukuLv1:
                    case SkillEnum.InotiShizukuLv2:
                        InotiShizuku(skillEnum); break;
                    case SkillEnum.EnergyTankLv1:
                    case SkillEnum.EnergyTankLv2:
                    case SkillEnum.EnergyTankLv3:
                        EnergyTank(skillEnum); break;
                    case SkillEnum.Gambler: Gambler(); break;
                    case SkillEnum.Magician: MagicianLaunch(); break;
                    case SkillEnum.DoubleEdgedSword: DoubleEdgedSwordLaunch(); break;
                    case SkillEnum.GiantLv1:
                    case SkillEnum.GiantLv2:
                    case SkillEnum.GiantLv3: Giant1(); break;
                    case SkillEnum.Shieid: Shield(); break;
                    case SkillEnum.AegisShieid: AegisShield(); break;
                    case SkillEnum.HollyArrow:
                        gameManager.playerChip.ChangeWepon(SkillEnum.HollyArrow);
                        break;
                    case SkillEnum.Trident:
                        gameManager.playerChip.ChangeWepon(SkillEnum.Trident);
                        break;
                    case SkillEnum.FriendOfLight:
                        FriendOfLight();
                        break;
                }
                #endregion
                //非接触攻撃更新
                //gameManager.damageManager.TriggerDamageRegister();
                break;
        }
    }
    #endregion
    #region//スキルアイテムロスト
    public void SkillItemLost(SkillEnum skillEnum)
    {
        if (skillDataDic[skillEnum].count == 0) return; //そもそも持っていないならリターン
        
        skillDataDic[skillEnum].count -= 1;
        skillDataDic[skillEnum].skillItemUI.SkillRegister(skillEnum); //個数を再登録
        if (skillDataDic[skillEnum].count <= 0) //個数がゼロならデストロイ
        {
            Destroy(skillDataDic[skillEnum].skillItemUI.gameObject);
        }
    }
    #endregion
    #region//アップデート処理
    void FixedUpdate()
    {
        //スキル_シールド
        UpdateSheild();
        //スキル_イージスシールド
        AegisShieldRotate();
    }
    #endregion

    #region//スキル

    #region//ゼログラビティ
    public void ZeroGravity(Rigidbody2D rb2D)
    {
        if (skillDataDic[SkillEnum.ZeroGravity].count == 0) return;
        rb2D.velocity = new Vector2(0, 0);
    }
    #endregion
    #region//ローグラビティ
    public void LowGravity(Rigidbody2D rb2D)
    {
        if (skillDataDic[SkillEnum.LowGravity].count == 0) return;
        rb2D.gravityScale = 1 - (skillDataDic[SkillEnum.LowGravity].count * 0.25f);
    }
    #endregion
    #region//小型ナイフ
    public int SmallKnife()
    {
        if (skillDataDic[SkillEnum.SmallKnifeLv1].count == 0 
            && skillDataDic[SkillEnum.SmallKnifeLv2].count == 0
            && skillDataDic[SkillEnum.SmallKnifeLv3].count == 0) return 0;

        int value = 0;
        value += skillDataDic[SkillEnum.SmallKnifeLv1].count;
        value += skillDataDic[SkillEnum.SmallKnifeLv2].count * 2;
        value += skillDataDic[SkillEnum.SmallKnifeLv3].count * 3;

        return value;
    }
    #endregion
    #region//アスリート
    public float Atthlete()
    {
        if (skillDataDic[SkillEnum.AthleteLv1].count == 0
            && skillDataDic[SkillEnum.AthleteLv2].count == 0
            && skillDataDic[SkillEnum.AthleteLv3].count == 0
            ) return 0;

        float value = 0;
        value += skillDataDic[SkillEnum.AthleteLv1].count * 0.05f;
        value += skillDataDic[SkillEnum.AthleteLv2].count * 0.08f;
        value += skillDataDic[SkillEnum.AthleteLv3].count * 0.15f;

        return value;
    }
    public void AtthleteLaunch(SkillEnum skillEnum)//起動時
    {
        switch (skillEnum)
        {
            case SkillEnum.AthleteLv1: gameManager.hpManager.MaxHPFluctuation(2); break;
            case SkillEnum.AthleteLv2: gameManager.hpManager.MaxHPFluctuation(5); break;
            case SkillEnum.AthleteLv3: gameManager.hpManager.MaxHPFluctuation(10); break;
        }
    }
    #endregion
    #region//雷
    public void Thunder(Vector2 pos)
    {
        if (skillDataDic[SkillEnum.Thunder].count == 0) return;
        float multiple = 0.3f + (0.5f * skillDataDic[SkillEnum.Thunder].count);
        int damageValue = (int)(gameManager.damageManager.DamageFinalCalculation(gameManager.damageManager.atk, DealDamageType.nonContact, multiple:multiple));

        gameManager.effectManager.AttackEffectPlay("thunder", new Vector2(pos.x, pos.y + 2.5f), damageValue, rotate: -90);
        gameManager.soundManager.Play("thunder_1", 1f, 0.1f);
    }
    #endregion
    #region//ボマー
    public void Bomber(Vector2 pos)
    {
        if (skillDataDic[SkillEnum.Bomber].count == 0) return;
        float multiple = 1 + (0.25f * skillDataDic[SkillEnum.Bomber].count);
        int damageValue = (int)(gameManager.damageManager.DamageFinalCalculation(gameManager.damageManager.atk, DealDamageType.nonContact, multiple: multiple));

        gameManager.effectManager.AttackEffectPlay("explosion", pos, damageValue, scale: 1.25f);
        gameManager.soundManager.Play("fire_1", 1.2f, 0.05f);
    }
    #endregion
    #region//ファインスパーク
    public void FineSpark(Vector2 pos)
    {
        if (skillDataDic[SkillEnum.FineSpark].count == 0) return;
        int count = skillDataDic[SkillEnum.FineSpark].count;

        for (int a = 0;a < count; ++a)
        {
            gameManager.effectManager.EffectPlay("sparkEffect", pos, effectType: EffectType.particle, particleType:ParticleType.shot, shotType: ShotType.explosion);
            gameManager.soundManager.Play("spark_1", 1f, 0.05f);
        }
    }
    public void FineSparkShot(Vector2 pos)
    {
        int damageValue = (int)(gameManager.damageManager.DamageFinalCalculation(gameManager.damageManager.atk, DealDamageType.nonContact, multiple: 0.5f));

        gameManager.effectManager.AttackEffectPlay("spark", pos, damageValue, scale: 1.5f);
        gameManager.soundManager.Play("thunder_1", 2f, 0.03f);
    }
    #endregion
    #region//鉄の鎧
    public void IronArmor(SkillEnum skillEnum)
    {
        int value;
        switch (skillEnum)
        {
            case SkillEnum.IronArmorLv1:
                value = 3;
                gameManager.hpManager.MaxHPFluctuation(value);
                gameManager.hpManager.HPFluctuation(value, default, TakeDamageType.recovery);
                break;
            case SkillEnum.IronArmorLv2:
                value = 5;
                gameManager.hpManager.MaxHPFluctuation(value);
                gameManager.hpManager.HPFluctuation(value, default, TakeDamageType.recovery);
                break;
            case SkillEnum.IronArmorLv3:
                value = 8;
                gameManager.hpManager.MaxHPFluctuation(value);
                gameManager.hpManager.HPFluctuation(value, default, TakeDamageType.recovery);
                break;
        }
    }
    #endregion
    #region//命の欠片
    public void LifeFragment(SkillEnum skillEnum)
    {
        int value;
        switch (skillEnum)
        {
            case SkillEnum.LifeFragmentLv1:
                value = (int)(gameManager.hpManager.maxHp * 0.15f);
                gameManager.hpManager.MaxHPFluctuation(value);
                gameManager.hpManager.HPFluctuation(value, default,TakeDamageType.recovery);
                break;
            case SkillEnum.LifeFragmentLv2:
                value = (int)(gameManager.hpManager.maxHp * 0.3f);
                gameManager.hpManager.MaxHPFluctuation(value);
                gameManager.hpManager.HPFluctuation(value, default, TakeDamageType.recovery);
                break;
        }
    }
    #endregion
    #region//命の雫
    public void InotiShizuku(SkillEnum skillEnum)
    {
        int value;
        switch (skillEnum)
        {
            case SkillEnum.InotiShizukuLv1:
                value = (int)(gameManager.hpManager.maxHp * 0.5);
                gameManager.hpManager.MaxHPFluctuation(value);
                value = (int)(gameManager.hpManager.maxHp * 0.25f);
                gameManager.hpManager.HPFluctuation(value, default, TakeDamageType.recovery);
                break;
            case SkillEnum.InotiShizukuLv2:
                value = (int)(gameManager.hpManager.maxHp * 0.75);
                gameManager.hpManager.MaxHPFluctuation(value);
                value = (int)(gameManager.hpManager.maxHp * 0.4f);
                gameManager.hpManager.HPFluctuation(value, default, TakeDamageType.recovery);
                break;
        }
    }
    #endregion
    #region//エネルギータンク
    public void EnergyTank(SkillEnum skillEnum)
    {
        float value;
        switch (skillEnum)
        {
            case SkillEnum.EnergyTankLv1:
                value = gameManager.hpManager.initialMaxEnergy * 0.2f;
                gameManager.hpManager.MaxEnergyFluctuation(value);
                break;
            case SkillEnum.EnergyTankLv2:
                value = gameManager.hpManager.initialMaxEnergy * 0.35f;
                gameManager.hpManager.MaxEnergyFluctuation(value);
                break;
            case SkillEnum.EnergyTankLv3:
                value = gameManager.hpManager.initialMaxEnergy * 0.75f;
                gameManager.hpManager.MaxEnergyFluctuation(value);
                break;
        }
    }
    public float EnergyTank()
    {
        if (skillDataDic[SkillEnum.EnergyTankLv1].count == 0
    && skillDataDic[SkillEnum.EnergyTankLv2].count == 0
    && skillDataDic[SkillEnum.EnergyTankLv3].count == 0) return 1;

        float value = 1;
        value += skillDataDic[SkillEnum.EnergyTankLv1].count * 0.2f;
        value += skillDataDic[SkillEnum.EnergyTankLv2].count * 0.35f;
        value += skillDataDic[SkillEnum.EnergyTankLv3].count * 0.75f;

        return value;
    }
    #endregion
    #region//リザレクション
    bool resurrectionBool;
    public bool Resurrection()
    {
        if (skillDataDic[SkillEnum.Resurrection].count == 0) return false;
        if (resurrectionBool) return true; //実行中のためtrueで返してreturn
        resurrectionBool = true;
        StartCoroutine(ResurrectionCorutine());
        return true;
    }
    IEnumerator ResurrectionCorutine()
    {
        Vector2 pos = new Vector2(0, 0);

        yield return new WaitForSeconds(2.5f);

        gameManager.effectManager.EffectPlay("wing", new Vector2(pos.x, pos.y + 0.5f), scale: 1.5f);
        gameManager.effectManager.EffectPlay("magicCircle_red", pos, scale: 0.75f);
        gameManager.soundManager.Play("magic_1", 1.5f, 0.1f);
        gameManager.soundManager.Play("summon_1", 1.25f, 0.1f);

        yield return new WaitForSeconds(0.5f);

        gameManager.hpManager.HPFluctuation((int)(gameManager.hpManager.maxHp * 0.5f), default, TakeDamageType.normal);
        gameManager.HorsDeCombat = false;
        gameManager.playerChip.DisableSwitch(false);
        gameManager.canvasManager.InoperableSwitch(false);
        gameManager.player.transform.position = pos;
        SkillItemLost(SkillEnum.Resurrection);

        gameManager.waveManager.MonsterCountFluctuation(value: 0); //リスポーン中にWAVEが終わってないか確認
        
        resurrectionBool = false;
    }
    #endregion
    #region//エンチャント炎
    List<Transform> enchantmentFireList = new List<Transform>();
    public void EnchantmentFire(Transform monsTrans)
    {
        if (skillDataDic[SkillEnum.EnchantmentFire].count == 0) return;
        foreach(var trans in enchantmentFireList)
        {
            if (monsTrans == trans) return;
        }
        enchantmentFireList.Add(monsTrans);

        float multiple = 0.25f * skillDataDic[SkillEnum.Thunder].count;
        int damageValue = (int)(gameManager.damageManager.DamageFinalCalculation(gameManager.damageManager.atk, DealDamageType.nonContact, multiple: multiple));
        if (damageValue <= 0) damageValue = 1;

        StartCoroutine(EnchantmentFireCoroutine(monsTrans,damageValue));
    }
    IEnumerator EnchantmentFireCoroutine(Transform monsTrans, int damageValue)
    {
        int count = 7;
        while (count > 0)
        {
            if (monsTrans == null) break;
            gameManager.effectManager.AttackEffectPlay("fire", monsTrans.position, damageValue, scale: 0.75f);
            gameManager.soundManager.Play("burn_1", 2f, 0.1f);
            --count;
            yield return new WaitForSeconds(0.5f);
        }
        enchantmentFireList.Remove(monsTrans);
    }
    #endregion
    #region//ハイスピード
    public float HighSpeed()
    {
        if (skillDataDic[SkillEnum.HighSpeedLv1].count == 0
            && skillDataDic[SkillEnum.HighSpeedLv2].count == 0
            && skillDataDic[SkillEnum.HighSpeedLv3].count == 0
            ) return 1;

        float value = 1;
        value += skillDataDic[SkillEnum.HighSpeedLv1].count * 0.05f;
        value += skillDataDic[SkillEnum.HighSpeedLv2].count * 0.1f;
        value += skillDataDic[SkillEnum.HighSpeedLv3].count * 0.25f;

        return value;

    }
    #endregion
    #region//ギャンブラー
    void Gambler()
    {
        gameManager.canvasManager.IncreaseSkillItemSelectCount();
    }
    #endregion
    #region//マジシャン
    [System.NonSerialized] public int magicianInt;
    void MagicianLaunch()//起動時
    {
        if (skillDataDic[SkillEnum.Magician].count != 1) return; //最初に手に入れた時だけ
        gameManager.canvasManager.rewardUIData.magicianButton.magicianButton.SetActive(true);
    }
    public bool Magician()
    {
        if (magicianInt >= skillDataDic[SkillEnum.Magician].count) return false; 

        ++magicianInt;

        if(magicianInt >= skillDataDic[SkillEnum.Magician].count)
        {
            gameManager.canvasManager.rewardUIData.magicianButton.selected.enabled = true;
            ColorUtility.TryParseHtmlString("#636363", out Color color);
            gameManager.canvasManager.rewardUIData.magicianButton.skillImage.color = color;
        }

        return true;
    }
    public void MagicianReset()
    {
        magicianInt = 0;
        gameManager.canvasManager.rewardUIData.magicianButton.selected.enabled = false;
        gameManager.canvasManager.rewardUIData.magicianButton.skillImage.color = Color.white;
    }
    #endregion
    #region//サモンサークル
    public void SummonCircle(Vector2 pos)
    {
        if (skillDataDic[SkillEnum.SummonCircle].count == 0) return;
        float multiple = 0.1f * skillDataDic[SkillEnum.SummonCircle].count;
        int damageValue = (int)(gameManager.damageManager.DamageFinalCalculation(gameManager.damageManager.atk, DealDamageType.nonContact, multiple: multiple));
        if (damageValue <= 0) damageValue = 1;

        gameManager.effectManager.AttackEffectPlay("summonCircle", new Vector2(pos.x, pos.y), damageValue);
        gameManager.soundManager.Play("summon_1", 1.25f, 0.1f);
    }
    #endregion
    #region//両刃剣
    void DoubleEdgedSwordLaunch()//起動時
    {
        gameManager.playerChip.playerChipskillClass.DoubleEdgedSword.SetActive(true);
    }
    public void DoubleEdgedSword(GameObject damageStrig, int value)
    {
        if (skillDataDic[SkillEnum.DoubleEdgedSword].count == 0) return;
        value = (int)(value * 0.75f);
        damageStrig.name = DealDamageType.spear.ToString() + "_" + value.ToString();
    }
    #endregion
    #region//鍛冶職人
    public int Blacksmith()
    {
        if (skillDataDic[SkillEnum.Blacksmith].count == 0) return 0;
        int value = skillDataDic[SkillEnum.Blacksmith].count * 5;
        return value;
    }
    #endregion
    #region//必殺
    public int Deadly(int value, ref EffectTextType effectTextType, ref Color color)
    {
        if (skillDataDic[SkillEnum.Deadly].count == 0) return value;
        if(Random.value < skillDataDic[SkillEnum.Deadly].count * 0.03)
        {
            value = 99;
            effectTextType = EffectTextType.skillCritical;
            color = default;
        }
        return value;
    }
    #endregion
    #region//チェイン
    [System.NonSerialized] public Transform chainTrans;
    public void Chain(Transform trans)
    {
        if (skillDataDic[SkillEnum.Chain].count == 0) return;
        chainTrans = trans;
        gameManager.effectManager.EffectPlay("chainEffect", trans.position, effectType: EffectType.particle, particleType: ParticleType.shot, shotType: ShotType.chain);
        gameManager.soundManager.Play("sword_1", 1.75f, 0.05f);
    }
    public string Chain()
    {
        int damageValue = (int)(gameManager.damageManager.DamageFinalCalculation(gameManager.damageManager.atk, DealDamageType.nonContact, 0.25f));
        if (damageValue <= 0) damageValue = 1;

        return DealDamageType.nonContactParticle.ToString() + "_" + damageValue;
    }
    #endregion
    #region//インフィニティ
    public bool Infinity(ref int value, ref EffectTextType effectTextType, ref Color color)
    {
        if (skillDataDic[SkillEnum.Infinity].count == 0) return false;
        if (Random.value < skillDataDic[SkillEnum.Infinity].count * 0.01f)
        {
            value = 888;
            effectTextType = EffectTextType.skillCritical;
            color = default;
            return true;
        }
        else
        {
            return false;
        }
    }
    #endregion
    #region//巨大化
    public void Giant1() //Spearの大きさ
    {
        if (skillDataDic[SkillEnum.GiantLv1].count == 0
            && skillDataDic[SkillEnum.GiantLv2].count == 0
            && skillDataDic[SkillEnum.GiantLv3].count == 0
            ) return;

        float value = 1;
        value += skillDataDic[SkillEnum.GiantLv1].count * 0.15f;
        value += skillDataDic[SkillEnum.GiantLv2].count * 0.2f;
        value += skillDataDic[SkillEnum.GiantLv3].count * 0.35f;

        gameManager.playerChip.SpearLengh(value);
    }
    public float Giant2() //Arrowの大きさ
    {
        if (skillDataDic[SkillEnum.GiantLv1].count == 0
            && skillDataDic[SkillEnum.GiantLv2].count == 0
            && skillDataDic[SkillEnum.GiantLv3].count == 0
            ) return 1;

        float value = 1;
        value += skillDataDic[SkillEnum.GiantLv1].count * 0.15f;
        value += skillDataDic[SkillEnum.GiantLv2].count * 0.2f;
        value += skillDataDic[SkillEnum.GiantLv3].count * 0.35f;

        return value;
    }
    public float Giant3()
    {
        if (skillDataDic[SkillEnum.GiantLv1].count == 0
            && skillDataDic[SkillEnum.GiantLv2].count == 0
            && skillDataDic[SkillEnum.GiantLv3].count == 0
            ) return 0;

        float value = 0;
        value += skillDataDic[SkillEnum.GiantLv1].count * 0.06f;
        value += skillDataDic[SkillEnum.GiantLv2].count * 0.12f;
        value += skillDataDic[SkillEnum.GiantLv3].count * 0.20f;

        return value;
    }
    #endregion
    #region//クライマー
    public void Climber(Rigidbody2D rb2D, List<TagDetection> climberDitection)
    {
        if (skillDataDic[SkillEnum.Climber].count == 0) return;
        foreach (var info in climberDitection)
        {
            if (info.Detection()) {
                rb2D.velocity = new Vector2(0, 0);
                gameManager.hpManager.EnergyFluctuation(0.01f);
                //gameManager.soundManager.Play("collision_1", 1.25f, 0.05f);
            } 
        }
    }
    #endregion
    #region//ヴァンパイア
    int vampireInt;
    public void Vampire()
    {
        if (skillDataDic[SkillEnum.Vampire].count == 0) return;

        vampireInt += 1;
        if (vampireInt < 3) return;

        int value = skillDataDic[SkillEnum.Vampire].count;

        Vector2 pos = gameManager.player.transform.position;
        gameManager.hpManager.HPFluctuation(value, new Vector2(pos.x + Random.Range(-0.25f, 0.25f), pos.y + Random.Range(-0.25f, 0.25f)), TakeDamageType.recovery);

        vampireInt = 0;
    }
    #endregion
    #region//捕食者
    int predatorInt;
    public void Predator()
    {
        if (skillDataDic[SkillEnum.Predator].count == 0) return;

        predatorInt += 1;
        if (predatorInt < 5) return;

        int value = skillDataDic[SkillEnum.Predator].count;
        gameManager.hpManager.MaxHPFluctuation(value);

        predatorInt = 0;
    }
    #endregion
    #region//バウンド矢
    public int BoundArrow()
    {
        int value = 1;

        value += skillDataDic[SkillEnum.BoundArrow].count;

        return value;
    }
    #endregion
    #region//スピードチェンジ
    public float SpeedChange()
    {
        if (skillDataDic[SkillEnum.SpeedChange].count == 0) return 1f;
        float value = 1f - (skillDataDic[SkillEnum.SpeedChange].count * 0.25f);
        return value;
    }
    #endregion
    #region//ラビットファイア
    public float RapidFire()
    {
        if (skillDataDic[SkillEnum.RapidFire].count == 0) return 1f;
        float value = 1f - (skillDataDic[SkillEnum.RapidFire].count * 0.2f);
        return value;
    }
    #endregion
    #region//ドラゴンショット
    public float DragonShot()
    {
        if (skillDataDic[SkillEnum.DragonShot].count == 0) return 1f;
        float value = 1f + (skillDataDic[SkillEnum.DragonShot].count * 0.2f);
        return value;
    }
    #endregion
    #region//ダイチェイン
    public void GreatChain(Transform trans)
    {
        if (skillDataDic[SkillEnum.GreatChain].count == 0) return;
        gameManager.effectManager.EffectPlay("greatChainEffect", trans.position, scale: 1f, effectType: EffectType.particle, destroyTime: 10f, particleType: ParticleType.shot, shotType: ShotType.greatChain);
        gameManager.soundManager.Play("wind_1", 1.25f, 0.1f);
    }
    public string GreatChain()
    {
        int damageValue = (int)(gameManager.damageManager.DamageFinalCalculation(gameManager.damageManager.atk, DealDamageType.nonContact, 0.75f));
        if (damageValue <= 0) damageValue = 1;

        return DealDamageType.nonContactParticle.ToString() + "_" + damageValue;
    }
    #endregion
    #region//エクスプロージョン
    public void Exposion(Vector2 pos)
    {
        if (skillDataDic[SkillEnum.Explosion].count == 0) return;
        float multiple = 1 + (0.25f * skillDataDic[SkillEnum.Explosion].count);
        int damageValue = (int)(gameManager.damageManager.DamageFinalCalculation(gameManager.damageManager.atk, DealDamageType.nonContact, multiple: multiple));

        gameManager.effectManager.AttackEffectPlay("explosion", pos, damageValue, scale: 1.5f + (0.5f * skillDataDic[SkillEnum.Explosion].count));
        gameManager.soundManager.Play("fire_1", 1.1f, 0.05f);
    }
    #endregion
    #region//野宿セット
    public void CampSet()
    {
        if (skillDataDic[SkillEnum.CampSetLv1].count == 0
            && skillDataDic[SkillEnum.CampSetLv2].count == 0
            && skillDataDic[SkillEnum.CampSetLv3].count == 0
            ) return;

        float value = 0;
        value += skillDataDic[SkillEnum.CampSetLv1].count * 0.05f * gameManager.hpManager.maxHp;
        value += skillDataDic[SkillEnum.CampSetLv2].count * 0.10f * gameManager.hpManager.maxHp;
        value += skillDataDic[SkillEnum.CampSetLv3].count * 0.20f * gameManager.hpManager.maxHp;

        int returnValue = (int)value;
        if (returnValue <= 0) returnValue = 1;

        gameManager.hpManager.HPFluctuation(returnValue, gameManager.player.transform.position, TakeDamageType.recovery);
    }
    #endregion
    #region//シールド
    [SerializeField] GameObject sheild;
    List<Transform> shieldList = new List<Transform>();
    void Shield()
    {
        if (skillDataDic[SkillEnum.Shieid].count == 0) return;
        GameObject insSheild = Instantiate(sheild, gameManager.player.transform.position, Quaternion.identity, gameManager.player.transform.Find("SkillSpace"));
        shieldList.Add(insSheild.transform);
    }
    void UpdateSheild()
    {
        if (skillDataDic[SkillEnum.Shieid].count == 0) return;
        for (int count = 0; count < skillDataDic[SkillEnum.Shieid].count; ++count)
        {
            Vector2 deltaCoordinate = ReturnShieldPos((360f / skillDataDic[SkillEnum.Shieid].count) * count);
            //Debug.Log("バリュー:" + ((360f / skillDataDic[SkillEnum.Shieid].count) * count));
            shieldList[count].position = gameManager.player.transform.position + (Vector3)deltaCoordinate;
        }
        shieldCurrentRotate += 2f;
    }
    float shieldCurrentRotate;
    Vector2 ReturnShieldPos(float setDeltaDegree = default)
    {
        float degree = default;
        if (setDeltaDegree == default) degree = shieldCurrentRotate;
        else degree = shieldCurrentRotate + setDeltaDegree;  

        Vector2 coordinate = DataBase.CoordinateUpperLimit(degree, 0.75f);
        return coordinate;
    }
    #endregion
    #region//イージスシールド
    [System.Serializable] public class AegisShieldData
    {
        public GameObject aegisShield;
        [HideInInspector] public Vector3 aegisShieldPointer;
        [HideInInspector] public Transform aegisShieldTrans;
        [HideInInspector] public SpriteRenderer aegisShieldSprite;
        [HideInInspector] public Collider2D aegisShieldCollider;
        [HideInInspector] public float playerArmDegree;
    }
    public AegisShieldData aegisShieldData = new AegisShieldData();
    void AegisShield()
    {
        if (skillDataDic[SkillEnum.AegisShieid].count == 0) return;
        if (skillDataDic[SkillEnum.AegisShieid].count == 1)
        {
            GameObject insAegisShield = Instantiate(aegisShieldData.aegisShield, gameManager.player.transform.position, Quaternion.identity, gameManager.player.transform.Find("SkillSpace"));
            insAegisShield.name = SkillDataBase.SkillEnum.AegisShieid.ToString();
            aegisShieldData.aegisShieldSprite = insAegisShield.GetComponent<SpriteRenderer>();
            aegisShieldData.aegisShieldCollider = insAegisShield.GetComponent<Collider2D>();
            aegisShieldData.aegisShieldSprite.DOFade(0f, 0.5f);
            aegisShieldData.aegisShieldTrans = insAegisShield.transform;
        }
        else
        {
            aegisShieldData.aegisShieldTrans.localScale *= 1.5f;
        }

    }
    public void AegisShieldRotate()
    {
        if (skillDataDic[SkillEnum.AegisShieid].count == 0) return;
        Vector2 coordinate = DataBase.CoordinateUpperLimit(aegisShieldData.playerArmDegree -180, 0.75f);
        aegisShieldData.aegisShieldPointer = gameManager.player.transform.position + (Vector3)coordinate;

        aegisShieldData.aegisShieldTrans.position = Vector2.MoveTowards(aegisShieldData.aegisShieldTrans.position, aegisShieldData.aegisShieldPointer, 0.15f);
    }
    public void AegisShieldFade(bool fade)
    {
        if (skillDataDic[SkillEnum.AegisShieid].count == 0) return;
        if (fade)
        {
            aegisShieldData.aegisShieldSprite.DOFade(0f, 0.5f);
            aegisShieldData.aegisShieldCollider.enabled = false;
        }
        else
        {
            aegisShieldData.aegisShieldSprite.DOFade(1f, 0.5f);
            aegisShieldData.aegisShieldCollider.enabled = true;
        }
    }
    public void AegisShieldShot()
    {
        gameManager.effectManager.EffectPlay("energy_1", aegisShieldData.aegisShieldTrans.position, scale: 1.75f, effectType: EffectType.animation);

        float degree = DataBase.GetAngle(aegisShieldData.aegisShieldTrans.position - gameManager.player.transform.position);
        Vector3 coordinate = (Vector3)DataBase.CoordinateUpperLimit(degree, 3.75f);

        float multiple = 2.5f + (0.5f * skillDataDic[SkillEnum.AegisShieid].count);
        int damageValue = (int)(gameManager.damageManager.DamageFinalCalculation(gameManager.damageManager.atk, DealDamageType.nonContact, multiple: multiple));

        gameManager.effectManager.AttackEffectPlay("energy_2", aegisShieldData.aegisShieldTrans.position + coordinate, damageValue, rotate: degree, scale: 2f);

        GameManager.gameManager.soundManager.Play("sword_2", 1.25f, 0.5f);

    }
    #endregion
    #region//ミニショット
    public int MiniShot()
    {
        if (skillDataDic[SkillEnum.MiniShot].count == 0) return 0;

        return skillDataDic[SkillEnum.MiniShot].count;
    }
    #endregion
    #region//ダブルショット
    public int DoubleShot()
    {
        if (skillDataDic[SkillEnum.DoubleShot].count == 0) return 1;

        return 1 + skillDataDic[SkillEnum.DoubleShot].count;
    }
    #endregion
    #region//クリティカル
    public float Critical()
    {
        float criticalRate = 0f;

        criticalRate += skillDataDic[SkillEnum.CriticalLv1].count * 0.05f;
        criticalRate += skillDataDic[SkillEnum.CriticalLv2].count * 0.08f;
        criticalRate += skillDataDic[SkillEnum.CriticalLv3].count * 0.15f;

        return criticalRate;
    }
    #endregion
    #region//盾錬成
    public void ShieldForge()
    {
        if (skillDataDic[SkillEnum.ShieldForge].count == 0) return;

        gameManager.hpManager.InvincibleTime(skillDataDic[SkillEnum.ShieldForge].count * 0.5f);
        gameManager.soundManager.Play("sword_2", 1.5f, 0.1f);
    }
    #endregion
    #region//プロテクション
    public float Protection()
    {
        float protectionTime = 0f;

        protectionTime += skillDataDic[SkillEnum.Protection].count * 0.5f;

        return protectionTime;
    }
    #endregion
    #region//幸運
    public void Lucky(Vector2 pos)
    {
        if (skillDataDic[SkillEnum.Lucky].count == 0) return;

        if(Random.value < skillDataDic[SkillEnum.Lucky].count * 0.02f)
        {
            SkillDrop(SkillRarity.common, pos);
        }

    }
    #endregion
    #region//クリティカルウェーブ
    public void CriticalWave(Vector2 pos)
    {
        if (skillDataDic[SkillEnum.CricalWave].count == 0) return;
        float multiple = 0.1f * skillDataDic[SkillEnum.CricalWave].count;
        int damageValue = (int)(gameManager.damageManager.DamageFinalCalculation(gameManager.damageManager.atk, DealDamageType.nonContactParticle, multiple: multiple));

        gameManager.effectManager.AttackEffectPlay("explosion_2", pos, damageValue, scale: 10f, dealDamageType: DealDamageType.nonContactParticle);
        gameManager.soundManager.Play("block_1", 1.25f, 0.1f);
    }
    #endregion
    #region//聖なる矢
    public float HollyArrow(float gravityScale)
    {
        if (skillDataDic[SkillEnum.HollyArrow].count == 0) return gravityScale;
        return 0f;
    }
    public ArrowType HollyArrow(ArrowType arrowType)
    {
        if (skillDataDic[SkillEnum.HollyArrow].count == 0) return arrowType;
        return ArrowType.HollyArrow;
    }
    public float HollyArrow()
    {
        if (skillDataDic[SkillEnum.HollyArrow].count == 0) return 1f;
        float value = 1f + (skillDataDic[SkillEnum.HollyArrow].count * 0.25f);
        return value;
    }
    public void HollyArrow(Vector2 pos)
    {
        if (skillDataDic[SkillEnum.HollyArrow].count == 0) return;

        float multiple = 0.5f +( 0.25f * skillDataDic[SkillEnum.HollyArrow].count);
        int damageValue = (int)(gameManager.damageManager.DamageFinalCalculation(gameManager.damageManager.atk, DealDamageType.nonContact, multiple: multiple));

        gameManager.effectManager.AttackEffectPlay("shine_1", pos, damageValue, scale: 1f);
        gameManager.soundManager.Play("defense_2", 2f, 0.05f);
    }
    #endregion
    #region//トライデント
    public float TridentDegree { protected get; set; }
    public void Trident()
    {
        if (skillDataDic[SkillEnum.Trident].count == 0) return;

        StartCoroutine(TridentCoroutine());
    }
    public float Trident(float defaltValue)
    {
        if (skillDataDic[SkillEnum.Trident].count == 0) return defaltValue;

        return 0f;
    }
    public void Trident(bool gravity, Rigidbody2D rigidbody2D)
    {
        if (skillDataDic[SkillEnum.Trident].count == 0) return;

        if (gravity)
        {
            rigidbody2D.gravityScale = 0f;
        }
        else
        {
            rigidbody2D.gravityScale = 1f;
            LowGravity(rigidbody2D);
        }
    }
    IEnumerator TridentCoroutine()
    {
        float multiple = 0.8f * skillDataDic[SkillEnum.Trident].count;
        float scale = 0.5f +( 0.5f * skillDataDic[SkillEnum.Trident].count);
        int damageValue = (int)(gameManager.damageManager.DamageFinalCalculation(gameManager.damageManager.atk, DealDamageType.nonContact, multiple: multiple));
        float angle = TridentDegree;
        Vector2 playerPos = gameManager.player.transform.position;

        Vector2 pos = DataBase.CoordinateUpperLimit(angle, 1.5f) + playerPos;
        gameManager.effectManager.AttackEffectPlay("slash_2", pos, damageValue, rotate: angle - 90, scale: 1.5f * scale);
        gameManager.soundManager.Play("sword_5", 1.25f, 0.05f);

        yield return new WaitForSeconds(0.1f);

        pos = DataBase.CoordinateUpperLimit(angle, 2f) + playerPos;
        gameManager.effectManager.AttackEffectPlay("slash_1", pos, damageValue, rotate: angle - 30, scale: 0.5f * scale);
        gameManager.soundManager.Play("sword_4", 1f, 0.025f);
    }
    #endregion
    #region//グングニル
    public void Gungnir(Vector2 pos)
    {
        if (skillDataDic[SkillEnum.Gungnir].count == 0) return;

        StartCoroutine(GungnirCoroutine(pos));
    }
    IEnumerator GungnirCoroutine(Vector2 pos)
    {
        for(int cnt = 0; cnt < skillDataDic[SkillEnum.Gungnir].count; ++cnt)
        {
            gameManager.effectManager.EffectPlay("gungnirLightEffect", pos, effectType: EffectType.particle, particleType: ParticleType.shot, shotType: ShotType.gungnirLight);
            gameManager.soundManager.Play("shine_1", 1.25f, 0.3f);
            yield return new WaitForSeconds(Random.Range(0.2f, 0.25f));
        }
    }
    public float Gungnir()
    {
        float multiple = 1 + (0.75f);
        int damageValue = (int)(gameManager.damageManager.DamageFinalCalculation(gameManager.damageManager.atk, DealDamageType.nonContact, multiple: multiple));

        return damageValue;
    }
    public System.Tuple< Vector2[],float> GungnirLightPoint(Vector2 center,float length = 12.5f)
    {
        float angle = Random.Range(315f, 405f);
        Vector2 start = center + (Vector2)(Quaternion.Euler(0, 0, angle + 90f) * Vector2.right * length);
        Vector2 end = center + (Vector2)(Quaternion.Euler(0, 0, angle + 90f) * Vector2.right * -length);
        return System.Tuple.Create(new Vector2[] { start, end }, angle);
    }
    #endregion
    #region//光の友
    [System.Serializable]public class FriendOfLightData
    {
        public Skill_FriendOfLight instanceFriendOfLight;
        [HideInInspector] public List<Transform> monsterTransforms = new List<Transform>();
        public LayerMask layerMask; //検知する対象のレイヤーマスク
    }
    public FriendOfLightData friendOfLightData = new FriendOfLightData();
    void FriendOfLight()
    {
        if (skillDataDic[SkillEnum.FriendOfLight].count == 1) StartCoroutine(FriendOfLightCoroutine());

        var friend = Instantiate(friendOfLightData.instanceFriendOfLight, gameManager.player.transform.position, Quaternion.identity);
        friend.StarProcess();
    }
    public float FriendOfLightFloat()
    {
        int damageValue = (int)(gameManager.damageManager.DamageFinalCalculation(gameManager.damageManager.atk, DealDamageType.nonContact, multiple: 0.25f));

        return damageValue;
    }
    IEnumerator FriendOfLightCoroutine()
    {
        while (true)
        {
            friendOfLightData.monsterTransforms = GetObjectsWithinRange();
            yield return new WaitForSeconds(0.5f);
        }
    }
    List<Transform> GetObjectsWithinRange() //範囲内のモンスターのTransform値をリストで取得
    {
        List<Transform> objectsInRange = new List<Transform>();

        Collider2D[] colliders = Physics2D.OverlapCircleAll(gameManager.player.transform.position, 2f,friendOfLightData.layerMask);
        foreach (Collider2D collider in colliders)
        {
            Transform objectTransform = collider.transform;
            objectsInRange.Add(objectTransform);
        }

        return objectsInRange;
    }
    /*
    private void OnDrawGizmos()
    {
        Gizmos.color = Color.yellow;
        Gizmos.DrawWireSphere(gameManager.player.transform.position, 1.5f);
    }
    */
    #endregion
    #endregion

    #region//アイテム

    #region//リンゴ
    void Apple(SkillEnum skillEnum)
    {
        int value;
        switch (skillEnum)
        {
            case SkillEnum.GreenApple:
                value = 7;
                gameManager.hpManager.HPFluctuation(value, default, TakeDamageType.recovery);
                break;
            case SkillEnum.Apple:
                value = 18;
                gameManager.hpManager.HPFluctuation(value, default, TakeDamageType.recovery);
                break;
        }
    }
    #endregion
    #region//ポーション
    void Potion(SkillEnum skillEnum)
    {
        int value;
        switch (skillEnum)
        {
            case SkillEnum.Potion:
                value = (int)(gameManager.hpManager.maxHp * 0.4f);
                gameManager.hpManager.HPFluctuation(value, default, TakeDamageType.recovery);
                break;
            case SkillEnum.HighPotion:
                value = (int)(gameManager.hpManager.maxHp * 1f);
                gameManager.hpManager.HPFluctuation(value, default, TakeDamageType.recovery);
                break;
        }
    }
    #endregion

    #endregion
}
