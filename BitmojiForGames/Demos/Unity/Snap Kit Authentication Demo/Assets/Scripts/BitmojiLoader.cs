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
using Siccity.GLTFUtility;

public class BitmojiLoader : MonoBehaviour
{
    public GameObject Bitmoji;
    public SnapKitHandler Snapkit;
    public Text DebugText;
    public Animator Animation;

    private const string avatarPath = "/RootNode/AVATAR";
    private const string LOCAL_BITMOJI = "Models/NpcBitmoji.glb";
    private const string GHOST_AVATAR_PATH = "https://bitmoji.api.snapchat.com/bitmoji-for-games/default_avatar?lod=3";
    private const string NPC_AVATAR_PATH = "https://bitmoji.api.snapchat.com/bitmoji-for-games/test_avatar?lod=3";
    private const string AUTHENTICATED_AVATAR_PATH = "https://bitmoji.api.snapchat.com/bitmoji-for-games/model?avatar_id={0}&lod=3";

    // TODO: Add animation

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
            var npcBitmoji = await DownloadBitmoji(GHOST_AVATAR_PATH, "ghostBitmoji.glb");
            DebugText.text = "Downloaded placeholder (ghost) Bitmoji successfully. Login with Snapchat to see your Bitmoji";
            ReplaceBitmoji(npcBitmoji, false, false);
        }
        catch (Exception ex)
        {
            DebugText.text = "Couldn't download NPC Bitmoji, using local fallback";
            Debug.Log("Error downloading NPC Bitmoji \n " + ex.Message);
            ReplaceBitmoji(LOCAL_BITMOJI, true, false);
        }

    }

    public void OnButtonTap_Login()
    {
        if (Application.isEditor)
        {
            DebugText.text = "Using fallback Bitmoji. Build to a mobile device to use the LoginKit flow";
            ReplaceBitmoji(LOCAL_BITMOJI, true, true);
        }
        else
        {
            Snapkit.StartLogin();
        }
    }

    /***
     * Replaces the current Bitmoji in the scene with a new one
     */
    private void ReplaceBitmoji(string localGlbPath, bool relativeUrl, bool doTheDance)
    {
        // Clear children
        var children = new List<GameObject>();
        foreach (Transform child in Bitmoji.transform)
        {
            children.Add(child.gameObject);
        }
        children.ForEach(child => Destroy(child));

        // Load new model
        var url = localGlbPath;
        if (relativeUrl)
        {
            url = Path.Combine(Application.streamingAssetsPath, url);
        }
        var resultGO = Importer.LoadFromFile(url);

        // Set animation
        if (doTheDance)
        {
            var avatar = resultGO.transform.Find(avatarPath);
            var animator = avatar.gameObject.AddComponent<Animator>();
            animator.runtimeAnimatorController = Animation.runtimeAnimatorController;
            MobileFacialAnimationEvent.glb_Object = url;
            MobileFacialAnimationEvent.head_path = "C_head_GEO";
            avatar.gameObject.AddComponent<MobileFacialAnimationEvent>();
        }        

        // Set parent
        resultGO.transform.parent = Bitmoji.transform;
        resultGO.transform.localRotation = Quaternion.identity;

        // Nit: remove shadows (looks nicer)
        var meshes = resultGO.GetComponentsInChildren<SkinnedMeshRenderer>();
        foreach (var mesh in meshes)
        {
            mesh.receiveShadows = false;
        }

    }

    /***
     * Downloads a Bitmoji and saves to a file in temporaryCachePath
     */
    private async Task<string> DownloadBitmoji(string uri, string destinationFileName, string accessToken = null)
    {
        DebugText.text = "Attempting to download Bitmoji...";

        var destFile = new FileInfo(Path.Combine(Application.temporaryCachePath, destinationFileName));

        // Automatically deflate Gzip response
        var handler = new HttpClientHandler();
        handler.AutomaticDecompression = System.Net.DecompressionMethods.Deflate | System.Net.DecompressionMethods.GZip;

        // Create request
        var httpClient = new HttpClient(handler);
        if (accessToken != null)
        {
            // For the Test and NPC Bitmoji, no authentication is required.
            // But a LoginKit Access Token is required to retrieve the user's Bitmoji
            httpClient.DefaultRequestHeaders.Authorization = new AuthenticationHeaderValue("Bearer", accessToken);
        }

        Debug.Log($"AvatarId {Snapkit.AvatarId} \nAccess Token {Snapkit.AccessToken} \nURL:{uri}");


        // Send Request and save it to a temp file
        var response = await httpClient.GetAsync(uri);
        response.EnsureSuccessStatusCode();
        using (var ms = await response.Content.ReadAsStreamAsync())
        {
            using (var fs = File.Create(destFile.FullName))
            {
                ms.Seek(0, SeekOrigin.Begin);
                ms.CopyTo(fs);
            }
        }

        // Success!
        DebugText.text = "Bitmoji downloaded successfully!";
        Debug.Log($"Downloaded Bitmoji! \n to {destFile} \n from {uri}");

        return destFile.FullName;
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
        var uri = String.Format(AUTHENTICATED_AVATAR_PATH, Snapkit.AvatarId);
        var bitmoji = await DownloadBitmoji(uri, "authenticatedBitmoji.glb", Snapkit.AccessToken);
        DebugText.text = "3D Bitmoji downloaded successfully.";
        ReplaceBitmoji(bitmoji, false, true);
    }

}

