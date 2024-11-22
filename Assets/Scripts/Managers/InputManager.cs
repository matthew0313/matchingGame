using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class InputManager : MonoBehaviour
{
    public static InputManager Instance { get; private set; }
    public InputManager() => Instance = this;
    public static bool IsTouchDown()
    {
        if (SystemInfo.deviceType == DeviceType.Handheld) return Input.GetTouch(0).phase == TouchPhase.Began;
        else return Input.GetMouseButtonDown(0);
    }
    public static bool IsTouchOver()
    {
        if (SystemInfo.deviceType == DeviceType.Handheld) return Input.GetTouch(0).phase == TouchPhase.Ended;
        else return Input.GetMouseButton(0) == false;
    }
    public static Vector2 GetTouchPosition()
    {
        if (SystemInfo.deviceType == DeviceType.Handheld) return Input.GetTouch(0).position;
        else return Input.mousePosition;
    }
    public static Vector2 GetDrag()
    {
        if (Instance == null || !Instance.dragging) return Vector2.zero;
        else
        {
            return GetTouchPosition() - Instance.prevDragPos;
        }
    }
    bool dragging = false;
    Vector2 prevDragPos;
    private void LateUpdate()
    {
        if (dragging)
        {
            prevDragPos = GetTouchPosition();
            if (IsTouchOver()) dragging = false;
        }
        else
        {
            if (IsTouchDown() && UIScanner.CheckIfOnUI(GetTouchPosition()) == false)
            {
                prevDragPos = GetTouchPosition();
                dragging = true;
            }
        }
    }
}