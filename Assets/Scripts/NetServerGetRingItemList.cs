using LitJson;
using System.Collections.Generic;

public class NetServerGetRingItemList : NetBase
{
	public int resultRingItemStates
	{
		get
		{
			if (resultRingItemStateList != null)
			{
				return resultRingItemStateList.Count;
			}
			return 0;
		}
	}

	private List<ServerRingItemState> resultRingItemStateList
	{
		get;
		set;
	}

	protected override void DoRequest()
	{
		SetAction("Game/getRingItemList");
	}

	protected override void DoDidSuccess(JsonData jdata)
	{
		GetResponse_RingItemStateList(jdata);
	}

	protected override void DoDidSuccessEmulation()
	{
	}

	public ServerRingItemState GetResultRingItemState(int index)
	{
		if (0 <= index && resultRingItemStates > index)
		{
			return resultRingItemStateList[index];
		}
		return null;
	}

	private void GetResponse_RingItemStateList(JsonData jdata)
	{
		resultRingItemStateList = new List<ServerRingItemState>();
		JsonData jsonArray = NetUtil.GetJsonArray(jdata, "ringItemList");
		int count = jsonArray.Count;
		for (int i = 0; i < count; i++)
		{
			JsonData jdata2 = jsonArray[i];
			ServerRingItemState item = NetUtil.AnalyzeRingItemStateJson(jdata2, string.Empty);
			resultRingItemStateList.Add(item);
		}
	}
}
