using LitJson;

public class NetServerGetEventState : NetBase
{
	public int paramEventId;

	public ServerEventState resultEventState;

	public NetServerGetEventState(int eventId)
	{
		paramEventId = eventId;
	}

	protected override void DoRequest()
	{
		SetAction("Event/getEventState");
		CPlusPlusLink instance = CPlusPlusLink.Instance;
		if (instance != null)
		{
			string getEventStateString = instance.GetGetEventStateString(paramEventId);
			Debug.Log("CPlusPlusLink.actRetry");
			WriteJsonString(getEventStateString);
		}
	}

	protected override void DoDidSuccess(JsonData jdata)
	{
		GetResponse_EventState(jdata);
	}

	protected override void DoDidSuccessEmulation()
	{
	}

	private void SetParameter_EventId()
	{
		WriteActionParamValue("eventId", paramEventId);
	}

	private void GetResponse_EventState(JsonData jdata)
	{
		resultEventState = NetUtil.AnalyzeEventState(jdata);
	}
}
