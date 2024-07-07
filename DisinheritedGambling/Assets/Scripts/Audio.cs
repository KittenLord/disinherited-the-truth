using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Audio : MonoBehaviour
{
    public static AudioClip Load(string clip)
    {
        var path = "SFX/" + clip;
        return Resources.Load<AudioClip>(path);
    }

    public static void Play(string path, float volume = 1) => Play(Load(path), volume);
    public static void Play(AudioClip clip, float volume = 1)
    {
        Main.Source.PlayOneShot(clip, volume);
    }

    private AudioSource Source;
    private static Audio Main;
    void Start()
    {
        if(Main != null) Destroy(Main.gameObject);
        Main = this;
        Source = GetComponent<AudioSource>();
    }
}
