using System.IO;
using UnityEditor;
using UnityEngine;

namespace YaGaPlugin.Editor
{
public class WebGLTemplateImporter : MonoBehaviour
{
    private const string DirTemplateName = "YaGa";
    private const string DirPackageTemplate = "Packages/com.tools-unity.yaga/" + DirTemplateName + "~";
    private const string DirAssetsTemplate = "Assets/WebGLTemplates/" + DirTemplateName;
    private const string WebLogoFileName = "logo.png";

    [MenuItem("YaGa/Import Template")]
    public static void ImportTemplate()
    {
        if (Directory.Exists(DirAssetsTemplate))
        {
            if (Directory.GetFiles(DirAssetsTemplate).Length != 0 && !EditorUtility.DisplayDialog(
                    $"The {DirTemplateName} template already exists!",
                    "Are you sure you want to continue and replace the template?",
                    "Replace",
                    "Cancel"))
                return;
            var filesToDelete = Directory.GetFiles(DirAssetsTemplate, "*", SearchOption.AllDirectories);
            foreach (var file in filesToDelete)
                if (!file.EndsWith(WebLogoFileName))
                    File.Delete(file);
        }
        else
            Directory.CreateDirectory(DirAssetsTemplate);

        var filesToCopy = Directory.GetFiles(DirPackageTemplate, "*", SearchOption.AllDirectories);
        foreach (var file in filesToCopy)
        {
            var newFile = file.Replace(DirPackageTemplate, DirAssetsTemplate);
            if (!File.Exists(newFile))
                File.Copy(file, newFile);
        }

        AssetDatabase.Refresh();
    }
}
}
