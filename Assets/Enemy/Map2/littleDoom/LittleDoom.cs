using System;
using System.Collections;
using System.Collections.Generic;
using UnityEditor.Rendering;
using UnityEngine;
using UnityEngine.UIElements;

public class LittleDoom : MonoBehaviour
{
    [SerializeField] private float hp;
    [SerializeField] private Rigidbody2D rb;
    public GameObject pointA;
    public GameObject pointB;
    [SerializeField] private Transform currentTransform;
    [SerializeField] private float speed;
    [SerializeField] private Animator animator;
    [SerializeField] private DmgFlash _dmgFlash;
    [SerializeField] private LayerMask playerLayer;
    [SerializeField] private float detectionRange;
    private Slash slashDmg;
    private PlayerMoveBehave player;
    private bool isMoving = true;
    private Transform playerTransform;
    private Vector3 originalPosition;
    private bool isFollowingPlayer = false;
    [SerializeField] private Vector2 rayOffSet;
    private bool isFacingRight = false;
    [SerializeField] BoxCollider2D triggerBox;
    [SerializeField] SpriteRenderer noitice;
    private bool isScaling = false;
    private bool getHit = false;
    void Start()
    {
        slashDmg = GameObject.FindGameObjectWithTag("Slash").GetComponent<Slash>();
        currentTransform = pointA.transform;
      
        originalPosition = transform.position;
        if (player == null)
        {
            player = FindObjectOfType<PlayerMoveBehave>();
            if (player == null)
            {
                Debug.LogError("Không tìm thấy đối tượng Player với component PlayerMoveBehave");
            }
        }
        noitice.transform.localScale = Vector3.zero;
    }

    // Update is called once per frame
    void Update()
    {
        if (hp <= 0)
        {
            StartCoroutine(Dying());
        }
        Flip();
    }
    private void Flip()
    {
        if (isFacingRight && rb.linearVelocity.x < 0f || rb.linearVelocity.x > 0 && !isFacingRight)
        {
            isFacingRight = !isFacingRight;
            Vector3 localScale = transform.localScale;
            localScale.x *= -1;
            transform.localScale = localScale;
        }
    }
    void FixedUpdate()
    {
        if (isMoving)
        {
            DetectPlayer();
            if (isFollowingPlayer)
            {
                FollowPlayer();
            }
            else
            {
                ReturnToPatrol();
            }
        }

    }
    private void DetectPlayer()
    {
        Vector2 Ray = (Vector2)transform.position + rayOffSet;
        Vector2 rayDirection = isFacingRight ? Vector2.right : Vector2.left;
        RaycastHit2D hit = Physics2D.Raycast(Ray, rayDirection, detectionRange, playerLayer);

        Debug.DrawRay(Ray, rayDirection * detectionRange, Color.yellow);
        if (hit.collider != null)
        {
            noitice.enabled = true;
            if (isScaling)
            {
                StartCoroutine(ScaleNoitice());
            }

            playerTransform = hit.transform;
            isFollowingPlayer = true;
        }
        else
        {
            isScaling = true;
            noitice.transform.localScale = Vector3.zero;
            noitice.enabled = false;
            isFollowingPlayer = false;
        }
    }
    private IEnumerator ScaleNoitice()
    {
        isScaling = false;
        float timeElapsed = 0f;

        while (timeElapsed < 0.1f)
        {
            noitice.transform.localScale = Vector3.Lerp(Vector3.zero, Vector3.one, timeElapsed / 0.1f);
            timeElapsed += Time.deltaTime;
            yield return null;

        }

        noitice.transform.localScale = Vector3.one;

    }
    private void FollowPlayer()
    {
        if (playerTransform != null)
        {
            Vector2 direction = (playerTransform.position - transform.position).normalized;
            if (Vector2.Distance(transform.position, playerTransform.position) > 0f)
            {
                rb.linearVelocity = new Vector2(Mathf.Sign(direction.x) * 3, 0f);
            }

        }
    }
    private void ReturnToPatrol()
    {


        if (isFollowingPlayer == false)
        {

            CheckDistance();
        }
        Patrol();





    }
    private void Patrol()
    {

        if (currentTransform == pointB.transform)
        {
            rb.linearVelocity = new Vector2(speed, 0f);
        }
        else if (currentTransform == pointA.transform)
        {
            rb.linearVelocity = new Vector2(-speed, 0f);
        }


    }
    private void CheckDistance()
    {
        if (Vector2.Distance(transform.position, currentTransform.position) < 0.5f)
        {
            if (currentTransform == pointB.transform)
            {

                currentTransform = pointA.transform;
            }
            else if (currentTransform == pointA.transform)
            {

                currentTransform = pointB.transform;
            }
        }
        if (!isFollowingPlayer)
        {
            if (Vector2.Distance(transform.position, pointA.transform.position) > Vector2.Distance(pointB.transform.position, pointA.transform.position) ||
            Vector2.Distance(transform.position, pointB.transform.position) > Vector2.Distance(pointB.transform.position, pointA.transform.position))
            {

                if (Vector2.Distance(transform.position, pointA.transform.position) < Vector2.Distance(transform.position, pointB.transform.position))
                {
                    currentTransform = pointB.transform;
                }
                else
                {
                    currentTransform = pointA.transform;
                }


            }
        }
    }
    private void OnTriggerEnter2D(Collider2D collision)
    {
        // Kiểm tra nếu va chạm là từ đối tượng có tag là "Slash" và kẻ địch chưa bị đánh
        if (collision.CompareTag("Slash") && !getHit)
        {
            if (player != null)
            {
                // Chỉ cho phép nhận sát thương nếu hướng nhìn của người chơi lớn hơn hoặc bằng 0
                if (player.getLook() == 0f)
                {
                    StartCoroutine(GetHit());
                    hp -= slashDmg.getSlashDmg(); // Trừ máu
                    _dmgFlash.CallDmgFlash();     // Hiệu ứng bị đánh
                }
                else
                {
                    // Người chơi đang nhìn theo hướng ngược lại, không nhận sát thương
                    Debug.Log("Kẻ địch không nhận sát thương vì người chơi đang nhìn theo hướng khác.");
                }
            }
        }
    }
    private IEnumerator GetHit()
    {
        getHit = true;
        isMoving = false;
        transform.position += new Vector3(0.5f * player.getLocalScaleX(), 0f, 0f);
        rb.linearVelocity = Vector2.zero;
        yield return new WaitForSeconds(0.15f);
        isMoving = true;
        yield return new WaitForSeconds(0.05f);
        getHit = false;
    }
    private IEnumerator Dying()
    {
        noitice.enabled = false;
        animator.SetBool("isDead", true);

        rb.linearVelocity = Vector2.zero;
        isMoving = false;
        GetComponent<BoxCollider2D>().enabled = false;
        triggerBox.enabled = false;
        yield return new WaitForSeconds(0.5f);
        Destroy(gameObject);

    }
    private void OnDrawGizmos()
    {
        Gizmos.DrawSphere(pointA.transform.position, 0.5f);
        Gizmos.DrawSphere(pointB.transform.position, 0.5f);
        Gizmos.DrawLine(pointA.transform.position, pointB.transform.position);

    }
}
