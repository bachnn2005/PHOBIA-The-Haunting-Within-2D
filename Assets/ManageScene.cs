using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;

public class ManageScene : MonoBehaviour
{
    [SerializeField] private string[] sceneName;
    [SerializeField] private int sceneIndex;

    // Start is called before the first frame update
    void Start()
    {
        if(SceneManager.GetActiveScene().name == "Ending")
        {
            PlayerPrefs.SetInt("SceneIndex", 0);
        }
        //if (SceneManager.GetActiveScene().name == "StartMenu")
        //{
        //    PlayerPrefs.SetInt("SceneIndex", 2);
        //    PlayerPrefs.SetInt("unlockDash", 0);
        //}
        sceneIndex = PlayerPrefs.GetInt("SceneIndex");
        Debug.Log("SceneIndex: " + sceneIndex);
        PlayerPrefs.SetInt("SceneIndex", ++sceneIndex);
        Debug.Log(PlayerPrefs.GetInt("SceneIndex") + " Scene");
    }

    // Update is called once per frame
    void Update()
    {
        
    }
    public void ChangeScene()
    {
        SceneManager.LoadScene(sceneName[PlayerPrefs.GetInt("SceneIndex")]);
    }
}
