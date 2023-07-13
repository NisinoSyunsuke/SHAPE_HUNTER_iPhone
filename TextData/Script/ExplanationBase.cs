using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using LanguageData;

[CreateAssetMenu(fileName = "Data", menuName = "ScriptableObjects/ExplanationData")]
public class ExplanationBase : ScriptableObject
{
    [System.Serializable]
    public class TextExplanation
    {
        public string text_name;
        [TextArea(1, 5)] public string explanation_japanese_text;
        [TextArea(1, 5)] public string explanation_english_text;
    }
    [SerializeField] TextExplanation[] textExplanation;
    Dictionary<string, TextExplanation> languaeDictionary = new Dictionary<string, TextExplanation>();

    void OnEnable()
    {
        foreach (var _textExplanation in textExplanation)
        {
            languaeDictionary.Add(_textExplanation.text_name, _textExplanation);
        }
    }
    public string RetrunText(string text_name, Language language)
    {
        if (languaeDictionary.TryGetValue(text_name, out TextExplanation eachLanguage))
        {
            string text = "";
            switch (language)
            {
                case Language.english: text = eachLanguage.explanation_english_text; break;
                case Language.japanese:
                    text = eachLanguage.explanation_japanese_text;
                    //text = "<size=75%>" + text;
                    break;
            }

            /*
            if (text.Contains("[MouseIcon0]")) text = text.Replace("[MouseIcon0]", "<size=100%>" + "<sprite=\"MouseIcon\" index=0>" + "</size>");
            if (text.Contains("[MouseIcon1]")) text = text.Replace("[MouseIcon1]", "<size=100%>" + "<sprite=\"MouseIcon\" index=1>" + "</size>");
            if (text.Contains("[PassiveIcon]")) text = text.Replace("[PassiveIcon]", "<size=100%>" + "<sprite=\"PassiveIcon\" index=0>" + "</size>");
            if (text.Contains("[SkillButton]")) text = text.Replace("[SkillButton]", "<size=100%>" + "[Q]" + "</size>");
            */
            return text;
        }
        else return "エラー:存在しないテキストが呼ばれました";

    }
}

