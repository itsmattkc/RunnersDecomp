using LitJson;

public class NetServerGetMigrationPassword : NetBase
{
	public string paramUserPassword
	{
		private get;
		set;
	}

	public string paramMigrationPassword
	{
		get;
		set;
	}

	public NetServerGetMigrationPassword()
		: this(string.Empty)
	{
	}

	public NetServerGetMigrationPassword(string userPassword)
	{
		paramUserPassword = userPassword;
	}

	protected override void DoRequest()
	{
		SetAction("Login/getMigrationPassword");
		CPlusPlusLink instance = CPlusPlusLink.Instance;
		if (instance != null)
		{
			string getMigrationPasswordString = instance.GetGetMigrationPasswordString(paramUserPassword);
			WriteJsonString(getMigrationPasswordString);
		}
	}

	protected override void DoDidSuccess(JsonData jdata)
	{
		GetResponse_MigrationPassword(jdata);
	}

	protected override void DoDidSuccessEmulation()
	{
	}

	private void SetParameter_UserPassword()
	{
		WriteActionParamValue("userPassword", paramUserPassword);
	}

	private void GetResponse_MigrationPassword(JsonData jdata)
	{
		paramMigrationPassword = NetUtil.GetJsonString(jdata, "password");
	}
}
