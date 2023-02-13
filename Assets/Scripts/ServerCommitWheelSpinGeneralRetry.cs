using UnityEngine;

public class ServerCommitWheelSpinGeneralRetry : ServerRetryProcess
{
	private int m_eventId;

	private int m_spinId;

	private int m_spinCostItemId;

	private int m_spinNum;

	public ServerCommitWheelSpinGeneralRetry(int eventId, int spinId, int spinCostItemId, int spinNum, GameObject callbackObject)
		: base(callbackObject)
	{
		m_eventId = eventId;
		m_spinId = spinId;
		m_spinCostItemId = spinCostItemId;
		m_spinNum = spinNum;
	}

	public override void Retry()
	{
		ServerInterface loggedInServerInterface = ServerInterface.LoggedInServerInterface;
		if (loggedInServerInterface != null)
		{
			loggedInServerInterface.RequestServerCommitWheelSpinGeneral(m_eventId, m_spinId, m_spinCostItemId, m_spinNum, m_callbackObject);
		}
	}
}
