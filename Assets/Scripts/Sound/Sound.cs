using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public enum SoundType
{
    SFX,
    BGM,
}
[Serializable]
public class Sound
{
    public string name;

    public AudioClip audioClip;

    public SoundType type;
    public Sound(string _name, AudioClip _audioClip)
    {
        name = _name;
        audioClip = _audioClip;
    }
}
