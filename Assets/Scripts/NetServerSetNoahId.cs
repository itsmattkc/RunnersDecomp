using LitJson;

public class NetServerSetNoahId : NetBase
{
	public string noahId
	{
		get;
		set;
	}

	public NetServerSetNoahId()
		: this(string.Empty)
	{
	}

	public NetServerSetNoahId(string noahId)
	{
		this.noahId = noahId;
	}

	protected override void DoRequest()
	{
		SetAction("Sgn/setNoahId");
		CPlusPlusLink instance = CPlusPlusLink.Instance;
		if (instance != null)
		{
			string setNoahIdString = instance.GetSetNoahIdString(noahId);
			Debug.Log("CPlusPlusLink.actRetry");
			WriteJsonString(setNoahIdString);
		}
	}

	protected override void DoDidSuccess(JsonData jdata)
	{
	}

	protected override void DoDidSuccessEmulation()
	{
	}

	private void SetParameter_NoahId()
	{
		WriteActionParamValue("noahId", noahId);
	}
}
