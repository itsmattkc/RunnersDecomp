using UnityEngine;

public class ServerGetDailyBattleDataRetry : ServerRetryProcess
{
	public ServerGetDailyBattleDataRetry(GameObject callbackObject)
		: base(callbackObject)
	{
	}

	public override void Retry()
	{
		ServerInterface loggedInServerInterface = ServerInterface.LoggedInServerInterface;
		if (loggedInServerInterface != null)
		{
			loggedInServerInterface.RequestServerGetDailyBattleData(m_callbackObject);
		}
	}
}
