using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class NPCScript : MonoBehaviour
{
    [SerializeField] private GameObject dialoguePanel;
    [SerializeField] private Text dialogueText;
    [SerializeField] private string[] dialogue;
    private PlayerMoveBehave player;
    private int index = 0;
    [SerializeField] private float wordSpeed;
    public bool playerIsClose = false;
    private bool isTyping = false;
    public bool isTalking = false;
    private int count = 0;
    void Start()
    {
        player = GameObject.FindGameObjectWithTag("Player").GetComponent<PlayerMoveBehave>();
        playerIsClose = false;
        dialogueText.text = "";
    }

    void Update()
    {
        if(playerIsClose && Input.GetButton("Vertical") && !isTalking)
        {
            player.rb.linearVelocity = Vector2.zero;
            player.setCanMove(false);
            dialoguePanel.SetActive(true);
            StartCoroutine(Typing(dialogue[index]));
            isTalking = true;
        }
        else if (Input.anyKeyDown && !isTyping && isTalking)
        {
            NextLine();
            count++;
        }
    }
    private void zeroText()
    {
        dialogueText.text = "";
        index = 0;
        dialoguePanel.SetActive(false);
    }

    private IEnumerator Typing(string s)
    {
        isTyping = true;
        yield return new WaitForSeconds(0.75f);
        dialogueText.text = "";
        foreach (char letter in s)
        {
            dialogueText.text += letter;
            yield return new WaitForSeconds(wordSpeed);
        }
        isTyping = false;
    }

    private void NextLine()
    {
        if (index < dialogue.Length - 1)
        {
            index++;
            dialogueText.text = "";
            StartCoroutine(Typing(dialogue[index]));
        }
        else
        {
            Invoke("playerImmortal", 3f);
            isTalking = false;
            player.setCanMove(true);
            zeroText();

        }
    }
    private void playerImmortal()
    {
        player.setIsImmortal(false);
    }
    private void OnTriggerEnter2D(Collider2D other)
    {
        if (other.CompareTag("Player"))
        {
            player.setIsImmortal(true);
            playerIsClose = true;
        }
    }

    private void OnTriggerExit2D(Collider2D other)
    {
        if (other.CompareTag("Player"))
        {
            playerIsClose = false;
        }
    }
    public void OpenDialog()
    {
        player.rb.linearVelocity = Vector2.zero;
        player.setCanMove(false);
        dialoguePanel.SetActive(true);
        StartCoroutine(Typing(dialogue[index]));
        isTalking = true;
    }
    public int getDialogLength()
    {
        return dialogue.Length; 
    }
    public int getCount()
    {
        return count;
    }
} 





