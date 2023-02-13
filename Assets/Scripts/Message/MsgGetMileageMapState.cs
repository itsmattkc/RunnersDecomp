namespace Message
{
	public class MsgGetMileageMapState : MessageBase
	{
		public MileageMapState m_mileageMapState;

		public uint m_debugLevel;

		public bool m_succeed;

		public MsgGetMileageMapState()
			: base(12327)
		{
		}
	}
}
