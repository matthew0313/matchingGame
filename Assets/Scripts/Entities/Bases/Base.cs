using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public abstract class Base : MonoBehaviour, IDamagable
{
    [SerializeField] float m_maxHealth;
    [SerializeField] Transform spawnPoint;
    [SerializeField] MoveDirection direction;
    public abstract Alliance side { get; }
    public float maxHealth => m_maxHealth;
    public float health { get; private set; }
    void Awake()
    {
        health = maxHealth;
    }
    bool destroyed = false;
    public Action onHpChange;
    public void OnDamage(float damage)
    {
        if (destroyed) return;
        health = Mathf.Max(0, health - damage);
        onHpChange?.Invoke();
        if(health <= 0)
        {
            Die();
        }
    }
    void Die()
    {
        if (destroyed) return;
        destroyed = true;
        OnDeath();
    }
    protected virtual void OnDeath()
    {

    }
    public void SpawnUnit(Unit prefab)
    {
        if (destroyed) return;
        prefab.Instantiate(spawnPoint.position, side, direction);
    }
}