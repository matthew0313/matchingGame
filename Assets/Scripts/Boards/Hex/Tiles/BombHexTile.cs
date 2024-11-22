using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System;

public class BombHexTile : HexTile
{
    [SerializeField] int bombRange = 1;
    public override void Pop(Action<HexTile> onPopFinish)
    {
        base.Pop(onPopFinish);
        Search(gridPos, 0);
    }
    readonly int[] X = new int[] { 1, 0, -1, -1, 0, 1 };
    readonly int[] Y = new int[] { -1, -2, -1, 1, 2, 1 };
    void Search(Vector2Int pos, int count)
    {
        if(count == bombRange)
        {
            if (owner.OutOfBound(pos)) return;
            owner.PopAt(pos);
        }
        else
        {
            for(int i = 0; i < 6; i++)
            {
                Search(new Vector2Int(pos.x + X[i], pos.y + Y[i]), count + 1);
            }
        }
    }
}