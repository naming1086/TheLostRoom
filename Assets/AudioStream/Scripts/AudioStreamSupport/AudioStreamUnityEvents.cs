// (c) 2016-2021 Martin Cvengros. All rights reserved. Redistribution of source code without permission not allowed.
// uses FMOD by Firelight Technologies Pty Ltd

using UnityEngine;
using UnityEngine.Events;

namespace AudioStream
{
    // ========================================================================================================================================
    #region Unity events
    [System.Serializable]
    public class EventWithStringParameter : UnityEvent<string> { };
    [System.Serializable]
    public class EventWithStringBoolParameter : UnityEvent<string, bool> { };
    [System.Serializable]
    public class EventWithStringStringParameter : UnityEvent<string, string> { };
    [System.Serializable]
    public class EventWithStringStringObjectParameter : UnityEvent<string, string, object> { };
    [System.Serializable]
    public class EventWithStringAudioClipParameter : UnityEvent<string, AudioClip> { };
    #endregion
}