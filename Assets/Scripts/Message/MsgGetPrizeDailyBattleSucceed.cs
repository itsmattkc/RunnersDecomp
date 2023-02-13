using System.Collections.Generic;

namespace Message
{
	public class MsgGetPrizeDailyBattleSucceed : MessageBase
	{
		public List<ServerDailyBattlePrizeData> battlePrizeDataList;

		public MsgGetPrizeDailyBattleSucceed()
			: base(61476)
		{
			battlePrizeDataList = new List<ServerDailyBattlePrizeData>();
		}
	}
}
