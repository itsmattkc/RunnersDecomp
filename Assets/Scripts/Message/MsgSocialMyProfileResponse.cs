namespace Message
{
	public class MsgSocialMyProfileResponse : MessageBase
	{
		public SocialResult m_result;

		public SocialUserData m_profile;

		public MsgSocialMyProfileResponse()
			: base(63489)
		{
		}
	}
}
