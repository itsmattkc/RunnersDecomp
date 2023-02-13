using LitJson;
using System.Collections.Generic;

public class NetDebugLogin : NetBase
{
	public string paramLineId
	{
		get;
		set;
	}

	public string paramAltLineId
	{
		get;
		set;
	}

	public string paramLineAuth
	{
		get;
		set;
	}

	public string resultSessionId
	{
		get;
		private set;
	}

	public long resultEnergyRefreshTime
	{
		get;
		private set;
	}

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

	public NetDebugLogin()
		: this(string.Empty, string.Empty, string.Empty)
	{
	}

	public NetDebugLogin(string lineId, string altLineId, string lineAuth)
	{
		paramLineId = lineId;
		paramAltLineId = altLineId;
		paramLineAuth = lineAuth;
	}

	protected override void DoRequest()
	{
		SetAction("Debug/login");
		SetParameter_LineAuth();
	}

	protected override void DoDidSuccess(JsonData jdata)
	{
		GetResponse_SessionId(jdata);
		GetResponse_EnergyRefreshTime(jdata);
		GetResponse_RingItemStateList(jdata);
	}

	protected override void DoDidSuccessEmulation()
	{
	}

	private void SetParameter_LineAuth()
	{
		Dictionary<string, object> dictionary = new Dictionary<string, object>(2);
		dictionary.Add("lineId", paramLineId);
		dictionary.Add("altLineId", paramAltLineId);
		dictionary.Add("lineAuthToken", paramLineAuth);
		WriteActionParamObject("lineAuth", dictionary);
		dictionary.Clear();
		dictionary = null;
	}

	public ServerRingItemState GetResultRingItemState(int index)
	{
		if (0 <= index && resultRingItemStates > index)
		{
			return resultRingItemStateList[index];
		}
		return null;
	}

	private void GetResponse_SessionId(JsonData jdata)
	{
		resultSessionId = NetUtil.GetJsonString(jdata, "sessionId");
	}

	private void GetResponse_EnergyRefreshTime(JsonData jdata)
	{
		resultEnergyRefreshTime = NetUtil.GetJsonLong(jdata, "energyRecoveryTime");
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
