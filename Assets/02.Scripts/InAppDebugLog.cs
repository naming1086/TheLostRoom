using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using TMPro;

public class InAppDebugLog : MonoBehaviour
{
    public TMP_Text text;

    public void ShowDebugLog(string msg)
    {
        text.text += "\n" + msg;
    }

}
