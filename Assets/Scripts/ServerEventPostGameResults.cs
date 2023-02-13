using Message;
using System.Collections;
using UnityEngine;

public class ServerEventPostGameResults
{
	public static IEnumerator Process(int eventId, int numRaidBossRings, GameObject callbackObject)
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
		NetServerEventPostGameResults net = new NetServerEventPostGameResults(eventId, numRaidBossRings);
		net.Request();
		monitor.StartMonitor(new ServerEventPostGameResultsRetry(eventId, numRaidBossRings, callbackObject));
		while (net.IsExecuting())
		{
			yield return null;
		}
		if (net.IsSucceeded())
		{
			if (EventManager.Instance != null)
			{
				ServerEventUserRaidBossState userRaidBossState = net.UserRaidBossState;
				EventManager.Instance.SynchServerEventUserRaidBossState(userRaidBossState);
			}
			GeneralUtil.SetItemCount(ServerItem.Id.RAIDRING, net.UserRaidBossState.NumRaidbossRings);
			MsgEventPostGameResultsSucceed msg2 = new MsgEventPostGameResultsSucceed();
			if (monitor != null)
			{
				monitor.EndMonitorForward(msg2, callbackObject, "ServerEventPostGameResults_Succeeded");
			}
			if (callbackObject != null)
			{
				callbackObject.SendMessage("ServerEventPostGameResults_Succeeded", msg2, SendMessageOptions.DontRequireReceiver);
			}
		}
		else
		{
			MsgServerConnctFailed msg = new MsgServerConnctFailed(net.resultStCd);
			if (monitor != null)
			{
				monitor.EndMonitorForward(msg, callbackObject, "ServerEventPostGameResults_Failed");
			}
		}
		if (monitor != null)
		{
			monitor.EndMonitorBackward();
		}
	}
}
