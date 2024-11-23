using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

[RequireComponent(typeof(Image))]
public class ImageColorSync : MonoBehaviour
{
    [SerializeField] Text text;
    Image origin;
    private void Awake()
    {
        origin = GetComponent<Image>();
    }
    private void OnValidate()
    {
        if (text == null) return;
        if(origin == null) origin = GetComponent<Image>();
        text.color = new Color(text.color.r, text.color.g, text.color.b, origin.color.a);
    }
    private void LateUpdate()
    {
        if (text == null) return;
        text.color = new Color(text.color.r, text.color.g, text.color.b, origin.color.a);
    }
}
