using Cinemachine;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Map0 : MonoBehaviour
{
    [SerializeField] private ManageScene sm;
    [SerializeField] private PlayerMoveBehave player;
    [SerializeField] private CinemachineVirtualCamera vcam;
    [SerializeField] private CinemachineVirtualCamera vcam2;
    [SerializeField] private GameManager gameManager;
    [SerializeField] private string[] s;
    private BoxCollider2D triggerBox;
    [SerializeField] private GameObject moveToPoint;
    [SerializeField] private GameObject boss3;
    [SerializeField] private Animator boss3Ani;
    [SerializeField] private GameObject blackScreen;
    private bool isDone;
    private int count = 0;
    // Start is called before the first frame update
    void Start()
    {
        count = gameManager.getDialogueLength();
        triggerBox = GetComponent<BoxCollider2D>();
    }

    // Update is called once per frame
    void Update()
    {
        if(gameManager.getCount() == count + s.Length  && !isDone)
        {
            player.rb.linearVelocity = Vector2.zero;
            player.setCanMove(false);
            StartCoroutine(Event2());
            isDone = true;
        }
    }
    private void OnTriggerEnter2D(Collider2D collision)
    {
        if (collision.CompareTag("Player"))
        {
            vcam.Priority = 0;
            vcam2.Priority = 10;
            StartCoroutine(Event());
            Destroy(triggerBox);
        }
    }
    private IEnumerator Event()
    {
        yield return new WaitForSeconds(1f);
        player.rb.linearVelocity = Vector2.zero;
        player.setCanMove(false);
        yield return new WaitForSeconds(2f);
        gameManager.OpenDialogue(s);
    }
    private IEnumerator Event2()
    {
        Vector2 move = moveToPoint.transform.position + new Vector3(0.5f, -2f, 0f);
        yield return new WaitForSeconds(1f);
        boss3Ani.SetBool("isMoving", true);
        while(Vector2.Distance(boss3.transform.position,move) > 0.5f)
        {
            boss3.transform.position = Vector2.MoveTowards(boss3.transform.position, move, 30f*Time.deltaTime);
            yield return null;
        }
        boss3Ani.SetTrigger("slashing");
        yield return new WaitForSeconds(0.1f);
        blackScreen.SetActive(true);
        sm.ChangeScene();
        
    }
    
}
