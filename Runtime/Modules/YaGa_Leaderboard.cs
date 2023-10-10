using System;
using System.Runtime.InteropServices;

namespace YaGaPlugin
{
public partial class YaGa
{
    /*#region JS Call Mapping

    private void OnGetLeaderboardScore(int score) => Leaderboard.OnGetLeaderboardScore(score);
    private void OnGetLeaderboardScoreFailed() => Leaderboard.OnGetLeaderboardScoreFailed();

    #endregion

    public static class Leaderboard
    {
        private static Action<int> _onGetLeaderboardScore;
        private static Action _onGetLeaderboardScoreFailed;
#if UNITY_EDITOR || !UNITY_WEBGL
        private static int _score = -1;
#endif

        internal static void OnGetLeaderboardScore(int score)
        {
            _onGetLeaderboardScore?.Invoke(score);
        }

        internal static void OnGetLeaderboardScoreFailed()
        {
            _onGetLeaderboardScoreFailed?.Invoke();
        }

        public static void GetLeaderboardScore(string lbName, Action<int> onGetScore = null, Action onFailed = null)
        {
#if UNITY_EDITOR || !UNITY_WEBGL
            if (_score != -1)
                onGetScore?.Invoke(_score);
            else
                onFailed?.Invoke();
#else
        _onGetLeaderboardScore = onGetScore;
        _onGetLeaderboardScoreFailed = onFailed;
        GetScoreFromLeaderboard(lbName);
#endif
        }

        public static void SetLeaderboardScore(string lbName, int score, string extraData = null)
        {
#if UNITY_EDITOR || !UNITY_WEBGL
            _score = score;
            print($"Leaderboard '{lbName}', score: {score}{(extraData != null ? $", extraData: '{extraData}'" : "")}");
#else
        SetScoreInLeaderboard(lbName, score, extraData);
#endif
        }
    }*/
}
}
