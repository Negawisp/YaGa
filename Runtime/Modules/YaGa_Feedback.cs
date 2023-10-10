using System;
using System.Runtime.InteropServices;

namespace YaGaPlugin
{
public partial class YaGa
{
    #region JS Call Mapping

    private void OnCanReviewFailed(string reason) => Feedback.OnCanReviewFailed(reason);
    private void OnRequestReview(bool feedbackSent) => Feedback.OnRequestReview(feedbackSent);

    #endregion

    public static class Feedback
    {
        [DllImport("__Internal")]
        private static extern void YaGa_requestReview();

        private static Action<ReviewReason> _onCanReviewFailed;
        private static Action<bool> _onRequestReview;

        public enum ReviewReason
        {
            /// <summary>The user isn't logged in.</summary>
            NO_AUTH,
            /// <summary>The user already rated the game.</summary>
            GAME_RATED,
            /// <summary>A request was sent, now awaiting the user's action.</summary>
            REVIEW_ALREADY_REQUESTED,
            /// <summary>A request was sent and the user performed an action (gave a rating or closed the pop-up window).</summary>
            REVIEW_WAS_REQUESTED,
            /// <summary>A request wasn't sent because an error occurred on the Yandex side.</summary>
            UNKNOWN
        }

        internal static void OnCanReviewFailed(string reason)
        {
            _onCanReviewFailed?.Invoke(Enum.Parse<ReviewReason>(reason));
        }

        internal static void OnRequestReview(bool feedbackSent)
        {
            _onRequestReview?.Invoke(feedbackSent);
        }

        public static void RequestReview(Action<bool> onRequestReview = null, Action<ReviewReason> onCanReviewFailed = null)
        {
#if UNITY_EDITOR || !UNITY_WEBGL
            Console.Log("Rating requested");
            onRequestReview?.Invoke(true);
#else
            _onRequestReview = onRequestReview;
            _onCanReviewFailed = onCanReviewFailed;
            YaGa_requestReview();
#endif
        }
    }
}
}
