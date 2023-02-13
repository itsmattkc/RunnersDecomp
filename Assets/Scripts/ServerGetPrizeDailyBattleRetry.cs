using UnityEngine;

public class ServerGetPrizeDailyBattleRetry : ServerRetryProcess
{
	public ServerGetPrizeDailyBattleRetry(GameObject callbackObject)
		: base(callbackObject)
	{
	}

	public override void Retry()
	{
		ServerInterface loggedInServerInterface = ServerInterface.LoggedInServerInterface;
		if (loggedInServerInterface != null)
		{
			loggedInServerInterface.RequestServerGetPrizeDailyBattle(m_callbackObject);
		}
	}
}
