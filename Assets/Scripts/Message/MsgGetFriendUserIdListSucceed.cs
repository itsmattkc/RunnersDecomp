using System.Collections.Generic;

namespace Message
{
	public class MsgGetFriendUserIdListSucceed : MessageBase
	{
		public List<ServerUserTransformData> m_transformDataList;

		public MsgGetFriendUserIdListSucceed()
			: base(61493)
		{
		}
	}
}
