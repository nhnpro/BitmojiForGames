using System;
using UnityEngine;

namespace Snap
{
	public static class CreativeKit
	{
		public static event Action OnSendSucceededEvent
		{
			add => SnapKitManager.OnSendSucceededEvent += value;
			remove => SnapKitManager.OnSendSucceededEvent -= value;
		}

		public static event Action<string> OnSendFailedEvent
		{
			add => SnapKitManager.OnSendFailedEvent += value;
			remove => SnapKitManager.OnSendFailedEvent -= value;
		}

		#pragma warning disable CS0649
		static ICreativeKit _creativeKit;
		#pragma warning restore

		static CreativeKit()
		{
#if UNITY_IOS && !UNITY_EDITOR
			_creativeKit = new CreativeKitIos();
#elif UNITY_ANDROID && !UNITY_EDITOR
			_creativeKit = new CreativeKitAndroid();
#endif
		}

		public static void Share(ShareContent share)
		{
			share.Validate();
			_creativeKit?.Send(share);
		}
	}
}