namespace Message
{
	public class MsgGetMigrationPasswordSucceed : MessageBase
	{
		public string m_migrationPassword;

		public MsgGetMigrationPasswordSucceed()
			: base(61442)
		{
		}
	}
}
