namespace Message
{
	public class MsgMenuItemSetStart : MessageBase
	{
		public enum SetMode
		{
			NORMAL,
			TUTORIAL,
			TUTORIAL_SUBCHARA
		}

		public SetMode m_setMode;

		public MsgMenuItemSetStart(SetMode mode)
			: base(57344)
		{
			m_setMode = mode;
		}
	}
}
