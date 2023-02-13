using LitJson;

public class NetServerGetCharacterState : NetBase
{
	public ServerCharacterState[] resultCharacterState
	{
		get;
		private set;
	}

	protected override void DoRequest()
	{
		SetAction("Player/getCharacterState");
		CPlusPlusLink instance = CPlusPlusLink.Instance;
		if (instance != null)
		{
			string onlySendBaseParamString = instance.GetOnlySendBaseParamString();
			WriteJsonString(onlySendBaseParamString);
		}
	}

	protected override void DoDidSuccess(JsonData jdata)
	{
		GetResponse_CharacterState(jdata);
	}

	protected override void DoDidSuccessEmulation()
	{
	}

	private void GetResponse_CharacterState(JsonData jdata)
	{
		resultCharacterState = NetUtil.AnalyzePlayerState_CharactersStates(jdata);
	}
}
