using System.Collections.Generic;

namespace Message
{
	public class MsgUpdateMesseageSucceed : MessageBase
	{
		public List<ServerPresentState> m_presentStateList;

		public List<int> m_notRecvMessageList;

		public List<int> m_notRecvOperatorMessageList;

		public MsgUpdateMesseageSucceed()
			: base(61485)
		{
		}
	}
}
