using UnityEngine;

public class ServerDrawRaidBossRetry : ServerRetryProcess
{
	private int m_eventId;

	private long m_score;

	public ServerDrawRaidBossRetry(int eventId, long score, GameObject callbackObject)
		: base(callbackObject)
	{
		m_eventId = eventId;
		m_score = score;
	}

	public override void Retry()
	{
		ServerInterface loggedInServerInterface = ServerInterface.LoggedInServerInterface;
		if (loggedInServerInterface != null)
		{
			loggedInServerInterface.RequestServerDrawRaidBoss(m_eventId, m_score, m_callbackObject);
		}
	}
}
