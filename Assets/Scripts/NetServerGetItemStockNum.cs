using LitJson;
using System.Collections.Generic;

public class NetServerGetItemStockNum : NetBase
{
	public int paramEventId;

	public List<int> paramItemId;

	public List<ServerItemState> m_itemStockList
	{
		get;
		set;
	}

	public NetServerGetItemStockNum(int eventId, List<int> itemId)
	{
		paramEventId = eventId;
		paramItemId = itemId;
	}

	protected override void DoRequest()
	{
		SetAction("RaidbossSpin/getItemStockNum");
		int eventId = paramEventId;
		int[] itemIdList = paramItemId.ToArray();
		CPlusPlusLink instance = CPlusPlusLink.Instance;
		if (instance != null)
		{
			string getItemStockNumString = instance.GetGetItemStockNumString(eventId, itemIdList);
			Debug.Log("NetServerGetItemStockNum.json = " + getItemStockNumString);
			WriteJsonString(getItemStockNumString);
		}
	}

	protected override void DoDidSuccess(JsonData jdata)
	{
		GetResponse_itemStock(jdata);
	}

	protected override void DoDidSuccessEmulation()
	{
	}

	private void SetParameter()
	{
		WriteActionParamValue("eventId", paramEventId);
		List<object> list = new List<object>();
		foreach (int item2 in paramItemId)
		{
			object item = item2;
			list.Add(item);
		}
		WriteActionParamArray("itemIdList", list);
	}

	private void GetResponse_itemStock(JsonData jdata)
	{
		m_itemStockList = new List<ServerItemState>();
		JsonData jsonArray = NetUtil.GetJsonArray(jdata, "itemStockList");
		int count = jsonArray.Count;
		for (int i = 0; i < count; i++)
		{
			ServerItemState item = NetUtil.AnalyzeItemStateJson(jsonArray[i], string.Empty);
			m_itemStockList.Add(item);
		}
	}
}
