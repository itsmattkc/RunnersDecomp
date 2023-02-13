using UnityEngine;

public class ServerGetChaoRentalStatesRetry : ServerRetryProcess
{
	public string[] m_friendId;

	public ServerGetChaoRentalStatesRetry(string[] friendId, GameObject callbackObject)
		: base(callbackObject)
	{
		m_friendId = friendId;
	}

	public override void Retry()
	{
		ServerInterface loggedInServerInterface = ServerInterface.LoggedInServerInterface;
		if (loggedInServerInterface != null)
		{
			loggedInServerInterface.RequestServerGetChaoRentalStates(m_friendId, m_callbackObject);
		}
	}
}
