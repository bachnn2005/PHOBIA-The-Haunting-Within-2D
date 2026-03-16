using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
public class PlayerUI : MonoBehaviour
{
    private PlayerMoveBehave player;
    private Animator animator;
    [SerializeField] private Text hp;
    // Start is called before the first frame update
    void Start()
    {
        animator = GetComponent<Animator>();
        player = GameObject.FindGameObjectWithTag("Player").GetComponent<PlayerMoveBehave>();
    }

    // Update is called once per frame
    void Update()
    {
        Debug.Log(player.GetCurrentHp());
        if (player.GetCurrentHp() >= 0)
        {
            hp.text = player.GetCurrentHp().ToString();
        }
        if(player.animator.GetBool("isTakingDmg"))
        {
            animator.SetBool("isTakingDmg",true);
        }
        else
        {
            animator.SetBool("isTakingDmg", false);
        }
    }
}
