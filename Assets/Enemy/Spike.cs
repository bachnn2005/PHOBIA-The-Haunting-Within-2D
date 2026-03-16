using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UIElements;

public class Spike : MonoBehaviour
{
    [SerializeField] private GameObject point;
    [SerializeField] private GameObject startScreen;
    [SerializeField] private GameObject endScreen;
    private PlayerMoveBehave player;
    private bool spiked;
    // Start is called before the first frame update
    void Start()
    {
        player = GameObject.FindGameObjectWithTag("Player").GetComponent<PlayerMoveBehave>();  
    }

    // Update is called once per frame
    void Update()
    {
        //if(spiked)
        //{
        //    player.rb.velocity = Vector2.zero;
        //}

    }
    private void OnTriggerEnter2D(Collider2D collision)
    {
        if (collision.CompareTag("Player") && !spiked && player.GetCurrentHp() > 0f)
        {
            player.DreceasetHp();
            player.TakingDmg();
            if (!player.GetIsDead())
            {
                StartCoroutine(Spiking(collision));
            }

        }
    }
    private IEnumerator Spiking(Collider2D collision)
    {
        
        spiked = true;
        startScreen.SetActive(true);
        player.setCanMove(false);
        yield return new WaitForSeconds(1f);
        player.rb.linearVelocity = Vector2.zero;
        collision.transform.position = point.transform.position + new Vector3(0f, -2f, 0f);
        yield return new WaitForSeconds(0.5f);
        endScreen.SetActive(true);
        startScreen.SetActive(false);
        yield return new WaitForSeconds(0.1f);
        player.setCanMove(true);
        yield return new WaitForSeconds(0.45f);
        endScreen.SetActive(false);
        spiked = false;
    }
}