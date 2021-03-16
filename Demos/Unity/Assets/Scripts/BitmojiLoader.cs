using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using UnityEngine;
using System.IO;
using UnityEngine.UI;
using Bitmoji.BitmojiForGames;

public class BitmojiLoader : MonoBehaviour
{
    public GameObject BitmojiAvatar;
    public SnapKitHandler Snapkit;
    public Text DebugText;

    private const string LOCAL_FALLBACK_BITMOJI = "Models/FallbackBitmoji.glb";
    private const string LOCAL_DANCE_ANIMATION = "Animations/win_dance_LOD3.glb";

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
            GameObject npcBitmoji = await Assets.AddDefaultAvatarToScene(Assets.LevelOfDetail.LOD3, null);
            DebugText.text = "Downloaded placeholder (ghost) Bitmoji successfully. Login with Snapchat to see your Bitmoji";
            ReplaceBitmoji(npcBitmoji, false);
        }
        catch (Exception ex)
        {
            DebugText.text = "Couldn't download NPC Bitmoji, using local fallback";
            Debug.Log("Error downloading NPC Bitmoji \n " + ex.Message);
            GameObject fallbackAvatar = Assets.AddAvatarToSceneFromFile(Path.Combine(Application.streamingAssetsPath, LOCAL_FALLBACK_BITMOJI), Assets.LevelOfDetail.LOD3);
            ReplaceBitmoji(fallbackAvatar, false);
        }

    }

    public async void OnButtonTap_Login()
    {
        if (Application.isEditor)
        {
            DebugText.text = "Using test Bitmoji. Build to a mobile device to use the LoginKit flow";
            GameObject testAvatar = await Assets.AddTestAvatarToScene(Assets.LevelOfDetail.LOD3);
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
    private void ReplaceBitmoji(GameObject avatarObject, bool doTheDance)
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
            AnimationClip danceAnimation = Assets.AddAnimationClipFromFile(Path.Combine(Application.streamingAssetsPath, LOCAL_DANCE_ANIMATION), Assets.LevelOfDetail.LOD3, true);
            danceAnimation.wrapMode = WrapMode.Loop;
            animation.AddClip(danceAnimation, danceAnimation.name);
            animation.CrossFade(danceAnimation.name);
        }

        // Set parent
        avatarObject.transform.parent = BitmojiAvatar.transform;
        avatarObject.transform.localRotation = Quaternion.identity;
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
        GameObject bitmoji = await Assets.AddAvatarToScene(Snapkit.AvatarId, Assets.LevelOfDetail.LOD3, Snapkit.AccessToken);
        DebugText.text = "3D Bitmoji downloaded successfully.";
        ReplaceBitmoji(bitmoji, true);
    }

}

