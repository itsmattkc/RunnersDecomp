using LitJson;
using System.Collections.Generic;

public class NetServerUpdateMessage : NetBase
{
	public const int INVALID_ID = int.MinValue;

	public List<int> paramMessageIdList
	{
		get;
		set;
	}

	public List<int> paramOperatorMessageIdList
	{
		get;
		set;
	}

	public ServerMessageEntry.MessageState paramMessageState
	{
		get;
		set;
	}

	public ServerPlayerState resultPlayerState
	{
		get;
		private set;
	}

	public ServerCharacterState[] resultCharacterState
	{
		get;
		private set;
	}

	public List<ServerChaoState> resultChaoState
	{
		get;
		private set;
	}

	public int resultPresentStates
	{
		get
		{
			return (resultPresentStateList != null) ? resultPresentStateList.Count : 0;
		}
	}

	public int resultMissingMessages
	{
		get
		{
			return (resultMissingMessageIdList != null) ? resultMissingMessageIdList.Count : 0;
		}
	}

	public int resultMissingOperatorMessages
	{
		get
		{
			return (resultMissingOperatorMessageIdList != null) ? resultMissingOperatorMessageIdList.Count : 0;
		}
	}

	private List<ServerPresentState> resultPresentStateList
	{
		get;
		set;
	}

	private List<int> resultMissingMessageIdList
	{
		get;
		set;
	}

	private List<int> resultMissingOperatorMessageIdList
	{
		get;
		set;
	}

	public NetServerUpdateMessage(List<int> messageIdList, List<int> operatorMessageIdList)
	{
		paramMessageIdList = messageIdList;
		paramOperatorMessageIdList = operatorMessageIdList;
	}

	protected override void DoRequest()
	{
		SetAction("Message/getMessage");
		CPlusPlusLink instance = CPlusPlusLink.Instance;
		if (instance != null)
		{
			string getMessageString = instance.GetGetMessageString(paramMessageIdList, paramOperatorMessageIdList);
			Debug.Log("CPlusPlusLink.actRetry");
			WriteJsonString(getMessageString);
		}
	}

	protected override void DoDidSuccess(JsonData jdata)
	{
		GetResponse_PlayerState(jdata);
		GetResponse_CharacterState(jdata);
		GetResponse_ChaoState(jdata);
		GetResponse_PresentStateList(jdata);
		GetResponse_MissingMessage(jdata);
		GetResponse_MissingOperatorMessage(jdata);
	}

	protected override void DoDidSuccessEmulation()
	{
	}

	private void SetParameter_MessageId()
	{
		if (paramMessageIdList == null)
		{
			if (ServerInterface.MessageList != null && ServerInterface.MessageList.Count > 0)
			{
				WriteActionParamValue("messageId", 0);
			}
		}
		else
		{
			List<object> list = new List<object>();
			if (list != null)
			{
				foreach (int paramMessageId in paramMessageIdList)
				{
					object item = paramMessageId;
					list.Add(item);
				}
				if (list.Count != 0)
				{
					WriteActionParamArray("messageId", list);
				}
			}
		}
		if (paramOperatorMessageIdList == null)
		{
			if (ServerInterface.OperatorMessageList != null && ServerInterface.OperatorMessageList.Count > 0)
			{
				WriteActionParamValue("operationMessageId", 0);
			}
			return;
		}
		List<object> list2 = new List<object>();
		if (list2 == null)
		{
			return;
		}
		foreach (int paramOperatorMessageId in paramOperatorMessageIdList)
		{
			object item2 = paramOperatorMessageId;
			list2.Add(item2);
		}
		if (list2.Count != 0)
		{
			WriteActionParamArray("operationMessageId", list2);
		}
	}

	public ServerPresentState GetResultPresentState(int index)
	{
		if (0 <= index && resultPresentStates > index)
		{
			return resultPresentStateList[index];
		}
		return null;
	}

	public int GetResultMissingMessageId(int index)
	{
		if (0 <= index && resultMissingMessages > index)
		{
			return resultMissingMessageIdList[index];
		}
		return int.MinValue;
	}

	public int GetResultMissingOperatorMessageId(int index)
	{
		if (0 <= index && resultMissingOperatorMessages > index)
		{
			return resultMissingOperatorMessageIdList[index];
		}
		return int.MinValue;
	}

	private void GetResponse_PlayerState(JsonData jdata)
	{
		resultPlayerState = NetUtil.AnalyzePlayerStateJson(jdata, "playerState");
	}

	private void GetResponse_CharacterState(JsonData jdata)
	{
		resultCharacterState = NetUtil.AnalyzePlayerState_CharactersStates(jdata);
	}

	private void GetResponse_ChaoState(JsonData jdata)
	{
		resultChaoState = NetUtil.AnalyzePlayerState_ChaoStates(jdata);
	}

	private void GetResponse_PresentStateList(JsonData jdata)
	{
		resultPresentStateList = new List<ServerPresentState>();
		JsonData jsonArray = NetUtil.GetJsonArray(jdata, "presentList");
		int count = jsonArray.Count;
		for (int i = 0; i < count; i++)
		{
			JsonData jdata2 = jsonArray[i];
			ServerPresentState item = NetUtil.AnalyzePresentStateJson(jdata2, string.Empty);
			resultPresentStateList.Add(item);
		}
	}

	private void GetResponse_MissingMessage(JsonData jdata)
	{
		resultMissingMessageIdList = new List<int>();
		JsonData jsonArray = NetUtil.GetJsonArray(jdata, "notRecvMessageList");
		int count = jsonArray.Count;
		for (int i = 0; i < count; i++)
		{
			int jsonInt = NetUtil.GetJsonInt(jsonArray[i]);
			resultMissingMessageIdList.Add(jsonInt);
		}
	}

	private void GetResponse_MissingOperatorMessage(JsonData jdata)
	{
		resultMissingOperatorMessageIdList = new List<int>();
		JsonData jsonArray = NetUtil.GetJsonArray(jdata, "notRecvOperatorMessageList");
		int count = jsonArray.Count;
		for (int i = 0; i < count; i++)
		{
			int jsonInt = NetUtil.GetJsonInt(jsonArray[i]);
			resultMissingOperatorMessageIdList.Add(jsonInt);
		}
	}
}
