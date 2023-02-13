using Message;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ServerGetRingExchangeList
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
		NetServerGetRingExchangeList net = new NetServerGetRingExchangeList();
		net.Request();
		monitor.StartMonitor(new ServerGetRingExchangeListRetry(callbackObject), 0f, HudNetworkConnect.DisplayType.NO_BG);
		while (net.IsExecuting())
		{
			yield return null;
		}
		if (net.IsSucceeded())
		{
			List<ServerRingExchangeList> exchangeList = ServerInterface.RingExchangeList;
			List<ServerRingExchangeList> resultExchangeList = net.m_ringExchangeList;
			exchangeList.Clear();
			for (int index = 0; index < resultExchangeList.Count; index++)
			{
				ServerRingExchangeList e = new ServerRingExchangeList();
				resultExchangeList[index].CopyTo(e);
				exchangeList.Add(e);
			}
			MsgGetRingExchangeListSucceed msg2 = new MsgGetRingExchangeListSucceed
			{
				m_exchangeList = resultExchangeList
			};
			if (monitor != null)
			{
				monitor.EndMonitorForward(msg2, callbackObject, "ServerGetRingExchangeList_Succeeded");
			}
			if (callbackObject != null)
			{
				callbackObject.SendMessage("ServerGetRingExchangeList_Succeeded", msg2, SendMessageOptions.DontRequireReceiver);
			}
		}
		else
		{
			MsgServerConnctFailed msg = new MsgServerConnctFailed(net.resultStCd);
			if (monitor != null)
			{
				monitor.EndMonitorForward(msg, callbackObject, "ServerGetRingExchangeList_Failed");
			}
		}
		if (monitor != null)
		{
			monitor.EndMonitorBackward();
		}
	}
}
