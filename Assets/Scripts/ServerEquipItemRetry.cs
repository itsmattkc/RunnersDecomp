using System.Collections.Generic;
using UnityEngine;

public class ServerEquipItemRetry : ServerRetryProcess
{
	public List<ItemType> m_items;

	public ServerEquipItemRetry(List<ItemType> items, GameObject callbackObject)
		: base(callbackObject)
	{
		m_items = items;
	}

	public override void Retry()
	{
		ServerInterface loggedInServerInterface = ServerInterface.LoggedInServerInterface;
		if (loggedInServerInterface != null)
		{
			loggedInServerInterface.RequestServerEquipItem(m_items, m_callbackObject);
		}
	}
}
