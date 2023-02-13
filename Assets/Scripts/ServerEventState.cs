public class ServerEventState
{
	private long m_param;

	private int m_rewardId;

	public long Param
	{
		get
		{
			return m_param;
		}
		set
		{
			m_param = value;
		}
	}

	public int RewardId
	{
		get
		{
			return m_rewardId;
		}
		set
		{
			m_rewardId = value;
		}
	}

	public ServerEventState()
	{
		m_param = 0L;
		m_rewardId = 0;
	}

	public void CopyTo(ServerEventState to)
	{
		to.m_param = m_param;
		to.m_rewardId = m_rewardId;
	}
}
