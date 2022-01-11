﻿﻿using System;
using System.IO;
using System.Linq;
using UnityEditor;
using UnityEngine;
using Altom.AltUnityTester;
using Altom.AltUnityTesterEditor;

public class BuildTrashCat
{

  [MenuItem("Build/WindowsWithAltunity")]
  static void WindowsBuildInspectorWithAltUnity()
  {
    WindowsBuildFromCommandLine(true, 13000);
  }
  [MenuItem("Build/AndroidWithAltUnity")]
  static void AndroidBuild()
  {
    string proxyHost = System.Environment.GetEnvironmentVariable("PROXY_HOST");

    AndroidBuildFromCommandLine(true, proxyHost, 13000);
  }

  [MenuItem("Build/macOSWithAltUnity")]
  static void MacOSBuildInspectorWithAltUnity()
  {
    MacOSBuildFromCommandLine(true, 13000);
  }
  static void WindowsBuildFromCommandLine(bool withAltunity, int proxyPort = 13000)
  {
    SetPlayerSettings();

    Debug.Log("Starting Windows build..." + PlayerSettings.productName + " : " + PlayerSettings.bundleVersion);
    BuildPlayerOptions buildPlayerOptions = new BuildPlayerOptions();
    buildPlayerOptions.scenes = new string[]
    {
            "Assets/Scenes/Main.unity",
            "Assets/Scenes/Shop.unity",
            "Assets/Scenes/Start.unity"
    };
    if (withAltunity)
    {
      buildPlayerOptions.locationPathName = "TrashCatWindowsTest/TrashCat.exe";

    }
    else
    {
      buildPlayerOptions.locationPathName = "TrashCatWindows/TrashCat.exe";

    }
    buildPlayerOptions.target = BuildTarget.StandaloneWindows;
    buildPlayerOptions.targetGroup = BuildTargetGroup.Standalone;
    if (withAltunity)
    {
      buildPlayerOptions.options = BuildOptions.Development;
    }
    BuildGame(buildPlayerOptions, withAltunity, proxyPort: proxyPort);

  }

  static void AndroidBuildFromCommandLine(bool withAltunity, string proxyHost, int proxyPort = 13000)
  {
    SetPlayerSettings();

    Debug.Log("Starting Android build..." + PlayerSettings.productName + " : " + PlayerSettings.bundleVersion);
    BuildPlayerOptions buildPlayerOptions = new BuildPlayerOptions();
    buildPlayerOptions.scenes = new string[]
    {
            "Assets/Scenes/Main.unity",
            "Assets/Scenes/Shop.unity",
            "Assets/Scenes/Start.unity"
    };
    if (withAltunity)
    {
      buildPlayerOptions.locationPathName = "TraschatAndroidTest/TrashCat.apk";
    }
    else
    {
      buildPlayerOptions.locationPathName = "TraschatAndroid/TrashCat.apk";

    }
    buildPlayerOptions.target = BuildTarget.Android;
    buildPlayerOptions.targetGroup = BuildTargetGroup.Android;
    if (withAltunity)
    {
      buildPlayerOptions.options = BuildOptions.Development;
    }
    BuildGame(buildPlayerOptions, withAltunity, proxyHost, proxyPort);

  }

  private static void MacOSBuildFromCommandLine(bool withAltUnity, int proxyPort = 13000)
  {
    SetPlayerSettings();
    PlayerSettings.macRetinaSupport = true;

    Debug.Log("Starting Mac build..." + PlayerSettings.productName + " : " + PlayerSettings.bundleVersion);
    BuildPlayerOptions buildPlayerOptions = new BuildPlayerOptions();
    buildPlayerOptions.scenes = new string[]
    {
            "Assets/Scenes/Main.unity",
            "Assets/Scenes/Shop.unity",
            "Assets/Scenes/Start.unity"
    };
    if (withAltUnity)
    {
      buildPlayerOptions.locationPathName = "TrashCatTest.app";

    }
    else
    {
      buildPlayerOptions.locationPathName = "TrashCat.app";

    }
    buildPlayerOptions.target = BuildTarget.StandaloneOSX;
    buildPlayerOptions.targetGroup = BuildTargetGroup.Standalone;
    if (withAltUnity)
    {
      buildPlayerOptions.options = BuildOptions.Development;
    }

    BuildGame(buildPlayerOptions, withAltUnity, proxyPort: proxyPort);

  }


  private static void SetPlayerSettings()
  {
    PlayerSettings.companyName = "Altom";
    PlayerSettings.productName = "TrashCat";
    PlayerSettings.bundleVersion = "1.0";
    PlayerSettings.resizableWindow = true;
    PlayerSettings.defaultScreenHeight = 900;
    PlayerSettings.defaultScreenWidth = 1200;
    PlayerSettings.fullScreenMode = FullScreenMode.Windowed;
    PlayerSettings.SetApiCompatibilityLevel(BuildTargetGroup.Standalone, ApiCompatibilityLevel.NET_4_6);
    PlayerSettings.runInBackground = true;

  }
  static void BuildGame(BuildPlayerOptions buildPlayerOptions, bool withAltUnity, string proxyHost = null, int proxyPort = 13000)
  {
    try
    {
      if (withAltUnity)
      {
        AddAltUnity(buildPlayerOptions.targetGroup, buildPlayerOptions.scenes[0], proxyHost, proxyPort);
      }
      var results = BuildPipeline.BuildPlayer(buildPlayerOptions);

      if (withAltUnity)
      {
        RemoveAltUnity(buildPlayerOptions.targetGroup);
      }

      if (results.summary.totalErrors == 0 || results.summary.result == UnityEditor.Build.Reporting.BuildResult.Succeeded)
      {
        Debug.Log("Build succeeded!");

      }
      else
      {
        Debug.LogError("Build failed! " + results.steps + "\n Result: " + results.summary.result + "\n Stripping info: " + results.strippingInfo);
        // EditorApplication.Exit(1);
      }

      Debug.Log("Finished. " + PlayerSettings.productName + " : " + PlayerSettings.bundleVersion);
      // EditorApplication.Exit(0);
    }
    catch (Exception exception)
    {

      Debug.LogException(exception);
      // EditorApplication.Exit(1);
    }
  }
  static void AddAltUnity(BuildTargetGroup buildTargetGroup, string firstSceneName, string proxyHost = null, int proxyPort = 13000)
  {
    AltUnityBuilder.PreviousScenePath = firstSceneName;
    var instrumentationSettings = new AltUnityInstrumentationSettings();
    instrumentationSettings.AltUnityTesterPort = proxyPort;
    if (!string.IsNullOrEmpty(proxyHost)) instrumentationSettings.ProxyHost = proxyHost;
    AltUnityBuilder.AddAltUnityTesterInScritpingDefineSymbolsGroup(buildTargetGroup);
    AltUnityBuilder.InsertAltUnityInScene(firstSceneName, instrumentationSettings);
    Debug.Log("Instrumenting with proxyHost: " + proxyHost + ", proxyPort: " + proxyPort);

  }
  static void RemoveAltUnity(BuildTargetGroup buildTargetGroup)
  {
    AltUnityBuilder.RemoveAltUnityTesterFromScriptingDefineSymbols(buildTargetGroup);
  }

}
