using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Serialization;
using UnityEngine.EventSystems;
using UnityEngine.Events;


namespace Bitmoji.GLTFUtility {
	[Serializable]
	public class ImportSettings {

		public bool materials = true;
		[FormerlySerializedAs("shaders")]
		public ShaderSettings shaderOverrides = new ShaderSettings();
		public AnimationSettings animationSettings = new AnimationSettings();
		public bool generateLightmapUVs;

		[Tooltip("Script used to process extra data.")]
		public GLTFExtrasProcessor extrasProcessor;

	}
}