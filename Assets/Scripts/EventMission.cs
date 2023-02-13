public class EventMission
{
	private string m_name = string.Empty;

	private long m_point;

	private int m_reward;

	private int m_index;

	public string name
	{
		get
		{
			return m_name;
		}
	}

	public long point
	{
		get
		{
			return m_point;
		}
	}

	public int reward
	{
		get
		{
			return m_reward;
		}
	}

	public int index
	{
		get
		{
			return m_index;
		}
	}

	public EventMission(string name, long point, int reward, int index)
	{
		m_name = name;
		m_point = point;
		m_reward = reward;
		m_index = index;
	}

	public bool IsAttainment(long point)
	{
		bool result = false;
		if (point >= m_point)
		{
			result = true;
		}
		return result;
	}
}
