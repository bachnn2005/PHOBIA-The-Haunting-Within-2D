using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class SleepEvent : MonoBehaviour
{
    private Animator animator;
    private bool isSleeping = true;
    [SerializeField] private SpriteRenderer noitice;
    // Start is called before the first frame update
    void Start()
    {
        noitice.transform.localScale = Vector3.zero;
        animator = GetComponent<Animator>();
    }

    // Update is called once per frame
    void Update()
    {
        if(Input.anyKeyDown && isSleeping)
        {
            isSleeping = false;
            StartCoroutine(ScaleNoitice());
            animator.SetTrigger("CheckPhone");
        }
    }

    private IEnumerator ScaleNoitice()
    {
        yield return new WaitForSeconds(0.4f);
        float timeElapsed = 0f;

        while (timeElapsed < 0.1f)
        {
            noitice.transform.localScale = Vector3.Lerp(Vector3.zero, Vector3.one, timeElapsed / 0.1f);
            timeElapsed += Time.deltaTime;
            yield return null;

        }
        noitice.transform.localScale = Vector3.one;
    }
}
