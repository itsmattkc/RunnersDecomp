namespace Message
{
	public class MsgGetEventRaidBossUserListSucceed : MessageBase
	{
		public ServerEventRaidBossBonus m_bonus;

		public MsgGetEventRaidBossUserListSucceed()
			: base(61507)
		{
		}
	}
}
