using UnityEngine;

public class ServerGetFreeItemListRetry : ServerRetryProcess
{
	public ServerGetFreeItemListRetry(GameObject callbackObject)
		: base(callbackObject)
	{
	}

	public override void Retry()
	{
		ServerInterface loggedInServerInterface = ServerInterface.LoggedInServerInterface;
		if (loggedInServerInterface != null)
		{
			loggedInServerInterface.RequestServerGetFreeItemList(m_callbackObject);
		}
	}
}
