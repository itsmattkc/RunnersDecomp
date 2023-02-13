using UnityEngine;

public class ServerGetPrizeChaoWheelSpinRetry : ServerRetryProcess
{
	private int m_chaoWheelSpinType;

	public ServerGetPrizeChaoWheelSpinRetry(int chaoWheelSpinType, GameObject callbackObject)
		: base(callbackObject)
	{
		m_chaoWheelSpinType = chaoWheelSpinType;
	}

	public override void Retry()
	{
		ServerInterface loggedInServerInterface = ServerInterface.LoggedInServerInterface;
		if (loggedInServerInterface != null)
		{
			loggedInServerInterface.RequestServerGetPrizeChaoWheelSpin(m_chaoWheelSpinType, m_callbackObject);
		}
	}
}
