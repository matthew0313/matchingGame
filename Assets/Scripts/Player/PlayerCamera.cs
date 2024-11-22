using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PlayerCamera : MonoBehaviour
{
    [SerializeField] float moveSpeed = 1.0f;
    private void Update()
    {
        transform.Translate(new Vector2(-InputManager.GetDrag().x * moveSpeed * Time.deltaTime, 0.0f));
        transform.position = new Vector3(Mathf.Clamp(transform.position.x, GameManager.Instance.playerBase.transform.position.x, GameManager.Instance.enemyBase.transform.position.x), transform.position.y, -10.0f);
    }
}
