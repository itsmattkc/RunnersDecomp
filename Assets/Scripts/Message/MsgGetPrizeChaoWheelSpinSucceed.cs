namespace Message
{
	public class MsgGetPrizeChaoWheelSpinSucceed : MessageBase
	{
		public ServerPrizeState m_prizeState;

		public int m_type;

		public MsgGetPrizeChaoWheelSpinSucceed()
			: base(61464)
		{
		}
	}
}
