using System;
using System.IO;
using System.IO.Compression;
using UnityEditor.Build;
using UnityEditor.Build.Reporting;

namespace YaGaPlugin.Editor
{
public class BuildProcessing : IPreprocessBuildWithReport, IPostprocessBuildWithReport
{
    int IOrderedCallback.callbackOrder { get; }

    void IPreprocessBuildWithReport.OnPreprocessBuild(BuildReport report)
    {
        var indexHtmlPath = report.summary.outputPath + "/index.html";
        if (File.Exists(indexHtmlPath))
            File.Delete(indexHtmlPath);
    }

    void IPostprocessBuildWithReport.OnPostprocessBuild(BuildReport report)
    {
        var buildPath = report.summary.outputPath;
        ZipFile.CreateFromDirectory(buildPath, $"{buildPath}_{DateTime.Now:yy.MM.dd.HH.mm}.zip");
    }
}
}
