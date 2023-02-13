using System.Collections.Generic;

namespace Message
{
	public class MsgGetEventRewardSucceed : MessageBase
	{
		public List<ServerEventReward> m_eventRewardList;

		public MsgGetEventRewardSucceed()
			: base(61503)
		{
		}
	}
}
