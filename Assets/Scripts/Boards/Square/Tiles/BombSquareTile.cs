using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class BombSquareTile : SquareTile
{
    [SerializeField] int range = 1;
    public override void Pop(Action<SquareTile> onPopFinish)
    {
        base.Pop(onPopFinish);
        Explode();
    }
    void Explode()
    {
        for(int i = gridPos.x - range; i <= gridPos.x + range; i++)
        {
            for(int k = gridPos.y - range; k <= gridPos.y + range; k++)
            {
                owner.PopAt(new Vector2Int(i, k));
            }
        }
    }
}