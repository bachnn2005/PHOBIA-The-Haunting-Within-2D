using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class TrainingDummy : MonoBehaviour
{
    // Start is called before the first frame update
    [SerializeField] private DmgFlash _dmgFlash;
    [SerializeField] private Animator animator;
    [SerializeField] private PlayerMoveBehave player;
    void Start()
    {
        
    }

    // Update is called once per frame
    void Update()
    {
        
    }
    private void OnTriggerEnter2D(Collider2D collision)
    {
        if(collision.CompareTag("Slash"))
        {
            animator.SetTrigger("isAttacked");
            if(player.GetisFacingRight())
            {
                animator.SetInteger("AttackedIndex", 1);
            }
            else
            {
                animator.SetInteger("AttackedIndex", 2);
            }
            _dmgFlash.CallDmgFlash();
        }
    }
}
