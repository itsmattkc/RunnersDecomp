using LitJson;

public class NetServerChangeCharacter : NetBase
{
	public int mainCharaId
	{
		get;
		set;
	}

	public int subCharaId
	{
		get;
		set;
	}

	public ServerPlayerState resultPlayerState
	{
		get;
		private set;
	}

	public NetServerChangeCharacter()
		: this(0, 0)
	{
	}

	public NetServerChangeCharacter(int mainCharaId, int subCharaId)
	{
		this.mainCharaId = mainCharaId;
		this.subCharaId = subCharaId;
	}

	protected override void DoRequest()
	{
		SetAction("Character/changeCharacter");
		CPlusPlusLink instance = CPlusPlusLink.Instance;
		if (instance != null)
		{
			string changeCharacterString = instance.GetChangeCharacterString(mainCharaId, subCharaId);
			Debug.Log("CPlusPlusLink.actRetry");
			WriteJsonString(changeCharacterString);
		}
	}

	protected override void DoDidSuccess(JsonData jdata)
	{
		GetResponse_PlayerState(jdata);
	}

	protected override void DoDidSuccessEmulation()
	{
	}

	private void SetParameter_UseFlag()
	{
		WriteActionParamValue("mainCharacterId", mainCharaId);
		WriteActionParamValue("subCharacterId", subCharaId);
	}

	private void GetResponse_PlayerState(JsonData jdata)
	{
		resultPlayerState = NetUtil.AnalyzePlayerStateJson(jdata, "playerState");
	}
}
