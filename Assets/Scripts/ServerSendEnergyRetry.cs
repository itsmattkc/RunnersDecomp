using UnityEngine;

public class ServerSendEnergyRetry : ServerRetryProcess
{
	public string m_friendId;

	public ServerSendEnergyRetry(string friendId, GameObject callbackObject)
		: base(callbackObject)
	{
		m_friendId = friendId;
	}

	public override void Retry()
	{
		ServerInterface loggedInServerInterface = ServerInterface.LoggedInServerInterface;
		if (loggedInServerInterface != null)
		{
			loggedInServerInterface.RequestServerSendEnergy(m_friendId, m_callbackObject);
		}
	}
}
