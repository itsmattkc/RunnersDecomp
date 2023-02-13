namespace Message
{
	public class MsgMenuPlayerStatus : MessageBase
	{
		public enum StatusType
		{
			USE_SUB_CHAR
		}

		private StatusType m_status;

		public StatusType Status
		{
			get
			{
				return m_status;
			}
		}

		public MsgMenuPlayerStatus(StatusType status)
			: base(57344)
		{
			m_status = status;
		}
	}
}
