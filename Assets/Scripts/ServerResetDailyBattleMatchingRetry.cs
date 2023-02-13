using UnityEngine;

public class ServerResetDailyBattleMatchingRetry : ServerRetryProcess
{
	private int m_type;

	public ServerResetDailyBattleMatchingRetry(int type, GameObject callbackObject)
		: base(callbackObject)
	{
		m_type = type;
	}

	public override void Retry()
	{
		ServerInterface loggedInServerInterface = ServerInterface.LoggedInServerInterface;
		if (loggedInServerInterface != null)
		{
			loggedInServerInterface.RequestServerResetDailyBattleMatching(m_type, m_callbackObject);
		}
	}
}
