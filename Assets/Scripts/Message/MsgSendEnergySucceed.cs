namespace Message
{
	public class MsgSendEnergySucceed : MessageBase
	{
		public int m_expireTime;

		public MsgSendEnergySucceed()
			: base(61483)
		{
		}
	}
}
