using LitJson;

public class NetServerGetChaoWheelOptions : NetBase
{
	public ServerChaoWheelOptions resultChaoWheelOptions
	{
		get;
		private set;
	}

	protected override void DoRequest()
	{
		SetAction("Chao/getChaoWheelOptions");
		CPlusPlusLink instance = CPlusPlusLink.Instance;
		if (instance != null)
		{
			string onlySendBaseParamString = instance.GetOnlySendBaseParamString();
			WriteJsonString(onlySendBaseParamString);
		}
	}

	protected override void DoDidSuccess(JsonData jdata)
	{
		GetResponse_ChaoWheelOptions(jdata);
	}

	protected override void DoDidSuccessEmulation()
	{
	}

	private void GetResponse_ChaoWheelOptions(JsonData jdata)
	{
		if (NetUtil.IsExist(jdata, "chaoWheelOptions"))
		{
			resultChaoWheelOptions = NetUtil.AnalyzeChaoWheelOptionsJson(jdata, "chaoWheelOptions");
		}
		else
		{
			resultChaoWheelOptions = NetUtil.AnalyzeChaoWheelOptionsJson(jdata, "wheelOptions");
		}
	}
}
