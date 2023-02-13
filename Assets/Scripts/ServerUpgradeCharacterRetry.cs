using UnityEngine;

public class ServerUpgradeCharacterRetry : ServerRetryProcess
{
	public int m_characterId;

	public int m_abilityId;

	public ServerUpgradeCharacterRetry(int characterId, int abilityId, GameObject callbackObject)
		: base(callbackObject)
	{
		m_characterId = characterId;
		m_abilityId = abilityId;
	}

	public override void Retry()
	{
		ServerInterface loggedInServerInterface = ServerInterface.LoggedInServerInterface;
		if (loggedInServerInterface != null)
		{
			loggedInServerInterface.RequestServerUpgradeCharacter(m_characterId, m_abilityId, m_callbackObject);
		}
	}
}
