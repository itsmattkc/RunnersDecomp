namespace Mission
{
	public class MissionCheckCount : MissionCheck
	{
		private long m_count;

		private EventID m_eventID;

		public MissionCheckCount(EventID id)
		{
			m_eventID = id;
		}

		public override void ProcEvent(MissionEvent missionEvent)
		{
			m_count = CheckCompletedAddCount(missionEvent, m_eventID, m_count);
		}

		public override void SetInitialValue(long initialValue)
		{
			m_count = initialValue;
		}

		public override long GetValue()
		{
			return m_count;
		}

		private long CheckCompletedAddCount(MissionEvent missionEvent, EventID check_id, long in_count)
		{
			long num = in_count;
			if (!IsCompleted() && check_id == missionEvent.m_id)
			{
				num += missionEvent.m_num;
				if (num >= m_data.quota)
				{
					SetCompleted();
				}
			}
			return num;
		}
	}
}
