using System.Collections.Generic;
using UnityEngine;

public class ServerQuickModeStartActRetry : ServerRetryProcess
{
	public List<ItemType> m_modifiersItem;

	public List<BoostItemType> m_modifiersBoostItem;

	public List<string> m_distanceFriendIdList;

	public bool m_tutorial;

	public ServerQuickModeStartActRetry(List<ItemType> modifiersItem, List<BoostItemType> modifiersBoostItem, bool tutorial, GameObject callbackObject)
		: base(callbackObject)
	{
		m_modifiersItem = modifiersItem;
		m_modifiersBoostItem = modifiersBoostItem;
		m_tutorial = tutorial;
	}

	public override void Retry()
	{
		ServerInterface loggedInServerInterface = ServerInterface.LoggedInServerInterface;
		if (loggedInServerInterface != null)
		{
			loggedInServerInterface.RequestServerQuickModeStartAct(m_modifiersItem, m_modifiersBoostItem, m_tutorial, m_callbackObject);
		}
	}
}
