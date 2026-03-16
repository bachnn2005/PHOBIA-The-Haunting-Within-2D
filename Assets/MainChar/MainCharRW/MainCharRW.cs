using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class MainCharRW : MonoBehaviour
{
    // Start is called before the first frame update
    [SerializeField] private float move;
    [SerializeField] private Rigidbody2D rb;
    [SerializeField] private float moveSpeed;
    public bool isFacingRight = false;
    [SerializeField] private Animator animator;
    private bool canMove = true;
    void Start()
    {
        
    }

    // Update is called once per frame
    void Update()
    {
        if(!canMove)
        {
            rb.linearVelocity = Vector2.zero;
            animator.SetBool("isWalking", false);
            return;
        }
        move = Input.GetAxis("Horizontal");
        rb.linearVelocity = new Vector2(move * moveSpeed, rb.linearVelocity.y);
        Flip();
        if(Mathf.Abs(move) > 0)
        {
            animator.SetBool("isWalking", true);
        }
        else
        {
            animator.SetBool("isWalking", false);
        }

    }
    private void Flip()
    {
        if (isFacingRight && move < 0 || move > 0 && !isFacingRight)
        {
            isFacingRight = !isFacingRight;
            Vector3 localScale = transform.localScale;
            localScale.x *= -1;
            transform.localScale = localScale;
        }
    }
    private void FixedUpdate()
    {

    }
    public void SetfalseCanMove()
    {
        canMove = false;
    }
    public void SetTrueCanMove()
    {
        canMove = true;
    }
    public bool GetIsFacingRight()
    {
        return isFacingRight;
    }
}
