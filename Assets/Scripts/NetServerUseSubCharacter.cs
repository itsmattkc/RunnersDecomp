using LitJson;

public class NetServerUseSubCharacter : NetBase
{
	public bool useFlag
	{
		get;
		set;
	}

	public ServerPlayerState resultPlayerState
	{
		get;
		private set;
	}

	public NetServerUseSubCharacter()
		: this(false)
	{
	}

	public NetServerUseSubCharacter(bool useFlag)
	{
		this.useFlag = useFlag;
	}

	protected override void DoRequest()
	{
		SetAction("Character/useSubCharacter");
		SetParameter_UseFlag();
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
		WriteActionParamValue("use_flag", useFlag ? 1 : 0);
	}

	private void GetResponse_PlayerState(JsonData jdata)
	{
		resultPlayerState = NetUtil.AnalyzePlayerStateJson(jdata, "playerState");
	}
}
