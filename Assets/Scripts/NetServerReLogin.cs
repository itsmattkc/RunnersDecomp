using LitJson;

public class NetServerReLogin : NetBase
{
	public string resultSessionId
	{
		get;
		private set;
	}

	public int sessionTimeLimit
	{
		get;
		private set;
	}

	protected override void DoRequest()
	{
		SetAction("Login/reLogin");
		CPlusPlusLink instance = CPlusPlusLink.Instance;
		if (instance != null)
		{
			string onlySendBaseParamString = instance.GetOnlySendBaseParamString();
			WriteJsonString(onlySendBaseParamString);
		}
	}

	protected override void DoDidSuccess(JsonData jdata)
	{
		GetResponse_SessionId(jdata);
	}

	protected override void DoDidSuccessEmulation()
	{
	}

	private void GetResponse_SessionId(JsonData jdata)
	{
		resultSessionId = NetUtil.GetJsonString(jdata, "sessionId");
		sessionTimeLimit = NetUtil.GetJsonInt(jdata, "sessionTimeLimit");
	}
}
