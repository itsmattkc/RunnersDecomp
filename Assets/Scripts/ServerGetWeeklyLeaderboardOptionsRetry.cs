using UnityEngine;

public class ServerGetWeeklyLeaderboardOptionsRetry : ServerRetryProcess
{
	private int m_mode;

	public ServerGetWeeklyLeaderboardOptionsRetry(int mode, GameObject callbackObject)
		: base(callbackObject)
	{
		m_mode = mode;
	}

	public override void Retry()
	{
		ServerInterface loggedInServerInterface = ServerInterface.LoggedInServerInterface;
		if (loggedInServerInterface != null)
		{
			loggedInServerInterface.RequestServerGetWeeklyLeaderboardOptions(m_mode, m_callbackObject);
		}
	}
}
