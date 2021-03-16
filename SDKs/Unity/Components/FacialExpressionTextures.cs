using System.Collections.Generic;
using UnityEngine;

namespace Bitmoji.BitmojiForGames.Components
{
    public class FacialExpressionTextures : MonoBehaviour
    {
        public GameObject headObject;
        public Dictionary<string, Texture2D> faceTextures;

        // Must match the name used in ExtrasProcessor.
        public void FacialTextureSwapHandler(AnimationEvent animationEvent)
        {
            // Get animation event paramenter
            string faceTextureName = animationEvent.stringParameter;
            // facial texture animation function
            if (headObject != null && faceTextures != null && faceTextures.ContainsKey(faceTextureName))
            {
                headObject.GetComponent<Renderer>().material.mainTexture = faceTextures[faceTextureName];
            }
        }
    }
}
