using LitJson;

public class NetServerGetWheelOptionsGeneral : NetBase
{
	public int paramEventId;

	public int paramSpinId;

	public ServerWheelOptionsGeneral resultWheelOptionsGeneral
	{
		get;
		private set;
	}

	public NetServerGetWheelOptionsGeneral(int eventId, int spinId)
	{
		paramEventId = eventId;
		paramSpinId = spinId;
	}

	protected override void DoRequest()
	{
		SetAction("RaidbossSpin/getRaidbossWheelOptions");
		CPlusPlusLink instance = CPlusPlusLink.Instance;
		if (instance != null)
		{
			string getWheelSpinGeneralString = instance.GetGetWheelSpinGeneralString(paramEventId, paramSpinId);
			Debug.Log("CPlusPlusLink.actRetry");
			WriteJsonString(getWheelSpinGeneralString);
		}
	}

	protected override void DoDidSuccess(JsonData jdata)
	{
		GetResponse_WheelOptionsGeneral(jdata);
	}

	protected override void DoDidSuccessEmulation()
	{
	}

	private void SetParameter()
	{
		WriteActionParamValue("eventId", paramEventId);
		WriteActionParamValue("id", paramSpinId);
	}

	private void GetResponse_WheelOptionsGeneral(JsonData jdata)
	{
		resultWheelOptionsGeneral = NetUtil.AnalyzeWheelOptionsGeneralJson(jdata, "raidbossWheelOptions");
	}
}
