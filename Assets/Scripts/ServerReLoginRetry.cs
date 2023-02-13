using UnityEngine;

public class ServerReLoginRetry : ServerRetryProcess
{
	public ServerReLoginRetry(GameObject callbackObject)
		: base(callbackObject)
	{
	}

	public override void Retry()
	{
		ServerInterface loggedInServerInterface = ServerInterface.LoggedInServerInterface;
		if (loggedInServerInterface != null)
		{
			loggedInServerInterface.RequestServerReLogin(m_callbackObject);
		}
	}
}
