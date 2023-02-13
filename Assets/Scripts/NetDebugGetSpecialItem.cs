using LitJson;

public class NetDebugGetSpecialItem : NetBase
{
	public int paramItemId
	{
		get;
		set;
	}

	public int paramAddQuantity
	{
		get;
		set;
	}

	public NetDebugGetSpecialItem()
		: this(0, 0)
	{
	}

	public NetDebugGetSpecialItem(int itemId, int addQuantity)
	{
		paramItemId = itemId;
		paramAddQuantity = addQuantity;
	}

	protected override void DoRequest()
	{
		SetAction("Debug/getSpecialItem");
		SetParameter_Item();
	}

	protected override void DoDidSuccess(JsonData jdata)
	{
	}

	protected override void DoDidSuccessEmulation()
	{
	}

	private void SetParameter_Item()
	{
		WriteActionParamValue("addItemId", paramItemId);
		WriteActionParamValue("addNumItem", paramAddQuantity);
	}
}
