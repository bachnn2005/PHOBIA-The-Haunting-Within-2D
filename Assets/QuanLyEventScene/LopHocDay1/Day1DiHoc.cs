using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.AI;

public class Day1DiHoc : MonoBehaviour
{
    // Start is called before the first frame update
    [SerializeField] private Animator cogiaoAni;
    [SerializeField] private Animator playerAni;
    [SerializeField] private GameManager gameManager;
    [SerializeField] private Rigidbody2D playerRb;
    [SerializeField] private GameObject lungTung;
    private bool isDiDenXinCo;
     void Start()
    {
        StartCoroutine(DiDenXinCo());
    }

    // Update is called once per frame
    void Update()
    {

        if (gameManager.getCount() == 1)
        {
            playerAni.SetBool("XinXo2", true);
        }
        else if(gameManager.getCount() == 2)
        {
            StartCoroutine(CoGiaoLacDau());
        }
        else if(gameManager.getCount() ==3)
        {
            playerAni.SetBool("XinXo3", true);
        }
    }
    private IEnumerator CoGiaoLacDau()
    {
        cogiaoAni.SetBool("LacDau", true);
        yield return new WaitForSeconds(2f);
        cogiaoAni.SetBool("LacDau", false);
    }
    private IEnumerator DiDenXinCo()
    {
        
        yield return new WaitForSeconds(2f);
        lungTung.SetActive(true);
        yield return new WaitForSeconds(1f);
        lungTung.SetActive(false);
        playerAni.SetBool("isWalking", true);
        playerRb.linearVelocity = new Vector2(-3f, 0f);   
        yield return new WaitForSeconds(2.5f);
        playerAni.SetBool("isWalking", false);
        playerRb.linearVelocity = Vector2.zero;        

    }
}
