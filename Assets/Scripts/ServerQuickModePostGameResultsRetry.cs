using UnityEngine;

public class ServerQuickModePostGameResultsRetry : ServerRetryProcess
{
	private ServerQuickModeGameResults m_results;

	public ServerQuickModePostGameResultsRetry(ServerQuickModeGameResults results, GameObject callbackObject)
		: base(callbackObject)
	{
		m_results = results;
	}

	public override void Retry()
	{
		ServerInterface loggedInServerInterface = ServerInterface.LoggedInServerInterface;
		if (loggedInServerInterface != null)
		{
			loggedInServerInterface.RequestServerQuickModePostGameResults(m_results, m_callbackObject);
		}
	}
}
