using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Bullet : MonoBehaviour
{
    public Alliance side;
    public float moveSpeed;
    public int direction;
    public float damage;
    private void Update()
    {
        transform.position += new Vector3(direction, 0, 0)*Time.deltaTime * moveSpeed;
    }
    private void OnTriggerEnter2D(Collider2D collision)
    {
        if(collision.transform.TryGetComponent(out IDamagable tmp))
        {
            if (tmp.side != side)
            {
                tmp.OnDamage(damage);
                Destroy(gameObject);
            }
        }
    }
}
