using System.Collections;
using System.Collections.Generic;
using System.Runtime.CompilerServices;
using Unity.VisualScripting;
using UnityEngine;

public class Boss1 : MonoBehaviour
{
    [SerializeField] private Transform shooter;
    [SerializeField] private HoneyBullet honeyBullet;
    [SerializeField] private GameObject pointA;
    [SerializeField] private GameObject pointB;
    [SerializeField] private CameraManager cam;
    [SerializeField] private Animator animator;
    [SerializeField] private PolygonCollider2D closeAttackBox;
    [SerializeField] private PolygonCollider2D triggerBox;
    [SerializeField] private GameManager gameManager;
    [SerializeField] private string[] boss3Noi;
    [SerializeField] private string[] noi;
    [SerializeField] private Animator boss3;
    [SerializeField] private ManageScene sm;
    [SerializeField] private GameObject standPoint;
    [SerializeField] private GameObject deadEffect;
    private bool isDead;
    private PlayerMoveBehave player;
    private bool isShooting;
    private Transform currentTransform;
    [SerializeField] private float moveSpeed;
    private Rigidbody2D rb;
    private bool isMoving = true;
    private bool isCloseAttack;
    private Vector2 originPosition;
    private int randomInt;
    [SerializeField] private float attackTime = 2f;
    private float attackTimeCount;
    private DmgFlash _dmgFlash;
    [SerializeField] private float hp;
    private float currentHp;
    [SerializeField] private Slash slash;
    private bool getHit = false;
    private bool canFlip = true;
    private bool intoPhase2;
    private bool intoPhase1;
    private bool intoPhase3;
    private bool intoPhase4;
    private Vector3 firstPosition;
    // Start is called before the first frame update
    void Start()
    {
        firstPosition = transform.position;
        currentHp = hp;
        _dmgFlash = GetComponent<DmgFlash>();
        rb = GetComponent<Rigidbody2D>();
        currentTransform = pointA.transform;
        originPosition = (Vector2)pointA.transform.position; 
        player = GameObject.FindGameObjectWithTag("Player").GetComponent<PlayerMoveBehave>();
        IntoPhase1();
    }

    // Update is called once per frame
    void Update()
    {
        if(isDead && gameManager.getCount() == noi.Length && !intoPhase3)
        {
            if (player.GetisFacingRight())
            {
                player.transform.localScale = new Vector3(-1f, 1f, 1f);
            }
            player.rb.linearVelocity = Vector2.zero;
            player.setCanMove(false);
            intoPhase3 = true;
            boss3.gameObject.SetActive(true);
            Invoke("Open", 2f);
        }
        else if(isDead && gameManager.getCount() == noi.Length + boss3Noi.Length && !intoPhase4)
        {
            if (player.GetisFacingRight())
            {
                player.transform.localScale = new Vector3(-1f, 1f, 1f);
            }
            player.rb.linearVelocity = Vector2.zero;
            player.setCanMove(false);
            intoPhase4 = true;
            StartCoroutine(Boss3Apprear());
        }
        if(currentHp <= 0)
        {
            StopAllCoroutines();
            StartCoroutine(Dying());
            return;
        }
        if( !intoPhase1 || isDead)
        {
            if (isDead)
            {
                player.setBossDead(true);
            }
            return;
        }
        if(currentHp <= hp/2)
        {
            intoPhase2 = true;
        }
        attackTimeCount -= Time.deltaTime;
        if(attackTimeCount < 0 )
        {
            randomInt = Random.Range(1, 3);
        }
        Flip();
        RandomAttackSystem();
        if(!isMoving)
        {
            return;
        }
        Patroling();
    }
    private void RandomAttackSystem()
    {
        if (!isCloseAttack)
        {
            if (randomInt == 1)
            {
                if (intoPhase2)
                {
                    StartCoroutine(Shooting2());
                }
                else
                {
                    StartCoroutine(Shooting());
                }
                randomInt = 0;
                attackTimeCount = attackTime;
            }
            else if (randomInt == 2)
            {
                StartCoroutine(CloseAttack());
                randomInt = 0;
                attackTimeCount = attackTime;
            }
        }
    }
    public void IntoPhase1()
    {
        if(isActiveAndEnabled)
        {
            StartCoroutine(StartBossFightRoutine());
        }
    }
    private IEnumerator StartBossFightRoutine()
    {
        while (Vector2.Distance(transform.position, originPosition) > 0.1f)
        {
            Vector2 returnDirection = (originPosition - (Vector2)transform.position).normalized;

            rb.linearVelocity = returnDirection * moveSpeed*1.5f;
            yield return null;
        }
        rb.linearVelocity = Vector2.zero; 
        yield return new WaitForSeconds(1f);
        cam.Shake(2f,2f,1.5f);
        yield return new WaitForSeconds(2.5f);
        intoPhase1 = true;
    }
    #region shooting attack
    private IEnumerator Shooting2()
    {
        isShooting = true;
        animator.SetTrigger("shooting");
        yield return new WaitForSeconds(0.35f);
        Instantiate(honeyBullet, shooter.position, Quaternion.identity);
        yield return new WaitForSeconds(0.1f);
        animator.SetTrigger("shooting");
        yield return new WaitForSeconds(0.35f);
        Instantiate(honeyBullet, shooter.position, Quaternion.identity);
        isShooting = false;
        animator.ResetTrigger("shooting");
    }
    private IEnumerator Shooting()
    {
        isShooting = true;
        animator.SetTrigger("shooting");
        yield return new WaitForSeconds(0.35f);
        Instantiate(honeyBullet, shooter.position, Quaternion.identity);
        isShooting =false;
        animator.ResetTrigger("shooting");
    }
    #endregion
    #region close attack
    private void CloseAttackingBox(bool closeAttackBox,bool triggerBox)
    {
        this.closeAttackBox.enabled = closeAttackBox;
        this.triggerBox.enabled = triggerBox;
    }
    private IEnumerator CloseAttack()
    {
        CloseAttackingBox(true,false);
        isCloseAttack = true;
        originPosition = transform.position;
        isMoving = false;
        rb.linearVelocity = Vector2.zero;
        animator.SetTrigger("readyCloseAttack");
        yield return new WaitForSeconds(0.25f);
        animator.SetBool("closeAttacking", true);
        yield return new WaitForSeconds(0.75f);
        canFlip = false;
        Vector2 direction = (player.transform.position - transform.position).normalized;
        rb.linearVelocity = direction * moveSpeed * 5f;
    }
    private IEnumerator ReturnPatrol()
    {
        yield return new WaitForSeconds(1f);
        canFlip = true;
        CloseAttackingBox(false,true);
        animator.SetBool("closeAttacking", false);
        animator.SetTrigger("doneCloseAttack");
        while (Vector2.Distance(transform.position, originPosition) > 0.1f)
        {
            Vector2 returnDirection = (originPosition - (Vector2)transform.position).normalized;

            rb.linearVelocity = returnDirection * moveSpeed*1.5f;
            yield return null;
        }
        animator.ResetTrigger("doneCloseAttack");
        animator.ResetTrigger("readyCloseAttack");
        rb.linearVelocity = Vector2.zero;
        isMoving = true;
        isCloseAttack = false;
    }
    #endregion
    private void Flip()
    {
        if(canFlip)
        {
            float flipper = transform.position.x - player.transform.position.x > 0 ? 180f : 0f;
            transform.rotation = Quaternion.Euler(0f, flipper, 0f);
        }
        
    }
    private void Patroling()
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
        if (currentTransform == pointB.transform)
        {
            
            rb.linearVelocity = new Vector2(moveSpeed, 0f);
        }
        else if (currentTransform == pointA.transform)
        {
            rb.linearVelocity = new Vector2(-moveSpeed, 0f);
        }

    }
    public void ResetBoss()
    {
        StopAllCoroutines();
        ResetBossStat();
        currentTransform = pointA.transform;
    }
    private void ResetBossStat()
    {
        transform.rotation = Quaternion.identity;
        transform.position = firstPosition;
        rb.linearVelocity = Vector2.zero;
        intoPhase1 = false;
        intoPhase2 = false;
        isMoving = true;
        isShooting = false;
        isCloseAttack = false;
        currentHp = hp;
        canFlip = true;
        /*
        animator.SetBool("isCloseAttacking", false);
        animator.ResetTrigger("doneCloseAttack");
        animator.ResetTrigger("readyCloseAttack");
        animator.ResetTrigger("shooting");
        */
    }
    private IEnumerator GetHit()
    {
        getHit = true;
        currentHp -= slash.getSlashDmg();
        _dmgFlash.CallDmgFlash();
        yield return new WaitForSeconds(0.2f);
        getHit = false;
    }
    private void OnTriggerEnter2D(Collider2D collision)
    {
        if(collision.CompareTag("Obstacle") && isCloseAttack)
        {
            cam.Shake(2f, 2f, 0.25f);
            rb.linearVelocity = Vector2.zero;
            StartCoroutine(ReturnPatrol());
            return;
        }
        if(collision.CompareTag("Slash") && !getHit)
        {
            StartCoroutine(GetHit());
            return;
        }
    }
    private IEnumerator Dying()
    {
        triggerBox.enabled = false;
        closeAttackBox.enabled = false;
        gameManager.ResetCount();
        currentHp = hp;
        isDead = true;
        animator.SetTrigger("isDead");
        rb.linearVelocity = Vector2.zero;
        StartCoroutine(Explose());
        yield return new WaitForSeconds(1f);
        deadEffect.SetActive(true);
        cam.Shake(2f, 2f, 6.5f);
        yield return new WaitForSeconds(4f);
        deadEffect.SetActive(false);
        gameManager.StartScreen(true);
        yield return new WaitForSeconds(1f);
        player.setCanMove(false);
        ResetBossStat();
        player.transform.position = standPoint.transform.position + new Vector3(0f,-2f,0f);
        player.rb.linearVelocity = Vector2.zero;
        if(player.GetisFacingRight())
        {
            player.transform.localScale = new Vector3(-1f,1f,1f);
        }
        yield return new WaitForSeconds(0.5f);
        gameManager.StartScreen(false);
        gameManager.EndScreen(true);
        yield return new WaitForSeconds(1f);
        gameManager.EndScreen(false);
        yield return new WaitForSeconds(2f);
        gameManager.OpenDialogue(noi);
    }
    private IEnumerator Explose()
    {
        for (int i = 0; i < 2; i++)
        {
            _dmgFlash.CallDmgFlash();
            yield return new WaitForSeconds(0.65f);
        }
        for (int i = 0; i < 3; i++)
        {
            _dmgFlash.CallDmgFlash();
            yield return new WaitForSeconds(0.4f);
        }
        for(int i = 0; i < 20; i++)
        {
            _dmgFlash.CallDmgFlash();
            yield return new WaitForSeconds(0.25f);
        }
    }
    private IEnumerator Boss3Apprear()
    {
        
        Vector2 move = player.transform.position;
        yield return new WaitForSeconds(3f);
        boss3.SetBool("isMoving", true);
        while (Vector2.Distance(boss3.transform.position, move) > 2f)
        {
            boss3.transform.position = Vector2.MoveTowards(boss3.transform.position, move, 20f * Time.deltaTime);
            yield return null;
        }
        boss3.SetTrigger("slashing");
        yield return new WaitForSeconds(0.1f);
        gameManager.BlackScreen(true);
        sm.ChangeScene();
    }
    private void Open()
    {
        gameManager.OpenDialogue(boss3Noi);
    }
}
