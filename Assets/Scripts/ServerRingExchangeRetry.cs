using UnityEngine;

public class ServerRingExchangeRetry : ServerRetryProcess
{
	public int m_itemId;

	public int m_itemNum;

	public ServerRingExchangeRetry(int itemId, int itemNum, GameObject callbackObject)
		: base(callbackObject)
	{
		m_itemId = itemId;
		m_itemNum = itemNum;
	}

	public override void Retry()
	{
		ServerInterface loggedInServerInterface = ServerInterface.LoggedInServerInterface;
		if (loggedInServerInterface != null)
		{
			loggedInServerInterface.RequestServerRingExchange(m_itemId, m_itemNum, m_callbackObject);
		}
	}
}
