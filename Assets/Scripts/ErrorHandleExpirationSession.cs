using Message;
using Text;
using UnityEngine;

public class ErrorHandleExpirationSession : ErrorHandleBase
{
	private enum State
	{
		NONE = -1,
		IDLE,
		VALIDATING,
		SUCCESS,
		FAILED,
		COUNT
	}

	private bool m_isEnd;

	private ServerRetryProcess m_retryProcess;

	private ServerSessionWatcher.ValidateType m_validateType;

	private State m_state;

	public override void Setup(GameObject callbackObject, string callbackFuncName, MessageBase msg)
	{
	}

	public void SetRetryProcess(ServerRetryProcess retryProcess)
	{
		m_retryProcess = retryProcess;
	}

	public void SetSessionValidateType(ServerSessionWatcher.ValidateType validateType)
	{
		m_validateType = validateType;
	}

	public override void StartErrorHandle()
	{
		if (m_state == State.IDLE)
		{
			m_state = State.VALIDATING;
			ServerSessionWatcher serverSessionWatcher = GameObjectUtil.FindGameObjectComponent<ServerSessionWatcher>("NetMonitor");
			if (serverSessionWatcher != null)
			{
				serverSessionWatcher.ValidateSession(m_validateType, ValidateSessionCallback);
			}
		}
	}

	public override void Update()
	{
		if (m_state == State.FAILED && NetworkErrorWindow.IsOkButtonPressed)
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
		if (m_state == State.SUCCESS && m_retryProcess != null)
		{
			m_retryProcess.Retry();
		}
	}

	private void ValidateSessionCallback(bool isSuccess)
	{
		if (isSuccess)
		{
			m_state = State.SUCCESS;
			m_isEnd = true;
			return;
		}
		NetworkErrorWindow.CInfo info = default(NetworkErrorWindow.CInfo);
		info.buttonType = NetworkErrorWindow.ButtonType.Ok;
		info.anchor_path = string.Empty;
		info.caption = TextManager.GetText(TextManager.TextType.TEXTTYPE_FIXATION_TEXT, "NetworkError", "ui_Lbl_session_timeout_caption").text;
		info.message = TextManager.GetText(TextManager.TextType.TEXTTYPE_FIXATION_TEXT, "NetworkError", "ui_Lbl_session_timeout_text").text;
		NetworkErrorWindow.Create(info);
		m_state = State.FAILED;
	}
}
