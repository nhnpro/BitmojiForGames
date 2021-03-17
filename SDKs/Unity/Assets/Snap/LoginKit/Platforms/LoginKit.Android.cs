using System.Collections.Generic;
using UnityEngine;

namespace Snap
{
	internal class LoginKitAndroid : ILoginKit
	{
		static AndroidJavaObject _plugin;

		static LoginKitAndroid()
		{
			if (Application.platform != RuntimePlatform.Android)
				return;

			// find the plugin instance
			using (var pluginClass = new AndroidJavaClass("com.snap.unity.LoginKit"))
				_plugin = pluginClass.CallStatic<AndroidJavaObject>("instance");
		}

		public void Login() => _plugin.Call("login");

		public bool IsLoggedIn() => _plugin.Call<bool>("isLoggedIn");

		public void Verify(string phone, string region) => _plugin.Call("verify", phone, region);

		public void UnlinkAllSessions() => _plugin.Call("unlinkAllSessions");

		public string GetAccessToken() => _plugin.Call<string>("getAccessToken");

		public bool HasAccessToScope(string scope) => _plugin.Call<bool>("hasAccessToScope", scope);

		public void FetchUserDataWithQuery(string query, Dictionary<string, object> variables)
		{
			variables = variables ?? new Dictionary<string, object>();
			_plugin.Call("fetchUserDataWithQuery", query, JsonUtility.ToJson(variables));
		}
	}
}