using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public enum SoundType { Hit }
public class Sounds : MonoBehaviour
{
    public static void PlaySound(AudioSource source, SoundType sound)
    {
        AudioClip[] clips = Resources.LoadAll<AudioClip>(GetSoundPath(sound));
        source.clip = Choose(clips);
        source.Play();
    }

    private static AudioClip Choose(AudioClip[] clips)
    {
        return clips[Random.Range(0, clips.Length - 1)];
    }

    private static string GetSoundPath(SoundType sound)
    {
        switch (sound)
        {
            case SoundType.Hit:
                return "Sound/Hits";
            default:
                Debug.Log(sound + " not handled (Sounds.GetSoundPath())");
                return "Sound/Hits";
        }
    }
}
