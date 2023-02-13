namespace Message
{
	public class MsgChangeChara : MessageBase
	{
		public bool m_changeByBtn;

		public bool m_changeByMiss;

		public bool m_succeed;

		public MsgChangeChara()
			: base(12313)
		{
		}
	}
}
