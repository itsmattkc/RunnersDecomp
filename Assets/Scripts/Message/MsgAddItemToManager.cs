namespace Message
{
	public class MsgAddItemToManager : MessageBase
	{
		public readonly ItemType m_itemType;

		public MsgAddItemToManager(ItemType itemType)
			: base(12291)
		{
			m_itemType = itemType;
		}
	}
}
