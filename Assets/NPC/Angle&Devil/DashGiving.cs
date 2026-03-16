using System.Collections;
using UnityEngine;

public class DashGiving : MonoBehaviour
{
    private NPCScript npc;
    private PlayerMoveBehave player;
    [SerializeField] private GameObject dashGiving;
    [SerializeField] private GameObject dashGivingUI;
    [SerializeField] private Animator dashGivingAni;
    [SerializeField] private GameObject dashGivingSmoke;
    private bool isGiving;
    private bool isDone;
    // Start is called before the first frame update
    void Start()
    {
        player = GameObject.FindGameObjectWithTag("Player").GetComponent<PlayerMoveBehave>();
        npc = GetComponent<NPCScript>();
    }

    // Update is called once per frame
    void Update()
    {
        if(npc.getCount() == npc.getDialogLength() && !isGiving)
        {
            StartCoroutine(GivingDash());
            isGiving = true;
        }
        if(Input.anyKeyDown && isDone)
        {
            StartCoroutine(Done());
            isDone = false;
        }
    }
    private IEnumerator Done()
    {
        dashGivingAni.SetTrigger("isDone");
        yield return new WaitForSeconds(2f);
        player.setCanMove(true);
    }
    private IEnumerator GivingDash()
    {
        player.setCanMove(false);
        player.rb.linearVelocity = Vector2.zero;
        Vector2 target = (Vector2)player.transform.position + new Vector2(0f, -2f);
        dashGiving.SetActive(true);
        yield return new WaitForSeconds(1f);
        while(Vector2.Distance(player.transform.position,dashGiving.transform.position) > 2.75f)
        {
            dashGiving.transform.position = Vector2.MoveTowards(dashGiving.transform.position,target,2f*Time.deltaTime);
            yield return null;
        }
        dashGivingSmoke.SetActive(true);
        yield return new WaitForSeconds(0.8f);
        dashGivingSmoke.SetActive(false);
        dashGivingUI.SetActive(true);
        player.unlockingDash();
        dashGiving.SetActive(false);
        yield return new WaitForSeconds(2f);
        isDone = true;
    }
}
