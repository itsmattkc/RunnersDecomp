using LitJson;

public class NetServerGetVariousParameter : NetBase
{
	public int resultEnergyRecveryTime
	{
		get;
		set;
	}

	public int resultEnergyRecoveryMax
	{
		get;
		set;
	}

	public int resultOnePlayCmCount
	{
		get;
		set;
	}

	public int resultOnePlayContinueCount
	{
		get;
		set;
	}

	public int resultCmSkipCount
	{
		get;
		set;
	}

	public bool resultIsPurchased
	{
		get;
		set;
	}

	protected override void DoRequest()
	{
		SetAction("Login/getVariousParameter");
		CPlusPlusLink instance = CPlusPlusLink.Instance;
		if (instance != null)
		{
			string onlySendBaseParamString = instance.GetOnlySendBaseParamString();
			WriteJsonString(onlySendBaseParamString);
		}
	}

	protected override void DoDidSuccess(JsonData jdata)
	{
		GetResponseVariousParameter(jdata);
	}

	protected override void DoDidSuccessEmulation()
	{
	}

	private void GetResponseVariousParameter(JsonData jdata)
	{
		resultEnergyRecveryTime = NetUtil.GetJsonInt(jdata, "energyRecveryTime");
		resultEnergyRecoveryMax = NetUtil.GetJsonInt(jdata, "energyRecoveryMax");
		resultOnePlayCmCount = NetUtil.GetJsonInt(jdata, "onePlayCmCount");
		resultOnePlayContinueCount = NetUtil.GetJsonInt(jdata, "onePlayContinueCount");
		resultCmSkipCount = NetUtil.GetJsonInt(jdata, "cmSkipCount");
		resultIsPurchased = NetUtil.GetJsonFlag(jdata, "isPurchased");
	}
}
