using System;

namespace Message
{
	public class MsgGetDailyBattleStatusSucceed : MessageBase
	{
		public ServerDailyBattleStatus battleStatus;

		public DateTime endTime;

		public MsgGetDailyBattleStatusSucceed()
			: base(61471)
		{
			battleStatus = new ServerDailyBattleStatus();
			endTime = NetBase.GetCurrentTime();
		}
	}
}
