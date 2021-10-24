using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class LostStoryRecorder : MonoBehaviour
{
    private int sampleFre;
    public int recordLen;
    public int resultValue;
    public int cutValue = 15;

    private float[] samples;
    public float rmsValue;
    public float moderate;

    public bool isRecord;
    public new AudioSource audio;

    void Awake()
    {
        audio = GetComponent<AudioSource>();
        samples = new float[sampleFre];
    }

    void Start()
    {
        isRecord = false;
        sampleFre = 44100;
        recordLen = 10;
    }

    void Update()
    {
        audio.GetOutputData(samples, 0);  //-1.0f ~ 1.0f
        float sum = 0;
        for (int i = 0; i < samples.Length; i++)
        {
            sum += samples[i] * samples[i];
        }

        rmsValue = Mathf.Sqrt(sum / samples.Length);    // 말을 하면 float 값으로 인식됨
        rmsValue = rmsValue * moderate;
        rmsValue = Mathf.Clamp(rmsValue, 0, 100);
        resultValue = Mathf.RoundToInt(rmsValue);

        if (resultValue < cutValue)
        {
            resultValue = 0;
        }
    }

    // 램프 버튼을 누르고 && 램프 주둥이가 입 콜라이더에 충돌하면 >> 녹음 가능한 상태
    public void RecordSound()
    {
        audio.clip = Microphone.Start(Microphone.devices[0].ToString(), false, recordLen, sampleFre);
    }

    public void SaveSound()
    {

    }

    public void PlaySound()
    {
        audio.Play();
    }
}
