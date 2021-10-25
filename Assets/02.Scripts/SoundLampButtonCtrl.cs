using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class SoundLampButtonCtrl : MonoBehaviour
{
    public LostStoryRecorder lostStoryRecorder;

    private Animator animator;

    private new AudioSource audio;
    public AudioClip buttonOn;
    public AudioClip buttonOff;

    public GameObject buttonCollider;

    public bool onButton;

    void Awake()
    {
        audio = GetComponent<AudioSource>();
        animator = GetComponent<Animator>();
    }

    void Start()
    {
        onButton = false;
    }

    void Update()
    {

    }

    void OnTriggerEnter(Collider coll)
    {
        // 버튼 콜라이더가 버튼 몸체가 닿고 && 버튼이 off
        if (coll.CompareTag("BUTTON") && !onButton)
        {
            buttonCollider.GetComponent<BoxCollider>().enabled = false;     // 콜라이더 비활성화
            Debug.Log("버튼온");
            //onButton = true;                                              // 버튼 on
            audio.PlayOneShot(buttonOn, 1.0f);                              // 버튼 on Sound 재생
            this.gameObject.GetComponent<Rigidbody>().isKinematic = true;   // configurable joint 무효화
            transform.localPosition = new Vector3(-0.01f, 0.1039f, 0);           // 위치 고정
            animator.SetBool("isRecord", true);                             // 버튼 깜빡임 애니메이션 on

            Invoke("OnButton", 1.0f);
        }
    }

    void OnCollisionEnter(Collision coll)
    {
        // 버튼 콜라이더가 오른손의 검지 손가락에 닿고 && 버튼 on
        if (coll.collider.CompareTag("RIGHTINDEX") && onButton)
        {
            Debug.Log("버튼오프");
            audio.PlayOneShot(buttonOff, 1.0f);                             // 버튼 off 사운드 재생
            transform.localPosition = new Vector3(-0.0323f, 0.1039f, 0);         // 위치 재고정
            buttonCollider.GetComponent<BoxCollider>().enabled = true;      // 콜라이더 활성화
            this.gameObject.GetComponent<Rigidbody>().isKinematic = false;  // configurale joint 재 활성화
            animator.SetBool("isRecord", false);                            // 버튼 깜빡임 애니메이션 off
            Invoke("OffButton", 1.0f);
        }
    }

    void OnButton()
    {
        onButton = true;
    }

    void OffButton()
    {
        onButton = false;
    }
}
