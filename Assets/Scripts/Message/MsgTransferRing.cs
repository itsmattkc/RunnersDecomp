namespace Message
{
	public class MsgTransferRing
	{
		public int m_ring;

		public bool m_isContinue;

		public MsgTransferRing()
		{
		}

		public MsgTransferRing(int ring, bool isContinue)
		{
			m_ring = ring;
			m_isContinue = isContinue;
		}
	}
}
