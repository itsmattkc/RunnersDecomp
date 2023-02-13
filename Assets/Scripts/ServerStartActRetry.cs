using System.Collections.Generic;
using UnityEngine;

public class ServerStartActRetry : ServerRetryProcess
{
	public List<ItemType> m_modifiersItem;

	public List<BoostItemType> m_modifiersBoostItem;

	public List<string> m_distanceFriendIdList;

	public bool m_tutorial;

	private int? m_eventId;

	public ServerStartActRetry(List<ItemType> modifiersItem, List<BoostItemType> modifiersBoostItem, List<string> distanceFriendIdList, bool tutorial, int? eventId, GameObject callbackObject)
		: base(callbackObject)
	{
		m_modifiersItem = modifiersItem;
		m_modifiersBoostItem = modifiersBoostItem;
		m_distanceFriendIdList = distanceFriendIdList;
		m_tutorial = tutorial;
		m_eventId = eventId;
	}

	public override void Retry()
	{
		ServerInterface loggedInServerInterface = ServerInterface.LoggedInServerInterface;
		if (loggedInServerInterface != null)
		{
			loggedInServerInterface.RequestServerStartAct(m_modifiersItem, m_modifiersBoostItem, m_distanceFriendIdList, m_tutorial, m_eventId, m_callbackObject);
		}
	}
}
