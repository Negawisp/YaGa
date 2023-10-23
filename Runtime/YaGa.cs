using UnityEngine;

namespace YaGaPlugin
{
public partial class YaGa : MonoBehaviour
{
#if UNITY_EDITOR
    private const string JsYaGaName = "YaGa";
    private void OnValidate()
    {
        if (gameObject.name != JsYaGaName)
        {
            gameObject.name = JsYaGaName;
            UnityEditor.EditorUtility.SetDirty(this);
        }
        if (FindObjectsByType<YaGa>(FindObjectsInactive.Include, FindObjectsSortMode.None).Length > 1)
            Console.Warn("There is more than one YaGa component on the scene!");
    }
#endif
}
}
