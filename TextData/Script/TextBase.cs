using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using LanguageData;

[CreateAssetMenu(fileName = "Data", menuName = "ScriptableObjects/TextData")]
public class TextBase : ScriptableObject
{
    [System.Serializable]
    public class EachLanguage
    {
        public string japanese_text;
        public string english_text;
    }
    [SerializeField] EachLanguage[] eachLanguages;
    Dictionary<string, EachLanguage> languaeDictionary = new Dictionary<string, EachLanguage>();

    void OnEnable()
    {
        foreach (var eachLanguage in eachLanguages)
        {
            languaeDictionary.Add(eachLanguage.english_text, eachLanguage);
        }
    }
    public string RetrunText(string english_text, Language language)
    {
        if (languaeDictionary.TryGetValue(english_text, out EachLanguage eachLanguage))
        {
            string text = "";
            switch (language)
            {
                case Language.english: text = eachLanguage.english_text; break;
                case Language.japanese: text = eachLanguage.japanese_text; break;
            }
            return text;
        }
        else return "エラー:存在しないテキストが呼ばれました";

    }
}
