using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using JetBrains.Annotations;
using System;


#if UNITY_EDITOR
using UnityEditor;
#endif

public abstract class HexTile : MonoBehaviourCanvas
{
    [SerializeField] TileType m_type;
    [SerializeField] protected Animator anim;
    public TileType type => m_type;

    Pooler<HexTile> pool;
    bool instantiated = false;

    protected HexBoard owner;

    public Vector2Int gridPos;
    private void OnValidate()
    {
        if (gameObject.CompareTag("Tile") == false) gameObject.tag = "Tile";
    }
    public HexTile Instantiate(HexBoard owner)
    {
        if (instantiated) return null;
        if (pool == null) pool = new Pooler<HexTile>(this);
        HexTile tmp = pool.GetObject(owner.transform);
        tmp.instantiated = true;
        tmp.pool = pool;
        tmp.owner = owner;
        return tmp;
    }
    readonly int poppedID = Animator.StringToHash("Popped");
    readonly int popID = Animator.StringToHash("Pop");
    readonly int spawnedID = Animator.StringToHash("Spawned");
    readonly int hintID = Animator.StringToHash("Hint");
    protected virtual void OnEnable()
    {
        anim.SetTrigger(spawnedID);
    }
    public virtual void Release()
    {
        if (!instantiated) return;
        owner.tiles[gridPos.x, gridPos.y] = null;
        pool.ReleaseObject(this);
    }
    public virtual void Pop(Action<HexTile> onPopFinish)
    {
        StartCoroutine(PopAnim(onPopFinish));
    }
    IEnumerator PopAnim(Action<HexTile> onPopFinish)
    {
        anim.SetTrigger(popID);
        while (anim.GetBool(poppedID) == false) yield return null;
        Release();
        onPopFinish?.Invoke(this);
    }

    IEnumerator moving = null;
    public void MoveTo(Vector2 position, Action<HexTile> onMoveFinish)
    {
        if (moving != null) StopCoroutine(moving);
        moving = Moving(position, onMoveFinish);
        StartCoroutine(moving);
    }
    const float moveSpeed = 500.0f;
    IEnumerator Moving(Vector2 position, Action<HexTile> onMoveFinish)
    {
        while (Vector2.Distance(transform.position, position) > 0.1f)
        {
            transform.Translate(Vector2.ClampMagnitude(((position - (Vector2)transform.position).normalized) * moveSpeed * Time.deltaTime, Vector2.Distance(transform.position, position)), Space.World);
            yield return null;
        }
        onMoveFinish?.Invoke(this);
        moving = null;
    }
    public void Hint() => anim.SetBool(hintID, true);
    public void EndHint() => anim.SetBool(hintID, false);
}