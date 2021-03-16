using System;
using System.Collections.Generic;

namespace Snap
{
	public static class LoginKit
	{
		public static event Action<string> OnLoginCompletedEvent
		{
			add => SnapKitManager.OnLoginCompletedEvent += value;
			remove => SnapKitManager.OnLoginCompletedEvent -= value;
		}

		public static event Action OnLoginLinkDidStartEvent
		{
			add => SnapKitManager.OnLoginLinkDidStartEvent += value;
			remove => SnapKitManager.OnLoginLinkDidStartEvent -= value;
		}

		public static event Action OnLoginLinkDidSucceedEvent
		{
			add => SnapKitManager.OnLoginLinkDidSucceedEvent += value;
			remove => SnapKitManager.OnLoginLinkDidSucceedEvent -= value;
		}

		public static event Action OnLoginLinkDidFailEvent
		{
			add => SnapKitManager.OnLoginLinkDidFailEvent += value;
			remove => SnapKitManager.OnLoginLinkDidFailEvent -= value;
		}

		public static event Action OnLoginDidUnlinkEvent
		{
			add => SnapKitManager.OnLoginDidUnlinkEvent += value;
			remove => SnapKitManager.OnLoginDidUnlinkEvent -= value;
		}

		public static event Action<string> OnFetchUserDataSucceededEvent
		{
			add => SnapKitManager.OnFetchUserDataSucceededEvent += value;
			remove => SnapKitManager.OnFetchUserDataSucceededEvent -= value;
		}

		public static event Action<string> OnFetchUserDataFailedEvent
		{
			add => SnapKitManager.OnFetchUserDataFailedEvent += value;
			remove => SnapKitManager.OnFetchUserDataFailedEvent -= value;
		}

		public static event Action<Dictionary<string, object>> OnVerifySucceededEvent
		{
			add => SnapKitManager.OnVerifySucceededEvent += value;
			remove => SnapKitManager.OnVerifySucceededEvent -= value;
		}

		public static event Action<string> OnVerifyFailedEvent
		{
			add => SnapKitManager.OnVerifyFailedEvent += value;
			remove => SnapKitManager.OnVerifyFailedEvent -= value;
		}

		#pragma warning disable CS0649
		static ILoginKit _loginKit;
		#pragma warning restore

		static LoginKit()
		{
#if UNITY_IOS && !UNITY_EDITOR
			_loginKit = new LoginKitIos();
#elif UNITY_ANDROID && !UNITY_EDITOR
			_loginKit = new LoginKitAndroid();
#endif
		}

		public static void Login() => _loginKit?.Login();

		public static bool IsLoggedIn() => _loginKit?.IsLoggedIn() ?? false;

		public static void UnlinkAllSessions() => _loginKit?.UnlinkAllSessions();

		public static string GetAccessToken() => _loginKit?.GetAccessToken();

		public static void Verify(string phone, string region) => _loginKit?.Verify(phone, region);

		public static bool HasAccessToScope(string scope) => _loginKit?.HasAccessToScope(scope) ?? false;

		public static void FetchUserDataWithQuery(string query, Dictionary<string, object> variables) => _loginKit?.FetchUserDataWithQuery(query, variables);
	}
}