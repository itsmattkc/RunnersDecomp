using System.Collections.Generic;

namespace Message
{
	public class MsgPostGameResultsSucceed : MessageBase
	{
		public ServerPlayerState m_playerState;

		public ServerMileageMapState m_mileageMapState;

		public List<ServerMileageIncentive> m_mileageIncentive;

		public List<ServerItemState> m_dailyIncentive;

		public MsgPostGameResultsSucceed()
			: base(61449)
		{
		}
	}
}
