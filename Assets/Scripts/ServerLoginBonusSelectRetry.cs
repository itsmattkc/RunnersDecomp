using UnityEngine;

public class ServerLoginBonusSelectRetry : ServerRetryProcess
{
	private int m_rewardId;

	private int m_rewardDays;

	private int m_rewardSelect;

	private int m_firstRewardDays;

	private int m_firstRewardSelect;

	public ServerLoginBonusSelectRetry(int rewardId, int rewardDays, int rewardSelect, int firstRewardDays, int firstRewardSelect, GameObject callbackObject)
		: base(callbackObject)
	{
		m_rewardId = rewardId;
		m_rewardDays = rewardDays;
		m_rewardSelect = rewardSelect;
		m_firstRewardDays = firstRewardDays;
		m_firstRewardSelect = firstRewardSelect;
	}

	public override void Retry()
	{
		ServerInterface loggedInServerInterface = ServerInterface.LoggedInServerInterface;
		if (loggedInServerInterface != null)
		{
			loggedInServerInterface.RequestServerLoginBonusSelect(m_rewardId, m_rewardDays, m_rewardSelect, m_firstRewardDays, m_firstRewardSelect, m_callbackObject);
		}
	}
}
