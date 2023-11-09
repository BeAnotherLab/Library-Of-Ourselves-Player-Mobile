#if UNITY_EDITOR

using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEditor;
using UnityEditor.Build.Reporting;

public class BuildSettings {

	//Enable a scene index in the build
	static void enableScene(int sceneIndex) {
		EditorBuildSettingsScene[] scenes = EditorBuildSettings.scenes;
		for(int i = 0; i<scenes.Length; ++i) {
			scenes[i].enabled = i == sceneIndex;
		}
		EditorBuildSettings.scenes = scenes;
	}
	
	[MenuItem("VRP/Prepare Oculus Build")]
	public static void PrepareOculusBuild() {
		EditorUserBuildSettings.SwitchActiveBuildTarget(BuildTargetGroup.Android, BuildTarget.Android);
		PlayerSettings.virtualRealitySupported = true;
		PlayerSettings.productName = "Library of Ourselves (User)";
		PlayerSettings.SetApplicationIdentifier(BuildTargetGroup.Android, "sco.Haze.LibraryOfOurselves");
		enableScene(1);
		BuildPlayerWindow.ShowBuildPlayerWindow();
	}

	[MenuItem("VRP/Prepare Android Guide Build")]
	public static void PrepareAndroidGuideBuild() {
		EditorUserBuildSettings.SwitchActiveBuildTarget(BuildTargetGroup.Android, BuildTarget.Android);
		PlayerSettings.virtualRealitySupported = false;
		PlayerSettings.productName = "Library of Ourselves (Guide)";
		PlayerSettings.SetApplicationIdentifier(BuildTargetGroup.Android, "sco.Haze.LibraryOfOurselvesGuide");
		enableScene(0);
		BuildPlayerWindow.ShowBuildPlayerWindow();
	}

	[MenuItem("VRP/Build Oculus")]
	public static void BuildOculus() {
		PrepareOculusBuild();
		BuildPlayerOptions options = new BuildPlayerOptions();
		options.scenes = new string[] { "Assets/Scenes/VR.unity" };
		options.locationPathName = "Builds/oculus.apk";
		options.target = BuildTarget.Android;
		options.options = BuildOptions.None;

		BuildReport report = BuildPipeline.BuildPlayer(options);
		BuildSummary summary = report.summary;

		if(summary.result == BuildResult.Succeeded) {
			Debug.Log("Successfully built Oculus. Size: " + summary.totalSize + " bytes");
		}
		if(summary.result == BuildResult.Failed) {
			Debug.LogWarning("Build failed: could not build Oculus.");
		}
	}

	[MenuItem("VRP/Build Android Guide")]
	public static void BuildAndroidGuide() {
		PrepareAndroidGuideBuild();
		BuildPlayerOptions options = new BuildPlayerOptions();
		options.scenes = new string[] { "Assets/Scenes/Guide.unity" };
		options.locationPathName = "Builds/guide.apk";
		options.target = BuildTarget.Android;
		options.options = BuildOptions.None;

		BuildReport report = BuildPipeline.BuildPlayer(options);
		BuildSummary summary = report.summary;

		if(summary.result == BuildResult.Succeeded) {
			Debug.Log("Successfully built Android Guide. Size: " + summary.totalSize + " bytes");
		}
		if(summary.result == BuildResult.Failed) {
			Debug.LogWarning("Build failed: could not build Android Guide.");
		}
	}

}

#endif