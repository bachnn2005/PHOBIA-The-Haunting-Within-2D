using Cinemachine;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System.Threading;

public class OldFriend : MonoBehaviour
{
    private NPCScript npc;
    private bool hasTalked = false;

    void Start()
    {
        npc = GetComponent<NPCScript>();
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

    private void OnTriggerExit2D(Collider2D other)
    {
        if (other.CompareTag("Player"))
        {
            npc.playerIsClose = false;
        }
    }
}

