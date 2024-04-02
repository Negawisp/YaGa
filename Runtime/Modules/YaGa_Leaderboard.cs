using System;
using System.Runtime.InteropServices;
using UnityEngine;

namespace YaGaPlugin
{
public partial class YaGa
{
    #region JS Call Mapping

    private void OnGetDescription(string lbData) => Leaderboard.OnGetDescription(lbData);
    private void OnGetPlayerEntry(string playerEntry) => Leaderboard.OnGetPlayerEntry(playerEntry);
    private void OnPlayerNotPresentError() => Leaderboard.OnPlayerNotPresentError();

    #endregion

    public static class Leaderboard
    {
        [DllImport("__Internal")]
        private static extern string YaGa_getLeaderboardDescription(string lbName);

        [DllImport("__Internal")]
        private static extern string YaGa_getLeaderboardPlayerEntry(string lbName, string avatarSize);

        [DllImport("__Internal")]
        private static extern string YaGa_setLeaderboardScore(string lbName, int score, string extraData);

        #region GetDescription

        private static Action<LeaderboardData> _onGetDescription;

        internal static void OnGetDescription(string lbData)
        {
            _onGetDescription?.Invoke(JsonUtility.FromJson<LeaderboardData>(lbData));
        }

        public static void GetDescription(string lbName, Action<LeaderboardData> onGet = null)
        {
            _onGetDescription = onGet;
#if UNITY_EDITOR
            OnGetDescription("");
#elif UNITY_WEBGL
            YaGa_getLeaderboardDescription(lbName);
#endif
        }

        #endregion

        #region GetPlayerEntry

        private static Action<PlayerEntry> _onGetPlayerEntry;
        private static Action _onPlayerNotPresentError;
#if UNITY_EDITOR || !UNITY_WEBGL
        private static readonly System.Collections.Generic.Dictionary<string, string> _cachedPlayerEntry = new();
#endif

        internal static void OnGetPlayerEntry(string playerEntry)
        {
            _onGetPlayerEntry?.Invoke(JsonUtility.FromJson<PlayerEntry>(playerEntry));
        }

        internal static void OnPlayerNotPresentError()
        {
            _onPlayerNotPresentError?.Invoke();
        }

        public static void GetPlayerEntry(string lbName, Player.AvatarSize avatarSize = Player.AvatarSize.none,
            Action<PlayerEntry> onGet = null, Action onNotPresentError = null)
        {
            _onGetPlayerEntry = onGet;
            _onPlayerNotPresentError = onNotPresentError;
#if UNITY_EDITOR
            if (_cachedPlayerEntry.TryGetValue(lbName, out var playerEntry))
                OnGetPlayerEntry(playerEntry);
            else
                OnPlayerNotPresentError();
#elif UNITY_WEBGL
            YaGa_getLeaderboardPlayerEntry(lbName, avatarSize.ToString());
#endif
        }

        #endregion

        public static void SetScore(string lbName, int score, string extraData = null)
        {
#if UNITY_EDITOR
            _cachedPlayerEntry[lbName] = JsonUtility.ToJson(new PlayerEntry { score = score, extraData = extraData });
            Console.Log($"Leaderboard '{lbName}', score: {score}{(extraData != null ? $", extraData: '{extraData}'" : "")}");
#elif UNITY_WEBGL
            YaGa_setLeaderboardScore(lbName, score, extraData);
#endif
        }
    }
}

// ReSharper disable InconsistentNaming
public class PlayerEntry
{
    [Serializable]
    public class _player
    {
        public string publicName;
        public string uniqueID;
        public string avatarURL;

#if PACKAGE_WEB_REQUEST_TEXTURE
        private Texture _avatar;
        public void GetAvatar(Action<Texture> onGet)
        {
            if (_avatar)
            {
                onGet?.Invoke(_avatar);
                return;
            }

            var request = UnityEngine.Networking.UnityWebRequestTexture.GetTexture(avatarURL);
            request.SendWebRequest().completed += _ =>
            {
                if (request.result != UnityEngine.Networking.UnityWebRequest.Result.Success)
                    YaGa.Console.Error(request.error);
                else
                {
                    var handlerTexture = request.downloadHandler as UnityEngine.Networking.DownloadHandlerTexture;

                    if (handlerTexture!.isDone)
                    {
                        _avatar = handlerTexture.texture;
                        onGet?.Invoke(_avatar);
                    }
                }
            };
        }
#endif
    }

    public int score;
    public string extraData;
    public int rank;
    public _player player;
    // public string formattedScore;
}

public class LeaderboardData
{
    public enum LeaderboardType
    {
        numeric = 0,
        time = 1
    }

    [Serializable]
    public class _title {
        public string ru;
        public string en;
        public string be;
        public string uk;
        public string kk;
        public string uz;
    }

    // public string appID;
    public bool isDefault;
    public bool isInvertSortOrder;
    public int decimalOffset;
    public LeaderboardType type;
    public string name;
    public _title title;
}
}
