using System.Collections;
using System.Collections.Generic;
using System.IO;
using System.Threading.Tasks;
using Snap;
using UnityEngine;
using UnityEngine.UI;

public class CameraShareHandler : MonoBehaviour
{
    public Text DebugText;

    private void OnEnable()
    {
        CreativeKit.OnSendSucceededEvent += OnCreativeKitShareSucceeded;
        CreativeKit.OnSendFailedEvent += OnCreativeKitShareFailed;
    }

    private void OnDisable()
    {
        CreativeKit.OnSendSucceededEvent -= OnCreativeKitShareSucceeded;
        CreativeKit.OnSendFailedEvent -= OnCreativeKitShareFailed;

    }

    public void OnButtonTap_CameraShare()
    {
        if (Application.isEditor)
        {
            DebugText.text = "Can't share from Editor. Build to a mobile device to test CreativeKit share";
        }
        else
        {
            var sticker = new Sticker("sticker.png")
            {
                Height = 300,
                Width = 300,
                PosX = 0f,
                PosY = 0f,
                RotationDegreesClockwise = 0f
            };
            var shareContent = new ShareContent(ShareKind.NoSnap)
            {
                Sticker = sticker
            };
            CreativeKit.Share(shareContent);
        }
        
    }

    private void OnCreativeKitShareSucceeded()
    {
        DebugText.text = "Creative Kit Share Succeeded";
        Debug.Log("Creative Kit Share succeeded");
    }

    private void OnCreativeKitShareFailed(string obj)
    {
        DebugText.text = "Creative Kit Share Failed";
        Debug.Log("Creative Kit Share failed");
    }
}