using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;

public class MenuUI : MonoBehaviour
{
    [SerializeField] private Animator newGameButtonAni;
    [SerializeField] private Animator continueButtonAni;
    [SerializeField] private Animator exitButtonAni;
    [SerializeField] private SpriteRenderer newGameButton;
    [SerializeField] private SpriteRenderer continueButton;
    [SerializeField] private SpriteRenderer exitButton;
    [SerializeField] private ManageScene sm;
    [SerializeField] private GameObject startScreen;
    private int selectIndex = 2;
    // Start is called before the first frame update
    void Start()
    {
        int temp = PlayerPrefs.GetInt("SceneIndex") - 2;
        PlayerPrefs.SetInt("SceneIndex", temp);
        newGameButtonAni.SetBool("isSelected", true);
    }

    // Update is called once per frame
    void Update()
    {
        if(Input.GetKeyDown(KeyCode.UpArrow))
        {
            selectIndex++;
        }
        else if(Input.GetKeyDown(KeyCode.DownArrow)) { selectIndex--; }
        UpdateSelect();
        if(Input.GetKeyDown(KeyCode.Space) || Input.GetKeyDown(KeyCode.Return))
        {
            if(selectIndex == 2)
            {
                PlayerPrefs.SetInt("unlockDash", 0);
                PlayerPrefs.SetInt("SceneIndex", 1);
                StartCoroutine(ChangSceneTransition());
            }
            else if(selectIndex == 1)
            {
                StartCoroutine(ChangSceneTransition());
            }
            else if(selectIndex == 0)
            {
                QuitGame();
            }
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
    private IEnumerator ChangSceneTransition()
    {
        startScreen.SetActive(true);
        yield return new WaitForSeconds(1f);
        sm.ChangeScene();
    }
    private void UpdateSelect()
    {
        if (selectIndex == 3)
        {
            selectIndex = 0;
        }
        else if (selectIndex == -1)
        {
            selectIndex = 2;
        }
        if (selectIndex == 2)
        {
            newGameButton.sortingLayerName = "Default";
            continueButton.sortingLayerName = "NoneLight";
            exitButton.sortingLayerName = "NoneLight";
            newGameButtonAni.SetBool("isSelected", true);
            continueButtonAni.SetBool("isSelected", false);
            exitButtonAni.SetBool("isSelected", false);
        }
        else if (selectIndex == 1)
        {
            newGameButton.sortingLayerName = "NoneLight";
            continueButton.sortingLayerName = "Default";
            exitButton.sortingLayerName = "NoneLight";
            newGameButtonAni.SetBool("isSelected", false);
            continueButtonAni.SetBool("isSelected", true);
            exitButtonAni.SetBool("isSelected", false);
        }
        else if (selectIndex == 0)
        {
            newGameButton.sortingLayerName = "NoneLight";
            continueButton.sortingLayerName = "NoneLight";
            exitButton.sortingLayerName = "Defaultt";
            newGameButtonAni.SetBool("isSelected", false);
            continueButtonAni.SetBool("isSelected", false);
            exitButtonAni.SetBool("isSelected", true);
        }
    }
}
