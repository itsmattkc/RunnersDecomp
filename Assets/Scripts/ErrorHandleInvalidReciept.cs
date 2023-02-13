using Message;
using Text;
using UnityEngine;

public class ErrorHandleInvalidReciept : ErrorHandleBase
{
	private bool m_isEnd;

	private GameObject m_callbackObject;

	private string m_callbackFuncName;

	private MessageBase m_msg;

	public override void Setup(GameObject callbackObject, string callbackFuncName, MessageBase msg)
	{
		m_callbackObject = callbackObject;
		m_callbackFuncName = callbackFuncName;
		m_msg = msg;
	}

	public override void StartErrorHandle()
	{
		NetworkErrorWindow.CInfo info = default(NetworkErrorWindow.CInfo);
		info.buttonType = NetworkErrorWindow.ButtonType.Ok;
		info.anchor_path = string.Empty;
		info.caption = TextManager.GetText(TextManager.TextType.TEXTTYPE_FIXATION_TEXT, "NetworkError", "ui_Lbl_caption").text;
		info.message = TextManager.GetText(TextManager.TextType.TEXTTYPE_FIXATION_TEXT, "NetworkError", "ui_Lbl_invalid_receipt").text;
		NetworkErrorWindow.Create(info);
	}

	public override void Update()
	{
		if (NetworkErrorWindow.IsOkButtonPressed)
		{
			NetworkErrorWindow.Close();
			if (m_callbackObject != null)
			{
				m_callbackObject.SendMessage(m_callbackFuncName, m_msg, SendMessageOptions.DontRequireReceiver);
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
