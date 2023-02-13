using System.Collections.Generic;

namespace Message
{
	public class MsgGetCostListSucceed : MessageBase
	{
		public List<ServerConsumedCostData> m_costList;

		public MsgGetCostListSucceed()
			: base(61454)
		{
		}
	}
}
