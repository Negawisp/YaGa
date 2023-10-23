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
        static Audio() => Adv.AdStatusChanged += Set;

        private static bool _hasFocus;
        private static bool _isPaused;

        internal static bool HasFocus
        {
            set
            {
                _hasFocus = value;
                Set();
            }
        }

        internal static bool IsPaused
        {
            set
            {
                _isPaused = value;
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
