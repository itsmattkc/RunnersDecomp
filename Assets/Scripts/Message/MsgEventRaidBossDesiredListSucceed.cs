using System.Collections.Generic;

namespace Message
{
	public class MsgEventRaidBossDesiredListSucceed : MessageBase
	{
		public List<ServerEventRaidBossDesiredState> m_desiredList;

		public MsgEventRaidBossDesiredListSucceed()
			: base(61511)
		{
		}
	}
}
