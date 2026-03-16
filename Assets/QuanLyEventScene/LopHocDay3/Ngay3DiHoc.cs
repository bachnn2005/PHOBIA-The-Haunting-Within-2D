using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class Ngay3DiHoc : MonoBehaviour
{
    [SerializeField] private Animator mainChar;
    [SerializeField] private GameObject foGiao;
    [SerializeField] private GameManager gameManager;
    [SerializeField] private string[] coTuyenDuong;
    [SerializeField] private string[] nhatKy;
    [SerializeField] private string[] docThoai;
    [SerializeField] private string[] s;
    [SerializeField] private ManageScene m;
    [SerializeField] private GameObject transScreen;
    [SerializeField] private GameObject conNguoi;
    [SerializeField] private GameObject people;
    [SerializeField] private Text text;
    [SerializeField] private float wordSpeed;
    [SerializeField] private GameObject blackScreen;
    private bool isMinhLamDuoc;
    private int index = 0;
    private bool pharse2;
    private bool pharse3;
    private bool pharse4;
    private bool pharse5;
    private int count;
    // Start is called before the first frame update
    void Start()
    {
        text.text = "";
        count = gameManager.getDialogueLength() + coTuyenDuong.Length;
        Debug.Log(count);
    }

    // Update is called once per frame
    void Update()
    {

        if (gameManager.getCount() == gameManager.getDialogueLength() - 1)
        {
            StartCoroutine(GioTay());
        }
        else if(gameManager.getCount() == gameManager.getDialogueLength() && !pharse2)
        {
            StartCoroutine(TransScreen());
            pharse2 = true;
        }
        else if( gameManager.getCount() == count && !pharse3)
        {
            StartCoroutine(VietNhatKy());
            pharse3 = true;
        }
        else if(!pharse4 &&  gameManager.getCount() == count + nhatKy.Length)
        {
            Invoke("DocThoai", 3f);
            mainChar.SetTrigger("QuaySangNhin");
            pharse4 = true;
        }
        else if(!pharse5 && gameManager.getCount() == count + nhatKy.Length + docThoai.Length)
        {
            StartCoroutine(ChangeScene());
            pharse5 = true;
        }

        if(gameManager.getCount() == count - 1 && pharse2)
        {
            mainChar.SetBool("DungDayNgacNhien", true);
            
        }
    }
    private void DocThoai()
    {
        gameManager.OpenDialogue(docThoai);
    }
    private IEnumerator GioTay()
    {
        mainChar.SetTrigger("DoTay");
        yield return new WaitForSeconds(1f);
        mainChar.SetBool("DoTay 0", true);
    }
    private IEnumerator TransScreen()
    {
        gameManager.StartScreen(true);
        yield return new WaitForSeconds(0.5f);
        mainChar.SetBool("DungDay", true);

        yield return new WaitForSeconds(0.5f);
        StartCoroutine(MinhLamDuoc());
        while (!isMinhLamDuoc)
        {
            yield return null;
        }
        gameManager.EndScreen(true);
        gameManager.StartScreen(false);
        yield return new WaitForSeconds(1f);
        gameManager.EndScreen(false);
        gameManager.OpenDialogue(coTuyenDuong);
    }
    private IEnumerator VietNhatKy()
    {
        gameManager.StartScreen(true);
        yield return new WaitForSeconds(0.75f);
        conNguoi.SetActive(false);
        foGiao.SetActive(false);
        people.SetActive(true);
        mainChar.SetBool("NgoiViet", true);
        yield return new WaitForSeconds(0.75f);
        gameManager.EndScreen(true);
        gameManager.StartScreen(false);
        yield return new WaitForSeconds(1f);
        gameManager.EndScreen(false);
        gameManager.OpenDialogue(nhatKy);
        
    }
    private IEnumerator ChangeScene()
    {
        transScreen.SetActive(true);
        yield return new WaitForSeconds(1f);
        m.ChangeScene();
    }
    private IEnumerator MinhLamDuoc()
    {
        blackScreen.SetActive(true);
        StartCoroutine(Typing(s[index]));
        while (index < s.Length - 1)
        {
            if (text.text == s[index])
            {
                yield return new WaitForSeconds(1f);
                NextLine();
            }
            yield return null;
        }
        yield return new WaitForSeconds(2f);
        zeroText();
        blackScreen.SetActive(false);
        gameManager.gameObject.SetActive(true);
        isMinhLamDuoc = true;
    }
    private IEnumerator Typing(string s)
    {

        yield return new WaitForSeconds(0.75f);
        text.text = "";
        foreach (char letter in s)
        {
            text.text += letter;
            yield return new WaitForSeconds(wordSpeed);
        }

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

