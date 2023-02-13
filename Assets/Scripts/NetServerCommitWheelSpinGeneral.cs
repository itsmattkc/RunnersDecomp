using LitJson;
using System.Collections.Generic;

public class NetServerCommitWheelSpinGeneral : NetBase
{
	public int paramEventId;

	public int paramSpinId;

	public int paramSpinCostItemId;

	public int paramSpinNum;

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

	public ServerWheelOptionsGeneral resultWheelOptionsGen
	{
		get;
		private set;
	}

	public ServerSpinResultGeneral resultWheelResultGen
	{
		get;
		private set;
	}

	public NetServerCommitWheelSpinGeneral(int eventId, int spinId, int spinCostItemId, int spinNum)
	{
		paramEventId = eventId;
		paramSpinId = spinId;
		paramSpinCostItemId = spinCostItemId;
		paramSpinNum = spinNum;
	}

	protected override void DoRequest()
	{
		SetAction("RaidbossSpin/commitRaidbossWheelSpin");
		CPlusPlusLink instance = CPlusPlusLink.Instance;
		if (instance != null)
		{
			string commitWheelSpinGeneralString = instance.GetCommitWheelSpinGeneralString(paramEventId, paramSpinCostItemId, paramSpinId, paramSpinNum);
			Debug.Log("CPlusPlusLink.actRetry");
			WriteJsonString(commitWheelSpinGeneralString);
		}
	}

	protected override void DoDidSuccess(JsonData jdata)
	{
		GetResponse_PlayerState(jdata);
		GetResponse_CharacterState(jdata);
		GetResponse_ChaoState(jdata);
		GetResponse_WheelOptionsGen(jdata);
		GetResponse_WheelResultGen(jdata);
	}

	protected override void DoDidSuccessEmulation()
	{
	}

	private void SetParameter()
	{
		WriteActionParamValue("eventId", paramEventId);
		WriteActionParamValue("id", paramSpinId);
		WriteActionParamValue("costItemId", paramSpinCostItemId);
		WriteActionParamValue("num", paramSpinNum);
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

	private void GetResponse_WheelOptionsGen(JsonData jdata)
	{
		resultWheelOptionsGen = NetUtil.AnalyzeWheelOptionsGeneralJson(jdata, "raidbossWheelOptions");
	}

	private void GetResponse_WheelResultGen(JsonData jdata)
	{
		resultWheelResultGen = NetUtil.AnalyzeSpinResultGeneralJson(jdata, "raidbossSpinResult");
	}
}
