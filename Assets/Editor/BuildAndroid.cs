using System.IO;
using UnityEditor;
using UnityEditor.Build.Reporting;
using UnityEngine;

public static class BuildAndroid
{
    public static void Build()
    {
        string outDir = Path.Combine("Builds", "Android");
        Directory.CreateDirectory(outDir);
        string apkPath = Path.Combine(outDir, "solar-system.apk");

        var report = BuildPipeline.BuildPlayer(new BuildPlayerOptions
        {
            scenes = System.Array.ConvertAll(
                System.Array.FindAll(EditorBuildSettings.scenes, s => s.enabled),
                s => s.path),
            locationPathName = apkPath,
            target = BuildTarget.Android,
            options = BuildOptions.None
        });

        Debug.Log($"[BuildAndroid] {report.summary.result} {apkPath} {report.summary.totalSize} bytes");
        if (report.summary.result != BuildResult.Succeeded)
            EditorApplication.Exit(1);
    }
}
