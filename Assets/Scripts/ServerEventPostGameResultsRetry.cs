using UnityEngine;

public class ServerEventPostGameResultsRetry : ServerRetryProcess
{
	public int m_eventId;

	public int m_numRaidbossRings;

	public ServerEventPostGameResultsRetry(int eventId, int numRaidbossRings, GameObject callbackObject)
		: base(callbackObject)
	{
		m_eventId = eventId;
		m_numRaidbossRings = numRaidbossRings;
	}

	public override void Retry()
	{
		ServerInterface loggedInServerInterface = ServerInterface.LoggedInServerInterface;
		if (loggedInServerInterface != null)
		{
			loggedInServerInterface.RequestServerEventPostGameResults(m_eventId, m_numRaidbossRings, m_callbackObject);
		}
	}
}
