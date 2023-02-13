using UnityEngine;

public class ServerGetRingExchangeListRetry : ServerRetryProcess
{
	public ServerGetRingExchangeListRetry(GameObject callbackObject)
		: base(callbackObject)
	{
	}

	public override void Retry()
	{
		ServerInterface loggedInServerInterface = ServerInterface.LoggedInServerInterface;
		if (loggedInServerInterface != null)
		{
			loggedInServerInterface.RequestServerGetRingExchangeList(m_callbackObject);
		}
	}
}
