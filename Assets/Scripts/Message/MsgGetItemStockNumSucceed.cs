using System.Collections.Generic;

namespace Message
{
	public class MsgGetItemStockNumSucceed : MessageBase
	{
		public List<ServerItemState> m_itemStockList;

		public MsgGetItemStockNumSucceed()
			: base(61458)
		{
		}
	}
}
