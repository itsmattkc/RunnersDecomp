using UnityEngine;

public class ServerSetNoahIdRetry : ServerRetryProcess
{
	public string m_noahId;

	public ServerSetNoahIdRetry(string noahId, GameObject callbackObject)
		: base(callbackObject)
	{
		m_noahId = noahId;
	}

	public override void Retry()
	{
		ServerInterface loggedInServerInterface = ServerInterface.LoggedInServerInterface;
		if (loggedInServerInterface != null)
		{
			loggedInServerInterface.RequestServerSetNoahId(m_noahId, m_callbackObject);
		}
	}
}
