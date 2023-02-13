using System.Collections.Generic;

namespace Message
{
	public class MsgGetRingItemStateSucceed : MessageBase
	{
		public List<ServerRingItemState> m_RingStateList;

		public MsgGetRingItemStateSucceed()
			: base(61453)
		{
		}
	}
}
