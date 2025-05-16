using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class SoundBox : MonoBehaviour
{
    // Start is called before the first frame update
    public enum SoundAct
    {
        Play,
        Stop,
        Pause,
        UnPause
    }

    public static SoundBox instance;

    [SerializeField]
    public Sound[] common_clips;

    [SerializeField]
    public List<Sound> act_clips = new List<Sound>();

    public AudioSource BGM;

    [SerializeField]
    public List<AudioSource> Effects = new List<AudioSource>();

    public AudioSource Etc;

    public AudioSource Common;

    private float Volume = 1f;

    private void Awake()
    {
        Initialize();
        SetVolume();
        if (instance == null)
        {
            instance = this;
            DontDestroyOnLoad(this);
        }
        else
            Destroy(gameObject);
        OptionUI.BGMValue = PlayerPrefs.GetFloat("BGMvalue", 1f);
        OptionUI.SFXValue = PlayerPrefs.GetFloat("SFXvalue", 1f);
    }
    public void Update()
    {
        BGM.volume = Volume * OptionUI.BGMValue;
        for (int i = 0; i < Effects.Count; i++)
        {
            Effects[i].volume = Volume*OptionUI.SFXValue;
        }
    }

    public void Initialize()
    {
        for (int i = 0; i < common_clips.Length; i++)
        {
            act_clips.Add(common_clips[i]);
        }
    }
    public void Play(string name)
    {
        Sound sound_Members = act_clips.Find((Sound x) => x.name == name);
        if (sound_Members != null)
        {
            if (sound_Members.type == SoundType.BGM)
            {
                PlayBGM(name);
            }
            else
            {
                PlaySFX(name);
            }
        }
    }
    public void PlayBGM(string name)
    {
        AudioSource audioSource = Etc;
        Sound sound_Members = act_clips.Find((Sound x) => x.name == name);
        if (sound_Members != null)
        {
            if (sound_Members.type==SoundType.SFX)
            {
                return;
            }
            if (sound_Members.type==SoundType.BGM)
            {
                audioSource = BGM;
                if (!audioSource.loop)
                {
                    audioSource.loop = true;
                }
                audioSource.clip = sound_Members.audioClip;
                audioSource.Play();
            }
        }
    }
    public void StopBGM()
    {
        BGM.Stop();
    }
    public void PlaySFX(string name, SoundAct soundAct = SoundAct.Play)
    {
        AudioSource audioSource = Etc;
        Sound sound_Members = act_clips.Find((Sound x) => x.name == name);
        if (sound_Members == null)
            return;
        
        if (sound_Members.type==SoundType.BGM)
        {
            PlayBGM(name);
            return;
        }
        if(isPlaySameSound(sound_Members.audioClip.name))
        {
            return;
        }

        bool flag = false;
        switch (soundAct)
        {
            case SoundAct.Play:
                {
                    for (int i = 0; i < Effects.Count; i++)
                    {
                        if (!Effects[i].isPlaying)
                        {
                            flag = true;
                            Effects[i].clip = sound_Members.audioClip;
                            audioSource = Effects[i];
                            break;
                        }
                    }

                    if (!flag)
                    {
                        AudioSource audioSource2 = Object.Instantiate(Effects[0]);
                        audioSource2.name = string.Concat(Effects.Count);
                        audioSource2.transform.parent = Effects[0].transform.parent;
                        audioSource2.clip = sound_Members.audioClip;
                        audioSource2.loop = false;
                        Effects.Add(audioSource2);
                        audioSource = audioSource2;
                        audioSource2.volume = 1f * OptionUI.SFXValue;
                    }

                    audioSource.Play();
                    break;
                }
            case SoundAct.Stop:
                for (int i = 0; i < Effects.Count; i++)
                {
                    if (Effects[i].clip != null && (Effects[i].clip.name.Equals(sound_Members.audioClip.name) || Effects[i].clip.name.Equals(sound_Members.name)))
                    {
                        flag = true;
                        audioSource = Effects[i];
                        break;
                    }
                }
                if (flag)
                {
                    audioSource.Stop();
                }

                break;
            case SoundAct.Pause:
                for (int i = 0; i < Effects.Count; i++)
                {
                    if (Effects[i].clip != null && (Effects[i].clip.name.Equals(sound_Members.audioClip.name) || Effects[i].clip.name.Equals(sound_Members.name)))
                    {
                        flag = true;
                        audioSource = Effects[i];
                        break;
                    }
                }
                if (flag)
                {
                    audioSource.Pause();
                }

                break;
            case SoundAct.UnPause:
                for (int i = 0; i < Effects.Count; i++)
                {
                    if (Effects[i].clip != null && (Effects[i].clip.name.Equals(sound_Members.audioClip.name) || Effects[i].clip.name.Equals(sound_Members.name)))
                    {
                        flag = true;
                        audioSource = Effects[i];
                        break;
                    }
                }
                if (flag)
                {
                    audioSource.UnPause();
                }
                break;
        }
    }

  

    public void AddClip(string name, AudioClip clip)
    {
        Sound sound_Members = FindClip(name);
        if (sound_Members != null)
        {
            sound_Members.audioClip = clip;
        }
        else
        {
            act_clips.Add(new Sound(name, clip));
        }
    }

    public float GetDuration(string name)
    {
        Sound sound_Members = act_clips.Find((Sound x) => x.name == name);
        if (sound_Members == null)
        {
            return 0f;
        }

        if (sound_Members.audioClip == null)
        {
            return 0f;
        }

        return sound_Members.audioClip.length;
    }

    public Sound FindClip(string name)
    {
        Sound sound_Members = act_clips.Find((Sound x) => x.name == name);
        if (sound_Members == null)
        {
            return null;
        }

        return sound_Members;
    }
    public void SetVolume()
    {
        BGM.volume = Volume * OptionUI.BGMValue;
        Etc.volume = Volume * OptionUI.SFXValue;
        Common.volume = Volume * OptionUI.SFXValue;
        for (int num = 0; num < Effects.Count; num++)
        {
            Effects[num].volume = Volume * OptionUI.SFXValue;           
        }
    }
    public bool isPlaySameSound(string _name)
    {
        if (Effects == null)
            return false;

        for (int i = 0; i < Effects.Count; i++)
        {
            var src = Effects[i];
            if (src == null || src.clip == null)
                continue;   // null이면 건너뛰기

            if (src.clip.name.Equals(_name)&&src.isPlaying)
            {
                src.Stop();
                src.Play();
                return true;
            }
        }
        return false;
    }
}


