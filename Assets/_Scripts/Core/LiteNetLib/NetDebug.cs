using System;
using System.Diagnostics;

namespace LiteNetLib
{
    public enum NetLogLevel
    {
        Warning,
        Error,
        Trace,
        Info
    }
    public enum NetWorld
    {
        SERVER,
        CLIENT,
        ClientAndServer 
    }
    /// <summary>
    /// Interface to implement for your own logger
    /// </summary>
    public interface INetLogger
    {
        void WriteNet(NetLogLevel level, string str, params object[] args);
    }

    /// <summary>
    /// /// Статический класс для определения вашего собственного регистратора LiteNetLib вместо Console.WriteLine
        
        /// или Debug.Log, если скомпилирован с флагом UNITY
    /// </summary>
    public static class NetDebug
    {
        public static INetLogger Logger = null;
        private static readonly object DebugLogLock = new object();
        private static void WriteLogic(NetLogLevel logLevel, string str, params object[] args)
        {
            lock (DebugLogLock)
            {
                if (Logger == null)
                {
#if UNITY_4 || UNITY_5 || UNITY_5_3_OR_NEWER
                    UnityEngine.Debug.Log(string.Format(str, args));
#else
                    Console.WriteLine(str, args);
#endif
                }
                else
                {
                    Logger.WriteNet(logLevel, str, args);
                }
            }
        }
        private static void WriteLogic(NetWorld world,NetLogLevel logLevel, string str, params object[] args)
        {
            lock (DebugLogLock)
            {
                if (Logger == null)
                {
#if UNITY_4 || UNITY_5 || UNITY_5_3_OR_NEWER
                    UnityEngine.Debug.Log($"<color=cyan>[{world.ToString()}]</color> " + string.Format(str, args));
#else
                    Console.WriteLine(str, args);
#endif
                }
                else
                {
                    Logger.WriteNet(logLevel, str, args);
                }
            }
        }
        [Conditional("DEBUG_MESSAGES")]
        internal static void Write(string str, params object[] args)
        {
            WriteLogic(NetLogLevel.Trace, str, args);
        }

        [Conditional("DEBUG_MESSAGES")]
        internal static void Write(NetLogLevel level, string str, params object[] args)
        {
            WriteLogic(level, str, args);
        }

        [Conditional("DEBUG_MESSAGES"), Conditional("DEBUG")]
        internal static void WriteForce(string str, params object[] args)
        {
            WriteLogic(NetLogLevel.Trace, str, args);
        }
        [Conditional("DEBUG_MESSAGES"), Conditional("DEBUG")]
        public static void Log(NetWorld netWorld, string str, params object[] args)
        {
            WriteLogic(netWorld,NetLogLevel.Trace, str, args);
        }
        [Conditional("DEBUG_MESSAGES"), Conditional("DEBUG")]
        internal static void WriteForce(NetLogLevel level, string str, params object[] args)
        {
            WriteLogic(level, str, args);
        }

        internal static void WriteError(string str, params object[] args)
        {
            WriteLogic(NetLogLevel.Error, str, args);
        }
    }
}
