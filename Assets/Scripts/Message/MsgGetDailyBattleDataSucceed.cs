namespace Message
{
	public class MsgGetDailyBattleDataSucceed : MessageBase
	{
		public ServerDailyBattleDataPair battleDataPair;

		public MsgGetDailyBattleDataSucceed()
			: base(61474)
		{
			battleDataPair = new ServerDailyBattleDataPair();
		}
	}
}
