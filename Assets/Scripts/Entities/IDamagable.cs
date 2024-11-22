using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public interface IDamagable
{
    public Alliance side { get; }
    public void OnDamage(float damage);
}
[System.Serializable]
public enum Alliance
{
    Player,
    Enemy,
    None
}