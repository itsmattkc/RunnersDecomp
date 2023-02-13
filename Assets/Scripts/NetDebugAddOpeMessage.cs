using LitJson;

public class NetDebugAddOpeMessage : NetBase
{
	public class OpeMsgInfo
	{
		public string userID;

		public int messageKind;

		public int infoId;

		public int itemId;

		public int numItem;

		public int additionalInfo1;

		public int additionalInfo2;

		public string msgTitle;

		public string msgContent;

		public string msgImageId;
	}

	private OpeMsgInfo paramOpeMsgInfo;

	public NetDebugAddOpeMessage(OpeMsgInfo info)
	{
		paramOpeMsgInfo = info;
	}

	protected override void DoRequest()
	{
		SetAction("Debug/addOpeMessage");
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
		if (paramOpeMsgInfo != null)
		{
			WriteActionParamValue("messageKind", paramOpeMsgInfo.messageKind);
			WriteActionParamValue("infoid", paramOpeMsgInfo.infoId);
			WriteActionParamValue("item_id", paramOpeMsgInfo.itemId);
			WriteActionParamValue("num_item", paramOpeMsgInfo.numItem);
			WriteActionParamValue("additional_info_1", paramOpeMsgInfo.additionalInfo1);
			WriteActionParamValue("additional_info_2", paramOpeMsgInfo.additionalInfo2);
			WriteActionParamValue("msg_title", paramOpeMsgInfo.msgTitle);
			WriteActionParamValue("msg_content", paramOpeMsgInfo.msgContent);
			WriteActionParamValue("msg_image_id", paramOpeMsgInfo.msgImageId);
			WriteActionParamValue("hspToId", paramOpeMsgInfo.userID);
		}
	}
}
