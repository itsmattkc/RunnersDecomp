using LitJson;

public class NetDebugResetUserData : NetBase
{
	public int paramUserId
	{
		get;
		set;
	}

	public NetDebugResetUserData()
	{
	}

	public NetDebugResetUserData(int userId)
	{
		paramUserId = userId;
	}

	protected override void DoRequest()
	{
		SetAction("Debug/resetUserData");
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
