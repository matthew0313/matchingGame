using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Haechi : Unit
{
    [SerializeField] float damageLimit;
    [SerializeField] float weighIn;

    private bool isAttack;

    private float tmpDamage;
    private IDamagable tmpScanned;
    private void Start()
    {
        tmpDamage = damage;
        
    }
    public void Attack()
    {
        if (scanned != null && scanned == tmpScanned)
        {
            scanned.OnDamage(damage);
            isAttack = true;
            if(damage < damageLimit) damage += weighIn;

        }
        else
        {
            tmpScanned = scanned;
            damage = tmpDamage;
        }

    }

    
}
