using System.Collections.Generic;

namespace Message
{
	public class MsgActStartSucceed : MessageBase
	{
		public ServerPlayerState m_playerState;

		public List<ServerDistanceFriendEntry> m_friendDistanceList;

		public MsgActStartSucceed()
			: base(61447)
		{
		}
	}
}
