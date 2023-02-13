using LitJson;
using System.Collections.Generic;

public class NetServerGetChaoState : NetBase
{
	public List<ServerChaoState> resultChaoState
	{
		get;
		private set;
	}

	protected override void DoRequest()
	{
		SetAction("Player/getChaoState");
		CPlusPlusLink instance = CPlusPlusLink.Instance;
		if (instance != null)
		{
			string onlySendBaseParamString = instance.GetOnlySendBaseParamString();
			WriteJsonString(onlySendBaseParamString);
		}
	}

	protected override void DoDidSuccess(JsonData jdata)
	{
		GetResponse_ChaoState(jdata);
	}

	protected override void DoDidSuccessEmulation()
	{
	}

	private void GetResponse_ChaoState(JsonData jdata)
	{
		resultChaoState = NetUtil.AnalyzePlayerState_ChaoStates(jdata);
	}
}
