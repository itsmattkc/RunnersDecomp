using UnityEngine;

public class ServerGetRedStarExchangeListRetry : ServerRetryProcess
{
	public int m_itemType;

	public ServerGetRedStarExchangeListRetry(int itemType, GameObject callbackObject)
		: base(callbackObject)
	{
		m_itemType = itemType;
	}

	public override void Retry()
	{
		ServerInterface loggedInServerInterface = ServerInterface.LoggedInServerInterface;
		if (loggedInServerInterface != null)
		{
			loggedInServerInterface.RequestServerGetRedStarExchangeList(m_itemType, m_callbackObject);
		}
	}
}
