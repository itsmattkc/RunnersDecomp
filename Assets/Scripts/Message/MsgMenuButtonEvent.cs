namespace Message
{
	public class MsgMenuButtonEvent : MessageBase
	{
		private ButtonInfoTable.ButtonType m_buttonType;

		public bool m_clearHistories;

		public ButtonInfoTable.ButtonType ButtonType
		{
			get
			{
				return m_buttonType;
			}
		}

		public MsgMenuButtonEvent(ButtonInfoTable.ButtonType type)
			: base(57344)
		{
			m_buttonType = type;
		}
	}
}
