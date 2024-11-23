using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

[RequireComponent(typeof(Text))]
public class LanguageStringText : MonoBehaviour
{
    [SerializeField] LanguageString text;
    Text origin;
    private void Awake()
    {
        origin = GetComponent<Text>();
        GlobalManager.Instance.onLanguageChange += TextUpdate;
        TextUpdate();
    }
    void TextUpdate() => origin.text = text.content;
    private void OnDestroy()
    {
        GlobalManager.Instance.onLanguageChange -= TextUpdate;
    }
}