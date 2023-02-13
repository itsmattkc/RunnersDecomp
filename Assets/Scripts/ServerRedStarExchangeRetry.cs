using UnityEngine;

public class ServerRedStarExchangeRetry : ServerRetryProcess
{
	public int m_storeItemId;

	public ServerRedStarExchangeRetry(int storeItemId, GameObject callbackObject)
		: base(callbackObject)
	{
		m_storeItemId = storeItemId;
	}

	public override void Retry()
	{
		ServerInterface loggedInServerInterface = ServerInterface.LoggedInServerInterface;
		if (loggedInServerInterface != null)
		{
			loggedInServerInterface.RequestServerRedStarExchange(m_storeItemId, m_callbackObject);
		}
	}
}
