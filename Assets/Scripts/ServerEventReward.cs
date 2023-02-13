public class ServerEventReward : ServerItemState
{
	private int m_rewardId;

	private long m_param;

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

	public ServerEventReward()
	{
		m_rewardId = 0;
		m_param = 0L;
	}

	public void CopyTo(ServerEventReward to)
	{
		CopyTo((ServerItemState)to);
		to.m_rewardId = m_rewardId;
		to.m_param = m_param;
	}
}
