using System.Collections.Generic;
using System.Runtime.InteropServices;
using UnityEngine;

namespace Snap
{
	internal class LoginKitIos : ILoginKit
	{
		[DllImport("__Internal")]
		static extern void _snapKitLogin();

		public void Login() => _snapKitLogin();

		[DllImport("__Internal")]
		static extern bool _snapKitIsLoggedIn();

		public bool IsLoggedIn() => _snapKitIsLoggedIn();

		[DllImport("__Internal")]
		static extern void _snapKitVerify(string phone, string region);

		public void Verify(string phone, string region) => _snapKitVerify(phone, region);

		[DllImport("__Internal")]
		static extern void _snapKitUnlinkAllSessions();

		public void UnlinkAllSessions() => _snapKitUnlinkAllSessions();

		[DllImport("__Internal")]
		static extern string _snapKitGetAccessToken();

		public string GetAccessToken() => _snapKitGetAccessToken();

		[DllImport("__Internal")]
		static extern bool _snapKitHasAccessToScope(string scope);

		public bool HasAccessToScope(string scope) => _snapKitHasAccessToScope(scope);

		[DllImport("__Internal")]
		static extern void _snapKitFetchUserDataWithQuery(string query, string variables);

		public void FetchUserDataWithQuery(string query, Dictionary<string, object> variables)
		{
			variables = variables ?? new Dictionary<string, object>();
			_snapKitFetchUserDataWithQuery(query, JsonUtility.ToJson(variables));
		}
	}
}