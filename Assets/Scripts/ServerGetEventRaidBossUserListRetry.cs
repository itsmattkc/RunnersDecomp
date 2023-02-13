using UnityEngine;

public class ServerGetEventRaidBossUserListRetry : ServerRetryProcess
{
	private int m_eventId;

	private long m_raidBossId;

	public ServerGetEventRaidBossUserListRetry(int eventId, long raidBossId, GameObject callbackObject)
		: base(callbackObject)
	{
		m_eventId = eventId;
		m_raidBossId = raidBossId;
	}

	public override void Retry()
	{
		ServerInterface loggedInServerInterface = ServerInterface.LoggedInServerInterface;
		if (loggedInServerInterface != null)
		{
			loggedInServerInterface.RequestServerGetEventRaidBossUserList(m_eventId, m_raidBossId, m_callbackObject);
		}
	}
}
