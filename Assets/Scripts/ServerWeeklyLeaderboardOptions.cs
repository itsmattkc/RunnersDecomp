public class ServerWeeklyLeaderboardOptions
{
	public int mode
	{
		get;
		set;
	}

	public int type
	{
		get;
		set;
	}

	public int param
	{
		get;
		set;
	}

	public int startTime
	{
		get;
		set;
	}

	public int endTime
	{
		get;
		set;
	}

	public RankingUtil.RankingScoreType rankingScoreType
	{
		get
		{
			RankingUtil.RankingScoreType result = RankingUtil.RankingScoreType.HIGH_SCORE;
			if (type != 0)
			{
				result = RankingUtil.RankingScoreType.TOTAL_SCORE;
			}
			return result;
		}
	}

	public ServerWeeklyLeaderboardOptions()
	{
		mode = 0;
		type = 0;
		param = 0;
		startTime = 0;
		endTime = 0;
	}

	public void CopyTo(ServerWeeklyLeaderboardOptions to)
	{
		to.mode = mode;
		to.type = type;
		to.param = param;
		to.startTime = startTime;
		to.endTime = endTime;
	}
}
