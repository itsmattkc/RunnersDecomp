using System.Collections.Generic;

namespace Message
{
	public class MsgGetTickerDataSucceed : MessageBase
	{
		public List<ServerTickerData> m_tickerData;

		public MsgGetTickerDataSucceed()
			: base(61489)
		{
		}
	}
}
