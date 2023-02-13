using UnityEngine;

public class ServerRequestEnergyRetry : ServerRetryProcess
{
	public string m_friendId;

	public ServerRequestEnergyRetry(string friendId, GameObject callbackObject)
		: base(callbackObject)
	{
		m_friendId = friendId;
	}

	public override void Retry()
	{
		ServerInterface loggedInServerInterface = ServerInterface.LoggedInServerInterface;
		if (loggedInServerInterface != null)
		{
			loggedInServerInterface.RequestServerRequestEnergy(m_friendId, m_callbackObject);
		}
	}
}
