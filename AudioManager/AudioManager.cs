using UnityEngine;
using System;
using System.Collections;
using System.Collections.Generic;

public class AudioManager : Singleton<AudioManager>
{
    public PoolManager channels;
    public List<AudioClip> clips = new List<AudioClip>();   

    protected override void Awake()
    {
        base.Awake();
        GameObject channel = new GameObject("Channel");
        channel.AddComponent<AudioSource>();
        channel.AddComponent<AudioEventSystem>();
        channel.transform.SetParent(transform);
        channel.GetComponent<AudioSource>().playOnAwake = false;

        channels = new PoolManager(channel);
    }

    void Start()
    {

    }

    void FixedUpdate()
    {
        
    }
    
    void Update()
    {
        ReleaseUnusedChannels();
    }

    // Plays a clip randomly 
    public static void PlayRandom()
    {
        Play(instance.clips[UnityEngine.Random.Range(0, instance.clips.Count)]);
    }

    // Plays the clip that matches the name in an available channel
    public static AudioSource Play(string name)
    {
        return Play(instance.clips.Find(x => x.name.Equals(name)));
    }

    // Plays the clip attached to the referenced channel
    public static void Play(AudioSource channel)
    {
        if (channel) channel.Play();
    }

    public static AudioSource Play(AudioClip clip, EventHandler eventHandler = null)
    {
        if (!clip) return null;

        GameObject channel = instance.channels.PickAvailable();
        AudioSource audioSource = channel.GetComponent<AudioSource>();
        audioSource.clip = clip;
        audioSource.Play();
        if (eventHandler != null)
            channel.GetComponent<AudioEventSystem>().ClipEnded += eventHandler;

        return audioSource;
    }

    // Stops the clip attached to the referenced channel
    public static void Stop(AudioSource channel)
    {
        if (!channel) return;
        channel.Stop();
        ReleaseChannel(channel.gameObject);
    }

    // Pauses the clip attached to the referenced channel
    public static void Pause(AudioSource channel)
    {
        if (channel) channel.Pause();
    }

    // Resumes the clip attached to the referenced channel
    public static void Resume(AudioSource channel)
    {
        if (channel) channel.Play();        
    }

    private void ReleaseUnusedChannels()
    {
        if (channels == null) return;
        
        foreach (GameObject channel in channels.pool)
        {
            AudioSource audioSource = channel.GetComponent<AudioSource>();
            if (audioSource.clip && audioSource.timeSamples == audioSource.clip.samples)
                ReleaseChannel(channel);
        }   
    }

    private static void ReleaseChannel(GameObject channel)
    {
        channel.GetComponent<AudioEventSystem>().ClipEnded(channel, EventArgs.Empty);
        channel.GetComponent<AudioEventSystem>().ClipEnded = null;
        channel.SetActive(false);
    }
}

public class AudioEventSystem : MonoBehaviour
{
    public EventHandler ClipEnded;
}
