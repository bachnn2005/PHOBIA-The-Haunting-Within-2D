using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class DrinkSelect : MonoBehaviour
{

    [SerializeField] private int count = 1;
    // Start is called before the first frame update
    void Start()
    {
        
    }

    // Update is called once per frame
    void Update()
    {

        if(Input.GetKeyDown(KeyCode.D) || Input.GetKeyDown(KeyCode.RightArrow))
        {   
            if(count == 3)
            {
                count = 1;
                transform.position += new Vector3(-4.1f, 0f, 0f);
            }
            else
            {
                count++;
                transform.position += new Vector3(2.05f, 0f, 0f);
            }
        }
        if (Input.GetKeyDown(KeyCode.A) || Input.GetKeyDown(KeyCode.LeftArrow))
        {
            if (count == 1)
            {
                count = 3;
                transform.position += new Vector3(4.1f, 0f, 0f);
            }
            else
            {
                count--;
                transform.position += new Vector3(-2.05f, 0f, 0f);
            }
        }
    }
}
