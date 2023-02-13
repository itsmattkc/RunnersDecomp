using App;
using Message;
using SaveData;
using Text;
using UnityEngine;

public class ErrorHandleServerNextVersion : ErrorHandleBase
{
	private int m_buyRSRNum;

	private int m_freeRSRNum;

	private string m_userId = string.Empty;

	private string m_userName = string.Empty;

	private bool m_titleBack;

	private bool m_isEnd;

	private bool IsRegionJapan()
	{
		if (RegionManager.Instance != null)
		{
			return RegionManager.Instance.IsJapan();
		}
		return false;
	}

	private string GetReplaceText(string srcText, string tag, string replace)
	{
		if (!string.IsNullOrEmpty(srcText) && !string.IsNullOrEmpty(tag) && !string.IsNullOrEmpty(replace))
		{
			return TextUtility.Replace(srcText, tag, replace);
		}
		return srcText;
	}

	public override void Setup(GameObject callbackObject, string callbackFuncName, MessageBase msg)
	{
	}

	public override void StartErrorHandle()
	{
		NetworkErrorWindow.CInfo info = default(NetworkErrorWindow.CInfo);
		info.anchor_path = string.Empty;
		m_titleBack = GameModeTitle.Logined;
		if (m_titleBack)
		{
			info.buttonType = NetworkErrorWindow.ButtonType.Ok;
			info.caption = TextUtility.GetText(TextManager.TextType.TEXTTYPE_FIXATION_TEXT, "NetworkError", "ui_Lbl_caption");
			info.message = TextUtility.GetText(TextManager.TextType.TEXTTYPE_FIXATION_TEXT, "NetworkError", "ui_Lbl_reset");
			info.message += -19990;
		}
		else
		{
			m_userId = SystemSaveManager.GetGameID();
			info.caption = TextUtility.GetText(TextManager.TextType.TEXTTYPE_FIXATION_TEXT, "NetworkError", "ui_Lbl_notification_caption");
			string text = (Env.language != 0) ? ServerInterface.NextVersionState.m_eMsg : ServerInterface.NextVersionState.m_jMsg;
			if (m_userId != "0" && IsRegionJapan())
			{
				m_buyRSRNum = ServerInterface.NextVersionState.m_buyRSRNum;
				m_freeRSRNum = ServerInterface.NextVersionState.m_freeRSRNum;
				m_userName = ServerInterface.NextVersionState.m_userName;
				if (string.IsNullOrEmpty(m_userName))
				{
					ServerSettingState settingState = ServerInterface.SettingState;
					if (settingState != null)
					{
						m_userName = settingState.m_userName;
					}
					if (string.IsNullOrEmpty(m_userName))
					{
						m_userName = " ";
					}
				}
				info.buttonType = NetworkErrorWindow.ButtonType.Repayment;
				string text2 = TextUtility.GetText(TextManager.TextType.TEXTTYPE_FIXATION_TEXT, "NetworkError", "ui_Lbl_user_info_text");
				text2 = GetReplaceText(text2, "{PARAM1}", m_userName);
				text2 = GetReplaceText(text2, "{PARAM2}", m_userId);
				text2 = GetReplaceText(text2, "{PARAM3}", m_buyRSRNum.ToString());
				text2 = GetReplaceText(text2, "{PARAM4}", m_freeRSRNum.ToString());
				text = text + "\n" + text2;
			}
			else
			{
				info.buttonType = NetworkErrorWindow.ButtonType.TextOnly;
			}
			info.message = text;
		}
		NetworkErrorWindow.Create(info);
		m_isEnd = false;
	}

	public override void Update()
	{
		if (m_titleBack)
		{
			if (NetworkErrorWindow.IsOkButtonPressed)
			{
				NetworkErrorWindow.Close();
				HudMenuUtility.GoToTitleScene();
				m_isEnd = true;
			}
		}
		else if (NetworkErrorWindow.IsButtonPressed)
		{
			NetworkErrorWindow.ResetButton();
			string url = ServerInterface.NextVersionState.m_url;
			if (!string.IsNullOrEmpty(url))
			{
				Application.OpenURL(url);
			}
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
