using System;
using System.Diagnostics;
using System.IO;
using System.Linq;
using UnityEditor;
using UnityEditor.Build;
using UnityEditor.Build.Reporting;
using UnityEditor.Callbacks;
using Debug = UnityEngine.Debug;
#if UNITY_IOS
using UnityEditor.iOS.Xcode;
#endif

namespace Snap.Editor
{
#if UNITY_ANDROID
	public class SnapPostProcessor : IPreprocessBuildWithReport
	{
		[MenuItem("Snap Kit/Post Process Android Manifest")]
		static void PostProcess()
		{
			var t = new SnapPostProcessor();
			t.OnPreprocessBuild(null);
		}

		public int callbackOrder => 0;
		public void OnPreprocessBuild(BuildReport report)
		{
			var settingsGuid = AssetDatabase.FindAssets("t:SnapKitSettings").FirstOrDefault();
			if (settingsGuid == null)
				return;

			var settings = AssetDatabase.LoadAssetAtPath<SnapKitSettings>(AssetDatabase.GUIDToAssetPath(settingsGuid));
			settings.CleanInput();
			if (!settings.ValidateSettings())
				return;

			Debug.Log("Snap PostProcessor found Snap settings. Injecting into AndroidManifest");
			var srcPath = "Assets/Snap/Editor/AndroidManifestTemplate.xml";
			var destPath = "Assets/Plugins/Android/SnapManifest/AndroidManifest.xml";

			var contents = File.ReadAllText(srcPath);
			contents = contents.Replace("CLIENT_ID", settings.AndroidClientId);
			contents = contents.Replace("REDIRECT_URL", settings.AndroidRedirectUrl);
			contents = contents.Replace("SCHEME", settings.AndroidSnapScheme);
			contents = contents.Replace("HOST", settings.AndroidSnapHost);
			contents = contents.Replace("PATH_PREFIX", settings.AndroidSnapPathPrefix);
			File.WriteAllText(destPath, contents);

			// dump the scopes to the arrays.xml file
			var arraysPath = "Assets/Plugins/Android/SnapManifest/res/values/arrays.xml";
			using (var stream = new StreamWriter(File.OpenWrite(arraysPath)))
			{
				stream.WriteLine("<?xml version=\"1.0\" encoding=\"utf-8\"?>");
				stream.WriteLine("	<resources>");
				stream.WriteLine("		<string-array name=\"snap_connect_scopes\">");

				foreach (var scope in settings.AndroidScopes)
					stream.WriteLine($"			<item>{scope}</item>");

				stream.WriteLine("		</string-array>");
				stream.WriteLine("</resources>");
			}
		}
	}

#endif

#if UNITY_IOS
	public static class SnapPostProcessor
	{
		[PostProcessBuild]
		static void OnPostProcessGenPodfile(BuildTarget buildTarget, string pathToBuiltProject)
		{
			if (buildTarget != BuildTarget.iOS)
				return;

			// update the Xcode project file
			var projPath = Path.Combine(pathToBuiltProject, "Unity-iPhone.xcodeproj/project.pbxproj");
			var project = new UnityEditor.iOS.Xcode.PBXProject();
			project.ReadFromFile(projPath);

			// grab our Podfile and copy it into the Xcode project folder and open get the correct GUID based on Unity version
#if UNITY_2019_3_OR_NEWER
			var target = project.GetUnityFrameworkTargetGuid();
			File.Copy("Assets/Snap/Editor/iOS/Podfile", Path.Combine(pathToBuiltProject, "Podfile"), true);
#else
			var target = project.TargetGuidByName("Unity-iPhone");
			File.Copy("Assets/Snap/Editor/iOS/Podfile_Unity2018", Path.Combine(pathToBuiltProject, "Podfile"), true);
#endif

			project.AddBuildProperty(target, "CLANG_ENABLE_MODULES", "YES");
			File.WriteAllText(projPath, project.WriteToString());

			Debug.Log("Snap PostProcessor running cocoapods");
			var proc = new Process
			{
				StartInfo =
				{
					WorkingDirectory = pathToBuiltProject,
					FileName = "/usr/local/bin/pod",
					Arguments = "install",
					UseShellExecute = false,
					RedirectStandardOutput = true,
					CreateNoWindow = true
				}
			};
			proc.Start();
			while (!proc.StandardOutput.EndOfStream)
			{
				var line = proc.StandardOutput.ReadLine();
				Debug.Log(line);
			}

			UpdateXcodePlist(BuildTarget.iOS, pathToBuiltProject);
		}

		private static void UpdateXcodePlist(BuildTarget buildTarget, string pathToBuiltProject)
		{
			if (buildTarget == BuildTarget.iOS)
			{
				var settingsGuid = AssetDatabase.FindAssets("t:SnapKitSettings").FirstOrDefault();
				if (settingsGuid == null)
					return;

				var settings = AssetDatabase.LoadAssetAtPath<SnapKitSettings>(AssetDatabase.GUIDToAssetPath(settingsGuid));
				settings.CleanInput();
				if (!settings.ValidateSettings())
					return;

				Debug.Log("Snap PostProcessor found Snap settings. Injecting into Info.plist");

				// Get plist
				var plistPath = pathToBuiltProject + "/Info.plist";
				var plist = new PlistDocument();
				plist.ReadFromString(File.ReadAllText(plistPath));

				// Get root
				var rootDict = plist.root;

				var urlTypes = rootDict.CreateArray("CFBundleURLTypes");
				var urlType = urlTypes.AddDict();
				urlType["CFBundleURLName"] = new PlistElementString(PlayerSettings.applicationIdentifier);
				urlType["CFBundleURLSchemes"] = new PlistElementArray();
				urlType["CFBundleURLSchemes"].AsArray().AddString(settings.iOSUrlSceme);

				rootDict["SCSDKClientId"] = new PlistElementString(settings.iOSClientId);
				rootDict["SCSDKRedirectUrl"] = new PlistElementString(settings.iOSRedirectUrl);

				var scopes = rootDict.CreateArray("SCSDKScopes");
				foreach (var scope in settings.iOSScopes)
					scopes.AddString(scope);

				var appQueries = rootDict.CreateArray("LSApplicationQueriesSchemes");
				appQueries.AddString("snapchat");
				appQueries.AddString("bitmoji-sdk");
				appQueries.AddString("itms-apps");

				// Write to file
				File.WriteAllText(plistPath, plist.WriteToString());
			}
		}
	}
#endif
}