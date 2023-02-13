using Message;
using UnityEngine;

public class ErrorHandleSecurityError : ErrorHandleBase
{
	private bool m_isEnd;

	private ServerRetryProcess m_retryProcess;

	public void SetRetryProcess(ServerRetryProcess retryProcess)
	{
		m_retryProcess = retryProcess;
	}

	public override void Setup(GameObject callbackObject, string callbackFuncName, MessageBase msg)
	{
	}

	public override void StartErrorHandle()
	{
		ServerSessionWatcher serverSessionWatcher = GameObjectUtil.FindGameObjectComponent<ServerSessionWatcher>("NetMonitor");
		if (serverSessionWatcher != null)
		{
			serverSessionWatcher.InvalidateSession();
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
		if (m_retryProcess != null)
		{
			m_retryProcess.Retry();
		}
	}

	private void ValidateSessionEndCallback(bool isSuccess)
	{
		m_isEnd = true;
	}
}
