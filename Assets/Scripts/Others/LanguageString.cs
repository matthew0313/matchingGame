using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[System.Serializable]
public struct LanguageString
{
    [SerializeField][TextArea(1, 2)] string korean, english;
    public string content
    {
        get
        {
            switch (GlobalManager.Instance.settings.language)
            {
                case Language.English: return english;
                default: return korean;
            }
        }
    }
}
[System.Serializable]
public enum Language
{
    Korean,
    English
}