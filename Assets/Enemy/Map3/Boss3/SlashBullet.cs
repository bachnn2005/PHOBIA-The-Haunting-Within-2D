using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.EventSystems;

public class SlashBullet : MonoBehaviour
{
    private PlayerMoveBehave player;
    [SerializeField] private float moveSpeed;
    private Rigidbody2D rb;
    // Start is called before the first frame update
    void Start()
    {
        rb = GetComponent<Rigidbody2D>();
        player = GameObject.FindGameObjectWithTag("Player").GetComponent<PlayerMoveBehave>();
        Vector2 direction = (Vector2)player.transform.position + new Vector2(0f, 2f) - (Vector2)transform.position;
        direction.Normalize();
        float angle = Mathf.Atan2(direction.y, direction.x) * Mathf.Rad2Deg;
        transform.rotation = Quaternion.Euler(0, 0, angle - 180f);
        rb.linearVelocity = direction * moveSpeed;
        Invoke("DestroyGameObject", 4f);
    }
    private void DestroyGameObject()
    {
        Destroy(gameObject);
    }
}
