// (c) 2016-2021 Martin Cvengros. All rights reserved. Redistribution of source code without permission not allowed.
// uses FMOD by Firelight Technologies Pty Ltd

namespace AudioStream
{
    /// <summary>
    /// System with simple defaults for input devices enumeration
    /// </summary>
    public class FMODSystemInputDevice
    {
        public readonly FMOD.System system;
        public readonly string VersionString;
        /// <summary>
        /// FMOD's sytem handle (contrary to sound handle it seems) is completely unreliable / e.g. clearing it via .clearHandle() has no effect in following check for !null/hasHandle() /
        /// Use this pointer copied after creation as release/functionality guard instead
        /// </summary>
        public System.IntPtr SystemHandle = global::System.IntPtr.Zero;
        FMOD.RESULT result = FMOD.RESULT.OK;

        public FMODSystemInputDevice(
            uint dspBufferLength_Custom
            , uint dspBufferCount_Custom
            , LogLevel logLevel
            , string gameObjectName
            , EventWithStringStringParameter onError
            , out uint dspBufferLength_Auto
            , out uint dspBufferCount_Auto
            )
        {
            /*
            Create a System object and initialize.
            */
            uint version = 0;

            result = FMOD.Factory.System_Create(out this.system);
            AudioStreamSupport.ERRCHECK(result, logLevel, gameObjectName, onError, "Factory.System_Create");

            result = system.getVersion(out version);
            AudioStreamSupport.ERRCHECK(result, logLevel, gameObjectName, onError, "system.getVersion");

            if (version < FMOD.VERSION.number)
            {
                var msg = string.Format("FMOD lib version {0} doesn't match header version {1}", version, FMOD.VERSION.number);
                AudioStreamSupport.LOG(LogLevel.ERROR, logLevel, gameObjectName, onError, msg);

                if (onError != null)
                    onError.Invoke(gameObjectName, msg);

                dspBufferLength_Auto =
                    dspBufferCount_Auto =
                    0;

                return;
            }

            /*
                FMOD version number: 0xaaaabbcc -> aaaa = major version number.  bb = minor version number.  cc = development version number.
            */
            var versionString = System.Convert.ToString(version, 16).PadLeft(8, '0');
            this.VersionString = string.Format("{0}.{1}.{2}", System.Convert.ToUInt32(versionString.Substring(0, 4)), versionString.Substring(4, 2), versionString.Substring(6, 2));

            /*
             * Adjust DSP buffer of the system if requested by user
             * This function cannot be called after FMOD is already activated with System::init.
             * It must be called before System::init, or after System::close.
             */

            if (dspBufferLength_Custom > 0
                && dspBufferCount_Custom > 0)
            {
                AudioStreamSupport.LOG(LogLevel.INFO, logLevel, gameObjectName, onError, "Setting custom FMOD DSP buffer: {0} length, {1} buffers", dspBufferLength_Custom, dspBufferCount_Custom);

                result = system.setDSPBufferSize(dspBufferLength_Custom, (int)dspBufferCount_Custom);
                AudioStreamSupport.ERRCHECK(result, logLevel, gameObjectName, onError, "recording_system.setDSPBufferSize");
            }


#if UNITY_ANDROID && !UNITY_EDITOR
            // For recording to work on Android OpenSL support is needed:
            // https://www.fmod.org/questions/question/is-input-recording-supported-on-android/

            result = system.setOutput(FMOD.OUTPUTTYPE.OPENSL);
            AudioStreamSupport.ERRCHECK(result, logLevel, gameObjectName, onError, "system.setOutput", false);

            if (result != FMOD.RESULT.OK)
            {
                var msg = "OpenSL support needed for recording not available.";

                AudioStreamSupport.LOG(LogLevel.ERROR, logLevel, gameObjectName, onError, msg);

                if (onError != null)
                    onError.Invoke(gameObjectName, msg);

                dspBufferLength_Auto =
                    dspBufferCount_Auto =
                    0;

                return;
            }
#endif
            /*
            System initialization
            */
            result = system.init(4093, FMOD.INITFLAGS.NORMAL, System.IntPtr.Zero);
            AudioStreamSupport.ERRCHECK(result, logLevel, gameObjectName, onError, "system.init");

            // retrieve & log effective DSP used
            uint bufferLength;
            int numBuffers;

            result = system.getDSPBufferSize(out bufferLength, out numBuffers);
            AudioStreamSupport.ERRCHECK(result, logLevel, gameObjectName, onError, "system.getDSPBufferSize");

            dspBufferLength_Auto = bufferLength;
            dspBufferCount_Auto = (uint)numBuffers;

            AudioStreamSupport.LOG(LogLevel.INFO, logLevel, gameObjectName, onError, "Effective FMOD DSP buffer: {0} length, {1} buffers", bufferLength, numBuffers);

            this.SystemHandle = this.system.handle;
        }

        // !
        // TODO: move this all into manager partial class to avoid public exposure

        /// <summary>
        /// Close and release for system
        /// </summary>
        public void Release(LogLevel logLevel
            , string gameObjectName
            , EventWithStringStringParameter onError
            )
        {
            if (this.SystemHandle != global::System.IntPtr.Zero)
            {
                result = system.close();
                AudioStreamSupport.ERRCHECK(result, logLevel, gameObjectName, onError, "System.close");

                result = system.release();
                AudioStreamSupport.ERRCHECK(result, logLevel, gameObjectName, onError, "System.release");

                system.clearHandle();
                // Debug.Log(System.handle);

                this.SystemHandle = global::System.IntPtr.Zero;
            }
        }
        /// <summary>
        /// Called continuosly (i.e. Update)
        /// </summary>
        public FMOD.RESULT Update()
        {
            if (this.SystemHandle != global::System.IntPtr.Zero)
            {
                return this.system.update();
            }
            else
            {
                return FMOD.RESULT.ERR_INVALID_HANDLE;
            }
        }
    }
}