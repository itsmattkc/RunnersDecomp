using System.Collections.Generic;

namespace Message
{
	public class MsgGetFriendChaoStateSucceed : MessageBase
	{
		public List<ServerChaoRentalState> m_chaoRentalStates;

		public MsgGetFriendChaoStateSucceed()
			: base(61480)
		{
		}
	}
}
