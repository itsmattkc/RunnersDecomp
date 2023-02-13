using UnityEngine;

public class ServerGetEventStateRetry : ServerRetryProcess
{
	private int m_eventId;

	public ServerGetEventStateRetry(int eventId, GameObject callbackObject)
		: base(callbackObject)
	{
		m_eventId = eventId;
	}

	public override void Retry()
	{
		ServerInterface loggedInServerInterface = ServerInterface.LoggedInServerInterface;
		if (loggedInServerInterface != null)
		{
			loggedInServerInterface.RequestServerGetEventState(m_eventId, m_callbackObject);
		}
	}
}
