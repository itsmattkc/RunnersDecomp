using Message;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ServerGetMessageList
{
	private const string SuccessEvent = "ServerGetMessageList_Succeeded";

	private const string FailEvent = "ServerGetMessageList_Failed";

	public static IEnumerator Process(GameObject callbackObject)
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
		NetServerGetMessageList net = new NetServerGetMessageList();
		net.Request();
		monitor.StartMonitor(new ServerGetMessageListRetry(callbackObject));
		while (net.IsExecuting())
		{
			yield return null;
		}
		if (net.IsSucceeded())
		{
			List<ServerMessageEntry> messageEntries = ServerInterface.MessageList;
			messageEntries.Clear();
			List<ServerOperatorMessageEntry> operatorMessageEntries = ServerInterface.OperatorMessageList;
			operatorMessageEntries.Clear();
			int resultMessageEntries = net.resultMessages;
			for (int index2 = 0; index2 < resultMessageEntries; index2++)
			{
				ServerMessageEntry messageEntry = net.GetResultMessageEntry(index2);
				messageEntries.Add(messageEntry);
			}
			int resultOperatorMessageEntries = net.resultOperatorMessages;
			for (int index = 0; index < resultOperatorMessageEntries; index++)
			{
				ServerOperatorMessageEntry messageEntry2 = net.GetResultOperatorMessageEntry(index);
				operatorMessageEntries.Add(messageEntry2);
			}
			MsgGetMessageListSucceed msg2 = new MsgGetMessageListSucceed();
			if (monitor != null)
			{
				monitor.EndMonitorForward(msg2, callbackObject, "ServerGetMessageList_Succeeded");
			}
			if (callbackObject != null)
			{
				callbackObject.SendMessage("ServerGetMessageList_Succeeded", msg2, SendMessageOptions.DontRequireReceiver);
			}
		}
		else
		{
			MsgServerConnctFailed msg = new MsgServerConnctFailed(net.resultStCd);
			if (monitor != null)
			{
				monitor.EndMonitorForward(msg, callbackObject, "ServerGetMessageList_Failed");
			}
		}
		if (monitor != null)
		{
			monitor.EndMonitorBackward();
		}
	}
}
