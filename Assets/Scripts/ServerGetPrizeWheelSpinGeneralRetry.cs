using UnityEngine;

public class ServerGetPrizeWheelSpinGeneralRetry : ServerRetryProcess
{
	private int m_spinType;

	private int m_eventId;

	public ServerGetPrizeWheelSpinGeneralRetry(int eventId, int spinType, GameObject callbackObject)
		: base(callbackObject)
	{
		m_eventId = eventId;
		m_spinType = spinType;
	}

	public override void Retry()
	{
		ServerInterface loggedInServerInterface = ServerInterface.LoggedInServerInterface;
		if (loggedInServerInterface != null)
		{
			loggedInServerInterface.RequestServerGetPrizeWheelSpinGeneral(m_eventId, m_spinType, m_callbackObject);
		}
	}
}
