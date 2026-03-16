using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PauseMenu : MonoBehaviour
{
    [SerializeField] private GameObject pauseMenu;
    [SerializeField] private Animator yes;
    [SerializeField] private Animator no;
    private int selectIndex = 1;
    private bool isPausing;
    // Start is called before the first frame update
    void Start()
    {
        
    }

    // Update is called once per frame
    void Update()
    {
        if (Input.GetKeyDown(KeyCode.Escape) && !isPausing)
        {
            Time.timeScale = 0f;
            pauseMenu.SetActive(true);
            isPausing = true;
        }
        else if(Input.GetKeyDown(KeyCode.Escape) && isPausing)
        {
            Time.timeScale = 1f;
            selectIndex = 1;
            no.SetBool("isSelected", true);
            yes.SetBool("isSelected", false);
            pauseMenu.SetActive(false);
            isPausing = false;
        }
        if (isPausing)
        {
            Pausing();
        }
    }
    private void Pausing()
    {
        if (Input.GetKeyDown(KeyCode.RightArrow) || Input.GetKeyDown(KeyCode.D))
        {
            selectIndex++;
        }
        else if (Input.GetKeyDown(KeyCode.LeftArrow) || Input.GetKeyDown(KeyCode.A)) { selectIndex--; }
        UpdateSelect();
        if (Input.GetKeyDown(KeyCode.Space) || Input.GetKeyDown(KeyCode.Return))
        {
            if (selectIndex == 1)
            {
                //no
                pauseMenu.SetActive(false);
                isPausing = false;
                Time.timeScale = 1f;
            }
            else if (selectIndex == 0)
            {
                //yes
                QuitGame();
            }
        }
    }
    private void UpdateSelect()
    {
        if (selectIndex == 2)
        {
            selectIndex = 0;
        }
        else if (selectIndex == -1)
        {
            selectIndex = 1;
        }
        if (selectIndex == 1)
        {
            no.SetBool("isSelected", true);
            yes.SetBool("isSelected", false);
        }
        else if(selectIndex == 0)
        {
            no.SetBool("isSelected", false);
            yes.SetBool("isSelected", true);
        }
    }
    private void QuitGame()
    {
#if UNITY_EDITOR
        UnityEditor.EditorApplication.isPlaying = false; // Stop play mode in the editor
#else
        Application.Quit(); // Quit the application in a built version
#endif
    }
}
