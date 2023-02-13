using LitJson;

public class NetServerPreparePurchase : NetBase
{
	public int paramItemId
	{
		get;
		set;
	}

	public NetServerPreparePurchase()
		: this(0)
	{
	}

	public NetServerPreparePurchase(int itemId)
	{
		paramItemId = itemId;
	}

	protected override void DoRequest()
	{
		SetAction("Store/preparePurchase");
		SetParameter_ItemId();
	}

	protected override void DoDidSuccess(JsonData jdata)
	{
	}

	protected override void DoDidSuccessEmulation()
	{
	}

	private void SetParameter_ItemId()
	{
		WriteActionParamValue("itemId", paramItemId);
	}
}
