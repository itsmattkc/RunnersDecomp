using LitJson;
using System.Collections.Generic;

public class NetServerGetPrizeDailyBattle : NetBase
{
	public List<ServerDailyBattlePrizeData> battleDataPrizeList
	{
		get;
		private set;
	}

	protected override void DoRequest()
	{
		SetAction("Battle/getPrizeDailyBattle");
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
		battleDataPrizeList = NetUtil.AnalyzeDailyBattlePrizeDataJson(jdata, "battlePrizeDataList");
	}
}
