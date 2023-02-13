namespace Message
{
	public class MsgSetPause : MessageBase
	{
		public bool m_backMainMenu;

		public bool m_backKey;

		public MsgSetPause(bool backMainMenu, bool backKey)
			: base(12359)
		{
			m_backMainMenu = backMainMenu;
			m_backKey = backKey;
		}
	}
}
