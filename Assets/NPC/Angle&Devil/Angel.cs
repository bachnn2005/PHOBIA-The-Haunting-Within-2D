
using UnityEngine;

public class Angel : MonoBehaviour
{
    private NPCScript npc;
    private bool hasTalked = false;
    private PlayerMoveBehave player;
    private bool isFacingRight;
    private BoxCollider2D triggerBox;
    // Start is called before the first frame update
    void Start()
    {
        player = GameObject.FindGameObjectWithTag("Player").GetComponent<PlayerMoveBehave>();
        npc = GetComponent<NPCScript>();
        triggerBox = GetComponent<BoxCollider2D>();
    }

    // Update is called once per frame
    void Update()
    {
        if (npc.playerIsClose && !hasTalked)
        {
            if (!npc.isTalking)
            {
                npc.OpenDialog();
                hasTalked = true;
            }
        }
        Flip();
    }
    private void Flip()
    {
        if(transform.position.x - player.transform.position.x > 0f && !isFacingRight || transform.position.x - player.transform.position.x < 0f && isFacingRight)
        {
            isFacingRight = !isFacingRight;
            Vector3 localScale = transform.localScale;
            localScale.x *= -1;
            transform.localScale = localScale;
        }
    }
    private void OnTriggerEnter2D(Collider2D other)
    {
        if (other.CompareTag("Player") && npc != null && !hasTalked)
        {
            if (!npc.isTalking)
            {
                npc.OpenDialog();
                hasTalked = true;
                Destroy(triggerBox);
            }
        }
    }
}
