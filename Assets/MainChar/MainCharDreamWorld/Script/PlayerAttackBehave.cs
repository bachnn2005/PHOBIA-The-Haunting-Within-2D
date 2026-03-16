using System.Collections;
using System.Collections.Generic;
using UnityEditor.Rendering;
using UnityEngine;

public class PlayerAttackBehave : MonoBehaviour
{
    [SerializeField] private GameObject upDownSlash;
    [SerializeField] private PlayerMoveBehave player;
    [SerializeField] private GameObject sideSlash;
    [SerializeField] private bool isAttacking;
    [SerializeField] private Transform playerTransform_;
    public bool onTrigger;
    [Header("SlashOffset")]
    [SerializeField] private Vector3 offsetRight;
    [SerializeField] private Vector3 offsetLeft;
    private bool isDownAttacking;
    private bool isUpAttacking;
    // Start is called before the first frame update
    void Start()
    {
        upDownSlash.GetComponent<PolygonCollider2D>().enabled = false;
        sideSlash.GetComponent<PolygonCollider2D>().enabled = false;    
    }

    // Update is called once per frame
    void Update()
    {
        FlipUpDownAttack();
        UpDownAttackPosition();
        SideAttackPosition();
        if (player.getAttacking() && player.getLook() != 0 && !isAttacking)
        {
            if (!(player.getLook() < 0f && player.GetisGrounded()))
            StartCoroutine(UpDownAttack());
        }

        if(player.getAttacking() && (player.getLook() == 0f || player.getLook() < 0f && player.GetisGrounded() ) && !isAttacking)
        {
            StartCoroutine(SideAttack());
        }

    }
    private IEnumerator SideAttack()
    {
        isAttacking = true;
        sideSlash.GetComponent<PolygonCollider2D>().enabled = true;
        onTrigger = false;
        yield return new WaitForSeconds(0.2f);
        sideSlash.GetComponent<PolygonCollider2D>().enabled = false;
        yield return new WaitForSeconds(0.2f);
        isAttacking = false;
    }
    private IEnumerator UpDownAttack()
    {
        isAttacking=true;
        onTrigger = false;
        if (player.getLook()>0f)
        {
            isUpAttacking = true;
        }
        else
        {
            isDownAttacking = true;
        }
        upDownSlash.GetComponent<PolygonCollider2D>().enabled = true;
        yield return new WaitForSeconds(0.2f);
        upDownSlash.GetComponent<PolygonCollider2D>().enabled = false;
        yield return new WaitForSeconds(0.2f);
        isAttacking = false;
        isUpAttacking=false;
        isDownAttacking=false;
    }
    private void SideAttackPosition()
    {
        sideSlash.transform.position = playerTransform_.position + (player.GetisFacingRight() ? offsetRight : offsetLeft);
        sideSlash.transform.localScale = playerTransform_.localScale;   
    }
    private void UpDownAttackPosition()
    {
        if(isDownAttacking) 
        {
            upDownSlash.transform.position = playerTransform_.position + new Vector3(0f, 1.35f, 0f);
        }
        else if(isUpAttacking)
        {
            upDownSlash.transform.position = playerTransform_.position + new Vector3(0f, 4.16f, 0f);
        }
        //upDownSlash.transform.position = playerTransform_.position + (player.getLook() < 0f ? new Vector3(0f, 1.35f, 0f): new Vector3(0f, 4.16f, 0f)) ;
    }
    private void FlipUpDownAttack()
    {
        if(player.getLook() < 0f && !isAttacking && !player.GetisGrounded())
        {
            upDownSlash.transform.rotation = Quaternion.Euler(180f, 0f, 0f);
            upDownSlash.transform.localScale = playerTransform_.localScale;
        }
        else if(player.getLook() > 0f && !isAttacking)
        {
            upDownSlash.transform.rotation = Quaternion.identity;
            upDownSlash.transform.localScale = playerTransform_.localScale;
        }
    }
}
