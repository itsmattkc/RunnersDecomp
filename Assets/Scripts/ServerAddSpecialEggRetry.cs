using UnityEngine;

public class ServerAddSpecialEggRetry : ServerRetryProcess
{
	public int m_numSpecialEgg;

	public ServerAddSpecialEggRetry(int numSpecialEgg, GameObject callbackObject)
		: base(callbackObject)
	{
		m_numSpecialEgg = numSpecialEgg;
	}

	public override void Retry()
	{
		ServerInterface loggedInServerInterface = ServerInterface.LoggedInServerInterface;
		if (loggedInServerInterface != null)
		{
			loggedInServerInterface.RequestServerAddSpecialEgg(m_numSpecialEgg, m_callbackObject);
		}
	}
}
