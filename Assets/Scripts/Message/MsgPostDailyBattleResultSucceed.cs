namespace Message
{
	public class MsgPostDailyBattleResultSucceed : MessageBase
	{
		public ServerDailyBattleStatus battleStatus;

		public ServerDailyBattleDataPair battleDataPair;

		public bool rewardFlag;

		public ServerDailyBattleDataPair rewardBattleDataPair;

		public MsgPostDailyBattleResultSucceed()
			: base(61473)
		{
			battleStatus = new ServerDailyBattleStatus();
			battleDataPair = new ServerDailyBattleDataPair();
			rewardFlag = false;
			rewardBattleDataPair = null;
		}
	}
}
