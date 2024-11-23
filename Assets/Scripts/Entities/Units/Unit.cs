using System;
using System.Collections;
using System.Collections.Generic;
using System.Diagnostics;
using UnityEngine;

public abstract class Unit : MonoBehaviour, IDamagable
{
    [Header("Unit")]
    [SerializeField] float m_maxHealth;
    [SerializeField] float m_moveSpeed;
    [SerializeField] float m_scanRange;
    [SerializeField] protected float damage;
    [SerializeField] protected Animator anim;

    
    public float maxHealth => m_maxHealth;
    public float health { get; protected set; }
    public float moveSpeed => m_moveSpeed;
    public float scanRange => m_scanRange;


    Pooler<Unit> pool;
    bool instantiated = false;
    public Alliance side { get; private set; }
    public MoveDirection moveDir { get; private set; }
    protected virtual void OnEnable()
    {
        health = maxHealth;
        dead = false;
    }
    public Unit Instantiate(Vector2 position, Alliance side, MoveDirection direction)
    {
        if (instantiated) return null;
        if (pool == null) pool = new Pooler<Unit>(this);
        Unit tmp = pool.GetObject(position, Quaternion.identity);
        tmp.instantiated = true;
        tmp.transform.localScale = new Vector2(direction == MoveDirection.Right ? 1.0f : -1.0f, 1.0f);
        tmp.side = side;
        tmp.moveDir = direction;
        tmp.pool = pool;

        return tmp;
    }
    public void Release()
    {
        if (!instantiated) return;
        pool.ReleaseObject(this);
    }

    public Action onHealthChange;
    protected bool dead = false;
    public void OnDamage(float damage)
    {
        if (dead) return;
        health = Mathf.Max(0, health - damage);
        onHealthChange?.Invoke();
        if(health <= 0)
        {
            Die();
        }
    }
    void Die()
    {
        if (dead) return;
        dead = true;
        OnDeath();
    }
    protected virtual void OnDeath()
    {

    }
    readonly int movingID = Animator.StringToHash("Moving");
    readonly int attackingID = Animator.StringToHash("Attacking");
    protected virtual void Update()
    {
        ScanEnemy();
        anim.SetBool(attackingID, scanned != null);
        if (anim.GetBool(movingID))
        {
            transform.Translate(transform.right * (moveDir == MoveDirection.Right ? 1 : -1) * moveSpeed * Time.deltaTime);
        }
    }
    protected IDamagable scanned = null;
    void ScanEnemy()
    {
        RaycastHit2D[] hit = Physics2D.RaycastAll(transform.position, transform.right * (moveDir==MoveDirection.Right ? 1 : -1), scanRange, LayerMask.GetMask("Damagable"));
        scanned = null;
        foreach (var i in hit)
        {
            UnityEngine.Debug.Log(i.transform.name);
            if (i.transform.TryGetComponent(out IDamagable tmp))
            {
                if (tmp.side != side)
                {
                    scanned = tmp;
                    break;
                }
            }
        }
    }
}
[System.Serializable]
public enum MoveDirection
{
    Right,
    Left
}