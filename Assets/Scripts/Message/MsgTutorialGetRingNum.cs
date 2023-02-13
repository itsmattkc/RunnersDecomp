namespace Message
{
	public class MsgTutorialGetRingNum : MessageBase
	{
		public int m_ring;

		public MsgTutorialGetRingNum()
			: base(12346)
		{
		}
	}
}
