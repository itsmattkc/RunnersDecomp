using LitJson;
using System.Collections.Generic;

public class NetServerGetWheelSpinInfo : NetBase
{
	public List<ServerWheelSpinInfo> resultWheelSpinInfos
	{
		get;
		private set;
	}

	protected override void DoRequest()
	{
		SetAction("Spin/getWheelSpinInfo");
		CPlusPlusLink instance = CPlusPlusLink.Instance;
		if (instance != null)
		{
			string onlySendBaseParamString = instance.GetOnlySendBaseParamString();
			WriteJsonString(onlySendBaseParamString);
		}
	}

	protected override void DoDidSuccess(JsonData jdata)
	{
		GetResponse_WheelSpinInfo(jdata);
	}

	protected override void DoDidSuccessEmulation()
	{
	}

	private void GetResponse_WheelSpinInfo(JsonData jdata)
	{
		resultWheelSpinInfos = NetUtil.AnalyzeWheelSpinInfo(jdata, "infoList");
	}
}
