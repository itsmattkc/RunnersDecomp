using UnityEngine;

public class ServerEquipChaoRetry : ServerRetryProcess
{
	public int m_mainChaoId;

	public int m_subChaoId;

	public ServerEquipChaoRetry(int mainChaoId, int subChaoId, GameObject callbackObject)
		: base(callbackObject)
	{
		m_mainChaoId = mainChaoId;
		m_subChaoId = subChaoId;
	}

	public override void Retry()
	{
		ServerInterface loggedInServerInterface = ServerInterface.LoggedInServerInterface;
		if (loggedInServerInterface != null)
		{
			loggedInServerInterface.RequestServerEquipChao(m_mainChaoId, m_subChaoId, m_callbackObject);
		}
	}
}
