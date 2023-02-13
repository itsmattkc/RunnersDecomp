using LitJson;
using System.Collections.Generic;

public class NetServerGetLeagueOperatorData : NetBase
{
	public int paramMode
	{
		get;
		set;
	}

	public List<ServerLeagueOperatorData> leagueOperatorData
	{
		get;
		set;
	}

	public int mode
	{
		get;
		set;
	}

	public NetServerGetLeagueOperatorData(int mode)
	{
		paramMode = mode;
		SetSecureFlag(false);
	}

	protected override void DoRequest()
	{
		SetAction("Leaderboard/getLeagueOperatorData");
		CPlusPlusLink instance = CPlusPlusLink.Instance;
		if (instance != null)
		{
			string getLeagueOperatorDataString = instance.GetGetLeagueOperatorDataString(paramMode, -1);
			Debug.Log("CPlusPlusLink.actRetry");
			WriteJsonString(getLeagueOperatorDataString);
		}
	}

	protected void SetParameter_NetServerGetLeagueOperatorData()
	{
		SetParameter_League();
	}

	protected override void DoDidSuccess(JsonData jdata)
	{
		GetResponse_LeagueOperatorData(jdata);
	}

	protected override void DoDidSuccessEmulation()
	{
	}

	private void SetParameter_League()
	{
		WriteActionParamValue("league", -1);
	}

	private void SetParameter_Mode()
	{
		WriteActionParamValue("mode", paramMode);
	}

	private void GetResponse_LeagueOperatorData(JsonData jdata)
	{
		leagueOperatorData = NetUtil.AnalyzeLeagueDatas(jdata, "leagueOperatorList");
		mode = NetUtil.GetJsonInt(jdata, "leagueId");
	}
}
