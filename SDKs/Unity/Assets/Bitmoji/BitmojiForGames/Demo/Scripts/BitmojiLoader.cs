using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using UnityEngine;
using System.IO;
using UnityEngine.UI;
using Bitmoji.BitmojiForGames;
using Bitmoji.BitmojiForGames.Components;

public class BitmojiLoader : MonoBehaviour
{
    public GameObject BitmojiAvatar;
    public SnapKitHandler Snapkit;
    public Text DebugText;
    public GameObject StickerObject;
    public Assets.LevelOfDetail LevelOfDetail = Assets.LevelOfDetail.LOD3;

    private const string LOCAL_FALLBACK_BITMOJI_PREFIX = "Models/fallback_bitmoji_LOD";
    private const string LOCAL_DANCE_ANIMATION_PREFIX = "Animations/win_dance_";
    private const string STICKER_ID = "9e669e76-bd42-43ba-bc81-83741de280f5";

    private void OnEnable()
    {
        Snapkit.OnUserDataFetched += OnBitmojiDataFetchCompleted;
    }

    private void OnDisable()
    {
        Snapkit.OnUserDataFetched -= OnBitmojiDataFetchCompleted;
    }

    // Start is called before the first frame update
    private async Task Start()
    {
        try
        {
            GameObject npcBitmoji = await Assets.AddDefaultAvatarToScene(LevelOfDetail, null);
            DebugText.text = "Downloaded placeholder (ghost) Bitmoji successfully. Login with Snapchat to see your Bitmoji";
            ReplaceBitmoji(npcBitmoji, false);
        }
        catch (Exception ex)
        {
            DebugText.text = "Couldn't download NPC Bitmoji, using local fallback";
            Debug.Log("Error downloading NPC Bitmoji \n " + ex.Message);
            string fallbackBitmojiFilename = LOCAL_FALLBACK_BITMOJI_PREFIX + (LevelOfDetail.Equals(Assets.LevelOfDetail.LOD0) ? "0" : "3");
            GameObject fallbackAvatar = Assets.AddAvatarToSceneFromFile(fallbackBitmojiFilename, LevelOfDetail, true);
            ReplaceBitmoji(fallbackAvatar, false);
        }
    }

    public async void OnButtonTap_Login()
    {
        if (Application.isEditor)
        {
            DebugText.text = "Using test Bitmoji. Build to a mobile device to use the LoginKit flow";
            GameObject testAvatar = await Assets.AddTestAvatarToScene(LevelOfDetail);
            ReplaceBitmoji(testAvatar, true);
        }
        else
        {
            Snapkit.StartLogin();
        }
    }

    /***
     * Replaces the current Bitmoji in the scene with a new one
     */
    private async void ReplaceBitmoji(GameObject avatarObject, bool doTheDance, string avatarId = null)
    {
        // Clear children
        var children = new List<GameObject>();
        foreach (Transform child in BitmojiAvatar.transform)
        {
            children.Add(child.gameObject);
        }
        children.ForEach(child => Destroy(child));

        // Set animation
        if (doTheDance)
        {
            Animation animation = avatarObject.AddComponent<Animation>();

            string danceAnimationFilename = LOCAL_DANCE_ANIMATION_PREFIX;
            AvatarAttributes avatarAttributes = avatarObject.GetComponent<AvatarAttributes>();
            if (avatarAttributes != null && avatarAttributes.AnimationBodyType != null)
            {
                danceAnimationFilename += avatarAttributes.AnimationBodyType;
            }
            else
            {
                danceAnimationFilename += "default";
            }
            danceAnimationFilename += (LevelOfDetail.Equals(Assets.LevelOfDetail.LOD0) ? "_LOD0.glb" : "_LOD3.glb");

            AnimationClip danceAnimation = Assets.AddAnimationClipFromFile(danceAnimationFilename, LevelOfDetail, true);
            danceAnimation.wrapMode = WrapMode.Loop;
            animation.AddClip(danceAnimation, danceAnimation.name);
            animation.CrossFade(danceAnimation.name);
        }

        // Set parent
        avatarObject.transform.parent = BitmojiAvatar.transform;
        avatarObject.transform.localRotation = Quaternion.identity;

        if (avatarId != null)
        {
            RawImage rawImageComponent = StickerObject.GetComponent<RawImage>();
            if (rawImageComponent != null)
            {
                rawImageComponent.texture = await Assets.GetStickerAsTexture(avatarId, STICKER_ID);
            }
        }
    }

    /**
     * Triggered when the SnapKitHandler is done fetching the Avatar ID
     */
    private void OnBitmojiDataFetchCompleted()
    {
        DebugText.text = "Authenticated. Fetching Bitmoji from API...";
        Debug.Log("Bitmoji Data Fetch completed. Going to download authenticated Bitmoji");
        new Task(async () => { await FetchAuthenticatedBitmoji(); }).RunSynchronously();
    }

    /**
     * Async function to fetch the user's real avatar
     */
    private async Task FetchAuthenticatedBitmoji()
    {
        Dictionary<string, string> additionalParameters = new Dictionary<string, string>();
        if (LevelOfDetail.Equals(Assets.LevelOfDetail.LOD0))
        {
            additionalParameters.Add("usePbr", "true");
        }

        GameObject bitmoji = await Assets.AddAvatarToScene(Snapkit.AvatarId, LevelOfDetail, Snapkit.AccessToken, null, additionalParameters);
        DebugText.text = "3D Bitmoji downloaded successfully.";
        ReplaceBitmoji(bitmoji, true, Snapkit.AvatarId);
    }

}

