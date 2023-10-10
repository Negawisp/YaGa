using UnityEngine;

namespace YaGaPlugin
{
public partial class YaGa
{
    private void OnApplicationFocus(bool hasFocus) =>
        Audio.HasFocus = hasFocus;

    private void OnApplicationPause(bool isPaused) =>
        Audio.IsPaused = isPaused;

    private static class Audio
    {
        static Audio()
        {
            Adv.AdStatusChanged += _ => Set();
        }

        private static bool _isPaused;
        private static bool _hasFocus;

        internal static bool IsPaused
        {
            set
            {
                _isPaused = value;
                Set();
            }
        }

        internal static bool HasFocus
        {
            set
            {
                _hasFocus = value;
                Set();
            }
        }

        private static void Set()
        {
            var isOn = !_isPaused && _hasFocus && !Adv.IsAdNow;
            AudioListener.pause = !isOn;
            AudioListener.volume = isOn ? 1 : 0;
        }
    }
}
}
