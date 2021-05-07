using System;
using System.Collections;
using System.Collections.Generic;
using Snap;
using UnityEngine;

public class SnapKitHandler : MonoBehaviour
{
    public event Action OnUserDataFetched;

    public string AccessToken { get; private set; }
    public string AvatarId { get; private set; }

    private const string BITMOJI_SCOPE = "https://auth.snapchat.com/oauth2/api/user.bitmoji.avatar";

    void OnEnable()
    {
        LoginKit.OnLoginLinkDidSucceedEvent += OnLoginCompleted;
        LoginKit.OnFetchUserDataSucceededEvent += OnUserDataQuerySucceeded;
    }

    private void OnDisable()
    {
        LoginKit.OnLoginLinkDidSucceedEvent -= OnLoginCompleted;
        LoginKit.OnFetchUserDataSucceededEvent -= OnUserDataQuerySucceeded;
    }

    public void StartLogin()
    {
        if (LoginKit.HasAccessToScope(BITMOJI_SCOPE))
        {
            this.OnLoginCompleted();
        }
        else
        {
            LoginKit.Login();
        }

    }

    private void OnLoginCompleted()
    {
        LoginKit.FetchUserDataWithQuery("{me{displayName, bitmoji{id,selfie}}}", null);
    }

    private void OnUserDataQuerySucceeded(string json)
    {
        this.AccessToken = LoginKit.GetAccessToken();

        if (Application.platform == RuntimePlatform.Android)
        {
            var response = JsonUtility.FromJson<SnapKitUserInfo>(json);
            this.AvatarId = response.bitmoji.id;

        }
        else if (Application.platform == RuntimePlatform.IPhonePlayer)
        {
            var response = JsonUtility.FromJson<SnapKitResponse>(json);
            this.AvatarId = response.data.me.bitmoji.id;

            OnUserDataFetched?.Invoke();
        }

        OnUserDataFetched?.Invoke();


    }
}
