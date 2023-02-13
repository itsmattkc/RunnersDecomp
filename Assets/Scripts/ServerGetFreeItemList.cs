using Message;
using System.Collections;
using UnityEngine;

public class ServerGetFreeItemList
{
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
		NetServerGetFreeItemList net = new NetServerGetFreeItemList();
		net.Request();
		monitor.StartMonitor(new ServerGetFreeItemListRetry(callbackObject));
		while (net.IsExecuting())
		{
			yield return null;
		}
		if (net.IsSucceeded())
		{
			ServerFreeItemState freeItemState = ServerInterface.FreeItemState;
			if (freeItemState != null)
			{
				ServerFreeItemState resultFreeItemState = net.resultFreeItemState;
				if (resultFreeItemState != null)
				{
					freeItemState.ClearList();
					resultFreeItemState.CopyTo(freeItemState);
				}
			}
			MsgGetFreeItemListSucceed msg2 = new MsgGetFreeItemListSucceed
			{
				m_freeItemState = freeItemState
			};
			if (monitor != null)
			{
				monitor.EndMonitorForward(msg2, callbackObject, "ServerGetFreeItemList_Succeeded");
			}
			if (callbackObject != null)
			{
				callbackObject.SendMessage("ServerGetFreeItemList_Succeeded", msg2, SendMessageOptions.DontRequireReceiver);
			}
		}
		else
		{
			MsgServerConnctFailed msg = new MsgServerConnctFailed(net.resultStCd);
			if (monitor != null)
			{
				monitor.EndMonitorForward(msg, callbackObject, "ServerGetFreeItemList_Failed");
			}
		}
		if (monitor != null)
		{
			monitor.EndMonitorBackward();
		}
	}
}
