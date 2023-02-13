using LitJson;

public class NetDebugUpdUserData : NetBase
{
	public int paramAddRank
	{
		get;
		set;
	}

	public NetDebugUpdUserData()
		: this(0)
	{
	}

	public NetDebugUpdUserData(int addRank)
	{
		paramAddRank = addRank;
	}

	protected override void DoRequest()
	{
		SetAction("Debug/updUserData");
		SetParameter_AddRank();
	}

	protected override void DoDidSuccess(JsonData jdata)
	{
	}

	protected override void DoDidSuccessEmulation()
	{
	}

	private void SetParameter_AddRank()
	{
		WriteActionParamValue("addRank", paramAddRank);
	}
}
