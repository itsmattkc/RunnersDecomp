using System.Collections.Generic;
using UnityEngine;

public class ServerGetEventRaidBossDesiredListRetry : ServerRetryProcess
{
	public int m_eventId;

	public long m_raidBossId;

	public List<string> m_friendIdList;

	public ServerGetEventRaidBossDesiredListRetry(int eventId, long raidBossId, List<string> friendIdList, GameObject callbackObject)
		: base(callbackObject)
	{
		m_eventId = eventId;
		m_raidBossId = raidBossId;
		m_friendIdList = friendIdList;
	}

	public override void Retry()
	{
		ServerInterface loggedInServerInterface = ServerInterface.LoggedInServerInterface;
		if (loggedInServerInterface != null)
		{
			loggedInServerInterface.RequestServerGetEventRaidBossDesiredList(m_eventId, m_raidBossId, m_friendIdList, m_callbackObject);
		}
	}
}
