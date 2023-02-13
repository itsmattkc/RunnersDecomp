using LitJson;

public class NetServerAtomSerial : NetBase
{
	public string campaign
	{
		get;
		set;
	}

	public string serial
	{
		get;
		set;
	}

	public bool new_user
	{
		get;
		set;
	}

	public NetServerAtomSerial()
		: this(null, null, false)
	{
	}

	public NetServerAtomSerial(string campaign, string serial, bool new_user)
	{
		this.campaign = campaign;
		this.serial = serial;
		this.new_user = new_user;
	}

	protected override void DoRequest()
	{
		SetAction("Sgn/setSerialCode");
		CPlusPlusLink instance = CPlusPlusLink.Instance;
		if (instance != null)
		{
			string setSerialCodeString = instance.GetSetSerialCodeString(campaign, serial, new_user);
			Debug.Log("CPlusPlusLink.actRetry");
			WriteJsonString(setSerialCodeString);
		}
	}

	protected override void DoDidSuccess(JsonData jdata)
	{
	}

	protected override void DoDidSuccessEmulation()
	{
	}

	private void SetParameter_Data()
	{
		WriteActionParamValue("campaignId", campaign);
		WriteActionParamValue("serialCode", serial);
		ushort num = 0;
		if (new_user)
		{
			num = 1;
		}
		WriteActionParamValue("newUser", num.ToString());
	}
}
