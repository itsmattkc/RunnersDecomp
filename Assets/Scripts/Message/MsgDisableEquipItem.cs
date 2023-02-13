namespace Message
{
	public class MsgDisableEquipItem : MessageBase
	{
		public bool m_disable;

		public MsgDisableEquipItem(bool disable)
			: base(12320)
		{
			m_disable = disable;
		}
	}
}
