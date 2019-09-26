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
	


	// --- Build all ---

	[MenuItem("VRP/Build All")]
	public static void BuildAll() {
		//First build android apks if the active build target is already Android to prevent switching twice.
		if(EditorUserBuildSettings.activeBuildTarget == BuildTarget.Android) {
			BuildAndroid();
			BuildWin64();
		} else {
			BuildWin64();
			BuildAndroid();
		}
	}

	[MenuItem("VRP/Build All - Prepare Daydream")]
	public static void BuildAllPrepareDaydream() {
		BuildWin64();
		BuildAndroidGuide();
		Build0ldAndroidGuide();
		BuildOculus();
		PrepareDaydreamBuild();
	}



	// --- Prepare builds ---

	[MenuItem("VRP/Prepare Oculus Build")]
	public static void PrepareOculusBuild() {
		EditorUserBuildSettings.SwitchActiveBuildTarget(BuildTargetGroup.Android, BuildTarget.Android);
		PlayerSettings.virtualRealitySupported = true;
		PlayerSettings.SetVirtualRealitySDKs(BuildTargetGroup.Android, new[] { "Oculus" });
		PlayerSettings.productName = "Library of Ourselves (User)";
		PlayerSettings.SetApplicationIdentifier(BuildTargetGroup.Android, "sco.Haze.LibraryOfOurselves");
		enableScene(1);
		BuildPlayerWindow.ShowBuildPlayerWindow();
	}

	[MenuItem("VRP/Prepare Daydream Build")]
	public static void PrepareDaydreamBuild() {
		EditorUserBuildSettings.SwitchActiveBuildTarget(BuildTargetGroup.Android, BuildTarget.Android);
		PlayerSettings.virtualRealitySupported = true;
		PlayerSettings.SetVirtualRealitySDKs(BuildTargetGroup.Android, new[] { "daydream" });
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

	[MenuItem("VRP/Prepare Win64 Guide Build")]
	public static void PrepareWin64GuideBuild() {
		EditorUserBuildSettings.SwitchActiveBuildTarget(BuildTargetGroup.Standalone, BuildTarget.StandaloneWindows64);
		PlayerSettings.productName = "Library of Ourselves (Guide)";
		PlayerSettings.SetApplicationIdentifier(BuildTargetGroup.Android, "sco.Haze.LibraryOfOurselvesGuide");
		enableScene(0);
		BuildPlayerWindow.ShowBuildPlayerWindow();
	}

	[MenuItem("VRP/Prepare 0ld Android Guide Build")]
	public static void Prepare0ldAndroidGuideBuild() {
		EditorUserBuildSettings.SwitchActiveBuildTarget(BuildTargetGroup.Android, BuildTarget.Android);
		PlayerSettings.virtualRealitySupported = false;
		PlayerSettings.productName = "Library of Ourselves (Guide 0)";
		PlayerSettings.SetApplicationIdentifier(BuildTargetGroup.Android, "sco.Haze.LibraryOfOurselvesGuide0");
		enableScene(2);
		BuildPlayerWindow.ShowBuildPlayerWindow();
	}

	[MenuItem("VRP/Prepare 0ld Win64 Guide Build")]
	public static void Prepare0ldWin64GuideBuild() {
		EditorUserBuildSettings.SwitchActiveBuildTarget(BuildTargetGroup.Standalone, BuildTarget.StandaloneWindows64);
		PlayerSettings.productName = "Library of Ourselves (Guide 0)";
		PlayerSettings.SetApplicationIdentifier(BuildTargetGroup.Android, "sco.Haze.LibraryOfOurselvesGuide0");
		enableScene(2);
		BuildPlayerWindow.ShowBuildPlayerWindow();
	}



	// --- Perform builds ---

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

	[MenuItem("VRP/Build Daydream")]
	public static void __buildDaydream() {
		BuildDaydream();
	}
	public static void BuildDaydream(string filename = "daydream.apk") {
		PrepareDaydreamBuild();
		BuildPlayerOptions options = new BuildPlayerOptions();
		options.scenes = new string[] { "Assets/Scenes/VR.unity" };
		options.locationPathName = "Builds/" + filename;
		options.target = BuildTarget.Android;
		options.options = BuildOptions.None;

		BuildReport report = BuildPipeline.BuildPlayer(options);
		BuildSummary summary = report.summary;

		if(summary.result == BuildResult.Succeeded) {
			Debug.Log("Successfully built Daydream. Size: " + summary.totalSize + " bytes");
		}
		if(summary.result == BuildResult.Failed) {
			Debug.LogWarning("Build failed: could not build Daydream.");
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

	[MenuItem("VRP/Build Win64 Guide")]
	public static void BuildWin64Guide() {
		PrepareWin64GuideBuild();
		BuildPlayerOptions options = new BuildPlayerOptions();
		options.scenes = new string[] { "Assets/Scenes/Guide.unity" };
		options.locationPathName = "Builds/win64/Library of Ourselves (Guide).exe";
		options.target = BuildTarget.StandaloneWindows64;
		options.options = BuildOptions.None;

		BuildReport report = BuildPipeline.BuildPlayer(options);
		BuildSummary summary = report.summary;

		if(summary.result == BuildResult.Succeeded) {
			Debug.Log("Successfully built Win64 Guide. Size: " + summary.totalSize + " bytes");
		}
		if(summary.result == BuildResult.Failed) {
			Debug.LogWarning("Build failed: could not build Win64 Guide.");
		}
	}

	[MenuItem("VRP/Build 0ld Android Guide")]
	public static void Build0ldAndroidGuide() {
		Prepare0ldAndroidGuideBuild();
		BuildPlayerOptions options = new BuildPlayerOptions();
		options.scenes = new string[] { "Assets/Scenes/OldGuide.unity" };
		options.locationPathName = "Builds/guide0.apk";
		options.target = BuildTarget.Android;
		options.options = BuildOptions.None;

		BuildReport report = BuildPipeline.BuildPlayer(options);
		BuildSummary summary = report.summary;

		if(summary.result == BuildResult.Succeeded) {
			Debug.Log("Successfully built 0ld Android Guide. Size: " + summary.totalSize + " bytes");
		}
		if(summary.result == BuildResult.Failed) {
			Debug.LogWarning("Build failed: could not build 0ld Android Guide.");
		}
	}

	[MenuItem("VRP/Build 0ld Win64 Guide")]
	public static void Build0ldWin64Guide() {
		Prepare0ldWin64GuideBuild();
		BuildPlayerOptions options = new BuildPlayerOptions();
		options.scenes = new string[] { "Assets/Scenes/OldGuide.unity" };
		options.locationPathName = "Builds/win640/Library of Ourselves (Guide 0).exe";
		options.target = BuildTarget.StandaloneWindows64;
		options.options = BuildOptions.None;

		BuildReport report = BuildPipeline.BuildPlayer(options);
		BuildSummary summary = report.summary;

		if(summary.result == BuildResult.Succeeded) {
			Debug.Log("Successfully built 0ld Win64 Guide. Size: " + summary.totalSize + " bytes");
		}
		if(summary.result == BuildResult.Failed) {
			Debug.LogWarning("Build failed: could not build 0ld Win64 Guide.");
		}
	}



	// --- Bulk builds ---

	[MenuItem("VRP/VR Builds")]
	public static void BuildVR() {
		BuildOculus();
		BuildDaydream();
	}

	[MenuItem("VRP/Guide Builds")]
	public static void BuildGuide() {
		BuildAndroidGuide();
		BuildWin64Guide();
	}

	[MenuItem("VRP/0ld Guide Builds")]
	public static void Build0ldGuide() {
		Build0ldAndroidGuide();
		Build0ldWin64Guide();
	}

	[MenuItem("VRP/Build All Android")]
	public static void BuildAndroid() {
		BuildVR();
		BuildAndroidGuide();
		Build0ldAndroidGuide();
	}

	[MenuItem("VRP/Build All Win64")]
	public static void BuildWin64() {
		BuildWin64Guide();
		Build0ldWin64Guide();
	}

	[MenuItem("VRP/Build 5 Daydreams")]
	public static void Build5Daydream() {
		BuildDaydream();
		for(int i = 0; i<4; ++i) {
			BuildDaydream("daydream" + (i + 2) + ".apk");
		}
	}

}

#endif