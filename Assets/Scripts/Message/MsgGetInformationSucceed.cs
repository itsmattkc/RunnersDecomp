namespace Message
{
	public class MsgGetInformationSucceed : MessageBase
	{
		public ServerNoticeInfo m_information;

		public MsgGetInformationSucceed()
			: base(61488)
		{
		}
	}
}
