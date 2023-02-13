using UnityEngine;

public class ServerGetChaoStateRetry : ServerRetryProcess
{
	public ServerGetChaoStateRetry(GameObject callbackObject)
		: base(callbackObject)
	{
	}

	public override void Retry()
	{
		ServerInterface loggedInServerInterface = ServerInterface.LoggedInServerInterface;
		if (loggedInServerInterface != null)
		{
			loggedInServerInterface.RequestServerGetChaoState(m_callbackObject);
		}
	}
}
