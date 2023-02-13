using Message;
using System.Collections;
using UnityEngine;

public class ServerDrawRaidBoss
{
	public static IEnumerator Process(int eventId, long score, GameObject callbackObject)
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
		NetServerDrawRaidBoss net = new NetServerDrawRaidBoss(eventId, score);
		net.Request();
		monitor.StartMonitor(new ServerDrawRaidBossRetry(eventId, score, callbackObject));
		while (net.IsExecuting())
		{
			yield return null;
		}
		if (net.IsSucceeded())
		{
			if (EventManager.Instance != null)
			{
				EventManager.Instance.SynchServerEventRaidBossList(net.RaidBossState);
			}
			MsgDrawRaidBossSucceed msg2 = new MsgDrawRaidBossSucceed
			{
				m_raidBossState = net.RaidBossState
			};
			if (monitor != null)
			{
				monitor.EndMonitorForward(msg2, callbackObject, "ServerDrawRaidBoss_Succeeded");
			}
			if (callbackObject != null)
			{
				callbackObject.SendMessage("ServerDrawRaidBoss_Succeeded", msg2, SendMessageOptions.DontRequireReceiver);
			}
		}
		else
		{
			MsgServerConnctFailed msg = new MsgServerConnctFailed(net.resultStCd);
			if (monitor != null)
			{
				monitor.EndMonitorForward(msg, callbackObject, "ServerDrawRaidBoss_Failed");
			}
		}
		if (monitor != null)
		{
			monitor.EndMonitorBackward();
		}
	}
}
