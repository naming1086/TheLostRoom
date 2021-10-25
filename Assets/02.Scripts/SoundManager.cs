using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Audio;
using UnityEngine.Events;
using UnityEngine.SceneManagement;

[System.Serializable]
public class Sound
{
    public string name;     // 곡 이름
    public AudioClip clip;  // 곡
}

public class SoundManager : MonoBehaviour
{
    static public SoundManager instance;

    #region singleton
    void Awake()
    {
        if (instance == null)
        {
            instance = this;

            DontDestroyOnLoad(gameObject);
        }
        else
        {
            Destroy(gameObject);
        }
    }
    #endregion singleton

    public AudioSource[] sfxAudio;
    public AudioSource bgmAudio;

    public string[] playingSoundName;

    [SerializeField]
    public Sound[] sfxSounds;
    public Sound[] bgmSounds;

    public void SFXSoundPlay(string _name)
    {
        for (int i = 0; i < sfxSounds.Length; i++)
        {
            if (_name == sfxSounds[i].name)
            {
                for (int j = 0; j < sfxAudio.Length; j++)
                {
                    if (!sfxAudio[j].isPlaying)
                    {
                        playingSoundName[j] = sfxSounds[i].name;
                        sfxAudio[j].clip = sfxSounds[i].clip;
                        sfxAudio[j].Play();
                        return;
                    }
                }
                Debug.Log("모든 가용 Audio Source가 사용 중 입니다.");
                return;
            }
        }
        Debug.Log($"{_name} 사운드가 SoundManager에 등록되지 않았습니다.");
    }

    public void SFXSoundAllStop()
    {
        for (int i = 0; i < sfxAudio.Length; i++)
        {
            sfxAudio[i].Stop();
        }
    }

    public void SFXSoundStop(string _name)
    {
        for (int i = 0; i < sfxAudio.Length; i++)
        {
            if (playingSoundName[i] == _name)
            {
                sfxAudio[i].Stop();
                return;
            }
        }
        Debug.Log($"재생 중인 {_name} 사운드가 없습니다.");
    }
}
