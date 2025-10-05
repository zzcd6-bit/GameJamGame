using System;
using UnityEngine;

public class AudioManager : MonoBehaviour
{
    public static AudioManager instance;
    private AudioSource audioSource;
    // Start is called once before the first execution of Update after the MonoBehaviour is created
    void Start()
    {
        if (instance == null)
        {
            instance = this;
            audioSource = gameObject.AddComponent<AudioSource>();
        }
        
    }

    public void PlaySound(string sound)
    {
        AudioClip clip = Resources.Load<AudioClip>(sound);
        if (clip != null)
        {
            audioSource.PlayOneShot(clip);
        }
    }

    // Update is called once per frame
    void Update()
    {
        
    }

    private void OnDisable()
    {
        instance = null;
    }
}
