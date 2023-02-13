namespace Message
{
	public class MsgSetEquippedItem : MessageBase
	{
		public readonly ItemType[] m_itemType;

		public bool m_enabled;

		public MsgSetEquippedItem(ItemType[] itemType)
			: base(12296)
		{
			m_itemType = new ItemType[itemType.Length];
			itemType.CopyTo(m_itemType, 0);
		}
	}
}
