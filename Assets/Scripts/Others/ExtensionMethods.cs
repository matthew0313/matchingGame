using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Pool;
using System;

public static class ExtensionMethods
{
    public static void DrawBox(Vector2 position, Vector2 size, Color color, float duration)
    {
        Debug.DrawLine(new Vector2(position.x - size.x / 2, position.y - size.y / 2), new Vector2(position.x - size.x / 2, position.y + size.y / 2), color, duration);
        Debug.DrawLine(new Vector2(position.x - size.x / 2, position.y - size.y / 2), new Vector2(position.x + size.x / 2, position.y - size.y / 2), color, duration);
        Debug.DrawLine(new Vector2(position.x + size.x / 2, position.y + size.y / 2), new Vector2(position.x - size.x / 2, position.y + size.y / 2), color, duration);
        Debug.DrawLine(new Vector2(position.x + size.x / 2, position.y + size.y / 2), new Vector2(position.x + size.x / 2, position.y - size.y / 2), color, duration);
    }
    public static Vector2 AngleVector(float angle)
    {
        return new Vector2(Mathf.Cos(angle * Mathf.Deg2Rad), Mathf.Sin(angle * Mathf.Deg2Rad));
    }
}