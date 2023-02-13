using Message;
using System.Collections.Generic;
using Text;
using UnityEngine;

public class ErrorHandleUnExpectedError : ErrorHandleBase
{
	private class TextInfo
	{
		private string m_captionId;

		private string m_messageId;

		public string CaptionId
		{
			get
			{
				return m_captionId;
			}
			private set
			{
			}
		}

		public string MessageId
		{
			get
			{
				return m_messageId;
			}
			private set
			{
			}
		}

		public TextInfo(string captionId, string messageId)
		{
			m_captionId = captionId;
			m_messageId = messageId;
		}
	}

	private bool m_isEnd;

	private MessageBase m_msg;

	private static readonly Dictionary<ServerInterface.StatusCode, TextInfo> ErrorTextPair = new Dictionary<ServerInterface.StatusCode, TextInfo>
	{
		{
			ServerInterface.StatusCode.PassWordError,
			new TextInfo("ui_Lbl_caption_local", "ui_Lbl_password_error")
		}
	};

	public override void Setup(GameObject callbackObject, string callbackFuncName, MessageBase msg)
	{
		m_msg = msg;
	}

	public override void StartErrorHandle()
	{
		string caption = string.Empty;
		string message = string.Empty;
		MsgServerConnctFailed msgServerConnctFailed = m_msg as MsgServerConnctFailed;
		if (msgServerConnctFailed != null)
		{
			TextInfo value = null;
			if (ErrorTextPair.TryGetValue(msgServerConnctFailed.m_status, out value))
			{
				caption = TextManager.GetText(TextManager.TextType.TEXTTYPE_FIXATION_TEXT, "NetworkError", value.CaptionId).text;
				message = TextManager.GetText(TextManager.TextType.TEXTTYPE_FIXATION_TEXT, "NetworkError", value.MessageId).text;
			}
			else
			{
				caption = TextManager.GetText(TextManager.TextType.TEXTTYPE_FIXATION_TEXT, "NetworkError", "ui_Lbl_caption").text;
				message = TextManager.GetText(TextManager.TextType.TEXTTYPE_FIXATION_TEXT, "NetworkError", "ui_Lbl_reset").text;
				string str = message;
				int status = (int)msgServerConnctFailed.m_status;
				message = str + status;
			}
		}
		NetworkErrorWindow.CInfo info = default(NetworkErrorWindow.CInfo);
		info.buttonType = NetworkErrorWindow.ButtonType.Ok;
		info.anchor_path = string.Empty;
		info.caption = caption;
		info.message = message;
		NetworkErrorWindow.Create(info);
	}

	public override void Update()
	{
		if (NetworkErrorWindow.IsOkButtonPressed)
		{
			NetworkErrorWindow.Close();
			HudMenuUtility.GoToTitleScene();
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
