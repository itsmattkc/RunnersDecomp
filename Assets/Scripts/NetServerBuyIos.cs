using LitJson;

public class NetServerBuyIos : NetBase
{
	public string receipt_data
	{
		get;
		set;
	}

	public ServerPlayerState resultPlayerState
	{
		get;
		private set;
	}

	public NetServerBuyIos()
		: this(string.Empty)
	{
	}

	public NetServerBuyIos(string receiptData)
	{
		receipt_data = receiptData;
	}

	protected override void DoRequest()
	{
		SetAction("Store/buyIos");
		CPlusPlusLink instance = CPlusPlusLink.Instance;
		if (instance != null)
		{
			string buyIosString = instance.GetBuyIosString(receipt_data);
			Debug.Log("CPlusPlusLink.actRetry");
			WriteJsonString(buyIosString);
		}
	}

	protected override void DoDidSuccess(JsonData jdata)
	{
		GetResponse_PlayerState(jdata);
	}

	protected override void DoDidSuccessEmulation()
	{
	}

	private void SetParameter_ReceiptData()
	{
		WriteActionParamValue("receipt_data", receipt_data);
	}

	private void GetResponse_PlayerState(JsonData jdata)
	{
		resultPlayerState = NetUtil.AnalyzePlayerStateJson(jdata, "playerState");
	}
}
