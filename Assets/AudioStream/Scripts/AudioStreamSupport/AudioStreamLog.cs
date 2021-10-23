// (c) 2016-2021 Martin Cvengros. All rights reserved. Redistribution of source code without permission not allowed.
// uses FMOD by Firelight Technologies Pty Ltd

using System;
using UnityEngine;

namespace AudioStream
{
    // ========================================================================================================================================
    #region just LogLevel enum
    public enum LogLevel
    {
        ERROR = 0
            , WARNING = 1 << 0
            , INFO = 1 << 1
            , DEBUG = 1 << 2
    }
    #endregion

    public static partial class AudioStreamSupport
    {
        // ========================================================================================================================================
        #region Logging
        /// <summary>
        /// Checks FMOD result and either throws an exception with error message, or logs error message
        /// Log requires game object's current log level, name and error event handler
        /// TODO: !thread safe because of event handler
        /// </summary>
        /// <param name="result"></param>
        /// <param name="currentLogLevel"></param>
        /// <param name="gameObjectName"></param>
        /// <param name="onError"></param>
        /// <param name="customMessage"></param>
        /// <param name="throwOnError"></param>
        public static void ERRCHECK(
            FMOD.RESULT result
            , LogLevel currentLogLevel
            , string gameObjectName
            , EventWithStringStringParameter onError
            , string customMessage
            , bool throwOnError = true
            )
        {
            if (result != FMOD.RESULT.OK)
            {
                var m = string.Format("{0} {1} - {2}", customMessage, result, FMOD.Error.String(result));

                if (throwOnError)
                    throw new System.Exception(m);
                else
                    LOG(LogLevel.ERROR, currentLogLevel, gameObjectName, onError, m);
            }
            else
            {
                LOG(LogLevel.DEBUG, currentLogLevel, gameObjectName, onError, "{0} {1} - {2}", customMessage, result, FMOD.Error.String(result));
            }
        }
        /// <summary>
        /// Logs message based on log level and invokes error handler (for calling from ERRCHECK)
        /// TODO: !thread safe because of event handler
        /// </summary>
        /// <param name="requestedLogLevel"></param>
        /// <param name="currentLogLevel"></param>
        /// <param name="gameObjectName"></param>
        /// <param name="onError"></param>
        /// <param name="format"></param>
        /// <param name="args"></param>
        public static void LOG(
            LogLevel requestedLogLevel
            , LogLevel currentLogLevel
            , string gameObjectName
            , EventWithStringStringParameter onError
            , string format
            , params object[] args
            )
        {
            if (requestedLogLevel == LogLevel.ERROR)
            {
                var time = DateTime.Now.ToString("s");
                var msg = string.Format(format, args);

                Debug.LogError(
                    gameObjectName + " [ERROR][" + time + "] " + msg + "\r\n=======================================\r\n"
                    );

                if (onError != null)
                    onError.Invoke(gameObjectName, msg);
            }
            else if (currentLogLevel >= requestedLogLevel)
            {
                var time = DateTime.Now.ToString("s");

                if (requestedLogLevel == LogLevel.WARNING)
                    Debug.LogWarningFormat(
                        gameObjectName + " [WARNING][" + time + "] " + format + "\r\n=======================================\r\n"
                        , args);
                else
                    Debug.LogFormat(
                        gameObjectName + " [" + currentLogLevel + "][" + time + "] " + format + "\r\n=======================================\r\n"
                        , args);
            }
        }

        public static string TimeStringFromSeconds(double seconds)
        {
            // There are 10,000 ticks in a millisecond:
            var ticks = seconds * 1000 * 10000;
            var span = new TimeSpan((long)ticks);

            return string.Format("{0:D2}h : {1:D2}m : {2:D2}s : {3:D3}ms"
                , span.Hours
                , span.Minutes
                , span.Seconds
                , span.Milliseconds
                );
        }
        #endregion
    }
}