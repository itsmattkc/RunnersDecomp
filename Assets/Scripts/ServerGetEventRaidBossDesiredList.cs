using Message;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ServerGetEventRaidBossDesiredList
{
	public static IEnumerator Process(int eventId, long raidBossId, List<string> friendIdList, GameObject callbackObject)
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
		NetServerGetEventRaidBossDesiredList net = new NetServerGetEventRaidBossDesiredList(eventId, raidBossId, friendIdList);
		net.Request();
		monitor.StartMonitor(new ServerGetEventRaidBossDesiredListRetry(eventId, raidBossId, friendIdList, callbackObject));
		while (net.IsExecuting())
		{
			yield return null;
		}
		if (net.IsSucceeded())
		{
			MsgEventRaidBossDesiredListSucceed msg2 = new MsgEventRaidBossDesiredListSucceed
			{
				m_desiredList = net.DesiredList
			};
			if (monitor != null)
			{
				monitor.EndMonitorForward(msg2, callbackObject, "ServerGetEventRaidBossDesiredList_Succeeded");
			}
			if (callbackObject != null)
			{
				callbackObject.SendMessage("ServerGetEventRaidBossDesiredList_Succeeded", msg2, SendMessageOptions.DontRequireReceiver);
			}
		}
		else
		{
			MsgServerConnctFailed msg = new MsgServerConnctFailed(net.resultStCd);
			if (monitor != null)
			{
				monitor.EndMonitorForward(msg, callbackObject, "ServerGetEventRaidBossDesiredList_Failed");
			}
		}
		if (monitor != null)
		{
			monitor.EndMonitorBackward();
		}
	}
}
