using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class RangedUnit : Unit
{
    [SerializeField] Bullet bullet;
    public void Attack()
    {
        if(scanned != null)
        {
            var obj = Instantiate(bullet);
            obj.transform.position = transform.position;
            obj.side = side;
            obj.damage = damage;
            obj.direction = moveDir == MoveDirection.Right ? 1 : -1;
        }
    }
    protected override void OnDeath()
    {
        base.OnDeath();
        Release();
    }
}
