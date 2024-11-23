using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class BlueDragon : Unit
{
    [SerializeField] float knockback = 3.0f;
    public void Attack()
    {
        if(scanned != null)
        {
            scanned.OnDamage(damage);
            if (scanned is IKnockable) (scanned as IKnockable).OnKnockback(knockback);
        }
    }
    protected override void OnDeath()
    {
        base.OnDeath();
        Release();
    }
}
