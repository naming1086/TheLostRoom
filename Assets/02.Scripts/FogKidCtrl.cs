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
}
