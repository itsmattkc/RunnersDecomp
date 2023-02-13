using LitJson;

public class NetServerSendEnergy : NetBase
{
	public string paramFriendId
	{
		get;
		set;
	}

	public int resultExpireTime
	{
		get;
		private set;
	}

	public NetServerSendEnergy()
	{
	}

	public NetServerSendEnergy(string friendId)
	{
		paramFriendId = friendId;
	}

	protected override void DoRequest()
	{
		SetAction("Message/sendEnergy");
		CPlusPlusLink instance = CPlusPlusLink.Instance;
		if (instance != null)
		{
			string sendEnergyString = instance.GetSendEnergyString(paramFriendId);
			WriteJsonString(sendEnergyString);
		}
	}

	protected override void DoDidSuccess(JsonData jdata)
	{
		GetResponse_ExpireTime(jdata);
	}

	protected override void DoDidSuccessEmulation()
	{
	}

	private void SetParameter_FriendId()
	{
		WriteActionParamValue("friendId", paramFriendId);
	}

	private void GetResponse_ExpireTime(JsonData jdata)
	{
		resultExpireTime = NetUtil.GetJsonInt(jdata, "expireTime");
	}
}
