using LitJson;

public class NetDebugAddMessage : NetBase
{
	public string paramFromHspId
	{
		get;
		set;
	}

	public string paramToHspId
	{
		get;
		set;
	}

	public int paramMessageType
	{
		get;
		set;
	}

	public NetDebugAddMessage()
		: this(string.Empty, string.Empty, 0)
	{
	}

	public NetDebugAddMessage(string fromHspId, string toHspId, int messageType)
	{
		paramFromHspId = fromHspId;
		paramToHspId = toHspId;
		paramMessageType = messageType;
	}

	protected override void DoRequest()
	{
		SetAction("Debug/addMessage");
		SetParameter_Message();
	}

	protected override void DoDidSuccess(JsonData jdata)
	{
	}

	protected override void DoDidSuccessEmulation()
	{
	}

	private void SetParameter_Message()
	{
		WriteActionParamValue("hspFromId", paramFromHspId);
		WriteActionParamValue("hspToId", paramToHspId);
		WriteActionParamValue("messageKind", paramMessageType);
	}
}
