using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class VerticalSweepSquareTile : SquareTile
{
    public override void Pop(Action<SquareTile> onPopFinish)
    {
        base.Pop(onPopFinish);
        VerticalPop();
    }
    public void VerticalPop()
    {
        for(int i = 0; i < owner.gridSize.y; i++)
        {
            owner.PopAt(new Vector2Int(gridPos.x, i));
        }
    }
}