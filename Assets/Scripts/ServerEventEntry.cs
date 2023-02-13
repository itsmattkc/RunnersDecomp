using System;

public class ServerEventEntry
{
	private int m_eventId;

	private int m_eventType;

	private DateTime m_eventStartTime;

	private DateTime m_eventEndTime;

	private DateTime m_eventCloseTime;

	public int EventId
	{
		get
		{
			return m_eventId;
		}
		set
		{
			m_eventId = value;
		}
	}

	public int EventType
	{
		get
		{
			return m_eventType;
		}
		set
		{
			m_eventType = value;
		}
	}

	public DateTime EventStartTime
	{
		get
		{
			return m_eventStartTime;
		}
		set
		{
			m_eventStartTime = value;
		}
	}

	public DateTime EventEndTime
	{
		get
		{
			return m_eventEndTime;
		}
		set
		{
			m_eventEndTime = value;
		}
	}

	public DateTime EventCloseTime
	{
		get
		{
			return m_eventCloseTime;
		}
		set
		{
			m_eventCloseTime = value;
		}
	}

	public ServerEventEntry()
	{
		m_eventId = 0;
		m_eventType = 0;
		m_eventStartTime = NetBase.GetCurrentTime();
		m_eventEndTime = NetBase.GetCurrentTime();
		m_eventCloseTime = NetBase.GetCurrentTime();
	}

	public void CopyTo(ServerEventEntry to)
	{
		to.m_eventId = m_eventId;
		to.m_eventType = m_eventType;
		to.m_eventStartTime = m_eventStartTime;
		to.m_eventEndTime = m_eventEndTime;
		to.m_eventCloseTime = m_eventCloseTime;
	}
}
