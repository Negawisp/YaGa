using System.Runtime.InteropServices;

namespace YaGaPlugin
{
public partial class YaGa
{
    public static class Features
    {
        [DllImport("__Internal")]
        private static extern string YaGa_loadingAPIReady();

        private static bool _isReady;

        public static void LoadingAPIReady()
        {
#if UNITY_EDITOR
            if (!_isReady)
            {
                Console.Log("LoadingAPI Ready");
                _isReady = true;
            }
            else
                Console.Warn("Don't call <color=green>features.LoadingAPI.ready()</color> more than one time.");
#elif UNITY_WEBGL
            if (_isReady) return;
            YaGa_loadingAPIReady();
            _isReady = true;
#endif
        }
    }
}
}
