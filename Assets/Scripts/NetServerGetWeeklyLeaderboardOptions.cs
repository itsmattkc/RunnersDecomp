using LitJson;

public class NetServerGetWeeklyLeaderboardOptions : NetBase
{
	public int paramMode
	{
		get;
		set;
	}

	public ServerWeeklyLeaderboardOptions weeklyLeaderboardOptions
	{
		get;
		set;
	}

	public NetServerGetWeeklyLeaderboardOptions(int mode)
	{
		paramMode = mode;
		SetSecureFlag(false);
	}

	protected override void DoRequest()
	{
		SetAction("Leaderboard/getWeeklyLeaderboardOptions");
		CPlusPlusLink instance = CPlusPlusLink.Instance;
		if (instance != null)
		{
			string getWeeklyLeaderboardOptionsString = instance.GetGetWeeklyLeaderboardOptionsString(paramMode);
			WriteJsonString(getWeeklyLeaderboardOptionsString);
		}
	}

	protected void SetParameter_NetServerGetWeeklyLeaderboardOptions()
	{
		SetParameter_Option();
	}

	protected override void DoDidSuccess(JsonData jdata)
	{
		GetResponse_WeeklyLeaderboardOptions(jdata);
	}

	protected override void DoDidSuccessEmulation()
	{
	}

	private void SetParameter_Option()
	{
		WriteActionParamValue("mode", paramMode);
	}

	private void GetResponse_WeeklyLeaderboardOptions(JsonData jdata)
	{
		weeklyLeaderboardOptions = NetUtil.AnalyzeWeeklyLeaderboardOptions(jdata);
	}
}
