using System.Runtime.InteropServices;

namespace YaGaPlugin
{
public partial class YaGa
{
    public static class Metrika
    {
        [DllImport("__Internal")]
        private static extern void YaGa_ymReachGoal(string target);

        public static void ReachGoal(string target)
        {
#if UNITY_EDITOR
            Console.Log($"Metrika.ReachGoal: {target}");
#elif UNITY_WEBGL
            YaGa_ymReachGoal(target);
#endif
        }
    }
}
}
