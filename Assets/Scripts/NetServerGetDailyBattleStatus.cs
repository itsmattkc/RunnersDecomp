using LitJson;
using System;

public class NetServerGetDailyBattleStatus : NetBase
{
	public ServerDailyBattleStatus battleStatus
	{
		get;
		private set;
	}

	public DateTime endTime
	{
		get;
		private set;
	}

	protected override void DoRequest()
	{
		SetAction("Battle/getDailyBattleStatus");
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
		endTime = NetUtil.AnalyzeDateTimeJson(jdata, "endTime");
		battleStatus = NetUtil.AnalyzeDailyBattleStatusJson(jdata, "battleStatus");
	}
}
