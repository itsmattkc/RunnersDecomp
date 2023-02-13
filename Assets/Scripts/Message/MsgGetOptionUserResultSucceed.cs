namespace Message
{
	public class MsgGetOptionUserResultSucceed : MessageBase
	{
		public ServerOptionUserResult m_serverOptionUserResult;

		public MsgGetOptionUserResultSucceed()
			: base(61487)
		{
		}
	}
}
