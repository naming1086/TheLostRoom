using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class SpeakingChecker : MonoBehaviour
{
    public SoundLampButtonCtrl soundLampButtonCtrl;
    public GameObject voiceParticle;

    void Start()
    {
        voiceParticle.SetActive(false);
    }

    // 녹음 버튼 on && MainCamera 콜라이더와 닿고 있는 동안 이펙터 형성
    void OnTriggerStay(Collider coll)
    {
        if (soundLampButtonCtrl.onButton && coll.CompareTag("MainCamera"))
        {
            voiceParticle.SetActive(true);
        }
        else
        {
            voiceParticle.SetActive(false);
        }
    }
}
