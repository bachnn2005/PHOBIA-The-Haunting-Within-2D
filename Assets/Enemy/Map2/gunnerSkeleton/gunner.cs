using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class gunner : MonoBehaviour
{


    private bool getHit = false;
    [SerializeField] private float speed;
    [SerializeField] private float lineOfSight;
    [SerializeField] private Rigidbody2D rb;
    [SerializeField] private LayerMask groundLayer;
    [SerializeField] private Animator animator;
    [SerializeField] private GameObject bullet;
    [SerializeField] private Transform currentTransform;
    [SerializeField] private GameObject fireBullet;
    [SerializeField] private float fireRate;
    [SerializeField] private Vector3 offset;
    [SerializeField] private SpriteRenderer noitice;
    [SerializeField] private LayerMask playerLayer;
    [SerializeField] private float hp;
    [SerializeField] private DmgFlash _dmgFlash;
    private Slash slashDmg;

    private PlayerMoveBehave player;
    private float nextFireTime;
    private bool isMoving = true;
    private bool isScaling = true;
    void Start()
    {
        slashDmg = GameObject.FindGameObjectWithTag("Slash").GetComponent<Slash>();
        player = FindObjectOfType<PlayerMoveBehave>();
        noitice.transform.localScale = Vector3.zero;
    }

    void Update()
    {

        if (isMoving)
        {
            MoveBehavior();
            animator.SetBool("isStanding", true); 
          
        }
        if (hp <= 0)
        {
            StartCoroutine(Dying());
        }
    }
    private void MoveBehavior()
    {

        RaycastHit2D hit = Physics2D.Raycast(transform.position + offset, Vector2.left, lineOfSight, playerLayer);
        Debug.DrawRay(transform.position + offset, Vector2.left*lineOfSight,Color.red);
        bool hasLineOfSight = hit.collider != null;
        if (hasLineOfSight)
        {
            if (nextFireTime < Time.time)
            {
                StartCoroutine(ShootRoutine());
                nextFireTime = Time.time + fireRate;
            }
            if(isScaling)
            {
                StartCoroutine(ScaleNoitice());
            }
            rb.linearVelocity = Vector2.zero;
        }
        else if (!hasLineOfSight)
        {
            //noitice.enabled = false;
            if (!animator.GetBool("isAiming"))
            {
                noitice.transform.localScale = Vector3.zero;
                nextFireTime = Time.time;
                isScaling = true;
            }
            
            rb.linearVelocity = Vector2.zero;
        }

        Flip();
    }
    private IEnumerator ScaleNoitice()
    {
        isScaling = false;
        float timeElapsed = 0f;
        noitice.enabled = true;
        while (timeElapsed < 0.1f)
        {
            noitice.transform.localScale = Vector3.Lerp(Vector3.zero, Vector3.one, timeElapsed / 0.1f);
            timeElapsed += Time.deltaTime;
            yield return null;

        }

        noitice.transform.localScale = Vector3.one;

    }
    private IEnumerator ShootRoutine()

    {
        animator.SetBool("isAiming", true);  // Bắt đầu nhắm
        
        yield return new WaitForSeconds(1f);
        GameObject spawnedBullet = Instantiate(bullet, fireBullet.transform.position, Quaternion.identity);
        animator.SetBool("isShooting", true);
        if (transform.localScale.x < 0)
        {
            spawnedBullet.GetComponent<Rigidbody2D>().linearVelocity = new Vector2(10f, 0f); 
        }
        else
        {
            spawnedBullet.GetComponent<Rigidbody2D>().linearVelocity = new Vector2(-10f, 0f); // Đạn bắn sang phải
        }
        yield return new WaitForSeconds(0.15f);
        animator.SetBool("isShooting", false);  
        yield return new WaitForSeconds(2f);
        animator.SetBool("isAiming", false);
        animator.SetBool("isStanding", true);

    }


    private void OnTriggerEnter2D(Collider2D collision)
    {

        if (collision.CompareTag("Slash") && !getHit)
        {
            StartCoroutine(GetHit());
            hp -= slashDmg.getSlashDmg();
            _dmgFlash.CallDmgFlash();
        }
    }
    private IEnumerator GetHit()
    {
        getHit = true;
        isMoving = false;
        
        rb.linearVelocity = Vector2.zero;
        yield return new WaitForSeconds(0.15f);
        isMoving = true;
        yield return new WaitForSeconds(0.05f);
        getHit = false;
    }

    private void Flip()
    {
        // Lật đối tượng bằng cách thay đổi localScale.x
        Vector3 localScale = transform.localScale;

        if (transform.position.x > player.transform.position.x)
        {
            localScale.x = Mathf.Abs(localScale.x); // Hướng về bên phải
        }
        else
        {
            localScale.x = -Mathf.Abs(localScale.x); // Hướng về bên trái
        }

        transform.localScale = localScale;
    }

    private IEnumerator Dying()
    {
        noitice.enabled = false;
        animator.SetBool("isDead",true);
        isMoving = false;
        rb.linearVelocity = Vector2.zero;
        GetComponent<BoxCollider2D>().enabled = false;
        yield return new WaitForSeconds(0.5f);
        Destroy(gameObject);

    }
}