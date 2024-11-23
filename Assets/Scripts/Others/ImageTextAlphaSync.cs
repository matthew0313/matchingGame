using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
#if UNITY_EDITOR
using UnityEditor;
#endif

[RequireComponent(typeof(Image))]
public class ImageTextAlphaSync : MonoBehaviour
{
    [SerializeField] Text text;
    Image origin;
    private void Awake()
    {
        origin = GetComponent<Image>();
    }
    private void LateUpdate()
    {
        if (text == null) return;
        text.color = new Color(text.color.r, text.color.g, text.color.b, origin.color.a);
    }
    public void Sync()
    {
        if (text == null) return;
        if(origin == null) origin = GetComponent<Image>();
        text.color = new Color(text.color.r, text.color.g, text.color.b, origin.color.a);
    }
}
#if UNITY_EDITOR
[CustomEditor(typeof(ImageTextAlphaSync))]
public class ImageTextAlphaSync_Editor : Editor
{
    public override void OnInspectorGUI()
    {
        base.OnInspectorGUI();
        if (GUILayout.Button("Sync")) (target as ImageTextAlphaSync).Sync();
    }
}
#endif
