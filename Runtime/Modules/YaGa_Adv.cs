using System;
using System.Runtime.InteropServices;

namespace YaGaPlugin
{
public partial class YaGa
{
    #region JS Call Mapping

    private void Full_OnOpen() => Adv.Full_OnOpen();
    private void Full_OnClose(int wasShown) => Adv.Full_OnClose(wasShown == 1);
    private void Full_OnError() => Adv.Full_OnError();
    private void Full_OnOffline() => Adv.Full_OnOffline();

    private void Reward_OnOpen() => Adv.Reward_OnOpen();
    private void Reward_OnRewarded() => Adv.Reward_OnRewarded();
    private void Reward_OnClose() => Adv.Reward_OnClose();
    private void Reward_OnError() => Adv.Reward_OnError();

    #endregion

    public static class Adv
    {
        private static bool IsFullAdNow => _fullCallbacks != null;
        private static bool IsRewardedAdNow => _rewCallbacks != null;

        public static bool IsAdNow => IsFullAdNow || IsRewardedAdNow;

        internal static event Action AdStatusChanged;

        #region FULLSCREEN

        [DllImport("__Internal")]
        private static extern void YaGa_showFullscreenAdv();

        private static (Action onOpen, Action<bool> onClose, Action onError, Action onOffline)? _fullCallbacks;

        public static void ShowFullscreenAdv(Action onOpen = null, Action<bool> onClose = null, Action onError = null,
            Action onOffline = null)
        {
            if (IsAdNow) return;

            _fullCallbacks = (onOpen, onClose, onError, onOffline);
#if UNITY_EDITOR
            Full_OnOpen();
            Full_OnClose(true);
#elif UNITY_WEBGL
            YaGa_showFullscreenAdv();
#endif
        }

        internal static void Full_OnOpen()
        {
            AdStatusChanged?.Invoke();
            _fullCallbacks?.onOpen?.Invoke();
        }

        internal static void Full_OnClose(bool wasShown)
        {
            var onClose = _fullCallbacks?.onClose;
            _fullCallbacks = null;
            AdStatusChanged?.Invoke();
            onClose?.Invoke(wasShown);
        }

        internal static void Full_OnError()
        {
            _fullCallbacks?.onError?.Invoke();
        }

        internal static void Full_OnOffline()
        {
            _fullCallbacks?.onOffline?.Invoke();
        }

        #endregion

        #region REWARD

        [DllImport("__Internal")]
        private static extern void YaGa_showRewardedVideo();

        private static (Action onOpen, Action onRewarded, Action onClose, Action onError)? _rewCallbacks;

        public static void ShowRewardedVideo(Action onOpen = null, Action onRewarded = null, Action onClose = null,
            Action onError = null)
        {
            if (IsAdNow) return;

            _rewCallbacks = (onOpen, onRewarded, onClose, onError);
#if UNITY_EDITOR
            Reward_OnOpen();
            Reward_OnRewarded();
            Reward_OnClose();
#else
            YaGa_showRewardedVideo();
#endif
        }

        internal static void Reward_OnOpen()
        {
            AdStatusChanged?.Invoke();
            _rewCallbacks?.onOpen?.Invoke();
        }

        internal static void Reward_OnRewarded()
        {
            _rewCallbacks?.onRewarded?.Invoke();
        }

        internal static void Reward_OnClose()
        {
            var onClose = _rewCallbacks?.onClose;
            _rewCallbacks = null;
            AdStatusChanged?.Invoke();
            onClose?.Invoke();
        }

        internal static void Reward_OnError()
        {
            _rewCallbacks?.onError?.Invoke();
        }

        #endregion
    }
}
}
