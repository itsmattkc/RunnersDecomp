using LitJson;

public class NetServerGetPrizeChaoWheelSpin : NetBase
{
	public int paramChaoWheelSpinType;

	public ServerPrizeState resultPrizeState;

	public NetServerGetPrizeChaoWheelSpin(int chaoWheelSpinType)
	{
		paramChaoWheelSpinType = chaoWheelSpinType;
	}

	protected override void DoRequest()
	{
		SetAction("Chao/getPrizeChaoWheelSpin");
		CPlusPlusLink instance = CPlusPlusLink.Instance;
		if (instance != null)
		{
			string getPrizeChaoWheelSpinString = instance.GetGetPrizeChaoWheelSpinString(paramChaoWheelSpinType);
			Debug.Log("CPlusPlusLink.actRetry");
			WriteJsonString(getPrizeChaoWheelSpinString);
		}
	}

	protected override void DoDidSuccess(JsonData jdata)
	{
		GetResponse_PrizeChaoWheelSpin(jdata);
	}

	protected override void DoDidSuccessEmulation()
	{
	}

	private void SetParameter_ChaoWheelSpinType()
	{
		WriteActionParamValue("chaoWheelSpinType", paramChaoWheelSpinType);
	}

	private void GetResponse_PrizeChaoWheelSpin(JsonData jdata)
	{
		resultPrizeState = NetUtil.AnalyzePrizeChaoWheelSpin(jdata);
	}
}
