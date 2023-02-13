using System;

namespace Message
{
	public class MsgResetDailyBattleMatchingSucceed : MessageBase
	{
		public ServerPlayerState playerState;

		public ServerDailyBattleDataPair battleDataPair;

		public DateTime endTime;

		public MsgResetDailyBattleMatchingSucceed()
			: base(61477)
		{
			playerState = new ServerPlayerState();
			battleDataPair = new ServerDailyBattleDataPair();
			endTime = NetBase.GetCurrentTime();
		}
	}
}
