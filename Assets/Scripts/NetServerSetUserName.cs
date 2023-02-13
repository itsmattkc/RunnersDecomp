using LitJson;

public class NetServerSetUserName : NetBase
{
	public string userName
	{
		private get;
		set;
	}

	public ServerPlayerState resultPlayerState
	{
		get;
		private set;
	}

	public NetServerSetUserName(string userName)
	{
		this.userName = userName;
	}

	protected override void DoRequest()
	{
		SetAction("Player/setUserName");
		CPlusPlusLink instance = CPlusPlusLink.Instance;
		if (instance != null)
		{
			string setUserNameString = instance.GetSetUserNameString(userName);
			WriteJsonString(setUserNameString);
		}
	}

	protected override void DoDidSuccess(JsonData jdata)
	{
		GetResponse_PlayerState(jdata);
	}

	protected override void DoDidSuccessEmulation()
	{
	}

	private void SetParameter_UserName()
	{
		WriteActionParamValue("userName", userName);
	}

	private void GetResponse_PlayerState(JsonData jdata)
	{
		resultPlayerState = NetUtil.AnalyzePlayerStateJson(jdata, "playerState");
	}
}
