using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PlayerAudio : MonoBehaviour
{
    private PlayerMoveBehave player;
    [SerializeField] private Slash slash;
    [SerializeField] private Slash slash2;
    [SerializeField] private AudioSource musicSource;
    [SerializeField] private AudioSource sfx;
    [SerializeField] private AudioSource attackSfx;
    [SerializeField] private AudioSource walkSfx;
    [SerializeField] private AudioSource enemySfx;
    [SerializeField] private float startTime;
    [SerializeField] private AudioClip[] soundEffect;
    [SerializeField] private AudioClip backGround;
    private EnterBoss enterBoss;
    private bool isSlashing;
    private bool isWalkingSoundPlaying;
    private bool hasPlayedStandUpSound;
    private bool isDashing;
    private bool isJumping;
    private bool enemyDmg;
    private bool enterBossFight;
    // Start is called before the first frame update
    void Start()
    {
        enterBoss = GameObject.FindGameObjectWithTag("EnterBoss").GetComponent<EnterBoss>();
        player = GameObject.FindGameObjectWithTag("Player").GetComponent<PlayerMoveBehave>();
        Invoke("StartPlayBackGround", startTime);
    }
    
    // Update is called once per frame
    void Update()
    {
        if(enterBoss != null)
        {
            EnterBoss();
        }
        EnemyDamage();
        AttackSound();
        WalkSound();
        LandingSound();
        DashingSound();
        JumpingSound();
    }
    private void EnterBoss()
    {
        if(enterBoss.getEnterBossFight() && !enterBossFight)
        {
            Invoke("EnterBossMusic", 4f);
            enterBossFight = true;
        }
    }
    private void EnterBossMusic()
    {
        musicSource.clip = soundEffect[6];
        musicSource.Play();
    }
    private void EnemyDamage()
    {
        if ((slash.triggerSound || slash2.triggerSound) && !enemyDmg)
        {
            enemySfx.clip = soundEffect[5];
            enemySfx.Play();
            enemyDmg = true;
        }
        else if(!slash.triggerSound && !slash2.triggerSound)
        {
            enemyDmg = false;
        }
    }
    private void JumpingSound()
    {
        if(player.animator.GetBool("isJumping") && !isJumping && !player.animator.GetBool("isDashing"))
        {
            sfx.clip = soundEffect[3];
            sfx.Play();
            isJumping = true;
        }
        else if (!player.animator.GetBool("isJumping"))
        {
            isJumping = false;
        }
    }
    private void DashingSound()
    {
        if(player.animator.GetBool("isDashing") && !isDashing)
        {
            sfx.clip = soundEffect[4];
            sfx.Play();
            isDashing = true;
        }
        else if (!player.animator.GetBool("isDashing"))
        {
            isDashing = false;
        }
    }
    private void AttackSound()
    { 
        if (Input.GetButtonDown("Attack") && !isSlashing && !player.animator.GetBool("isTakingDmg") && !player.animator.GetBool("isFalling") && !player.animator.GetBool("isDead") && player.getCanMove() && !player.animator.GetBool("isDashing")) 
        {
            StartCoroutine(SlashSound());
        }
    }
    private void LandingSound()
    {
        if (player.animator.GetBool("isStandingUp") && !hasPlayedStandUpSound)
        {
            sfx.clip = soundEffect[2];
            sfx.Play();
            hasPlayedStandUpSound = true;
        }
        else if (!player.animator.GetBool("isStandingUp"))
        {
            hasPlayedStandUpSound = false;
        }
    }
    private void WalkSound()
    {
        if (player.animator.GetBool("isMoving") && !player.animator.GetBool("isJumping") && !player.animator.GetBool("isFalling") && Mathf.Abs(player.rb.linearVelocity.x) > 0f)
        {
            if (!isWalkingSoundPlaying)
            {
                walkSfx.clip = soundEffect[1];
                walkSfx.Play();
                isWalkingSoundPlaying = true;
            }
        }
        else
        {
            walkSfx.Stop();
            isWalkingSoundPlaying = false;
        }
    }
    private void StartPlayBackGround()
    {
        musicSource.clip = backGround;
        musicSource.Play();
    }
    private IEnumerator SlashSound()
    {
        isSlashing = true;
        attackSfx.clip = soundEffect[0];
        attackSfx.Play();
        yield return new WaitForSeconds(0.25f);
        isSlashing = false;
    }
}
