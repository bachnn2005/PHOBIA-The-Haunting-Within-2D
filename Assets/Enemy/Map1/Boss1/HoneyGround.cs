using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class HoneyGround : MonoBehaviour
{
    // Start is called before the first frame update
    void Start()
    {
        StartCoroutine(s());
    }

    // Update is called once per frame
    void Update()
    {
        
    }
    private IEnumerator s()
    {
        yield return new WaitForSeconds(5f);
        Destroy(gameObject);
    }
}
