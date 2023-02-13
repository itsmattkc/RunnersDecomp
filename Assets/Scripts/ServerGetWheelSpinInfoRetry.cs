using UnityEngine;

public class ServerGetWheelSpinInfoRetry : ServerRetryProcess
{
	public ServerGetWheelSpinInfoRetry(GameObject callbackObject)
		: base(callbackObject)
	{
	}

	public override void Retry()
	{
		ServerInterface loggedInServerInterface = ServerInterface.LoggedInServerInterface;
		if (loggedInServerInterface != null)
		{
			loggedInServerInterface.RequestServerGetWheelSpinInfo(m_callbackObject);
		}
	}
}
