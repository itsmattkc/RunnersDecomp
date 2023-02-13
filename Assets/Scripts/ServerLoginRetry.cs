using UnityEngine;

public class ServerLoginRetry : ServerRetryProcess
{
	public string m_userId;

	public string m_password;

	public ServerLoginRetry(string userId, string password, GameObject callbackObject)
		: base(callbackObject)
	{
		m_userId = userId;
		m_password = password;
	}

	public override void Retry()
	{
		ServerInterface serverInterface = GameObjectUtil.FindGameObjectComponent<ServerInterface>("ServerInterface");
		if (serverInterface != null)
		{
			serverInterface.RequestServerLogin(m_userId, m_password, m_callbackObject);
		}
	}
}
