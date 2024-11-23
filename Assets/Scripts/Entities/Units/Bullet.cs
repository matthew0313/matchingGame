using System.Collections;
using System.Collections.Generic;
using UnityEditor.Experimental.GraphView;
using UnityEngine;

public class Bullet : MonoBehaviour
{
    public float moveSpeed;
    
    public int direction;

    public float damage;
    private void Update()
    {
        transform.position += new Vector3(direction, 0, 0)*Time.deltaTime * moveSpeed;
    }
    private void OnTriggerEnter2D(Collider2D collision)
    {
        
        Unit tmp = collision.transform.GetComponent<Unit>();
        if (tmp == null)
        {
            return;
        }
        if (tmp.side == Alliance.Enemy)
        {
            tmp.OnDamage(damage);
            Destroy(gameObject);
        }

    }
}
