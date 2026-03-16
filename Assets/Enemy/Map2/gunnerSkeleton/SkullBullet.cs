using System.Collections;
using System.Collections.Generic;
using Unity.VisualScripting;
using UnityEngine;

public class SkullBullet : MonoBehaviour
{
    // Start is called before the first frame update
    [SerializeField] private float speed;
    private gunner target;

    Rigidbody2D rb;
    void Start()
    {
        rb = GetComponent<Rigidbody2D>();
        target = FindObjectOfType<gunner>();
        //if (target != null)
        //{

        //    if (target.transform.localScale.x < 0)
        //    {
        //        rb.velocity = new Vector2(speed, 0f); // Bắn đạn sang trái
        //    }
        //    else
        //    {
        //        rb.velocity = new Vector2(-speed, 0f); // Bắn đạn sang phải
        //    }

        //}

        Invoke("DestroyGameObject", 2f);
    }
    
    private void DestroyGameObject()
    {
        Destroy(gameObject);
    }

    // Update is called once per frame
    void Update()
    {
        
    }
}
