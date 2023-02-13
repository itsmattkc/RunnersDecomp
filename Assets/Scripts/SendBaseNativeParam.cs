public struct SendBaseNativeParam
{
	public string sessionId
	{
		get;
		private set;
	}

	public string version
	{
		get;
		private set;
	}

	public string seq
	{
		get;
		private set;
	}

	public void Init()
	{
		ServerLoginState loginState = ServerInterface.LoginState;
		if (loginState.IsLoggedIn)
		{
			sessionId = loginState.sessionId;
		}
		else
		{
			sessionId = string.Empty;
		}
		version = CurrentBundleVersion.version;
		seq = (loginState.seqNum + 1).ToString();
	}
}
