namespace Message
{
	public class MsgSocialNormalResponse : MessageBase
	{
		public SocialResult m_result;

		public MsgSocialNormalResponse()
			: base(63488)
		{
		}
	}
}
