using LitJson;
using System.Collections.Generic;

public class NetServerGetDailyBattleDataHistory : NetBase
{
	public int historyCount
	{
		private get;
		set;
	}

	public List<ServerDailyBattleDataPair> battleDataPairList
	{
		get;
		private set;
	}

	public NetServerGetDailyBattleDataHistory(int count)
	{
		historyCount = count;
	}

	protected override void DoRequest()
	{
		SetAction("Battle/getDailyBattleDataHistory");
		CPlusPlusLink instance = CPlusPlusLink.Instance;
		if (instance != null)
		{
			string dailyBattleDataHistoryString = instance.GetDailyBattleDataHistoryString(historyCount);
			WriteJsonString(dailyBattleDataHistoryString);
		}
	}

	protected override void DoDidSuccess(JsonData jdata)
	{
		GetResponse_BattleData(jdata);
	}

	protected override void DoDidSuccessEmulation()
	{
	}

	private void SetParameter_HistoryCount()
	{
		WriteActionParamValue("count", historyCount);
	}

	private void GetResponse_BattleData(JsonData jdata)
	{
		battleDataPairList = NetUtil.AnalyzeDailyBattleDataPairListJson(jdata);
	}
}
