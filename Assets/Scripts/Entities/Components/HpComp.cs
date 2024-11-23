using System;
using System.Collections;
using System.Collections.Generic;
using UnityEditor.ShaderGraph.Internal;
using UnityEngine;

public class HpComp : MonoBehaviour
{
    [SerializeField] float m_maxHealth;
    [SerializeField] int m_targetPriority = 0;
    public float maxHealth => m_maxHealth;
    public float health { get; private set; }
    public int targetPriority { get; private set; }
    public Alliance side { get; private set; }
    private void OnEnable()
    {
        health = maxHealth;
    }
    public void Set(Alliance side)
    {
        this.side = side;
        dead = false;
    }
    public Action<float> onDamage;
    public Action onDeath;
    public bool dead { get; private set; } = false;
    public void GetDamage(float damage)
    {
        if (dead) return;
        health = Mathf.Max(0, health - damage);
        onDamage?.Invoke(health);
        if(health <= 0)
        {
            dead = true;
            onDeath?.Invoke();
        }
    }
}