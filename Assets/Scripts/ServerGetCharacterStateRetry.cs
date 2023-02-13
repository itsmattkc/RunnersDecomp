using UnityEngine;

public class ServerGetCharacterStateRetry : ServerRetryProcess
{
	public ServerGetCharacterStateRetry(GameObject callbackObject)
		: base(callbackObject)
	{
	}

	public override void Retry()
	{
		ServerInterface loggedInServerInterface = ServerInterface.LoggedInServerInterface;
		if (loggedInServerInterface != null)
		{
			loggedInServerInterface.RequestServerGetCharacterState(m_callbackObject);
		}
	}
}
