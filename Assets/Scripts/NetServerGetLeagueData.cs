using LitJson;

public class NetServerGetLeagueData : NetBase
{
	public int paramMode
	{
		get;
		set;
	}

	public ServerLeagueData leagueData
	{
		get;
		set;
	}

	public NetServerGetLeagueData(int mode)
	{
		paramMode = mode;
		SetSecureFlag(false);
	}

	protected override void DoRequest()
	{
		SetAction("Leaderboard/getLeagueData");
		CPlusPlusLink instance = CPlusPlusLink.Instance;
		if (instance != null)
		{
			string getLeagueDataString = instance.GetGetLeagueDataString(paramMode);
			Debug.Log("CPlusPlusLink.actRetry");
			WriteJsonString(getLeagueDataString);
		}
	}

	protected override void DoDidSuccess(JsonData jdata)
	{
		GetResponse_LeagueData(jdata);
	}

	protected override void DoDidSuccessEmulation()
	{
	}

	private void SetParameter_Mode()
	{
		WriteActionParamValue("mode", paramMode);
	}

	private void GetResponse_LeagueData(JsonData jdata)
	{
		leagueData = NetUtil.AnalyzeLeagueData(jdata, "leagueData");
	}
}
