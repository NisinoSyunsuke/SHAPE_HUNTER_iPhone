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
    [Header("playerAttack_activeがオンの場合breakエフェクトを使用する"), SerializeField] bool onBreakEffect;
    private bool isGround = false;
    private bool isGroundEnter, isGroundStay;
    //bool isGroundExit; //様子見
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
            //Debug.Log("何か来た");
            isGroundEnter = true;
            BreakEffect(collision);
        }
    }
    private void OnTriggerStay2D(Collider2D collision)
    {
        if ((collision.tag == groundTag) || (collision.tag == playerTag) || (collision.tag == wallTag) || (collision.tag == monsterTag) || (collision.tag == playerAttackTag))
        {
            //Debug.Log("何かいる");
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

        //スキル_ダイチェイン
        GameManager.gameManager.skillManager.GreatChain(transform);
        //スキル_エクスプロージョン
        GameManager.gameManager.skillManager.Exposion(collision.ClosestPoint(this.transform.position));
        //スキル_イージスシールド
        if (collision.gameObject.name == SkillDataBase.SkillEnum.AegisShieid.ToString()) GameManager.gameManager.skillManager.AegisShieldShot();
        //スキル_盾錬成
        GameManager.gameManager.skillManager.ShieldForge(); ;
        //Debug.Log(collision.gameObject.name + "**" + "Obj_" + SkillDataBase.SkillEnum.AegisShieid.ToString());
    }
}
