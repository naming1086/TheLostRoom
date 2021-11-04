using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class SeedCtrl : MonoBehaviour
{
    private Animator animator;

    void Awake()
    {
        animator = GetComponent<Animator>();
    }

    void Start()
    {

    }

    void Update()
    {

    }

    void OnTriggerEnter(Collider coll)
    {
        if (coll.CompareTag("FOG"))
        {
            animator.SetBool("isTwinkle", true);
        }
    }
}
