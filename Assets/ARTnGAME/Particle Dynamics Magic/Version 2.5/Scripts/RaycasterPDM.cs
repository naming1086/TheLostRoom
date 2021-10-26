using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class RaycasterPDM : MonoBehaviour {

    public GameObject particlePool;
    public bool sphereCast = false;
    public bool Always_on = false;
    public Transform Pointer;
    Transform this_transform;
    Vector3 Start_Position;
    public float rayCastDist = 0;
    public float sphereRadius = 0.1f;
    public string disableNamed = "FUR";

    // Use this for initialization
    void Start () {
        this_transform = transform;
    }

    // Update is called once per frame
    void Update()
    {
        if (Input.GetMouseButton(0) || Always_on)
        {
            Ray ray = Camera.main.ScreenPointToRay(Input.mousePosition);

            //if (particlePool)
            //{
            if (Always_on)
            {
                if (Pointer != null)
                {
                    ray = new Ray(Pointer.position, Pointer.forward);
                    Start_Position = Pointer.position;
                }
                else
                {
                    //ray = new Ray(this_transform.position, this_transform.forward);
                }
            }

           
            float maxDist = Mathf.Infinity;
            if (rayCastDist > 0)
            {
                maxDist = rayCastDist;
            }

            if (sphereCast) {

                RaycastHit[] hits = Physics.SphereCastAll(ray, sphereRadius, maxDist);

                if (hits != null && hits.Length > 0)
                {                    
                    for (int j = 0; j < hits.Length; j++)
                    {
                        MeshRenderer[] meshFilters = hits[j].transform.gameObject.GetComponentsInChildren<MeshRenderer>(false);
                        for (int i = 0; i < meshFilters.Length; i++)
                        {
                            if (hits[j].transform.gameObject.name.Contains(disableNamed))
                            {
                                meshFilters[i].enabled = false;
                            }
                        }
                    }
                }
            }
            else
            {
                RaycastHit hit = new RaycastHit();
                if (Physics.Raycast(ray, out hit, maxDist))
                {
                    MeshRenderer[] meshFilters = hit.transform.gameObject.GetComponentsInChildren<MeshRenderer>(false);
                    for (int i = 0; i < meshFilters.Length; i++)
                    {
                        if (hit.transform.gameObject.name.Contains(disableNamed))
                        {
                            meshFilters[i].enabled = false;
                        }
                    }
                }
            }
            //}
        }
    }
}
