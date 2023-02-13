using LitJson;

public class NetServerRequestEnergy : NetBase
{
	public string paramFriendId
	{
		get;
		set;
	}

	public ServerPlayerState resultPlayerState
	{
		get;
		private set;
	}

	public long resultExpireTime
	{
		get;
		private set;
	}

	public NetServerRequestEnergy()
		: this(string.Empty)
	{
	}

	public NetServerRequestEnergy(string friendId)
	{
		paramFriendId = friendId;
	}

	protected override void DoRequest()
	{
		SetAction("Friend/requestEnergy");
		SetParameter_FriendId();
	}

	protected override void DoDidSuccess(JsonData jdata)
	{
		GetResponse_PlayerState(jdata);
		GetResponse_ExpireTime(jdata);
	}

	protected override void DoDidSuccessEmulation()
	{
		resultPlayerState = ServerInterface.PlayerState;
		resultPlayerState.RefreshFakeState();
	}

	private void SetParameter_FriendId()
	{
		WriteActionParamValue("friendId", paramFriendId);
	}

	private void GetResponse_PlayerState(JsonData jdata)
	{
		resultPlayerState = NetUtil.AnalyzePlayerStateJson(jdata, "playerState");
	}

	private void GetResponse_ExpireTime(JsonData jdata)
	{
		resultExpireTime = NetUtil.GetJsonLong(jdata, "expireTime");
	}
}
