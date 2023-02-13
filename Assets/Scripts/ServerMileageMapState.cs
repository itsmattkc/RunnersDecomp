using System;

public class ServerMileageMapState
{
	public int m_episode;

	public int m_chapter;

	public int m_point;

	public int m_numBossAttack;

	public long m_stageTotalScore;

	public long m_stageMaxScore;

	public DateTime m_chapterStartTime;

	public ServerMileageMapState()
	{
		m_episode = 1;
		m_chapter = 1;
		m_point = 0;
		m_numBossAttack = 0;
		m_stageTotalScore = 0L;
		m_stageMaxScore = 0L;
		m_chapterStartTime = DateTime.Now;
	}

	public void CopyTo(ServerMileageMapState to)
	{
		if (to != null)
		{
			to.m_episode = m_episode;
			to.m_chapter = m_chapter;
			to.m_point = m_point;
			to.m_numBossAttack = m_numBossAttack;
			to.m_stageTotalScore = m_stageTotalScore;
			to.m_stageMaxScore = m_stageMaxScore;
			to.m_chapterStartTime = m_chapterStartTime;
		}
	}
}
