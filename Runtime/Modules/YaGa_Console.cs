using System.Runtime.InteropServices;

namespace YaGaPlugin
{
public partial class YaGa
{
    public static class Console
    {
        [DllImport("__Internal")]
        private static extern void YaGa_consoleLog(string logMessage, int logType);

#if UNITY_EDITOR
        private const string LogSymbol = "<color=orange>❂</color> ";
#endif

        public static void Log(object o)
        {
#if UNITY_EDITOR
            UnityEngine.Debug.Log(LogSymbol + o);
#elif UNITY_WEBGL
            YaGa_consoleLog(o.ToString(), 0);
#endif
        }

        public static void Info(object o)
        {
#if UNITY_EDITOR
            UnityEngine.Debug.Log(LogSymbol + "<color=cyan>ⓘ</color> " + o);
#elif UNITY_WEBGL
            YaGa_consoleLog(o.ToString(), 1);
#endif
        }

        public static void Warn(object o)
        {
#if UNITY_EDITOR
            UnityEngine.Debug.LogWarning(LogSymbol + o);
#elif UNITY_WEBGL
            YaGa_consoleLog(o.ToString(), 2);
#endif
        }

        public static void Error(object o)
        {
#if UNITY_EDITOR
            UnityEngine.Debug.LogError(LogSymbol + o);
#elif UNITY_WEBGL
            YaGa_consoleLog(o.ToString(), 3);
#endif
        }
    }
}
}
