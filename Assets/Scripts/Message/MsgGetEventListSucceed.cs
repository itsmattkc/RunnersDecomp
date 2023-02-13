using System.Collections.Generic;

namespace Message
{
	public class MsgGetEventListSucceed : MessageBase
	{
		public List<ServerEventEntry> m_eventList;

		public MsgGetEventListSucceed()
			: base(61502)
		{
		}
	}
}
