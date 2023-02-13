using UnityEngine;

public class ServerUnlockedCharacterRetry : ServerRetryProcess
{
	public CharaType m_charaType;

	public ServerItem m_item;

	public ServerUnlockedCharacterRetry(CharaType charaType, ServerItem serverItem, GameObject callbackObject)
		: base(callbackObject)
	{
		m_charaType = charaType;
		m_item = serverItem;
	}

	public override void Retry()
	{
		ServerInterface loggedInServerInterface = ServerInterface.LoggedInServerInterface;
		if (loggedInServerInterface != null)
		{
			loggedInServerInterface.RequestServerUnlockedCharacter(m_charaType, m_item, m_callbackObject);
		}
	}
}
