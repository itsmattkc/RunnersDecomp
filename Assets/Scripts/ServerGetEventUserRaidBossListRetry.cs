using UnityEngine;

public class ServerGetEventUserRaidBossListRetry : ServerRetryProcess
{
	private int m_eventId;

	public ServerGetEventUserRaidBossListRetry(int eventId, GameObject callbackObject)
		: base(callbackObject)
	{
		m_eventId = eventId;
	}

	public override void Retry()
	{
		ServerInterface loggedInServerInterface = ServerInterface.LoggedInServerInterface;
		if (loggedInServerInterface != null)
		{
			loggedInServerInterface.RequestServerGetEventUserRaidBossList(m_eventId, m_callbackObject);
		}
	}
}
