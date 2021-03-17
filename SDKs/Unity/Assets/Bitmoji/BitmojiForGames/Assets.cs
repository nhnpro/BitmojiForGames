using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Net.Http;
using System.Net.Http.Headers;
using System.Threading.Tasks;
using UnityEngine;

namespace Bitmoji.BitmojiForGames
{
	public static class Assets
	{
        public enum LevelOfDetail : ushort
        {
            LOD0 = 0,
            LOD3 = 3
        }

        public enum CharacterGender : ushort
        {
            Male = 1,
            Female = 2
        }

        private const string BFG_BASE_URL = "https://bitmoji.api.snapchat.com/bitmoji-for-games";
		private const string AVATAR_URL = BFG_BASE_URL + "/model";
		private const string DEFAULT_AVATAR_URL = BFG_BASE_URL + "/default_avatar";
		private const string ANIMATION_URL = BFG_BASE_URL + "/animation";
		private const string TEST_AVATAR_URL = BFG_BASE_URL + "/test_avatar";
        private const string PROP_URL = BFG_BASE_URL + "/prop";

        private const string SDK_BASE_URL = "https://sdk.bitmoji.com";
        private const string STICKER_URL = SDK_BASE_URL + "/me/sticker";

        internal const string LOD3_AVATAR_PATH = "AVATAR";
        internal const string LOD3_GLASSES_PATH = LOD3_AVATAR_PATH + "/C_glasses_GEO";
        internal const string LOD3_HEAD_PATH = LOD3_AVATAR_PATH + "/C_head_GEO";

        /*
        * Downloads a binary file and returns the byte array
        */
        private static async Task<byte[]> DownloadFile(string baseUrl, string snapAccessToken = null, Dictionary<string, string> queryParameters = null)
        {
            string finalUrl = baseUrl;
            // Add query parameters if available
            if (queryParameters != null)
            {
                for (int i = 0; i < queryParameters.Count; i++)
                {
                    KeyValuePair<string, string> entry = queryParameters.ElementAt(i);
                    finalUrl += (i == 0) ? "?" : "&";
                    finalUrl += entry.Key + "=" + entry.Value;
                }
            }

            // Automatically deflate Gzip response
            var handler = new HttpClientHandler();
            handler.AutomaticDecompression = System.Net.DecompressionMethods.Deflate | System.Net.DecompressionMethods.GZip;

            // Create request
            var httpClient = new HttpClient(handler);
            if (snapAccessToken != null)
            {
                // For the Test and Default Bitmoji, no authentication is required.
                // But a LoginKit Access Token is required to retrieve the user's Bitmoji, animations, and props
                httpClient.DefaultRequestHeaders.Authorization = new AuthenticationHeaderValue("Bearer", snapAccessToken);
            }

            var response = await httpClient.GetAsync(finalUrl);
            response.EnsureSuccessStatusCode();

            return await response.Content.ReadAsByteArrayAsync();
        }

        private static async Task<byte[]> DownloadAvatarAsync(string avatarId, LevelOfDetail levelOfDetail, string snapAccessToken, Dictionary<string, string> additionalParameters = null)
        {
            Dictionary<string, string> queryParameters = new Dictionary<string, string>();
            queryParameters.Add("lod", Convert.ToInt32(levelOfDetail).ToString());
            queryParameters.Add("avatar_id", avatarId);
            if (additionalParameters != null)
            {
                foreach (var parameter in additionalParameters) queryParameters.Add(parameter.Key, parameter.Value);
            }
            return await DownloadFile(AVATAR_URL, snapAccessToken, queryParameters);
        }

        private static async Task<byte[]> DownloadDefaultAvatarAsync(LevelOfDetail levelOfDetail, Dictionary<string, string> additionalParameters = null)
        {
            Dictionary<string, string> queryParameters = new Dictionary<string, string>();
            queryParameters.Add("lod", Convert.ToInt32(levelOfDetail).ToString());
            if (additionalParameters != null)
            {
                foreach (var parameter in additionalParameters) queryParameters.Add(parameter.Key, parameter.Value);
            }
            return await DownloadFile(DEFAULT_AVATAR_URL, null, queryParameters);
        }

        private static async Task<byte[]> DownloadAnimationAsync(string animationLibraryId, LevelOfDetail levelOfDetail, string snapAccessToken, string animationBodyType = "default", Dictionary<string, string> additionalParameters = null)
        {
            Dictionary<string, string> queryParameters = new Dictionary<string, string>();
            queryParameters.Add("lod", Convert.ToInt32(levelOfDetail).ToString());
            queryParameters.Add("assetId", animationLibraryId);
            queryParameters.Add("bodyType", animationBodyType);
            if (additionalParameters != null)
            {
                foreach (var parameter in additionalParameters) queryParameters.Add(parameter.Key, parameter.Value);
            }
            return await DownloadFile(ANIMATION_URL, snapAccessToken, queryParameters);
        }

        private static async Task<byte[]> DownloadTestAvatarAsync(LevelOfDetail levelOfDetail, Dictionary<string, string> additionalParameters = null)
        {
            Dictionary<string, string> queryParameters = new Dictionary<string, string>();
            queryParameters.Add("lod", Convert.ToInt32(levelOfDetail).ToString());
            if (additionalParameters != null)
            {
                foreach (var parameter in additionalParameters) queryParameters.Add(parameter.Key, parameter.Value);
            }
            return await DownloadFile(TEST_AVATAR_URL, null, queryParameters);
        }

        private static async Task<byte[]> DownloadPropAsync(string propLibraryId, LevelOfDetail levelOfDetail, string snapAccessToken, Dictionary<string, string> additionalParameters = null)
        {
            Dictionary<string, string> queryParameters = new Dictionary<string, string>();
            queryParameters.Add("lod", Convert.ToInt32(levelOfDetail).ToString());
            queryParameters.Add("assetId", propLibraryId);
            if (additionalParameters != null)
            {
                foreach (var parameter in additionalParameters) queryParameters.Add(parameter.Key, parameter.Value);
            }
            return await DownloadFile(PROP_URL, snapAccessToken, queryParameters);
        }

        private static async Task<byte[]> DownloadStickerAsync(string avatarId, string stickerId, bool isFriend = false)
        {
            string stickerUrl = STICKER_URL + "/" + avatarId + "/" + stickerId;
            Dictionary<string, string> queryParameters = new Dictionary<string, string>();
            if(isFriend)
            {
                queryParameters.Add("friend", "1");
            }
            return await DownloadFile(stickerUrl, null, queryParameters);
        }

        private static GameObject InstantiateGlb(in byte[] glbBytes, in LevelOfDetail levelOfDetail, in GameObject parentObject = null)
        {
            GLTFUtility.ImportSettings importSettings = new GLTFUtility.ImportSettings();

            importSettings.extrasProcessor = new BFGExtrasProcessor();

            GameObject importedObject = GLTFUtility.Importer.LoadFromBytes(glbBytes, importSettings);

            if (levelOfDetail == LevelOfDetail.LOD3)
            {
                Transform glassesTransform = importedObject.transform.Find(LOD3_GLASSES_PATH);
                if(glassesTransform != null)
                {
                    GameObject glassesObject = glassesTransform.gameObject;
                    SkinnedMeshRenderer skinnedMeshRenderer = glassesObject.GetComponent<SkinnedMeshRenderer>();
                    if (skinnedMeshRenderer != null)
                    {
                        skinnedMeshRenderer.shadowCastingMode = UnityEngine.Rendering.ShadowCastingMode.Off;
                    }
                }
            }

            if(parentObject != null)
            {
                importedObject.transform.parent = parentObject.transform;
            }

            return importedObject;
        }
		
		private static AnimationClip InstantiateGlbAnimation(in byte[] glbBytes, in LevelOfDetail levelOfDetail, in bool useLegacyClips)
        {
            GLTFUtility.ImportSettings importSettings = new GLTFUtility.ImportSettings();
            importSettings.animationSettings.frameRate = 30;
            importSettings.animationSettings.compressBlendShapeKeyFrames = true;
            importSettings.animationSettings.interpolationMode = GLTFUtility.InterpolationMode.ImportFromFile;
            importSettings.animationSettings.useLegacyClips = useLegacyClips;
            importSettings.extrasProcessor = new BFGExtrasProcessor();

            AnimationClip[] animations = null;
            GameObject importedObject = GLTFUtility.Importer.LoadFromBytes(glbBytes, importSettings, out animations);

            // Remove the mesh/textures associated with the animation.
            UnityEngine.Object.Destroy(importedObject);

            // We assume there's only one useful clip in the downloaded animation.
            return (animations.Length > 0) ? animations[0] : null;
        }

        private static byte[] GetBytesFromResourcePath(in string resourcePath)
        {
            TextAsset textAsset = Resources.Load<TextAsset>(resourcePath);
            if (textAsset != null)
            {
                return textAsset.bytes;
            }
            else
            {
                return null;
            }
        }

        private static Texture2D InstantiateImageTexture(in byte[] stickerBytes)
        {
            Texture2D tex = new Texture2D(1, 1);
            tex.LoadImage(stickerBytes);
            return tex;
        }

		public static async Task<GameObject> AddAvatarToScene(string avatarId, LevelOfDetail levelOfDetail, string snapAccessToken, GameObject parentObject = null, Dictionary<string, string> additionalParameters = null)
        {
			return InstantiateGlb(await DownloadAvatarAsync(avatarId, levelOfDetail, snapAccessToken, additionalParameters), levelOfDetail, parentObject);
        }

        public static GameObject AddAvatarToSceneFromFile(string avatarFilePath, LevelOfDetail levelOfDetail, bool isResourcePath = false, GameObject parentObject = null, Dictionary<string, string> additionalParameters = null)
        {
            byte[] avatarBytes = null;
            if(isResourcePath)
            {
                avatarBytes = GetBytesFromResourcePath(avatarFilePath);
            }
            else
            {
                avatarBytes = File.ReadAllBytes(avatarFilePath);
            }
            return InstantiateGlb(avatarBytes, levelOfDetail, parentObject);
        }

        public static async Task<GameObject> AddDefaultAvatarToScene(LevelOfDetail levelOfDetail, GameObject parentObject = null, Dictionary<string, string> additionalParameters = null)
        {
            return InstantiateGlb(await DownloadDefaultAvatarAsync(levelOfDetail, additionalParameters), levelOfDetail, parentObject);
        }
		
		public static async Task<GameObject> AddTestAvatarToScene(LevelOfDetail levelOfDetail, GameObject parentObject = null, Dictionary<string, string> additionalParameters = null)
        {
            return InstantiateGlb(await DownloadTestAvatarAsync(levelOfDetail, additionalParameters), levelOfDetail, parentObject);
        }

		public static async Task<GameObject> AddPropFromLibraryToScene(string propLibraryId, LevelOfDetail levelOfDetail, string snapAccessToken, GameObject parentObject = null, Dictionary<string, string> additionalParameters = null)
        {
            return InstantiateGlb(await DownloadPropAsync(propLibraryId, levelOfDetail, snapAccessToken, additionalParameters), levelOfDetail, parentObject);
        }

		public static async Task<AnimationClip> AddAnimationClipFromLibrary(string animationLibraryId, LevelOfDetail levelOfDetail, string snapAccessToken, string animationBodyType = "default", bool useLegacyClips = true, Dictionary<string, string> additionalParameters = null)
        {
            return InstantiateGlbAnimation(await DownloadAnimationAsync(animationLibraryId, levelOfDetail, snapAccessToken, animationBodyType, additionalParameters), levelOfDetail, useLegacyClips);
        }

        public static AnimationClip AddAnimationClipFromFile(string animationFilePath, LevelOfDetail levelOfDetail, bool isResourcePath = false, bool useLegacyClips = true, Dictionary<string, string> additionalParameters = null)
        {
            byte[] animationBytes = null;
            if (isResourcePath)
            {
                animationBytes = GetBytesFromResourcePath(animationFilePath);
            }
            else
            {
                animationBytes = File.ReadAllBytes(animationFilePath);
            }
            return InstantiateGlbAnimation(animationBytes, levelOfDetail, useLegacyClips);
        }

        public static async Task<Texture2D> GetStickerAsTexture(string avatarId, string stickerId, bool isFriend = false)
        {
            return InstantiateImageTexture(await DownloadStickerAsync(avatarId, stickerId, isFriend));
        }
    }
}
