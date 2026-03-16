using Cinemachine;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ImagineFriend : MonoBehaviour
{
    private NPCScript npc;
    private bool hasTalked = false;
    private Animator animator;

    void Start()
    {
        npc = GetComponent<NPCScript>();
        animator = GetComponent<Animator>();
    }

    private void Update()
    {
        if (hasTalked && !npc.isTalking)
        {
            animator.SetBool("isDisappearing", true);
            Invoke("Disappear", 3f);
            hasTalked = false;
        }
    }
    private void Disappear()
    {
        Destroy(gameObject);
    }
    private void OnTriggerEnter2D(Collider2D other)
    {
        if (other.CompareTag("Player") && npc != null && !hasTalked)
        {
            if (!npc.isTalking)
            {
                GetComponent<BoxCollider2D>().enabled = false;
                npc.OpenDialog();
                hasTalked = true;
            }
        }
    }
   
}

