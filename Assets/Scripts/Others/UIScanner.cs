using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.EventSystems;

public class UIScanner : MonoBehaviour
{
    static List<RaycastResult> results = new();
    public static bool FindWithTag(Vector2 position, string tag, out GameObject found)
    {
        ScanAt(position);
        found = results.Find((RaycastResult tmp) => tmp.gameObject.CompareTag(tag)).gameObject;
        if (found != null) return true;
        else return false;
    }
    public static bool CheckIfOnUI(Vector2 position)
    {
        ScanAt(position);
        return results.Count > 0;
    }
    static void ScanAt(Vector2 position)
    {
        EventSystem.current.RaycastAll(new PointerEventData(EventSystem.current) { position = position }, results);
    }
}