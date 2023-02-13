namespace Message
{
	public class MsgDisableInput : MessageBase
	{
		public bool m_disable;

		public MsgDisableInput(bool value)
			: base(12319)
		{
			m_disable = value;
		}
	}
}
