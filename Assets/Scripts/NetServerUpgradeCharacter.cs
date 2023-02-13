using LitJson;

public class NetServerUpgradeCharacter : NetBase
{
	public int paramCharacterId
	{
		get;
		set;
	}

	public int paramAbilityId
	{
		get;
		set;
	}

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

	public NetServerUpgradeCharacter()
		: this(0, 0)
	{
	}

	public NetServerUpgradeCharacter(int characterId, int abilityId)
	{
		paramCharacterId = characterId;
		paramAbilityId = abilityId;
	}

	protected override void DoRequest()
	{
		SetAction("Character/upgradeCharacter");
		CPlusPlusLink instance = CPlusPlusLink.Instance;
		if (instance != null)
		{
			string upgradeCharacterString = instance.GetUpgradeCharacterString(paramCharacterId, paramAbilityId);
			Debug.Log("CPlusPlusLink.actRetry");
			WriteJsonString(upgradeCharacterString);
		}
	}

	protected override void DoDidSuccess(JsonData jdata)
	{
		GetResponse_PlayerState(jdata);
		GetResponse_CharacterState(jdata);
	}

	protected override void DoDidSuccessEmulation()
	{
		resultPlayerState = ServerInterface.PlayerState;
		resultPlayerState.RefreshFakeState();
	}

	private void SetParameter()
	{
		WriteActionParamValue("characterId", paramCharacterId);
		WriteActionParamValue("abilityId", paramAbilityId);
	}

	private void GetResponse_PlayerState(JsonData jdata)
	{
		resultPlayerState = NetUtil.AnalyzePlayerStateJson(jdata, "playerState");
	}

	private void GetResponse_CharacterState(JsonData jdata)
	{
		resultCharacterState = NetUtil.AnalyzePlayerState_CharactersStates(jdata);
	}
}
