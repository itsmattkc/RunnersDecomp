namespace Message
{
	public class MsgDrawRaidBossSucceed : MessageBase
	{
		public ServerEventRaidBossState m_raidBossState;

		public MsgDrawRaidBossSucceed()
			: base(61511)
		{
		}
	}
}
