using UnityEngine;

public class ServerGetRingItemListRetry : ServerRetryProcess
{
	public ServerGetRingItemListRetry(GameObject callbackObject)
		: base(callbackObject)
	{
	}

	public override void Retry()
	{
		ServerInterface loggedInServerInterface = ServerInterface.LoggedInServerInterface;
		if (loggedInServerInterface != null)
		{
			loggedInServerInterface.RequestServerGetRingItemList(m_callbackObject);
		}
	}
}
