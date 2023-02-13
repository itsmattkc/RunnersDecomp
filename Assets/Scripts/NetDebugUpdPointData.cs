using LitJson;

public class NetDebugUpdPointData : NetBase
{
	public int paramAddEnergyFree
	{
		get;
		set;
	}

	public int paramAddEnergyBuy
	{
		get;
		set;
	}

	public int paramAddRingFree
	{
		get;
		set;
	}

	public int paramAddRingBuy
	{
		get;
		set;
	}

	public int paramAddRedRingFree
	{
		get;
		set;
	}

	public int paramAddRedRingBuy
	{
		get;
		set;
	}

	public NetDebugUpdPointData()
		: this(0, 0, 0, 0, 0, 0)
	{
	}

	public NetDebugUpdPointData(int addEnergyFree, int addEnergyBuy, int addRingFree, int addRingBuy, int addRedStarRingFree, int addRedStarRingBuy)
	{
		paramAddEnergyFree = addEnergyFree;
		paramAddEnergyBuy = addEnergyBuy;
		paramAddRingFree = addRingFree;
		paramAddRingBuy = addRingBuy;
		paramAddRedRingFree = addRedStarRingFree;
		paramAddRedRingBuy = addRedStarRingBuy;
	}

	protected override void DoRequest()
	{
		SetAction("Debug/updPointData");
		SetParameter_AddEnergy();
		SetParameter_AddRing();
		SetParameter_AddRedStarRing();
	}

	protected override void DoDidSuccess(JsonData jdata)
	{
	}

	protected override void DoDidSuccessEmulation()
	{
	}

	private void SetParameter_AddEnergy()
	{
		WriteActionParamValue("addEnergyFree", paramAddEnergyFree);
		WriteActionParamValue("addEnergyBuy", paramAddEnergyBuy);
	}

	private void SetParameter_AddRing()
	{
		WriteActionParamValue("addRingFree", paramAddRingFree);
		WriteActionParamValue("addRingBuy", paramAddRingBuy);
	}

	private void SetParameter_AddRedStarRing()
	{
		WriteActionParamValue("addRedstarFree", paramAddRedRingFree);
		WriteActionParamValue("addRedstarBuy", paramAddRedRingBuy);
	}
}
