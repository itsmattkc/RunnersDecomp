namespace Message
{
	public class MsgStockItem : MessageBase
	{
		public ItemType m_itemType;

		public MsgStockItem(ItemType itemType)
			: base(12295)
		{
			m_itemType = itemType;
		}
	}
}
