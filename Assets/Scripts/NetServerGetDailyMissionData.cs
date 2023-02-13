using LitJson;
using System;
using System.Collections.Generic;

public class NetServerGetDailyMissionData : NetBase
{
	public int resultNumContinue
	{
		get;
		private set;
	}

	public int resultIncentives
	{
		get
		{
			if (resultDailyMissionIncentiveList != null)
			{
				return resultDailyMissionIncentiveList.Count;
			}
			return 0;
		}
	}

	public int resultNumDailyChalDay
	{
		get;
		private set;
	}

	public int resultMaxDailyChalDay
	{
		get;
		private set;
	}

	public int resultMaxIncentive
	{
		get;
		private set;
	}

	public DateTime resultChalEndTime
	{
		get;
		private set;
	}

	protected List<ServerDailyChallengeIncentive> resultDailyMissionIncentiveList
	{
		get;
		set;
	}

	protected override void DoRequest()
	{
		SetAction("Game/getDailyChalData");
		CPlusPlusLink instance = CPlusPlusLink.Instance;
		if (instance != null)
		{
			string onlySendBaseParamString = instance.GetOnlySendBaseParamString();
			WriteJsonString(onlySendBaseParamString);
		}
	}

	protected override void DoDidSuccess(JsonData jdata)
	{
		GetResponse_IncentiveList(jdata);
		GetResponse_NumContinue(jdata);
		GetResponse_NumDailyChalDay(jdata);
		GetResponse_MaxDailyChalDay(jdata);
		GetResponse_MaxIncentive(jdata);
		GetResponse_ChalEndTime(jdata);
	}

	protected override void DoDidSuccessEmulation()
	{
	}

	public ServerDailyChallengeIncentive GetResultDailyMissionIncentive(int index)
	{
		if (0 <= index && resultIncentives > index)
		{
			return resultDailyMissionIncentiveList[index];
		}
		return null;
	}

	private void GetResponse_IncentiveList(JsonData jdata)
	{
		resultDailyMissionIncentiveList = new List<ServerDailyChallengeIncentive>();
		JsonData jsonArray = NetUtil.GetJsonArray(jdata, "incentiveList");
		int count = jsonArray.Count;
		for (int i = 0; i < count; i++)
		{
			JsonData jdata2 = jsonArray[i];
			ServerDailyChallengeIncentive item = NetUtil.AnalyzeDailyMissionIncentiveJson(jdata2, string.Empty);
			resultDailyMissionIncentiveList.Add(item);
		}
	}

	private void GetResponse_NumContinue(JsonData jdata)
	{
		resultNumContinue = NetUtil.GetJsonInt(jdata, "numDailyChalCont");
	}

	private void GetResponse_NumDailyChalDay(JsonData jdata)
	{
		resultNumDailyChalDay = NetUtil.GetJsonInt(jdata, "numDailyChalDay");
	}

	private void GetResponse_MaxDailyChalDay(JsonData jdata)
	{
		resultMaxDailyChalDay = NetUtil.GetJsonInt(jdata, "maxDailyChalDay");
	}

	private void GetResponse_MaxIncentive(JsonData jdata)
	{
		resultMaxIncentive = NetUtil.GetJsonInt(jdata, "incentiveListCont");
	}

	private void GetResponse_ChalEndTime(JsonData jdata)
	{
		resultChalEndTime = NetUtil.GetLocalDateTime(NetUtil.GetJsonLong(jdata, "chalEndTime"));
		Debug.Log("resultChalEndTime:" + resultChalEndTime.ToString());
	}
}
