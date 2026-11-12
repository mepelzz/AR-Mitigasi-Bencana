using UnityEditor;
using UnityEditor.Build.Reporting;
using UnityEngine;
using System.IO;

// Letakkan di Assets/Editor/BuildAndroid.cs
// Menjalankan build APK dan mencatat hasil serta pesan error ke Console dan file.
public static class BuildAndroid
{
    [MenuItem("Tools/Build Android APK")]
    public static void Build()
    {
        Directory.CreateDirectory("Builds");

        var opts = new BuildPlayerOptions
        {
            scenes = new[] { "Assets/Scenes/MainMenu.unity", "Assets/Scenes/SceneAR.unity" },
            locationPathName = "Builds/ARMitigasi.apk",
            target = BuildTarget.Android,
            options = BuildOptions.None
        };

        BuildReport report = BuildPipeline.BuildPlayer(opts);
        BuildSummary s = report.summary;

        string log = "HASIL BUILD: " + s.result + "\n" +
                     "Error: " + s.totalErrors + ", Warning: " + s.totalWarnings + "\n" +
                     "Waktu: " + s.totalTime + "\n";

        foreach (var step in report.steps)
        {
            foreach (var msg in step.messages)
            {
                if (msg.type == LogType.Error || msg.type == LogType.Exception || msg.type == LogType.Assert)
                    log += "[" + step.name + "] " + msg.content + "\n";
            }
        }

        File.WriteAllText("Builds/hasil_build.txt", log);
        Debug.Log(log);
    }
}
