namespace Message
{
	public class MsgGetFreeItemListSucceed : MessageBase
	{
		public ServerFreeItemState m_freeItemState;

		public MsgGetFreeItemListSucceed()
			: base(61456)
		{
		}
	}
}
