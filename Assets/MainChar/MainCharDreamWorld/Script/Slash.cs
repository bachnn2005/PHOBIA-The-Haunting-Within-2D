using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Slash : MonoBehaviour
{
    private bool hit;
    [SerializeField] private PlayerAttackBehave player;
    [SerializeField] private CameraManager camera_;
    [SerializeField] private PlayerMoveBehave playerMove;
    [SerializeField] private Transform playerTransform_;
    [SerializeField] private Vector3 offset;
    [SerializeField] private GameObject slashParticle;
    [SerializeField] private float slashDmg;
    public bool triggerSound;
    [Header("~~~~~~~~~~~~Shake System~~~~~~~~~~~~~")]
    [SerializeField] private float shakeAmp;
    [SerializeField] private float shakeFru;
    [SerializeField] private float shakeDur;
    // Start is called before the first frame update
    void Start()
    {
        slashDmg = 1f;
    }

    // Update is called once per frame
    void Update()
    {

    }
    private void OnTriggerEnter2D(Collider2D collision)
    {
        if ((collision.CompareTag("Enemy") || collision.CompareTag("Dummy")) && !player.onTrigger)
        {
            triggerSound = true;
            slashParticle.transform.eulerAngles = new Vector3(0f, 0f, UnityEngine.Random.Range(0f, 360f));
            slashParticle.transform.position = collision.ClosestPoint(transform.position);

            StartCoroutine(Slashing());
            if(playerMove.getLook() < 0f && !playerMove.GetisGrounded())
            {
                playerMove.rb.linearVelocity = new Vector2(playerMove.rb.linearVelocity.x, 12f);
                playerMove.SetLook(0f);
                playerMove.SetCanDash();
            }
            camera_.Shake(shakeAmp, shakeFru, shakeDur);
            player.onTrigger = true;
        }
    }
    private IEnumerator Slashing()
    {
        slashParticle.SetActive(true);
        yield return new WaitForSeconds(0.3f);
        slashParticle.SetActive(false);
        triggerSound = false;
    }
    public float getSlashDmg()
    {
        return slashDmg;
    }
}
