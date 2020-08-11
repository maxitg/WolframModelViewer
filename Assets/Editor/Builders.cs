using System;
using System.Collections.Generic;
using System.Linq;
using UnityEditor;
using UnityEditor.PackageManager.Requests;
using UnityEditor.PackageManager;
using UnityEditor.SceneManagement;
using UnityEngine;

public class Builds : EditorWindow
{
    public static PlatformID GetPlatformID()
    {
        PlatformID id = System.Environment.OSVersion.Platform;

        // Distinguish Linux and OSX.
        if (id == System.PlatformID.Unix)
        {
            bool isOSX = false;
            try
            {
                isOSX = string.Equals("Darwin", System.Diagnostics.Process.Start(
                    new System.Diagnostics.ProcessStartInfo("uname") { 
                            UseShellExecute = false, RedirectStandardOutput = true }).StandardOutput.ReadToEnd().Trim());
            }
            catch { }
            if (isOSX)
                id = System.PlatformID.MacOSX;
        }

        return id;
    }

    static void AddPackages(string[] packageIDs)
    {
        foreach (string id in packageIDs)
        {
            AddRequest request = Client.Add(id);
            while (true) {
                if (request.IsCompleted)
                {
                    if (request.Status == StatusCode.Success)
                    {
                        Debug.Log("Package installed: " + request.Result.packageId);
                    }
                    else if (request.Status >= StatusCode.Failure)
                    {
                        Debug.LogError(request.Error.message);
                    }
                    break;
                }
            }
        }
    }

    // ------------------- Common modules -------------------------------------

    static void CommonPackages()
    {
        // List of packages
        string[] packages = {
            "com.unity.textmeshpro"
        };

        AddPackages(packages);
    }

    // ------------------- Wolfram / Set Desktop Build ------------------------

    [MenuItem("Wolfram/ Set Desktop Build", false, 10)]
    static void SetDesktopBuild()
    {
        // Get build platform
        PlatformID platform = GetPlatformID();

        // Switch target build platform
        if (platform == System.PlatformID.MacOSX)
        {
            EditorUserBuildSettings.SwitchActiveBuildTarget(BuildTargetGroup.Standalone, BuildTarget.StandaloneOSX);
        }
        else if (platform != System.PlatformID.Unix) 
        {
            EditorUserBuildSettings.SwitchActiveBuildTarget(BuildTargetGroup.Standalone, BuildTarget.StandaloneWindows64);
        } 
        else 
        {
            Debug.LogError("Build platform not supported");
        }

        // Load common packages
        CommonPackages();

        // Load the scene
        EditorSceneManager.OpenScene("Assets/Scenes/Viewer.unity", OpenSceneMode.Single);

    }

    // ------------------- Wolfram / Set Desktop Build ------------------------

    [MenuItem("Wolfram/ Set iOS AR Build", false, 10)]
    static void SetiOSARBuild()
    {
        string scenePath = "Assets/Scenes/iOSViewer.unity";

        // Get build platform
        PlatformID platform = GetPlatformID();

        // Switch target build platform
        if (platform == System.PlatformID.MacOSX)
        {
            bool result = EditorUserBuildSettings.SwitchActiveBuildTarget(BuildTargetGroup.iOS, BuildTarget.iOS);
            if (result) 
            {
                Debug.Log("Target plaform: iOS");
            }
            else 
            {
                Debug.LogError("Failed to switch to target iOS platform ");
            }
        }
        else 
        {
            Debug.Log("Build platform not supported");
        }

        // Add build scene
        EditorBuildSettings.scenes = new EditorBuildSettingsScene[] {new EditorBuildSettingsScene(scenePath, true)};

        // Load common packages and iOS AR packages
        CommonPackages();

        string[] iOSPackages = {
            "com.unity.xr.arkit",
            "com.unity.xr.arkit-face-tracking",
            "com.unity.xr.arfoundation"
        };
        AddPackages(iOSPackages);

        // Load the scene
        EditorSceneManager.OpenScene(scenePath, OpenSceneMode.Single);
    }
}
