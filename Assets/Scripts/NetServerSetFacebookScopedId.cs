using LitJson;

public class NetServerSetFacebookScopedId : NetBase
{
	public string paramUserId
	{
		get;
		set;
	}

	public NetServerSetFacebookScopedId()
		: this(string.Empty)
	{
	}

	public NetServerSetFacebookScopedId(string userId)
	{
		paramUserId = userId;
	}

	protected override void DoRequest()
	{
		SetAction("Friend/setFacebookScopedId");
		CPlusPlusLink instance = CPlusPlusLink.Instance;
		if (instance != null)
		{
			string setFacebookScopedIdString = instance.GetSetFacebookScopedIdString(paramUserId);
			Debug.Log("CPlusPlusLink.actRetry");
			WriteJsonString(setFacebookScopedIdString);
		}
	}

	protected override void DoDidSuccess(JsonData jdata)
	{
	}

	protected override void DoDidSuccessEmulation()
	{
	}

	private void SetParameter_UserId()
	{
		WriteActionParamValue("facebookId", paramUserId);
	}
}
