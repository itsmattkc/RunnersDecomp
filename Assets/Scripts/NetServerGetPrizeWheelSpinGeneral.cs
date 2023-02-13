using LitJson;

public class NetServerGetPrizeWheelSpinGeneral : NetBase
{
	public int paramEventId;

	public int paramSpinType;

	public ServerPrizeState resultPrizeState;

	public NetServerGetPrizeWheelSpinGeneral(int eventId, int spinType)
	{
		paramEventId = eventId;
		paramSpinType = spinType;
	}

	protected override void DoRequest()
	{
		SetAction("RaidbossSpin/getPrizeRaidbossWheelSpin");
		CPlusPlusLink instance = CPlusPlusLink.Instance;
		if (instance != null)
		{
			string getPrizeWheelSpinGeneralString = instance.GetGetPrizeWheelSpinGeneralString(paramEventId, paramSpinType);
			Debug.Log("CPlusPlusLink.actRetry");
			WriteJsonString(getPrizeWheelSpinGeneralString);
		}
	}

	protected override void DoDidSuccess(JsonData jdata)
	{
		GetResponse_PrizeWheelSpinGeneral(jdata);
	}

	protected override void DoDidSuccessEmulation()
	{
	}

	private void SetParameter_WheelSpinGeneral()
	{
		WriteActionParamValue("eventId", paramEventId);
		WriteActionParamValue("raidbossWheelSpinType", paramSpinType);
	}

	private void GetResponse_PrizeWheelSpinGeneral(JsonData jdata)
	{
		resultPrizeState = NetUtil.AnalyzePrizeWheelSpinGeneral(jdata);
	}
}
