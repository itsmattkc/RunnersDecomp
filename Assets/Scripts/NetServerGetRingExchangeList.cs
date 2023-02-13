using LitJson;
using System.Collections.Generic;

public class NetServerGetRingExchangeList : NetBase
{
	public List<ServerRingExchangeList> m_ringExchangeList;

	public int m_totalItems;

	protected override void DoRequest()
	{
		SetAction("Store/getRingExchangeList");
	}

	protected override void DoDidSuccess(JsonData jdata)
	{
		GetResponse_RingExchangeList(jdata);
	}

	protected override void DoDidSuccessEmulation()
	{
	}

	private void GetResponse_RingExchangeList(JsonData jdata)
	{
		m_ringExchangeList = NetUtil.AnalyzeRingExchangeList(jdata);
		m_totalItems = NetUtil.AnalyzeRingExchangeListTotalItems(jdata);
	}
}
