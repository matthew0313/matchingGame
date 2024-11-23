using System;
using System.Collections;
using System.Collections.Generic;
using Unity.VisualScripting;
using UnityEngine;
public abstract class Unit : MonoBehaviour, IKnockable
{
    [Header("Unit")]
    [SerializeField] float m_maxHealth;
    [SerializeField] float m_moveSpeed;
    [SerializeField] float m_scanRange;
    [SerializeField] protected float damage, knockbackResistance = 1.0f;
    [SerializeField] protected int hitPriority;
    [SerializeField] protected Animator anim;

    public float maxHealth => m_maxHealth;
    public float health { get; protected set; }
    public float moveSpeed => m_moveSpeed;
    public float scanRange => m_scanRange;

    protected virtual bool knockbackImmune => false;
    public Alliance side { get; protected set; }

    MoveDirection m_moveDir;
    protected MoveDirection moveDir
    {
        get { return m_moveDir; }
        set
        {
            m_moveDir = value;
            if (m_moveDir == MoveDirection.Right) transform.localScale = new Vector2(1.0f, 1.0f);
            else if(m_moveDir == MoveDirection.Left) transform.localScale = new Vector2(-1.0f, 1.0f);
        }
    }
    protected virtual void Awake()
    {
        health = maxHealth;
    }
    public void Set(Alliance side, MoveDirection direction)
    {
        this.side = side;
        moveDir = direction;
    }

    public Action onHealthChange;
    protected bool dead = false;
    readonly int deadID = Animator.StringToHash("Dead");
    public virtual void OnDamage(float damage)
    {
        if (dead) return;
        health = Mathf.Max(health - damage, 0);
        if (health <= 0)
        {
            dead = true;
            OnDeath();
        }
    }
    public void OnKnockback(float knockback)
    {
        if (knockbackImmune) return;
        knockbackForce = knockback;
    }
    protected virtual void OnDeath()
    {
        anim.SetTrigger(deadID);
    }
    protected readonly int movingID = Animator.StringToHash("Moving");
    protected readonly int attackingID = Animator.StringToHash("Attacking");
    protected readonly int knockbackID = Animator.StringToHash("Knockback");
    float knockbackForce = 0.0f;
    protected virtual void Update()
    {
        if (dead || !GameManager.Instance.gameInProgress)
        {
            return;
        }
        if(knockbackForce > 0.0f)
        {
            transform.Translate(transform.right * (moveDir == MoveDirection.Right ? 1 : -1) * -1.0f * knockbackForce * Time.deltaTime);
            knockbackForce = Mathf.Max(0, knockbackForce - Time.deltaTime);
        }
        anim.SetBool(knockbackID, knockbackForce > 0.0f);
        ScanEnemy();
        anim.SetBool(attackingID, scanned != null);
        if (anim.GetBool(movingID) && scanned == null)
        {
            transform.Translate(transform.right * (moveDir == MoveDirection.Right ? 1 : -1) * moveSpeed * Time.deltaTime);
        }
    }
    protected IDamagable scanned = null;
    void ScanEnemy()
    {
        RaycastHit2D[] hit = Physics2D.RaycastAll(transform.position, transform.right * (moveDir==MoveDirection.Right ? 1 : -1), scanRange, LayerMask.GetMask("Damagable"));
        scanned = null;
        List<IDamagable> scannedList = new();
        foreach (var i in hit)
        {
            if (i.transform.TryGetComponent(out IDamagable tmp) && tmp.side != side)
            {
                scannedList.Add(tmp);
            }
        }
        scannedList.Sort((a, b) => b.priority.CompareTo(a.priority));
        if (scannedList.Count > 0) scanned = scannedList[0];
    }
    public void DestroySelf() => Destroy(gameObject);
}
[System.Serializable]
public enum MoveDirection
{
    Right,
    Left
}