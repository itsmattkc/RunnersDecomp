using LitJson;
using System.Collections.Generic;

public class NetServerGetEventList : NetBase
{
	public List<ServerEventEntry> resultEventList;

	protected override void DoRequest()
	{
		SetAction("Event/getEventList");
		CPlusPlusLink instance = CPlusPlusLink.Instance;
		if (instance != null)
		{
			string onlySendBaseParamString = instance.GetOnlySendBaseParamString();
			WriteJsonString(onlySendBaseParamString);
		}
	}

	protected override void DoDidSuccess(JsonData jdata)
	{
		GetResponse_EventList(jdata);
	}

	protected override void DoDidSuccessEmulation()
	{
	}

	private void GetResponse_EventList(JsonData jdata)
	{
		resultEventList = NetUtil.AnalyzeEventList(jdata);
	}
}
