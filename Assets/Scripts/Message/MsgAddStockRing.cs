namespace Message
{
	public class MsgAddStockRing : MessageBase
	{
		public int m_numAddRings;

		public MsgAddStockRing(int num)
			: base(49154)
		{
			m_numAddRings = num;
		}
	}
}
