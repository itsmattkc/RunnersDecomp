using UnityEngine;

public abstract class ServerRetryProcess
{
	protected GameObject m_callbackObject;

	public ServerRetryProcess(GameObject callbackObject)
	{
		m_callbackObject = callbackObject;
	}

	public abstract void Retry();
}
