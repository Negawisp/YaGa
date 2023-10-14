using System;
using System.IO;
using System.IO.Compression;
using UnityEditor;
using UnityEditor.Build;
using UnityEditor.Build.Reporting;
using UnityEngine.SceneManagement;

namespace YaGaPlugin.Editor
{
public class BuildProcessing : IPreprocessBuildWithReport, IPostprocessBuildWithReport
{
    int IOrderedCallback.callbackOrder { get; }

    void IPreprocessBuildWithReport.OnPreprocessBuild(BuildReport report)
    {
        CheckForYaGaComponent();

        var indexHtmlPath = report.summary.outputPath + "/index.html";
        if (File.Exists(indexHtmlPath))
            File.Delete(indexHtmlPath);
    }

    void IPostprocessBuildWithReport.OnPostprocessBuild(BuildReport report)
    {
        var buildPath = report.summary.outputPath;
        ZipFile.CreateFromDirectory(buildPath, $"{buildPath}_{DateTime.Now:yy.MM.dd.HH.mm}.zip");
    }

    private void CheckForYaGaComponent()
    {
        YaGa y = null;
        for (var i = 0; i < SceneManager.sceneCountInBuildSettings; i++)
        {
            var s = SceneManager.GetSceneAt(i);
            var gameObjects = s.GetRootGameObjects();
            foreach (var go in gameObjects)
            {
                var c = go.GetComponentInChildren<YaGa>(true);
                if (c == null) continue;
                y = c;
                break;
            }
        }

        if (y == null)
        {
            if (!EditorUtility.DisplayDialog(
                    "❂ YaGa WARNING!",
                    "There is no YaGa component in scenes!\n\nAre you sure you want to continue?",
                    "Continue",
                    "Cancel"))
                throw new BuildFailedException("The YaGa component is missing!");
        }
        else
        {
            if (!y.isActiveAndEnabled && !EditorUtility.DisplayDialog(
                    "❂ YaGa WARNING!",
                    "The YaGa component is not active and enabled!\n\nAre you sure you want to continue?",
                    "Continue",
                    "Cancel"))
                throw new BuildFailedException("The YaGa component is deactivated!");
        }
    }
}
}
