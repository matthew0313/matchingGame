using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class TestUnit : Unit
{
    public void Attack()
    {
        Debug.Log("Attack");
        if (scanned != null) scanned.OnDamage(damage);
    }
    protected override void OnDeath()
    {
        base.OnDeath();
        Release();
    }
}