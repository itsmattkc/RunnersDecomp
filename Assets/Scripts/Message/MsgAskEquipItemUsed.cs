namespace Message
{
	public class MsgAskEquipItemUsed : MessageBase
	{
		public bool m_ok;

		public ItemType m_itemType;

		public MsgAskEquipItemUsed(ItemType itemType)
			: base(12321)
		{
			m_itemType = itemType;
		}
	}
}
