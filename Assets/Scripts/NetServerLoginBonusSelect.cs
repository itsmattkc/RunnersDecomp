using LitJson;

public class NetServerLoginBonusSelect : NetBase
{
	public int paramRewardId;

	public int paramRewardDays;

	public int paramRewardSelect;

	public int paramFirstRewardDays;

	public int paramFirstRewardSelect;

	public ServerLoginBonusReward loginBonusReward
	{
		get;
		private set;
	}

	public ServerLoginBonusReward firstLoginBonusReward
	{
		get;
		private set;
	}

	public NetServerLoginBonusSelect()
		: this(0, 0, 0, 0, 0)
	{
	}

	public NetServerLoginBonusSelect(int rewardId, int rewardDays, int rewardSelect, int firstRewardDays, int firstRewardSelect)
	{
		paramRewardId = rewardId;
		paramRewardDays = rewardDays;
		paramRewardSelect = rewardSelect;
		paramFirstRewardDays = firstRewardDays;
		paramFirstRewardSelect = firstRewardSelect;
	}

	protected override void DoRequest()
	{
		SetAction("Login/loginBonusSelect");
		CPlusPlusLink instance = CPlusPlusLink.Instance;
		if (instance != null)
		{
			string jsonString = instance.LoginBonusSelectString(paramRewardId, paramRewardDays, paramRewardSelect, paramFirstRewardDays, paramFirstRewardSelect);
			WriteJsonString(jsonString);
		}
	}

	protected override void DoDidSuccess(JsonData jdata)
	{
		GetResponse_LoginBonusRewardList(jdata);
		GetResponse_FirstLoginBonusRewardList(jdata);
	}

	protected override void DoDidSuccessEmulation()
	{
	}

	private void GetResponse_LoginBonusRewardList(JsonData jdata)
	{
		JsonData jsonArray = NetUtil.GetJsonArray(jdata, "rewardList");
		if (jsonArray == null)
		{
			loginBonusReward = null;
			return;
		}
		loginBonusReward = new ServerLoginBonusReward();
		int count = jsonArray.Count;
		for (int i = 0; i < count; i++)
		{
			loginBonusReward.m_itemList.Add(NetUtil.AnalyzeItemStateJson(jsonArray[i], string.Empty));
		}
	}

	private void GetResponse_FirstLoginBonusRewardList(JsonData jdata)
	{
		JsonData jsonArray = NetUtil.GetJsonArray(jdata, "firstRewardList");
		firstLoginBonusReward = new ServerLoginBonusReward();
		if (jsonArray == null)
		{
			firstLoginBonusReward = null;
			return;
		}
		int count = jsonArray.Count;
		for (int i = 0; i < count; i++)
		{
			firstLoginBonusReward.m_itemList.Add(NetUtil.AnalyzeItemStateJson(jsonArray[i], string.Empty));
		}
	}
}
