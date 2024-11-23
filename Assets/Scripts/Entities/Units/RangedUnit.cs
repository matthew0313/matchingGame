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
            
            print("123");
            obj.side = side;
            obj.damage = damage;
            obj.direction = moveDir == MoveDirection.Right ? 1 : -1;
        }
    }
}
