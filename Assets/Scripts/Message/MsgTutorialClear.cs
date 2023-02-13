using System.Collections.Generic;
using Tutorial;

namespace Message
{
	public class MsgTutorialClear : MessageBase
	{
		public struct Data
		{
			public EventID eventid;
		}

		public List<Data> m_data = new List<Data>();

		public MsgTutorialClear(EventID id)
			: base(12336)
		{
			Add(id);
		}

		public void Add(EventID id)
		{
			Data item = default(Data);
			item.eventid = id;
			m_data.Add(item);
		}
	}
}
