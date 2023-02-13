using LitJson;

public class NetServerGetCountry : NetBase
{
	public int resultCountryId
	{
		get;
		set;
	}

	public string resultCountryCode
	{
		get;
		set;
	}

	protected override void DoRequest()
	{
		SetAction("Login/getCountry");
		CPlusPlusLink instance = CPlusPlusLink.Instance;
		if (instance != null)
		{
			string onlySendBaseParamString = instance.GetOnlySendBaseParamString();
			WriteJsonString(onlySendBaseParamString);
		}
	}

	protected override void DoDidSuccess(JsonData jdata)
	{
		GetResponse_Country(jdata);
	}

	protected override void DoDidSuccessEmulation()
	{
	}

	private void GetResponse_Country(JsonData jdata)
	{
		resultCountryId = NetUtil.GetJsonInt(jdata, "countryId");
		resultCountryCode = NetUtil.GetJsonString(jdata, "countryCode");
	}
}
