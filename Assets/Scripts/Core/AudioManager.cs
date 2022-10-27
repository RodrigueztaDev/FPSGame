using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[RequireComponent(typeof(AudioSource))]
public class AudioManager : MonoBehaviour
{
    AudioSource source_;

    void Start()
    {
        source_ = GetComponent<AudioSource>();
        source_.playOnAwake = false;
        source_.loop = true;
    }

    public void PlayAsMusic(AudioClip clip, float volume)
    {
        source_.clip = clip;
        source_.volume = volume;
        source_.Play();
    }

    public static void PlaySoundAtLocation(AudioClip clip, Vector3 position, float volume = 1.0f)
    {
        AudioSource.PlayClipAtPoint(clip, position, volume);
    }
}
