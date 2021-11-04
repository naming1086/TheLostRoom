using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class SpeakingChecker : MonoBehaviour
{
    private new AudioSource audio;

    public RecordButtonCtrl recordButtonCtrl;
    public GameObject voiceParticle;
    public GameObject fogKid;
    private ParticleSystem voicePS;

    void Awake()
    {
        audio = GetComponent<AudioSource>();
        voicePS = voiceParticle.GetComponent<ParticleSystem>();
    }

    void Start()
    {
        voiceParticle.SetActive(false);
        fogKid.SetActive(false);
    }

    void Update()
    {
        var main = voicePS.main;
        if (recordButtonCtrl.touchCount == 2)
        {
            main.gravityModifier = -0.5f;
            main.simulationSpeed = 0.2f;
            audio.Stop();
            audio.loop = true;

            StartCoroutine(OnFogKid());
        }
    }

    // 녹음 버튼 on && MainCamera 콜라이더와 닿고 있는 동안 이펙터 형성
    void OnTriggerEnter(Collider coll)
    {
        if (recordButtonCtrl.onRecordButton && coll.CompareTag("MainCamera"))
        {
            voiceParticle.SetActive(true);  // 이펙터 활성화    
            voiceParticle.GetComponent<ParticleSystem>().Play();
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
        yield return new WaitForSeconds(3.0f);

        fogKid.SetActive(true);

        yield return new WaitForSeconds(3.0f);

        voiceParticle.SetActive(false);

        yield return new WaitForSeconds(5.0f);

    }
}
