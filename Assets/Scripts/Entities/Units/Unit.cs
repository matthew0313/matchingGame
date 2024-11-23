using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Experimental.AI;

[RequireComponent(typeof(HpComp))]
public abstract class Unit : MonoBehaviour
{
    [Header("Unit")]
    [SerializeField] float m_maxHealth;
    [SerializeField] float m_moveSpeed;
    [SerializeField] float m_scanRange;
    [SerializeField] protected float damage;
    [SerializeField] protected int hitPriority;
    [SerializeField] protected Animator anim;
    protected HpComp hp;

    
    public float maxHealth => m_maxHealth;
    public float health { get; protected set; }
    public float moveSpeed => m_moveSpeed;
    public float scanRange => m_scanRange;


    Pooler<Unit> pool;
    bool instantiated = false;
    protected Alliance side => hp.side;

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
    private void Awake()
    {
        hp = GetComponent<HpComp>();
        hp.onDeath += OnDeath;
    }
    void Set(Alliance side, MoveDirection direction)
    {
        health = maxHealth;
        dead = false;
        hp.Set(side);
        moveDir = direction;
    }
    public virtual Unit Instantiate(Vector2 position, Alliance side, MoveDirection direction)
    {
        if (instantiated) return null;
        if (pool == null) pool = new Pooler<Unit>(this);
        Unit tmp = pool.GetObject(position, Quaternion.identity);
        tmp.instantiated = true;
        tmp.pool = pool;
        tmp.Set(side, direction);

        return tmp;
    }
    public virtual void Release()
    {
        if (!instantiated) return;
        pool.ReleaseObject(this);
    }

    public Action onHealthChange;
    protected bool dead = false;
    readonly int deadID = Animator.StringToHash("Dead");
    protected virtual void OnDeath()
    {
        anim.SetTrigger(deadID);
    }
    readonly int movingID = Animator.StringToHash("Moving");
    readonly int attackingID = Animator.StringToHash("Attacking");
    protected virtual void Update()
    {
        if (dead || !GameManager.Instance.gameInProgress)
        {
            Debug.Log("returned");
            return;
        }
        ScanEnemy();
        anim.SetBool(attackingID, scanned != null);
        if (anim.GetBool(movingID))
        {
            transform.Translate(transform.right * (moveDir == MoveDirection.Right ? 1 : -1) * moveSpeed * Time.deltaTime);
        }
    }
    protected HpComp scanned = null;
    void ScanEnemy()
    {
        RaycastHit2D[] hit = Physics2D.RaycastAll(transform.position, transform.right * (moveDir==MoveDirection.Right ? 1 : -1), scanRange, LayerMask.GetMask("Damagable"));
        scanned = null;
        List<HpComp> scannedList = new();
        foreach (var i in hit)
        {
            if (i.transform.TryGetComponent(out HpComp tmp) && tmp.side != side)
            {
                scannedList.Add(tmp);
            }
        }
        scannedList.Sort((a, b) => b.targetPriority.CompareTo(a.targetPriority));
        if (scannedList.Count > 0) scanned = scannedList[0];
    }
}
[System.Serializable]
public enum MoveDirection
{
    Right,
    Left
}