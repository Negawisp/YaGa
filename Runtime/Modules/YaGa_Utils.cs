using System.Runtime.InteropServices;
using UnityEngine;

namespace YaGaPlugin
{
public partial class YaGa
{
    public static class Utils
    {
        #region URL

        [DllImport("__Internal")]
        private static extern string YaGa_getAddress();

        private static string _address;

        /// <summary>
        /// Opens a link to the Yandex website with the current domain
        /// </summary>
        /// <param name="yandexSectionUrl">Everything that comes after the "games" section <i>(starting without the <b>/</b> sign)</i></param>
        /// <example>"yandex.com/games/tr/" + yandexSectionUrl</example>
        public static void OpenURL(string yandexSectionUrl)
        {
#if UNITY_EDITOR || !UNITY_WEBGL
            _address ??= "https://yandex.ru/games/";
#else
            _address ??= YaGa_getAddress().Split("app")[0];
#endif
            Application.OpenURL(_address + yandexSectionUrl);
        }

        #endregion

    }
}
}
