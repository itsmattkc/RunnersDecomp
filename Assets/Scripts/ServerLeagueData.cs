using System.Collections.Generic;

public class ServerLeagueData
{
	public int mode
	{
		get;
		set;
	}

	public RankingUtil.RankingMode rankinMode
	{
		get
		{
			RankingUtil.RankingMode result = RankingUtil.RankingMode.ENDLESS;
			if (mode >= 0 && mode < 2)
			{
				result = (RankingUtil.RankingMode)mode;
			}
			return result;
		}
	}

	public int leagueId
	{
		get;
		set;
	}

	public int groupId
	{
		get;
		set;
	}

	public int numGroupMember
	{
		get;
		set;
	}

	public int numLeagueMember
	{
		get;
		set;
	}

	public int numUp
	{
		get;
		set;
	}

	public int numDown
	{
		get;
		set;
	}

	public List<ServerRemainOperator> highScoreOpe
	{
		get;
		set;
	}

	public List<ServerRemainOperator> totalScoreOpe
	{
		get;
		set;
	}

	public ServerLeagueData()
	{
		mode = 0;
		leagueId = 1;
		groupId = 0;
		numGroupMember = 0;
		numLeagueMember = 0;
		numUp = 10;
		numDown = 10;
		highScoreOpe = new List<ServerRemainOperator>();
		totalScoreOpe = new List<ServerRemainOperator>();
	}

	public void Dump()
	{
	}

	public void CopyTo(ServerLeagueData to)
	{
		to.mode = mode;
		to.leagueId = leagueId;
		to.groupId = groupId;
		to.numGroupMember = numGroupMember;
		to.numLeagueMember = numLeagueMember;
		to.numUp = numUp;
		to.numDown = numDown;
		SetServerRemainOperator(highScoreOpe, to.highScoreOpe);
		SetServerRemainOperator(totalScoreOpe, to.totalScoreOpe);
	}

	public void AddHighScoreRemainOperator(ServerRemainOperator remainOperator)
	{
		highScoreOpe.Add(remainOperator);
	}

	public void AddTotalScoreRemainOperator(ServerRemainOperator remainOperator)
	{
		totalScoreOpe.Add(remainOperator);
	}

	private void SetServerRemainOperator(List<ServerRemainOperator> setData, List<ServerRemainOperator> getData)
	{
		if (setData != null && getData != null && setData.Count > 0)
		{
			getData.Clear();
			for (int i = 0; i < setData.Count; i++)
			{
				getData.Add(setData[i]);
			}
		}
	}
}
