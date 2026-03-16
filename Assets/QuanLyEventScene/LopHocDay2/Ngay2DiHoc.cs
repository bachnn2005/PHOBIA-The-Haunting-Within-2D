using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Ngay2DiHoc : MonoBehaviour
{
    [SerializeField] private Animator mainChar;
    [SerializeField] private GameManager gameManager;
    private bool isDone;
    // Start is called before the first frame update
    void Start()
    {
        
    }

    // Update is called once per frame
    void Update()
    {
        if(gameManager.getCount() == gameManager.getDialogueLength() - 2 && !isDone)
        {
            isDone = true;
            mainChar.SetBool("CuiDau", true);
        }
    }
}
