using System;

namespace Message
{
	public class MsgUpdateDailyBattleStatusSucceed : MessageBase
	{
		public ServerDailyBattleStatus battleStatus;

		public DateTime endTime;

		public bool rewardFlag;

		public ServerDailyBattleDataPair rewardBattleDataPair;

		public MsgUpdateDailyBattleStatusSucceed()
			: base(61472)
		{
			battleStatus = new ServerDailyBattleStatus();
			endTime = NetBase.GetCurrentTime();
			rewardFlag = false;
			rewardBattleDataPair = null;
		}
	}
}
