using LitJson;
using System.Collections.Generic;

public class NetServerGetTicker : NetBase
{
	private List<ServerTickerData> m_tickerData;

	public NetServerGetTicker()
	{
		m_tickerData = new List<ServerTickerData>();
	}

	protected override void DoRequest()
	{
		SetAction("login/getTicker");
		CPlusPlusLink instance = CPlusPlusLink.Instance;
		if (instance != null)
		{
			string onlySendBaseParamString = instance.GetOnlySendBaseParamString();
			WriteJsonString(onlySendBaseParamString);
		}
	}

	protected override void DoDidSuccess(JsonData jdata)
	{
		m_tickerData.Clear();
		JsonData jsonArray = NetUtil.GetJsonArray(jdata, "tickerList");
		int count = jsonArray.Count;
		for (int i = 0; i < count; i++)
		{
			JsonData jdata2 = jsonArray[i];
			long jsonLong = NetUtil.GetJsonLong(jdata2, "id");
			long jsonLong2 = NetUtil.GetJsonLong(jdata2, "start");
			long jsonLong3 = NetUtil.GetJsonLong(jdata2, "end");
			string jsonString = NetUtil.GetJsonString(jdata2, "param");
			AddInfo(jsonLong, jsonLong2, jsonLong3, jsonString);
		}
	}

	protected override void DoDidSuccessEmulation()
	{
	}

	public ServerTickerData GetInfo(int index)
	{
		if (index < m_tickerData.Count)
		{
			return m_tickerData[index];
		}
		return null;
	}

	public int GetInfoCount()
	{
		return m_tickerData.Count;
	}

	private void AddInfo(long id, long start, long end, string param)
	{
		ServerTickerData serverTickerData = new ServerTickerData();
		serverTickerData.Init(id, start, end, param);
		m_tickerData.Add(serverTickerData);
	}
}
