using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public interface IDamagable
{
    public Alliance side { get; }
    public virtual int priority => 0;
    public void OnDamage(float damage);
}
