namespace Message
{
	public class MsgEventUpdateGameResultsSucceed : MessageBase
	{
		public ServerEventRaidBossBonus m_bonus;

		public MsgEventUpdateGameResultsSucceed()
			: base(61509)
		{
		}
	}
}
