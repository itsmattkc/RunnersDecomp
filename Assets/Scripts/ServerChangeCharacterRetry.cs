using UnityEngine;

public class ServerChangeCharacterRetry : ServerRetryProcess
{
	public int m_mainCharaId;

	public int m_subCharaId;

	public ServerChangeCharacterRetry(int mainCharaId, int subCharaId, GameObject callbackObject)
		: base(callbackObject)
	{
		m_mainCharaId = mainCharaId;
		m_subCharaId = subCharaId;
	}

	public override void Retry()
	{
		ServerInterface loggedInServerInterface = ServerInterface.LoggedInServerInterface;
		if (loggedInServerInterface != null)
		{
			loggedInServerInterface.RequestServerChangeCharacter(m_mainCharaId, m_subCharaId, m_callbackObject);
		}
	}
}
