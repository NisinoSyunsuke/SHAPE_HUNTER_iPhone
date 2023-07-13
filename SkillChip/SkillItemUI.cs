using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using SkillDataBase;
using UnityEngine.UI;

public class SkillItemUI : MonoBehaviour
{
    //SkillEnum skillEnum;
    [SerializeField] Image image;
    [SerializeField] Image frame; [SerializeField] Image countFrame;
    [SerializeField] Text countText;
    [SerializeField] Material material;
    void Start()
    {
        
    }

    public void SkillRegister(SkillEnum _skillEnum)
    {
        image.sprite = GameManager.gameManager.skillManager.skillDataDic[_skillEnum].skillSprite;
        Color color = GameManager.gameManager.skillManager.rarityColorDic[GameManager.gameManager.skillManager.skillDataDic[_skillEnum].skillRarity];
        frame.color = color; countFrame.color = color;
        countText.text = GameManager.gameManager.skillManager.skillDataDic[_skillEnum].count.ToString();

        switch (GameManager.gameManager.skillManager.skillDataDic[_skillEnum].skillRarity)
        {
            case SkillRarity.epic:
            case SkillRarity.legendary:
                frame.material = material;
                break;
        }
        //skillEnum = _skillEnum;
    }
}
