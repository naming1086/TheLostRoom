using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using VR.ReadOnlys;

public class Planet : MonoBehaviour
{
    private string planetID;
    public string PlanetID { get => planetID; }

    [SerializeField]
    private SoundCtrl soundCtrl;
    public SoundCtrl SoundCtrl { get => soundCtrl; }

    void Start()
    {

    }

    // Update is called once per frame
    void Update()
    {

    }
}
