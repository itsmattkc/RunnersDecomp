using LitJson;
using System.Collections.Generic;

public class NetServerGetMessageList : NetBase
{
	public int resultTotalMessages
	{
		get;
		private set;
	}

	public int resultTotalOperatorMessages
	{
		get;
		private set;
	}

	public int resultMessages
	{
		get
		{
			return (resultMessageEntriesList != null) ? resultMessageEntriesList.Count : 0;
		}
	}

	public int resultOperatorMessages
	{
		get
		{
			return (resultOperatorMessageEntriesList != null) ? resultOperatorMessageEntriesList.Count : 0;
		}
	}

	private List<ServerMessageEntry> resultMessageEntriesList
	{
		get;
		set;
	}

	private List<ServerOperatorMessageEntry> resultOperatorMessageEntriesList
	{
		get;
		set;
	}

	protected override void DoRequest()
	{
		SetAction("Message/getMessageList");
		CPlusPlusLink instance = CPlusPlusLink.Instance;
		if (instance != null)
		{
			string onlySendBaseParamString = instance.GetOnlySendBaseParamString();
			WriteJsonString(onlySendBaseParamString);
		}
	}

	protected override void DoDidSuccess(JsonData jdata)
	{
		GetResponse_MessageList(jdata);
		GetResponse_OperatorMessageList(jdata);
		GetResponse_TotalMessaga(jdata);
		GetResponse_TotalOperatorMessaga(jdata);
	}

	protected override void DoDidSuccessEmulation()
	{
		resultMessageEntriesList = new List<ServerMessageEntry>();
		resultOperatorMessageEntriesList = new List<ServerOperatorMessageEntry>();
		for (int i = 0; i < 2; i++)
		{
			ServerOperatorMessageEntry serverOperatorMessageEntry = new ServerOperatorMessageEntry();
			serverOperatorMessageEntry.m_messageId = i + 1;
			serverOperatorMessageEntry.m_content = "dummy content " + (i + 1);
			serverOperatorMessageEntry.m_presentState.m_itemId = 400000;
			serverOperatorMessageEntry.m_expireTiem = NetUtil.GetCurrentUnixTime() + 10000000;
			resultOperatorMessageEntriesList.Add(serverOperatorMessageEntry);
		}
		ServerMessageEntry serverMessageEntry = null;
		serverMessageEntry = new ServerMessageEntry();
		serverMessageEntry.m_messageId = 1;
		serverMessageEntry.m_name = "dummy_0001";
		serverMessageEntry.m_url = (serverMessageEntry.m_fromId = "0123456789abcdefg1");
		serverMessageEntry.m_presentState.m_itemId = 900000;
		serverMessageEntry.m_expireTiem = NetUtil.GetCurrentUnixTime() + 10000000;
		serverMessageEntry.m_messageState = ServerMessageEntry.MessageState.Unread;
		serverMessageEntry.m_messageType = ServerMessageEntry.MessageType.SendEnergy;
		resultMessageEntriesList.Add(serverMessageEntry);
		serverMessageEntry = new ServerMessageEntry();
		serverMessageEntry.m_messageId = 2;
		serverMessageEntry.m_name = "dummy_0002";
		serverMessageEntry.m_url = (serverMessageEntry.m_fromId = "0123456789abcdefg2");
		serverMessageEntry.m_presentState.m_itemId = 900000;
		serverMessageEntry.m_expireTiem = NetUtil.GetCurrentUnixTime() + 10000000;
		serverMessageEntry.m_messageState = ServerMessageEntry.MessageState.Unread;
		serverMessageEntry.m_messageType = ServerMessageEntry.MessageType.ReturnSendEnergy;
		resultMessageEntriesList.Add(serverMessageEntry);
		serverMessageEntry = new ServerMessageEntry();
		serverMessageEntry.m_messageId = 3;
		serverMessageEntry.m_name = "dummy_0003";
		serverMessageEntry.m_url = (serverMessageEntry.m_fromId = "0123456789abcdefg3");
		serverMessageEntry.m_presentState.m_itemId = 900000;
		serverMessageEntry.m_expireTiem = NetUtil.GetCurrentUnixTime() + 10000000;
		serverMessageEntry.m_messageState = ServerMessageEntry.MessageState.Unread;
		serverMessageEntry.m_messageType = ServerMessageEntry.MessageType.RequestEnergy;
		resultMessageEntriesList.Add(serverMessageEntry);
		serverMessageEntry = new ServerMessageEntry();
		serverMessageEntry.m_messageId = 4;
		serverMessageEntry.m_name = "dummy_0004";
		serverMessageEntry.m_url = (serverMessageEntry.m_fromId = "0123456789abcdefg4");
		serverMessageEntry.m_presentState.m_itemId = 900000;
		serverMessageEntry.m_expireTiem = NetUtil.GetCurrentUnixTime() + 10000000;
		serverMessageEntry.m_messageState = ServerMessageEntry.MessageState.Unread;
		serverMessageEntry.m_messageType = ServerMessageEntry.MessageType.ReturnRequestEnergy;
		resultMessageEntriesList.Add(serverMessageEntry);
		serverMessageEntry = new ServerMessageEntry();
		serverMessageEntry.m_messageId = 5;
		serverMessageEntry.m_name = "dummy_0005";
		serverMessageEntry.m_url = (serverMessageEntry.m_fromId = "0123456789abcdefg5");
		serverMessageEntry.m_presentState.m_itemId = 900000;
		serverMessageEntry.m_expireTiem = NetUtil.GetCurrentUnixTime() + 10000000;
		serverMessageEntry.m_messageState = ServerMessageEntry.MessageState.Unread;
		serverMessageEntry.m_messageType = ServerMessageEntry.MessageType.LentChao;
		resultMessageEntriesList.Add(serverMessageEntry);
	}

	public ServerMessageEntry GetResultMessageEntry(int index)
	{
		if (0 <= index && resultMessages > index)
		{
			return resultMessageEntriesList[index];
		}
		return null;
	}

	public ServerOperatorMessageEntry GetResultOperatorMessageEntry(int index)
	{
		if (0 <= index && resultOperatorMessages > index)
		{
			return resultOperatorMessageEntriesList[index];
		}
		return null;
	}

	private void GetResponse_MessageList(JsonData jdata)
	{
		resultMessageEntriesList = new List<ServerMessageEntry>();
		int count = NetUtil.GetJsonArray(jdata, "messageList").Count;
		for (int i = 0; i < count; i++)
		{
			JsonData jsonArrayObject = NetUtil.GetJsonArrayObject(jdata, "messageList", i);
			ServerMessageEntry item = NetUtil.AnalyzeMessageEntryJson(jsonArrayObject, string.Empty);
			resultMessageEntriesList.Add(item);
		}
	}

	private void GetResponse_TotalMessaga(JsonData jdata)
	{
		resultTotalMessages = NetUtil.GetJsonInt(jdata, "totalMessage");
	}

	private void GetResponse_OperatorMessageList(JsonData jdata)
	{
		resultOperatorMessageEntriesList = new List<ServerOperatorMessageEntry>();
		int count = NetUtil.GetJsonArray(jdata, "operatorMessageList").Count;
		for (int i = 0; i < count; i++)
		{
			JsonData jsonArrayObject = NetUtil.GetJsonArrayObject(jdata, "operatorMessageList", i);
			ServerOperatorMessageEntry item = NetUtil.AnalyzeOperatorMessageEntryJson(jsonArrayObject, string.Empty);
			resultOperatorMessageEntriesList.Add(item);
		}
	}

	private void GetResponse_TotalOperatorMessaga(JsonData jdata)
	{
		resultTotalOperatorMessages = NetUtil.GetJsonInt(jdata, "totalOperatorMessage");
	}
}
