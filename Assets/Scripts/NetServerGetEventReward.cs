using LitJson;
using System.Collections.Generic;

public class NetServerGetEventReward : NetBase
{
	public int paramEventId;

	public List<ServerEventReward> resultEventRewardList;

	public NetServerGetEventReward(int eventId)
	{
		paramEventId = eventId;
	}

	protected override void DoRequest()
	{
		SetAction("Event/getEventReward");
		CPlusPlusLink instance = CPlusPlusLink.Instance;
		if (instance != null)
		{
			string eventRewardString = instance.GetEventRewardString(paramEventId);
			Debug.Log("CPlusPlusLink.actRetry");
			WriteJsonString(eventRewardString);
		}
	}

	protected override void DoDidSuccess(JsonData jdata)
	{
		GetResponse_EventRewardList(jdata);
	}

	protected override void DoDidSuccessEmulation()
	{
	}

	private void SetParameter_EventId()
	{
		WriteActionParamValue("eventId", paramEventId);
	}

	private void GetResponse_EventRewardList(JsonData jdata)
	{
		resultEventRewardList = NetUtil.AnalyzeEventReward(jdata);
	}
}
