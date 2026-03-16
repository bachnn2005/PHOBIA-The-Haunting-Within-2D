using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.UI;

public class BedScript : MonoBehaviour
{
    [SerializeField] private ManageScene m;
    private bool isInteract;
    private bool interactWithBed;
    private bool writtenDiary;
    [SerializeField] private SpriteRenderer giuongText;
    [SerializeField] private MainCharRW mainChar;
    [SerializeField] private GameObject dialoguePanel;
    [SerializeField] private Text dialogueText;
    [SerializeField] private float wordSpeed;
    [SerializeField] private string s;
    [SerializeField] private string s2;
    [SerializeField] private string s3;
    [SerializeField] private TableScript diary;
    [SerializeField] private GameObject startScreen;
    [SerializeField] private GameObject endScreen;
    private bool isTransScreen;
    // Start is called before the first frame update
    void Start()
    {
        dialogueText.text = "";
    }

    // Update is called once per frame
    void Update()
    {
        if (isTransScreen)
        {
            return;
        }
        if(PlayerPrefs.GetInt("TimeOfDay") == 0)
        {
            if (diary.GetDone())
            {
                writtenDiary = true;
            }
            if (writtenDiary)
            {
                VietXongNhatKy();
            }
            else
            {
                ChuaXongNhatKy();
            }
        }
        else
        {
            if (isInteract && (Input.GetKeyDown(KeyCode.UpArrow) || Input.GetKeyDown(KeyCode.W)))
            {
                mainChar.SetfalseCanMove();
                isInteract = false;
                dialoguePanel.SetActive(true);
                StartCoroutine(Typing(s3));
                interactWithBed = true;
            }
            else if (interactWithBed && Input.anyKeyDown && dialogueText.text == s3)
            {
                ClearText();
                mainChar.SetTrueCanMove();
                interactWithBed = false;
                dialoguePanel.SetActive(false);
                isInteract = true;
            }
        }
    }
    private void ChuaXongNhatKy()
    {
        if (isInteract && (Input.GetKeyDown(KeyCode.W) || Input.GetKeyDown(KeyCode.UpArrow)))
        {
            mainChar.SetfalseCanMove();
            isInteract = false;
            dialoguePanel.SetActive(true);
            StartCoroutine(Typing(s2));
            interactWithBed = true;
        }
        else if (interactWithBed && Input.anyKeyDown && dialogueText.text == s2)
        {
            ClearText();
            mainChar.SetTrueCanMove();
            interactWithBed = false;
            dialoguePanel.SetActive(false);
            isInteract = true;
        }
    }
    private void VietXongNhatKy()
    {
        if (isInteract && (Input.GetKeyDown(KeyCode.W) || Input.GetKeyDown(KeyCode.UpArrow)))
        {
            mainChar.SetfalseCanMove();
            isInteract = false;
            dialoguePanel.SetActive(true);
            StartCoroutine(Typing(s));
            interactWithBed = true;
        }
        else if (interactWithBed && Input.anyKeyDown && dialogueText.text == s)
        {
            ClearText();
            mainChar.SetTrueCanMove();
            interactWithBed = false;
            dialoguePanel.SetActive(false);
            isInteract = true;
            StartCoroutine(TransScreen());
        }
    }
    private IEnumerator TransScreen()
    {
        isTransScreen = true;
        startScreen.SetActive(true);
        yield return new WaitForSeconds(1f);
        m.ChangeScene();
        PlayerPrefs.SetInt("TimeOfDay", 1);
    }
    private void ClearText()
    {
        dialogueText.text = "";
    }
    private IEnumerator Typing(string s)
    {
        ClearText();
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
            giuongText.transform.localScale = Vector3.Lerp(Vector3.zero, Vector3.one, timeElapsed / 0.1f);
            timeElapsed += Time.deltaTime;
            yield return null;
        }
        giuongText.transform.localScale = Vector3.one;
    }
    private IEnumerator ScaleOut()
    {
        float timeElapsed = 0f;

        while (timeElapsed < 0.1f)
        {
            giuongText.transform.localScale = Vector3.Lerp(Vector3.one, Vector3.zero, timeElapsed / 0.1f);
            timeElapsed += Time.deltaTime;
            yield return null;
        }
        giuongText.transform.localScale = Vector3.zero;
    }
}
