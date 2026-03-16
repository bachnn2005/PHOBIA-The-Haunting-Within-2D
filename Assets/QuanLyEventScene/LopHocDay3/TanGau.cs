using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class TanGau : MonoBehaviour
{
    private int randomNumber;
    [SerializeField] private GameObject[] tanGau;
    [SerializeField] private float activeTime;
    private float timeCount = 0;
    // Start is called before the first frame update
    void Start()
    {
        
    }

    // Update is called once per frame
    void Update()
    {
        if(timeCount < 0)
        {
            randomNumber = Random.Range(0, tanGau.Length);
            StartCoroutine(f(tanGau[randomNumber]));
            timeCount = activeTime;
        }
        else
        {
            timeCount -= Time.deltaTime;
        }
    }
    private IEnumerator f(GameObject tangau)
    {
        tangau.SetActive(true);
        yield return new WaitForSeconds(0.5f);
        tangau.SetActive(false);
    }
}
