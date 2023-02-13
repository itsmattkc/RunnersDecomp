using Message;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ServerQuickModeStartAct
{
	public static IEnumerator Process(List<ItemType> modifiersItem, List<BoostItemType> modifiersBoostItem, bool tutorial, GameObject callbackObject)
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
		NetServerQuickModeStartAct net = new NetServerQuickModeStartAct(modifiersItem, modifiersBoostItem, tutorial);
		net.Request();
		monitor.StartMonitor(new ServerQuickModeStartActRetry(modifiersItem, modifiersBoostItem, tutorial, callbackObject));
		while (net.IsExecuting())
		{
			yield return null;
		}
		if (net.IsSucceeded())
		{
			SaveDataUtil.SetBeforeDailyMissionSaveData(net.resultPlayerState);
			ServerPlayerState playerState = ServerInterface.PlayerState;
			ServerPlayerState resultPlayerState = net.resultPlayerState;
			resultPlayerState.CopyTo(playerState);
			MsgQuickModeActStartSucceed msg2 = new MsgQuickModeActStartSucceed
			{
				m_playerState = playerState
			};
			if (monitor != null)
			{
				monitor.EndMonitorForward(msg2, callbackObject, "ServerQuickModeStartAct_Succeeded");
			}
			if (callbackObject != null)
			{
				callbackObject.SendMessage("ServerQuickModeStartAct_Succeeded", msg2, SendMessageOptions.DontRequireReceiver);
			}
		}
		else
		{
			MsgServerConnctFailed msg = new MsgServerConnctFailed(net.resultStCd);
			if (monitor != null)
			{
				monitor.EndMonitorForward(msg, callbackObject, "ServerQuickModeStartAct_Failed");
			}
		}
		if (monitor != null)
		{
			monitor.EndMonitorBackward();
		}
	}
}
