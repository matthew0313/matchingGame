using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Board : MonoBehaviourCanvas
{
    public int combo { get; protected set; } = 0;
    public Action onTilePop;

    protected const int requiredLineLength = 3;
    protected const int specialSpawnLineLength = 4;
}