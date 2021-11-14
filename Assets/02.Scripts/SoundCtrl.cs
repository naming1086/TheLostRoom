using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;

public class SoundCtrl : MonoBehaviour
{
    #region  Variables
    // 여러 음원을 리스트로 전달받아 이어서 재생하기 위해 오디오 소스가 2개 필요.
    private AudioSource[] audioSourceArray;
    private int audioSourceToggle = 0;  // audioSource를 toggle하기 위한 변수.

    // SoundManager로부터 전달받은 플레이할 오디오클립 리스트
    private AudioClip[] audioClipArray;

    // 오디오 플레이에 필요한 멤버변수들
    private int nextClip = 0;           // 다음 곡 index
    private double nextStartTime;       // 다음 곡 시작시간
    private double interval = 0.0d;     // 곡 사이의 간격
    private bool isRepeatable = false;  // 리스트 전체 반복재생 여부
    private bool isPlay = false;        // LateUpdate에서 사용할 플레이여부 Flag 변수

    // 오디오 일시정지 시점 기록용 변수
    private double audioPauseTime;
    private bool isPaused = false;

    [Header("본 사운드 컨트롤의 분류 선택")]
    [SerializeField]
    private AUDIOCATEGORY aUDIOCATEGORY;

    [SerializeField]
    private AudioSource ambienceAudioSource;
    public AudioSource AmbienceAudioSource { get => ambienceAudioSource; }
    #endregion

    #region  Unity Method
    void Awake()
    {
        audioSourceArray = GetComponents<AudioSource>();
    }

    void Start()
    {
        SetSoundCtrl();
    }

    // Update is called once per frame
    void Update()
    {

    }
    #endregion


    #region Custum Method
    // 사운드 매니저에 등록
    public void SetSoundCtrl()
    {
        switch (aUDIOCATEGORY)
        {
            case AUDIOCATEGORY.LOSTROOM:
                Managers.instance.SoundManager.BgmLostRoom = this;
                break;

            case AUDIOCATEGORY.PLANET:
                Managers.instance.SoundManager.BgmPlanet = this;
                break;

            case AUDIOCATEGORY.EFFECT:
                Managers.instance.SoundManager.Effect = this;
                break;
        }
    }

    // 기존 재생 클립 바로 정지. SoundManager에서 전달받은 값으로 SoundCtrl의 변수를 초기화 및 사운드 플레이 시작
    public void PlayAudioClipList(AudioClipData audioClipData, bool _isRepeatable = false, double _delayTime = 0.0d, int _startClipindex = 0)
    {
        StopSound();

        this.nextStartTime = AudioSettings.dspTime + 0.2d + _delayTime;
        this.audioClipArray = audioClipData.audioClips;
        this.isRepeatable = _isRepeatable;
        this.nextClip = _startClipindex;
        this.interval = audioClipData.intervalBetweenSongs;

        this.isPlay = true;

        if (ambienceAudioSource != null)
        {
            ambienceAudioSource.PlayScheduled(this.nextStartTime);
        }
    }

    // SoundManager를 통해, PlayOneShot으로 재생하고 싶을 경우 사용
    public void PlayOnce(AudioClipData audioClipData)
    {
        audioSourceArray[0].PlayOneShot(audioClipData.audioClips[0]);
    }

    // 모든 오디오소스 재생 정지 및 변수 초기화
    public void StopSound()
    {
        if (isPlay || (!isPlay & isPaused))
        {
            foreach (AudioSource audioSource in audioSourceArray)
            {
                audioSource.Stop();
                audioSource.clip = null;
            }

            if (ambienceAudioSource != null)
            {
                ambienceAudioSource.Stop();
            }

            isPlay = false;
            isPaused = false;

            InitVariables();
        }
    }

    // 모든 오디오소스 pause
    public void PauseSound()
    {
        if (isPlay && !isPaused)
        {
            foreach (AudioSource audioSource in audioSourceArray)
            {
                audioSource.Pause();
            }

            if (ambienceAudioSource != null)
            {
                ambienceAudioSource.Pause();
            }

            audioPauseTime = AudioSettings.dspTime;
            isPaused = true;
            isPlay = false;
        }
    }

    // 모든 오디오소스 resume
    public void ResumeSound()
    {
        if (isPaused)
        {
            foreach (AudioSource audioSource in audioSourceArray)
            {
                audioSource.UnPause();
            }

            if (ambienceAudioSource != null)
            {
                ambienceAudioSource.UnPause();
            }

            nextStartTime += (AudioSettings.dspTime - audioPauseTime) + 0.05d;  //0.05d는 약간의 시간 보정을 위해 도입
            isPaused = false;
            isPlay = true;
        }
    }


    // 멤버 변수 초기화
    void InitVariables()
    {
        this.audioSourceArray = null;
        this.nextClip = 0;
        this.nextStartTime = 0.0d;
        this.interval = 0.0d;
        this.isRepeatable = false;
    }
    #endregion
}
