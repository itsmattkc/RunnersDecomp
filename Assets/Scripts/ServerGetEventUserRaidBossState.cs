using Message;
using System.Collections;
using UnityEngine;

public class ServerGetEventUserRaidBossState
{
	private const string SUCCEEDED_FUNCTION_NAME = "ServerGetEventUserRaidBossState_Succeeded";

	private const string FAILED_FUNCTION_NAME = "ServerGetEventUserRaidBossState_Failed";

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
		NetServerGetEventUserRaidBossState net = new NetServerGetEventUserRaidBossState(eventId);
		net.Request();
		monitor.StartMonitor(new ServerGetEventUserRaidBossStateRetry(eventId, callbackObject), -1f, HudNetworkConnect.DisplayType.ONLY_ICON);
		while (net.IsExecuting())
		{
			yield return null;
		}
		if (net.IsSucceeded())
		{
			MsgGetEventUserRaidBossStateSucceed msg2 = new MsgGetEventUserRaidBossStateSucceed();
			if (EventManager.Instance != null)
			{
				EventManager.Instance.SynchServerEventUserRaidBossState(net.UserRaidBossState);
			}
			GeneralUtil.SetItemCount(ServerItem.Id.RAIDRING, net.UserRaidBossState.NumRaidbossRings);
			if (monitor != null)
			{
				monitor.EndMonitorForward(msg2, callbackObject, "ServerGetEventUserRaidBossState_Succeeded");
			}
			if (callbackObject != null)
			{
				callbackObject.SendMessage("ServerGetEventUserRaidBossState_Succeeded", msg2, SendMessageOptions.DontRequireReceiver);
			}
		}
		else
		{
			MsgServerConnctFailed msg = new MsgServerConnctFailed(net.resultStCd);
			if (monitor != null)
			{
				monitor.EndMonitorForward(msg, callbackObject, "ServerGetEventUserRaidBossState_Failed");
			}
		}
		if (monitor != null)
		{
			monitor.EndMonitorBackward();
		}
	}
}
