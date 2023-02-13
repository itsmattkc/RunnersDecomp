using Message;
using Text;
using UnityEngine;

public class ErrorHandleRetry : ErrorHandleBase
{
	private ServerRetryProcess m_retryProcess;

	private GameObject m_callbackObject;

	private string m_callbackFuncName;

	private MessageBase m_msg;

	private bool m_isEnd;

	public void SetRetryProcess(ServerRetryProcess retryProcess)
	{
		m_retryProcess = retryProcess;
	}

	public override void Setup(GameObject callbackObject, string callbackFuncName, MessageBase msg)
	{
		m_callbackObject = callbackObject;
		m_callbackFuncName = callbackFuncName;
		m_msg = msg;
	}

	public override void StartErrorHandle()
	{
		if (m_msg == null)
		{
			m_isEnd = true;
		}
		else if (m_msg.ID == 61517)
		{
			MsgServerConnctFailed msgServerConnctFailed = m_msg as MsgServerConnctFailed;
			if (msgServerConnctFailed != null)
			{
				string empty = string.Empty;
				switch (msgServerConnctFailed.m_status)
				{
				case ServerInterface.StatusCode.ExpirationSession:
					empty = TextManager.GetText(TextManager.TextType.TEXTTYPE_FIXATION_TEXT, "NetworkError", "ui_Lbl_timeout").text;
					break;
				case ServerInterface.StatusCode.NotReachability:
				case ServerInterface.StatusCode.TimeOut:
					empty = TextManager.GetText(TextManager.TextType.TEXTTYPE_FIXATION_TEXT, "NetworkError", "ui_Lbl_reload").text;
					break;
				default:
					empty = string.Empty;
					break;
				}
				NetworkErrorWindow.CInfo info = default(NetworkErrorWindow.CInfo);
				info.name = "NetworkErrorReload";
				info.buttonType = NetworkErrorWindow.ButtonType.Ok;
				info.anchor_path = string.Empty;
				info.caption = TextManager.GetText(TextManager.TextType.TEXTTYPE_FIXATION_TEXT, "NetworkError", "ui_Lbl_caption").text;
				info.message = empty;
				info.finishedCloseDelegate = OnFinishCloseAnimCallback;
				NetworkErrorWindow.Create(info);
			}
		}
		else if (m_msg.ID == 61520)
		{
			NetworkErrorWindow.CInfo info2 = default(NetworkErrorWindow.CInfo);
			info2.name = "NetworkErrorReload";
			info2.buttonType = NetworkErrorWindow.ButtonType.Ok;
			info2.anchor_path = string.Empty;
			info2.caption = TextManager.GetText(TextManager.TextType.TEXTTYPE_FIXATION_TEXT, "NetworkError", "ui_Lbl_caption").text;
			info2.message = TextManager.GetText(TextManager.TextType.TEXTTYPE_FIXATION_TEXT, "NetworkError", "ui_Lbl_reload").text;
			info2.finishedCloseDelegate = OnFinishCloseAnimCallback;
			NetworkErrorWindow.Create(info2);
		}
		else
		{
			m_isEnd = true;
		}
	}

	public override void Update()
	{
	}

	public override bool IsEnd()
	{
		return m_isEnd;
	}

	public override void EndErrorHandle()
	{
		NetworkErrorWindow.Close();
		if (m_retryProcess != null)
		{
			m_retryProcess.Retry();
		}
	}

	private void OnFinishCloseAnimCallback()
	{
		if (m_callbackObject != null)
		{
			m_callbackObject.SendMessage(m_callbackFuncName, m_msg, SendMessageOptions.DontRequireReceiver);
		}
		m_isEnd = true;
	}
}
