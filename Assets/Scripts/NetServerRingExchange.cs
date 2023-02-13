using LitJson;

public class NetServerRingExchange : NetBase
{
	public int itemId;

	public int itemNum;

	public ServerPlayerState resultPlayerState
	{
		get;
		private set;
	}

	public NetServerRingExchange()
		: this(0, 0)
	{
	}

	public NetServerRingExchange(int itemId, int itemNum)
	{
		this.itemId = itemId;
		this.itemNum = itemNum;
	}

	protected override void DoRequest()
	{
		SetAction("Store/ringExchange");
		SetParameter_ItemData();
	}

	protected override void DoDidSuccess(JsonData jdata)
	{
		GetResponse_PlayerState(jdata);
	}

	protected override void DoDidSuccessEmulation()
	{
	}

	private void SetParameter_ItemData()
	{
		WriteActionParamValue("itemId", itemId);
		WriteActionParamValue("itemNum", itemNum);
	}

	private void GetResponse_PlayerState(JsonData jdata)
	{
		resultPlayerState = NetUtil.AnalyzePlayerStateJson(jdata, "playerState");
	}
}
