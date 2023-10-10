using System.Runtime.InteropServices;

namespace YaGaPlugin
{
public partial class YaGa
{
    public static class DeviceInfo
    {
        [DllImport("__Internal")]
        private static extern string YaGa_getDeviceType();

        [DllImport("__Internal")]
        private static extern string YaGa_getMobileType();

        public enum DeviceType
        {
            Mobile,
            Tablet,
            Desktop,
            TV
        }

        public enum MobileType
        {
            None,
            Android,
            iOS
        }

        private static DeviceType _type = (DeviceType)(-1);

        public static DeviceType Type
        {
            get
            {
                if ((int)_type == -1)
                {
#if UNITY_EDITOR
                    _type = 0;
#elif UNITY_WEBGL
                    _type = System.Enum.Parse<DeviceType>(YaGa_getDeviceType(), true);
#endif
                }
                return _type;
            }
        }

        private static MobileType _mobile = (MobileType)(-1);

        public static MobileType Mobile
        {
            get
            {
                if ((int)_mobile == -1)
#if UNITY_EDITOR
                    _mobile = 0;
#elif UNITY_WEBGL
                    _mobile = System.Enum.Parse<MobileType>(YaGa_getMobileType(), true);
#endif
                return _mobile;
            }
        }
    }
}
}
