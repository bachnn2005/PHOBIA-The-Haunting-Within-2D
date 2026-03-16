using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class GameStartMap2 : MonoBehaviour
{
    // Start is called before the first frame update
    void Start()
    {
        PlayerPrefs.SetFloat("PosX", transform.position.x);
        PlayerPrefs.SetFloat("PosY", transform.position.y);
    }
}
