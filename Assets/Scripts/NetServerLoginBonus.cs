using LitJson;
using System;
using System.Collections.Generic;

public class NetServerLoginBonus : NetBase
{
	public ServerLoginBonusState loginBonusState
	{
		get;
		private set;
	}

	public DateTime startTime
	{
		get;
		private set;
	}

	public DateTime endTime
	{
		get;
		private set;
	}

	public List<ServerLoginBonusReward> loginBonusRewardList
	{
		get;
		private set;
	}

	public List<ServerLoginBonusReward> firstLoginBonusRewardList
	{
		get;
		private set;
	}

	public int rewardId
	{
		get;
		private set;
	}

	public int rewardDays
	{
		get;
		private set;
	}

	public int firstRewardDays
	{
		get;
		private set;
	}

	public int resultLoginBonusRewardCount
	{
		get
		{
			if (loginBonusRewardList != null)
			{
				return loginBonusRewardList.Count;
			}
			return 0;
		}
	}

	protected override void DoRequest()
	{
		SetAction("Login/loginBonus");
		CPlusPlusLink instance = CPlusPlusLink.Instance;
		if (instance != null)
		{
			string onlySendBaseParamString = instance.GetOnlySendBaseParamString();
			WriteJsonString(onlySendBaseParamString);
		}
	}

	protected override void DoDidSuccess(JsonData jdata)
	{
		GetResponse_LoginBonusStatus(jdata);
		GetResponse_StartTime(jdata);
		GetResponse_EndTime(jdata);
		GetResponse_LoginBonusRewardList(jdata);
		GetResponse_FirstLoginBonusRewardList(jdata);
		GetResponse_RewardId(jdata);
		GetResponse_RewardDays(jdata);
		GetResponse_FirstRewardDays(jdata);
	}

	protected override void DoDidSuccessEmulation()
	{
	}

	private void GetResponse_LoginBonusStatus(JsonData jdata)
	{
		loginBonusState = new ServerLoginBonusState();
		JsonData jsonObject = NetUtil.GetJsonObject(jdata, "loginBonusStatus");
		if (jsonObject != null)
		{
			ServerLoginBonusState serverLoginBonusState = new ServerLoginBonusState();
			serverLoginBonusState.m_numLogin = NetUtil.GetJsonInt(jsonObject, "numLogin");
			serverLoginBonusState.m_numBonus = NetUtil.GetJsonInt(jsonObject, "numBonus");
			serverLoginBonusState.m_lastBonusTime = NetUtil.GetLocalDateTime(NetUtil.GetJsonInt(jsonObject, "lastBonusTime"));
			loginBonusState = serverLoginBonusState;
		}
	}

	private void GetResponse_StartTime(JsonData jdata)
	{
		startTime = NetUtil.GetLocalDateTime(NetUtil.GetJsonInt(jdata, "startTime"));
	}

	private void GetResponse_EndTime(JsonData jdata)
	{
		endTime = NetUtil.GetLocalDateTime(NetUtil.GetJsonInt(jdata, "endTime"));
	}

	private void GetResponse_LoginBonusRewardList(JsonData jdata)
	{
		JsonData jsonArray = NetUtil.GetJsonArray(jdata, "loginBonusRewardList");
		loginBonusRewardList = new List<ServerLoginBonusReward>();
		if (jsonArray == null)
		{
			return;
		}
		int count = jsonArray.Count;
		for (int i = 0; i < count; i++)
		{
			JsonData jdata2 = jsonArray[i];
			ServerLoginBonusReward serverLoginBonusReward = new ServerLoginBonusReward();
			JsonData jsonArray2 = NetUtil.GetJsonArray(jdata2, "selectRewardList");
			int count2 = jsonArray2.Count;
			for (int j = 0; j < count2; j++)
			{
				JsonData jsonArray3 = NetUtil.GetJsonArray(jsonArray2[j], "itemList");
				int count3 = jsonArray3.Count;
				for (int k = 0; k < count3; k++)
				{
					serverLoginBonusReward.m_itemList.Add(NetUtil.AnalyzeItemStateJson(jsonArray3[k], string.Empty));
				}
			}
			loginBonusRewardList.Add(serverLoginBonusReward);
		}
	}

	private void GetResponse_FirstLoginBonusRewardList(JsonData jdata)
	{
		JsonData jsonArray = NetUtil.GetJsonArray(jdata, "firstLoginBonusRewardList");
		firstLoginBonusRewardList = new List<ServerLoginBonusReward>();
		if (jsonArray == null)
		{
			return;
		}
		int count = jsonArray.Count;
		for (int i = 0; i < count; i++)
		{
			JsonData jdata2 = jsonArray[i];
			ServerLoginBonusReward serverLoginBonusReward = new ServerLoginBonusReward();
			JsonData jsonArray2 = NetUtil.GetJsonArray(jdata2, "selectRewardList");
			int count2 = jsonArray2.Count;
			for (int j = 0; j < count2; j++)
			{
				JsonData jsonArray3 = NetUtil.GetJsonArray(jsonArray2[j], "itemList");
				int count3 = jsonArray3.Count;
				for (int k = 0; k < count3; k++)
				{
					serverLoginBonusReward.m_itemList.Add(NetUtil.AnalyzeItemStateJson(jsonArray3[k], string.Empty));
				}
			}
			firstLoginBonusRewardList.Add(serverLoginBonusReward);
		}
	}

	private void GetResponse_RewardId(JsonData jdata)
	{
		rewardId = NetUtil.GetJsonInt(jdata, "rewardId");
	}

	private void GetResponse_RewardDays(JsonData jdata)
	{
		rewardDays = NetUtil.GetJsonInt(jdata, "rewardDays");
	}

	private void GetResponse_FirstRewardDays(JsonData jdata)
	{
		firstRewardDays = NetUtil.GetJsonInt(jdata, "firstRewardDays");
	}
}
