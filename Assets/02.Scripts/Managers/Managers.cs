using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Events;
using UnityEngine.SceneManagement;

public class Managers : MonoBehaviour
{
    public static Managers instance;

    // 행성 매니저 

    // 씬 매니저

    // 모드 매니저 

    // 사운드 매니저 

    // 메뉴 매니저

    // HMD 매니저

    // 매니저 세팅 끝 이벤트 
    public UnityEvent onManagerSetDone;

    public InAppDebugLog debugLog;

    void Awake()
    {
        #region Singleton
        if (instance == null)
        {
            instance = this;
            DontDestroyOnLoad(this.gameObject);
        }
        else
        {
            Destroy(this.gameObject);
        }
        #endregion

        Initialize();
    }

    void OnEnable()
    {
        UnityEngine.SceneManagement.SceneManager.sceneLoaded += OnSceneLoaded;
    }

    void OnDisable()
    {
        UnityEngine.SceneManagement.SceneManager.sceneLoaded -= OnSceneLoaded;
    }

    void OnSceneLoaded(Scene scene, LoadSceneMode mode)
    {
        Initialize();
    }

    void Initialize()
    {
        onManagerSetDone.Invoke();
    }
}
