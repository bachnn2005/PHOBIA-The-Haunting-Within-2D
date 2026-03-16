using System.Collections;
using System.Collections.Generic;
using Unity.Collections.LowLevel.Unsafe;
using UnityEngine;
using UnityEngine.Animations;
using UnityEngine.Rendering.Universal;

public class nump : MonoBehaviour { 
 [SerializeField] private float hp;
[SerializeField] private Rigidbody2D rb;
public GameObject pointA;
public GameObject pointB;
    public GameObject checkpointC;
    public GameObject checkpointD;
    [SerializeField] private Transform currentTransform;
[SerializeField] private float speed;
[SerializeField] private DmgFlash _dmgFlash;
[SerializeField] private LayerMask playerLayer;
    [SerializeField] private Animator animator;
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
    private void FixedUpdate()
    {
        if (!isMoving) // Không làm gì nếu isMoving = false
        {
            rb.linearVelocity = Vector2.zero;
            return;
        }

        DetectPlayer(); // Kiểm tra người chơi nếu đang tuần tra

        if (isFollowingPlayer)
        {
            StartCoroutine(DelayBeforeDash());
        }
        else
        {
            ReturnToPatrol(); // Tuần tra khi không phát hiện người chơi
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
            animator.SetBool("isDash", true);
            isFollowingPlayer = false;
            isMoving = false;

            // Đảm bảo ngừng mọi vận tốc
            
            rb.linearVelocity = Vector2.zero;


            noitice.enabled = true;
            if (isScaling)
            {
                StartCoroutine(ScaleNoitice());
            }

            // Bắt đầu chuẩn bị dash sau khi phát hiện người chơi
            StartCoroutine(DelayBeforeDash());
        }
        else
        {
            animator.SetBool("isDash", false);
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
    private IEnumerator DelayBeforeDash()
    {
       
        // Ngừng hoạt động, không tuần tra hoặc phát hiện trong khi chờ 1 giây trước khi dash
        isMoving = false;

        // Dừng vận tốc của Rigidbody2D để đảm bảo không trôi
        rb.linearVelocity = Vector2.zero;
        
        // Đợi 1 giây trước khi thực hiện dash
        yield return new WaitForSeconds(1f);

     
            if (!isFollowingPlayer) // Dash ngay lập tức mà không cần kiểm tra khoảng cách hay hướng
            {
                StartCoroutine(DashToPosition());
            }
        
        
    }

    private IEnumerator DashToPosition()
    {
        float dashDistance = 5f;
        Vector2 startPosition = transform.position;

        Vector2 dashTarget = startPosition + (isFacingRight ? Vector2.right : Vector2.left) * dashDistance;

        float dashTime = 0.4f;
        float elapsedTime = 0f;

        float stopThreshold = 0.5f; // Thử tăng ngưỡng dừng

        bool reachedCheckpoint = false;

        while (elapsedTime < dashTime)
        {
            transform.position = Vector2.Lerp(startPosition, dashTarget, elapsedTime / dashTime);

            if (Vector2.Distance(transform.position, checkpointC.transform.position) <= stopThreshold)
            {
                reachedCheckpoint = true;
                rb.linearVelocity = Vector2.zero; // Dừng di chuyển
                transform.position = checkpointC.transform.position; // Đặt vị trí tại checkpointC
                break;
            }

            if (Vector2.Distance(transform.position, checkpointD.transform.position) <= stopThreshold)
            {
                reachedCheckpoint = true;
                rb.linearVelocity = Vector2.zero; // Dừng di chuyển
                transform.position = checkpointD.transform.position; // Đặt vị trí tại checkpointD
                break;
            }

            elapsedTime += Time.deltaTime;
            yield return null;
        }

        if (!reachedCheckpoint)
        {
            transform.position = dashTarget;
        }

        rb.linearVelocity = Vector2.zero; // Đảm bảo nhân vật dừng hẳn
        noitice.transform.localScale = Vector3.zero;
        noitice.enabled = false;
        isFollowingPlayer = false;

        isMoving = false;
        yield return new WaitForSeconds(2f);

        isMoving = true;
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
        if (!isMoving) // Không thực hiện tuần tra nếu isMoving = false
        {
            animator.SetBool("isWalking", false);
            rb.linearVelocity = Vector2.zero;
            return;
        }
        animator.SetBool("isWalking", true);

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

    if (collision.CompareTag("Slash") && !getHit)
    {
        if (player != null)
        {

            StartCoroutine(GetHit());
            hp -= slashDmg.getSlashDmg();
            _dmgFlash.CallDmgFlash();
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
        animator.SetBool("isDead", true);
        noitice.enabled = false;
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
        Gizmos.DrawSphere(checkpointC.transform.position, 0.5f);
        Gizmos.DrawSphere(checkpointD.transform.position, 0.5f);
        Gizmos.DrawLine(pointA.transform.position, pointB.transform.position);

}


}
