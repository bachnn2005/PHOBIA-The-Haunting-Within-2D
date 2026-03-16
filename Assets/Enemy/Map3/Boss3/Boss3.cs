using System.Collections;
using System.Collections.Generic;
using System.Runtime.CompilerServices;
using UnityEngine;
using UnityEngine.SceneManagement;

public class Boss3 : MonoBehaviour
{
    private PlayerMoveBehave player;
    private Animator animator;
    [SerializeField] private CameraManager cam;
    [SerializeField] private GameObject slash;
    private BoxCollider2D triggerBox;
    private bool getHit;
    private DmgFlash _dmgFlash;
    private Slash sl;
    private Vector3 firstPosition;
    private SpriteRenderer spriteRenderer;
    [SerializeField] private ManageScene sm;
    [SerializeField] private string[] noi;
    [SerializeField] private string[] endingDialog;
    [SerializeField] private GameManager gameManager;
    [SerializeField] private GameObject deadEffect;
    [SerializeField] private GameObject standPoint;
    //~~~~~~~~~~~~~~~~Stat~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~
    private float currentHp = 0;
    [SerializeField] private float hp;
    private bool isDead;
    //~~~~~~~~~~~~~~~TeleAttack~~~~~~~~~~~~~~~~~~~~~~~~~
    private bool teleAttack;
    //~~~~~~~~~~~~~~~SlashAttack~~~~~~~~~~~~~~~~~~~~~~~~
    private bool slashAttack;
    [SerializeField] private BossSlashAttack bossSlashAttack;
    [SerializeField] private GameObject slashAttackPoint;
    //~~~~~~~~~~~~~~DashAttack~~~~~~~~~~~~~~~~~~~~~~~~~~
    private bool dashAttack;
    private bool dashAttacking;
    //~~~~~~~~~~~~~~AttackRoutine~~~~~~~~~~~~~~~~~~~~~~~
    private int randomInt;
    [SerializeField] private float attackTime;
    private float attackTimeCount = 0;
    //~~~~~~~~~~~~~~~TanBien~~~~~~~~~~~~~~~~~~~~~~~~~~~~
    private bool intoTanBien;
    private bool ending;
    private bool afterBossFight;
    [SerializeField] private GameObject tanBien;
    [SerializeField] private GameObject endingNPC;
    // Start is called before the first frame update
    void Start()
    {
        spriteRenderer = GetComponent<SpriteRenderer>();
        firstPosition = transform.position;
        currentHp = hp;
        _dmgFlash = GetComponent<DmgFlash>();
        sl = GameObject.FindGameObjectWithTag("Slash").GetComponent<Slash>();
        triggerBox = GetComponent<BoxCollider2D>();
        player = GameObject.FindGameObjectWithTag("Player").GetComponent<PlayerMoveBehave>();
        animator = GetComponent<Animator>();
    }

    // Update is called once per frame
    void Update()
    {
        if (isDead && gameManager.getCount() == noi.Length && !afterBossFight)
        {
            if (!player.GetisFacingRight())
            {
                player.transform.localScale = new Vector3(1f, 1f, 1f);
            }
            player.rb.linearVelocity = Vector2.zero;
            player.setCanMove(false);
            intoTanBien = true;
            afterBossFight = true;
            //Invoke("Open", 2f);
        }
        if(gameManager.getCount() == noi.Length + endingDialog.Length && !ending && isDead)
        {
            StartCoroutine(ChangeScene());
             ending = true;
        }
        if(intoTanBien)
        {
            StartCoroutine(TanBien());
            intoTanBien = false;
        }
        if (currentHp <= 0)
        {
            StopAllCoroutines();
            StartCoroutine(Dying());
            return;
        }
        if(isDead)
        {
            player.setBossDead(true);
            return;
        }
        AttackTime();
    }
    private IEnumerator ChangeScene()
    {
        gameManager.StartScreen(true);
        yield return new WaitForSeconds(1f);
        sm.ChangeScene();
    }
    private IEnumerator TanBien()
    {
        _dmgFlash.CallDmgFlash();
        yield return new WaitForSeconds(0.1f);
        animator.SetTrigger("tanBien");
        yield return new WaitForSeconds(1f);
        tanBien.SetActive(true);
        yield return new WaitForSeconds(10f);
        endingNPC.SetActive(true);
        yield return new WaitForSeconds(1f);
        gameManager.OpenDialogue(endingDialog);
        spriteRenderer.enabled = false;
    }
    private void AttackTime()
    {
        if (!teleAttack && ! slashAttack && !dashAttack)
        {
            attackTimeCount -= Time.deltaTime;
        }
        if (attackTimeCount < 0)
        {
            animator.SetTrigger("intoAttack");
            triggerBox.enabled = false;
            randomInt = Random.Range(1,4);
        }
        RandomAttackSystem();
        if(!dashAttacking)
        {
            Flip();
        }
    }
    private void RandomAttackSystem()
    {
        if (!teleAttack && !slashAttack && !dashAttack)
        {
            if (randomInt == 1)
            {
                StartCoroutine(TeleAttack());
                randomInt = 0;
                attackTimeCount = attackTime;
            }
            else if (randomInt == 2)
            {
                StartCoroutine(SlashAttack());
                randomInt = 0;
                attackTimeCount = attackTime;
            }
            else if(randomInt == 3)
            {
                StartCoroutine(DashAttack());
                randomInt = 0;
                attackTimeCount = attackTime;
            }
        }
    }
    private void Flip()
    {
        float flipper = transform.position.x - player.transform.position.x > 0.5f ? 0f : 180f;
        transform.rotation = Quaternion.Euler(0f, flipper, 0f);
    }
    private IEnumerator TeleAttack()
    {
        teleAttack = true;
        yield return new WaitForSeconds(1.5f);
        for(int i = 0; i < 3;i++ )
        {
            float offset = player.GetisFacingRight() ? -2f : 2f;
            float x = player.transform.position.x + offset;
            transform.position = new Vector3(x, transform.position.y, 0f);
            animator.SetBool("attack1", true);
            triggerBox.enabled = true;
            yield return new WaitForSeconds(0.4f);
            cam.Shake(1.2f, 1.2f, 0.25f);   
            slash.SetActive(true);
            yield return new WaitForSeconds(0.23f);
            slash.SetActive(false);
            yield return new WaitForSeconds(1f);
            animator.SetBool("attack1", false);
            triggerBox.enabled = false;
            yield return new WaitForSeconds(1f);
        }
        teleAttack = false;
    }
    private IEnumerator SlashAttack()
    {
        slashAttack = true;
        transform.position = slashAttackPoint.transform.position + new Vector3(0f,-2f,0f);
        animator.SetBool("attack2", true);
        triggerBox.enabled = true;
        yield return new WaitForSeconds(1.7f);
        bossSlashAttack.Slash();
        cam.Shake(1.2f, 1.2f, 0.25f);
        yield return new WaitForSeconds(0.3f);
        bossSlashAttack.Slash();
        cam.Shake(1.2f, 1.2f, 0.25f);
        yield return new WaitForSeconds(0.4f);
        bossSlashAttack.Slash();
        cam.Shake(1.2f, 1.2f, 0.25f);
        yield return new WaitForSeconds(2f);
        animator.SetBool("attack2", false);
        triggerBox.enabled = false;
        slashAttack = false;
    }
    private IEnumerator DashAttack()
    {
        transform.position = slashAttackPoint.transform.position + new Vector3(0f, -2f, 0f);
        Vector3 move = new Vector3 (player.transform.position.x,transform.position.y,0f);
        dashAttack = true;
        animator.SetBool("attack3", true);
        triggerBox.enabled = true;
        yield return new WaitForSeconds(0.5f);
        dashAttacking = true;
        while (Vector2.Distance(transform.position, move) > 1.5f)
        {
            transform.position = Vector2.MoveTowards(transform.position, move, 20f * Time.deltaTime);
            yield return null;
        }
        animator.SetTrigger("dashAttack");
        slash.SetActive(true);
        animator.SetBool("attack3", false);
        cam.Shake(1.2f, 1.2f, 0.25f);
        yield return new WaitForSeconds(0.23f);
        slash.SetActive(false);
        triggerBox.enabled = false;
        yield return new WaitForSeconds(0.8f);
        dashAttack = false;
        dashAttacking = false;
    }
    public void ResetBoss()
    {
        StopAllCoroutines();
        ResetBossStat();
    }
    private void ResetBossStat()
    {
        transform.position = firstPosition;
        currentHp = hp;
        dashAttack = false;
        dashAttacking = false;
        slashAttack = false;
        teleAttack = false;
        slash.SetActive(false);
        spriteRenderer.enabled = true;
        /*
        animator.ResetTrigger("tanBien");
        animator.ResetTrigger("intoAttack");
        animator.ResetTrigger("isDead");
        animator.SetBool("attack1", false);
        animator.SetBool("attack2", false);
        animator.SetBool("attack3", false);
        animator.SetBool("afterDead", false);
        */
    }
    private IEnumerator Dying()
    {
        triggerBox.enabled = false;
        slash.SetActive(false);
        gameManager.ResetCount();
        currentHp = hp;
        isDead = true;
        animator.SetTrigger("isDead");
        StartCoroutine(Explose());
        yield return new WaitForSeconds(1f);
        deadEffect.SetActive(true);
        cam.Shake(2f, 2f, 6.5f);
        yield return new WaitForSeconds(4f);
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
        yield return new WaitForSeconds(0.5f);
        deadEffect.SetActive(false);
        gameManager.StartScreen(false);
        gameManager.EndScreen(true);
        animator.SetTrigger("afterDead");
        transform.rotation = Quaternion.Euler(0f, 0f, 0f);
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
        for (int i = 0; i < 15; i++)
        {
            _dmgFlash.CallDmgFlash();
            yield return new WaitForSeconds(0.25f);
        }
    }
    private void OnTriggerEnter2D(Collider2D collision)
    {
        if(collision.CompareTag("Slash") && !getHit)
        {
            StartCoroutine(GetHit());
        }
    }
    private IEnumerator GetHit()
    {
        getHit = true;
        currentHp -= sl.getSlashDmg();
        _dmgFlash.CallDmgFlash();
        yield return new WaitForSeconds(0.2f);
        getHit = false;
    }
    public void EnableSprite()
    {
        spriteRenderer.enabled = true;
    }
    public void EnableHitBox(bool active)
    {
        triggerBox.enabled = active;
    }
}
