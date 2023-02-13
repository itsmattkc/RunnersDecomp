namespace Message
{
	public class MsgGetEventStateSucceed : MessageBase
	{
		public ServerEventState m_eventState;

		public MsgGetEventStateSucceed()
			: base(61504)
		{
		}
	}
}
