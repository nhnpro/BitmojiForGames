using System;
using UnityEngine;

namespace Snap
{
	public enum ShareKind
	{
		Photo,
		Video,
		NoSnap
	}

	public class ShareContent
	{
		#pragma warning disable CS0414
		public string AttachmentUrl;
		public string CaptionText;
		[SerializeField] public Sticker Sticker;

		[SerializeField] string file;
		[SerializeField] ShareKind shareKind;

		[SerializeField] bool hasSticker;

		public ShareContent(ShareKind shareKind, string file = null)
		{
			this.shareKind = shareKind;
			this.file = file;
		}

		public void Validate()
		{
			if (Sticker != null)
			{
				hasSticker = true;
				Sticker.Validate();
			}
		}
	}

	[Serializable]
	public class Sticker
	{
		public float PosX;
		public float PosY;
		public float Height;
		public float Width;
		public float RotationDegreesClockwise;

		[SerializeField] string file;


		public Sticker(string file)
		{
			this.file = file;
		}

		public void Validate()
		{
			if (Width == 0 || Height == 0)
			{
				throw new Exception("Sticker Width and Height cannot be 0");
			}

			if (Width > 300 || Height > 300)
			{
				throw new Exception("Sticker Width and Height cannot exceed 300");
			}
		}
	}
}