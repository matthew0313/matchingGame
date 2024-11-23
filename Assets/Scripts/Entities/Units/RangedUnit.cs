using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class RangedUnit : Unit
{
    [SerializeField] GameObject bullet;
    public void Attack()
    {
        if(scanned != null)
        {
            var obj = Instantiate(bullet);
            obj.transform.position = transform.position;
            Debug.Log("ÃÑ¾Ë »ý¼º");
            Bullet tmp = bullet.GetComponent<Bullet>();
            tmp.damage = damage;
            tmp.direction = moveDir == MoveDirection.Right ? 1 : -1;
           
        }
    }
    protected override void OnDeath()
    {
        base.OnDeath();
        Release();
    }
}
