using Mission;
using System.Collections.Generic;

namespace Message
{
	public class MsgMissionEvent : MessageBase
	{
		public struct Data
		{
			public EventID eventid;

			public long num;
		}

		public List<Data> m_missions = new List<Data>();

		public MsgMissionEvent()
			: base(12318)
		{
		}

		public MsgMissionEvent(EventID id_)
			: base(12318)
		{
			Add(id_, 0L);
		}

		public MsgMissionEvent(EventID id_, long num)
			: base(12318)
		{
			Add(id_, num);
		}

		public void Add(EventID id_, long num)
		{
			Data item = default(Data);
			item.eventid = id_;
			item.num = num;
			m_missions.Add(item);
		}

		public void Add(EventID id_)
		{
			Add(id_, 0L);
		}
	}
}
