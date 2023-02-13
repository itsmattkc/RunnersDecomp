using LitJson;
using System.Collections.Generic;

public class NetServerCommitChaoWheelSpin : NetBase
{
	public int paramCount;

	public ServerPlayerState resultPlayerState
	{
		get;
		private set;
	}

	public ServerCharacterState[] resultCharacterState
	{
		get;
		private set;
	}

	public List<ServerChaoState> resultChaoState
	{
		get;
		private set;
	}

	public ServerChaoWheelOptions resultChaoWheelOptions
	{
		get;
		private set;
	}

	public ServerSpinResultGeneral resultSpinResultGeneral
	{
		get;
		private set;
	}

	public NetServerCommitChaoWheelSpin(int count)
	{
		paramCount = count;
	}

	protected override void DoRequest()
	{
		SetAction("Chao/commitChaoWheelSpin");
		CPlusPlusLink instance = CPlusPlusLink.Instance;
		if (instance != null)
		{
			string commitChaoWheelSpinString = instance.GetCommitChaoWheelSpinString(paramCount);
			Debug.Log("CPlusPlusLink.actRetry");
			WriteJsonString(commitChaoWheelSpinString);
		}
	}

	protected override void DoDidSuccess(JsonData jdata)
	{
		GetResponse_PlayerState(jdata);
		GetResponse_CharacterState(jdata);
		GetResponse_ChaoState(jdata);
		GetResponse_ChaoWheelOptions(jdata);
		GetResponse_ChaoWheelResult(jdata);
	}

	protected override void DoDidSuccessEmulation()
	{
	}

	private void SetParameter_ChaoWheelSpin()
	{
		WriteActionParamValue("count", paramCount);
	}

	private void GetResponse_PlayerState(JsonData jdata)
	{
		resultPlayerState = NetUtil.AnalyzePlayerStateJson(jdata, "playerState");
	}

	private void GetResponse_CharacterState(JsonData jdata)
	{
		resultCharacterState = NetUtil.AnalyzePlayerState_CharactersStates(jdata);
	}

	private void GetResponse_ChaoState(JsonData jdata)
	{
		resultChaoState = NetUtil.AnalyzePlayerState_ChaoStates(jdata);
	}

	private void GetResponse_ChaoWheelOptions(JsonData jdata)
	{
		resultChaoWheelOptions = NetUtil.AnalyzeChaoWheelOptionsJson(jdata, "chaoWheelOptions");
	}

	private void GetResponse_ChaoWheelResult(JsonData jdata)
	{
		resultSpinResultGeneral = NetUtil.AnalyzeSpinResultJson(jdata, "chaoSpinResultList");
	}
}
