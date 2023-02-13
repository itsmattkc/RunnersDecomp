using UnityEngine;

public class ServerUpdateDailyBattleStatusRetry : ServerRetryProcess
{
	public ServerUpdateDailyBattleStatusRetry(GameObject callbackObject)
		: base(callbackObject)
	{
	}

	public override void Retry()
	{
		ServerInterface loggedInServerInterface = ServerInterface.LoggedInServerInterface;
		if (loggedInServerInterface != null)
		{
			loggedInServerInterface.RequestServerUpdateDailyBattleStatus(m_callbackObject);
		}
	}
}
