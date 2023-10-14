using System;
using System.Collections.Generic;
using System.Linq;
using System.Runtime.InteropServices;
using UnityEngine;

namespace YaGaPlugin
{
public partial class YaGa
{
    #region JS Call Mapping

    private void OnGetPlayerData(string dataString) => Player.OnGetData(dataString);
    private void OnGetPlayerStats(string dataString) => Player.OnGetStats(dataString);

    #endregion

#if UNITY_EDITOR
    [ContextMenu(nameof(ClearPrefsData))]
    private void ClearPrefsData()
    {
        PlayerPrefs.DeleteKey(Player.DataKey);
    }

    [ContextMenu(nameof(ClearPrefsStats))]
    private void ClearPrefsStats()
    {
        PlayerPrefs.DeleteKey(Player.StatsKey);
    }
#endif

    public static class Player
    {
        [DllImport("__Internal")]
        private static extern string YaGa_getCachedData();

        [DllImport("__Internal")]
        private static extern void YaGa_playerGetData();

        [DllImport("__Internal")]
        private static extern void YaGa_playerSetData(string data);

        [DllImport("__Internal")]
        private static extern void YaGa_playerGetStats();

        [DllImport("__Internal")]
        private static extern void YaGa_playerSetStats(string data);

        #region DATA

#if UNITY_EDITOR
        internal const string DataKey = "YaGa_data";
#endif
        private static Action<string> _onGetData;

        internal static void OnGetData(string data)
        {
            _onGetData?.Invoke(data);
        }

        public static T CachedData<T>()
        {
#if UNITY_EDITOR || !UNITY_WEBGL
            var d = PlayerPrefs.GetString(DataKey, default);
            Console.Log($"Cached data!\n{JsonUtility.ToJson(JsonUtility.FromJson<T>(d), true)}\n");
            return JsonUtility.FromJson<T>(d);
#else
            return JsonUtility.FromJson<T>(YaGa_getCachedData());
#endif
        }

        public static void GetData<T>(Action<T> onGetData)
        {
#if UNITY_EDITOR || !UNITY_WEBGL
            var d = PlayerPrefs.GetString(DataKey, default);
            Console.Log($"Data get!\n{JsonUtility.ToJson(JsonUtility.FromJson<T>(d), true)}\n");
            onGetData?.Invoke(JsonUtility.FromJson<T>(d));
#else
            _onGetData = s => onGetData?.Invoke(JsonUtility.FromJson<T>(s));
            YaGa_playerGetData();
#endif
        }

        public static void SetData(object objData)
        {
#if UNITY_EDITOR
            PlayerPrefs.SetString(DataKey, JsonUtility.ToJson(objData));
            Console.Log($"Data saved!\n{JsonUtility.ToJson(objData, true)}\n");
#elif UNITY_WEBGL
            YaGa_playerSetData(JsonUtility.ToJson(objData));
#endif
        }

        #endregion

        #region STATS

#if UNITY_EDITOR || !UNITY_WEBGL
        internal const string StatsKey = "YaGa_stats";
#endif
        private static Action<Dictionary<string, float>> _onGetStats;
        public static Dictionary<string, float> CachedStats = new();

        private static Dictionary<string, float> Data2Stats(string data) => !string.IsNullOrEmpty(data)
            ? System.Text.RegularExpressions.Regex.Matches(data, @"(?:[""'])([^""']*)(?:[""':\s]*)([0-9-.]*)")
                .ToDictionary(m => m.Groups[1].Value, m => float.Parse(m.Groups[2].Value))
            : new Dictionary<string, float>();

        private static string Stats2Data(Dictionary<string, float> stats) =>
            '{' + stats.Aggregate("", (current, stat) => current + $"\"{stat.Key}\":{stat.Value},")[..^1] + '}';

        internal static void OnGetStats(string data)
        {
            CachedStats = Data2Stats(data);
            _onGetStats?.Invoke(CachedStats);
        }

        public static void GetStats(Action<Dictionary<string, float>> onGetStats)
        {
#if UNITY_EDITOR || !UNITY_WEBGL
            var data = PlayerPrefs.GetString(StatsKey, default);
            onGetStats?.Invoke(CachedStats = Data2Stats(data));
#else
            _onGetStats = onGetStats;
            YaGa_playerGetStats();
#endif
        }

        public static void SetStats(Dictionary<string, float> data)
        {
#if UNITY_EDITOR || !UNITY_WEBGL
            var stats = Stats2Data(data);
            PlayerPrefs.SetString(StatsKey, stats);
            Console.Log($"Stats saved!\n{stats}\n");
#else
            YaGa_playerSetStats(Stats2Data(data));
#endif
        }

        #endregion
    }
}
}
