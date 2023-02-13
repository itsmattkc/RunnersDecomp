namespace Message
{
	public class MsgGetWheelOptionsSucceed : MessageBase
	{
		public ServerWheelOptions m_wheelOptions;

		public MsgGetWheelOptionsSucceed()
			: base(61461)
		{
		}
	}
}
