using Cinemachine;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class EnterBoss : MonoBehaviour
{
    [SerializeField] private CameraManager cameraManager;
    [SerializeField] private CinemachineVirtualCamera vcam;
    [SerializeField] private CinemachineVirtualCamera bossCam;
    [SerializeField] private Boss1 boss1;
    [SerializeField] private Boss2 boss2;
    [SerializeField] private Boss3 boss3;
    [SerializeField] private GameManager m;
    [SerializeField] private string[] bossDialog;
    [SerializeField] private float enterDuration = 2.25f;
    private PlayerMoveBehave player;
    private CinemachineFramingTransposer framingTransposer;
    private bool isEntered;
    private bool enterBossFight;
    public bool isDone;
    private bool isRestart;
    private bool playerDead;
    // Start is called before the first frame update
    void Start()
    {
        player = GameObject.FindGameObjectWithTag("Player").GetComponent<PlayerMoveBehave>();   
        framingTransposer = vcam.GetCinemachineComponent<CinemachineFramingTransposer>();
    }

    // Update is called once per frame
    void Update()
    {
        if(enterBossFight && (m.GetEndDialog() || playerDead) && !isDone)
        {
            enterBossFight = false; 
            isDone = true;
            IntoPhase1();
        }
        if(player.GetIsDead() && !isRestart && isEntered)
        {
            vcam.Priority = 10;
            bossCam.Priority = 0;
            cameraManager.ChangeVCam(vcam);
            StartCoroutine(ResetBoss());
        }
    }
    private void IntoPhase1()
    {
        if (boss1 != null)
        {
            boss1.gameObject.SetActive(true);
            boss1.IntoPhase1();
        }
        else if(boss2 != null)
        {
            Invoke("EnableBoss2", 2f);
        }
        else if(boss3 != null)
        {
            Invoke("EnableBoss3", 4f);
        }
    }
    private void EnableBoss2()
    {
        boss2.gameObject.SetActive(true);
        boss2.EnableHitBox(true);
    }
    private void EnableBoss3()
    {
        boss3.gameObject.SetActive(true);
        boss3.EnableHitBox(true);
        boss3.EnableSprite();
    }
    private void ChooseResetBoss()
    {
        if (boss1 != null)
        {
            boss1.ResetBoss();
        } 
        else if (boss2 != null)
        {
            boss2.ResetBoss();
        }
        else if (boss3 != null)
        {
            boss3.ResetBoss();
        }
    }
    private void ChooseActiveBoss(bool active)
    {
        if (boss1 != null)
        {
            boss1.gameObject.SetActive(active);
        }
        else if (boss2 != null)
        {
            boss2.gameObject.SetActive(active);
            
        }
        else if (boss3 != null)
        {
            boss3.gameObject.SetActive(active);
            
        }
    }
    private IEnumerator ResetBoss()
    {
        playerDead = true;
        enterBossFight = false;
        isDone = false;
        isRestart = true;
        yield return new WaitForSeconds(4f);
        ChooseResetBoss();
        ChooseActiveBoss(false);
        isRestart = false;
    }
    private void OnTriggerEnter2D(Collider2D collision)
    {
        if(collision.CompareTag("Player"))
        {
            enterBossFight = true;
            isEntered = true;
            if (isDone)
            {
                ChooseActiveBoss(true);
                StartCoroutine(EnterBossFight());
                return;
            }
            m.SetEndDialog(false);
            StartCoroutine(EnterBossFight());
            
        }

    }
    private IEnumerator EnterBossFight()
    {
        yield return new WaitForSeconds(enterDuration);
        vcam.Priority = 0;
        bossCam.Priority = 10;
        cameraManager.ChangeVCam(bossCam);
        if (framingTransposer != null)
        {
           framingTransposer.m_XDamping = 1f;
            framingTransposer.m_YDamping = 1f;
            framingTransposer.m_ZDamping = 1f;
        }
        yield return new WaitForSeconds(1f);
        if(!playerDead)
        {
            m.OpenDialogue(bossDialog);
        }
        
    }
    public bool getEnterBossFight()
    {
        return enterBossFight;  
    }
}
