using LitJson;
using System.Collections.Generic;

public class NetServerGetLeaderboardEntries : NetBase
{
	public int paramMode
	{
		get;
		set;
	}

	public int paramFirst
	{
		get;
		set;
	}

	public int paramCount
	{
		get;
		set;
	}

	public int paramIndex
	{
		get;
		set;
	}

	public int paramEventId
	{
		get;
		set;
	}

	public int paramRankingType
	{
		get;
		set;
	}

	public string[] paramFriendIdList
	{
		get;
		set;
	}

	private int resultTotalEntries
	{
		get;
		set;
	}

	private ServerLeaderboardEntry resultMyLeaderboardEntry
	{
		get;
		set;
	}

	private int resultEntries
	{
		get
		{
			if (resultLeaderboardEntriesList != null)
			{
				return resultLeaderboardEntriesList.Count;
			}
			return 0;
		}
	}

	private int resetTime
	{
		get;
		set;
	}

	private int startTime
	{
		get;
		set;
	}

	private int startIndex
	{
		get;
		set;
	}

	public ServerLeaderboardEntries leaderboardEntries
	{
		get;
		protected set;
	}

	protected List<ServerLeaderboardEntry> resultLeaderboardEntriesList
	{
		get;
		set;
	}

	public NetServerGetLeaderboardEntries()
		: this(0, -1, -1, -1, -1, -1, null)
	{
		SetSecureFlag(false);
	}

	public NetServerGetLeaderboardEntries(int mode, int first, int count, int index, int rankingType, int eventId, string[] friendIdList)
	{
		paramMode = mode;
		paramFirst = first;
		paramCount = count;
		paramIndex = index;
		paramEventId = eventId;
		paramRankingType = rankingType;
		paramFriendIdList = friendIdList;
		SetSecureFlag(false);
	}

	protected override void DoRequest()
	{
		SetAction("Leaderboard/getWeeklyLeaderboardEntries");
		CPlusPlusLink instance = CPlusPlusLink.Instance;
		if (!(instance != null))
		{
			return;
		}
		List<string> list = new List<string>();
		if (this.paramFriendIdList != null)
		{
			string[] paramFriendIdList = this.paramFriendIdList;
			foreach (string item in paramFriendIdList)
			{
				list.Add(item);
			}
		}
		string getWeeklyLeaderboardEntries = instance.GetGetWeeklyLeaderboardEntries(paramMode, paramFirst, paramCount, paramRankingType, list, paramEventId);
		Debug.Log("CPlusPlusLink.actRetry");
		WriteJsonString(getWeeklyLeaderboardEntries);
	}

	protected void SetParameter_LeaderboardEntries()
	{
		SetParameter_Mode();
		SetParameter_First();
		SetParameter_Count();
		SetParameter_RankingType();
		SetParameter_FriendIdList();
		SetParameter_EventId();
	}

	protected override void DoDidSuccess(JsonData jdata)
	{
		GetResponse_LeaderboardEntries(jdata);
	}

	protected void GetResponse_LeaderboardEntries(JsonData jdata)
	{
		GetResponse_MyEntry(jdata);
		GetResponse_EntriesList(jdata);
		GetResponse_TotalEntries(jdata);
		GetResponse_ResetTime(jdata);
		GetResponse_StartTime(jdata);
		GetResponse_StartIndex(jdata);
		leaderboardEntries = new ServerLeaderboardEntries
		{
			m_myLeaderboardEntry = resultMyLeaderboardEntry,
			m_resultTotalEntries = resultTotalEntries,
			m_leaderboardEntries = resultLeaderboardEntriesList,
			m_resetTime = resetTime,
			m_startTime = startTime,
			m_startIndex = startIndex,
			m_mode = paramMode,
			m_first = paramFirst,
			m_count = paramCount,
			m_index = paramIndex,
			m_rankingType = paramRankingType,
			m_eventId = paramEventId,
			m_friendIdList = paramFriendIdList
		};
		leaderboardEntries.CopyTo(ServerInterface.LeaderboardEntries);
		if (IsRivalHighScore())
		{
			leaderboardEntries.CopyTo(ServerInterface.LeaderboardEntriesRivalHighScore);
		}
		if (ServerLeaderboardEntries.IsRivalHighScore(0, leaderboardEntries.m_rankingType))
		{
			ServerLeaderboardEntry rankTop = leaderboardEntries.GetRankTop();
			if (rankTop != null)
			{
				rankTop.CopyTo(ServerInterface.LeaderboardEntryRivalHighScoreTop);
			}
		}
	}

	protected override void DoDidSuccessEmulation()
	{
		resultTotalEntries = paramCount;
		resultLeaderboardEntriesList = new List<ServerLeaderboardEntry>(paramCount);
		resultMyLeaderboardEntry = new ServerLeaderboardEntry();
		resultMyLeaderboardEntry.m_grade = 0;
		int paramFirst = this.paramFirst;
		for (int i = paramFirst; i < 125 && i < paramFirst + paramCount; i++)
		{
			ServerLeaderboardEntry serverLeaderboardEntry = new ServerLeaderboardEntry();
			serverLeaderboardEntry.m_hspId = "Xeen_" + string.Format("{0:D4}", i);
			serverLeaderboardEntry.m_grade = i + 1;
			serverLeaderboardEntry.m_score = 10000 - i * 100;
			serverLeaderboardEntry.m_name = "Xeen_" + string.Format("{0:D4}", i);
			serverLeaderboardEntry.m_url = string.Empty;
			resultLeaderboardEntriesList.Add(serverLeaderboardEntry);
		}
	}

	public bool IsRivalHighScore()
	{
		return ServerLeaderboardEntries.IsRivalHighScore(paramFirst, paramRankingType);
	}

	private void SetParameter_Mode()
	{
		WriteActionParamValue("mode", paramMode);
	}

	private void SetParameter_First()
	{
		if (-1 < paramFirst && -1 < paramCount)
		{
			WriteActionParamValue("first", paramFirst);
		}
	}

	private void SetParameter_Count()
	{
		if (-1 < paramFirst && -1 < paramCount)
		{
			WriteActionParamValue("count", paramCount);
		}
	}

	private void SetParameter_RankingType()
	{
		WriteActionParamValue("type", paramRankingType);
	}

	private void SetParameter_FriendIdList()
	{
		if (paramFriendIdList != null && paramFriendIdList.Length != 0)
		{
			WriteActionParamArray("friendIdList", new List<object>(paramFriendIdList));
		}
	}

	private void SetParameter_EventId()
	{
		WriteActionParamValue("eventId", paramEventId);
	}

	public ServerLeaderboardEntry GetResultLeaderboardEntry(int index)
	{
		if (0 <= index && resultEntries > index)
		{
			return resultLeaderboardEntriesList[index];
		}
		return null;
	}

	private void GetResponse_MyEntry(JsonData jdata)
	{
		resultMyLeaderboardEntry = NetUtil.AnalyzeLeaderboardEntryJson(jdata, "playerEntry");
	}

	private void GetResponse_EntriesList(JsonData jdata)
	{
		resultLeaderboardEntriesList = new List<ServerLeaderboardEntry>();
		JsonData jsonArray = NetUtil.GetJsonArray(jdata, "entriesList");
		int count = jsonArray.Count;
		for (int i = 0; i < count; i++)
		{
			JsonData jdata2 = jsonArray[i];
			ServerLeaderboardEntry item = NetUtil.AnalyzeLeaderboardEntryJson(jdata2, string.Empty);
			resultLeaderboardEntriesList.Add(item);
		}
	}

	private void GetResponse_TotalEntries(JsonData jdata)
	{
		resultTotalEntries = NetUtil.GetJsonInt(jdata, "totalEntries");
	}

	private void GetResponse_ResetTime(JsonData jdata)
	{
		resetTime = NetUtil.GetJsonInt(jdata, "resetTime");
	}

	private void GetResponse_StartTime(JsonData jdata)
	{
		startTime = NetUtil.GetJsonInt(jdata, "startTime");
	}

	private void GetResponse_StartIndex(JsonData jdata)
	{
		startIndex = NetUtil.GetJsonInt(jdata, "startIndex");
	}
}
