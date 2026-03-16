using System.Collections;
using UnityEngine;

public class Boss2 : MonoBehaviour
{
    private SpriteRenderer bossSpirte;
    [SerializeField] private Slash slash;
    [SerializeField] private float bounceSpeedX;
    private float bounceX;
    [SerializeField] private float bounceSpeedY;
    private float bounceY;
    [SerializeField] private CameraManager cameraManager;
    private Animator animator;
    private Rigidbody2D rb;
    [SerializeField] private PolygonCollider2D triggerBox;
    [SerializeField] private GameObject bossLight;
    private DmgFlash _dmgFlash;
    private Vector3 firstPosition;
    [SerializeField] private CameraManager cam;
    [SerializeField] private GameObject deadEffect;
    [SerializeField] private GameObject standPoint;
    [SerializeField] private GameManager gameManager;
    [SerializeField] private ManageScene sm;
    [SerializeField] private Animator boss3;
    //~~~~~~~~~~~~~~~~Dialog~~~~~~~~~~~~~~~~~~~~~~~~~~~~
    [SerializeField] private string[] noi;
    [SerializeField] private string[] boss3Noi;
    //~~~~~~~~~~~~~~~~~Stat~~~~~~~~~~~~~~~~~~~~~~~~~~~~~
    [SerializeField] private float hp;
    private float currentHp;
    private bool getHit;
    private bool isDead;
    //~~~~~~~~~~~~~~BounceAttack~~~~~~~~~~~~~~~~~~~~~~~~
    private CircleCollider2D bounceBox;
    private bool bounceAttack = false;
    [SerializeField] private float bounceTime;
    private float bounceTimeCounter = 0;
    private PlayerMoveBehave player;
    //~~~~~~~~~~~~~~SpikeAttack~~~~~~~~~~~~~~~~~~~~~~~~~
    [SerializeField] private PolygonCollider2D spikeBox;
    [SerializeField] private float spikeAttackTime;
    private float spikeAttackTimeCounter = 0;
    private bool spikeAttack;
    //~~~~~~~~~~~~~~AttackRoutine~~~~~~~~~~~~~~~~~~~~~~~
    private int randomInt;
    [SerializeField] private float attackTime = 5f;
    private float attackTimeCount = 0;
    private bool intoPhase3;
    private bool intoPhase4;
    // Start is called before the first frame update
    void Start()
    {
        currentHp = hp;
        bossSpirte = GetComponent<SpriteRenderer>();
        firstPosition = transform.position;
        _dmgFlash = GetComponent<DmgFlash>();
        attackTimeCount = attackTime;
        bounceBox = GetComponent<CircleCollider2D>();   
        animator = GetComponent<Animator>();
        rb = GetComponent<Rigidbody2D>();   
        player = GameObject.FindGameObjectWithTag("Player").GetComponent<PlayerMoveBehave>();
    }

    // Update is called once per frame
    void Update()
    {
        if (isDead && gameManager.getCount() == noi.Length && !intoPhase3)
        {
            if (!player.GetisFacingRight())
            {
                player.transform.localScale = new Vector3(1f, 1f, 1f);
            }
            player.rb.linearVelocity = Vector2.zero;
            player.setCanMove(false);
            intoPhase3 = true;
            boss3.gameObject.SetActive(true);
            Invoke("Open", 2f);
        }
        else if (isDead && gameManager.getCount() == noi.Length + boss3Noi.Length && !intoPhase4)
        {
            if (!player.GetisFacingRight())
            {
                player.transform.localScale = new Vector3(1f, 1f, 1f);
            }
            player.rb.linearVelocity = Vector2.zero;
            player.setCanMove(false);
            intoPhase4 = true;
            StartCoroutine(Boss3Apprear());
        }
        if (currentHp <= 0 )
        {
            StopAllCoroutines();
            StartCoroutine(Dying());
 
        }
        if(isDead)
        {
            player.setBossDead(true);
            return;
        }
        Flip();
        if(bounceAttack)
        {
           rb.linearVelocity = new Vector2(bounceX, bounceY);
           bounceTimeCounter -= Time.deltaTime;
        }
        else if(spikeAttack)
        {
            spikeAttackTimeCounter -= Time.deltaTime;
            if(spikeAttackTimeCounter < 0)
            {
                spikeAttack = false;
            }
        }
        AttackTime();
    }
    private void OnTriggerEnter2D(Collider2D collision)
    {
        if (bounceAttack)
        {
            if (collision.CompareTag("Wall"))
            {
                cameraManager.Shake(1.2f, 1.2f, 0.5f);
                bounceX *= -1;
            }
            else if (collision.CompareTag("Obstacle") && bounceTimeCounter < 0f)
            {
                bounceBox.enabled = false;
                animator.SetBool("doneAttack1",true);
                animator.SetBool("attack1", false);
                Invoke("doneAttack", 1f);
                cameraManager.Shake(1.2f, 1.2f, 0.5f);
                transform.position += new Vector3(0f, 0.25f, 0f);
                rb.linearVelocity = Vector2.zero;
                bounceAttack = false;
            }
            else if (collision.CompareTag("Ceil") || collision.CompareTag("Obstacle"))
            {
                cameraManager.Shake(1.2f, 1.2f, 0.5f);
                bounceY *= -1;
            }
        }
        if(collision.CompareTag("Slash") && !getHit)
        {
            StartCoroutine(GetHit());
        }
    }
    private IEnumerator GetHit()
    {
        getHit = true;
        currentHp -= slash.getSlashDmg();
        _dmgFlash.CallDmgFlash();
        yield return new WaitForSeconds(0.2f);
        getHit = false;
    }
    public void ResetBoss()
    {
        StopAllCoroutines();
        ResetBossStat();
    }
    private void ResetBossStat()
    {
        transform.position = firstPosition;
        bounceAttack = false;
        spikeAttack = false;
        rb.linearVelocity = Vector2.zero;
        currentHp = hp;
        triggerBox.enabled = false;
        bounceBox.enabled = false;
        spikeBox.enabled = false;
        /*
        animator.ResetTrigger("intoAttack");
        animator.ResetTrigger("doneSpikeAttack");
        animator.SetBool("attack1", false);
        animator.SetBool("attack2", false);
        animator.SetBool("doneAttack1", false);
        animator.SetBool("spikeAttacking", false);
        animator.SetBool("doneAttack2", false);
        */
    }
    private void doneAttack()
    {
        triggerBox.enabled = true;
    }
    private void AttackTime()
    {
        if(!bounceAttack && !spikeAttack)
        {
            attackTimeCount -= Time.deltaTime;
        }
        if (attackTimeCount < 0)
        {
            triggerBox.enabled = false;
            animator.SetTrigger("intoAttack");
            randomInt = Random.Range(1, 3);
        }
        RandomAttackSystem();
    }
    private void RandomAttackSystem()
    {
        if (!bounceAttack && !spikeAttack)
        {
            if (randomInt == 1)
            {
                animator.SetBool("doneAttack1", false);
                StartCoroutine(BounceAttack());
                randomInt = 0;
                attackTimeCount = attackTime;
            }
            else if (randomInt == 2)
            {
                animator.SetBool("doneAttack2", false);
                StartCoroutine(SpikeAttack());
                randomInt = 0;
                attackTimeCount = attackTime;
            }
        }
    }
    private IEnumerator SpikeAttack()
    {
        spikeAttackTimeCounter = spikeAttackTime;
        spikeAttack = true;
        yield return new WaitForSeconds(1.5f);
        for(int i = 0; i < 3; i++)
        {
            spikeBox.enabled = false;
            animator.SetBool("attack2", true);
            yield return new WaitForSeconds(1f);
            bossLight.SetActive(false);
            float x = player.transform.position.x;
            transform.position = new Vector3(x, transform.position.y, 0f);
            animator.SetBool("spikeAttacking", true);
            bossLight.SetActive(true);
            yield return new WaitForSeconds(0.7f);
            spikeBox.enabled = true;
            yield return new WaitForSeconds(1f);
            spikeBox.enabled = false;
            animator.SetTrigger("doneSpikeAttack");
            animator.SetBool("spikeAttacking", false);
            animator.SetBool("attack2", false);
            yield return new WaitForSeconds(1f);
        }
        animator.SetBool("doneAttack2", true);
        triggerBox.enabled = true;
    }
    private IEnumerator BounceAttack()
    {
        yield return new WaitForSeconds(1.5f);
        bounceBox.enabled = true;
        animator.SetBool("attack1",true);
        yield return new WaitForSeconds(2f);
        bounceX = bounceSpeedX;
        bounceY = bounceSpeedY;
        bounceAttack = true;
        bounceTimeCounter = bounceTime;
    }
    private IEnumerator Dying()
    {
        triggerBox.enabled = false;
        spikeBox.enabled = false;
        bounceBox.enabled = false;
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
        player.transform.position = standPoint.transform.position + new Vector3(0f, -2f, 0f);
        player.rb.linearVelocity = Vector2.zero;
        if (!player.GetisFacingRight())
        {
            player.transform.localScale = new Vector3(1f, 1f, 1f);
        }
        TurnOffBoss();
        yield return new WaitForSeconds(0.5f);
        gameManager.StartScreen(false);
        gameManager.EndScreen(true);
        yield return new WaitForSeconds(1f);
        gameManager.EndScreen(false);
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
        for (int i = 0; i < 20; i++)
        {
            _dmgFlash.CallDmgFlash();
            yield return new WaitForSeconds(0.25f);
        }
    }
    private void TurnOffBoss()
    {
        triggerBox.enabled = false;
        bounceBox.enabled = false;
        spikeBox.enabled = false;
        bossSpirte.enabled = false;
        bossLight.SetActive(false);
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
    private void Flip()
    {
        float flipper = transform.position.x - player.transform.position.x > 0.5f  ? 0f : 180f;
        transform.rotation = Quaternion.Euler(0f, flipper, 0f);
    }
    private void Open()
    {
        gameManager.OpenDialogue(boss3Noi);
    }
    public void EnableHitBox(bool active)
    {
        triggerBox.enabled = active;
    }
}
