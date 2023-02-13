namespace Message
{
	public class MsgServerConnctFailed : MessageBase
	{
		public ServerInterface.StatusCode m_status;

		public MsgServerConnctFailed(ServerInterface.StatusCode status)
			: base(61517)
		{
			m_status = status;
		}
	}
}
