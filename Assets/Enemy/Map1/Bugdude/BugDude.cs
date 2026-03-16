using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class BugDude : MonoBehaviour
{
    private FlyEnemy flyBehave;
    [SerializeField] private float hp;
    private Slash slashDmg;
    [SerializeField] private Animator animator;
    [SerializeField] private BoxCollider2D triggerBox;
    private PlayerMoveBehave player;
    private DmgFlash _dmgFlash;
    private Rigidbody2D rb;
    private bool getHit;
    // Start is called before the first frame update
    void Start()
    {
        slashDmg = GameObject.FindGameObjectWithTag("Slash").GetComponent<Slash>();
        player = FindAnyObjectByType<PlayerMoveBehave>();
        rb = GetComponent<Rigidbody2D>();   
        flyBehave = GetComponent<FlyEnemy>();
        _dmgFlash = GetComponent<DmgFlash>();
    }

    // Update is called once per frame
    private void Update()
    {
        if(hp <= 0)
        {
            StartCoroutine(Dying());
        }
    }
    private void OnTriggerEnter2D(Collider2D collision)
    {
        if (collision.CompareTag("Slash") && !getHit)
        {
            StartCoroutine(GetHit());
            hp -= slashDmg.getSlashDmg();
            _dmgFlash.CallDmgFlash();
        }
    }
    private IEnumerator GetHit()
    {
        //isPursuing = true;
        getHit = true;
        if (player.getLook() > 0f)
        {
            transform.position += new Vector3(0f, 0.5f, 0f);
        }
        else if (player.getLook() < 0f)
        {
            transform.position += new Vector3(0f, -0.5f, 0f);
        }
        else
        {
            transform.position += new Vector3(0.5f * player.getLocalScaleX(), 0f, 0f);
        }
        flyBehave.SetFalseIsMoving();
        rb.linearVelocity = Vector2.zero;
        yield return new WaitForSeconds(0.15f);
        flyBehave.SetTrueIsMoving();
        yield return new WaitForSeconds(0.05f);
        getHit = false;
    }
    private IEnumerator Dying()
    {
        //noitice.enabled = false;
        animator.SetTrigger("isDead");
        rb.linearVelocity = Vector2.zero;
        flyBehave.SetFalseIsMoving();
        triggerBox.enabled = false;
        flyBehave.SetTrueIsDying();
        yield return new WaitForSeconds(0.5f);
        Destroy(gameObject);

    }
}

