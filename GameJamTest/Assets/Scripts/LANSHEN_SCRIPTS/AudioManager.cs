using System;
using UnityEngine;

public class AudioManager : MonoBehaviour
{
    public static AudioManager instance;
    private AudioSource audioSource;

    private AudioSource bgm;
    // Start is called once before the first execution of Update after the MonoBehaviour is created
    void Start()
    {
        if (instance == null)
        {
            instance = this;
            audioSource = gameObject.AddComponent<AudioSource>();
            bgm = gameObject.AddComponent<AudioSource>();
        }
        
    }

    public static void PlaySound(string sound)
    {
        if (instance == null)
        {
            var obj = new GameObject("AudioManager");
            instance = obj.AddComponent<AudioManager>();
            instance.audioSource = obj.AddComponent<AudioSource>();
            instance.bgm = obj.AddComponent<AudioSource>();
        }
        AudioClip clip = Resources.Load<AudioClip>($"audio/{sound}");
        if (clip != null)
        {
            if (instance.audioSource.isPlaying && instance.audioSource.clip == clip)
            {
                
            }
            else
            {
                instance.audioSource.PlayOneShot(clip); 
            }
        }
    }
    public static void PlayBGM(string sound)
    {
        if (instance == null)
        {
            var obj = new GameObject("AudioManager");
            instance = obj.AddComponent<AudioManager>();
            instance.audioSource = obj.AddComponent<AudioSource>();
            instance.bgm = obj.AddComponent<AudioSource>();
            instance.bgm.clip = Resources.Load<AudioClip>($"audio/{sound}");
            instance.bgm.loop = true;
            instance.bgm.playOnAwake = true;
            instance.bgm.Play();
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
