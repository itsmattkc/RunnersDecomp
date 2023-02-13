using UnityEngine;

public class ServerGetLeagueDataRetry : ServerRetryProcess
{
	public int m_mode;

	public ServerGetLeagueDataRetry(int mode, GameObject callbackObject)
		: base(callbackObject)
	{
		m_mode = mode;
	}

	public override void Retry()
	{
		ServerInterface loggedInServerInterface = ServerInterface.LoggedInServerInterface;
		if (loggedInServerInterface != null)
		{
			loggedInServerInterface.RequestServerGetLeagueData(m_mode, m_callbackObject);
		}
	}
}
