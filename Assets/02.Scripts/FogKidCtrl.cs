using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Events;
using VR.ReadOnlys;

public class FogKidCtrl : MonoBehaviour
{
    [SerializeField] private int touchCount = 0;
    [SerializeField] private float moveSpeed = 2.0f;
    [SerializeField] private float damping = 10.0f;
    [SerializeField] private Vector3 startPos = new Vector3(0, 1.4f, 0);    // 안개아이 시작 지점

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
        if (coll.CompareTag(Defines.TAG_INDEX))
        {
            // 손이 안개아이를 만지면 > touchCount++
            touchCount++;

            if (touchCount == 1)
            {
                // touchCount = 1일때, 자기 자신을 부모로 && 시작지점(0,1.4f,0)으로 이동
                if (transform.parent != null)
                {
                    transform.SetParent(null);
                }
                StartCoroutine(MoveToStartPos());

                touchCount = 2;
            }
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

    // 안개아이 시작지점(0,1.4f,0)으로 이동
    IEnumerator MoveToStartPos()
    {
        Vector3 dir = startPos - transform.position;
        Quaternion rot = Quaternion.LookRotation(dir);

        while (transform.position != startPos)
        {
            transform.rotation = Quaternion.Slerp(transform.rotation, rot, Time.deltaTime * damping);
            transform.Translate(Vector3.forward * Time.deltaTime * moveSpeed);

            yield return null;
        }
    }

    // 2. 방 안을 이동하다가 : 이동 로직 
    // 하나를 선택하는 로직 >> 음성 인식 Neutral, Sad, Happy, Bored, Angry에 따른 선택
    // 2) 각각의 씨앗마다의 이동경로로 이동
    // 3. 다섯개의 씨앗 중 하나의 안으로 들어간다. 



}
