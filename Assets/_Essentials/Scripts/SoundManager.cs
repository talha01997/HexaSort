using System;
using UnityEngine;

public class SoundManager : MonoBehaviour
{
    public static SoundManager instance;
    
    [Header("BG Audio Source")] public AudioSource BGAudioSource;
    [Header("SFX Audio Source")] public AudioSource SFXAudioSource;
    
    public bool canPlayBg;
    public bool canPlaySFX;
    public bool canPlayHaptic;

    public Sounds[] soundsContainer;

    private void Awake()
    {
        //print("sound manager awake");
        if (instance == null)
        {
            instance = this;
            DontDestroyOnLoad(this);
        }
        else
        {
            DestroyImmediate(gameObject);
        }
    }

    private void Start()
    {
        BGAudioSource.enabled = true;
        PlayBGSound();
        CheckSoundStatePrefs();

       
    }

    void CheckSoundStatePrefs()
    {
        if (!PlayerPrefs.HasKey("SFXState"))
        {
            CheckSoundsState(true);
        }
        else if(PlayerPrefs.GetInt("SFXState")==0)
        {
            CheckSoundsState(false);
        }

        if (!PlayerPrefs.HasKey("MusicState"))
        {
            CheckMusicState(true);
        }
        else if (PlayerPrefs.GetInt("MusicState")==0)
        {
            CheckMusicState(false);
        }
        
        if (!PlayerPrefs.HasKey("HapticState"))
        {
            CheckHapticState(true);
        }
        else if (PlayerPrefs.GetInt("HapticState")==0)
        {
            CheckHapticState(false);
        }
    }
    public void PlayBGSound()
    {
        if (!canPlayBg)
            BGAudioSource.mute = true;
        else
            BGAudioSource.mute = false;
    }
    
    public void PlaySFXSound(string name)
    {
        if(!canPlaySFX)
            return;
        Sounds _sound = Array.Find(soundsContainer, sound => sound.name == name);
        SFXAudioSource.PlayOneShot(_sound.clip);
    }
    
    // public void DoHaptic(UIFeedbackType hapticType)
    // {
    //     if(!canPlayHaptic)
    //         return;
    //     switch (hapticType)
    //     {
    //         case UIFeedbackType.ImpactLight:
    //             HapticFeedback.Generate(hapticType);
    //             break;
    //         case UIFeedbackType.ImpactMedium:
    //             HapticFeedback.Generate(hapticType);
    //             break;
    //         case UIFeedbackType.ImpactHeavy:
    //             HapticFeedback.Generate(hapticType);
    //             break;
    //     }
    // }

    public void PlayHaptic()
    {
        
    }
    public void ToggleState(int num, bool state)
    {
        switch (num)
        {
            case 0:
                CheckMusicState(state);
                break;
            case 1:
                CheckSoundsState(state);
                break;
            case 2:
                CheckHapticState(state);
                break;
            default:
                break;
        }
    }
    public void CheckSoundsState(bool state)
    {
        canPlaySFX = state;
        SFXAudioSource.gameObject.SetActive(state);
        PlayerPrefs.SetInt("SFXState", ConvertToInteger(state));
    }
    public void CheckMusicState(bool state)
    {
        canPlayBg = state;
        BGAudioSource.gameObject.SetActive(state);
        PlayerPrefs.SetInt("MusicState", ConvertToInteger(state));
    }

    public void CheckHapticState(bool state)
    {
        canPlayHaptic = state;
        PlayerPrefs.SetInt("HapticState", ConvertToInteger(state));
    }
    public void OnLevelComplete()
    {
        //PlaySFXSound("win");
    }
    public int ConvertToInteger(bool value)
    {
        return value ? 1 : 0;
    }
}
[System.Serializable]
public class Sounds
{
    public string name;
    public AudioClip clip;
}
