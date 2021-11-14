using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Audio;
using UnityEngine.Events;
using UnityEngine.SceneManagement;
using VR.ReadOnlys;

public enum AUDIOCATEGORY { LOSTROOM, PLANET, EFFECT }

[System.Serializable]
public struct AudioClipData
{
    public AUDIOCATEGORY aUDIOCATEGORY;
    public string audioName;
    public double intervalBetweenSongs;
    public AudioClip[] audioClips;
}

public class SoundManager : MonoBehaviour
{
    public AudioMixer audioMixer;

    // Inspector에서 클립을 할당시키기 위해
    public List<AudioClipData> audioClipDataList;

    private SoundCtrl bgmLostRoom, bgmPlanet, effect;
    public SoundCtrl BgmLostRoom { get => bgmLostRoom; set => bgmLostRoom = value; }
    public SoundCtrl BgmPlanet { get => bgmPlanet; set => bgmPlanet = value; }
    public SoundCtrl Effect { get => effect; set => effect = value; }

    // 일시정지, 다시 재생 시 실행되는 이벤트
    public UnityEvent onPauseAll, onResumeAll;

    // 초기 볼륨 세팅
    [SerializeField]
    [Range(0.0f, 1.0f)]
    private float initialMasterVolume;

    [SerializeField]
    [Range(0.0f, 1.0f)]
    private float initialBGMVolume;

    [SerializeField]
    [Range(0.0f, 1.0f)]
    private float initialEffectVolume;


    #region Unity Methods
    void Start()
    {
        MasterVolumeCtrl(initialMasterVolume);
        BGMVolumeCtrl(initialBGMVolume);
        EffectVolumeCtrl(initialEffectVolume);
    }
    #endregion


    #region Play and Stop
    // 여러개의 음원을 리스트로 담아두었을 경우, 반복 여부도 설정 가능
    public void PlaySound(string audioName, string planetID = null, bool isRepeatable = false, double delayTime = 0.0d, int startClipIndex = 0)
    {
        try
        {
            AudioClipData audioClipData = FindAudioClipDataByName(audioName);
            SoundCtrl soundCtrl = GetSoundCtrlByType(audioClipData.aUDIOCATEGORY, planetID);

            if (soundCtrl == null)
            {
                throw new System.Exception(audioClipData.aUDIOCATEGORY.ToString() + "의" + Defines.ERROR_MISSING_SOUNDCTRL);
            }

            StopOtherBGM(audioClipData.aUDIOCATEGORY);

            soundCtrl.PlayAudioClipList(audioClipData, isRepeatable, delayTime, startClipIndex);
        }
        catch (System.Exception e)
        {
            Debug.LogError(e.Message);
        }
    }

    // PlayOneShot으로 재생할 경우
    public void PlaySoundOnce(string audioName, string planetID = null)
    {
        try
        {
            AudioClipData audioClipData = FindAudioClipDataByName(audioName);
            SoundCtrl soundCtrl = GetSoundCtrlByType(audioClipData.aUDIOCATEGORY, planetID);

            if (soundCtrl == null)
            {
                throw new System.Exception(audioClipData.aUDIOCATEGORY.ToString() + "의" + Defines.ERROR_MISSING_SOUNDCTRL);
            }

            soundCtrl.PlayOnce(audioClipData);
        }
        catch (System.Exception e)
        {
            Debug.LogError(e.Message);
        }
    }

    // 사운드 정지할 경우 -> 사운드 정지의 경우, 오디오 소스에 영향이 가는 것이므로, 오디오클립 이름이 아니라, 오디오 카테고리를 매개변수로 전달한다.
    public void StopSound(AUDIOCATEGORY aUDIOCATEGORY, string planetID = null)
    {
        SoundCtrl soundCtrl = GetSoundCtrlByType(aUDIOCATEGORY, planetID);

        if (soundCtrl == null) return;

        soundCtrl.StopSound();
    }
    #endregion


    #region  Audio Ctrl(설정창 제어용)
    // 각 카테고리 별 볼륨 설정
    public void MasterVolumeCtrl(float value)
    {
        audioMixer.SetFloat("Master", (1 - value) * -80.0f);
    }

    public void BGMVolumeCtrl(float value)
    {
        audioMixer.SetFloat("BGM", (1 - value) * -80.0f);
    }
    public void EffectVolumeCtrl(float value)
    {
        audioMixer.SetFloat("Effect", (1 - value) * -80.0f);
    }
    public float GetVolume(string name)
    {
        audioMixer.GetFloat(name, out float _volume);
        return ((_volume / 80.0f) + 1);
    }

    // 모든 사운드 pause
    public void PauseAllSound()
    {
        AudioListener.pause = true;
        onPauseAll.Invoke();
    }

    // 모든 사운드 다시 재생(일시 정지 된 시점부터 다시 재생)
    public void ResumeAllSound()
    {
        AudioListener.pause = false;
        onResumeAll.Invoke();
    }

    // 특정 사운드 카테고리 pause
    public void PauseSound(AUDIOCATEGORY aUDIOCATEGORY, string planetID = null)
    {
        SoundCtrl soundCtrl = GetSoundCtrlByType(aUDIOCATEGORY, planetID);

        if (soundCtrl == null) return;

        soundCtrl.ResumeSound();
    }

    // 특정 사운드 카테고리 다시 재생
    public void ResumeSound(AUDIOCATEGORY aUDIOCATEGORY, string planetID = null)
    {
        SoundCtrl soundCtrl = GetSoundCtrlByType(aUDIOCATEGORY, planetID);

        if (soundCtrl == null) return;

        soundCtrl.ResumeSound();
    }

    // 엠비언스 오디오 클립 교체
    public void ChangeAmbience(string audioName, string planetID = null)
    {
        AudioClipData audioClipData = FindAudioClipDataByName(audioName);
        SoundCtrl soundCtrl = GetSoundCtrlByType(audioClipData.aUDIOCATEGORY, planetID);
        soundCtrl.AmbienceAudioSource.clip = audioClipData.audioClips[0];
    }
    #endregion


    #region 매니저 내부 처리용 Methods
    // 오디오 타입에 다른 사운드 컨트롤 리턴(행성의 경우 행성 ID 필요)
    private SoundCtrl GetSoundCtrlByType(AUDIOCATEGORY aUDIOCATEGORY, string planetID = null)
    {
        switch (aUDIOCATEGORY)
        {
            case AUDIOCATEGORY.LOSTROOM:
                return bgmLostRoom;

            case AUDIOCATEGORY.PLANET:
                Planet planet = Managers.instance.PlanetManager.GetPlanetInSceneByID(planetID);
                if (planetID == null || planet == null)
                {
                    throw new System.Exception(Defines.ERROR_NO_PLANETID_FOR_PARA);
                }
                else
                {
                    return planet.SoundCtrl;
                }

            case AUDIOCATEGORY.EFFECT:
                return effect;

            default:
                return null;
        }
    }

    // 오디오 클립 이름에 따른 오디오 클립 데이터(struct) 리턴, 없을시 에러 throw
    private AudioClipData FindAudioClipDataByName(string _audioName)
    {
        foreach (var audioClipData in audioClipDataList)
        {
            if (audioClipData.audioName == _audioName)
            {
                return audioClipData;
            }

        }

        throw new System.Exception(Defines.ERROR_NO_AUDIOCLIPDATA);
    }

    // 사운드 컨트롤 미리 찾아서 변수에 할당
    void SetAudioSource()
    {
        bgmLostRoom = GameObject.FindGameObjectWithTag(Defines.Tag_BGM_LOSTROOM)?.GetComponent<SoundCtrl>();
        bgmPlanet = GameObject.FindGameObjectWithTag(Defines.Tag_BGM_PLANETBGM)?.GetComponent<SoundCtrl>();
        effect = GameObject.FindGameObjectWithTag(Defines.Tag_SOUNDEFFECT)?.GetComponent<SoundCtrl>();
    }

    // BGM 사운드 겹치지 않게 조정
    void StopOtherBGM(AUDIOCATEGORY aUDIOCATEGORY)
    {
        switch (aUDIOCATEGORY)
        {
            case AUDIOCATEGORY.LOSTROOM:
                StopSound(AUDIOCATEGORY.PLANET);
                break;

            case AUDIOCATEGORY.PLANET:
                StopSound(AUDIOCATEGORY.LOSTROOM);
                break;
        }
    }
    #endregion
}
