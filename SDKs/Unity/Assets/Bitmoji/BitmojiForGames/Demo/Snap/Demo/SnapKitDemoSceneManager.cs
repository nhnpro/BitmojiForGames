using System.Collections;
using System.Collections.Generic;
using Snap;
using UnityEngine;
using UnityEngine.UI;
using System.Linq;

public class SnapKitDemoSceneManager : MonoBehaviour
{
	[Header("UI")]
	public Text statusText;

	[Header("Verify")]
	public string verifyPhoneNumber;
	public CountryCodes verifyPhoneRegion;

    #region GameObject

    void OnEnable()
	{
		LoginKit.OnLoginCompletedEvent += OnLoginCompletedEvent;
		LoginKit.OnLoginLinkDidStartEvent += OnLoginLinkDidStartEvent;
		LoginKit.OnLoginLinkDidSucceedEvent += OnLoginLinkDidSucceedEvent;
		LoginKit.OnLoginLinkDidFailEvent += OnLoginLinkDidFailEvent;
		LoginKit.OnLoginDidUnlinkEvent += OnLoginDidUnlinkEvent;
		LoginKit.OnFetchUserDataSucceededEvent += OnFetchUserDataSucceededEvent;
		LoginKit.OnFetchUserDataFailedEvent += OnFetchUserDataFailedEvent;
		LoginKit.OnVerifySucceededEvent += OnVerifySucceededEvent;
		LoginKit.OnVerifyFailedEvent += OnVerifyFailedEvent;

		CreativeKit.OnSendSucceededEvent += OnSendSucceededEvent;
		CreativeKit.OnSendFailedEvent += OnSendFailedEvent;
	}


	void OnDisable()
	{
		LoginKit.OnLoginCompletedEvent -= OnLoginCompletedEvent;
		LoginKit.OnLoginLinkDidStartEvent -= OnLoginLinkDidStartEvent;
		LoginKit.OnLoginLinkDidSucceedEvent -= OnLoginLinkDidSucceedEvent;
		LoginKit.OnLoginLinkDidFailEvent -= OnLoginLinkDidFailEvent;
		LoginKit.OnLoginDidUnlinkEvent -= OnLoginDidUnlinkEvent;
		LoginKit.OnFetchUserDataSucceededEvent -= OnFetchUserDataSucceededEvent;
		LoginKit.OnFetchUserDataFailedEvent -= OnFetchUserDataFailedEvent;
		LoginKit.OnVerifySucceededEvent -= OnVerifySucceededEvent;
		LoginKit.OnVerifyFailedEvent -= OnVerifyFailedEvent;

		CreativeKit.OnSendSucceededEvent -= OnSendSucceededEvent;
		CreativeKit.OnSendFailedEvent -= OnSendFailedEvent;
	}

    private void OnValidate()
    {
		var numbersOnly = new string(verifyPhoneNumber.Where(char.IsDigit).ToArray());
		if (numbersOnly.Length < this.verifyPhoneNumber.Length)
        {
			Debug.LogWarning("[Snap Verify] Please use only numbers for your Verify phone number");
			this.verifyPhoneNumber = new string(verifyPhoneNumber.Where(char.IsDigit).ToArray());
		}
	}

	#endregion

	#region Button Handlers
	public void OnButtonTapped_Login()
    {
		LoginKit.Login();
    }

	public void OnButtonTapped_Logout()
	{
		LoginKit.UnlinkAllSessions();
	}

	public void OnButtonTapped_Verify()
	{
		if (!string.IsNullOrEmpty(this.verifyPhoneNumber))
        {
			LoginKit.Verify(this.verifyPhoneNumber, this.verifyPhoneRegion.ToString());
		}
		else
        {
			statusText.text = "Missing Verify options in SnapKitDemoSceneManager";
		}
		
	}	

	public void OnButtonTapped_IsLoggedIn()
	{
		statusText.text = "Is Logged in? " + LoginKit.IsLoggedIn();
	}

	public void OnButtonTapped_FetchUserScopes()
	{
		LoginKit.FetchUserDataWithQuery("{me{displayName, bitmoji{avatar}}}", null);
	}

	public void OnButtonTapped_ShareToCamera()
	{
		var content = new ShareContent(ShareKind.NoSnap);
		content.Sticker = GetSticker();

		CreativeKit.Share(content);
	}

	public void OnButtonTapped_SharePhoto()
	{
		var content = new ShareContent(ShareKind.Photo, "photo.png");
		CreativeKit.Share(content);
	}

	public void OnButtonTapped_ShareWithOptions()
	{
		var content = new ShareContent(ShareKind.Photo, "photo.png")
		{
			CaptionText = "friends having fun",
			AttachmentUrl = "https://snap.com",
		};
		content.Sticker = GetSticker();

		CreativeKit.Share(content);
	}


	#endregion

	#region Event Handlers
	void OnLoginDidUnlinkEvent()
	{
		statusText.text = "Logout successful";
	}

	void OnLoginLinkDidFailEvent()
	{
		statusText.text = "Login Failed. Please check your app settings";
	}

	void OnLoginLinkDidSucceedEvent()
	{
		statusText.text = "Login successful. Try fetching user data";
	}

	void OnLoginLinkDidStartEvent()
	{
		statusText.text = "Login started. Waiting for response";
	}

	void OnLoginCompletedEvent(string errorOrNull)
	{
		statusText.text = "Login completed";
	}

	void OnFetchUserDataSucceededEvent(string json)
	{
		statusText.text = "Fetch user data succeeded. JSON:" + json;
	}

	void OnFetchUserDataFailedEvent(string errorOrNull)
	{
		statusText.text = "Fetch user data failed. Check your app settings <br /> " + errorOrNull;
	}

	void OnSendSucceededEvent()
	{
		statusText.text = "Send succeeded";
	}

	void OnSendFailedEvent(string error)
	{
		statusText.text = "Send failed. Error: " + error;
	}

	void OnVerifySucceededEvent(Dictionary<string, object> obj)
	{
		statusText.text = "Verify succeeded " + JsonUtility.ToJson(obj);
	}

	void OnVerifyFailedEvent(string error)
	{
		statusText.text = "Verify failed. Error:" + error;
	}
	#endregion

	#region Helpers
	Sticker GetSticker()
	{
		return new Sticker("sticker.png")
		{
			PosX = 0.5f,
			PosY = 0.5f,
			Width = 200,
			Height = 200,
			RotationDegreesClockwise = UnityEngine.Random.Range(0, 360),
		};
	}

	#endregion
}

#region Verify Country Codes
public enum CountryCodes
{
	US,
	AD,
	AE,
	AF,
	AG,
	AI,
	AL,
	AM,
	AO,
	AQ,
	AR,
	AS,
	AT,
	AU,
	AW,
	AX,
	AZ,
	BA,
	BB,
	BD,
	BE,
	BF,
	BG,
	BH,
	BI,
	BJ,
	BL,
	BM,
	BN,
	BO,
	BQ,
	BR,
	BS,
	BT,
	BV,
	BW,
	BY,
	BZ,
	CA,
	CC,
	CD,
	CF,
	CG,
	CH,
	CI,
	CK,
	CL,
	CM,
	CN,
	CO,
	CR,
	CU,
	CV,
	CW,
	CX,
	CY,
	CZ,
	DE,
	DJ,
	DK,
	DM,
	DO,
	DZ,
	EC,
	EE,
	EG,
	ES,
	ET,
	FI,
	FJ,
	FK,
	FM,
	FO,
	FR,
	GA,
	GB,
	GD,
	GE,
	GF,
	GG,
	GH,
	GI,
	GL,
	GM,
	GN,
	GP,
	GQ,
	GR,
	GS,
	GT,
	GU,
	GW,
	GY,
	HK,
	HM,
	HN,
	HR,
	HT,
	HU,
	ID,
	IE,
	IL,
	IM,
	IN,
	IQ,
	IR,
	IS,
	IT,
	JE,
	JM,
	JO,
	JP,
	KE,
	KG,
	KH,
	KI,
	KM,
	KN,
	KP,
	KR,
	KW,
	KY,
	KZ,
	LA,
	LB,
	LC,
	LI,
	LK,
	LR,
	LS,
	LT,
	LU,
	LV,
	LY,
	MA,
	MC,
	MD,
	ME,
	MF,
	MG,
	MH,
	MK,
	ML,
	MM,
	MN,
	MO,
	MP,
	MQ,
	MR,
	MS,
	MT,
	MU,
	MV,
	MW,
	MX,
	MY,
	MZ,
	NA,
	NC,
	NE,
	NF,
	NG,
	NI,
	NL,
	NO,
	NP,
	NR,
	NU,
	NZ,
	OM,
	PA,
	PE,
	PF,
	PG,
	PH,
	PK,
	PL,
	PM,
	PN,
	PR,
	PS,
	PT,
	PW,
	PY,
	QA,
	RE,
	RO,
	RS,
	RU,
	RW,
	SA,
	SB,
	SC,
	SD,
	SE,
	SG,
	SH,
	SI,
	SJ,
	SK,
	SL,
	SM,
	SN,
	SO,
	SR,
	SS,
	ST,
	SV,
	SX,
	SY,
	SZ,
	TC,
	TD,
	TF,
	TG,
	TH,
	TJ,
	TK,
	TL,
	TM,
	TN,
	TO,
	TR,
	TT,
	TV,
	TW,
	TZ,
	UA,
	UG,
	UY,
	UZ,
	VA,
	VC,
	VE,
	VG,
	VI,
	VN,
	VU,
	WS,
	YE,
	YT,
	ZA,
	ZM,
	ZW,
}
#endregion
