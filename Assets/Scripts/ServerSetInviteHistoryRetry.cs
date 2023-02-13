using UnityEngine;

public class ServerSetInviteHistoryRetry : ServerRetryProcess
{
	private string m_facebookIdHash;

	public ServerSetInviteHistoryRetry(string facebookIdHash, GameObject callbackObject)
		: base(callbackObject)
	{
		m_facebookIdHash = facebookIdHash;
	}

	public override void Retry()
	{
		ServerInterface loggedInServerInterface = ServerInterface.LoggedInServerInterface;
		if (loggedInServerInterface != null)
		{
			loggedInServerInterface.RequestServerSetInviteHistory(m_facebookIdHash, m_callbackObject);
		}
	}
}
