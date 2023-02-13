using Message;
using Text;
using UnityEngine;

public class ErrorHandleAskGiveUpRetry : ErrorHandleBase
{
	private enum State
	{
		NONE = -1,
		INIT,
		ASK_GIVEUP,
		CONFIRMATION,
		END,
		COUNT
	}

	private State m_state = State.NONE;

	private ServerRetryProcess m_retryProcess;

	private GameObject m_callbackObject;

	private string m_callbackFuncName;

	private MessageBase m_msg;

	private bool m_isRetry;

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
			m_state = State.END;
			return;
		}
		m_isRetry = false;
		m_state = State.INIT;
	}

	public override void Update()
	{
		switch (m_state)
		{
		case State.END:
			break;
		case State.INIT:
			if (m_msg.ID == 61517)
			{
				MsgServerConnctFailed msgServerConnctFailed = m_msg as MsgServerConnctFailed;
				if (msgServerConnctFailed != null)
				{
					string empty = string.Empty;
					switch (msgServerConnctFailed.m_status)
					{
					case ServerInterface.StatusCode.NotReachability:
					case ServerInterface.StatusCode.TimeOut:
						empty = TextManager.GetText(TextManager.TextType.TEXTTYPE_FIXATION_TEXT, "NetworkError", "ui_Lbl_ask_to_giveup_retry").text;
						break;
					default:
						empty = string.Empty;
						break;
					}
					NetworkErrorWindow.CInfo info = default(NetworkErrorWindow.CInfo);
					info.name = "NetworkErrorRetry";
					info.buttonType = NetworkErrorWindow.ButtonType.YesNo;
					info.anchor_path = string.Empty;
					info.caption = TextManager.GetText(TextManager.TextType.TEXTTYPE_FIXATION_TEXT, "NetworkError", "ui_Lbl_caption").text;
					info.finishedCloseDelegate = OnFinishedCloseAnimCallback;
					info.message = empty;
					NetworkErrorWindow.Create(info);
				}
				m_state = State.ASK_GIVEUP;
			}
			else if (m_msg.ID == 61520)
			{
				NetworkErrorWindow.CInfo info2 = default(NetworkErrorWindow.CInfo);
				info2.name = "NetworkErrorRetry";
				info2.buttonType = NetworkErrorWindow.ButtonType.YesNo;
				info2.anchor_path = string.Empty;
				info2.caption = TextManager.GetText(TextManager.TextType.TEXTTYPE_FIXATION_TEXT, "NetworkError", "ui_Lbl_caption").text;
				info2.message = TextManager.GetText(TextManager.TextType.TEXTTYPE_FIXATION_TEXT, "NetworkError", "ui_Lbl_ask_to_giveup_retry").text;
				info2.finishedCloseDelegate = OnFinishedCloseAnimCallback;
				NetworkErrorWindow.Create(info2);
				m_state = State.ASK_GIVEUP;
			}
			else
			{
				m_state = State.END;
			}
			break;
		case State.ASK_GIVEUP:
			if (!NetworkErrorWindow.IsYesButtonPressed && NetworkErrorWindow.IsNoButtonPressed)
			{
				NetworkErrorWindow.Close();
				NetworkErrorWindow.CInfo info3 = default(NetworkErrorWindow.CInfo);
				info3.buttonType = NetworkErrorWindow.ButtonType.YesNo;
				info3.anchor_path = string.Empty;
				info3.caption = TextManager.GetText(TextManager.TextType.TEXTTYPE_FIXATION_TEXT, "NetworkError", "ui_Lbl_caption").text;
				info3.message = TextManager.GetText(TextManager.TextType.TEXTTYPE_FIXATION_TEXT, "NetworkError", "ui_Lbl_giveup_confirmation").text;
				NetworkErrorWindow.Create(info3);
				m_state = State.CONFIRMATION;
			}
			break;
		case State.CONFIRMATION:
			if (NetworkErrorWindow.IsYesButtonPressed)
			{
				NetworkErrorWindow.Close();
				if (m_callbackObject != null)
				{
					m_callbackObject.SendMessage(m_callbackFuncName, m_msg, SendMessageOptions.DontRequireReceiver);
				}
				GameObjectUtil.SendMessageFindGameObject("NetMonitor", "OnResetConnectFailedCount", null, SendMessageOptions.DontRequireReceiver);
				HudMenuUtility.GoToTitleScene();
				m_state = State.END;
			}
			else if (NetworkErrorWindow.IsNoButtonPressed)
			{
				NetworkErrorWindow.Close();
				m_state = State.INIT;
			}
			break;
		}
	}

	public override bool IsEnd()
	{
		if (m_state == State.END)
		{
			return true;
		}
		return false;
	}

	public override void EndErrorHandle()
	{
		NetworkErrorWindow.Close();
		if (m_isRetry && m_retryProcess != null)
		{
			m_retryProcess.Retry();
		}
	}

	private void OnFinishedCloseAnimCallback()
	{
		if (m_state == State.ASK_GIVEUP && NetworkErrorWindow.IsYesButtonPressed)
		{
			if (m_callbackObject != null)
			{
				m_callbackObject.SendMessage(m_callbackFuncName, m_msg, SendMessageOptions.DontRequireReceiver);
			}
			m_isRetry = true;
			m_state = State.END;
		}
	}
}
