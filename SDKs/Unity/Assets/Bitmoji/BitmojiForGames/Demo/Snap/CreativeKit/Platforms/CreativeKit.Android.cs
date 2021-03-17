using UnityEngine;

namespace Snap
{
	internal class CreativeKitAndroid : ICreativeKit
	{
		static AndroidJavaObject _plugin;

		static CreativeKitAndroid()
		{
			if (Application.platform != RuntimePlatform.Android)
				return;

			// find the plugin instance
			using (var pluginClass = new AndroidJavaClass("com.snap.unity.CreativeKit"))
				_plugin = pluginClass.CallStatic<AndroidJavaObject>("instance");
		}

		public void Send(ShareContent share)
		{
			_plugin.Call("send", JsonUtility.ToJson(share));
		}
	}
}