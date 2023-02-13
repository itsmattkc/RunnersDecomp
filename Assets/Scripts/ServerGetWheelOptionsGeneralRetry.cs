using UnityEngine;

public class ServerGetWheelOptionsGeneralRetry : ServerRetryProcess
{
	private int m_spinId;

	private int m_eventId;

	public ServerGetWheelOptionsGeneralRetry(int eventId, int spinId, GameObject callbackObject)
		: base(callbackObject)
	{
		m_eventId = eventId;
		m_spinId = spinId;
	}

	public override void Retry()
	{
		ServerInterface loggedInServerInterface = ServerInterface.LoggedInServerInterface;
		if (loggedInServerInterface != null)
		{
			loggedInServerInterface.RequestServerGetWheelOptionsGeneral(m_eventId, m_spinId, m_callbackObject);
		}
	}
}
