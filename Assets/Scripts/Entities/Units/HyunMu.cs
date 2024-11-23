using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class HyunMu : Unit
{
    [SerializeField] float guardReduction = 0.5f;
    public override void OnDamage(float damage)
    {
        if (anim.GetBool(attackingID)) base.OnDamage(damage * 0.5f);
        else base.OnDamage(damage);
    }
}
