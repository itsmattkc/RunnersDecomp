using LitJson;
using System.Collections.Generic;

public class NetServerGetMenuData : NetBase
{
	private ServerPlayerState m_playerState = new ServerPlayerState();

	private ServerWheelOptions m_wheelOptions = new ServerWheelOptions();

	private ServerChaoWheelOptions m_chaoWheelOptions = new ServerChaoWheelOptions();

	private int m_subCharaRingExchange;

	private ServerDailyChallengeState m_dailyChallengeIncentive = new ServerDailyChallengeState();

	private ServerMileageMapState m_resultMileageMapState = new ServerMileageMapState();

	private List<ServerMileageFriendEntry> m_resultMileageFriendList = new List<ServerMileageFriendEntry>();

	private List<ServerMessageEntry> m_messageEntryList = new List<ServerMessageEntry>();

	private List<ServerOperatorMessageEntry> m_operatorMessageEntryList = new List<ServerOperatorMessageEntry>();

	private int m_totalMessage;

	private int m_totalOperatorMessage;

	private List<ServerRedStarItemState> m_redstarItemStateList = new List<ServerRedStarItemState>();

	private int m_redstarTotalItems;

	private List<ServerRedStarItemState> m_ringItemStateList = new List<ServerRedStarItemState>();

	private int m_ringTotalItems;

	private List<ServerRedStarItemState> m_energyItemStateList = new List<ServerRedStarItemState>();

	private int m_energyTotalItems;

	private int m_monthPurchase;

	private string m_birthDay = string.Empty;

	private List<ServerConsumedCostData> m_consumedCostList = new List<ServerConsumedCostData>();

	public ServerPlayerState PlayerState
	{
		get
		{
			return m_playerState;
		}
	}

	public ServerWheelOptions WheelOptions
	{
		get
		{
			return m_wheelOptions;
		}
	}

	public ServerChaoWheelOptions ChaoWheelOptions
	{
		get
		{
			return m_chaoWheelOptions;
		}
	}

	public int SubCharaRingExchange
	{
		get
		{
			return m_subCharaRingExchange;
		}
	}

	public ServerDailyChallengeState DailyChallengeState
	{
		get
		{
			return m_dailyChallengeIncentive;
		}
	}

	public ServerMileageMapState MileageMapState
	{
		get
		{
			return m_resultMileageMapState;
		}
	}

	public List<ServerMileageFriendEntry> MileageFriendEntryList
	{
		get
		{
			return m_resultMileageFriendList;
		}
	}

	public List<ServerMessageEntry> MessageEntryList
	{
		get
		{
			return m_messageEntryList;
		}
	}

	public List<ServerOperatorMessageEntry> OperatorMessageEntryList
	{
		get
		{
			return m_operatorMessageEntryList;
		}
	}

	public int TotalMessage
	{
		get
		{
			return m_totalMessage;
		}
	}

	public int TotalOperatorMessage
	{
		get
		{
			return m_totalOperatorMessage;
		}
	}

	public List<ServerRedStarItemState> RedstarItemStateList
	{
		get
		{
			return m_redstarItemStateList;
		}
	}

	public List<ServerRedStarItemState> RingItemStateList
	{
		get
		{
			return m_ringItemStateList;
		}
	}

	public List<ServerRedStarItemState> EnergyItemStateList
	{
		get
		{
			return m_energyItemStateList;
		}
	}

	public int RedstarTotalItems
	{
		get
		{
			return m_redstarTotalItems;
		}
	}

	public int RingTotalItems
	{
		get
		{
			return m_ringTotalItems;
		}
	}

	public int EnergyTotalItems
	{
		get
		{
			return m_energyTotalItems;
		}
	}

	public int MonthPurchase
	{
		get
		{
			return m_monthPurchase;
		}
	}

	public string BirthDay
	{
		get
		{
			return m_birthDay;
		}
	}

	public List<ServerConsumedCostData> ConsumedCostList
	{
		get
		{
			return m_consumedCostList;
		}
	}

	protected override void DoRequest()
	{
		SetAction("Game/getMenuData");
	}

	protected override void DoDidSuccess(JsonData jdata)
	{
		GetResponse_PlayerState(jdata);
		GetResponse_WheelOptions(jdata);
		GetResponse_ChaoWheelOptions(jdata);
		GetResponse_SubCharaRingExchange(jdata);
		GetResponse_DailyMission(jdata);
		GetResponse_MileageState(jdata);
		GetResponse_MessageList(jdata);
		GetResponse_RSRingItemRsRringStateList(jdata);
		GetResponse_RSRingItemRingStateList(jdata);
		GetResponse_RSRingItemEnergyStateList(jdata);
		GetResponse_MonthPurchase(jdata);
		GetResponse_Birthday(jdata);
		GetResponse_ConsumedCostList(jdata);
		NetUtil.GetResponse_CampaignList(jdata);
	}

	protected override void DoDidSuccessEmulation()
	{
	}

	private void GetResponse_PlayerState(JsonData jdata)
	{
		m_playerState = NetUtil.AnalyzePlayerStateJson(jdata, "playerState");
	}

	private void GetResponse_WheelOptions(JsonData jdata)
	{
		m_wheelOptions = NetUtil.AnalyzeWheelOptionsJson(jdata, "wheelOptions");
		if (m_wheelOptions != null && m_wheelOptions.m_numJackpotRing > 0)
		{
			RouletteManager.numJackpotRing = m_wheelOptions.m_numJackpotRing;
			Debug.Log("numJackpotRing : " + RouletteManager.numJackpotRing);
		}
	}

	private void GetResponse_ChaoWheelOptions(JsonData jdata)
	{
		m_chaoWheelOptions = NetUtil.AnalyzeChaoWheelOptionsJson(jdata, "chaoWheelOptions");
	}

	private void GetResponse_SubCharaRingExchange(JsonData jdata)
	{
		m_subCharaRingExchange = NetUtil.GetJsonInt(jdata, "subCharaRingExchange");
	}

	private void GetResponse_DailyMission(JsonData jdata)
	{
		JsonData jsonArray = NetUtil.GetJsonArray(jdata, "dailyChallengeIncentive");
		int count = jsonArray.Count;
		for (int i = 0; i < count; i++)
		{
			JsonData jdata2 = jsonArray[i];
			ServerDailyChallengeIncentive item = NetUtil.AnalyzeDailyMissionIncentiveJson(jdata2, string.Empty);
			m_dailyChallengeIncentive.m_incentiveList.Add(item);
		}
		m_dailyChallengeIncentive.m_numIncentiveCont = NetUtil.GetJsonInt(jdata, "numDailyChalCont");
		m_dailyChallengeIncentive.m_numDailyChalDay = NetUtil.GetJsonInt(jdata, "numDailyChalDay");
	}

	private void GetResponse_MileageState(JsonData jdata)
	{
		m_resultMileageMapState = NetUtil.AnalyzeMileageMapStateJson(jdata, "mileageMapState");
	}

	private void GetResponse_MileageFriendList(JsonData jdata)
	{
		m_resultMileageFriendList = NetUtil.AnalyzeMileageFriendListJson(jdata, "mileageFriendList");
	}

	private void GetResponse_MessageList(JsonData jdata)
	{
		JsonData jsonArray = NetUtil.GetJsonArray(jdata, "messageList");
		int count = jsonArray.Count;
		for (int i = 0; i < count; i++)
		{
			JsonData jdata2 = jsonArray[i];
			ServerMessageEntry item = NetUtil.AnalyzeMessageEntryJson(jdata2, string.Empty);
			m_messageEntryList.Add(item);
		}
		JsonData jsonArray2 = NetUtil.GetJsonArray(jdata, "operatorMessageList");
		int count2 = jsonArray2.Count;
		for (int j = 0; j < count2; j++)
		{
			JsonData jdata3 = jsonArray2[j];
			ServerOperatorMessageEntry item2 = NetUtil.AnalyzeOperatorMessageEntryJson(jdata3, string.Empty);
			m_operatorMessageEntryList.Add(item2);
		}
		m_totalMessage = NetUtil.GetJsonInt(jdata, "totalMessage");
		m_totalOperatorMessage = NetUtil.GetJsonInt(jdata, "totalOperatorMessage");
	}

	private void GetResponse_RSRingItemRsRringStateList(JsonData jdata)
	{
		JsonData jsonArray = NetUtil.GetJsonArray(jdata, "redstarItemList");
		int count = jsonArray.Count;
		for (int i = 0; i < count; i++)
		{
			JsonData jdata2 = jsonArray[i];
			ServerRedStarItemState item = NetUtil.AnalyzeRedStarItemStateJson(jdata2, string.Empty);
			m_redstarItemStateList.Add(item);
		}
		m_redstarTotalItems = NetUtil.GetJsonInt(jdata, "redstarTotalItems");
	}

	private void GetResponse_RSRingItemRingStateList(JsonData jdata)
	{
		JsonData jsonArray = NetUtil.GetJsonArray(jdata, "ringItemList");
		int count = jsonArray.Count;
		for (int i = 0; i < count; i++)
		{
			JsonData jdata2 = jsonArray[i];
			ServerRedStarItemState item = NetUtil.AnalyzeRedStarItemStateJson(jdata2, string.Empty);
			m_ringItemStateList.Add(item);
		}
		m_ringTotalItems = NetUtil.GetJsonInt(jdata, "ringTotalItems");
	}

	private void GetResponse_RSRingItemEnergyStateList(JsonData jdata)
	{
		JsonData jsonArray = NetUtil.GetJsonArray(jdata, "energyItemList");
		int count = jsonArray.Count;
		for (int i = 0; i < count; i++)
		{
			JsonData jdata2 = jsonArray[i];
			ServerRedStarItemState item = NetUtil.AnalyzeRedStarItemStateJson(jdata2, string.Empty);
			m_energyItemStateList.Add(item);
		}
		m_energyTotalItems = NetUtil.GetJsonInt(jdata, "energyTotalItems");
	}

	private void GetResponse_MonthPurchase(JsonData jdata)
	{
		m_monthPurchase = NetUtil.GetJsonInt(jdata, "monthPurchase");
	}

	private void GetResponse_Birthday(JsonData jdata)
	{
		m_birthDay = NetUtil.GetJsonString(jdata, "birthday");
	}

	private void GetResponse_ConsumedCostList(JsonData jdata)
	{
		m_consumedCostList = NetUtil.AnalyzeConsumedCostDataList(jdata);
	}
}
