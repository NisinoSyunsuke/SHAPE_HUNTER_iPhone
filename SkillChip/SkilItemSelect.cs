using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using SkillDataBase;

public class SkilItemSelect : MonoBehaviour
{
    SkillEnum skillEnum;
    Animator anim;
    //GameManager gameManager;

    [SerializeField] Image image;
    [SerializeField] Image frame;
    [SerializeField] Material material;
    //public Image selected;
    public bool Selected {
        protected get { return Selected; } 
        set {
            if(value) anim.CrossFadeInFixedTime("selected", 0.1f, layer: 0);
            else anim.CrossFadeInFixedTime("default", 0.1f, layer: 0);
        } 
    }

    [SerializeField] Sprite skillFrame; [SerializeField] Sprite itemFrame;
    void Start()
    {
        anim = GetComponent<Animator>();
    }

    public void SkillRegister(SkillEnum _skillEnum)
    {
        image.sprite = GameManager.gameManager.skillManager.skillDataDic[_skillEnum].skillSprite;
        frame.color = GameManager.gameManager.skillManager.rarityColorDic[GameManager.gameManager.skillManager.skillDataDic[_skillEnum].skillRarity];

        skillEnum = _skillEnum;

        //Typeごとにスプライトを変更
        switch (GameManager.gameManager.skillManager.skillDataDic[skillEnum].type)
        {
            case SkillItemType.skill: frame.sprite = skillFrame; break;
            case SkillItemType.item: frame.sprite = itemFrame; break;
        }

        switch (GameManager.gameManager.skillManager.skillDataDic[_skillEnum].skillRarity)
        {
            case SkillRarity.epic:
            case SkillRarity.legendary:
                frame.material = material;
                break;
            default:
                frame.material = default;
                break;
        }
    }

    public void SkillSelectButton()
    {
        GameManager.gameManager.canvasManager.SelectSkill(skillEnum, this);
    }
}
