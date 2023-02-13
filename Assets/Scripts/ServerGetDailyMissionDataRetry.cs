using UnityEngine;

public class ServerGetDailyMissionDataRetry : ServerRetryProcess
{
	public ServerGetDailyMissionDataRetry(GameObject callbackObject)
		: base(callbackObject)
	{
	}

	public override void Retry()
	{
		ServerInterface loggedInServerInterface = ServerInterface.LoggedInServerInterface;
		if (loggedInServerInterface != null)
		{
			loggedInServerInterface.RequestServerGetDailyMissionData(m_callbackObject);
		}
	}
}
