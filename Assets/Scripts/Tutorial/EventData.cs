namespace Tutorial
{
	public class EventData
	{
		private static readonly EventClearType[] EVENT_TYPE_TBL = new EventClearType[8]
		{
			EventClearType.CLEAR,
			EventClearType.CLEAR,
			EventClearType.CLEAR,
			EventClearType.CLEAR,
			EventClearType.NO_DAMAGE,
			EventClearType.NO_MISS,
			EventClearType.CLEAR,
			EventClearType.CLEAR
		};

		public static EventClearType GetEventClearType(EventID id)
		{
			if ((uint)id < 8u)
			{
				return EVENT_TYPE_TBL[(int)id];
			}
			return EventClearType.CLEAR;
		}
	}
}
