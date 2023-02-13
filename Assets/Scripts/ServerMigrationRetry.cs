using UnityEngine;

public class ServerMigrationRetry : ServerRetryProcess
{
	public string m_migrationId;

	public string m_migrationPassword;

	public ServerMigrationRetry(string migrationId, string migrationPassword, GameObject callbackObject)
		: base(callbackObject)
	{
		m_migrationId = migrationId;
		m_migrationPassword = migrationPassword;
	}

	public override void Retry()
	{
		ServerInterface serverInterface = GameObjectUtil.FindGameObjectComponent<ServerInterface>("ServerInterface");
		if (serverInterface != null)
		{
			serverInterface.RequestServerMigration(m_migrationId, m_migrationPassword, m_callbackObject);
		}
	}
}
