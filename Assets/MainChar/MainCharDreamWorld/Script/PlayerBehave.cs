using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Animations;
using UnityEngine.Rendering.Universal;

public class PlayerMoveBehave : MonoBehaviour
{
    [SerializeField] private GameObject startScreen;
    [SerializeField] private GameObject endScreen;
    [SerializeField] private GameObject takeDmgVFX;
    [SerializeField] public Rigidbody2D rb;
    public bool isFacingRight = true;
    private bool canMove = true;
    [SerializeField] public Animator animator;
    private bool bossDead;
    //~~~~~~~~~~Moving~~~~~~~~~~~~~
    private float move;
    [SerializeField] private float moveSpeed;
    [SerializeField] public float look;
    //~~~~~~~~~~Jumping~~~~~~~~~~~~
    [SerializeField] private GameObject jumpSmoke;
    [SerializeField] private float jumpForce;
    private float jumpTimeCounter;
    [SerializeField] private float jumpTime;
    [SerializeField] private bool isJumping;
    private float coyoteTime = 0.05f;
    private float coyoteTimeCounter;
    private float jumpBufferTime = 0.05f;
    private float jumpBufferCounter;
    private float fallTime = 2f;
    private float fallTimeCounter;
    //~~~~~~~~~CheckGround~~~~~~~~~
    [SerializeField] private LayerMask whatIsGround;
    [SerializeField] private bool isGrounded;
    [SerializeField] private Vector2 groundCheckOffset;
    [SerializeField] private float checkGroundRange;
    //~~~~~~~~~~~Dashing~~~~~~~~~~~
    [SerializeField] private bool isDashing;
    [SerializeField] private float dashingPower;
    [SerializeField] private float dashTime;
    [SerializeField] private bool canDash;
    //~~~~~~~~~~~~Stat~~~~~~~~~~~~~
    [SerializeField] private int hp;
    private int currentHp;
    //~~~~~~~~~~TakingDmg~~~~~~~~~~
    [SerializeField] private GameObject takeDmgEffect;
    private bool isTakingDmg;
    private bool isImmortal;
    [SerializeField] private float blinkInterval = 0.1f;
    [SerializeField] private SpriteRenderer playerSprite;
    private bool isDead;
    //~~~~~~~~~Attacking~~~~~~~~~~~
    [SerializeField] private float attackRate;
    [SerializeField] private bool isAttacking;
    [SerializeField] private int attackIndex = 0;
    [SerializeField] private float chainAttackResetTime = 1f;
    private float attackingTime;
    private float chainAttackTime;
    //~~~~~~~~~~~~Camera~~~~~~~~~~~
    [Header("~~~~~~~~~~~~Shake System~~~~~~~~~~~~~")]
    [SerializeField] private CameraManager camera_;
    [SerializeField] private float shakeAmp;
    [SerializeField] private float shakeFru;
    [SerializeField] private float shakeDur;
    // Start is called before the first frame update
    void Start()
    {
        currentHp = hp;
        fallTimeCounter = fallTime;
    }
    // Update is called once per frame
    void Update()
    {
        //~~~~~Animation~~~~
        AnimationManager();
        //~~~~~~~~~~~~~~~~~~
        FallCheck();
        CheckGround();
        if(currentHp <= 0 && !isDead && !bossDead)
        {
            StartCoroutine(Spawn());
        }
        if (isDashing || !canMove || isTakingDmg || isDead)
        {
            animator.SetBool("isMoving", false);
            return;
        }
        if (isGrounded)
        {
            canDash = true;
        }
        if (Input.GetButtonUp("Vertical"))
        {
            look = 0f;
        }
        MoveBehave();
        if (Input.GetButtonDown("Dash") && canDash && PlayerPrefs.GetInt("unlockDash") == 1)
        {
            StartCoroutine(Dash());
        }
        Attack();
        JumpBehave();
        Flip();
    }
    private void FixedUpdate()
    {

    }
    private void OnTriggerEnter2D(Collider2D collision)
    {
        
        if ((collision.CompareTag("Enemy") || collision.CompareTag("Enemy2")) && !isImmortal)
        {
            if(!isDead)
            {
                currentHp--;
                TakeDmg();
            }
        }
    }
    private void OnTriggerStay2D(Collider2D collision)
    {
        if ((collision.CompareTag("Enemy") || collision.CompareTag("Enemy2")) && !isImmortal)
        {
            if(!isDead)
            {
                currentHp--;
                TakeDmg();
            }
            
        }
    }
    
    //~~~~~~~~~
    #region attacking
    private void Attack()
    {
        
        if (Time.time >= attackingTime && Input.GetButtonDown("Attack"))
        {
            ChainAttack();
        }
        if (Time.time >= chainAttackTime)
        {
            ResetChainAttack();
        }
    }
    private void ChainAttack()
    {
        attackIndex++;

        if (attackIndex > 2)
        {
            attackIndex = 1;
        }
        if (!isGrounded && look < 0f)
        {
            attackIndex = 4;
            animator.SetTrigger("isAttacking");
            animator.SetInteger("AttackIndex", attackIndex);
        }
        else if (look > 0f)
        {
            attackIndex = 5;
            animator.SetTrigger("isAttacking");
            animator.SetInteger("AttackIndex", attackIndex);
        }
        else
        {
            animator.SetInteger("AttackIndex", attackIndex);
            animator.SetTrigger("isAttacking");
        }

        chainAttackTime = Time.time + chainAttackResetTime;
        attackingTime = Time.time + 1f / attackRate;
        StartCoroutine(AttackingAnimation());
    }
    private void ResetChainAttack()
    {
        attackIndex = 0;
        animator.SetInteger("AttackIndex", attackIndex);
    }
    #endregion
    //~~~~~~~~~
    #region takedmg

    private void TakeDmg()
    {
        StartCoroutine(KnockBack());
        StartCoroutine(Immortal());
    }
    private IEnumerator KnockBack()
    {
        takeDmgVFX.SetActive(true);
        animator.SetBool("isTakingDmg", true);
        isTakingDmg = true;
        StartCoroutine(FreezeScreen());
        yield return new WaitForSeconds(0.2f);

        rb.linearVelocity = new Vector2(-5f * transform.localScale.x, 5f);
        yield return new WaitForSeconds(0.3f);
        takeDmgVFX.SetActive(false);
        yield return new WaitForSeconds(0.2f);
        animator.SetBool("isTakingDmg", false);
        if(isDead)
        {
            rb.linearVelocity = Vector2.zero;
        }
        isTakingDmg = false;
    }
    private IEnumerator FreezeScreen()
    {
        camera_.Shake(shakeAmp, shakeFru, shakeDur);
        takeDmgEffect.SetActive(true);
        rb.linearVelocity = Vector2.zero;
        rb.isKinematic = true;
        yield return new WaitForSeconds(0.2f);
        GetComponent<Rigidbody2D>().isKinematic = false;
        yield return new WaitForSeconds(1f);
        takeDmgEffect.SetActive(false);
    }
    private IEnumerator Immortal()
    {
        isImmortal = true;
        yield return new WaitForSeconds(0.25f);
        float elapsedTime = 0f;
        Color playerColor = playerSprite.color;
        while (elapsedTime < 3f)
        {
            if(!isDead)
            {
                float t = Mathf.PingPong(Time.time / blinkInterval, 1);
                playerColor.a = Mathf.Lerp(1f, 0f, t);
                playerSprite.color = playerColor;
            }
            else
            {
                playerColor.a = 1f;
                playerSprite.color = playerColor;
            }

            yield return null;

            elapsedTime += Time.deltaTime;
        }
        playerColor.a = 1f;
        playerSprite.color = playerColor;
        isImmortal = false;
    }

    #endregion
    //~~~~~~~~~
    #region Dying
    private IEnumerator Spawn()
    {
        isDead = true;
        while(!isGrounded)
        {
            animator.SetBool("isTakingDmg",true);
            yield return null;
        }
        animator.SetBool("isTakingDmg", false);
        animator.SetBool("isDead", true);
        yield return new WaitForSeconds(2f);
        startScreen.SetActive(true);
        yield return new WaitForSeconds(1f);
        float x = PlayerPrefs.GetFloat("PosX");
        float y = PlayerPrefs.GetFloat("PosY");
       
        animator.SetBool("isDead", false);
        isDead = false;
        transform.position = new Vector3(x, y -2f, 0f);
        currentHp = hp;
        yield return new WaitForSeconds(2f);
        startScreen.SetActive(false);
        endScreen.SetActive(true);
        yield return new WaitForSeconds(1f);
        endScreen.SetActive(false);
    }

    #endregion
    private void Flip()
    {
        if (!isAttacking && (move < 0f && isFacingRight || move > 0f && !isFacingRight))
        {
            isFacingRight = !isFacingRight;
            Vector3 localScale = transform.localScale;
            localScale.x *= -1;
            transform.localScale = localScale;
        }
    }
    private void MoveBehave()
    {
        look = Input.GetAxis("Vertical");
        move = Input.GetAxis("Horizontal");
        rb.linearVelocity = new Vector2(move * moveSpeed, rb.linearVelocity.y);
    }
    private IEnumerator JumpSmoke()
    {
        jumpSmoke.transform.position = transform.position + new Vector3(0f, 1.75f, 0f);
        jumpSmoke.SetActive(true);
        yield return new WaitForSeconds(0.22f);
        jumpSmoke.SetActive(false);
    }
    private void FallCheck()
    {
        if (animator.GetBool("isFalling"))
        {
            fallTimeCounter -= Time.deltaTime;
        }
        else
        {
            if (fallTimeCounter <= 0)
            {
                StartCoroutine(StandUp());
                camera_.Shake(1.5f, 1.5f, 0.5f);
            }
            fallTimeCounter = fallTime;
        }
    }
    private IEnumerator StandUp()
    {
        canMove = false;
        rb.linearVelocity = Vector2.zero;
        animator.SetBool("isStandingUp",true);
        yield return new WaitForSeconds(3.3f);
        animator.SetBool("isStandingUp", false);
        canMove = true;
    }
    private void JumpBehave()
    {
        if (isGrounded)
        {
            coyoteTimeCounter = coyoteTime;
        }
        else
        {
            coyoteTimeCounter -= Time.deltaTime;
        }

        if (Input.GetButtonDown("Jumping"))
        {
            jumpBufferCounter = jumpBufferTime;
        }
        else
        {
            jumpBufferCounter -= Time.deltaTime;
        }
        if (coyoteTimeCounter > 0f && jumpBufferCounter > 0f && !isJumping)
        {
            isJumping = true;
            StartCoroutine(JumpSmoke());
            rb.linearVelocity = new Vector2(rb.linearVelocity.x, jumpForce);
            jumpTimeCounter = jumpTime;

        }
        if(Input.GetButton("Jumping") && isJumping)
        {
            if (jumpTimeCounter > 0)
            {
                rb.linearVelocity = new Vector2(rb.linearVelocity.x, jumpForce);
                jumpTimeCounter -= Time.deltaTime;
            }
            else
            {
                isJumping = false;
            }
        }
        if(Input.GetButtonUp("Jumping"))
        {
            isJumping = false;
        }
    }
    private IEnumerator Dash()
    {
        isDashing = true;
        coyoteTimeCounter = 0f;
        jumpBufferCounter = 0f;
        isJumping = false;
        rb.linearVelocity = new Vector2(dashingPower * transform.localScale.x, 0f);
        animator.SetBool("isDashing", true);
        float originGravity = rb.gravityScale;
        rb.gravityScale = 0f;
        yield return new WaitForSeconds(dashTime);
        rb.gravityScale = originGravity;
        animator.SetBool("isDashing", false);
        isDashing = false;
        canDash = false;
    }
    private void CheckGround()
    {
        groundCheckOffset.x = isFacingRight ? -0.25f : 0.25f;
        Vector2 position = (Vector2)transform.position + groundCheckOffset;
        Vector2 direction = isFacingRight ? Vector2.right : Vector2.left;
        RaycastHit2D hit = Physics2D.Raycast(position, direction, checkGroundRange, whatIsGround);
        Debug.DrawRay(position, checkGroundRange * direction, Color.red);
        if (hit.collider != null)
        {
            isGrounded = true;
        }
        else
        {
            isGrounded = false;
        }
    }
    //~~~~~~~~~~~~~~~~~~~ANIMATION~~~~~~~~~~~~~~~~~~~~~~~~
    #region animation
    private void AnimationManager()
    {
        MovingAnimation();
        JumpingAnimation();
    }
    private void MovingAnimation()
    {
        animator.SetBool("isMoving", move != 0f);
    }
    private void JumpingAnimation()
    {
        animator.SetBool("isJumping", rb.linearVelocity.y > 0.002f);
        animator.SetBool("isFalling", rb.linearVelocity.y < -4f);
    }
    private IEnumerator AttackingAnimation()
    {
        isAttacking = true;
        yield return new WaitForSeconds(0.3f);
        if (!isGrounded)
        {
            animator.SetInteger("AttackIndex", 0);
        }
        isAttacking = false;
    }
    #endregion
    //~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~
    public float getLook()
    {
        return look;
    }
    public bool getAttacking()
    {
        return isAttacking;
    }
    public bool GetisFacingRight()
    {
        return isFacingRight;
    }
    public bool GetisGrounded()
    {
        return isGrounded;
    }
    public float getLocalScaleX()
    {
        return transform.localScale.x;
    }
    public void SetLook(float look)
    {
        this.look = look;
    }
    public void SetCanDash()
    {
        canDash = true;
    }
    public void setCanMove(bool canMove)
    {
        this.canMove = canMove;
    }
    public bool getIsImmortal()
    {
        return isImmortal;
    }
    public bool GetIsDead()
    {
        return isDead;
    }
    public int GetHp()
    {
        return hp;
    }
    public int GetCurrentHp()
    {
        return currentHp;
    }
    public void SetFullHp()
    {
        currentHp = hp;
    }
    public void DreceasetHp()
    {
        --currentHp;
    }
    public void TakingDmg()
    {
        TakeDmg();
    }
    public bool getIsDashing()
    {
        return isDashing;
    }
    public void unlockingDash()
    {
        PlayerPrefs.SetInt("unlockDash", 1);
    }
    public bool getCanMove()
    {
        return canMove;
    }
    public void setFallTime()
    {
        fallTimeCounter = fallTime;
    }
    public void setBossDead(bool active)
    {
        bossDead = active;
    }
    public void setIsImmortal(bool active)
    {
        isImmortal = active;
    }
}
