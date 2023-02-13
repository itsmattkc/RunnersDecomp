namespace Message
{
	public class MsgEnablePause : MessageBase
	{
		public bool m_enable;

		public MsgEnablePause(bool value)
			: base(4099)
		{
			m_enable = value;
		}
	}
}
