using System.Collections.Generic;

namespace Message
{
	public class MsgGetNormalIncentiveSucceed : MessageBase
	{
		public ServerPlayerState m_playerState;

		public List<ServerPresentState> m_incentive;

		public MsgGetNormalIncentiveSucceed()
			: base(61490)
		{
		}
	}
}
