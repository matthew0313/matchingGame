using System;
using System.Collections;
using System.Collections.Generic;
using System.Security.Cryptography;
using UnityEngine;
using UnityEngine.EventSystems;

public abstract class SquareTile : MonoBehaviourCanvas
{
    [SerializeField] TileType m_type;
    [SerializeField] protected Animator anim;
    public TileType type => m_type;

    Pooler<SquareTile> pool;
    bool instantiated = false;

    protected SquareBoard owner;

    public Vector2Int gridPos;
    private void OnValidate()
    {
        if(gameObject.CompareTag("Tile") == false) gameObject.tag = "Tile";
    }
    public SquareTile Instantiate(SquareBoard owner)
    {
        if (instantiated) return null;
        if (pool == null) pool = new Pooler<SquareTile>(this);
        SquareTile tmp = pool.GetObject(owner.transform);
        tmp.instantiated = true;
        tmp.pool = pool;
        tmp.owner = owner;
        return tmp;
    }
    protected virtual void OnEnable()
    {
        anim.SetTrigger("Spawned");
    }
    public virtual void Release()
    {
        if (!instantiated) return;
        owner.tiles[gridPos.x, gridPos.y] = null;
        pool.ReleaseObject(this);
    }
    
    readonly int poppedID = Animator.StringToHash("Popped");
    readonly int popID = Animator.StringToHash("Pop");
    public virtual void Pop(Action<SquareTile> onPopFinish)
    {
        StartCoroutine(PopAnim(onPopFinish));
    }
    IEnumerator PopAnim(Action<SquareTile> onPopFinish)
    {
        anim.SetTrigger(popID);
        while (anim.GetBool(poppedID) == false) yield return null;
        Release();
        onPopFinish?.Invoke(this);
    }

    IEnumerator moving = null;
    public void MoveTo(Vector2 position, Action<SquareTile> onMoveFinish)
    {
        if (moving != null) StopCoroutine(moving);
        moving = Moving(position, onMoveFinish);
        StartCoroutine(moving);
    }
    const float moveSpeed = 500.0f;
    IEnumerator Moving(Vector2 position, Action<SquareTile> onMoveFinish)
    {
        while (Vector2.Distance(transform.position, position) > 0.1f)
        {
            transform.Translate(Vector2.ClampMagnitude(((position - (Vector2)transform.position).normalized) * moveSpeed * Time.deltaTime, Vector2.Distance(transform.position, position)), Space.World);
            yield return null;
        }
        onMoveFinish?.Invoke(this);
        moving = null;
    }
}
[System.Serializable]
public enum TileType
{
    Red,
    Blue,
    Green,
    Yellow,
    Pink,
    Cyan
}