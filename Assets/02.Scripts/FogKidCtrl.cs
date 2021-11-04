using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class FogKidCtrl : MonoBehaviour
{
    private Animator animator;

    public GameObject seeds;

    void Awake()
    {
        animator = GetComponent<Animator>();
    }

    void Start()
    {
        // 안개 아이 생성시 씨앗들 활성화 
        seeds.SetActive(true);
    }

    void Update()
    {

    }

    void OnTriggerEnter(Collider coll)
    {
        if (coll.CompareTag("INDEX"))
        {
            animator.SetBool("isGo", true);
        }

        if (coll.CompareTag("SEED"))
        {
            Invoke("OffFogKid", 2.0f);
        }
    }

    void OffFogKid()
    {
        this.gameObject.SetActive(false);
    }

    // 1. 재생 버튼을 누르면, spawn point에 생긴다.
    // 손이 안개아이를 만지면 > touchCoun++, touchCount1일때 1) 시작지점으로 이동
    // 2. 방 안을 이동하다가 : 이동 로직 
    // 하나를 선택하는 로직 >> 음성 인식 Neutral, Sad, Happy, Bored, Angry에 따른 선택
    // 2) 각각의 씨앗마다의 이동경로로 이동
    // 3. 다섯개의 씨앗 중 하나의 안으로 들어간다. 



}
