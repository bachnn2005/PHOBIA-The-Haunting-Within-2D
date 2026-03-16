using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class HoneyBullet : MonoBehaviour
{
    // Start is called before the first frame update
    [SerializeField] private GameObject honeyGround;
    [SerializeField] private Vector2 honeyGroundOffset;
    [SerializeField] private Transform target;
    [SerializeField] private float moveSpeed;
    private Animator animator;
    private Rigidbody2D rb;
    void Start()
    {
        target = GameObject.FindGameObjectWithTag("Player").transform;
        animator = GetComponent<Animator>();    
        rb = GetComponent<Rigidbody2D>();
        Vector2 moveDirection = (target.position - transform.position).normalized*moveSpeed;
        rb.linearVelocity = moveDirection;

    }

    // Update is called once per frame
    void Update()
    {
        
    }
    private void OnTriggerEnter2D(Collider2D collision)
    {
        if(collision.CompareTag("Obstacle"))
        {   
            rb.linearVelocity = Vector2.zero;
            Vector2 collisionPoint = collision.ClosestPoint(transform.position)+ honeyGroundOffset;
            StartCoroutine(ColliseObstacle(collisionPoint));
            return;
        }
        if(collision.CompareTag("Slash"))
        {
            Destroy(gameObject); 
            return;
        }
    }
    private IEnumerator ColliseObstacle(Vector2 collision)
    {
        animator.SetBool("isCollised", true);
        Instantiate(honeyGround, collision, Quaternion.identity);
        yield return new WaitForSeconds(0.5f);
        Destroy(gameObject);
        
    }
}
