using Message;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ServerEventStartAct
{
	public static IEnumerator Process(int eventId, int energyExpend, long raidBossId, List<ItemType> modifiersItem, List<BoostItemType> modifiersBoostItem, GameObject callbackObject)
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
		float dummyTime = Time.realtimeSinceStartup;
		do
		{
			yield return null;
		}
		while (Time.realtimeSinceStartup - dummyTime < 1f);
		NetServerEventStartAct net = new NetServerEventStartAct(eventId, energyExpend, raidBossId, modifiersItem, modifiersBoostItem);
		net.Request();
		monitor.StartMonitor(new ServerEventStartActRetry(eventId, energyExpend, raidBossId, modifiersItem, modifiersBoostItem, callbackObject));
		while (net.IsExecuting())
		{
			yield return null;
		}
		if (net.IsSucceeded())
		{
			ServerPlayerState playerState = ServerInterface.PlayerState;
			ServerPlayerState resultPlayerState = net.resultPlayerState;
			resultPlayerState.CopyTo(playerState);
			if (EventManager.Instance != null)
			{
				ServerEventUserRaidBossState userRaidBossState = net.userRaidBossState;
				EventManager.Instance.SynchServerEventUserRaidBossState(userRaidBossState);
			}
			GeneralUtil.SetItemCount(ServerItem.Id.RAIDRING, net.userRaidBossState.NumRaidbossRings);
			MsgEventActStartSucceed msg2 = new MsgEventActStartSucceed
			{
				m_playerState = playerState
			};
			if (monitor != null)
			{
				monitor.EndMonitorForward(msg2, callbackObject, "ServerEventStartAct_Succeeded");
			}
			if (callbackObject != null)
			{
				callbackObject.SendMessage("ServerEventStartAct_Succeeded", msg2, SendMessageOptions.DontRequireReceiver);
			}
		}
		else
		{
			MsgServerConnctFailed msg = new MsgServerConnctFailed(net.resultStCd);
			if (monitor != null)
			{
				monitor.EndMonitorForward(msg, callbackObject, "ServerEventStartAct_Failed");
			}
		}
		if (monitor != null)
		{
			monitor.EndMonitorBackward();
		}
	}
}
