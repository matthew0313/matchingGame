using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
#if UNITY_EDITOR
using UnityEditor;
#endif
public class BroadFontSwitcher : MonoBehaviour
{
    [SerializeField] Font font;
    public void ChangeText()
    {
        foreach (var i in FindObjectsOfType<Text>()) i.font = font;
    }
}
#if UNITY_EDITOR
[CustomEditor(typeof(BroadFontSwitcher))]
public class BroadFontSwitcher_Editor : Editor
{
    public override void OnInspectorGUI()
    {
        base.OnInspectorGUI();
        if (GUILayout.Button("Change Font")) (target as BroadFontSwitcher).ChangeText();
    }
}
#endif