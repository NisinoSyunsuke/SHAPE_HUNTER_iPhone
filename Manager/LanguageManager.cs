using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using LanguageData;
using TMPro;
using UnityEngine.UI;

public class LanguageManager : MonoBehaviour
{
    [SerializeField] TextBase uiText;
    [SerializeField] TextBase skillNameText;
    [SerializeField] ExplanationBase skillExplaiontext;
    //[SerializeField] ExplanationBase explanationText;

    public void StarProcess()
    {
        if (DataBase.firstLaunchBool) return;
        GameManager.gameManager.saveDataManager.saveData.firstLaunchBool = true;
        switch (Application.systemLanguage)
        {
            case SystemLanguage.Japanese:
                DataBase.Language = Language.japanese;
                break;
            case SystemLanguage.English:
            default:
                DataBase.Language = Language.english;
                break;
        }
        GameManager.gameManager.titleManager.OpsionSetUp();
        GameManager.gameManager.saveDataManager.Save();
        UITextDataApply();
    }

    public string RetrunText(string english_text, TextGroup textGroup)
    {
        string text = "";
        switch (textGroup)
        {
            case TextGroup.uiText: text = uiText.RetrunText(english_text, DataBase.Language); break;
            case TextGroup.skillNameText: text = skillNameText.RetrunText(english_text, DataBase.Language); break;
            case TextGroup.skillExplaiontext: text = skillExplaiontext.RetrunText(english_text, DataBase.Language); break;
                //case TextGroup.explanation_text: text = explanationText.RetrunText(english_text, language); break;

        }
        return text;
    }

    #region//ï∂éöï™äÑ
    public string[] RerunStringArray(string text) //"_"Ç≈ï∂éöÇï™äÑ
    {
        string[] arrText = text.Split('_');
        return arrText;
    }
    #endregion
    #region//UITextÉfÅ[É^
    [System.Serializable]
    class UITextDataClass
    {
        [System.Serializable]
        public class UITextData
        {
            public string englishText;
            public Text uiText;
        }
        [System.Serializable]
        public class UITextMeshProData
        {
            public string englishText;
            public TextMeshProUGUI uiText;
        }

        public List<UITextData> uiTextDatas = new List<UITextData>();
        public List<UITextMeshProData> uiTextMeshProDatas = new List<UITextMeshProData>();
    }
    [SerializeField] UITextDataClass uiTextDataClass = new UITextDataClass();
    public void UITextDataApply()
    {
        foreach (var uiTextData in uiTextDataClass.uiTextDatas)
        {
            uiTextData.uiText.text = RetrunText(uiTextData.englishText, TextGroup.uiText);
        }
        foreach (var uiTextMeshProData in uiTextDataClass.uiTextMeshProDatas)
        {
            uiTextMeshProData.uiText.text = RetrunText(uiTextMeshProData.englishText, TextGroup.uiText);
        }
    }
    #endregion
}
