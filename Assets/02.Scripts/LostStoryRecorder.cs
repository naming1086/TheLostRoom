using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class LostStoryRecorder : MonoBehaviour
{
    public SoundLampCtrl soundLampCtrl;

    public int recordTime;
    public int resultValue;
    public int cutValue = 15;

    public float rmsValue;
    public float moderate;

    public bool isRecord;
    public new AudioSource audio;

    void Awake()
    {
        audio = GetComponent<AudioSource>();
    }

    void Start()
    {
        isRecord = false;
        recordTime = 180;
    }

    void Update()
    {

    }

    // 램프 버튼을 누르고 && 램프 주둥이가 입 콜라이더에 충돌하면 >> 녹음 가능한 상태
    public void StartRecord()
    {
        audio.clip = Microphone.Start(Microphone.devices[0].ToString(), true, recordTime, 44100);
    }

    public void StopRecord()
    {
        int lastTime = Microphone.GetPosition(null);

        if (lastTime == 0)
        {
            return;
        }
        else
        {
            Microphone.End(Microphone.devices[0]);  // 녹음 중지

            float[] samples = new float[audio.clip.samples];
            audio.clip.GetData(samples, 0);

        }
    }

    public void SaveSound()
    {

    }

    public void PlaySound()
    {
        audio.Play();
    }

}
