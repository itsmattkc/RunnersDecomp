using System.Collections.Generic;

namespace Message
{
	public class MsgGetRedStarExchangeListSucceed : MessageBase
	{
		public List<ServerRedStarItemState> m_itemList;

		public int m_totalItems;

		public MsgGetRedStarExchangeListSucceed()
			: base(61469)
		{
		}
	}
}
