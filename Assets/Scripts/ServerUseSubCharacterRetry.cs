using UnityEngine;

public class ServerUseSubCharacterRetry : ServerRetryProcess
{
	public bool m_useFlag;

	public ServerUseSubCharacterRetry(bool useFlag, GameObject callbackObject)
		: base(callbackObject)
	{
		m_useFlag = useFlag;
	}

	public override void Retry()
	{
		ServerInterface loggedInServerInterface = ServerInterface.LoggedInServerInterface;
		if (loggedInServerInterface != null)
		{
			loggedInServerInterface.RequestServerUseSubCharacter(m_useFlag, m_callbackObject);
		}
	}
}
