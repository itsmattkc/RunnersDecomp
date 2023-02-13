using System;

public class ServerMileageReward
{
	public int m_episode;

	public int m_chapter;

	public int m_type;

	public int m_point;

	public int m_itemId;

	public int m_numItem;

	public int m_limitTime;

	public DateTime m_startTime;

	public ServerMileageReward()
	{
		m_episode = 1;
		m_chapter = 1;
		m_type = 1;
		m_point = 0;
		m_itemId = 0;
		m_numItem = 0;
		m_limitTime = 0;
	}

	public void CopyTo(ServerMileageReward to)
	{
		if (to != null)
		{
			to.m_episode = m_episode;
			to.m_chapter = m_chapter;
			to.m_type = m_type;
			to.m_point = m_point;
			to.m_itemId = m_itemId;
			to.m_numItem = m_numItem;
			to.m_limitTime = m_limitTime;
			to.m_startTime = m_startTime;
		}
	}
}
