using System.Collections;
using System.Collections.Generic;
using System.Runtime.CompilerServices;
using UnityEngine;
using UnityEngine.SceneManagement;

public class Day2Sang : MonoBehaviour
{
    // Start is called before the first frame update
    [SerializeField] private Animator mainCharAni;
    private GameManager gameManager;
    private bool goiDien;
    private bool viet;
    [SerializeField] private GameObject mainChar;
    [SerializeField] private GameObject sitEvent;
    [SerializeField] private string[] goiChoMe;
    [SerializeField] private TableScript table;
    [SerializeField] private ManageScene sm;
    private int count = 0;
    private bool isDone;
    void Start()
    {
        gameManager = FindAnyObjectByType<GameManager>();
        StartCoroutine(StartScene());
        count += gameManager.getDialogueLength() + goiChoMe.Length;
    }

    // Update is called once per frame
    void Update()
    {
        if (gameManager.getCount() == count + table.NhatKy().Length && !isDone)
        {
            gameManager.StartScreen(true);
            table.TypingSoundStop();
            Invoke("ChangeScene", 1f);
            isDone= true;
        }
        if (viet)
        {
            return;
        }
        if(gameManager.getCount() == gameManager.getDialogueLength() && !goiDien)
        {
            StartCoroutine(GoiDienChoMe());
        }
        if(gameManager.getCount() == count)
        {
            viet = true;
        }
        if(viet)
        {
            StartCoroutine(VietNhatKy());
        }
        
    }
    private void ChangeScene()
    {
        sm.ChangeScene();
    }
    private IEnumerator VietNhatKy()
    {

        gameManager.StartScreen(true);
        yield return new WaitForSeconds(1f);
        sitEvent.SetActive(true);
        mainChar.SetActive(false);
        yield return new WaitForSeconds(0.5f);
        gameManager.EndScreen(true);
        gameManager.StartScreen(false);
        table.TypingSoundPlay();
        yield return new WaitForSeconds(1f);
        gameManager.EndScreen(false);
        gameManager.OpenDialogue(table.NhatKy());
    }
    private IEnumerator GoiDienChoMe()
    {
        goiDien = true;
        mainCharAni.SetBool("GoiDienChoMe", true);
        yield return new WaitForSeconds(2.2f);
        mainCharAni.SetBool("GoiDienChoMe2", true);
        gameManager.OpenDialogue(goiChoMe);
    }
    private IEnumerator StartScene()
    {
        yield return new WaitForSeconds(1.5f);
        mainCharAni.SetBool("BatDay", true);
        yield return new WaitForSeconds(0.65f);
        mainCharAni.SetBool("BatDayTho", true);
    }
}
