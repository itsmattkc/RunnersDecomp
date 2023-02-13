using LitJson;

public class NetServerPostDailyBattleResult : NetBase
{
	public ServerDailyBattleStatus battleStatus
	{
		get;
		private set;
	}

	public ServerDailyBattleDataPair battleDataPair
	{
		get;
		private set;
	}

	public bool rewardFlag
	{
		get;
		private set;
	}

	public ServerDailyBattleDataPair rewardBattleDataPair
	{
		get;
		private set;
	}

	protected override void DoRequest()
	{
		SetAction("Battle/postDailyBattleResult");
	}

	protected override void DoDidSuccess(JsonData jdata)
	{
		GetResponse_BattleData(jdata);
	}

	protected override void DoDidSuccessEmulation()
	{
	}

	private void GetResponse_BattleData(JsonData jdata)
	{
		battleDataPair = NetUtil.AnalyzeDailyBattleDataPairJson(jdata);
		battleStatus = NetUtil.AnalyzeDailyBattleStatusJson(jdata, "battleStatus");
		if (NetUtil.GetJsonBoolean(jdata, "rewardFlag"))
		{
			rewardFlag = true;
			rewardBattleDataPair = NetUtil.AnalyzeDailyBattleDataPairJson(jdata, "rewardBattleData", "rewardRivalBattleData", "rewardStartTime", "rewardEndTime");
		}
		else
		{
			rewardFlag = false;
			rewardBattleDataPair = null;
		}
	}
}
