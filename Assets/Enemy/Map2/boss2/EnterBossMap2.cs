using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Tilemaps;

public class EnterBossMap2 : MonoBehaviour
{
    [SerializeField] private TilemapCollider2D wall;
    [SerializeField] private GameObject gate;
    private BoxCollider2D triggerBox;
    private PlayerMoveBehave player;
    [SerializeField] private float time;
    private bool playerDead;
    // Start is called before the first frame update
    void Start()
    {   
        player = GameObject.FindGameObjectWithTag("Player").GetComponent<PlayerMoveBehave>();
        triggerBox = GetComponent<BoxCollider2D>();
    }

    // Update is called once per frame
    void Update()
    {
        if (player.GetIsDead())
        {
            playerDead = true;
        }
        if (player.GetIsDead())
        {
            Invoke("EnableBox", 4f);
        }
    }
    private void EnableBox()
    {
        if (wall != null)
        {
            wall.offset = new Vector2(0f, 0f);
        }
        if(gate!= null)
        {
            gate.SetActive(false);
        }
        triggerBox.enabled = true;
    }
    private void OnTriggerEnter2D(Collider2D collision)
    {
        if(collision.CompareTag("Player"))
        {
            triggerBox.enabled = false;
            if (gate != null)
            {
                gate.SetActive(true);
            }
            if (wall != null)
            {
                wall.offset = new Vector2(0f, -2f);
            }
            if(!playerDead)
            {
                Invoke("DisableMove", time);
            }
        }
    }
    private void DisableMove()
    {
        player.rb.linearVelocity = Vector2.zero;
        player.setCanMove(false);
    }
}
