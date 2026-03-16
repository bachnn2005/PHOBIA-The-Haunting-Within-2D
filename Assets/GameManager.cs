using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.UI;
public class GameManager : MonoBehaviour
{
    [SerializeField] private ManageScene m;
    [SerializeField] private GameObject dialoguePanel;
    [SerializeField] private Text dialogueText;
    [SerializeField] private GameObject startScreen;
    [SerializeField] private GameObject endScreen;
    [SerializeField] private GameObject blackScreen;
    [SerializeField] private MainCharRW mainChar;
    [SerializeField] private PlayerMoveBehave mainCharDream;
    [SerializeField] private string[] dialogue;
    private int index;
    [SerializeField] private float wordSpeed;
    [SerializeField] private float startTime = 2f;
    [SerializeField] private float blackScreenTime = 1f;
    private bool changeSceneAble = false;
    private bool endline = false;
    private int count = 0;
    private bool endDialog;
    // Start is called before the first frame update
    void Start()
    {
        if("Day1DiHoc" == SceneManager.GetActiveScene().name)
        {
            StartCoroutine(OpenScreen());
            changeSceneAble=true;
        }
        if ("Day1Toi" == SceneManager.GetActiveScene().name)
        {
            StartCoroutine(OpenScreen());
            PlayerPrefs.SetInt("TimeOfDay", 0);
        }
        else if("Day2Sang" == SceneManager.GetActiveScene().name )
        {
            StartCoroutine(OpenScreen());
            PlayerPrefs.SetInt("TimeOfDay", 1);
        }
        else if("Day1Sang" == SceneManager.GetActiveScene().name)
        {
            changeSceneAble = true;
            dialoguePanel.SetActive(true);
            StartCoroutine(Typing(dialogue[index]));
        }
        else if("Map1Dream" == SceneManager.GetActiveScene().name)
        {
            StartCoroutine(OpenScreen());
        }
        else if("Day2DiHoc" == SceneManager.GetActiveScene().name)
        {
            changeSceneAble = true;
            StartCoroutine(OpenScreen());
        }
        else if("Day2Toi" == SceneManager.GetActiveScene().name)
        {
            StartCoroutine(OpenScreen());
            PlayerPrefs.SetInt("TimeOfDay", 0);
        }
        else if ("Day3DiHoc" == SceneManager.GetActiveScene().name)
        {
            StartCoroutine(OpenScreen());
        }
        else if("Day3Sang" == SceneManager.GetActiveScene().name)
        {
            changeSceneAble = true;
            StartCoroutine(OpenScreen());
        }
        else if("Map0Dream" == SceneManager.GetActiveScene().name)
        {
            StartCoroutine(OpenScreen());
        }
        else if("Day3SauHoc" == SceneManager.GetActiveScene().name)
        {
            StartCoroutine(OpenScreen());
        }
        else if("Map2Dream" == SceneManager.GetActiveScene().name)
        {
            StartCoroutine(OpenScreen());
        }
        else if("Map3Dream" == SceneManager.GetActiveScene().name)
        {
            StartCoroutine(OpenScreen());
        }
        else if("Day3Toi" == SceneManager.GetActiveScene().name)
        {
            PlayerPrefs.SetInt("TimeOfDay", 0);
            StartCoroutine(OpenScreen());
        }
        Debug.Log(PlayerPrefs.GetInt("TimeOfDay"));
        ClearText();
        if(mainCharDream != null)
        {
            mainCharDream.setCanMove(false);
        }
        if(mainChar != null)
        {
            mainChar.SetfalseCanMove();
        }
        
        
    }

    // Update is called once per frame
    void Update()
    {
       
        if (Input.anyKeyDown && endline)
        {
            NextLine(dialogue);
            count++;
        }
        if(dialoguePanel.activeSelf)
        {
            if(mainCharDream != null)
            {
                mainCharDream.setCanMove(false);
            }
        }
    }
    private void NextLine(string[] s)
    {
        if (index < s.Length - 1)
        {
            index++;
            dialogueText.text = "";
            StartCoroutine(Typing(s[index]));
        }
        else
        {
            if(mainCharDream != null)
            {
                mainCharDream.setCanMove(true);
            }
            if (mainChar != null)
            {
                mainChar.SetTrueCanMove();
            }
            endDialog = true;
            ZeroText();
            if (changeSceneAble)
            {
                StartCoroutine(CloseScreen());
            }
        }
    }
    private void ZeroText()
    {
        dialogueText.text = "";
        index = 0;
        endline = false;
        dialoguePanel.SetActive(false);
    }
    private IEnumerator Typing(string s)
    {
        if (s[0] == '>')
        {
              dialoguePanel.transform.rotation = Quaternion.Euler(0f, 180f, 0f);
        }
        else
        {
            dialoguePanel.transform.rotation = Quaternion.Euler(0f, 0f, 0f);
        }
        endline = false;
        yield return new WaitForSeconds(0.75f);
        foreach (char letter in s)
        {
            if (letter == '/')
            {
                yield return new WaitForSeconds(0.5f);
            }
            else if(letter != '>')
            {
                dialogueText.text += letter;
                yield return new WaitForSeconds(wordSpeed);
            }
        }
        endline = true;
    }
    private void ClearText()
    {
        dialogueText.text = "";
    }
    private IEnumerator OpenScreen()
    {
       
        blackScreen.SetActive(true);
        yield return new WaitForSeconds(blackScreenTime);
        blackScreen.SetActive(false);
        endScreen.SetActive(true);
        yield return new WaitForSeconds(startTime);
        dialoguePanel.SetActive(true);
        StartCoroutine(Typing(dialogue[index]));
        endScreen.SetActive(false);
    }
    public IEnumerator CloseScreen()
    {
        startScreen.SetActive(true);
        yield return new WaitForSeconds(3f);
        m.ChangeScene();
    }
    public void OpenDialogue(string[] s)
    {
        if(mainCharDream != null)
        {
            mainCharDream.setCanMove(false);
            mainCharDream.rb.linearVelocity = Vector2.zero;
        }
        endDialog = false;
        dialoguePanel.SetActive(true);
        dialogue = s;
        StartCoroutine(Typing(dialogue[index]));
    }
    public int getCount()
    {
        return count;
    }
    public int getDialogueLength()
    {
        return dialogue.Length;
    }
    public void EndScreen(bool active)
    {
        endScreen.SetActive(active);
    }
    public void StartScreen(bool active)
    {
        startScreen.SetActive(active);
    }
    public void ResetCount()
    {
        count = 0;
    }
    public bool GetEndDialog()
    {
        return endDialog;
    }
    public void SetEndDialog(bool endDialog)
    {
        this.endDialog = endDialog;
    }
    public void BlackScreen(bool active)
    {
        blackScreen.SetActive(active);  
    }
}
