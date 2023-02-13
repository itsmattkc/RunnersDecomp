using UnityEngine;

public class ServerGetCostListRetry : ServerRetryProcess
{
	public ServerGetCostListRetry(GameObject callbackObject)
		: base(callbackObject)
	{
	}

	public override void Retry()
	{
		ServerInterface loggedInServerInterface = ServerInterface.LoggedInServerInterface;
		if (loggedInServerInterface != null)
		{
			loggedInServerInterface.RequestServerGetCostList(m_callbackObject);
		}
	}
}
