using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class TestUnit : Unit
{
    public void Attack()
    {
        if (scanned != null) scanned.OnDamage(damage);
    }
}