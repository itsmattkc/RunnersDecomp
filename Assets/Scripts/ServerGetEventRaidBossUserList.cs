using Message;
using System.Collections;
using UnityEngine;

public class ServerGetEventRaidBossUserList
{
	private const string SUCCEEDED_FUNCTION_NAME = "ServerGetEventRaidBossUserList_Succeeded";

	private const string FAILED_FUNCTION_NAME = "ServerGetEventRaidBossUserList_Failed";

	public static IEnumerator Process(int eventId, long raidBossId, GameObject callbackObject)
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
		NetServerGetEventRaidBossUserList net = new NetServerGetEventRaidBossUserList(eventId, raidBossId);
		net.Request();
		monitor.StartMonitor(new ServerGetEventRaidBossUserListRetry(eventId, raidBossId, callbackObject));
		while (net.IsExecuting())
		{
			yield return null;
		}
		if (net.IsSucceeded())
		{
			if (EventManager.Instance != null)
			{
				if (net.RaidBossState != null)
				{
					EventManager.Instance.SynchServerEventRaidBossList(net.RaidBossState);
				}
				EventManager.Instance.SynchServerEventRaidBossUserList(net.RaidBossUserStateList, raidBossId, net.RaidBossBonus);
			}
			MsgGetEventRaidBossUserListSucceed msg2 = new MsgGetEventRaidBossUserListSucceed
			{
				m_bonus = net.RaidBossBonus
			};
			if (monitor != null)
			{
				monitor.EndMonitorForward(msg2, callbackObject, "ServerGetEventRaidBossUserList_Succeeded");
			}
			if (callbackObject != null)
			{
				callbackObject.SendMessage("ServerGetEventRaidBossUserList_Succeeded", msg2, SendMessageOptions.DontRequireReceiver);
			}
		}
		else
		{
			MsgServerConnctFailed msg = new MsgServerConnctFailed(net.resultStCd);
			if (monitor != null)
			{
				monitor.EndMonitorForward(msg, callbackObject, "ServerGetEventRaidBossUserList_Failed");
			}
		}
		if (monitor != null)
		{
			monitor.EndMonitorBackward();
		}
	}
}
