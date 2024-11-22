using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class HorizontalSweepSquareTile : SquareTile
{
    public override void Pop(Action<SquareTile> onPopFinish)
    {
        base.Pop(onPopFinish);
        HorizontalPop();
    }
    public void HorizontalPop()
    {
        for(int i = 0; i < owner.gridSize.x; i++)
        {
            owner.PopAt(new Vector2Int(i, gridPos.y));
        }
    }
}