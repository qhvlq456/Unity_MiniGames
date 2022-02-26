using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public enum EEffactClipType
{
    None,
    DefaultButton,
    CoinButton,
    Flap,
    Angry,
    Obstacle,
    AddScore,
    Put,
    Victory,
    Lose
}
public enum EBGMClipType
{
    Logo,
    Main,
    Lobby,
    Flappy,
    Angry,
    Nightmare,
    Omok,
    Othello
}
public class SoundManager : MonoBehaviour
{
    public static SoundManager instance = null;
    public AudioSource bgmSource;
    public AudioSource[] effectSources;
    public AudioClip[] effectClips;
    public AudioClip[] BGMClips;
    int cursor = 0;
    int maxLength;
    private void Awake() {
        if(instance == null)
        {
            instance = this;
            DontDestroyOnLoad(gameObject);
        }
        else if(instance != this)
        {
            Destroy(gameObject);
        }
        maxLength = effectSources.Length;
    }
    private void Start() {
        LoadVolume();
    }
    public void PlayClip(EEffactClipType clipType)
    {
        effectSources[cursor].clip = effectClips[(int)clipType];
        effectSources[cursor].Play();

        cursor = (cursor + 1) % maxLength;
    }
    public void ChangeBGM(EBGMClipType clipType)
    {
        if(BGMClips[(int)clipType] == bgmSource.clip || BGMClips[(int)clipType] == null) return;

        bgmSource.clip = BGMClips[(int)clipType];
        bgmSource.Play();
    }
    void LoadVolume()
    {
        bgmSource.volume = PlayerPrefs.GetFloat("BGMSound",1);

        foreach(var v in effectSources)
        {
            v.volume = PlayerPrefs.GetFloat("EffectSound",1);
        }
    }
}
