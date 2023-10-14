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
        private static bool _isFullAdNow;
        private static bool _isRewardedAdNow;

        public static bool IsAdNow => _isFullAdNow || _isRewardedAdNow;

        internal static event Action<bool> AdStatusChanged;

        #region FULLSCREEN

        [DllImport("__Internal")]
        private static extern void YaGa_showFullscreenAdv();

        private static Action _full_onOpen;
        private static Action<bool> _full_onClose;
        private static Action _full_onError;
        private static Action _full_onOffline;

        public static void ShowFullscreenAdv(Action onOpen = null, Action<bool> onClose = null, Action onError = null,
            Action onOffline = null)
        {
            if (IsAdNow) return;

#if UNITY_EDITOR
            onOpen?.Invoke();
            onClose?.Invoke(true);
            onError?.Invoke();
            onOffline?.Invoke();
#elif UNITY_WEBGL
            _full_onOpen = onOpen;
            _full_onClose = onClose;
            _full_onError = onError;
            _full_onOffline = onOffline;
            YaGa_showFullscreenAdv();
#endif
        }

        internal static void Full_OnOpen()
        {
            AdStatusChanged?.Invoke(_isFullAdNow = true);
            _full_onOpen?.Invoke();
        }

        internal static void Full_OnClose(bool wasShown)
        {
            AdStatusChanged?.Invoke(_isFullAdNow = false);
            _full_onClose?.Invoke(wasShown);
        }

        internal static void Full_OnError()
        {
            _full_onError?.Invoke();
        }

        internal static void Full_OnOffline()
        {
            _full_onOffline?.Invoke();
        }

        #endregion

        #region REWARD

        [DllImport("__Internal")]
        private static extern void YaGa_showRewardedVideo();

        private static Action _reward_onOpen;
        private static Action _reward_onRewarded;
        private static Action _reward_onClose;
        private static Action _reward_onError;

        public static void ShowRewardedVideo(Action onOpen = null, Action onRewarded = null, Action onClose = null,
            Action onError = null)
        {
            if (IsAdNow) return;

#if UNITY_EDITOR || !UNITY_WEBGL
            onOpen?.Invoke();
            onRewarded?.Invoke();
            onClose?.Invoke();
            onError?.Invoke();
#else
            _reward_onOpen = onOpen;
            _reward_onRewarded = onRewarded;
            _reward_onClose = onClose;
            _reward_onError = onError;
            YaGa_showRewardedVideo();
#endif
        }

        internal static void Reward_OnOpen()
        {
            AdStatusChanged?.Invoke(_isRewardedAdNow = true);
            _reward_onOpen?.Invoke();
        }

        internal static void Reward_OnRewarded()
        {
            _reward_onRewarded?.Invoke();
        }

        internal static void Reward_OnClose()
        {
            AdStatusChanged?.Invoke(_isRewardedAdNow = false);
            _reward_onClose?.Invoke();
        }

        internal static void Reward_OnError()
        {
            _reward_onError?.Invoke();
        }

        #endregion
    }
}
}
