using UnityEngine;

public class ServerSetFacebookScopedIdRetry : ServerRetryProcess
{
	public string m_userId;

	public ServerSetFacebookScopedIdRetry(string userId, GameObject callbackObject)
		: base(callbackObject)
	{
		m_userId = userId;
	}

	public override void Retry()
	{
		ServerInterface loggedInServerInterface = ServerInterface.LoggedInServerInterface;
		if (loggedInServerInterface != null)
		{
			loggedInServerInterface.RequestServerSetFacebookScopedId(m_userId, m_callbackObject);
		}
	}
}
