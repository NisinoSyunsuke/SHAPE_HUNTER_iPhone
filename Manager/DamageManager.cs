using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using HPDataBase;

public class DamageManager : MonoBehaviour
{
    GameManager gameManager; SkillManager skillManager;
    Rigidbody2D playerRb2D;
    GameObject attackValue;

    //�ϐ�
    [Header("�U����")]public int atk;
    public void StartProcess()
    {
        gameManager = GameManager.gameManager; skillManager = gameManager.skillManager;
        playerRb2D = gameManager.player.GetComponent<Rigidbody2D>();
        attackValue = gameManager.playerChip.attackCollider;

        //TriggerDamageRegister();
    }
    #region//�Փ˃_���[�W
    public int CollsionDamageCalculation(DealDamageType dealDamageType)
    {
        //float value = playerRb2D.velocity.magnitude * 0.5f;

        int damage = DamageFinalCalculation(atk, dealDamageType);

        gameManager.hpManager.EnergyFluctuation(0.25f);
        //if (damage == 0) damage = 1;
        return damage;
    }
    #endregion
    #region//��Փ˃_���[�W
    public void TriggerDamageRegister()
    {
        int value = DamageFinalCalculation(atk, DealDamageType.spear);
        attackValue.name = DealDamageType.spear.ToString() + "_" + value.ToString();
        #region//�X�L������
        gameManager.skillManager.DoubleEdgedSword(gameManager.playerChip.playerChipskillClass.DoubleEdgedSword, value);
        #endregion
    }
    #endregion
    #region//�ŏI�_���[�W�v�Z
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
                break;//���̍U���ɂ����t�^
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
