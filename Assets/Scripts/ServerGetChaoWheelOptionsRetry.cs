using UnityEngine;

public class ServerGetChaoWheelOptionsRetry : ServerRetryProcess
{
	public ServerGetChaoWheelOptionsRetry(GameObject callbackObject)
		: base(callbackObject)
	{
	}

	public override void Retry()
	{
		ServerInterface loggedInServerInterface = ServerInterface.LoggedInServerInterface;
		if (loggedInServerInterface != null)
		{
			loggedInServerInterface.RequestServerGetChaoWheelOptions(m_callbackObject);
		}
	}
}
