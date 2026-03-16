using System.Collections;
using System.Collections.Generic;
using Unity.VisualScripting;
using UnityEngine;

public class SaveDream : MonoBehaviour
{
    private PlayerMoveBehave player;
    [SerializeField] private GameObject restoreHpVFX;
    // Start is called before the first frame update
    void Start()
    {
        player = GameObject.FindGameObjectWithTag("Player").GetComponent<PlayerMoveBehave>();
    }

    // Update is called once per frame
    void Update()
    {
        
    }
    private void OnTriggerEnter2D(Collider2D collision)
    {
        if (collision.CompareTag("Player"))
        {
            StartCoroutine(RestoreHp());
            player.SetFullHp();
            PlayerPrefs.SetFloat("PosX", transform.position.x);
            PlayerPrefs.SetFloat("PosY", transform.position.y);
        }
    }
    private IEnumerator RestoreHp()
    {
        GameObject vfxInstance = Instantiate(restoreHpVFX,transform.position + new Vector3(0f, 0.5f, 0f), Quaternion.identity);
        yield return new WaitForSeconds(1f);
        Destroy(vfxInstance);
    }
}
