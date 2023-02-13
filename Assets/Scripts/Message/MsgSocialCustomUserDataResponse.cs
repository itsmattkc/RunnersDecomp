namespace Message
{
	public class MsgSocialCustomUserDataResponse : MessageBase
	{
		public bool m_isCreated;

		public SocialUserData m_userData;

		public MsgSocialCustomUserDataResponse()
			: base(63491)
		{
		}
	}
}
