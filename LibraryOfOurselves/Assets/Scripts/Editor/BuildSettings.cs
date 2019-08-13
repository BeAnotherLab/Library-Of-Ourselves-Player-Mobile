#if UNITY_EDITOR

using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEditor;

public class BuildSettings {

	[MenuItem("VRP/Prepare Oculus Build")]
	public static void PrepareOculusBuild() {
		EditorUserBuildSettings.SwitchActiveBuildTarget(BuildTargetGroup.Android, BuildTarget.Android);
		PlayerSettings.virtualRealitySupported = true;
		PlayerSettings.SetVirtualRealitySDKs(BuildTargetGroup.Android, new[] { "Oculus" });
		PlayerSettings.productName = "Library of Ourselves (User)";
		BuildPlayerWindow.ShowBuildPlayerWindow();
	}

	[MenuItem("VRP/Prepare Daydream Build")]
	public static void PrepareDaydreamBuild() {
		EditorUserBuildSettings.SwitchActiveBuildTarget(BuildTargetGroup.Android, BuildTarget.Android);
		PlayerSettings.virtualRealitySupported = true;
		PlayerSettings.SetVirtualRealitySDKs(BuildTargetGroup.Android, new[] { "daydream" });
		PlayerSettings.productName = "Library of Ourselves (User)";
		BuildPlayerWindow.ShowBuildPlayerWindow();
	}

	[MenuItem("VRP/Prepare Android Guide Build")]
	public static void PrepareAndroidGuideBuild() {
		EditorUserBuildSettings.SwitchActiveBuildTarget(BuildTargetGroup.Android, BuildTarget.Android);
		PlayerSettings.virtualRealitySupported = false;
		PlayerSettings.productName = "Library of Ourselves (Guide)";
		BuildPlayerWindow.ShowBuildPlayerWindow();
	}

	[MenuItem("VRP/Prepare Win64 Guide Build")]
	public static void PrepareWin64BuideBuild() {
		EditorUserBuildSettings.SwitchActiveBuildTarget(BuildTargetGroup.Standalone, BuildTarget.StandaloneWindows64);
		PlayerSettings.productName = "Library of Ourselves (Guide)";
		BuildPlayerWindow.ShowBuildPlayerWindow();
	}

}

#endif