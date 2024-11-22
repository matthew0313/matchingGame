using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System;

public class SweepHexTile : HexTile
{
    [SerializeField] Vector2Int sweepDir;
    public override void Pop(Action<HexTile> onPopFinish)
    {
        base.Pop(onPopFinish);
        for (int i = 1; !owner.OutOfBound(gridPos + sweepDir * i); i++)
        {
            owner.PopAt(gridPos + sweepDir * i);
        }
        for (int i = -1; !owner.OutOfBound(gridPos + sweepDir * i); i--)
        {
            owner.PopAt(gridPos + sweepDir * i);
        }
    }
}