using Message;
using Text;
using UnityEngine;

public class ErrorHandleVersionDifference : ErrorHandleBase
{
	private bool m_isEnd;

	public override void Setup(GameObject callbackObject, string callbackFuncName, MessageBase msg)
	{
	}

	public override void StartErrorHandle()
	{
		NetworkErrorWindow.CInfo info = default(NetworkErrorWindow.CInfo);
		info.buttonType = NetworkErrorWindow.ButtonType.Ok;
		info.anchor_path = string.Empty;
		info.caption = TextManager.GetText(TextManager.TextType.TEXTTYPE_FIXATION_TEXT, "NetworkError", "ui_Lbl_caption").text;
		#if UNITY_IOS // Make sure the iOS version is not telling the user to update using the Google Play Store
		info.message = TextManager.GetText(TextManager.TextType.TEXTTYPE_FIXATION_TEXT, "NetworkError", "ui_Lbl_update_version_ios").text;
		#endif
		info.message = TextManager.GetText(TextManager.TextType.TEXTTYPE_FIXATION_TEXT, "NetworkError", "ui_Lbl_update_version_android").text;
		NetworkErrorWindow.Create(info);
	}

	public override void Update()
	{
		if (NetworkErrorWindow.IsOkButtonPressed)
		{
			NetworkErrorWindow.Close();
			HudMenuUtility.GoToTitleScene();
			string redirectInstallPageUrl = NetBaseUtil.RedirectInstallPageUrl;
			if (!string.IsNullOrEmpty(redirectInstallPageUrl))
			{
				Application.OpenURL(redirectInstallPageUrl);
			}
			m_isEnd = true;
		}
	}

	public override bool IsEnd()
	{
		return m_isEnd;
	}

	public override void EndErrorHandle()
	{
	}
}
