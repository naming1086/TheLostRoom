using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Events;
using UnityEngine.UI;
using VR.ReadOnlys;
using System.Threading;
using System.Threading.Tasks;
using Autohand;

public class PlanetManager : MonoBehaviour
{
    #region Variables

    //씬 내 모든 행성 관리용 리스트
    private List<Planet> planetListForTheScene = new List<Planet>();
    public List<Planet> PlanetListForTheScene { get => planetListForTheScene; }
    #endregion


    #region extra getters
    public Planet GetPlanetInSceneByID(string _planetID)
    {
        foreach (var planetInScene in planetListForTheScene)
        {
            if (planetInScene.PlanetID == _planetID)
            {
                return planetInScene;
            }
        }
        return null;
    }
    #endregion
}
