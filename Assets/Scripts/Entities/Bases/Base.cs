using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public abstract class Base : MonoBehaviour, IDamagable
{
    [SerializeField] float m_maxHealth;
    [SerializeField] Transform spawnPoint;
    [SerializeField] MoveDirection direction;
    public float maxHealth => m_maxHealth;
    public float health { get; private set; }
    public abstract Alliance side { get; }
    protected virtual void Awake()
    {
        health = maxHealth;
    }
    protected bool dead { get; private set; } = false;
    public void OnDamage(float damage)
    {
        if (dead) return;
        health = Mathf.Max(0, health - damage);
        if(health <= 0)
        {
            dead = true;
            OnDeath();
        }
    }
    protected virtual void OnDeath()
    {

    }
    public void SpawnUnit(Unit prefab)
    {
        if (dead) return;
        Instantiate(prefab, spawnPoint.position, Quaternion.identity).Set(side, direction);
    }
}