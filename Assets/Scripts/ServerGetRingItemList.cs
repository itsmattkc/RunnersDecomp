using Message;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ServerGetRingItemList
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
		NetServerGetRingItemList net = new NetServerGetRingItemList();
		net.Request();
		monitor.StartMonitor(new ServerGetRingItemListRetry(callbackObject));
		while (net.IsExecuting())
		{
			yield return null;
		}
		if (net.IsSucceeded())
		{
			int resultRingItemStates = net.resultRingItemStates;
			List<ServerRingItemState> ringItemList = new List<ServerRingItemState>(resultRingItemStates);
			for (int i = 0; i < resultRingItemStates; i++)
			{
				ServerRingItemState ringItemState = net.GetResultRingItemState(i);
				ringItemList.Add(ringItemState);
			}
			MsgGetRingItemStateSucceed msg2 = new MsgGetRingItemStateSucceed
			{
				m_RingStateList = ringItemList
			};
			if (monitor != null)
			{
				monitor.EndMonitorForward(msg2, callbackObject, "GetRingItemList_Succeeded");
			}
			if (callbackObject != null)
			{
				callbackObject.SendMessage("GetRingItemList_Succeeded", msg2, SendMessageOptions.DontRequireReceiver);
			}
		}
		else
		{
			MsgServerConnctFailed msg = new MsgServerConnctFailed(net.resultStCd);
			if (monitor != null)
			{
				monitor.EndMonitorForward(msg, callbackObject, "GetRingItemList_Failed");
			}
		}
		if (monitor != null)
		{
			monitor.EndMonitorBackward();
		}
	}
}
