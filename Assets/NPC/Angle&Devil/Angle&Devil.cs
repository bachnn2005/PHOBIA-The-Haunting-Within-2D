using Cinemachine;
using System.Collections;
using UnityEngine;

public class AngleDevil : MonoBehaviour
{
    [SerializeField] private Animator angelAni;
    [SerializeField] private Animator devilAni;
    [SerializeField] private GameObject angel;
    [SerializeField] private GameObject devil;
    [SerializeField] private CinemachineVirtualCamera vcam;
    private NPCScript npc;
    private BoxCollider2D triggerBox;
    private PlayerMoveBehave player;
    private float originSize;
    private bool isDone;
    // Start is called before the first frame update
    void Start()
    {
        originSize = vcam.m_Lens.OrthographicSize;
        triggerBox = GetComponent<BoxCollider2D>();
        npc = GetComponent<NPCScript>();
        player = GameObject.FindGameObjectWithTag("Player").GetComponent<PlayerMoveBehave>();
    }

    // Update is called once per frame
    void Update()
    {
        if (npc.getCount() == npc.getDialogLength() && !isDone)
        {
            player.setCanMove(false);
            StartCoroutine(DisableAngelDevil());
            isDone = true;
        }
    }
    private void OnTriggerEnter2D(Collider2D collision)
    {
        StartCoroutine(TriggerEvent());
        Destroy(triggerBox);
    }
    private IEnumerator TriggerEvent()
    {
        angel.SetActive(true);
        devil.SetActive(true);
        StartCoroutine(Event());
        yield return new WaitForSeconds(1f);
        npc.OpenDialog();
    }
    private IEnumerator DisableAngelDevil()
    {
        yield return new WaitForSeconds(1f);
        angelAni.SetTrigger("disappear");
        devilAni.SetTrigger("disappear");
        yield return new WaitForSeconds(0.5f);
        angel.SetActive(false);
        devil.SetActive(false);
        player.setCanMove(true);
        float targetSize = originSize;
        float duration = 2f;
        float elapsedTime = 0f;
        while (elapsedTime < duration)
        {
            vcam.m_Lens.OrthographicSize = Mathf.Lerp(vcam.m_Lens.OrthographicSize, targetSize, (elapsedTime / duration));
            elapsedTime += Time.deltaTime;
            yield return null;
        }
        vcam.m_Lens.OrthographicSize = targetSize;
        
    }
    private IEnumerator Event()
    {
        player.setCanMove(false);
        player.animator.SetBool("isTakingDmg", true);
        yield return new WaitForSeconds(0.1f);
        player.rb.linearVelocity = new Vector2(-2f * transform.localScale.x, 5f);
        yield return new WaitForSeconds(0.5f);
        player.animator.SetBool("isTakingDmg", false);
        player.rb.linearVelocity = Vector2.zero;
        float targetSize = originSize - 0.75f;
        float duration = 1f;
        float elapsedTime = 0f;
        while (elapsedTime < duration)
        {
            vcam.m_Lens.OrthographicSize = Mathf.Lerp(vcam.m_Lens.OrthographicSize, targetSize, (elapsedTime / duration));
            elapsedTime += Time.deltaTime;
            yield return null;
        }
        vcam.m_Lens.OrthographicSize = targetSize;
    }
}
