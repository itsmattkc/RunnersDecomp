using Message;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ServerStartAct
{
	public static IEnumerator Process(List<ItemType> modifiersItem, List<BoostItemType> modifiersBoostItem, List<string> distanceFriendIdList, bool tutorial, int? eventId, GameObject callbackObject)
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
		NetServerStartAct net = new NetServerStartAct(modifiersItem, modifiersBoostItem, distanceFriendIdList, tutorial, eventId);
		net.Request();
		monitor.StartMonitor(new ServerStartActRetry(modifiersItem, modifiersBoostItem, distanceFriendIdList, tutorial, eventId, callbackObject));
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
			List<ServerDistanceFriendEntry> distanceFriendList = ServerInterface.DistanceFriendEntry;
			List<ServerDistanceFriendEntry> resultDistanceFriendList = net.resultDistanceFriendEntry;
			distanceFriendList.Clear();
			foreach (ServerDistanceFriendEntry entry in resultDistanceFriendList)
			{
				distanceFriendList.Add(entry);
			}
			MsgActStartSucceed msg2 = new MsgActStartSucceed
			{
				m_playerState = playerState,
				m_friendDistanceList = distanceFriendList
			};
			if (monitor != null)
			{
				monitor.EndMonitorForward(msg2, callbackObject, "ServerStartAct_Succeeded");
			}
			if (callbackObject != null)
			{
				callbackObject.SendMessage("ServerStartAct_Succeeded", msg2, SendMessageOptions.DontRequireReceiver);
			}
		}
		else
		{
			MsgServerConnctFailed msg = new MsgServerConnctFailed(net.resultStCd);
			if (monitor != null)
			{
				monitor.EndMonitorForward(msg, callbackObject, "ServerStartAct_Failed");
			}
		}
		if (monitor != null)
		{
			monitor.EndMonitorBackward();
		}
	}
}
