using System;
using System.Collections.Generic;

public class ServerLoginBonusData
{
	public ServerLoginBonusState m_loginBonusState;

	public DateTime m_startTime;

	public DateTime m_endTime;

	public List<ServerLoginBonusReward> m_loginBonusRewardList;

	public List<ServerLoginBonusReward> m_firstLoginBonusRewardList;

	public int m_rewardId;

	public int m_rewardDays;

	public int m_firstRewardDays;

	public ServerLoginBonusReward m_lastBonusReward;

	public ServerLoginBonusReward m_firstLastBonusReward;

	public ServerLoginBonusData()
	{
		m_loginBonusState = new ServerLoginBonusState();
		m_startTime = DateTime.Now;
		m_endTime = DateTime.Now;
		m_loginBonusRewardList = new List<ServerLoginBonusReward>();
		m_firstLoginBonusRewardList = new List<ServerLoginBonusReward>();
		m_rewardId = 0;
		m_rewardDays = 0;
		m_firstRewardDays = 0;
		m_lastBonusReward = null;
		m_firstLastBonusReward = null;
	}

	public void CopyTo(ServerLoginBonusData dest)
	{
		m_loginBonusState.CopyTo(dest.m_loginBonusState);
		dest.m_startTime = m_startTime;
		dest.m_endTime = m_endTime;
		foreach (ServerLoginBonusReward loginBonusReward in m_loginBonusRewardList)
		{
			dest.m_loginBonusRewardList.Add(loginBonusReward);
		}
		foreach (ServerLoginBonusReward firstLoginBonusReward in m_firstLoginBonusRewardList)
		{
			dest.m_firstLoginBonusRewardList.Add(firstLoginBonusReward);
		}
		dest.m_rewardId = m_rewardId;
		dest.m_rewardDays = m_rewardDays;
		dest.m_firstRewardDays = m_firstRewardDays;
	}

	public int CalcTodayCount()
	{
		int result = 0;
		ServerInterface loggedInServerInterface = ServerInterface.LoggedInServerInterface;
		if (loggedInServerInterface != null)
		{
			DateTime startTime = m_startTime;
			DateTime endTime = m_endTime;
			DateTime currentTime = NetUtil.GetCurrentTime();
			if (endTime < currentTime)
			{
				return -1;
			}
			if (startTime < currentTime)
			{
				result = (currentTime - startTime).Days;
			}
			else if (currentTime < startTime)
			{
				return -1;
			}
		}
		return result;
	}

	public int getTotalDays()
	{
		int result = 0;
		DateTime startTime = m_startTime;
		DateTime endTime = m_endTime;
		if (m_startTime < m_endTime)
		{
			result = (m_endTime - m_startTime).Days + 1;
		}
		return result;
	}

	public bool isGetLoginBonusToday()
	{
		DateTime lastBonusTime = m_loginBonusState.m_lastBonusTime;
		DateTime currentTime = NetUtil.GetCurrentTime();
		if (currentTime < lastBonusTime)
		{
			return true;
		}
		if (m_rewardId == -1 || m_rewardDays == -1)
		{
			return true;
		}
		return false;
	}

	public void setLoginBonusList(ServerLoginBonusReward reward, ServerLoginBonusReward firstReward)
	{
		clearLoginBonusList();
		if (reward != null)
		{
			m_lastBonusReward = new ServerLoginBonusReward();
			reward.CopyTo(m_lastBonusReward);
		}
		if (firstReward != null)
		{
			m_firstLastBonusReward = new ServerLoginBonusReward();
			firstReward.CopyTo(m_firstLastBonusReward);
		}
	}

	public void clearLoginBonusList()
	{
		m_lastBonusReward = null;
		m_firstLastBonusReward = null;
	}

	public void replayTodayBonus()
	{
		int numLogin = m_loginBonusState.m_numLogin;
		int numBonus = m_loginBonusState.m_numBonus;
		ServerLoginBonusReward reward = null;
		if (numBonus > 0 && m_loginBonusRewardList != null && m_loginBonusRewardList.Count > 0)
		{
			reward = m_loginBonusRewardList[numBonus - 1];
		}
		ServerLoginBonusReward firstReward = null;
		if (numLogin > 0 && m_firstLoginBonusRewardList != null && m_firstLoginBonusRewardList.Count > 0)
		{
			firstReward = m_firstLoginBonusRewardList[numLogin - 1];
		}
		setLoginBonusList(reward, firstReward);
	}
}
