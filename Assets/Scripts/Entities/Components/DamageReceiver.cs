using System;
using System.Collections;
using System.Collections.Generic;
using UnityEditor.ShaderGraph.Internal;
using UnityEngine;

public class DamageReceiver : MonoBehaviour
{
    public void Set(Alliance side)
    {
        this.side = side;
    }
    public Alliance side { get; private set; }
    public Action<float> onDamage, onKnockback;
    public void GetDamage(float damage)
    {
        if (!enabled) return;
        onDamage?.Invoke(damage);
    }
    public void GetKnockback(float knockback)
    {
        if (!enabled) return;
        onKnockback?.Invoke(knockback);
    }
}