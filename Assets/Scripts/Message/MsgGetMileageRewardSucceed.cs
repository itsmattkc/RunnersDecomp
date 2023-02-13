using System.Collections.Generic;

namespace Message
{
	public class MsgGetMileageRewardSucceed : MessageBase
	{
		public List<ServerMileageReward> m_mileageRewardList;

		public MsgGetMileageRewardSucceed()
			: base(61501)
		{
		}
	}
}
