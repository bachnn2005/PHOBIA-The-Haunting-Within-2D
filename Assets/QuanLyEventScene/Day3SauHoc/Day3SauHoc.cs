using System.Collections;
using System.Collections.Generic;
using UnityEditor.Rendering;
using UnityEngine;
using UnityEngine.UI;

public class Day3SauHoc : MonoBehaviour
{
    // Start is called before the first frame update
    [SerializeField] private string[] s;
    [SerializeField] private string[] choiPhobia;
    [SerializeField] private GameManager gameManager;
    [SerializeField] private Text text;
    [SerializeField] private float wordSpeed;
    [SerializeField] private GameObject blackScreen;
    [SerializeField] private Animator mainCharAni;
    [SerializeField] private GameObject anhBanMoi;
    [SerializeField] private GameObject anhBanMoiNgoi;
    private bool isTyping;
    private int index = 0;
    private bool intoPhase2;
    private bool intoPhase3;
    private bool intoPhase4;
    void Start()
    {
        blackScreen.SetActive(true);
        gameManager.gameObject.SetActive(false);
        text.text = "";
        StartCoroutine(Typing(s[0]));
        StartCoroutine(StartScreen());
    }

    // Update is called once per frame
    void Update()
    {
        if(gameManager.getDialogueLength() == gameManager.getCount() && !intoPhase2)
        {
            intoPhase2 = true;
            StartCoroutine(ChoiPhobia());
        }
        else if( gameManager.getCount() == 4 && !intoPhase3)
        {
            intoPhase3 = true;
            mainCharAni.SetTrigger("quaySangNhin");
        }
        else if(gameManager.getCount() ==  choiPhobia.Length && !intoPhase4 )
        {
            intoPhase4 = true;
            StartCoroutine(gameManager.CloseScreen());
        }
    }
    
    private IEnumerator ChoiPhobia()
    {
        gameManager.StartScreen(true);
        yield return new WaitForSeconds(0.5f);
        mainCharAni.SetBool("ngoiChoiPhobia", true);
        anhBanMoi.SetActive(false);
        anhBanMoiNgoi.SetActive(true);
        yield return new WaitForSeconds(0.5f);
        gameManager.EndScreen(true);
        gameManager.StartScreen(false);
        yield return new WaitForSeconds(1f);
        gameManager.ResetCount();
        gameManager.OpenDialogue(choiPhobia);
    }
    private IEnumerator StartScreen()
    {
        while(index < s.Length - 1)
        {
            if(text.text == s[index])
            {
                yield return new WaitForSeconds(1f);
                NextLine();
            }
            yield return null;
        }
        yield return new WaitForSeconds(2f);
        zeroText();
        gameManager.gameObject.SetActive(true);
        yield return new WaitForSeconds(1.1f);
        blackScreen.SetActive(true);
        yield return new WaitForSeconds(1f);
        blackScreen.SetActive(false);
        yield return new WaitForSeconds(0.3f);
        mainCharAni.SetTrigger("TinhDay");
    }
    private IEnumerator Typing(string s)
    {
        isTyping = true;
        yield return new WaitForSeconds(0.75f);
        text.text = "";
        foreach (char letter in s)
        {
            text.text += letter;
            yield return new WaitForSeconds(wordSpeed);
        }
        isTyping = false;
    }
    private void NextLine()
    {
        if (index < s.Length - 1)
        {
            index++;
            text.text = "";
            StartCoroutine(Typing(s[index]));
        }
        else
        {
            zeroText();
        }
    }
    private void zeroText()
    {
        text.text = "";
        index = 0;
    }
}
