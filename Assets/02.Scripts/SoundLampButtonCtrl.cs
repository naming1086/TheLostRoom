using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class SoundLampButtonCtrl : MonoBehaviour
{
    public LostStoryRecorder lostStoryRecorder;

    private new AudioSource audio;
    public AudioClip buttonOn;
    public AudioClip buttonOff;

    public bool onButton;
    public bool offButton;

    void Awake()
    {
        audio = GetComponent<AudioSource>();
    }

    void Start()
    {
        onButton = false;
    }

    void Update()
    {

    }

    public void OnPressed()
    {
        // 버튼 콜라이더가 버튼 몸체가 닿으면
        if (!onButton)          // 버튼이 off일 경우 버튼 on
        {
            Debug.Log("버튼온");
            onButton = true;                        // 버튼 on
            audio.PlayOneShot(buttonOn, 0.8f);      // 버튼 on Sound 재생
                                                    //lostStoryRecorder.StartRecord();        // 녹음 시작
        }
        else                    // 버튼이 on일 경우 버튼 off
        {
            Debug.Log("버튼오프");
            onButton = false;                       // 버튼 OFF
            audio.PlayOneShot(buttonOff, 0.8f);     // 버튼 off Sound 재생
                                                    //lostStoryRecorder.StopRecord();         // 녹음 중지
        }
    }

    // void OnCollisionEnter(Collision coll)
    // {
    //     // 버튼 콜라이더가 버튼 몸체가 닿으면
    //     if (coll.collider.CompareTag("RIGHTINDEX"))
    //     {
    //         if (!onButton)          // 버튼이 off일 경우 버튼 on
    //         {
    //             Debug.Log("버튼온");
    //             onButton = true;                        // 버튼 on
    //             audio.PlayOneShot(buttonOn, 0.8f);      // 버튼 on Sound 재생
    //             //lostStoryRecorder.StartRecord();        // 녹음 시작
    //         }
    //         else                    // 버튼이 on일 경우 버튼 off
    //         {
    //             Debug.Log("버튼오프");
    //             onButton = false;                       // 버튼 OFF
    //             audio.PlayOneShot(buttonOff, 0.8f);     // 버튼 off Sound 재생
    //             //lostStoryRecorder.StopRecord();         // 녹음 중지
    //         }
    //     }

    // }
}
