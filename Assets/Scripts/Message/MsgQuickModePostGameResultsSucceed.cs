using System.Collections.Generic;

namespace Message
{
	public class MsgQuickModePostGameResultsSucceed : MessageBase
	{
		public ServerPlayerState m_playerState;

		public List<ServerItemState> m_dailyIncentive;

		public MsgQuickModePostGameResultsSucceed()
			: base(61514)
		{
		}
	}
}
