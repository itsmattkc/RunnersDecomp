using Message;
using System.Collections;
using UnityEngine;

public class ServerGetEventUserRaidBossList
{
	private const string SUCCEEDED_FUNCTION_NAME = "ServerGetEventUserRaidBossList_Succeeded";

	private const string FAILED_FUNCTION_NAME = "ServerGetEventUserRaidBossList_Failed";

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
		NetServerGetEventUserRaidBossList net = new NetServerGetEventUserRaidBossList(eventId);
		net.Request();
		monitor.StartMonitor(new ServerGetEventUserRaidBossListRetry(eventId, callbackObject));
		while (net.IsExecuting())
		{
			yield return null;
		}
		if (net.IsSucceeded())
		{
			MsgGetEventUserRaidBossListSucceed msg2 = new MsgGetEventUserRaidBossListSucceed();
			if (EventManager.Instance != null)
			{
				EventManager.Instance.SynchServerEventRaidBossList(net.UserRaidBossList);
				if (net.UserRaidBossState != null)
				{
					EventManager.Instance.SynchServerEventUserRaidBossState(net.UserRaidBossState);
					GeneralUtil.SetItemCount(ServerItem.Id.RAIDRING, net.UserRaidBossState.NumRaidbossRings);
				}
			}
			if (monitor != null)
			{
				monitor.EndMonitorForward(msg2, callbackObject, "ServerGetEventUserRaidBossList_Succeeded");
			}
			if (callbackObject != null)
			{
				callbackObject.SendMessage("ServerGetEventUserRaidBossList_Succeeded", msg2, SendMessageOptions.DontRequireReceiver);
			}
		}
		else
		{
			MsgServerConnctFailed msg = new MsgServerConnctFailed(net.resultStCd);
			if (monitor != null)
			{
				monitor.EndMonitorForward(msg, callbackObject, "ServerGetEventUserRaidBossList_Failed");
			}
		}
		if (monitor != null)
		{
			monitor.EndMonitorBackward();
		}
	}
}
