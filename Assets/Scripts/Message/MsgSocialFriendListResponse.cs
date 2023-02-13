using System.Collections.Generic;

namespace Message
{
	public class MsgSocialFriendListResponse : MessageBase
	{
		public SocialResult m_result;

		public List<SocialUserData> m_friends;

		public MsgSocialFriendListResponse()
			: base(63490)
		{
		}
	}
}
