// (c) 2016-2021 Martin Cvengros. All rights reserved. Redistribution of source code without permission not allowed.
// uses FMOD by Firelight Technologies Pty Ltd

using UnityEngine;

namespace AudioStream
{
    // ========================================================================================================================================
    #region About
    /// <summary>
    /// About informational strings
    /// Uses build settings info
    /// </summary>
    public static class About
    {
        public static string versionNumber = "2.4.4";
        public static string versionString = "AudioStream v " + versionNumber + " © 2016-2021 Martin Cvengros";
        public static string fmodNotice = ", uses FMOD by Firelight Technologies Pty Ltd";
        public static string buildString = About.UpdateBuildString();
        public static string UpdateBuildString()
        {
            return string.Format("Built {0}, Unity version: {1}{2}, {3} bit, {4}"
            , BuildSettings.buildTimeS
            , Application.unityVersion
            , !string.IsNullOrEmpty(BuildSettings.scriptingBackendS) ? (", " + BuildSettings.scriptingBackendS) : ""
            , System.Runtime.InteropServices.Marshal.SizeOf(System.IntPtr.Zero) * 8
            , AudioStreamSupport.UnityAudioLatencyDescription()
            );
        }
        public static string defaultOutputProperties = string.Format("System default output samplerate: {0}, application speaker mode: {1} [HW: {2}]", AudioSettings.outputSampleRate, AudioSettings.speakerMode, AudioSettings.driverCapabilities);
        public static string proxyUsed
        {
            get
            {
                var proxyString = AudioStream_ProxyConfiguration.Instance.ProxyString(true);
                return string.IsNullOrEmpty(proxyString) ? null : string.Format("Proxy server to be used: {0}", proxyString);
            }
        }
    }
    #endregion
}