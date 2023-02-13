using Message;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ServerGetEventReward
{
	public static IEnumerator Process(int eventId, GameObject callbackObject)
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
		NetServerGetEventReward net = new NetServerGetEventReward(eventId);
		net.Request();
		monitor.StartMonitor(new ServerGetEventRewardRetry(eventId, callbackObject), -1f, HudNetworkConnect.DisplayType.ONLY_ICON);
		while (net.IsExecuting())
		{
			yield return null;
		}
		if (net.IsSucceeded())
		{
			List<ServerEventReward> rewards = ServerInterface.EventRewardList;
			rewards.Clear();
			if (net.resultEventRewardList != null)
			{
				int listCount = net.resultEventRewardList.Count;
				for (int index = 0; index < listCount; index++)
				{
					rewards.Add(net.resultEventRewardList[index]);
				}
			}
			MsgGetEventRewardSucceed msg2 = new MsgGetEventRewardSucceed
			{
				m_eventRewardList = net.resultEventRewardList
			};
			if (monitor != null)
			{
				monitor.EndMonitorForward(msg2, callbackObject, "ServerGetEventReward_Succeeded");
			}
			if (callbackObject != null)
			{
				callbackObject.SendMessage("ServerGetEventReward_Succeeded", msg2, SendMessageOptions.DontRequireReceiver);
			}
			if (EventManager.Instance != null)
			{
				EventManager.Instance.SynchServerRewardList();
			}
		}
		else
		{
			MsgServerConnctFailed msg = new MsgServerConnctFailed(net.resultStCd);
			if (monitor != null)
			{
				monitor.EndMonitorForward(msg, callbackObject, "ServerGetEventReward_Failed");
			}
		}
		if (monitor != null)
		{
			monitor.EndMonitorBackward();
		}
	}
}
