using LitJson;
using System.Collections.Generic;

public class NetServerGetFirstLaunchChao : NetBase
{
	public ServerPlayerState resultPlayerState
	{
		get;
		private set;
	}

	public List<ServerChaoState> resultChaoState
	{
		get;
		private set;
	}

	protected override void DoRequest()
	{
		SetAction("Chao/getFirstLaunchChao");
		CPlusPlusLink instance = CPlusPlusLink.Instance;
		if (instance != null)
		{
			string onlySendBaseParamString = instance.GetOnlySendBaseParamString();
			WriteJsonString(onlySendBaseParamString);
		}
	}

	protected override void DoDidSuccess(JsonData jdata)
	{
		GetResponse_PlayerState(jdata);
		GetResponse_ChaoState(jdata);
	}

	protected override void DoDidSuccessEmulation()
	{
	}

	private void GetResponse_PlayerState(JsonData jdata)
	{
		resultPlayerState = NetUtil.AnalyzePlayerStateJson(jdata, "playerState");
	}

	private void GetResponse_ChaoState(JsonData jdata)
	{
		resultChaoState = NetUtil.AnalyzePlayerState_ChaoStates(jdata);
	}
}
