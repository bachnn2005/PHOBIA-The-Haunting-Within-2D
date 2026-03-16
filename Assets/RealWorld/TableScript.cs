using System.Collections;
using System.Collections.Generic;
using UnityEditor.Rendering;
using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.UI;
public class TableScript : MonoBehaviour
{
    private bool isInteract;
    private bool interactWithTable;
    private bool exitable;
    private bool isTransScreen;
    private bool isDone;
    [SerializeField] private string doneWritting;
    [SerializeField] private string sang;
    [SerializeField] private SpriteRenderer banText;
    [SerializeField] private GameObject startScreen;
    [SerializeField] private GameObject endScreen;
    [SerializeField] private GameObject sitEvent;
    [SerializeField] private MainCharRW mainChar;
    //~~~~~~~~~~~~~dialogue~~~~~~~~~~~~~~~~~
    [SerializeField] private GameObject dialoguePanel;
    [SerializeField] private Text dialogueText;
    [SerializeField] private int index;
    [SerializeField] private string[] dialogue;
    [SerializeField] private float wordSpeed;
    //~~~~~~~~~~~~~audio~~~~~~~~~~~~~~~~~~~~~
    [SerializeField] private AudioSource TypingSound;
    [SerializeField] private AudioClip KeyBoard;
    // Start is called before the first frame update
    void Start()
    {
        TypingSound.clip = KeyBoard;
        dialogueText.text = "";
    }

    // Update is called once per frame
    void Update()
    {
        if(PlayerPrefs.GetInt("TimeOfDay") == 0 )
        {
            if (!isDone)
            {
                VietNhatKy();
            }
            else
            {
                if (isInteract && (Input.GetKeyDown(KeyCode.W) || Input.GetKeyDown(KeyCode.UpArrow)) )
                {
                    mainChar.SetfalseCanMove();
                    isInteract = false;
                    dialoguePanel.SetActive(true);
                    StartCoroutine(Done());
                    interactWithTable = true;
                }
                else if (Input.anyKeyDown && interactWithTable && dialogueText.text == doneWritting)
                {
                    mainChar.SetTrueCanMove();
                    interactWithTable = false;
                    dialoguePanel.SetActive(false);
                    ClearText();
                    isInteract = true;
                }
            }
        }
        else
        {
            if (isInteract && (Input.GetKeyDown(KeyCode.W) || Input.GetKeyDown(KeyCode.UpArrow)))
            {
                mainChar.SetfalseCanMove();
                isInteract = false;
                dialoguePanel.SetActive(true);
                StartCoroutine(Typing(sang));
                interactWithTable = true;
            }
            else if (Input.anyKeyDown && interactWithTable && dialogueText.text == sang)
            {
                mainChar.SetTrueCanMove();
                interactWithTable = false;
                dialoguePanel.SetActive(false);
                ClearText();
                isInteract = true;
            }
        }

    }
    private IEnumerator Done()
    {
        yield return new WaitForSeconds(0.75f);
        foreach (char letter in doneWritting)
        {
            dialogueText.text += letter;
            yield return new WaitForSeconds(wordSpeed);
        }
    }
    public void VietNhatKy()
    {
        if (index == dialogue.Length - 1 && dialogueText.text == dialogue[index])
        {
            exitable = true;
            index = 0;
        }
        //Co le minh nen viet nhat ki ahh brother
        if (isInteract && (Input.GetKeyDown(KeyCode.W) || Input.GetKeyDown(KeyCode.UpArrow)))
        {
            interactWithTable = true;
            mainChar.SetfalseCanMove();
            OpenDialogPanel();
            isInteract = false;
        }
        //Chuyen sang ngoi viet nhat ki
        else if (Input.anyKeyDown && interactWithTable && dialogueText.text == dialogue[index])
        {
            dialoguePanel.SetActive(false);
            StartCoroutine(SitTransScreen());
            interactWithTable = false;
        }
        //Viet nhat ki xong
        else if (Input.anyKeyDown && exitable)
        {
            TypingSound.Stop();
            ClearText();
            dialoguePanel.SetActive(false);
            StartCoroutine(TransScreenBack());
            mainChar.SetTrueCanMove();
            exitable = false;
            isDone = true;
        }
        if (dialogueText.text == dialogue[index])
        {
            if (Input.anyKeyDown && !isTransScreen)
            {
                NextLine();
            }
            else if (isTransScreen)
            {
                StartCoroutine(TransText());
            }
        }
    }
    #region dialog
    private IEnumerator TransText()
    {
        ClearText();
        yield return new WaitForSeconds(3f);
        NextLine();
    }
    private void OpenDialogPanel()
    {
        if(dialoguePanel.activeInHierarchy)
        {
            ZeroText();
        }
        else
        {
            dialoguePanel.SetActive(true);
            StartCoroutine(Typing(dialogue[index]));
        }
    }
    private void NextLine()
    {
        if (index < dialogue.Length - 1 && !isDone)
        {
            index++;
            ClearText();
            StartCoroutine(Typing(dialogue[index]));
        }
        else
        {
            
            ZeroText();
        }
    }
    private void ZeroText()
    {
        dialogueText.text = "";
        index = 0;
        dialoguePanel.SetActive(false);
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
    #endregion
    private void OnTriggerEnter2D(Collider2D collision)
    {
        if (collision.CompareTag("Player"))
        {
            StartCoroutine(ScaleIn());
            isInteract = true;
        }
    }
    private void OnTriggerExit2D(Collider2D collision)
    {
        if (collision.CompareTag("Player"))
        {
            isInteract = false;
            StartCoroutine(ScaleOut());
        }
    }
    private IEnumerator TransScreenBack()
    {
        isTransScreen = true;
        startScreen.SetActive(true);
        yield return new WaitForSeconds(1f);
        sitEvent.SetActive(false);
        mainChar.gameObject.SetActive(true);
        yield return new WaitForSeconds(0.5f);
        endScreen.SetActive(true);
        startScreen.SetActive(false);
        yield return new WaitForSeconds(1f);
        endScreen.SetActive(false);
        isTransScreen = false;
    }
    private IEnumerator SitTransScreen()
    {

        isTransScreen = true;
        startScreen.SetActive(true);
        yield return new WaitForSeconds(1f);
        sitEvent.SetActive(true);
        mainChar.gameObject.SetActive(false);
        yield return new WaitForSeconds(0.5f);
        endScreen.SetActive(true);
        startScreen.SetActive(false);
        TypingSound.Play();
        yield return new WaitForSeconds(1f);
        endScreen.SetActive(false);
        dialoguePanel.SetActive(true);
        isTransScreen = false;
    }
    
    private IEnumerator ScaleIn()
    {
        float timeElapsed = 0f;

        while (timeElapsed < 0.1f)
        {
            banText.transform.localScale = Vector3.Lerp(Vector3.zero, Vector3.one, timeElapsed / 0.1f);
            timeElapsed += Time.deltaTime;
            yield return null;
        }
        banText.transform.localScale = Vector3.one;
    }
    private IEnumerator ScaleOut()
    {
        float timeElapsed = 0f;

        while (timeElapsed < 0.1f)
        {
            banText.transform.localScale = Vector3.Lerp(Vector3.one, Vector3.zero, timeElapsed / 0.1f);
            timeElapsed += Time.deltaTime;
            yield return null;
        }
        banText.transform.localScale = Vector3.zero;
    }
    public bool GetDone()
    {
        return isDone;
    }
    public void TypingSoundPlay()
    {
        TypingSound.Play();
    }
    public void TypingSoundStop()
    {
        TypingSound.Stop();
    }
    public string[] NhatKy()
    {
        return dialogue;
    }
}
