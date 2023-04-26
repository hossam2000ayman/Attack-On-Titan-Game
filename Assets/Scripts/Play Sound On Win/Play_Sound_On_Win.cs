using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Play_Sound_On_Win : MonoBehaviour
{
    public AudioClip soundToPlay;
    public float volume;
    new AudioSource audio;
    public bool alreadyPlayed = false;


    private void Start()
    {
        audio= GetComponent<AudioSource>();
    }

    private void OnTriggerEnter(Collider other)
    {
        if (!alreadyPlayed)
        {
            audio.PlayOneShot(soundToPlay, volume);
            alreadyPlayed = true;
        }
    }
}
