using System.Runtime.InteropServices;
using UnityEngine;

namespace Snap
{
	internal class CreativeKitIos : ICreativeKit
	{
		[DllImport("__Internal")]
		static extern void _creativeKitShare(string json);

		public void Send(ShareContent share) => _creativeKitShare(JsonUtility.ToJson(share));
	}
}