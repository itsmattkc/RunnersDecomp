using LitJson;

public class NetServerUnlockedCharacter : NetBase
{
	public CharaType charaType
	{
		get;
		set;
	}

	public ServerItem serverItem
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

	public NetServerUnlockedCharacter(CharaType charaType, ServerItem serverItem)
	{
		this.charaType = charaType;
		this.serverItem = serverItem;
	}

	protected override void DoRequest()
	{
		SetAction("Character/unlockedCharacter");
		CPlusPlusLink instance = CPlusPlusLink.Instance;
		if (instance != null)
		{
			int characterId = 0;
			ServerCharacterState serverCharacterState = ServerInterface.PlayerState.CharacterState(charaType);
			if (serverCharacterState != null)
			{
				characterId = serverCharacterState.Id;
			}
			string unlockedCharacterString = instance.GetUnlockedCharacterString(characterId, (int)serverItem.id);
			Debug.Log("CPlusPlusLink.actRetry");
			WriteJsonString(unlockedCharacterString);
		}
	}

	protected override void DoDidSuccess(JsonData jdata)
	{
		GetResponse_PlayerState(jdata);
		GetResponse_CharacterState(jdata);
	}

	protected override void DoDidSuccessEmulation()
	{
	}

	private void SetParameter_CharaType()
	{
		ServerCharacterState serverCharacterState = ServerInterface.PlayerState.CharacterState(charaType);
		if (serverCharacterState != null)
		{
			int id = serverCharacterState.Id;
			WriteActionParamValue("characterId", id);
		}
	}

	private void SetParameter_Item()
	{
		int id = (int)serverItem.id;
		WriteActionParamValue("itemId", id);
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
