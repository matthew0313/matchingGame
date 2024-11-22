using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Pool;
using System;

[RequireComponent(typeof(RectTransform))]
public class MonoBehaviourCanvas : MonoBehaviour
{
    public RectTransform rectTransform => transform as RectTransform;
}