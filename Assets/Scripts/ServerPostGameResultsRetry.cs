using UnityEngine;

public class ServerPostGameResultsRetry : ServerRetryProcess
{
	private ServerGameResults m_results;

	public ServerPostGameResultsRetry(ServerGameResults results, GameObject callbackObject)
		: base(callbackObject)
	{
		m_results = results;
	}

	public override void Retry()
	{
		ServerInterface loggedInServerInterface = ServerInterface.LoggedInServerInterface;
		if (loggedInServerInterface != null)
		{
			loggedInServerInterface.RequestServerPostGameResults(m_results, m_callbackObject);
		}
	}
}
