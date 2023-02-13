using LitJson;
using System.Collections.Generic;

public class NetServerGetCostList : NetBase
{
	public List<ServerConsumedCostData> resultCostList
	{
		get;
		set;
	}

	protected override void DoRequest()
	{
		SetAction("Game/getCostList");
		CPlusPlusLink instance = CPlusPlusLink.Instance;
		if (instance != null)
		{
			string onlySendBaseParamString = instance.GetOnlySendBaseParamString();
			WriteJsonString(onlySendBaseParamString);
		}
	}

	protected override void DoDidSuccess(JsonData jdata)
	{
		GetResponse_CostList(jdata);
	}

	protected override void DoDidSuccessEmulation()
	{
	}

	private void GetResponse_CostList(JsonData jdata)
	{
		resultCostList = NetUtil.AnalyzeConsumedCostDataList(jdata);
	}
}
