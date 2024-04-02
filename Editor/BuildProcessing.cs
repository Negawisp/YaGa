using System;
using System.IO;
using System.IO.Compression;
using UnityEditor;
using UnityEditor.Build;
using UnityEditor.Build.Reporting;
using UnityEditor.SceneManagement;

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
        var yagaCount = 0;
        var yagaActive = 0;
        var currentScene = EditorSceneManager.GetActiveScene().path;
        var allScenes = EditorBuildSettings.scenes;
        foreach (var scene in allScenes)
        {
            var s = EditorSceneManager.OpenScene(scene.path);
            var gameObjects = s.GetRootGameObjects();
            foreach (var go in gameObjects)
            {
                var c = go.GetComponentInChildren<YaGa>(true);
                if (c == null) continue;
                yagaCount += 1;
                yagaActive += c.isActiveAndEnabled ? 1 : 0;
                break;
            }
        }
        EditorSceneManager.OpenScene(currentScene);

        if (yagaCount != allScenes.Length)
        {
            if (!EditorUtility.DisplayDialog(
                    "❂ YaGa WARNING!",
                    "There is no YaGa component in some scenes!\n\nAre you sure you want to continue?",
                    "Continue",
                    "Cancel"))
                throw new BuildFailedException("The YaGa component is missing!");
        }
        else
        {
            if (yagaActive != allScenes.Length && !EditorUtility.DisplayDialog(
                    "❂ YaGa WARNING!",
                    "The YaGa component is not active and enabled in some scenes!\n\nAre you sure you want to continue?",
                    "Continue",
                    "Cancel"))
                throw new BuildFailedException("The YaGa component is deactivated!");
        }
    }
}
}
