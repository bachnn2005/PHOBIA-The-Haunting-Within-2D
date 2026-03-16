using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;

public class MainMenu : MonoBehaviour
{
    // Start is called before the first frame update
    [SerializeField] private GameObject nhacBanStart;
    [SerializeField] private GameObject nhacBanEnd;
    [SerializeField] private GameObject blackScreen;
    [SerializeField] private GameObject endScreen;
    [SerializeField] private GameObject logo;
    [SerializeField] private GameObject theHaunting;
    [SerializeField] private GameObject MenuUI;
    void Start()
    {
        blackScreen.SetActive(true);
        StartCoroutine(StartGame());
    }

    // Update is called once per frame
    void Update()
    {
        
    }
    private IEnumerator StartGame()
    {
        yield return new WaitForSeconds(1f);
        nhacBanStart.SetActive(true);
        yield return new WaitForSeconds(3f);
        nhacBanEnd.SetActive(true);
        nhacBanStart.SetActive(false);
        yield return new WaitForSeconds(2f);
        endScreen.SetActive(true);
        blackScreen.SetActive(false);
        yield return new WaitForSeconds(1f);
        logo.SetActive(true);
        yield return new WaitForSeconds(1f);
        theHaunting.SetActive(true);
        yield return new WaitForSeconds(1f);
        MenuUI.SetActive(true);
    }
}
