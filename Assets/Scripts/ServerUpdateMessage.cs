using Message;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ServerUpdateMessage
{
	private const string SuccessEvent = "ServerUpdateMessage_Succeeded";

	private const string FailEvent = "ServerUpdateMessage_Failed";

	public static IEnumerator Process(List<int> messageIdList, List<int> operatorMessageIdList, GameObject callbackObject)
	{
		NetMonitor monitor = NetMonitor.Instance;
		if (!(monitor != null))
		{
			yield break;
		}
		monitor.PrepareConnect();
		while (!monitor.IsEndPrepare())
		{
			yield return null;
		}
		if (!monitor.IsSuccessPrepare())
		{
			yield break;
		}
		NetServerUpdateMessage net = new NetServerUpdateMessage(messageIdList, operatorMessageIdList);
		net.Request();
		monitor.StartMonitor(new ServerUpdateMessageRetry(messageIdList, operatorMessageIdList, callbackObject));
		while (net.IsExecuting())
		{
			yield return null;
		}
		if (net.IsSucceeded())
		{
			ServerPlayerState playerState = ServerInterface.PlayerState;
			ServerPlayerState resultPlayerState = net.resultPlayerState;
			if (resultPlayerState != null)
			{
				resultPlayerState.CopyTo(playerState);
			}
			ServerCharacterState[] characterState = net.resultCharacterState;
			if (characterState != null)
			{
				playerState.SetCharacterState(characterState);
			}
			List<ServerChaoState> chaoState = net.resultChaoState;
			if (chaoState != null)
			{
				playerState.SetChaoState(chaoState);
			}
			int chaoRoulette = 0;
			int itemRoulette = 0;
			int presentCount = net.resultPresentStates;
			List<ServerPresentState> resultPresentList = new List<ServerPresentState>(presentCount);
			for (int k = 0; k < presentCount; k++)
			{
				ServerPresentState presentState = net.GetResultPresentState(k);
				resultPresentList.Add(presentState);
				if (presentState.m_itemId == 230000)
				{
					chaoRoulette += presentState.m_numItem;
				}
				else if (presentState.m_itemId == 240000)
				{
					itemRoulette += presentState.m_numItem;
				}
			}
			ServerInterface.ChaoWheelOptions.NumRouletteToken += chaoRoulette;
			ServerInterface.WheelOptions.m_numRemaining += itemRoulette;
			int missingMessageCount = net.resultMissingMessages;
			List<int> resultMissingMessageIdList = new List<int>(missingMessageCount);
			for (int j = 0; j < missingMessageCount; j++)
			{
				int missingId = net.GetResultMissingMessageId(j);
				resultMissingMessageIdList.Add(missingId);
			}
			int missingOperatorMessageCount = net.resultMissingOperatorMessages;
			List<int> resultMissingOperatorMessageIdList = new List<int>(missingOperatorMessageCount);
			for (int i = 0; i < missingOperatorMessageCount; i++)
			{
				int missingId2 = net.GetResultMissingOperatorMessageId(i);
				resultMissingOperatorMessageIdList.Add(missingId2);
			}
			if (messageIdList == null)
			{
				UpdateMessageList(ServerInterface.MessageList, resultMissingMessageIdList);
			}
			else
			{
				UpdateMessageList(ServerInterface.MessageList, messageIdList, resultMissingMessageIdList);
			}
			if (operatorMessageIdList == null)
			{
				UpdateOperatorMessageList(ServerInterface.OperatorMessageList, resultMissingOperatorMessageIdList);
			}
			else
			{
				UpdateOperatorMessageList(ServerInterface.OperatorMessageList, operatorMessageIdList, resultMissingOperatorMessageIdList);
			}
			MsgUpdateMesseageSucceed msg2 = new MsgUpdateMesseageSucceed
			{
				m_presentStateList = resultPresentList,
				m_notRecvMessageList = resultMissingMessageIdList,
				m_notRecvOperatorMessageList = resultMissingOperatorMessageIdList
			};
			GeneralUtil.SetPresentItemCount(msg2);
			if (monitor != null)
			{
				monitor.EndMonitorForward(msg2, callbackObject, "ServerUpdateMessage_Succeeded");
			}
			if (callbackObject != null)
			{
				callbackObject.SendMessage("ServerUpdateMessage_Succeeded", msg2, SendMessageOptions.DontRequireReceiver);
			}
		}
		else
		{
			MsgServerConnctFailed msg = new MsgServerConnctFailed(net.resultStCd);
			if (monitor != null)
			{
				monitor.EndMonitorForward(msg, callbackObject, "ServerUpdateMessage_Failed");
			}
		}
		if (monitor != null)
		{
			monitor.EndMonitorBackward();
		}
	}

	private static void UpdateMessageList(List<ServerMessageEntry> msgList, List<int> missingList)
	{
		if (msgList == null || missingList == null)
		{
			return;
		}
		List<ServerMessageEntry> list = new List<ServerMessageEntry>();
		foreach (int missing in missingList)
		{
			for (int i = 0; i < msgList.Count; i++)
			{
				if (missing == msgList[i].m_messageId)
				{
					ServerMessageEntry serverMessageEntry = new ServerMessageEntry();
					msgList[i].CopyTo(serverMessageEntry);
					list.Add(serverMessageEntry);
					break;
				}
			}
		}
		msgList.Clear();
		for (int j = 0; j < list.Count; j++)
		{
			msgList.Add(list[j]);
		}
	}

	private static void UpdateOperatorMessageList(List<ServerOperatorMessageEntry> msgList, List<int> missingList)
	{
		if (msgList == null || missingList == null)
		{
			return;
		}
		List<ServerOperatorMessageEntry> list = new List<ServerOperatorMessageEntry>();
		foreach (int missing in missingList)
		{
			for (int i = 0; i < msgList.Count; i++)
			{
				if (missing == msgList[i].m_messageId)
				{
					ServerOperatorMessageEntry serverOperatorMessageEntry = new ServerOperatorMessageEntry();
					msgList[i].CopyTo(serverOperatorMessageEntry);
					list.Add(serverOperatorMessageEntry);
					break;
				}
			}
		}
		msgList.Clear();
		for (int j = 0; j < list.Count; j++)
		{
			msgList.Add(list[j]);
		}
	}

	private static void UpdateMessageList(List<ServerMessageEntry> msgList, List<int> requestList, List<int> missingList)
	{
		if (msgList == null || requestList == null || missingList == null)
		{
			return;
		}
		foreach (int missing in missingList)
		{
			for (int i = 0; i < requestList.Count; i++)
			{
				if (missing == requestList[i])
				{
					requestList.Remove(requestList[i]);
					break;
				}
			}
		}
		foreach (int request in requestList)
		{
			for (int j = 0; j < msgList.Count; j++)
			{
				if (request == msgList[j].m_messageId)
				{
					msgList.Remove(msgList[j]);
					break;
				}
			}
		}
	}

	private static void UpdateOperatorMessageList(List<ServerOperatorMessageEntry> msgList, List<int> requestList, List<int> missingList)
	{
		if (msgList == null || requestList == null || missingList == null)
		{
			return;
		}
		foreach (int missing in missingList)
		{
			for (int i = 0; i < requestList.Count; i++)
			{
				if (missing == requestList[i])
				{
					requestList.Remove(requestList[i]);
					break;
				}
			}
		}
		foreach (int request in requestList)
		{
			for (int j = 0; j < msgList.Count; j++)
			{
				if (request == msgList[j].m_messageId)
				{
					msgList.Remove(msgList[j]);
					break;
				}
			}
		}
	}
}
