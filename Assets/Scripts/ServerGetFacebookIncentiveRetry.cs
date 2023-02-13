using UnityEngine;

public class ServerGetFacebookIncentiveRetry : ServerRetryProcess
{
	public int m_incentiveType;

	public int m_achievementCount;

	public ServerGetFacebookIncentiveRetry(int incentiveType, int achievementCount, GameObject callbackObject)
		: base(callbackObject)
	{
		m_incentiveType = incentiveType;
		m_achievementCount = achievementCount;
	}

	public override void Retry()
	{
		ServerInterface loggedInServerInterface = ServerInterface.LoggedInServerInterface;
		if (loggedInServerInterface != null)
		{
			loggedInServerInterface.RequestServerGetFacebookIncentive(m_incentiveType, m_achievementCount, m_callbackObject);
		}
	}
}
