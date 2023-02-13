using UnityEngine;

public class ServerGetLeagueOperatorDataRetry : ServerRetryProcess
{
	public int m_mode;

	public ServerGetLeagueOperatorDataRetry(int mode, GameObject callbackObject)
		: base(callbackObject)
	{
		m_mode = mode;
	}

	public override void Retry()
	{
		ServerInterface loggedInServerInterface = ServerInterface.LoggedInServerInterface;
		if (loggedInServerInterface != null)
		{
			loggedInServerInterface.RequestServerGetLeagueOperatorData(m_mode, m_callbackObject);
		}
	}
}
