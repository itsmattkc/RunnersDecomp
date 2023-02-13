using UnityEngine;

public class ServerEventUpdateGameResultsRetry : ServerRetryProcess
{
	private ServerEventGameResults m_results;

	public ServerEventUpdateGameResultsRetry(ServerEventGameResults results, GameObject callbackObject)
		: base(callbackObject)
	{
		m_results = results;
	}

	public override void Retry()
	{
		ServerInterface loggedInServerInterface = ServerInterface.LoggedInServerInterface;
		if (loggedInServerInterface != null)
		{
			loggedInServerInterface.RequestServerEventUpdateGameResults(m_results, m_callbackObject);
		}
	}
}
