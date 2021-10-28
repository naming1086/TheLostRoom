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
        seeds.SetActive(true);
    }

    void Update()
    {

    }
}
