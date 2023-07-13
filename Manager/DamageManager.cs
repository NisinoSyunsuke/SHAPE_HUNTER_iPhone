using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using HPDataBase;

public class DamageManager : MonoBehaviour
{
    GameManager gameManager; SkillManager skillManager;
    Rigidbody2D playerRb2D;
    GameObject attackValue;

    //変数
    [Header("攻撃力")]public int atk;
    public void StartProcess()
    {
        gameManager = GameManager.gameManager; skillManager = gameManager.skillManager;
        playerRb2D = gameManager.player.GetComponent<Rigidbody2D>();
        attackValue = gameManager.playerChip.attackCollider;

        //TriggerDamageRegister();
    }
    #region//衝突ダメージ
    public int CollsionDamageCalculation(DealDamageType dealDamageType)
    {
        //float value = playerRb2D.velocity.magnitude * 0.5f;

        int damage = DamageFinalCalculation(atk, dealDamageType);

        gameManager.hpManager.EnergyFluctuation(0.25f);
        //if (damage == 0) damage = 1;
        return damage;
    }
    #endregion
    #region//非衝突ダメージ
    public void TriggerDamageRegister()
    {
        int value = DamageFinalCalculation(atk, DealDamageType.spear);
        attackValue.name = DealDamageType.spear.ToString() + "_" + value.ToString();
        #region//スキル発動
        gameManager.skillManager.DoubleEdgedSword(gameManager.playerChip.playerChipskillClass.DoubleEdgedSword, value);
        #endregion
    }
    #endregion
    #region//最終ダメージ計算
    public int DamageFinalCalculation(float initialValue, DealDamageType dealDamageType, float multiple = 1f)
    {
        float value = initialValue; 
        value += skillManager.SmallKnife();
        value *= (1 + skillManager.Atthlete() + skillManager.Giant3());

        value *= multiple;

        switch (dealDamageType)
        {
            //case DealDamageType.collision: break;
            case DealDamageType.spear:
                value += skillManager.Blacksmith();
                break;//槍の攻撃にだけ付与
            case DealDamageType.arrow:
                value *= skillManager.DragonShot();
                value *= skillManager.HollyArrow();
                break;
            case DealDamageType.nonContact: break;
        }

        return (int)value;
    }
    #endregion
}
