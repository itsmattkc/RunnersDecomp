namespace Message
{
	public class MsgExternalGamePause : MessageBase
	{
		public bool m_backMainMenu;

		public bool m_backKey;

		public MsgExternalGamePause(bool backMainMenu, bool backKey)
			: base(12358)
		{
			m_backMainMenu = backMainMenu;
			m_backKey = backKey;
		}
	}
}
