using Message;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ServerEventUpdateGameResults
{
	public static IEnumerator Process(ServerEventGameResults results, GameObject callbackObject)
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
		NetServerEventUpdateGameResults net = new NetServerEventUpdateGameResults(results);
		net.Request();
		monitor.StartMonitor(new ServerEventUpdateGameResultsRetry(results, callbackObject));
		while (net.IsExecuting())
		{
			yield return null;
		}
		if (net.IsSucceeded())
		{
			ServerPlayerState playerState = ServerInterface.PlayerState;
			ServerPlayerState resultPlayerState = net.PlayerState;
			if (resultPlayerState != null)
			{
				resultPlayerState.CopyTo(playerState);
			}
			ServerPlayCharacterState[] playCharacterState = net.PlayerCharacterState;
			if (playCharacterState != null)
			{
				playerState.ClearPlayCharacterState();
				ServerPlayCharacterState[] array = playCharacterState;
				foreach (ServerPlayCharacterState playCharaState in array)
				{
					if (playCharaState != null)
					{
						playerState.SetPlayCharacterState(playCharaState);
					}
				}
			}
			ServerWheelOptions wheelOptions = ServerInterface.WheelOptions;
			ServerWheelOptions resultWheelOptions = net.WheelOptions;
			if (resultWheelOptions != null)
			{
				resultWheelOptions.CopyTo(wheelOptions);
			}
			List<ServerMessageEntry> messageEntries = net.MessageEntryList;
			if (messageEntries != null)
			{
				List<ServerMessageEntry> serverMessageEntries = ServerInterface.MessageList;
				serverMessageEntries.Clear();
				for (int index2 = 0; index2 < messageEntries.Count; index2++)
				{
					ServerMessageEntry messageEntry2 = messageEntries[index2];
					serverMessageEntries.Add(messageEntry2);
				}
			}
			List<ServerOperatorMessageEntry> operatorMessageEntries = net.OperatorMessageEntryList;
			if (operatorMessageEntries != null)
			{
				List<ServerOperatorMessageEntry> serverOperatorMessageEntries = ServerInterface.OperatorMessageList;
				serverOperatorMessageEntries.Clear();
				for (int index = 0; index < operatorMessageEntries.Count; index++)
				{
					ServerOperatorMessageEntry messageEntry = operatorMessageEntries[index];
					serverOperatorMessageEntries.Add(messageEntry);
				}
			}
			EventUtility.SetEventIncentiveListToEventManager(net.EventIncentiveList);
			EventUtility.SetEventStateToEventManager(net.EventState);
			EventManager eventManager = EventManager.Instance;
			if (eventManager != null)
			{
				eventManager.RaidBossBonus = net.RaidBossBonus;
			}
			MsgEventUpdateGameResultsSucceed msg2 = new MsgEventUpdateGameResultsSucceed
			{
				m_bonus = net.RaidBossBonus
			};
			if (monitor != null)
			{
				monitor.EndMonitorForward(msg2, callbackObject, "ServerUpdateGameResults_Succeeded");
			}
			if (callbackObject != null)
			{
				callbackObject.SendMessage("ServerUpdateGameResults_Succeeded", msg2, SendMessageOptions.DontRequireReceiver);
			}
		}
		else
		{
			MsgServerConnctFailed msg = new MsgServerConnctFailed(net.resultStCd);
			if (monitor != null)
			{
				monitor.EndMonitorForward(msg, callbackObject, "ServerPostGameResults_Failed");
			}
		}
		if (monitor != null)
		{
			monitor.EndMonitorBackward();
		}
	}
}
