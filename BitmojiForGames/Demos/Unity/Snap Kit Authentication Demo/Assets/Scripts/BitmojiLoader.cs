using System;
using System.Collections;
using System.Collections.Generic;
using System.Threading.Tasks;
using UnityEngine;
using System.Net.Http;
using System.Net.Security;
using System.IO;
using System.Net.Http.Headers;
using UnityEngine.UI;
using Bitmoji.GLTFUtility;
using Bitmoji;

public class BitmojiLoader : MonoBehaviour
{
    public GameObject BitmojiAvatar;
    public SnapKitHandler Snapkit;
    public Text DebugText;
    public Animation Animation;

    private const string LOCAL_BITMOJI = "Models/NpcBitmoji.glb";
    private const string LOCAL_DANCE_ANIMATION = "Animations/2284cb1f-4c57-49a2-a298-afce7e848032_default_LOD3.glb";

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
            GameObject npcBitmoji = await Bitmoji.BitmojiForGames.Assets.AddDefaultAvatarToScene(Bitmoji.BitmojiForGames.Assets.LevelOfDetail.LOD3, null);
            DebugText.text = "Downloaded placeholder (ghost) Bitmoji successfully. Login with Snapchat to see your Bitmoji";
            ReplaceBitmoji(npcBitmoji, false);
        }
        catch (Exception ex)
        {
            DebugText.text = "Couldn't download NPC Bitmoji, using local fallback";
            Debug.Log("Error downloading NPC Bitmoji \n " + ex.Message);
            GameObject fallbackAvatar = Bitmoji.BitmojiForGames.Assets.AddAvatarToSceneFromFile(Path.Combine(Application.streamingAssetsPath, LOCAL_BITMOJI), Bitmoji.BitmojiForGames.Assets.LevelOfDetail.LOD3);
            ReplaceBitmoji(fallbackAvatar, false);
        }

    }

    public async void OnButtonTap_Login()
    {
        if (Application.isEditor)
        {
            DebugText.text = "Using test Bitmoji. Build to a mobile device to use the LoginKit flow";
            GameObject testAvatar = await Bitmoji.BitmojiForGames.Assets.AddTestAvatarToScene(Bitmoji.BitmojiForGames.Assets.LevelOfDetail.LOD3);
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
            AnimationClip danceAnimation = Bitmoji.BitmojiForGames.Assets.AddAnimationClipFromFile(Path.Combine(Application.streamingAssetsPath, LOCAL_DANCE_ANIMATION), Bitmoji.BitmojiForGames.Assets.LevelOfDetail.LOD3, true);
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
        GameObject bitmoji = await Bitmoji.BitmojiForGames.Assets.AddAvatarToScene(Snapkit.AvatarId, Bitmoji.BitmojiForGames.Assets.LevelOfDetail.LOD3, Snapkit.AccessToken);
        DebugText.text = "3D Bitmoji downloaded successfully.";
        ReplaceBitmoji(bitmoji, true);
    }

}

