using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Events;
using VR.ReadOnlys;

public class RecordButtonCtrl : MonoBehaviour
{
    public LostStoryRecorder lostStoryRecorder;

    private Animator animator;

    private new AudioSource audio;
    public AudioClip buttonOn;
    public AudioClip buttonOff;

    public GameObject buttonCollider;
    private ConfigurableJoint joint;
    public SoftJointLimit softJointLimit;
    public float limit;

    public int touchCount;

    public bool onRecordButton;     // 녹음 버튼 on
    public bool onPlayButton;       // 플레이 버튼 on

    public GameObject fogKid;           // 안개아이
    public Transform fogKidSpawnPoint;  // 안개아이 생성지점

    public GameObject player;

    void Awake()
    {
        player = GameObject.FindGameObjectWithTag(Defines.TAG_MainCamera);
        audio = GetComponent<AudioSource>();
        animator = GetComponent<Animator>();
        joint = GetComponent<ConfigurableJoint>();
        softJointLimit = new SoftJointLimit();
    }

    void Start()
    {
        onRecordButton = false;
        touchCount = 0;
    }

    void Update()
    {

    }

    void OnTriggerEnter(Collider coll)
    {

    }

    void OnCollisionEnter(Collision coll)
    {
        // 버튼 콜라이더가 오른손의 검지 손가락에 닿고 && 버튼이 off
        if (coll.collider.CompareTag("INDEX") && !onRecordButton)
        {
            touchCount = 1;
            buttonCollider.GetComponent<BoxCollider>().enabled = false;     // 콜라이더 비활성화
            Debug.Log("버튼온");
            //onButton = true;                                              // 버튼 on
            audio.PlayOneShot(buttonOn, 1.0f);                              // 버튼 on Sound 재생
            transform.localPosition = new Vector3(-0.01f, 0.1039f, 0);      // 위치 이동
            softJointLimit.limit = 0;
            joint.linearLimit = softJointLimit;

            animator.SetBool("isRecord", true);                             // 버튼 깜빡임 애니메이션 on

            Invoke("OnButton", 1.0f);
        }

        // 버튼 콜라이더가 오른손의 검지 손가락에 닿고 && 버튼 on
        else if (coll.collider.CompareTag("INDEX") && onRecordButton)
        {
            touchCount = 2;
            Debug.Log("버튼오프");
            audio.PlayOneShot(buttonOff, 1.0f);                             // 버튼 off 사운드 재생
            transform.localPosition = new Vector3(-0.0323f, 0.1039f, 0);    // 위치 이동
            buttonCollider.GetComponent<BoxCollider>().enabled = true;      // 콜라이더 활성화
            softJointLimit.limit = 0.01f;
            joint.linearLimit = softJointLimit;

            animator.SetBool("isRecord", false);                            // 버튼 깜빡임 애니메이션 off
            Invoke("OffButton", 1.0f);
        }


    }

    void OnButton()
    {
        onRecordButton = true;
    }

    void OffButton()
    {
        onRecordButton = false;
    }

    // playButton을 누르면, FOGSPAWM에 안개아이 생성
    // OnPressed Event 발생시 호출
    public void SpawnFogKid()
    {
        // 사운드 램프의 하위 오브젝트로 생성
        GameObject fog = Instantiate(fogKid, transform.position, Quaternion.identity);
        fog.transform.SetParent(fogKidSpawnPoint);
    }


    // 한번 녹음을 하면 재생 버튼 physics gadgetbutton 활성화
}
