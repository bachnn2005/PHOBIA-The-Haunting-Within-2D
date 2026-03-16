using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Rendering.Universal;

public class MutantMurshroom : MonoBehaviour
{
    [SerializeField] private float hp;
    [SerializeField] private Animator animator;
    [SerializeField] private DmgFlash _dmgFlash;
    [SerializeField] private PlayerMoveBehave player;
    [SerializeField] private float moveSpeed;
    [SerializeField] private float lineOfSite;
    [SerializeField] private Rigidbody2D rb;
    [SerializeField] private LayerMask whatIsPlayer;
    [SerializeField] private Vector2 rayOffSet;
    [SerializeField] private float rayDistance;
    [SerializeField] PolygonCollider2D triggerBox;
    [SerializeField] SpriteRenderer noitice;
    private Slash slashDmg;
    private bool isMoving = true;
    private bool isScaling = false;
    private bool getHit = false;
    private bool isPursuing;
    private float pursuingDuration = 2f;
    private float pursuingTime;
    private bool isFacingRight;
    // Start is called before the first frame update
    void Start()
    {
        slashDmg = GameObject.FindGameObjectWithTag("Slash").GetComponent<Slash>(); 
        player = FindAnyObjectByType<PlayerMoveBehave>();
        noitice.transform.localScale = Vector3.zero;
    }

    // Update is called once per frame
    void Update()
    {
        if(!isMoving)
        {
            return;
        }
        if(hp <= 0)
        {
            StartCoroutine(Dying());
        }
        Flip();
        if(Pursuing())
        {
            if(!isScaling)
            {
                StartCoroutine(ScaleNoitice());
                StartCoroutine(Noitice());
            }
            isPursuing = true;
        }
        float distance = Vector2.Distance(player.transform.position, transform.position);
        if ( distance < lineOfSite && isPursuing)
        {
            pursuingTime = pursuingDuration;
            float direction = 0f;
            if((player.transform.position.x - transform.position.x) > 0.25f )
            {
                direction = 1;
            }
            else if ((player.transform.position.x - transform.position.x) < -0.25f) {
                direction = -1;
            }
            rb.linearVelocity = new Vector2(direction * moveSpeed, rb.linearVelocity.y);
            isScaling = true;
            animator.SetBool("isWalking", true);
        }
        else
        {
            pursuingTime -= Time.deltaTime;
            if(pursuingTime < 0f)
            {
                isScaling = false;
                noitice.transform.localScale = Vector3.zero;
                isPursuing = false;
                animator.SetBool("isWalking", false);
            }
        }
    }
    private void Flip()
    {
        if (rb.linearVelocity.x < -1f && isFacingRight || rb.linearVelocity.x > 1f && !isFacingRight) 
        {
            isFacingRight = !isFacingRight;
            Vector3 localScale = transform.localScale;
            localScale.x *= -1;
            transform.localScale = localScale;
        }
    }
    private bool Pursuing()
    {
        Vector2 origin = (Vector2)transform.position + rayOffSet;
        Debug.DrawRay(origin, Vector2.right*rayDistance, Color.red);
        RaycastHit2D hit = Physics2D.Raycast(origin, Vector2.right, rayDistance, whatIsPlayer);
        return hit;
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
    private IEnumerator Noitice()
    {
        animator.SetTrigger("isNoiticing");
        isMoving = false;
        rb.linearVelocity = Vector2.zero;
        yield return new WaitForSeconds(0.65f);
        isMoving = true;
    }
    private IEnumerator GetHit()
    {
        if(!isPursuing)
        {
            StartCoroutine(Noitice());
            isPursuing = true;
        }
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
        animator.SetTrigger("isDead");
        rb.linearVelocity = Vector2.zero;
        isMoving = false;
        GetComponent<PolygonCollider2D>().enabled = false;
        triggerBox.enabled = false;
        yield return new WaitForSeconds(0.5f);
        Destroy(gameObject);

    }
    private void OnTriggerEnter2D(Collider2D collision)
    {

        if (collision.CompareTag("Slash") && !getHit)
        {
            if (player != null)
            {
                StartCoroutine(GetHit());
                hp-= slashDmg.getSlashDmg();
                _dmgFlash.CallDmgFlash();
            }
        }
        if (collision.CompareTag("Spike"))
        {
            StartCoroutine(Dying());
        }
    }
    private void OnDrawGizmos()
    {
        Gizmos.DrawWireSphere(transform.position, lineOfSite);  
    }
}
