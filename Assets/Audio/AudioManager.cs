using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class AudioManager : MonoBehaviour
{
    [SerializeField] private AudioSource musicSource;
    [SerializeField] private AudioSource sfx;

    [SerializeField] private AudioClip openCloseDoor;
    [SerializeField] private AudioClip backGround;

    void Start()
    {
        sfx.clip = openCloseDoor;
        sfx.Play();
        musicSource.clip = backGround;
        musicSource.Play();
    }
   
}
