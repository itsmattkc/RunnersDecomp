using LitJson;
using System;

public class NetServerResetDailyBattleMatching : NetBase
{
	public int matchingType
	{
		private get;
		set;
	}

	public ServerPlayerState playerState
	{
		get;
		private set;
	}

	public ServerDailyBattleDataPair battleDataPair
	{
		get;
		private set;
	}

	public DateTime endTime
	{
		get;
		private set;
	}

	public NetServerResetDailyBattleMatching(int type)
	{
		matchingType = type;
	}

	protected override void DoRequest()
	{
		SetAction("Battle/resetDailyBattleMatching");
		CPlusPlusLink instance = CPlusPlusLink.Instance;
		if (instance != null)
		{
			string jsonString = instance.ResetDailyBattleMatchingString(matchingType);
			WriteJsonString(jsonString);
		}
	}

	protected override void DoDidSuccess(JsonData jdata)
	{
		GetResponse_PlayerState(jdata);
		GetResponse_BattleData(jdata);
	}

	protected override void DoDidSuccessEmulation()
	{
	}

	private void SetParameter_MatchingType()
	{
		WriteActionParamValue("type", matchingType);
	}

	private void GetResponse_PlayerState(JsonData jdata)
	{
		playerState = NetUtil.AnalyzePlayerStateJson(jdata, "playerState");
	}

	private void GetResponse_BattleData(JsonData jdata)
	{
		endTime = NetUtil.AnalyzeDateTimeJson(jdata, "endTime");
		battleDataPair = NetUtil.AnalyzeDailyBattleDataPairJson(jdata);
	}
}
