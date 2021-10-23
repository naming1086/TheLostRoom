// (c) 2016-2021 Martin Cvengros. All rights reserved. Redistribution of source code without permission not allowed.
// uses FMOD by Firelight Technologies Pty Ltd

using System;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;

namespace AudioStream
{
    /// <summary>
    /// Output device + specific channel selection via mix matrix for media played directly by FMOD only
    /// There is no Editor/Inspector functionality for user sounds (except Unity events) - only API is exposed via this component currently -
    /// Please see how it's used in MediaSourceOutputDeviceDemo scene
    /// </summary>
    public class MediaSourceOutputDevice : MonoBehaviour
	{
        // ========================================================================================================================================
        #region Editor
        [Header("[Setup]")]
        [Tooltip("You can set initial output driver on which audio will be played by default.\r\nMost of functionality is accessible via API only currently. Please see 'MediaSourceOutputDeviceDemo' for more.")]
        /// <summary>
        /// This' FMOD system current output device
        /// </summary>
        public int outputDriverID = 0;
        [Tooltip("Turn on/off logging to the Console. Errors are always printed.")]
        public LogLevel logLevel = LogLevel.ERROR;

        #region Unity events
        [Header("[Events]")]
        public EventWithStringParameter OnPlaybackStarted;
        public EventWithStringParameter OnPlaybackStopped;
        public EventWithStringParameter OnPlaybackPaused;
        public EventWithStringStringParameter OnError;
        #endregion

        [Header("[Output device latency (ms) (info only)]")]
        [Tooltip("TODO: PD readonly Computed for current output device at runtime")]
        public float latencyBlock;
        [Tooltip("TODO: PD readonly Computed for current output device at runtime")]
        public float latencyTotal;
        [Tooltip("TODO: PD readonly Computed for current output device at runtime")]
        public float latencyAverage;
        /// <summary>
        /// GO name to be accessible from all the threads if needed
        /// </summary>
        string gameObjectName = string.Empty;
        #endregion
        // ========================================================================================================================================
        #region FMOD start/release/play sound
        /// <summary>
        /// Component can play multiple user's sounds via API - manage all of them separately + their volumes
        /// </summary>
        List<FMOD.Channel> channels = new List<FMOD.Channel>();
        /// <summary>
        /// Component startup sync
        /// </summary>
        [HideInInspector]
        public bool ready = false;
        [HideInInspector]
        public string fmodVersion;
        /// <summary>
        /// The system which plays the sound on selected output - one per output, released when sound (redirection) is stopped,
        /// </summary>
        FMODSystemOutputDevice outputdevice_system = null;
        protected FMOD.RESULT result = FMOD.RESULT.OK;
        FMOD.RESULT lastError = FMOD.RESULT.OK;

        void StartFMODSystem()
        {
            /*
             * before creating system for target output, check if it wouldn't fail with it first :: device list can be changed @ runtime now
             * if it would, fallback to default ( 0 ) which should be hopefully always available - otherwise we would have failed miserably some time before already
             */
            if (!FMODSystemsManager.AvailableOutputs(this.logLevel, this.gameObjectName, this.OnError).Select(s => s.id).Contains(this.outputDriverID))
            {
                LOG(LogLevel.WARNING, "Output device {0} is not available, using default output (0) as fallback", this.outputDriverID);
                this.outputDriverID = 0;
            }

            // TODO: throws exception in the constructor
            this.outputdevice_system = FMODSystemsManager.FMODSystemForOutputDevice_Create(this.outputDriverID, false, this.logLevel, this.gameObjectName, this.OnError);
            this.fmodVersion = this.outputdevice_system.VersionString;

            // compute latency as last step
            // TODO: move latency to system creation

            uint blocksize;
            int numblocks;
            result = this.outputdevice_system.System.getDSPBufferSize(out blocksize, out numblocks);
            ERRCHECK(result, "outputdevice_system.System.getDSPBufferSize");

            int samplerate;
            FMOD.SPEAKERMODE sm;
            int speakers;
            result = this.outputdevice_system.System.getSoftwareFormat(out samplerate, out sm, out speakers);
            ERRCHECK(result, "outputdevice_system.System.getSoftwareFormat");

            float ms = (float)blocksize * 1000.0f / (float)samplerate;

            this.latencyBlock = ms;
            this.latencyTotal = ms * numblocks;
            this.latencyAverage = ms * ((float)numblocks - 1.5f);
        }

        /*
         * System is released automatically when the last sound being played via it is stopped (sounds are tracked by 'sounds' member list)
         * There was not a good place to release it otherwise since it has to be released after all sounds are released and:
         * - OnApplicationQuit is called *before* OnDestroy so it couldn't be used (sound can be released when switching scenes)
         * - when released in class destructor (exit/ domain reload) it led to crashes / deadlocks in FMOD - *IF* a sound was created/played on that system before -
         */

        /// <summary>
        /// Stops sound created by this component on shared FMOD system and removes reference to it
        /// If the system is playing 0 sounds afterwards, it is released too
        /// </summary>
        void StopFMODSystem()
        {
            if (this.outputdevice_system != null)
            {
                foreach (var channel in this.channels)
                {
                    this.outputdevice_system.ReleaseUserSound(channel, this.logLevel, this.gameObjectName, this.OnError);
                }

                this.channels.Clear();

                FMODSystemsManager.FMODSystemForOutputDevice_Release(this.outputdevice_system, this.logLevel, this.gameObjectName, this.OnError);

                this.outputdevice_system = null;
            }
        }
        #endregion
        // ========================================================================================================================================
        #region Unity lifecycle
        void Start()
        {
            // FMODSystemsManager.InitializeFMODDiagnostics(FMOD.DEBUG_FLAGS.LOG);
            this.gameObjectName = this.gameObject.name;

            this.StartFMODSystem();

            this.ready = true;
        }
        void Update()
        {
            // update output system
            if (
                this.outputdevice_system != null
                && this.outputdevice_system.SystemHandle != IntPtr.Zero
                )
            {
                this.outputdevice_system.Update();
            }
        }

        void OnDestroy()
        {
            this.StopFMODSystem();
        }
        #endregion
        // ========================================================================================================================================
        #region Support
        void ERRCHECK(FMOD.RESULT result, string customMessage, bool throwOnError = true)
        {
            this.lastError = result;

            AudioStreamSupport.ERRCHECK(result, this.logLevel, this.gameObjectName, this.OnError, customMessage, throwOnError);
        }

        void LOG(LogLevel requestedLogLevel, string format, params object[] args)
        {
            AudioStreamSupport.LOG(requestedLogLevel, this.logLevel, this.gameObjectName, this.OnError, format, args);
        }

        public string GetLastError(out FMOD.RESULT errorCode)
        {
            if (!this.ready)
                errorCode = FMOD.RESULT.ERR_NOTREADY;
            else
                errorCode = this.lastError;

            return FMOD.Error.String(errorCode);
        }
        #endregion
        // ========================================================================================================================================
        #region Output device
        /// <summary>
        /// Use different output
        /// </summary>
        /// <param name="_outputDriverID"></param>
        public void SetOutput(int _outputDriverID)
        {
            if (!this.ready)
            {
                Debug.LogErrorFormat("Please make sure to wait for 'ready' flag before calling this method");
                return;
            }

            if (_outputDriverID == this.outputDriverID)
                return;

            this.outputDriverID = _outputDriverID;

            // redirection is always running so restart it with new output
            this.StopFMODSystem();

            this.StartFMODSystem();
        }
        #endregion
        // ========================================================================================================================================
        #region User sound/channel management / playback
        /// <summary>
        /// Creates an user sound and optionally plays it immediately; returns created channel so it can be played/unpaused later
        /// </summary>
        /// <param name="audioUri"></param>
        /// <param name="volume"></param>
        /// <param name="loop"></param>
        /// <param name="playImmediately"></param>
        /// <param name="mixMatrix"></param>
        /// <param name="outchannels"></param>
        /// <param name="inchannels"></param>
        /// <param name="channel"></param>
        public void StartUserSound(string audioUri
            , float volume
            , bool loop
            , bool playImmediately
            , float[,] mixMatrix
            , int outchannels
            , int inchannels
            , out FMOD.Channel channel
            )
        {
            result = this.outputdevice_system.CreateUserSound(
                audioUri
                , loop
                , volume
                , playImmediately
                , mixMatrix
                , outchannels
                , inchannels
                , this.logLevel
                , this.gameObjectName
                , this.OnError
                , out channel
                );

            if (result == FMOD.RESULT.OK)
            {
                this.channels.Add(channel);

                if (this.OnPlaybackStarted != null && playImmediately)
                    this.OnPlaybackStarted.Invoke(this.gameObjectName);
            }
            else
            {
                var msg = string.Format("Can't create sound: {0}", FMOD.Error.String(result));

                LOG(LogLevel.ERROR, msg);
                
                if (this.OnError != null)
                    this.OnError.Invoke(this.gameObjectName, msg);
            }
        }
        /// <summary>
        /// Effectively just unpauses created channel
        /// </summary>
        /// <param name="channel"></param>
        /// <returns></returns>
        public FMOD.RESULT PlayUserSound(FMOD.Channel channel
            , float volume
            , bool loop
            , float[,] mixMatrix
            , int outchannels
            , int inchannels
            , out FMOD.Channel newChannel
            )
        {
            result = channel.setPaused(false);
            // ERRCHECK(result, "channel.setPaused", false);

            // channel was released already, most likely due to finished playback - start a new one and return it
            // usually with FMOD.RESULT.ERR_INVALID_HANDLE / FMOD.RESULT.ERR_CHANNEL_STOLEN but we'll try in any case..
            if (result != FMOD.RESULT.OK)
            {
                LOG(LogLevel.WARNING, "Channel finished/stolen, creating new one.. ");

                result = this.outputdevice_system.PlayUserChannel(channel, volume, loop, mixMatrix, outchannels, inchannels, this.logLevel, this.gameObjectName, this.OnError, out newChannel);
                if (result == FMOD.RESULT.OK)
                {
                    this.channels.Remove(channel);
                    this.channels.Add(newChannel);
                }
            }
            else
            {
                // just update parameters
                result = channel.setLoopCount(loop ? -1 : 0);
                ERRCHECK(result, "channel.setLoopCount", false);

                result = channel.setVolume(volume);
                ERRCHECK(result, "channel.setVolume", false);

                // keep the behaviour consistent 
                newChannel = channel;

                result = FMOD.RESULT.OK;
            }

            if (result == FMOD.RESULT.OK)
                if (this.OnPlaybackStarted != null)
                    this.OnPlaybackStarted.Invoke(this.gameObjectName);

            return result;
        }
        /// <summary>
        /// Effectively just pauses the channel, and sets playback position to 0
        /// </summary>
        /// <param name="channel"></param>
        /// <returns></returns>
        public FMOD.RESULT StopUserSound(FMOD.Channel channel)
        {
            result = channel.setPosition(0, FMOD.TIMEUNIT.MS);
            ERRCHECK(result, "channel.setPosition", false);

            if (result != FMOD.RESULT.OK)
                return result;

            result = channel.setPaused(true);
            ERRCHECK(result, "channel.setPaused", false);

            if (result != FMOD.RESULT.OK)
                return result;

            if (this.OnPlaybackStopped != null)
                this.OnPlaybackStopped.Invoke(this.gameObjectName);

            return result;
        }

        public FMOD.RESULT PauseUserSound(FMOD.Channel channel, bool paused)
        {
            result = channel.setPaused(paused);
            ERRCHECK(result, "channel.setPaused", false);

            if (result == FMOD.RESULT.OK)
                if (this.OnPlaybackPaused != null)
                    this.OnPlaybackPaused.Invoke(this.gameObjectName);

            return result;
        }
        /// <summary>
        /// Stops and releases the sound associated with channel
        /// </summary>
        /// <param name="audioUri"></param>
        public void ReleaseUserSound(FMOD.Channel channel)
        {
            // prevent editor reloading related event/s
            if (!channel.hasHandle())
                return;

            if (this.outputdevice_system != null)
            {
                // this _will_ get called with valid handles despite the fact that system released running sounds in OnDestroy on the component and also e.g. in OnDestroy in the demo/test scene..
                result = this.outputdevice_system.ReleaseUserSound(channel, this.logLevel, this.gameObjectName, this.OnError);
                // ERRCHECK(result, "outputdevice_system.ReleaseUserSound", false);
            }
            else
            {
                result = FMOD.RESULT.OK;
            }

            if (result == FMOD.RESULT.OK)
            {
                this.channels.Remove(channel);

                if (this.OnPlaybackStopped != null)
                    this.OnPlaybackStopped.Invoke(this.gameObjectName);
            }
        }

        public bool IsSoundPaused(FMOD.Channel channel)
        {
            bool paused;
            result = channel.getPaused(out paused);
            // ERRCHECK(result, "channel.getPaused", false);

            if (result == FMOD.RESULT.OK)
                return paused;
            else
                return false;
        }

        public bool IsSoundPlaying(FMOD.Channel channel)
        {
            bool isPlaying;
            result = channel.isPlaying(out isPlaying);
            // ERRCHECK(result, "channel.isPlaying", false);

            bool paused = this.IsSoundPaused(channel);

            if (result == FMOD.RESULT.OK && !paused)
                return isPlaying;
            else
                return false;
        }
        public float GetVolume(FMOD.Channel channel)
        {
            float volume;
            result = channel.getVolume(out volume);
            // ERRCHECK(result, "channel.getVolume", false);

            if (result == FMOD.RESULT.OK)
                return volume;
            else
                return 0f;
        }
        public void SetVolume(FMOD.Channel channel, float volume)
        {
            result = channel.setVolume(volume);
            // ERRCHECK(result, "channel.setVolume", false);
        }

        public void SetPitch(FMOD.Channel channel, float pitch)
        {
            result = channel.setPitch(pitch);
            // ERRCHECK(result, "channel.setPitch", false);
        }
        public float GetPitch(FMOD.Channel channel)
        {
            float pitch;
            result = channel.getPitch(out pitch);
            // ERRCHECK(result, "channel.getPitch", false);

            if (result == FMOD.RESULT.OK)
                return pitch;
            else
                return 1f;
        }
        public void SetTimeSamples(FMOD.Channel channel, int timeSamples)
        {
            result = channel.setPosition((uint)timeSamples, FMOD.TIMEUNIT.PCM);
            // ERRCHECK(result, "channel.setPosition", false);
        }
        public int GetTimeSamples(FMOD.Channel channel)
        {
            uint timeSamples;
            result = channel.getPosition(out timeSamples, FMOD.TIMEUNIT.PCM);
            if (result == FMOD.RESULT.OK)
                return (int)timeSamples;
            else
                return -1;
        }

        public int GetLengthSamples(FMOD.Channel channel)
        {
            uint lengthSamples;
            FMOD.Sound sound;
            result = channel.getCurrentSound(out sound);
            if (result == FMOD.RESULT.OK)
            {
                result &= sound.getLength(out lengthSamples, FMOD.TIMEUNIT.PCM);
                if (result == FMOD.RESULT.OK)
                    return (int)lengthSamples;
                else return -1;
            }
            return -1;
        }
        #endregion
        // ========================================================================================================================================
        #region Channel mix matrix
        public FMOD.RESULT SetMixMatrix(FMOD.Channel channel
            , float[,] mixMatrix
            , int outchannels
            , int inchannels
            )
        {
            return this.outputdevice_system.SetMixMatrix(channel, mixMatrix, outchannels, inchannels);
        }

        public FMOD.RESULT GetMixMatrix(FMOD.Channel channel, out float[,] mixMatrix, out int outchannels, out int inchannels)
        {
            return this.outputdevice_system.GetMixMatrix(channel, out mixMatrix, out outchannels, out inchannels);
        }
        #endregion
    }
}
