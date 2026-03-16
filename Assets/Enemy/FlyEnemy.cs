using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class FlyEnemy : MonoBehaviour
{
    [SerializeField] private PlayerMoveBehave player;
    [SerializeField] private float lineOfSite;
    [SerializeField] private float detectRange;
    [SerializeField] private float moveSpeed;
    [SerializeField] private Vector2 targetOffset;
    [SerializeField] private LayerMask obstacleLayer;
    [SerializeField] private float dectectObstacleRange;
    [SerializeField] private Vector3 upOffset;
    [SerializeField] private Vector3 downOffset;
    [SerializeField] private SpriteRenderer noitice;
    private bool isScaling = false;
    private bool isPursuing;
    private float pursuingDuration = 2f;
    private float pursuingCounterTime;
    private Vector2 target;
    private bool isFacingRight;
    private bool isMoving = true;
    private bool isDying = false;
    // Start is called before the first frame update
    void Start()
    {
        noitice.transform.localScale = Vector3.zero;
        player = FindAnyObjectByType<PlayerMoveBehave>();
    }

    // Update is called once per frame
    void Update()
    {
        if(!isMoving)
        {
            if(isDying)
            {
                noitice.enabled = false;
            }
            return;
        }
        Debug.DrawRay(transform.position, Vector2.up * dectectObstacleRange, Color.red);
        Debug.DrawRay(transform.position, Vector2.down * 0.25f, Color.red);
        target = (Vector2)player.transform.position + targetOffset;
        Pursuing();
        float distance = Vector2.Distance(target, transform.position);
        if (distance < lineOfSite && isPursuing)
        {
            if(!isScaling)
            {
                StartCoroutine(ScaleNoitice());
                isScaling = true;
            }
            Flip();
            if (ObstacleAhead())
            {
                AvoidObstacle();
            }
            else
            {
                transform.position = Vector2.MoveTowards(transform.position, target, moveSpeed * Time.deltaTime);
            }

        }
        else
        {
            noitice.transform.localScale = Vector3.zero;
            isScaling = false;
            pursuingCounterTime -= Time.deltaTime;
            if (pursuingCounterTime <= 0)
            {
                isPursuing = false;
            }
        }

    }
    private void Flip()
    {
        if (transform.position.x - target.x < -0.2f)
        {
            isFacingRight = true;

            transform.localScale = new Vector3(-1f, 1f, 1f);
        }
        else if (transform.position.x - target.x > 0.2f)
        {
            isFacingRight = false;
            transform.localScale = Vector3.one;
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
    #region avoid Obstacle
    private bool ObstacleAhead()
    {
        Vector2 direction = isFacingRight ? Vector2.right : Vector2.left;
        Debug.DrawRay(transform.position + upOffset, direction * dectectObstacleRange, Color.red);
        Debug.DrawRay(transform.position + downOffset, direction * dectectObstacleRange, Color.red);
        RaycastHit2D hitSideUp = Physics2D.Raycast(transform.position + upOffset, direction, dectectObstacleRange, obstacleLayer);
        RaycastHit2D hitSideDown = Physics2D.Raycast(transform.position + downOffset, direction, dectectObstacleRange, obstacleLayer);
        RaycastHit2D hitDown = Physics2D.Raycast(transform.position, Vector2.down, 0.25f, obstacleLayer);
        RaycastHit2D hitUp = Physics2D.Raycast(transform.position, Vector2.up, dectectObstacleRange, obstacleLayer);
        return hitSideUp.collider != null || hitUp.collider != null || hitDown.collider != null || hitSideDown.collider != null;
    }
    private void AvoidObstacle()
    {
        Vector2 direction = isFacingRight ? Vector2.right : Vector2.left;
        RaycastHit2D hitSideUp = Physics2D.Raycast(transform.position + upOffset, direction, dectectObstacleRange, obstacleLayer);
        RaycastHit2D hitSideDown = Physics2D.Raycast(transform.position + downOffset, direction, dectectObstacleRange, obstacleLayer);
        RaycastHit2D hitDown = Physics2D.Raycast(transform.position, Vector2.down, 0.25f, obstacleLayer);
        RaycastHit2D hitUp = Physics2D.Raycast(transform.position, Vector2.up, dectectObstacleRange, obstacleLayer);

        if (hitSideUp.collider != null || hitSideDown.collider != null)
        {
            if (hitSideUp.collider != null && hitSideDown.collider != null)
            {
                if (transform.position.y > target.y)
                {
                    transform.position += Vector3.down * moveSpeed * Time.deltaTime;
                }
                else
                {
                    transform.position += Vector3.up * moveSpeed * Time.deltaTime;
                }
            }
            else if (hitSideUp.collider != null && hitSideDown.collider == null)
            {
                transform.position += Vector3.down * moveSpeed * Time.deltaTime;
            }
            else if (hitSideUp.collider == null && hitSideDown.collider != null)
            {
                if (transform.position.y > target.y)
                {
                    transform.position += Vector3.down * moveSpeed * Time.deltaTime;
                }
                else
                {
                    transform.position += Vector3.up * moveSpeed * Time.deltaTime;
                }

            }

        }
        if (hitUp.collider != null)
        {
            if (transform.position.y <= target.y)
            {
                if (transform.position.x <= target.x)
                {
                    transform.position += Vector3.right * moveSpeed * Time.deltaTime;
                }
                else
                {
                    transform.position += Vector3.left * moveSpeed * Time.deltaTime;
                }
            }
            else
            {
                transform.position += Vector3.down * moveSpeed * Time.deltaTime;
            }

        }
        if (hitDown.collider != null)
        {
            transform.position += Vector3.up * moveSpeed * Time.deltaTime;
            if (transform.position.y > target.y)
            {
                transform.position += Vector3.right * moveSpeed * Time.deltaTime;
            }
        }

    }
    #endregion
    private void Pursuing()
    {
        if (Vector2.Distance(target, transform.position) < detectRange)
        {
            isPursuing = true;
            pursuingCounterTime = pursuingDuration;
        }

    }
    private void OnDrawGizmosSelected()
    {
        Gizmos.DrawWireSphere(target, 0.25f);
        Gizmos.DrawWireSphere(transform.position, detectRange);
        Gizmos.color = Color.yellow;
        Gizmos.DrawWireSphere(transform.position, lineOfSite);
    }
    public void SetFalseIsMoving()
    {
        isMoving = false;
    }
    public void SetTrueIsMoving()
    {
        isMoving = true;
    }
    public void SetTrueIsDying()
    {
        isDying = true;
    }
}