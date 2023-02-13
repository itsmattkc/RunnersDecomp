using LitJson;

public class NetServerGetDailyBattleData : NetBase
{
	public ServerDailyBattleDataPair battleDataPair
	{
		get;
		private set;
	}

	protected override void DoRequest()
	{
		SetAction("Battle/getDailyBattleData");
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
	}
}
