using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public interface IKnockable : IDamagable
{
    public void ApplyKnockback(float knockback) { }
}