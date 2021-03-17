<p align="center">
<img src="../../../Shared/Logo.png" width="450"/>
</p>

# Unity Demo for SnapKit Authentication


Copyright © 2019-2020 Snap Inc. All rights reserved. All information subject to change without notice.

[Snap Developer Terms of Service](https://kit.snapchat.com/portal/eula?viewOnly=true)

[Snap Inc. Terms of Service](https://www.bitmoji.com/support/terms.html)

# About this demo

This is a Unity project that shows how to integrate [SnapKit's Unity SDK](https://assetstore.unity.com/packages/tools/integration/snap-kit-sdk-184712) into your Bitmoji For Games project in order to authenticate the player and get their current Avatar.

# App Registration

You need to register your app at the [Snap Kit developer portal](https://kit.snapchat.com).

After registering your app you will receive two **OAuth2 Client IDs**.

Initially we will onboard your **Development Client ID** in the Bitmoji For Games API. You can use that Client ID during development, before your app is reviewed and approved. But when using it, only accounts listed under the *Demo Users* in the app registration/profile page on [Snap Kit developer portal]([https://kit.snapchat.com/portal](https://kit.snapchat.com/portal)) will be able to use your application.

When your integration is approved for launch, we will onboard your **Production Client ID** into the Bitmoji For Games API. In parallel you should also submit your SnapKit application for approval via the SnapKit Portal. Once both of these approvals take place, your app authenticate any Snapchat account. 

# Setting up your app
This package includes a demo scene under `Assets/Scenes/SampleScene.unity`
When the setup steps below are correctly configured, running the demo scene will allow you to authenticate successfully with Snap Kit and download the currently logged in player's avatar.

In order for the Demo Scene to work, open your App Settings in the SnapKit Developer Portal and check the following:   
1. A version for your app is created in the **Versions** menu
2. Your Snapchat username is listed under **Demo Users**
3. Your package identifier is listed under **Platform Identifiers**, for the correct platform
4. LoginKit is enabled in your version, with the following options: **Display Name**, **Bitmoji Avatar**, **Snapchat Verify**. 
5. Still in the Login Kit pane, there's a valid **Redirect URI for OAuth**, for example: unitytest://snap-kit/oauth2
6. **Creative Kit** is enabled in your version. 

After checking all of the above, select the "Setup" section in the left navigation bar and make sure that the **Version** you just configured is the active Version on **Staging** for your app

___

## Unity Set up 
1. Open the `Assets/Snap/Editor` folder in your project explorer
4. If there's an asset called SnapKitSettings.asset file, that's where you'll input the information from your app. If the file is not present, create a new one in that folder by selecting **Assets->Create->Snap Kit Settings**
5. Click on the SnapKitSettings asset and make sure that the Inspector pane is visible
6. In the inspector pane, populate the information from your SnapKit Portal registered app.
   1. **Client ID**: your "OAuth2 Client ID"
   2. **Redirect URI**: one of your "Redirect URIs for OAuth"
   3. **URL Scheme**: the first part of the chosen Redirect URI. For example, if the Uri is `myapp://snap-kit/oauth/`, the URL Scheme is `myapp` (Note that the scheme needs to be defined as a lower-case string without special characters or numbers)
   4. **Host**: the second part of the chosen Redirect URI. For example, if the Uri is `myapp://snap-kit/oauth/`, the host is `snap-kit`
   5. **Path Prefix**: the third part of the chosen Redirect URI. For example, if the Uri is `myapp://snap-kit/oauth/`, the path prefix is `/oauth`
7. Go to Player Settings and make sure that the package name chosen for your app is one of the pre-defined "Platform Identifiers" defined on the Snap Kit portal

## Additional steps on Android
1. Go to Unity's Player Settings
2. Set **Internet Access** to "Require" 
3. Set the **Minimum API Level** to "21"

## Additional steps on iOS
1. Make sure to have [CocoaPods](https://cocoapods.org/) installed
2. Once the XCode project is built, select the Unity-iPhone Project -> (Targets) Unity iPhone -> Build Phases -> Expand "Embed Frameworks"
3. Drag the following Frameworks from the "Pods" project into that section: **SCSDKCoreKit**, **SCSDKLoginKit**, **SCSDKCreativeKit**
4. (Optional) if you get an error of "SCSDKLoginKit.h file not found", type `pod deintegrate && pod install` into a terminal at the iOS build folder and then on Xcode go to Product -> Clean Build Folder. 

-----

# Example: Logging in
The example below shows a minimal `MonoBehaviour` that can perform Login with Snapchat

```cs
using Snap;
using UnityEngine;

public class LoginManager : MonoBehaviour
{
    void OnEnable()
    {
        LoginKit.OnLoginLinkDidSucceedEvent += OnLoginSuccess;
        LoginKit.OnLoginLinkDidFailEvent += OnLoginFail;
    }

    void OnDisable()
    {
        LoginKit.OnLoginLinkDidSucceedEvent -= OnLoginSuccess;
        LoginKit.OnLoginLinkDidFailEvent -= OnLoginFail;
    }

    public void OnButtonTapped_Login()
    {
        LoginKit.Login();
    }

    void OnLoginSuccess()
    {
        // This is the Access Token you need to authenticate your Bitmoji Request
        Debug.Log("Login Succeeded. Access Token + " LoginKit.GetAccessToken());        
    }

    void OnLoginFail()
    {
        Debug.Log("Login Failed");
    }
}

```

# Example: Downloading a Bitmoji

```cs 
/***
 * Downloads a Bitmoji and saves to a file in temporaryCachePath
 */
private async Task<string> DownloadBitmoji(string avatarId, int levelOfDetail)
{
    // Request URI
    var uri = $"https://bitmoji.api.snapchat.com/bitmoji-for-games/model?avatar_id={avatarId}&lod={levelOfDetail}";

    // Destination File
    var destFile = new FileInfo(Path.Combine(Application.temporaryCachePath, "Bitmoji.glb"));

    // Automatically deflate Gzip response
    var handler = new HttpClientHandler();
    handler.AutomaticDecompression = System.Net.DecompressionMethods.Deflate | System.Net.DecompressionMethods.GZip;

    // Create request
    var httpClient = new HttpClient(handler);
    httpClient.DefaultRequestHeaders.Authorization = new AuthenticationHeaderValue("Bearer", LoginKit.GetAccessToken());    

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
    Debug.Log($"Downloaded Bitmoji! Avatar {avatarId} is saved to {destFile}.");

    return destFile.FullName;
}
```


# Providing Feedback			

We would love to hear your thoughts on our product! Please visit this form so that you can submit your suggestions and let us know how we can better improve your Bitmoji Gaming Experience! [Feedback Form](https://forms.gle/48xjwZPUazYGrBZu5) 

