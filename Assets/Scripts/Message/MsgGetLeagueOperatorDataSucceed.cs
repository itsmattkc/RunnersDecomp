using System.Collections.Generic;

namespace Message
{
	public class MsgGetLeagueOperatorDataSucceed : MessageBase
	{
		public List<ServerLeagueOperatorData> m_leagueOperatorData;

		public MsgGetLeagueOperatorDataSucceed()
			: base(61499)
		{
		}
	}
}
