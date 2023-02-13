using LitJson;

public class NetServerGetWheelOptions : NetBase
{
	public ServerWheelOptions resultWheelOptions
	{
		get;
		private set;
	}

	protected override void DoRequest()
	{
		SetAction("Spin/getWheelOptions");
		CPlusPlusLink instance = CPlusPlusLink.Instance;
		if (instance != null)
		{
			string onlySendBaseParamString = instance.GetOnlySendBaseParamString();
			WriteJsonString(onlySendBaseParamString);
		}
	}

	protected override void DoDidSuccess(JsonData jdata)
	{
		GetResponse_WheelOptions(jdata);
	}

	protected override void DoDidSuccessEmulation()
	{
		resultWheelOptions = ServerInterface.WheelOptions;
		resultWheelOptions.RefreshFakeState();
	}

	private void GetResponse_WheelOptions(JsonData jdata)
	{
		resultWheelOptions = NetUtil.AnalyzeWheelOptionsJson(jdata, "wheelOptions");
	}
}
