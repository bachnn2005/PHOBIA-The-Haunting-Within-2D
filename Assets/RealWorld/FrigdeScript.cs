using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class FrigdeScript : MonoBehaviour
{
    private bool isInteract;
    private bool interactWithFridge;
    private bool lastInteract;
    [SerializeField] private GameObject TuLanhMenu;
    [SerializeField] private SpriteRenderer tulanhText;
    [SerializeField] private GameObject startScreen;
    [SerializeField] private GameObject endScreen;
    [SerializeField] private MainCharRW mainChar;
    [SerializeField] private GameObject dialoguePanel;
    [SerializeField] private Text dialogueText;
    [SerializeField] private float wordSpeed;
    [SerializeField] private string[] s;

    private bool drink;
    private bool exitable;
    //~~~~~~~~~~~audio~~~~~~~~~~~~~~~
    [SerializeField] private AudioSource drinkingSound;
    [SerializeField] private AudioClip drinkingSFX;
    // Start is called before the first frame update
    void Start()
    {
        drinkingSound.clip = drinkingSFX; 
        ClearText();
    }

    // Update is called once per frame
    void Update()
    {
        if (PlayerPrefs.GetInt("TimeOfDay") ==0)
        {
            TuLanhVaoBuoiToi();
        }
        else
        {
            TuLanhVaoBuoiSang();
        }
       
    }
    private void InteractRoutine(int index)
    {
        if (isInteract && (Input.GetKeyDown(KeyCode.UpArrow) || Input.GetKeyDown(KeyCode.W)))
        {
            isInteract = false;
            interactWithFridge = true;
            mainChar.SetfalseCanMove();
            dialoguePanel.SetActive(true);
            StartCoroutine(Typing(s[index]));
        }
        else if (Input.anyKeyDown && interactWithFridge && dialogueText.text == s[index])
        {
            ClearText();
            dialoguePanel.SetActive(false);
            interactWithFridge = false;
            mainChar.SetTrueCanMove();
            isInteract = true;
        }
    }
    private void TuLanhVaoBuoiSang()
    {
        InteractRoutine(3);
    }
    private void TuLanhVaoBuoiToi()
    {
        if (!drink)
        {
            UongNuoc();
        }
        else
        {
            InteractRoutine(1);
        }
    }
    private void UongNuoc()
    {
        if (isInteract && (Input.GetKeyDown(KeyCode.W) || Input.GetKeyDown(KeyCode.UpArrow)))
        {
            isInteract = false;
            mainChar.SetfalseCanMove();
            interactWithFridge = true;
            dialoguePanel.SetActive(true);
            StartCoroutine(Typing(s[0]));
        }
        else if (Input.anyKeyDown && interactWithFridge && dialogueText.text == s[0])
        {
            ClearText();
            mainChar.SetfalseCanMove();
            dialoguePanel.SetActive(false);
            interactWithFridge = false;
            TuLanhMenu.SetActive(true);
            exitable = true;
        }
        else if (Input.GetButtonDown("Vertical") && exitable)
        {
            StartCoroutine(TransScreen());
            
            exitable = false;
            StartCoroutine(AfterDrink());
            
        }
        else if (Input.anyKeyDown && lastInteract)
        {
            drink = true;
            mainChar.SetTrueCanMove();
            lastInteract = false;
            dialoguePanel.SetActive(false);
            ClearText();
            isInteract = true;
        }
    } 
    private IEnumerator AfterDrink()
    {
        yield return new WaitForSeconds(2f);
        dialoguePanel.SetActive(true );
        yield return new WaitForSeconds(0.75f);
        foreach (char letter in s[2])
        {
            dialogueText.text += letter;
            yield return new WaitForSeconds(wordSpeed);
        }
        lastInteract = true;
    }
    private void ClearText()
    {
        dialogueText.text = "";
    }
    private IEnumerator Typing(string s)
    {
        yield return new WaitForSeconds(0.75f);
        foreach (char letter in s)
        {
            dialogueText.text += letter;
            yield return new WaitForSeconds(wordSpeed);
        }
    }
    private void OnTriggerEnter2D(Collider2D collision)
    {
        if (collision.CompareTag("Player"))
        {
            StartCoroutine(ScaleIn());
            isInteract = true;
        }
    }
    private IEnumerator TransScreen()
    {
        startScreen.SetActive(true);
        yield return new WaitForSeconds(0.5f);
        drinkingSound.Play();
        yield return new WaitForSeconds(1f);
        TuLanhMenu.SetActive(false);
        endScreen.SetActive(true);
        startScreen.SetActive(false);
        yield return new WaitForSeconds(1f);
        endScreen.SetActive(false);
    }
    private void OnTriggerExit2D(Collider2D collision)
    {
        if (collision.CompareTag("Player"))
        {
            isInteract = false;
            StartCoroutine(ScaleOut());
        }
    }
    private IEnumerator ScaleIn()
    {
        float timeElapsed = 0f;

        while (timeElapsed < 0.1f)
        {
            tulanhText.transform.localScale = Vector3.Lerp(Vector3.zero, Vector3.one, timeElapsed / 0.1f);
            timeElapsed += Time.deltaTime;
            yield return null;
        }
        tulanhText.transform.localScale = Vector3.one;
    }
    private IEnumerator ScaleOut()
    {
        float timeElapsed = 0f;

        while (timeElapsed < 0.1f)
        {
            tulanhText.transform.localScale = Vector3.Lerp(Vector3.one, Vector3.zero, timeElapsed / 0.1f);
            timeElapsed += Time.deltaTime;
            yield return null;
        }
        tulanhText.transform.localScale = Vector3.zero;
    }
}
