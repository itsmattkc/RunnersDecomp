using Message;
using System;
using System.Collections.Generic;

public class RankingDataContainer
{
	private Dictionary<RankingUtil.RankingRankerType, Dictionary<RankingUtil.RankingScoreType, List<MsgGetLeaderboardEntriesSucceed>>> m_rankingData;

	private Dictionary<RankingUtil.RankingRankerType, Dictionary<RankingUtil.RankingScoreType, int>> m_rankingUserOldRank;

	public RankingDataContainer()
	{
		Reset();
	}

	public void Reset(RankingUtil.RankingRankerType type)
	{
		if (m_rankingData != null && m_rankingData.Count > 0 && m_rankingData.ContainsKey(type))
		{
			m_rankingData[type].Clear();
			m_rankingData[type] = new Dictionary<RankingUtil.RankingScoreType, List<MsgGetLeaderboardEntriesSucceed>>();
		}
	}

	public void Reset()
	{
		if (m_rankingData != null && m_rankingData.Count > 0)
		{
			for (int i = 0; i < 6; i++)
			{
				RankingUtil.RankingRankerType key = (RankingUtil.RankingRankerType)i;
				m_rankingData[key].Clear();
				m_rankingData[key] = new Dictionary<RankingUtil.RankingScoreType, List<MsgGetLeaderboardEntriesSucceed>>();
			}
			return;
		}
		m_rankingData = new Dictionary<RankingUtil.RankingRankerType, Dictionary<RankingUtil.RankingScoreType, List<MsgGetLeaderboardEntriesSucceed>>>();
		m_rankingUserOldRank = null;
		for (int j = 0; j < 6; j++)
		{
			RankingUtil.RankingRankerType key2 = (RankingUtil.RankingRankerType)j;
			m_rankingData.Add(key2, new Dictionary<RankingUtil.RankingScoreType, List<MsgGetLeaderboardEntriesSucceed>>());
		}
	}

	public void SavePlayerRanking()
	{
		if (m_rankingUserOldRank == null)
		{
			m_rankingUserOldRank = new Dictionary<RankingUtil.RankingRankerType, Dictionary<RankingUtil.RankingScoreType, int>>();
			for (int i = 0; i < 6; i++)
			{
				RankingUtil.RankingRankerType key = (RankingUtil.RankingRankerType)i;
				m_rankingUserOldRank.Add(key, new Dictionary<RankingUtil.RankingScoreType, int>());
				m_rankingUserOldRank[key].Add(RankingUtil.RankingScoreType.HIGH_SCORE, -1);
				m_rankingUserOldRank[key].Add(RankingUtil.RankingScoreType.TOTAL_SCORE, -1);
			}
		}
		else
		{
			for (int j = 0; j < 6; j++)
			{
				RankingUtil.RankingRankerType key2 = (RankingUtil.RankingRankerType)j;
				m_rankingUserOldRank[key2][RankingUtil.RankingScoreType.HIGH_SCORE] = -1;
				m_rankingUserOldRank[key2][RankingUtil.RankingScoreType.TOTAL_SCORE] = -1;
			}
		}
		if (m_rankingData == null || m_rankingData.Count <= 0)
		{
			return;
		}
		for (int k = 0; k < 6; k++)
		{
			RankingUtil.RankingRankerType key3 = (RankingUtil.RankingRankerType)k;
			if (!m_rankingData.ContainsKey(key3))
			{
				continue;
			}
			if (m_rankingData[key3].ContainsKey(RankingUtil.RankingScoreType.HIGH_SCORE))
			{
				List<MsgGetLeaderboardEntriesSucceed> list = m_rankingData[key3][RankingUtil.RankingScoreType.HIGH_SCORE];
				if (list != null && list.Count > 0 && list[0] != null && list[0].m_leaderboardEntries != null && list[0].m_leaderboardEntries.m_myLeaderboardEntry != null)
				{
					ServerLeaderboardEntry myLeaderboardEntry = list[0].m_leaderboardEntries.m_myLeaderboardEntry;
					if (m_rankingUserOldRank != null && m_rankingUserOldRank.ContainsKey(key3) && m_rankingUserOldRank[key3] != null && m_rankingUserOldRank[key3].ContainsKey(RankingUtil.RankingScoreType.HIGH_SCORE))
					{
						m_rankingUserOldRank[key3][RankingUtil.RankingScoreType.HIGH_SCORE] = myLeaderboardEntry.m_grade;
					}
				}
			}
			if (!m_rankingData[key3].ContainsKey(RankingUtil.RankingScoreType.TOTAL_SCORE))
			{
				continue;
			}
			List<MsgGetLeaderboardEntriesSucceed> list2 = m_rankingData[key3][RankingUtil.RankingScoreType.TOTAL_SCORE];
			if (list2 != null && list2.Count > 0 && list2[0] != null && list2[0].m_leaderboardEntries != null && list2[0].m_leaderboardEntries.m_myLeaderboardEntry != null)
			{
				ServerLeaderboardEntry myLeaderboardEntry2 = list2[0].m_leaderboardEntries.m_myLeaderboardEntry;
				if (m_rankingUserOldRank != null && m_rankingUserOldRank.ContainsKey(key3) && m_rankingUserOldRank[key3] != null && m_rankingUserOldRank[key3].ContainsKey(RankingUtil.RankingScoreType.TOTAL_SCORE))
				{
					m_rankingUserOldRank[key3][RankingUtil.RankingScoreType.TOTAL_SCORE] = myLeaderboardEntry2.m_grade;
				}
			}
		}
	}

	public void SavePlayerRankingDummy(RankingUtil.RankingRankerType rankType, RankingUtil.RankingScoreType scoreType, int dammyRank)
	{
		if (m_rankingUserOldRank == null)
		{
			m_rankingUserOldRank = new Dictionary<RankingUtil.RankingRankerType, Dictionary<RankingUtil.RankingScoreType, int>>();
			for (int i = 0; i < 6; i++)
			{
				RankingUtil.RankingRankerType key = (RankingUtil.RankingRankerType)i;
				m_rankingUserOldRank.Add(key, new Dictionary<RankingUtil.RankingScoreType, int>());
				m_rankingUserOldRank[key].Add(RankingUtil.RankingScoreType.HIGH_SCORE, -1);
				m_rankingUserOldRank[key].Add(RankingUtil.RankingScoreType.TOTAL_SCORE, -1);
			}
		}
		else
		{
			for (int j = 0; j < 6; j++)
			{
				RankingUtil.RankingRankerType key2 = (RankingUtil.RankingRankerType)j;
				m_rankingUserOldRank[key2][RankingUtil.RankingScoreType.HIGH_SCORE] = -1;
				m_rankingUserOldRank[key2][RankingUtil.RankingScoreType.TOTAL_SCORE] = -1;
			}
		}
		if (m_rankingData == null || m_rankingData.Count <= 0)
		{
			return;
		}
		for (int k = 0; k < 6; k++)
		{
			RankingUtil.RankingRankerType rankingRankerType = (RankingUtil.RankingRankerType)k;
			if (!m_rankingData.ContainsKey(rankingRankerType))
			{
				continue;
			}
			if (m_rankingData[rankingRankerType].ContainsKey(RankingUtil.RankingScoreType.HIGH_SCORE))
			{
				List<MsgGetLeaderboardEntriesSucceed> list = m_rankingData[rankingRankerType][RankingUtil.RankingScoreType.HIGH_SCORE];
				if (list != null && list.Count > 0 && list[0] != null && list[0].m_leaderboardEntries != null && list[0].m_leaderboardEntries.m_myLeaderboardEntry != null)
				{
					ServerLeaderboardEntry myLeaderboardEntry = list[0].m_leaderboardEntries.m_myLeaderboardEntry;
					if (m_rankingUserOldRank != null && m_rankingUserOldRank.ContainsKey(rankingRankerType) && m_rankingUserOldRank[rankingRankerType] != null && m_rankingUserOldRank[rankingRankerType].ContainsKey(RankingUtil.RankingScoreType.HIGH_SCORE))
					{
						if (rankingRankerType == rankType && scoreType == RankingUtil.RankingScoreType.HIGH_SCORE)
						{
							m_rankingUserOldRank[rankingRankerType][RankingUtil.RankingScoreType.HIGH_SCORE] = dammyRank;
						}
						else
						{
							m_rankingUserOldRank[rankingRankerType][RankingUtil.RankingScoreType.HIGH_SCORE] = myLeaderboardEntry.m_grade;
						}
					}
				}
			}
			if (!m_rankingData[rankingRankerType].ContainsKey(RankingUtil.RankingScoreType.TOTAL_SCORE))
			{
				continue;
			}
			List<MsgGetLeaderboardEntriesSucceed> list2 = m_rankingData[rankingRankerType][RankingUtil.RankingScoreType.TOTAL_SCORE];
			if (list2 == null || list2.Count <= 0 || list2[0] == null || list2[0].m_leaderboardEntries == null || list2[0].m_leaderboardEntries.m_myLeaderboardEntry == null)
			{
				continue;
			}
			ServerLeaderboardEntry myLeaderboardEntry2 = list2[0].m_leaderboardEntries.m_myLeaderboardEntry;
			if (m_rankingUserOldRank != null && m_rankingUserOldRank.ContainsKey(rankingRankerType) && m_rankingUserOldRank[rankingRankerType] != null && m_rankingUserOldRank[rankingRankerType].ContainsKey(RankingUtil.RankingScoreType.TOTAL_SCORE))
			{
				if (rankingRankerType == rankType && scoreType == RankingUtil.RankingScoreType.TOTAL_SCORE)
				{
					m_rankingUserOldRank[rankingRankerType][RankingUtil.RankingScoreType.TOTAL_SCORE] = dammyRank;
				}
				else
				{
					m_rankingUserOldRank[rankingRankerType][RankingUtil.RankingScoreType.TOTAL_SCORE] = myLeaderboardEntry2.m_grade;
				}
			}
		}
	}

	public bool UpdateSendChallengeList(RankingUtil.RankingRankerType type, string id)
	{
		bool result = false;
		if (m_rankingData != null && m_rankingData.ContainsKey(type) && m_rankingData[type] != null && m_rankingData[type].Count > 0)
		{
			if (m_rankingData[type].ContainsKey(RankingUtil.RankingScoreType.HIGH_SCORE) && m_rankingData[type][RankingUtil.RankingScoreType.HIGH_SCORE].Count > 0)
			{
				int count = m_rankingData[type][RankingUtil.RankingScoreType.HIGH_SCORE].Count;
				if (m_rankingData[type][RankingUtil.RankingScoreType.HIGH_SCORE][0] != null && m_rankingData[type][RankingUtil.RankingScoreType.HIGH_SCORE][0].m_leaderboardEntries != null)
				{
					result = m_rankingData[type][RankingUtil.RankingScoreType.HIGH_SCORE][0].m_leaderboardEntries.UpdateSendChallenge(id);
				}
				if (type != RankingUtil.RankingRankerType.RIVAL)
				{
					if (count > 1 && m_rankingData[type][RankingUtil.RankingScoreType.HIGH_SCORE][1] != null && m_rankingData[type][RankingUtil.RankingScoreType.HIGH_SCORE][1].m_leaderboardEntries != null)
					{
						bool flag = m_rankingData[type][RankingUtil.RankingScoreType.HIGH_SCORE][1].m_leaderboardEntries.UpdateSendChallenge(id);
						if (flag)
						{
							result = flag;
						}
					}
					if (count > 2 && m_rankingData[type][RankingUtil.RankingScoreType.HIGH_SCORE][2] != null && m_rankingData[type][RankingUtil.RankingScoreType.HIGH_SCORE][2].m_leaderboardEntries != null)
					{
						bool flag2 = m_rankingData[type][RankingUtil.RankingScoreType.HIGH_SCORE][2].m_leaderboardEntries.UpdateSendChallenge(id);
						if (flag2)
						{
							result = flag2;
						}
					}
				}
			}
			if (m_rankingData[type].ContainsKey(RankingUtil.RankingScoreType.TOTAL_SCORE) && m_rankingData[type][RankingUtil.RankingScoreType.TOTAL_SCORE].Count > 0)
			{
				int count2 = m_rankingData[type][RankingUtil.RankingScoreType.TOTAL_SCORE].Count;
				if (m_rankingData[type][RankingUtil.RankingScoreType.TOTAL_SCORE][0] != null && m_rankingData[type][RankingUtil.RankingScoreType.TOTAL_SCORE][0].m_leaderboardEntries != null)
				{
					bool flag3 = m_rankingData[type][RankingUtil.RankingScoreType.TOTAL_SCORE][0].m_leaderboardEntries.UpdateSendChallenge(id);
					if (flag3)
					{
						result = flag3;
					}
				}
				if (type != RankingUtil.RankingRankerType.RIVAL)
				{
					if (count2 > 1 && m_rankingData[type][RankingUtil.RankingScoreType.TOTAL_SCORE][1] != null && m_rankingData[type][RankingUtil.RankingScoreType.TOTAL_SCORE][1].m_leaderboardEntries != null)
					{
						bool flag4 = m_rankingData[type][RankingUtil.RankingScoreType.TOTAL_SCORE][1].m_leaderboardEntries.UpdateSendChallenge(id);
						if (flag4)
						{
							result = flag4;
						}
					}
					if (count2 > 2 && m_rankingData[type][RankingUtil.RankingScoreType.TOTAL_SCORE][2] != null && m_rankingData[type][RankingUtil.RankingScoreType.TOTAL_SCORE][2].m_leaderboardEntries != null)
					{
						bool flag5 = m_rankingData[type][RankingUtil.RankingScoreType.TOTAL_SCORE][2].m_leaderboardEntries.UpdateSendChallenge(id);
						if (flag5)
						{
							result = flag5;
						}
					}
				}
			}
		}
		return result;
	}

	public RankingUtil.RankChange GetRankChange(RankingUtil.RankingScoreType scoreType, RankingUtil.RankingRankerType rankerType)
	{
		RankingUtil.RankChange result = RankingUtil.RankChange.NONE;
		if (m_rankingUserOldRank != null && m_rankingUserOldRank.ContainsKey(rankerType) && m_rankingData != null && m_rankingData.ContainsKey(rankerType) && m_rankingUserOldRank[rankerType].ContainsKey(scoreType) && m_rankingData[rankerType].ContainsKey(scoreType) && m_rankingData[rankerType][scoreType].Count > 0)
		{
			int num = m_rankingUserOldRank[rankerType][scoreType];
			if (num >= 0)
			{
				MsgGetLeaderboardEntriesSucceed msgGetLeaderboardEntriesSucceed = m_rankingData[rankerType][scoreType][0];
				if (msgGetLeaderboardEntriesSucceed != null && msgGetLeaderboardEntriesSucceed.m_leaderboardEntries != null && msgGetLeaderboardEntriesSucceed.m_leaderboardEntries.m_myLeaderboardEntry != null)
				{
					if (msgGetLeaderboardEntriesSucceed.m_leaderboardEntries.m_myLeaderboardEntry.m_grade == num)
					{
						result = RankingUtil.RankChange.STAY;
					}
					else if (msgGetLeaderboardEntriesSucceed.m_leaderboardEntries.m_myLeaderboardEntry.m_grade < num)
					{
						result = RankingUtil.RankChange.UP;
					}
					else if (msgGetLeaderboardEntriesSucceed.m_leaderboardEntries.m_myLeaderboardEntry.m_grade > num)
					{
						result = RankingUtil.RankChange.DOWN;
					}
				}
			}
		}
		return result;
	}

	public RankingUtil.RankChange GetRankChange(RankingUtil.RankingScoreType scoreType, RankingUtil.RankingRankerType rankerType, out int currentRank, out int oldRank)
	{
		RankingUtil.RankChange result = RankingUtil.RankChange.NONE;
		currentRank = -1;
		oldRank = -1;
		if (m_rankingData != null && m_rankingData.ContainsKey(rankerType) && m_rankingData[rankerType].ContainsKey(scoreType) && m_rankingData[rankerType][scoreType].Count > 0)
		{
			MsgGetLeaderboardEntriesSucceed msgGetLeaderboardEntriesSucceed = m_rankingData[rankerType][scoreType][0];
			if (m_rankingUserOldRank != null && m_rankingUserOldRank.ContainsKey(rankerType) && m_rankingUserOldRank[rankerType].ContainsKey(scoreType))
			{
				oldRank = m_rankingUserOldRank[rankerType][scoreType];
				if (oldRank >= 0)
				{
					if (msgGetLeaderboardEntriesSucceed != null && msgGetLeaderboardEntriesSucceed.m_leaderboardEntries != null && msgGetLeaderboardEntriesSucceed.m_leaderboardEntries.m_myLeaderboardEntry != null)
					{
						currentRank = msgGetLeaderboardEntriesSucceed.m_leaderboardEntries.m_myLeaderboardEntry.m_grade;
						if (msgGetLeaderboardEntriesSucceed.m_leaderboardEntries.m_myLeaderboardEntry.m_grade == oldRank)
						{
							result = RankingUtil.RankChange.STAY;
						}
						else if (msgGetLeaderboardEntriesSucceed.m_leaderboardEntries.m_myLeaderboardEntry.m_grade < oldRank)
						{
							result = RankingUtil.RankChange.UP;
						}
						else if (msgGetLeaderboardEntriesSucceed.m_leaderboardEntries.m_myLeaderboardEntry.m_grade > oldRank)
						{
							result = RankingUtil.RankChange.DOWN;
						}
					}
				}
				else
				{
					currentRank = msgGetLeaderboardEntriesSucceed.m_leaderboardEntries.m_myLeaderboardEntry.m_grade;
					oldRank = currentRank;
				}
			}
			else
			{
				currentRank = msgGetLeaderboardEntriesSucceed.m_leaderboardEntries.m_myLeaderboardEntry.m_grade;
				oldRank = currentRank;
			}
		}
		return result;
	}

	public void ResetRankChange(RankingUtil.RankingScoreType scoreType, RankingUtil.RankingRankerType rankerType)
	{
		if (m_rankingUserOldRank != null && m_rankingUserOldRank.ContainsKey(rankerType) && m_rankingUserOldRank[rankerType].ContainsKey(scoreType))
		{
			m_rankingUserOldRank[rankerType][scoreType] = -1;
		}
	}

	public bool IsRankerType(RankingUtil.RankingRankerType rankerType)
	{
		bool result = false;
		if (rankerType != RankingUtil.RankingRankerType.COUNT && m_rankingData != null && m_rankingData.ContainsKey(rankerType))
		{
			result = true;
		}
		return result;
	}

	public bool IsRankerType(RankingUtil.RankingRankerType rankerType, out Dictionary<RankingUtil.RankingScoreType, List<MsgGetLeaderboardEntriesSucceed>> data)
	{
		bool result = false;
		data = null;
		if (IsRankerType(rankerType))
		{
			data = m_rankingData[rankerType];
			result = true;
		}
		return result;
	}

	public bool IsRankerListNext(RankingUtil.RankingRankerType rankerType, RankingUtil.RankingScoreType scoreType)
	{
		bool result = false;
		List<MsgGetLeaderboardEntriesSucceed> data;
		if (IsRankerAndScoreType(rankerType, scoreType, out data, 1) && data != null && data.Count > 1)
		{
			MsgGetLeaderboardEntriesSucceed msgGetLeaderboardEntriesSucceed = data[1];
			if (msgGetLeaderboardEntriesSucceed != null && msgGetLeaderboardEntriesSucceed.m_leaderboardEntries != null)
			{
				result = msgGetLeaderboardEntriesSucceed.m_leaderboardEntries.IsNext();
			}
		}
		return result;
	}

	public bool IsRankerListPrev(RankingUtil.RankingRankerType rankerType, RankingUtil.RankingScoreType scoreType)
	{
		bool result = false;
		List<MsgGetLeaderboardEntriesSucceed> data;
		if (IsRankerAndScoreType(rankerType, scoreType, out data, 1) && data != null && data.Count > 1)
		{
			MsgGetLeaderboardEntriesSucceed msgGetLeaderboardEntriesSucceed = data[1];
			if (msgGetLeaderboardEntriesSucceed != null && msgGetLeaderboardEntriesSucceed.m_leaderboardEntries != null)
			{
				result = msgGetLeaderboardEntriesSucceed.m_leaderboardEntries.IsPrev();
			}
		}
		return result;
	}

	public bool IsRankerAndScoreType(RankingUtil.RankingRankerType rankerType, RankingUtil.RankingScoreType scoreType, int page = -1)
	{
		bool result = false;
		if (rankerType != RankingUtil.RankingRankerType.COUNT && m_rankingData != null && m_rankingData.ContainsKey(rankerType) && m_rankingData[rankerType] != null && m_rankingData[rankerType].Count > 0 && m_rankingData[rankerType].ContainsKey(scoreType))
		{
			if (page < 0 || rankerType == RankingUtil.RankingRankerType.RIVAL)
			{
				if (m_rankingData[rankerType][scoreType].Count > 0)
				{
					result = true;
				}
			}
			else if (m_rankingData[rankerType][scoreType].Count >= page + 1)
			{
				result = true;
			}
		}
		return result;
	}

	public bool IsRankerAndScoreType(RankingUtil.RankingRankerType rankerType, RankingUtil.RankingScoreType scoreType, out List<MsgGetLeaderboardEntriesSucceed> data, int page = -1)
	{
		bool result = false;
		data = null;
		if (IsRankerAndScoreType(rankerType, scoreType, page))
		{
			data = m_rankingData[rankerType][scoreType];
			result = true;
		}
		return result;
	}

	public List<RankingUtil.Ranker> GetRankerList(RankingUtil.RankingRankerType rankerType, RankingUtil.RankingScoreType scoreType, int page)
	{
		List<RankingUtil.Ranker> result = null;
		List<MsgGetLeaderboardEntriesSucceed> data;
		if (IsRankerAndScoreType(rankerType, scoreType, out data) && data != null && data.Count > 0 && data.Count > page)
		{
			result = RankingUtil.GetRankerList(data[page]);
		}
		return result;
	}

	public bool IsRankerListReload(RankingUtil.RankingRankerType rankerType, RankingUtil.RankingScoreType scoreType)
	{
		bool flag = false;
		List<MsgGetLeaderboardEntriesSucceed> data;
		if (IsRankerAndScoreType(rankerType, scoreType, out data, 0))
		{
			if ((data != null) & (data.Count > 0))
			{
				if (data[0] != null)
				{
					ServerLeaderboardEntries leaderboardEntries = data[0].m_leaderboardEntries;
					return leaderboardEntries.IsReload();
				}
				return true;
			}
			return true;
		}
		return true;
	}

	public TimeSpan GetResetTimeSpan(RankingUtil.RankingRankerType rankerType, RankingUtil.RankingScoreType scoreType)
	{
		MsgGetLeaderboardEntriesSucceed rankerListOrg = GetRankerListOrg(rankerType, scoreType);
		if (rankerListOrg != null)
		{
			return rankerListOrg.m_leaderboardEntries.GetResetTimeSpan();
		}
		return NetUtil.GetCurrentTime() - NetUtil.GetCurrentTime();
	}

	public MsgGetLeaderboardEntriesSucceed GetRankerListOrg(RankingUtil.RankingRankerType rankerType, RankingUtil.RankingScoreType scoreType, int page = 0)
	{
		MsgGetLeaderboardEntriesSucceed result = null;
		if (rankerType == RankingUtil.RankingRankerType.RIVAL)
		{
			page = 0;
		}
		List<MsgGetLeaderboardEntriesSucceed> data;
		if (IsRankerAndScoreType(rankerType, scoreType, out data, page) && data != null)
		{
			if (page > 1)
			{
				page = 1;
			}
			if (data.Count > page)
			{
				result = data[page];
			}
		}
		return result;
	}

	public void AddRankerList(MsgGetLeaderboardEntriesSucceed msg)
	{
		ServerLeaderboardEntries leaderboardEntries = msg.m_leaderboardEntries;
		int rankerPage = GetRankerPage(msg);
		int rankingType = leaderboardEntries.m_rankingType;
		int num = rankingType % 2;
		RankingUtil.RankingRankerType rankingRankerType = (RankingUtil.RankingRankerType)(rankingType / 2);
		RankingUtil.RankingScoreType key = (RankingUtil.RankingScoreType)num;
		Dictionary<RankingUtil.RankingScoreType, List<MsgGetLeaderboardEntriesSucceed>> data;
		IsRankerType(rankingRankerType, out data);
		if (data == null)
		{
			return;
		}
		if (!data.ContainsKey(key))
		{
			data.Add(key, new List<MsgGetLeaderboardEntriesSucceed>());
		}
		switch (rankerPage)
		{
		case 0:
			if (data[key].Count == 0)
			{
				data[key].Add(RankingUtil.CopyRankingMsg(msg));
			}
			else
			{
				data[key][0] = RankingUtil.CopyRankingMsg(msg);
			}
			if (rankingRankerType != RankingUtil.RankingRankerType.RIVAL)
			{
				if (data[key].Count == 1)
				{
					data[key].Add(RankingUtil.CopyRankingMsg(msg));
				}
				else
				{
					data[key][1] = RankingUtil.CopyRankingMsg(msg);
				}
			}
			return;
		case 1:
			if (data[key].Count == 0)
			{
				data[key].Add(RankingUtil.InitRankingMsg(msg));
				data[key].Add(RankingUtil.CopyRankingMsg(msg));
			}
			else if (data[key].Count == 1)
			{
				data[key][0].m_leaderboardEntries.m_resetTime = msg.m_leaderboardEntries.m_resetTime;
				data[key][0].m_leaderboardEntries.m_startTime = msg.m_leaderboardEntries.m_startTime;
				if (rankingRankerType != RankingUtil.RankingRankerType.RIVAL && msg.m_leaderboardEntries.m_myLeaderboardEntry != null && msg.m_leaderboardEntries.m_myLeaderboardEntry.m_score > 0 && msg.m_leaderboardEntries.m_myLeaderboardEntry.m_grade > 0)
				{
					msg.m_leaderboardEntries.m_myLeaderboardEntry.CopyTo(data[key][0].m_leaderboardEntries.m_myLeaderboardEntry);
				}
				data[key].Add(RankingUtil.CopyRankingMsg(msg));
			}
			else
			{
				data[key][0].m_leaderboardEntries.m_resetTime = msg.m_leaderboardEntries.m_resetTime;
				data[key][0].m_leaderboardEntries.m_startTime = msg.m_leaderboardEntries.m_startTime;
				if (rankingRankerType != RankingUtil.RankingRankerType.RIVAL && msg.m_leaderboardEntries.m_myLeaderboardEntry != null && msg.m_leaderboardEntries.m_myLeaderboardEntry.m_score > 0 && msg.m_leaderboardEntries.m_myLeaderboardEntry.m_grade > 0)
				{
					msg.m_leaderboardEntries.m_myLeaderboardEntry.CopyTo(data[key][0].m_leaderboardEntries.m_myLeaderboardEntry);
				}
				data[key][1] = RankingUtil.CopyRankingMsg(msg);
			}
			return;
		case 2:
			if (data[key].Count == 0)
			{
				data[key].Add(null);
				data[key].Add(RankingUtil.CopyRankingMsg(msg));
				return;
			}
			if (data[key].Count == 1)
			{
				data[key][0].m_leaderboardEntries.m_resetTime = msg.m_leaderboardEntries.m_resetTime;
				data[key][0].m_leaderboardEntries.m_startTime = msg.m_leaderboardEntries.m_startTime;
				if (rankingRankerType != RankingUtil.RankingRankerType.RIVAL && msg.m_leaderboardEntries.m_myLeaderboardEntry != null && msg.m_leaderboardEntries.m_myLeaderboardEntry.m_score > 0 && msg.m_leaderboardEntries.m_myLeaderboardEntry.m_grade > 0)
				{
					msg.m_leaderboardEntries.m_myLeaderboardEntry.CopyTo(data[key][0].m_leaderboardEntries.m_myLeaderboardEntry);
				}
				data[key].Add(RankingUtil.CopyRankingMsg(msg));
				return;
			}
			data[key][0].m_leaderboardEntries.m_resetTime = msg.m_leaderboardEntries.m_resetTime;
			data[key][0].m_leaderboardEntries.m_startTime = msg.m_leaderboardEntries.m_startTime;
			if (data[key][1] != null)
			{
				data[key][1] = RankingUtil.CopyRankingMsg(msg, data[key][1]);
			}
			else
			{
				data[key][1] = RankingUtil.CopyRankingMsg(msg);
			}
			if (rankingRankerType != RankingUtil.RankingRankerType.RIVAL && msg.m_leaderboardEntries.m_myLeaderboardEntry != null && msg.m_leaderboardEntries.m_myLeaderboardEntry.m_score > 0 && msg.m_leaderboardEntries.m_myLeaderboardEntry.m_grade > 0)
			{
				msg.m_leaderboardEntries.m_myLeaderboardEntry.CopyTo(data[key][0].m_leaderboardEntries.m_myLeaderboardEntry);
			}
			msg = data[key][1];
			return;
		}
		if (data[key].Count == 0)
		{
			data[key].Add(null);
			data[key].Add(null);
			data[key].Add(RankingUtil.CopyRankingMsg(msg));
		}
		else if (data[key].Count == 1)
		{
			data[key][0].m_leaderboardEntries.m_resetTime = msg.m_leaderboardEntries.m_resetTime;
			data[key][0].m_leaderboardEntries.m_startTime = msg.m_leaderboardEntries.m_startTime;
			data[key].Add(null);
			data[key].Add(RankingUtil.CopyRankingMsg(msg));
			if (rankingRankerType != RankingUtil.RankingRankerType.RIVAL && msg.m_leaderboardEntries.m_myLeaderboardEntry != null && msg.m_leaderboardEntries.m_myLeaderboardEntry.m_score > 0 && msg.m_leaderboardEntries.m_myLeaderboardEntry.m_grade > 0)
			{
				msg.m_leaderboardEntries.m_myLeaderboardEntry.CopyTo(data[key][0].m_leaderboardEntries.m_myLeaderboardEntry);
			}
		}
		else if (data[key].Count == 2)
		{
			data[key][0].m_leaderboardEntries.m_resetTime = msg.m_leaderboardEntries.m_resetTime;
			data[key][0].m_leaderboardEntries.m_startTime = msg.m_leaderboardEntries.m_startTime;
			data[key].Add(RankingUtil.CopyRankingMsg(msg));
			if (rankingRankerType != RankingUtil.RankingRankerType.RIVAL && msg.m_leaderboardEntries.m_myLeaderboardEntry != null && msg.m_leaderboardEntries.m_myLeaderboardEntry.m_score > 0 && msg.m_leaderboardEntries.m_myLeaderboardEntry.m_grade > 0)
			{
				msg.m_leaderboardEntries.m_myLeaderboardEntry.CopyTo(data[key][0].m_leaderboardEntries.m_myLeaderboardEntry);
			}
		}
		else
		{
			data[key][0].m_leaderboardEntries.m_resetTime = msg.m_leaderboardEntries.m_resetTime;
			data[key][0].m_leaderboardEntries.m_startTime = msg.m_leaderboardEntries.m_startTime;
			data[key][2] = RankingUtil.CopyRankingMsg(msg);
			if (rankingRankerType != RankingUtil.RankingRankerType.RIVAL && msg.m_leaderboardEntries.m_myLeaderboardEntry != null && msg.m_leaderboardEntries.m_myLeaderboardEntry.m_score > 0 && msg.m_leaderboardEntries.m_myLeaderboardEntry.m_grade > 0)
			{
				msg.m_leaderboardEntries.m_myLeaderboardEntry.CopyTo(data[key][0].m_leaderboardEntries.m_myLeaderboardEntry);
			}
		}
	}

	private int GetRankerPage(MsgGetLeaderboardEntriesSucceed msg)
	{
		int result = 0;
		if (msg != null && msg.m_leaderboardEntries != null)
		{
			result = msg.m_leaderboardEntries.m_index;
			if (msg.m_leaderboardEntries.IsRivalRanking())
			{
				result = 0;
			}
		}
		return result;
	}

	public int GetCurrentHighScoreRank(bool isEvent, ref long currentScore, out bool isHighScore, out long nextRankScore, out long prveRankScore, out int nextRank)
	{
		int num = 0;
		nextRankScore = -1L;
		prveRankScore = -1L;
		isHighScore = false;
		nextRank = 0;
		bool flag = false;
		List<RankingUtil.Ranker> list = null;
		RankingUtil.RankingScoreType rankingScoreType = RankingUtil.RankingScoreType.HIGH_SCORE;
		RankingUtil.RankingRankerType rankingRankerType = RankingUtil.RankingRankerType.RIVAL;
		if (isEvent)
		{
			rankingScoreType = RankingManager.EndlessSpecialRankingScoreType;
			rankingRankerType = RankingUtil.RankingRankerType.SP_ALL;
			if (rankingRankerType == RankingUtil.RankingRankerType.RIVAL)
			{
				list = GetRankerList(rankingRankerType, rankingScoreType, 0);
			}
			else
			{
				list = GetRankerList(rankingRankerType, rankingScoreType, 2);
				if (list == null)
				{
					list = GetRankerList(rankingRankerType, rankingScoreType, 1);
				}
				else
				{
					flag = true;
				}
			}
			if (list == null || list.Count <= 0)
			{
				isHighScore = true;
				return 0;
			}
			if (list[0] == null)
			{
				Debug.Log("RankingManager GetCurrentHighScoreRank  first try!");
				isHighScore = true;
			}
			else if (list[0].score <= 0)
			{
				Debug.Log("RankingManager GetCurrentHighScoreRank  first try!");
				isHighScore = true;
			}
			if (isHighScore)
			{
				return 0;
			}
		}
		else
		{
			rankingScoreType = RankingManager.EndlessRivalRankingScoreType;
			rankingRankerType = RankingUtil.RankingRankerType.RIVAL;
			if (rankingRankerType == RankingUtil.RankingRankerType.RIVAL)
			{
				list = GetRankerList(rankingRankerType, rankingScoreType, 0);
			}
			else
			{
				list = GetRankerList(rankingRankerType, rankingScoreType, 2);
				if (list == null)
				{
					list = GetRankerList(rankingRankerType, rankingScoreType, 1);
				}
				else
				{
					flag = true;
				}
			}
			if (list != null && list.Count > 0)
			{
				if (list[0] == null)
				{
					Debug.Log("RankingManager GetCurrentHighScoreRank  first try!");
					isHighScore = true;
				}
				else if (list[0].score <= 0)
				{
					Debug.Log("RankingManager GetCurrentHighScoreRank  first try!");
					isHighScore = true;
				}
			}
		}
		RankingUtil.Ranker ranker = list[0];
		if (list != null)
		{
			if (ranker != null)
			{
				if (currentScore > ranker.hiScore)
				{
					isHighScore = true;
				}
			}
			else
			{
				isHighScore = true;
			}
		}
		num = CheckRankingList(rankingRankerType, rankingScoreType, list, ref currentScore, ref isHighScore, out nextRankScore, out prveRankScore);
		if (num > 1 && nextRankScore == -1 && flag)
		{
			isHighScore = true;
			list = GetRankerList(rankingRankerType, rankingScoreType, 1);
			if (list != null && list.Count > 1)
			{
				if (list[list.Count - 1].score > currentScore)
				{
					CheckRankingList(rankingRankerType, rankingScoreType, list, ref currentScore, ref isHighScore, out nextRankScore, out prveRankScore);
					nextRank = list[list.Count - 1].rankIndex + 1;
				}
				else
				{
					num = CheckRankingList(rankingRankerType, rankingScoreType, list, ref currentScore, ref isHighScore, out nextRankScore, out prveRankScore);
					nextRank = num - 1;
				}
			}
			else
			{
				nextRank = num - 1;
			}
		}
		else
		{
			nextRank = num - 1;
		}
		return num;
	}

	private int CheckRankingList(RankingUtil.RankingRankerType rankerType, RankingUtil.RankingScoreType scoreType, List<RankingUtil.Ranker> list, ref long currentScore, ref bool isHighScore, out long nextRankScore, out long prveRankScore)
	{
		nextRankScore = -1L;
		prveRankScore = -1L;
		int num = 0;
		RankingUtil.Ranker ranker = null;
		if (list != null && list.Count > 1)
		{
			ranker = list[0];
			if (ranker != null && list[1] != null)
			{
				int num2 = ranker.rankIndex + 1;
				if (scoreType == RankingUtil.RankingScoreType.TOTAL_SCORE)
				{
					currentScore += ranker.score;
				}
				if (list.Count == 2 && ranker.id == list[1].id)
				{
					num = num2;
					nextRankScore = 0L;
					prveRankScore = 0L;
				}
				else
				{
					num = num2;
					if (isHighScore || scoreType == RankingUtil.RankingScoreType.TOTAL_SCORE)
					{
						long num3 = 0L;
						long num4 = 0L;
						bool flag = false;
						for (int i = 1; i < list.Count; i++)
						{
							RankingUtil.Ranker ranker2 = list[i];
							if (ranker2 == null)
							{
								continue;
							}
							if (ranker2.id == list[0].id)
							{
								num = num2;
								if (num <= 1)
								{
									nextRankScore = 0L;
								}
								else
								{
									nextRankScore = list[i - 1].score - currentScore + 1;
									if (nextRankScore < 0)
									{
										nextRankScore = -1L;
									}
								}
								if (list.Count > i + 1)
								{
									prveRankScore = currentScore - list[i + 1].score;
									if (prveRankScore < 0)
									{
										prveRankScore = 0L;
									}
								}
								else
								{
									prveRankScore = 0L;
								}
								break;
							}
							num3 = ranker2.score;
							if (num3 < currentScore)
							{
								num = ranker2.rankIndex + 1;
								if (num <= 1)
								{
									nextRankScore = 0L;
								}
								else
								{
									nextRankScore = list[i - 1].score - currentScore + 1;
									if (nextRankScore < 0)
									{
										nextRankScore = -1L;
									}
								}
								if (num >= list.Count - 2)
								{
									prveRankScore = currentScore - list[i].score;
									if (prveRankScore < 0)
									{
										prveRankScore = 0L;
									}
								}
								else
								{
									prveRankScore = currentScore - list[i + 1].score;
									if (num4 > 0 && num4 == currentScore)
									{
										prveRankScore = currentScore - list[i].score;
										if (prveRankScore < 0)
										{
											prveRankScore = 0L;
										}
									}
								}
								flag = true;
								break;
							}
							num = ((num2 >= ranker2.rankIndex + 1) ? (ranker2.rankIndex + 1) : (ranker2.rankIndex + 2));
							if (num <= 1)
							{
								nextRankScore = 0L;
							}
							else
							{
								nextRankScore = list[i].score - currentScore + 1;
								if (nextRankScore < 0)
								{
									nextRankScore = -1L;
								}
							}
							if (num >= list.Count - 2)
							{
								prveRankScore = 0L;
							}
							else
							{
								prveRankScore = currentScore - list[i + 1].score;
							}
							num4 = num3;
						}
						if (flag)
						{
							Debug.Log("RankingManager GetCurrentHighScoreRank : rank Out of range");
						}
					}
				}
			}
			else if (list.Count == 1)
			{
				num = 1;
				isHighScore = true;
				nextRankScore = 0L;
				prveRankScore = 0L;
			}
			else if (list.Count > 1 && ranker == null)
			{
				isHighScore = true;
				num = 1;
				long num5 = 0L;
				long num6 = 0L;
				for (int j = 1; j < list.Count; j++)
				{
					RankingUtil.Ranker ranker3 = list[j];
					if (ranker3 == null)
					{
						continue;
					}
					num5 = ranker3.score;
					if (num5 < currentScore)
					{
						num = ranker3.rankIndex + 1;
						if (num <= 1)
						{
							nextRankScore = 0L;
						}
						else
						{
							nextRankScore = list[j - 1].score - currentScore + 1;
							if (nextRankScore < 0)
							{
								nextRankScore = -1L;
							}
						}
						if (num >= list.Count - 2)
						{
							prveRankScore = currentScore - list[j].score;
							if (prveRankScore < 0)
							{
								prveRankScore = 0L;
							}
							break;
						}
						prveRankScore = currentScore - list[j + 1].score;
						if (num6 > 0 && num6 == currentScore)
						{
							prveRankScore = currentScore - list[j].score;
							if (prveRankScore < 0)
							{
								prveRankScore = 0L;
							}
						}
						break;
					}
					num = ranker3.rankIndex + 2;
					if (list.Count > j)
					{
						nextRankScore = list[j].score - currentScore + 1;
						if (nextRankScore < 0)
						{
							nextRankScore = -1L;
						}
					}
					if (num >= list.Count - 2)
					{
						prveRankScore = 0L;
					}
					else
					{
						prveRankScore = currentScore - list[j + 1].score;
					}
					num6 = num5;
				}
			}
			else
			{
				num = 1;
				isHighScore = true;
				nextRankScore = 0L;
				prveRankScore = 0L;
			}
		}
		else
		{
			num = 1;
			isHighScore = true;
			nextRankScore = 0L;
			prveRankScore = 0L;
		}
		if (scoreType == RankingUtil.RankingScoreType.TOTAL_SCORE && ranker != null && num > 1 && ranker.rankIndex + 1 < num)
		{
			num = ranker.rankIndex + 1;
		}
		return num;
	}
}
