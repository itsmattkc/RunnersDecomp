using System;
using System.Collections.Generic;

public class ServerLeaderboardEntries
{
	public ServerLeaderboardEntry m_myLeaderboardEntry;

	public List<ServerLeaderboardEntry> m_leaderboardEntries;

	public int m_resultTotalEntries;

	public int m_resetTime;

	public int m_startTime;

	public int m_startIndex;

	public int m_mode;

	public int m_first;

	public int m_count;

	public int m_index;

	public int m_rankingType;

	public int m_eventId;

	public string[] m_friendIdList;

	public ServerLeaderboardEntries()
	{
		m_leaderboardEntries = new List<ServerLeaderboardEntry>();
		m_mode = 0;
		m_first = -1;
		m_count = 0;
		m_rankingType = -1;
		m_index = 0;
		m_eventId = 0;
	}

	public void CopyTo(ServerLeaderboardEntries to)
	{
		if (m_myLeaderboardEntry != null)
		{
			if (to.m_myLeaderboardEntry == null)
			{
				to.m_myLeaderboardEntry = new ServerLeaderboardEntry();
			}
			m_myLeaderboardEntry.CopyTo(to.m_myLeaderboardEntry);
		}
		else
		{
			to.m_myLeaderboardEntry = null;
		}
		to.m_leaderboardEntries.Clear();
		foreach (ServerLeaderboardEntry leaderboardEntry in m_leaderboardEntries)
		{
			ServerLeaderboardEntry serverLeaderboardEntry = new ServerLeaderboardEntry();
			leaderboardEntry.CopyTo(serverLeaderboardEntry);
			to.m_leaderboardEntries.Add(serverLeaderboardEntry);
		}
		to.m_resultTotalEntries = m_resultTotalEntries;
		to.m_resetTime = m_resetTime;
		to.m_startTime = m_startTime;
		to.m_startIndex = m_startIndex;
		to.m_mode = m_mode;
		to.m_first = m_first;
		to.m_count = m_count;
		to.m_index = m_index;
		to.m_rankingType = m_rankingType;
		to.m_eventId = m_eventId;
		if (m_friendIdList != null)
		{
			to.m_friendIdList = new string[m_friendIdList.Length];
			m_friendIdList.CopyTo(to.m_friendIdList, 0);
		}
		else
		{
			to.m_friendIdList = null;
		}
	}

	public bool CompareParam(int mode, int first, int count, int index, int rankingType, int eventId, string[] friendIdList)
	{
		if (mode == m_mode && first == m_first && count == m_count && rankingType == m_rankingType && index == m_index && eventId == m_eventId)
		{
			if (friendIdList == null && m_friendIdList == null)
			{
				return true;
			}
			if (friendIdList == null || m_friendIdList == null || friendIdList.Length != m_friendIdList.Length)
			{
				return false;
			}
			for (int i = 0; i < friendIdList.Length; i++)
			{
				if (friendIdList[i] != m_friendIdList[i])
				{
					return false;
				}
			}
			return true;
		}
		return false;
	}

	public bool IsRivalHighScore()
	{
		return IsRivalHighScore(m_first, m_rankingType);
	}

	public bool IsRivalRanking()
	{
		if (m_rankingType == 4 || m_rankingType == 5)
		{
			return true;
		}
		return false;
	}

	public static bool IsRivalHighScore(int first, int rankingType)
	{
		if (first == 0 && rankingType == 4)
		{
			return true;
		}
		return false;
	}

	public ServerLeaderboardEntry GetRankTop()
	{
		if (m_leaderboardEntries != null && m_leaderboardEntries.Count > 0 && m_leaderboardEntries[0].m_grade == 1)
		{
			return m_leaderboardEntries[0];
		}
		return null;
	}

	public bool IsNext()
	{
		bool result = false;
		if (m_leaderboardEntries != null && m_count <= m_leaderboardEntries.Count)
		{
			result = true;
		}
		return result;
	}

	public bool GetNextRanking(ref int top, ref int count, int margin)
	{
		if (!IsNext())
		{
			return false;
		}
		top = m_count - margin + 1;
		if (top < 1)
		{
			count = margin + count + top;
			top = 1;
		}
		else
		{
			count = margin + count;
		}
		return true;
	}

	public bool IsPrev()
	{
		bool result = false;
		if (m_leaderboardEntries != null && m_leaderboardEntries.Count > 0)
		{
			if (m_first > 1)
			{
				result = true;
			}
			else if (m_first == 0 && m_leaderboardEntries[0].m_grade > 1)
			{
				result = true;
			}
		}
		return result;
	}

	public bool GetPrevRanking(ref int top, ref int count, int margin)
	{
		if (!IsPrev())
		{
			return false;
		}
		top = m_first - count;
		if (top < 1)
		{
			count += top - 1;
			top = 1;
		}
		return true;
	}

	public bool IsReload()
	{
		bool result = false;
		DateTime localDateTime = NetUtil.GetLocalDateTime(m_resetTime);
		if (localDateTime != default(DateTime) && NetUtil.GetCurrentTime() > localDateTime)
		{
			result = true;
		}
		return result;
	}

	public TimeSpan GetResetTimeSpan()
	{
		TimeSpan result = default(TimeSpan);
		DateTime localDateTime = NetUtil.GetLocalDateTime(m_resetTime);
		if (localDateTime != default(DateTime))
		{
			return localDateTime - NetUtil.GetCurrentTime();
		}
		return result;
	}

	public bool UpdateSendChallenge(string id)
	{
		bool result = false;
		if (m_leaderboardEntries != null && m_leaderboardEntries.Count > 0)
		{
			foreach (ServerLeaderboardEntry leaderboardEntry in m_leaderboardEntries)
			{
				if (id == leaderboardEntry.m_hspId)
				{
					leaderboardEntry.m_energyFlg = true;
					result = true;
				}
			}
			return result;
		}
		return result;
	}
}
