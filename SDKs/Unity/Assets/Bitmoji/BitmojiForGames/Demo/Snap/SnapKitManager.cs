using System;
using System.Collections.Generic;
using UnityEngine;

namespace Snap
{
	public class SnapKitManager : MonoBehaviour
	{
		internal static event Action<string> OnLoginCompletedEvent;
		internal static event Action OnLoginLinkDidStartEvent;
		internal static event Action OnLoginLinkDidSucceedEvent;
		internal static event Action OnLoginLinkDidFailEvent;
		internal static event Action OnLoginDidUnlinkEvent;
		internal static event Action<string> OnFetchUserDataSucceededEvent;
		internal static event Action<string> OnFetchUserDataFailedEvent;
		internal static event Action<Dictionary<string, object>> OnVerifySucceededEvent;
		internal static event Action<string> OnVerifyFailedEvent;
		internal static event Action OnSendSucceededEvent;
		internal static event Action<string> OnSendFailedEvent;


		static SnapKitManager()
		{
			// try/catch this just in case a user sticks this class on a GameObject in the scene
			try
			{
				// first we see if we already exist in the scene
				var obj = FindObjectOfType<SnapKitManager>();
				if (obj != null)
					return;

				// create a new GO for our manager. This name is crucial as all native code communicates with this class by its name.
				var managerGO = new GameObject("SnapKitManager");
				managerGO.AddComponent<SnapKitManager>();

				DontDestroyOnLoad(managerGO);
			}
			catch (UnityException)
			{
				Debug.LogWarning(
					"It looks like you have the SnapKitManager on a GameObject in your scene. It will be added to your scene at runtime automatically for you. Please remove the script from your scene.");
			}
		}

		void LoginCompleted(string error) => OnLoginCompletedEvent?.Invoke(error);

		void LoginLinkDidStart(string empty) => OnLoginLinkDidStartEvent?.Invoke();

		void LoginLinkDidSucceed(string error) => OnLoginLinkDidSucceedEvent?.Invoke();

		void LoginLinkDidFail(string empty) => OnLoginLinkDidFailEvent?.Invoke();

		void LoginDidUnlink(string empty) => OnLoginDidUnlinkEvent?.Invoke();

		void FetchUserDataSucceeded(string json) => OnFetchUserDataSucceededEvent?.Invoke(json);

		void FetchUserDataFailed(string error) => OnFetchUserDataFailedEvent?.Invoke(error);

		void OnSendFinished(string errorOrEmpty)
		{
			if (string.IsNullOrEmpty(errorOrEmpty))
				OnSendSucceededEvent?.Invoke();
			else
				OnSendFailedEvent?.Invoke(errorOrEmpty);
		}

		void VerifyCompleted(string json) => OnVerifySucceededEvent?.Invoke(JsonUtility.FromJson<Dictionary<string,object>>(json));

		void VerifyFailed(string error) => OnVerifyFailedEvent?.Invoke(error);
	}
}