using UnityEngine;

public class ServerSendApolloRetry : ServerRetryProcess
{
	public int m_type;

	public string[] m_value;

	public ServerSendApolloRetry(int type, string[] value, GameObject callbackObject)
		: base(callbackObject)
	{
		m_type = type;
		m_value = value;
	}

	public override void Retry()
	{
		ServerInterface loggedInServerInterface = ServerInterface.LoggedInServerInterface;
		if (loggedInServerInterface != null)
		{
			loggedInServerInterface.RequestServerSendApollo(m_type, m_value, m_callbackObject);
		}
	}
}
