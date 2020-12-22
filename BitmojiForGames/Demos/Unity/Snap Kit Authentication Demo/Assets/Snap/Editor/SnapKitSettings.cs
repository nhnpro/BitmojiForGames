using UnityEditor;
using UnityEngine;
using UnityEngine.Serialization;

[CreateAssetMenu]
public class SnapKitSettings : ScriptableObject
{
	[Header("iOS")]
	[FormerlySerializedAs("UrlSceme")] public string iOSUrlSceme;
	[FormerlySerializedAs("ClientId")] public string iOSClientId;
	[FormerlySerializedAs("RedirectUrl")] public string iOSRedirectUrl;
	[FormerlySerializedAs("Scopes")] public string[] iOSScopes;

	[Header("Android")]
	public string AndroidClientId;
	public string AndroidRedirectUrl;
	public string AndroidSnapScheme;
	public string AndroidSnapHost;
	public string AndroidSnapPathPrefix;
	public string[] AndroidScopes;

	public void CleanInput()
    {
		AndroidClientId = AndroidClientId.Trim();
		AndroidRedirectUrl = AndroidRedirectUrl.Trim();
		AndroidSnapScheme = AndroidSnapScheme.Trim();
		AndroidSnapHost = AndroidSnapHost.Trim();
		AndroidSnapPathPrefix = AndroidSnapPathPrefix.Trim();
		iOSUrlSceme = iOSUrlSceme.Trim();
		iOSClientId = iOSClientId.Trim();
		iOSRedirectUrl = iOSRedirectUrl.Trim();
    }


	public bool ValidateSettings()
	{
		if (EditorUserBuildSettings.activeBuildTarget == BuildTarget.iOS)
			return !string.IsNullOrEmpty(iOSUrlSceme) && !string.IsNullOrEmpty(iOSClientId) && !string.IsNullOrEmpty(iOSRedirectUrl);
		return !string.IsNullOrEmpty(AndroidClientId) && !string.IsNullOrEmpty(AndroidRedirectUrl);
	}
}
