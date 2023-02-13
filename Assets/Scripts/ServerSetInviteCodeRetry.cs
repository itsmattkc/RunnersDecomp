using UnityEngine;

public class ServerSetInviteCodeRetry : ServerRetryProcess
{
	public string m_friendId;

	public ServerSetInviteCodeRetry(string friendId, GameObject callbackObject)
		: base(callbackObject)
	{
		m_friendId = friendId;
	}

	public override void Retry()
	{
		ServerInterface loggedInServerInterface = ServerInterface.LoggedInServerInterface;
		if (loggedInServerInterface != null)
		{
			loggedInServerInterface.RequestServerSetInviteCode(m_friendId, m_callbackObject);
		}
	}
}
