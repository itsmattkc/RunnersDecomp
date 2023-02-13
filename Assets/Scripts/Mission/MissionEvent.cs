namespace Mission
{
	public class MissionEvent
	{
		public EventID m_id;

		public long m_num;

		public MissionEvent(EventID id, long num)
		{
			m_id = id;
			m_num = num;
		}
	}
}
