using UnityEngine;
using Newtonsoft.Json.Linq;
using Bitmoji.GLTFUtility;
using System;
using System.Collections.Generic;
using System.Linq;

namespace Bitmoji.BitmojiForGames
{
    internal class BFGExtrasProcessor : GLTFExtrasProcessor
    {
        override public void ProcessExtras(GameObject importedObject, AnimationClip[] animations, JObject extras)
        {
            if (extras != null)
            {
                if (extras.ContainsKey("facialExpressionTextures"))
                {
                    Dictionary<string, Texture2D> faceTextures = new Dictionary<string, Texture2D>();
                    JArray facialExpressionTextures = extras["facialExpressionTextures"].Value<JArray>();
                    if (facialExpressionTextures.Count > 0) {
                        foreach (JObject currentExpression in facialExpressionTextures)
                        {
                            string expressionName = currentExpression["name"].ToString();
                            // remove data:image/jpeg;base64,
                            string encodedFaceTexture = currentExpression["uri"].ToString().Split(',').Last();
                            // decode JPG
                            byte[] decodedBytes = Convert.FromBase64String(encodedFaceTexture);
                            Texture2D tex = new Texture2D(2, 2);
                            tex.LoadImage(decodedBytes);
                            faceTextures.Add(expressionName, tex);
                        }
                        Transform headTransform = importedObject.transform.Find(Assets.LOD3_HEAD_PATH);
                        if (headTransform != null) {
                            Components.FacialExpressionTextures facialExpressionTexturesComponent = importedObject.AddComponent<Components.FacialExpressionTextures>();
                            facialExpressionTexturesComponent.headObject = headTransform.gameObject;
                            facialExpressionTexturesComponent.faceTextures = faceTextures;
                        }
                    }
                }

                if(extras.ContainsKey("gender") && extras.ContainsKey("animationBodyType"))
                {
                    Components.AvatarAttributes avatarAttributes = importedObject.AddComponent<Components.AvatarAttributes>();
                    avatarAttributes.Gender = (Assets.CharacterGender)extras["gender"].Value<ushort>();
                    avatarAttributes.AnimationBodyType = extras["animationBodyType"].Value<string>();
                }

                if (extras.ContainsKey("animation"))
                {
                    JArray animationExtras = extras["animation"].Value<JArray>();
                    for (int i = 0; i < animationExtras.Count; i++)
                    {
                        JObject animationExtra = animationExtras[i].Value<JObject>();
                        if (animationExtra.ContainsKey("facial_swaps"))
                        {
                            JObject facialSwapData = animationExtra["facial_swaps"].Value<JObject>();
                            //float frameRate = facialSwapData["frame_rate"].Value<float>();
                            JArray expressions = facialSwapData["expressions"].Value<JArray>();
                            JArray times = facialSwapData["times"].Value<JArray>();
                            if (expressions.Count == times.Count)
                            {
                                for (int j = 0; j < expressions.Count; j++)
                                {
                                    AnimationEvent faceSwapEvent = new AnimationEvent();
                                    faceSwapEvent.functionName = "FacialTextureSwapHandler";
                                    faceSwapEvent.time = times[j].Value<float>();
                                    faceSwapEvent.stringParameter = expressions[j].Value<string>();
                                    animations[i].AddEvent(faceSwapEvent);
                                }                                
                            }
                        }
                    }
                }
            }
        }
    }
}