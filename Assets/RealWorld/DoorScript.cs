using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.UI;
public class DoorScript : MonoBehaviour
{
    [SerializeField] private ManageScene m;
    private bool isInteract;
    private bool interactWithDoor;
    [SerializeField] private SpriteRenderer cuaText;
    [SerializeField] private MainCharRW mainChar;
    [SerializeField] private GameObject dialoguePanel;
    [SerializeField] private Text dialogueText;
    [SerializeField] private float wordSpeed;
    [SerializeField] private string s;
    [SerializeField] private string diDenLop;
    [SerializeField] private GameObject startScreen;
    // Start is called before the first frame update
    void Start()
    {
        dialogueText.text = "";
    }

    // Update is called once per frame
    void Update()
    {
        if (PlayerPrefs.GetInt("TimeOfDay") == 1)
        {
            if (isInteract && Input.GetKeyDown(KeyCode.W))
            {
                mainChar.SetfalseCanMove();
                isInteract = false;
                dialoguePanel.SetActive(true);
                StartCoroutine(Typing(diDenLop));
                interactWithDoor = true;
            }
            else if (interactWithDoor && Input.anyKeyDown && dialogueText.text == diDenLop)
            {
                ClearText();
                StartCoroutine(CloseScreen());
                interactWithDoor = false;
                dialoguePanel.SetActive(false);
                isInteract = true;
            }
        }
        else
        {
            if (isInteract && (Input.GetKeyDown(KeyCode.UpArrow) || Input.GetKeyDown(KeyCode.W)))
            {
                mainChar.SetfalseCanMove();
                isInteract = false;
                dialoguePanel.SetActive(true);
                StartCoroutine(Typing(s));
                interactWithDoor = true;
            }
            else if (interactWithDoor && Input.anyKeyDown && dialogueText.text == s)
            {
                ClearText();
                mainChar.SetTrueCanMove();
                interactWithDoor = false;
                dialoguePanel.SetActive(false);
                isInteract = true;
            }
        }


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
    #region dialog
    private void ClearText()
    {
        dialogueText.text = "";
    }
    #endregion
    private void OnTriggerEnter2D(Collider2D collision)
    {
        if (collision.CompareTag("Player") && mainChar.GetIsFacingRight())
        {
            StartCoroutine(ScaleIn());
            isInteract = true;
        }
    }
    private void OnTriggerStay2D(Collider2D collision)
    {
        
    }

    private void OnTriggerExit2D(Collider2D collision)
    {
        if (collision.CompareTag("Player"))
        {
            isInteract= false;
            StartCoroutine(ScaleOut());
        }
    }
    private IEnumerator ScaleIn()
    {
        float timeElapsed = 0f;

        while (timeElapsed < 0.1f)
        {
            cuaText.transform.localScale = Vector3.Lerp(Vector3.zero, Vector3.one, timeElapsed / 0.1f);
            timeElapsed += Time.deltaTime;
            yield return null;
        }
        cuaText.transform.localScale = Vector3.one;
    }
    public void ChangeScene(string sceneName)
    {
        SceneManager.LoadScene(sceneName);
    }
    private IEnumerator ScaleOut()
    {
        float timeElapsed = 0f;

        while (timeElapsed < 0.1f)
        {
            cuaText.transform.localScale = Vector3.Lerp(Vector3.one, Vector3.zero, timeElapsed / 0.1f);
            timeElapsed += Time.deltaTime;
            yield return null;
        }
        cuaText.transform.localScale = Vector3.zero;
    }
    private IEnumerator CloseScreen()
    {
        startScreen.SetActive(true);
        yield return new WaitForSeconds(2f);
        m.ChangeScene();
    }

}
