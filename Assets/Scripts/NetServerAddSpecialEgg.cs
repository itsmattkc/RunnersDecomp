using LitJson;

public class NetServerAddSpecialEgg : NetBase
{
	public int numSpecialEgg
	{
		get;
		set;
	}

	public int resultSpecialEgg
	{
		get;
		set;
	}

	public NetServerAddSpecialEgg()
		: this(0)
	{
	}

	public NetServerAddSpecialEgg(int numSpecialEgg)
	{
		this.numSpecialEgg = numSpecialEgg;
		resultSpecialEgg = 0;
	}

	protected override void DoRequest()
	{
		SetAction("Chao/addSpecialEgg");
		CPlusPlusLink instance = CPlusPlusLink.Instance;
		if (instance != null)
		{
			string addSpecialEggString = instance.GetAddSpecialEggString(numSpecialEgg);
			Debug.Log("CPlusPlusLink.actRetry");
			WriteJsonString(addSpecialEggString);
		}
	}

	protected override void DoDidSuccess(JsonData jdata)
	{
		GetResponse_Data(jdata);
	}

	protected override void DoDidSuccessEmulation()
	{
	}

	private void SetParameter_Data()
	{
		WriteActionParamValue("numSpecialEgg", numSpecialEgg);
	}

	private void GetResponse_Data(JsonData jdata)
	{
		resultSpecialEgg = NetUtil.GetJsonInt(jdata, "numSpecialEgg");
	}
}
