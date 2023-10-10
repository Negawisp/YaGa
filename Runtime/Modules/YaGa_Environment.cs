using System;
using System.Runtime.InteropServices;
using UnityEngine;

namespace YaGaPlugin
{
public partial class YaGa
{
    [DllImport("__Internal")]
    private static extern string YaGa_getEnvironment();

    private static EnvironmentData _environment;
    public static EnvironmentData Environment =>
#if UNITY_EDITOR
        _environment ??= JsonUtility.FromJson<EnvironmentData>(@"{""app"":{""id"":""000001""},""browser"":{""lang"":""ru""},""i18n"":{""lang"":""ru"",""tld"":""ru""},""payload"":""someText""}");
#elif UNITY_WEBGL
        _environment ??= JsonUtility.FromJson<EnvironmentData>(YaGa_getEnvironment());
#endif
}

// ReSharper disable InconsistentNaming
public class EnvironmentData
{
    [Serializable]
    public struct _app
    {
        public string id;
    }

    [Serializable]
    public struct _browser
    {
        public string lang;
    }

    [Serializable]
    public struct _i18n
    {
        public string lang;
        public string tld;
    }

    public _app app;
    public _browser browser;
    public _i18n i18n;
    public string payload;
}
}
