using System.Collections.Generic;

public class ServerLeagueOperatorData
{
	public int leagueId
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

	public ServerLeagueOperatorData()
	{
		leagueId = 1;
		numUp = 10;
		numDown = 10;
		highScoreOpe = new List<ServerRemainOperator>();
		totalScoreOpe = new List<ServerRemainOperator>();
	}

	public void Dump()
	{
	}

	public void CopyTo(ServerLeagueOperatorData to)
	{
		to.leagueId = leagueId;
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
