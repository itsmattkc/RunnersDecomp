namespace Message
{
	public class MsgInvincible : MessageBase
	{
		public enum Mode
		{
			Start,
			End
		}

		public Mode m_mode;

		public MsgInvincible(Mode mode)
			: base(12329)
		{
			m_mode = mode;
		}
	}
}
