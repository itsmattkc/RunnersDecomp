using LitJson;

public class NetDebugDeleteUserData : NetBase
{
	public int paramUserId
	{
		get;
		set;
	}

	public NetDebugDeleteUserData()
		: this(0)
	{
	}

	public NetDebugDeleteUserData(int userId)
	{
		paramUserId = userId;
	}

	protected override void DoRequest()
	{
		SetAction("Debug/deleteUserData");
		SetParameter_User();
	}

	protected override void DoDidSuccess(JsonData jdata)
	{
	}

	protected override void DoDidSuccessEmulation()
	{
	}

	private void SetParameter_User()
	{
		WriteActionParamValue("userId", paramUserId);
	}
}
