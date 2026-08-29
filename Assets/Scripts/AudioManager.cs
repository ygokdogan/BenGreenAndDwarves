using System;
using UnityEngine;

public class AudioManager : MonoBehaviour
{
    public static AudioManager Instance;
    public AudioSource uiSource;
    public AudioSource challengeFX;

    public AudioSource musicSource;
    
    private void Awake()
    {
        if (Instance)
        {
            Destroy(gameObject);
            return;
        }
        
        Instance = this;
        DontDestroyOnLoad(gameObject);
    }

    public void PlayUISFX(AudioClip clip)
    {
        uiSource.PlayOneShot(clip);
    }

    public void PlayChallengeSFX(AudioClip clip)
    {
        challengeFX.PlayOneShot(clip);
    }
 }
