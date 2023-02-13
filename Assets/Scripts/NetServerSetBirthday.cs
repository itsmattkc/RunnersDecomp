using LitJson;

public class NetServerSetBirthday : NetBase
{
	public string birthday
	{
		get;
		set;
	}

	public NetServerSetBirthday()
		: this(string.Empty)
	{
	}

	public NetServerSetBirthday(string birthday)
	{
		this.birthday = birthday;
	}

	protected override void DoRequest()
	{
		SetAction("Store/setBirthday");
		CPlusPlusLink instance = CPlusPlusLink.Instance;
		if (instance != null)
		{
			string setBirthdayString = instance.GetSetBirthdayString(birthday);
			Debug.Log("CPlusPlusLink.actRetry");
			WriteJsonString(setBirthdayString);
		}
	}

	protected override void DoDidSuccess(JsonData jdata)
	{
		ServerInterface.SettingState.m_birthday = NetUtil.GetJsonString(jdata, "birthday");
	}

	protected override void DoDidSuccessEmulation()
	{
	}

	private void SetParameter_Birthday()
	{
		WriteActionParamValue("birthday", birthday);
	}
}
