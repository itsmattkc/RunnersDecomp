using System;
using System.Collections.Generic;

public class ServerDailyChallengeState
{
	public List<ServerDailyChallengeIncentive> m_incentiveList;

	public int m_numIncentiveCont;

	public int m_numDailyChalDay;

	public int m_maxDailyChalDay;

	public int m_maxIncentive;

	public DateTime m_chalEndTime;

	public ServerDailyChallengeState()
	{
		m_incentiveList = new List<ServerDailyChallengeIncentive>();
		m_numIncentiveCont = 0;
		m_numDailyChalDay = 1;
		m_maxDailyChalDay = 1;
		m_maxIncentive = 7;
	}
}
