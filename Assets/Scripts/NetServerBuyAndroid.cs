using LitJson;

public class NetServerBuyAndroid : NetBase
{
	public string receipt_data
	{
		get;
		set;
	}

	public string signature
	{
		get;
		set;
	}

	public ServerPlayerState resultPlayerState
	{
		get;
		private set;
	}

	public NetServerBuyAndroid()
		: this(string.Empty, string.Empty)
	{
	}

	public NetServerBuyAndroid(string receiptData, string signature)
	{
		receipt_data = receiptData;
		this.signature = signature;
	}

	protected override void DoRequest()
	{
		SetAction("Store/buyAndroid");
		CPlusPlusLink instance = CPlusPlusLink.Instance;
		if (instance != null)
		{
			string buyAndroidString = instance.GetBuyAndroidString(receipt_data, signature);
			Debug.Log("CPlusPlusLink.actRetry");
			WriteJsonString(buyAndroidString);
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
		WriteActionParamValue("receipt_signature", signature);
	}

	private void GetResponse_PlayerState(JsonData jdata)
	{
		resultPlayerState = NetUtil.AnalyzePlayerStateJson(jdata, "playerState");
	}
}
