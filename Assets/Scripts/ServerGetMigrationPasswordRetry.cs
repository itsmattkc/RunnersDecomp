using UnityEngine;

public class ServerGetMigrationPasswordRetry : ServerRetryProcess
{
	public string m_userPassword;

	public ServerGetMigrationPasswordRetry(string userPassword, GameObject callbackObject)
		: base(callbackObject)
	{
		m_userPassword = userPassword;
	}

	public override void Retry()
	{
		ServerInterface loggedInServerInterface = ServerInterface.LoggedInServerInterface;
		if (loggedInServerInterface != null)
		{
			loggedInServerInterface.RequestServerGetMigrationPassword(m_userPassword, m_callbackObject);
		}
	}
}
