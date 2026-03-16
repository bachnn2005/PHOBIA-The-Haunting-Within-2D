using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Rendering.Universal;

public class GameStartMap1 : MonoBehaviour
{
    [SerializeField] private PlayerMoveBehave player;
    [SerializeField] private CameraManager cam;
    [SerializeField] private Light2D globalLight;
    // Start is called before the first frame update
    void Start()
    {
        PlayerPrefs.SetFloat("PosX", transform.position.x);
        PlayerPrefs.SetFloat("PosY", transform.position.y);
    }

    // Update is called once per frame
    void Update()
    {
        
    }
    private void OnTriggerEnter2D(Collider2D collision)
    {
        if (player != null && globalLight != null)
        {
            StartCoroutine(GameStart(0.75f, 5f));
            player.rb.gravityScale = 3f;
        }
    }
    private IEnumerator GameStart(float targetIntensity, float duration)
    {
        float startIntensity = globalLight.intensity;
        float elapsedTime = 0f;

        while (elapsedTime < duration)
        {
            elapsedTime += Time.deltaTime;
            globalLight.intensity = Mathf.Lerp(startIntensity, targetIntensity, elapsedTime / duration);
            yield return null; 
        }
        globalLight.intensity = targetIntensity;
    }
}
