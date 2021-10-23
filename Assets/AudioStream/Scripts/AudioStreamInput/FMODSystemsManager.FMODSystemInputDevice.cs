// (c) 2016-2021 Martin Cvengros. All rights reserved. Redistribution of source code without permission not allowed.
// uses FMOD by Firelight Technologies Pty Ltd

using System.Collections.Generic;
using UnityEngine;

namespace AudioStream
{
    /// <summary>
    /// Manages FMOD system for general input enumeration and inputs changes notifications
    /// </summary>
    public static partial class FMODSystemsManager
    {
        static FMODSystemInputDevice system_input = null;
        static int system_input_refCount = 0;
        public static FMODSystemInputDevice FMODSystemInputDevice_Create(
            uint dspBufferLength_Custom
            , uint dspBufferCount_Custom
            , LogLevel logLevel
            , string gameObjectName
            , EventWithStringStringParameter onError
            , out uint dspBufferLength_Auto
            , out uint dspBufferCount_Auto
            )
        {
            FMODSystemsManager.system_input_refCount++;

            if (FMODSystemsManager.system_input == null)
            {
                FMODSystemsManager.system_input = new FMODSystemInputDevice(dspBufferLength_Custom, dspBufferCount_Custom, logLevel, gameObjectName, onError, out dspBufferLength_Auto, out dspBufferCount_Auto);
                AudioStreamSupport.LOG(LogLevel.INFO, logLevel, gameObjectName, onError, "Created FMOD system for input monitoring/enumeration {0}", FMODSystemsManager.system_input.system.handle);
            }
            else
            {
                // this is pointless but fill output values at least with correct ones..
                uint bufferLength;
                int numBuffers;
                FMODSystemsManager.system_input.system.getDSPBufferSize(out bufferLength, out numBuffers);

                dspBufferLength_Auto = bufferLength;
                dspBufferCount_Auto = (uint)numBuffers;
            }

            return FMODSystemsManager.system_input;
        }
        /// <summary>
        /// Manual system release
        /// </summary>
        /// <param name="fmodsystem"></param>
        /// <param name="logLevel"></param>
        /// <param name="gameObjectName"></param>
        /// <param name="onError"></param>
        public static void FMODSystemInputDevice_Release(FMODSystemInputDevice fmodsystem
            , LogLevel logLevel
            , string gameObjectName
            , EventWithStringStringParameter onError
            )
        {
            if (fmodsystem != FMODSystemsManager.system_input)
                Debug.LogErrorFormat("System being released was not previously created via FMODSystemsManager");

            if (FMODSystemsManager.system_input_refCount < 1)
                Debug.LogWarningFormat("System is being overreleased");

            if (--FMODSystemsManager.system_input_refCount < 1)
            {
                // although 'overreleasing' fmod call is harmless, the system has to be around until last release is requested
                // parameter _can_ be null though
                if (fmodsystem != null)
                {
                    fmodsystem.Release(logLevel, gameObjectName, onError);
                    AudioStreamSupport.LOG(LogLevel.INFO, logLevel, gameObjectName, onError, "Released system for input monitoring/enumeration {0}", fmodsystem.system.handle);
                }

                fmodsystem = FMODSystemsManager.system_input = null;
            }
        }

        // ========================================================================================================================================
        #region enumeration/support
        public struct INPUT_DEVICE
        {
            public int id;
            public string name;
            public System.Guid guid;
            public int samplerate;
            public FMOD.SPEAKERMODE speakermode;
            public int channels;
        }
        /// <summary>
        /// Enumerates available audio inputs in the system and returns their names and record device ids
        /// </summary>
        /// <param name="includeLoopbackInterfaces">If false should return 'common' user accessible inputs and be equal to what Unity's Microphone returns in that case. Default true.</param>
        /// <returns></returns>
        public static List<INPUT_DEVICE> AvailableInputs(LogLevel logLevel
            , string gameObjectName
            , EventWithStringStringParameter onError
            , bool includeLoopbackInterfaces = true)
        {
            // re/use enumeration/notification system
            // (make sure to not throw an exception anywhere so the system is always released)
            uint dspBufferLength_Auto;
            uint dspBufferCount_Auto;
            var fmodsystem = FMODSystemsManager.FMODSystemInputDevice_Create(0, 0, logLevel, gameObjectName, onError, out dspBufferLength_Auto, out dspBufferCount_Auto);

            var result = fmodsystem.Update();
            AudioStreamSupport.ERRCHECK(result, LogLevel.ERROR, "FMODSystemsManager.FMODSystemInputDevice", null, "fmodsystem.Update", false);

            List<INPUT_DEVICE> availableDriversNames = new List<INPUT_DEVICE>();

            /*
            Enumerate record devices
            */
            int numAllDrivers = 0;
            int numConnectedDrivers = 0;
            result = fmodsystem.system.getRecordNumDrivers(out numAllDrivers, out numConnectedDrivers);
            AudioStreamSupport.ERRCHECK(result, LogLevel.ERROR, "FMODSystemsManager.FMODSystemInputDevice", null, "fmodsystem.system.getRecordNumDrivers", false);

            for (int i = 0; i < numConnectedDrivers; ++i)
            {
                int recChannels;
                int recRate;
                int namelen = 255;
                string name;
                System.Guid guid;
                FMOD.SPEAKERMODE speakermode;
                FMOD.DRIVER_STATE driverstate;
                result = fmodsystem.system.getRecordDriverInfo(i, out name, namelen, out guid, out recRate, out speakermode, out recChannels, out driverstate);
                AudioStreamSupport.ERRCHECK(result, LogLevel.ERROR, "FMODSystemsManager.FMODSystemInputDevice", null, "fmodsystem.system.getRecordDriverInfo", false);

                if (result != FMOD.RESULT.OK)
                {
                    AudioStreamSupport.LOG(LogLevel.ERROR, logLevel, gameObjectName, onError, "!error input {0} guid: {1} systemrate: {2} speaker mode: {3} channels: {4} state: {5}"
                        , name
                        , guid
                        , recRate
                        , speakermode
                        , recChannels
                        , driverstate
                        );

                    continue;
                }

                // hardcoded string added by FMOD to the adapter name
                var isLoopback = name.ToLower().EndsWith("[loopback]");
                var addInterface = includeLoopbackInterfaces ? true : !isLoopback;

                if (addInterface)
                {
                    availableDriversNames.Add(new INPUT_DEVICE() { id = i, name = name, guid = guid, samplerate = recRate, speakermode = speakermode, channels = recChannels });
                }

                AudioStreamSupport.LOG(LogLevel.INFO, logLevel, gameObjectName, onError, "{0} guid: {1} systemrate: {2} speaker mode: {3} channels: {4} state: {5} - {6}"
                    , name
                    , guid
                    , recRate
                    , speakermode
                    , recChannels
                    , driverstate
                    , addInterface ? "ADDED" : "SKIPPED - IS LOOPBACK"
                    );
            }

            // release
            FMODSystemsManager.FMODSystemInputDevice_Release(fmodsystem, logLevel, gameObjectName, onError);

            return availableDriversNames;
        }
        #endregion
    }
}