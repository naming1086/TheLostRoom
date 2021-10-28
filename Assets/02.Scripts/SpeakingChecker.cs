using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class SpeakingChecker : MonoBehaviour
{
    private new AudioSource audio;

    public SoundLampButtonCtrl soundLampButtonCtrl;
    public GameObject voiceParticle;
    public GameObject fogKidUp;
    public GameObject fogKid;

    void Awake()
    {
        audio = GetComponent<AudioSource>();
    }

    void Start()
    {
        voiceParticle.SetActive(false);
        fogKidUp.SetActive(false);
        fogKid.SetActive(false);
    }

    void Update()
    {
        // 한번 켜진 후에
        if (soundLampButtonCtrl.onButton)
        {
            // 한번 꺼지면 
            if (!soundLampButtonCtrl.onButton)
            {
                voiceParticle.SetActive(false);
                audio.Stop();
                audio.loop = true;
                fogKidUp.SetActive(true);

                StartCoroutine(OnFogKid());
            }
        }
    }

    // 녹음 버튼 on && MainCamera 콜라이더와 닿고 있는 동안 이펙터 형성
    void OnTriggerEnter(Collider coll)
    {
        if (soundLampButtonCtrl.onButton && coll.CompareTag("MainCamera"))
        {
            voiceParticle.SetActive(true);  // 이펙터 활성화    
            audio.Play();                   // 오디오 재생
            audio.loop = true;
        }
    }

    void OnTriggerExit(Collider coll)
    {
        // 이펙터 일시정지
        voiceParticle.GetComponent<ParticleSystem>().Pause();
        audio.Pause();
        audio.loop = true;
    }

    // 안개 아이 활성화
    IEnumerator OnFogKid()
    {
        yield return new WaitForSeconds(1.0f);

        fogKid.SetActive(true);

        yield return new WaitForSeconds(1.0f);

        fogKidUp.SetActive(false);
    }
}
