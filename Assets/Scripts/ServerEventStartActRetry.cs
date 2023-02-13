using System.Collections.Generic;
using UnityEngine;

public class ServerEventStartActRetry : ServerRetryProcess
{
	public int m_eventId;

	public int m_energyExpend;

	public long m_raidBossId;

	public List<ItemType> m_modifiersItem;

	public List<BoostItemType> m_modifiersBoostItem;

	public ServerEventStartActRetry(int eventId, int energyExpend, long raidBossId, List<ItemType> modifiersItem, List<BoostItemType> modifiersBoostItem, GameObject callbackObject)
		: base(callbackObject)
	{
		m_eventId = eventId;
		m_energyExpend = energyExpend;
		m_raidBossId = raidBossId;
		m_modifiersItem = modifiersItem;
		m_modifiersBoostItem = modifiersBoostItem;
	}

	public override void Retry()
	{
		ServerInterface loggedInServerInterface = ServerInterface.LoggedInServerInterface;
		if (loggedInServerInterface != null)
		{
			loggedInServerInterface.RequestServerEventStartAct(m_eventId, m_energyExpend, m_raidBossId, m_modifiersItem, m_modifiersBoostItem, m_callbackObject);
		}
	}
}
