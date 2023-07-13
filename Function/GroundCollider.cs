using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class GroundCollider : MonoBehaviour
{
    string groundTag = "ground";
    //[SerializeField] bool wall_active; string wallTag = "none";
    //[SerializeField] bool thin_inactive; string thin_ground_tag = "thin_ground";
    [SerializeField] bool monster_active; string monsterTag = "none";
    [SerializeField] bool wall_active; string wallTag = "none";
    [SerializeField] bool player_active; string playerTag = "none";
    [SerializeField] bool playerAttack_active; string playerAttackTag = "none";
    [Header("playerAttack_active���I���̏ꍇbreak�G�t�F�N�g���g�p����"), SerializeField] bool onBreakEffect;
    private bool isGround = false;
    private bool isGroundEnter, isGroundStay;
    //bool isGroundExit; //�l�q��
    private void Start()
    {
        //if (thin_inactive) thin_ground_tag = "none";
        if (wall_active) wallTag = "wall";
        if (monster_active) monsterTag = "monster";
        if (player_active) playerTag= "Player";
        if (playerAttack_active) playerAttackTag = "playerAttack";
    }
    public bool IsGround()
    {
        isGround = false;
        if (isGroundEnter || isGroundStay)
        {
            isGround = true;
        }
        //else if(isGroundExit){isGround = false;}
        isGroundEnter = false;
        isGroundStay = false;
        //isGroundExit = false;
        return isGround;
    }
    private void OnTriggerEnter2D(Collider2D collision)
    {
        if ((collision.tag == groundTag) || (collision.tag == playerTag) || (collision.tag == wallTag) || (collision.tag == monsterTag) || (collision.tag == playerAttackTag))
        {
            //Debug.Log("��������");
            isGroundEnter = true;
            BreakEffect(collision);
        }
    }
    private void OnTriggerStay2D(Collider2D collision)
    {
        if ((collision.tag == groundTag) || (collision.tag == playerTag) || (collision.tag == wallTag) || (collision.tag == monsterTag) || (collision.tag == playerAttackTag))
        {
            //Debug.Log("��������");
            isGroundStay = true;
            BreakEffect(collision);
        }
    }

    void BreakEffect(Collider2D collision)
    {
        if (collision.tag != playerAttackTag) return;
        if (!onBreakEffect) return;

        GameManager.gameManager.effectManager.TextEffectPlay("BREAK", collision.ClosestPoint(this.transform.position), effectTextType: EffectData.EffectTextType.floatUp, activeOutline: true, multiple: 2.5f);
        GameManager.gameManager.soundManager.Play("defense_2", 1.25f, 0.075f);

        //�X�L��_�_�C�`�F�C��
        GameManager.gameManager.skillManager.GreatChain(transform);
        //�X�L��_�G�N�X�v���[�W����
        GameManager.gameManager.skillManager.Exposion(collision.ClosestPoint(this.transform.position));
        //�X�L��_�C�[�W�X�V�[���h
        if (collision.gameObject.name == SkillDataBase.SkillEnum.AegisShieid.ToString()) GameManager.gameManager.skillManager.AegisShieldShot();
        //�X�L��_���B��
        GameManager.gameManager.skillManager.ShieldForge(); ;
        //Debug.Log(collision.gameObject.name + "**" + "Obj_" + SkillDataBase.SkillEnum.AegisShieid.ToString());
    }
}
