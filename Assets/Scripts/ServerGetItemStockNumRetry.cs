using System.Collections.Generic;
using UnityEngine;

public class ServerGetItemStockNumRetry : ServerRetryProcess
{
	private int m_eventId;

	private List<int> m_itemId;

	public ServerGetItemStockNumRetry(int eventId, List<int> itemId, GameObject callbackObject)
		: base(callbackObject)
	{
		m_eventId = eventId;
		m_itemId = itemId;
	}

	public override void Retry()
	{
		ServerInterface loggedInServerInterface = ServerInterface.LoggedInServerInterface;
		if (loggedInServerInterface != null)
		{
			loggedInServerInterface.RequestServerGetItemStockNum(m_eventId, m_itemId, m_callbackObject);
		}
	}
}
