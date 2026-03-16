using UnityEngine;

public class vines : MonoBehaviour
{
    private float vertical;
    public float speed = 8f;
    private bool isLadder;
    private bool isClimbing;

    [SerializeField] private  PlayerMoveBehave player;

    void Update()
    {
        vertical = Input.GetAxisRaw("Vertical");

        if (isLadder && Mathf.Abs(vertical) > 0f)
        {
            isClimbing = true;
        }
        if(isLadder)
        {
            player.setFallTime();
        }
    }

    private void FixedUpdate()
    {
        
        if (isClimbing)
        {
            
            player.rb.gravityScale = 0f;
            player.rb.linearVelocity = new Vector2(player.rb.linearVelocity.x, vertical * speed);
        }
        else if(!player.getIsDashing())
        {
            player.rb.gravityScale = 3f;
        }
    }

    private void OnTriggerEnter2D(Collider2D collision)
    {
        if (collision.CompareTag("Ladder"))
        {
            
            isLadder = true;
        }
    }

    private void OnTriggerExit2D(Collider2D collision)
    {
        if (collision.CompareTag("Ladder"))
        {
            isLadder = false;
            isClimbing = false;
        }
    }
}