public class MileageMapState
{
	public int m_episode = 1;

	public int m_chapter = 1;

	public int m_point;

	public long m_score;

	public MileageMapState()
	{
	}

	public MileageMapState(ServerMileageMapState src)
	{
		Set(src);
	}

	public MileageMapState(MileageMapState src)
	{
		Set(src);
	}

	public void Set(ServerMileageMapState src)
	{
		if (src != null)
		{
			m_episode = src.m_episode;
			m_chapter = src.m_chapter;
			m_point = src.m_point;
			m_score = src.m_stageTotalScore;
		}
	}

	public void Set(MileageMapState src)
	{
		if (src != null)
		{
			m_episode = src.m_episode;
			m_chapter = src.m_chapter;
			m_point = src.m_point;
			m_score = src.m_score;
		}
	}
}
