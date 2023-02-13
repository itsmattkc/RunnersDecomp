using LitJson;
using System;

public class NetServerUpdateDailyBattleStatus : NetBase
{
	public ServerDailyBattleStatus battleDataStatus
	{
		get;
		private set;
	}

	public DateTime endTime
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
		SetAction("Battle/updateDailyBattleStatus");
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
		battleDataStatus = NetUtil.AnalyzeDailyBattleStatusJson(jdata, "battleStatus");
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
