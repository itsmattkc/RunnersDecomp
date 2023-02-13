using LitJson;
using System.Collections.Generic;

public class NetServerGetRedStarExchangeList : NetBase
{
	private int mParamItemType;

	public string resultBirthDay;

	public int resultMonthPurchase;

	public int paramItemType
	{
		get
		{
			return mParamItemType;
		}
		set
		{
			mParamItemType = value;
		}
	}

	public int resultTotalItems
	{
		get;
		private set;
	}

	public int resultItems
	{
		get
		{
			if (resultRedStarItemStateList != null)
			{
				return resultRedStarItemStateList.Count;
			}
			return 0;
		}
	}

	private List<ServerRedStarItemState> resultRedStarItemStateList
	{
		get;
		set;
	}

	public NetServerGetRedStarExchangeList()
		: this(0)
	{
	}

	public NetServerGetRedStarExchangeList(int itemType)
	{
		paramItemType = itemType;
	}

	protected override void DoRequest()
	{
		SetAction("Store/getRedstarExchangeList");
		CPlusPlusLink instance = CPlusPlusLink.Instance;
		if (instance != null)
		{
			string getRedStarExchangeListString = instance.GetGetRedStarExchangeListString(paramItemType);
			Debug.Log("CPlusPlusLink.actRetry");
			WriteJsonString(getRedStarExchangeListString);
		}
	}

	protected override void DoDidSuccess(JsonData jdata)
	{
		GetResponse_RedStarItemStateList(jdata);
		GetResponse_TotalItems(jdata);
		GetResponse_BirthDay(jdata);
		GetResponse_MonthPurchase(jdata);
	}

	protected override void DoDidSuccessEmulation()
	{
	}

	private void SetParameter_ItemType()
	{
		WriteActionParamValue("itemType", paramItemType);
	}

	public ServerRedStarItemState GetResultRedStarItemState(int index)
	{
		if (0 <= index && resultItems > index)
		{
			return resultRedStarItemStateList[index];
		}
		return null;
	}

	private void GetResponse_RedStarItemStateList(JsonData jdata)
	{
		resultRedStarItemStateList = new List<ServerRedStarItemState>();
		JsonData jsonArray = NetUtil.GetJsonArray(jdata, "itemList");
		int count = jsonArray.Count;
		for (int i = 0; i < count; i++)
		{
			ServerRedStarItemState item = NetUtil.AnalyzeRedStarItemStateJson(jsonArray[i], string.Empty);
			resultRedStarItemStateList.Add(item);
		}
	}

	private void GetResponse_TotalItems(JsonData jdata)
	{
		resultTotalItems = NetUtil.GetJsonInt(jdata, "totalItems");
	}

	private void GetResponse_BirthDay(JsonData jdata)
	{
		resultBirthDay = NetUtil.GetJsonString(jdata, "birthday");
	}

	private void GetResponse_MonthPurchase(JsonData jdata)
	{
		resultMonthPurchase = NetUtil.GetJsonInt(jdata, "monthPurchase");
	}
}
