using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public interface IKnockable : IDamagable
{
    public void OnKnockback(float knockback);
}
[System.Serializable]
public enum Alliance
{
    Player,
    Enemy,
    None
}