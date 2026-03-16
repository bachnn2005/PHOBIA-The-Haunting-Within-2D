using Cinemachine;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CameraManager : MonoBehaviour
{
    [SerializeField] private PlayerMoveBehave player;
    [SerializeField] private CinemachineVirtualCamera vcam;
    private CinemachineFramingTransposer framingTransposer;
    [SerializeField] private Vector3 maxOffsetLeft = new Vector3(-2, 1, -10);
    [SerializeField] private Vector3 maxOffsetRight = new Vector3(2, 1, -10);
    [SerializeField] private float transitionDuration = 1f;
    private Vector3 currentOffset;
    private float currentTimeHorizontal;
    private bool isChanged;
    //~~~~~~~~~~~~~~CameraShake~~~~~~~~~~~~~~~
    private CinemachineBasicMultiChannelPerlin noise;
    private float shakeDuration = 0.2f; 
    private float shakeAmplitude = 1.1f;
    private float shakeFrequency = 1.1f;
    // Start is called before the first frame update
    void Start()
    {
        framingTransposer = vcam.GetCinemachineComponent<CinemachineFramingTransposer>();
        currentOffset = framingTransposer.m_TrackedObjectOffset;
        noise = vcam.GetCinemachineComponent<CinemachineBasicMultiChannelPerlin>();
        noise.m_AmplitudeGain = 0f;
        noise.m_FrequencyGain = 0f;
    }

    // Update is called once per frame
    void Update()
    {

        if (framingTransposer == null) return;
        if (!isChanged)
        {
            TurningCamHorizontal();
        }
        

    }

    private void TurningCamHorizontal()
    {
        if (player.GetisFacingRight())
        {
            currentTimeHorizontal += Time.deltaTime / transitionDuration;
            framingTransposer.m_TrackedObjectOffset = Vector3.Lerp(currentOffset, maxOffsetRight, currentTimeHorizontal);
        }
        else
        {
            currentTimeHorizontal += Time.deltaTime / transitionDuration;
            framingTransposer.m_TrackedObjectOffset = Vector3.Lerp(currentOffset, maxOffsetLeft, currentTimeHorizontal);
        }


        if ((player.GetisFacingRight() && framingTransposer.m_TrackedObjectOffset == maxOffsetRight) ||
            (!player.GetisFacingRight() && framingTransposer.m_TrackedObjectOffset == maxOffsetLeft))
        {
            currentTimeHorizontal = 0f;
            currentOffset = framingTransposer.m_TrackedObjectOffset;
        }
    }
    public void Shake(float shakeAmp, float shakeFre, float shakeDur)
    {

        shakeAmplitude = shakeAmp;
        shakeFrequency = shakeFre;
        shakeDuration = shakeDur;
        noise.m_AmplitudeGain = shakeAmplitude;
        noise.m_FrequencyGain = shakeFrequency;

        StartCoroutine(StopShakeAfterDuration());
    }

    private IEnumerator StopShakeAfterDuration()
    {
        yield return new WaitForSeconds(shakeDuration);
        noise.m_AmplitudeGain = 0f;
        noise.m_FrequencyGain = 0f;

    }
    public void ChangeVCam(CinemachineVirtualCamera vcam)
    {
        isChanged = true;
        this.vcam = vcam;
        framingTransposer = vcam.GetCinemachineComponent<CinemachineFramingTransposer>();
        noise = vcam.GetCinemachineComponent<CinemachineBasicMultiChannelPerlin>();
        noise.m_AmplitudeGain = 0f;
        noise.m_FrequencyGain = 0f;
        currentOffset = framingTransposer.m_TrackedObjectOffset;
        currentTimeHorizontal = 0f;
    }
}
