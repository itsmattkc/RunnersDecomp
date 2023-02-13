namespace Message
{
	public class MsgEventButton : MessageBase
	{
		public enum ButtonType
		{
			RAID_BOSS,
			RAID_BOSS_BACK,
			SPECIAL_STAGE,
			SPECIAL_STAGE_BACK,
			COLLECT_EVENT,
			COLLECT_EVENT_BACK,
			UNKNOWN
		}

		private ButtonType m_buttonType = ButtonType.UNKNOWN;

		public ButtonType Type
		{
			get
			{
				return m_buttonType;
			}
		}

		public MsgEventButton(ButtonType type)
			: base(57344)
		{
			m_buttonType = type;
		}
	}
}
