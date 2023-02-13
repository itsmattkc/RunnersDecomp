using System.Collections.Generic;

namespace Message
{
	public class MsgGetRingExchangeListSucceed : MessageBase
	{
		public List<ServerRingExchangeList> m_exchangeList;

		public MsgGetRingExchangeListSucceed()
			: base(61470)
		{
		}
	}
}
