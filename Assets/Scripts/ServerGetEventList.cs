using Message;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ServerGetEventList
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
		NetServerGetEventList net = new NetServerGetEventList();
		net.Request();
		monitor.StartMonitor(new ServerGetEventListRetry(callbackObject), -1f, HudNetworkConnect.DisplayType.ONLY_ICON);
		while (net.IsExecuting())
		{
			yield return null;
		}
		if (net.IsSucceeded())
		{
			List<ServerEventEntry> entries = ServerInterface.EventEntryList;
			entries.Clear();
			if (net.resultEventList != null)
			{
				int listCount = net.resultEventList.Count;
				for (int index = 0; index < listCount; index++)
				{
					entries.Add(net.resultEventList[index]);
				}
			}
			MsgGetEventListSucceed msg2 = new MsgGetEventListSucceed
			{
				m_eventList = net.resultEventList
			};
			if (monitor != null)
			{
				monitor.EndMonitorForward(msg2, callbackObject, "ServerGetEventList_Succeeded");
			}
			if (callbackObject != null)
			{
				callbackObject.SendMessage("ServerGetEventList_Succeeded", msg2, SendMessageOptions.DontRequireReceiver);
			}
			if (EventManager.Instance != null)
			{
				EventManager.Instance.SynchServerEntryList();
			}
		}
		else
		{
			MsgServerConnctFailed msg = new MsgServerConnctFailed(net.resultStCd);
			if (monitor != null)
			{
				monitor.EndMonitorForward(msg, callbackObject, "ServerGetEventList_Failed");
			}
		}
		if (monitor != null)
		{
			monitor.EndMonitorBackward();
		}
	}
}
