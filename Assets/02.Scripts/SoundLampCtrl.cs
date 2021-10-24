using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class SoundLampCtrl : MonoBehaviour
{
    public bool onButton;
    public GameObject recordButton;
    public GameObject lampBody;

    void Start()
    {
        onButton = false;
    }

    void Update()
    {

    }

    void OnCollisionEnter(Collision coll)
    {
        // 버튼 콜라이더가 버튼 몸체가 닿으면
        if (coll.collider.CompareTag(""))
        {
            if (!onButton)          // 버튼이 off일 경우 버튼 on
            {
                onButton = true;
            }
            else
            {
                onButton = false;   // 버튼이 on일 경우 버튼 off
            }
        }

    }
}
