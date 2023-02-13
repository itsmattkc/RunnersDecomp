using Message;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ServerPostGameResults
{
	public static IEnumerator Process(ServerGameResults results, GameObject callbackObject)
	{
		NetMonitor monitor = NetMonitor.Instance;
		if (!(monitor != null))
		{
			yield break;
		}
		monitor.PrepareConnect(ServerSessionWatcher.ValidateType.LOGIN_OR_RELOGIN);
		while (!monitor.IsEndPrepare())
		{
			yield return null;
		}
		if (!monitor.IsSuccessPrepare())
		{
			yield break;
		}
		NetServerPostGameResults net = new NetServerPostGameResults(results);
		net.Request();
		monitor.StartMonitor(new ServerPostGameResultsRetry(results, callbackObject));
		while (net.IsExecuting())
		{
			yield return null;
		}
		if (net.IsSucceeded())
		{
			ServerPlayerState playerState = ServerInterface.PlayerState;
			ServerPlayerState resultPlayerState = net.m_resultPlayerState;
			if (resultPlayerState != null)
			{
				resultPlayerState.CopyTo(playerState);
			}
			ServerCharacterState[] characterState = net.resultCharacterState;
			if (characterState != null)
			{
				playerState.SetCharacterState(characterState);
			}
			List<ServerChaoState> chaoState = net.resultChaoState;
			if (chaoState != null)
			{
				playerState.SetChaoState(chaoState);
			}
			ServerPlayCharacterState[] playCharacterState = net.resultPlayCharacterState;
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
			ServerMileageMapState interfaceMileageMapState = ServerInterface.MileageMapState;
			net.m_resultMileageMapState.CopyTo(interfaceMileageMapState);
			if (results.m_chaoEggPresent && RouletteManager.Instance != null)
			{
				RouletteManager.Instance.specialEgg++;
			}
			if (net.m_messageEntryList != null)
			{
				List<ServerMessageEntry> messageEntries = ServerInterface.MessageList;
				messageEntries.Clear();
				int resultMessageEntries = net.m_totalMessage;
				for (int index2 = 0; index2 < resultMessageEntries; index2++)
				{
					ServerMessageEntry messageEntry2 = net.m_messageEntryList[index2];
					messageEntries.Add(messageEntry2);
				}
			}
			if (net.m_operatorMessageEntryList != null)
			{
				List<ServerOperatorMessageEntry> operatorMessageEntries = ServerInterface.OperatorMessageList;
				operatorMessageEntries.Clear();
				int resultOperatorMessageEntries = net.m_totalOperatorMessage;
				for (int index = 0; index < resultOperatorMessageEntries; index++)
				{
					ServerOperatorMessageEntry messageEntry = net.m_operatorMessageEntryList[index];
					operatorMessageEntries.Add(messageEntry);
				}
			}
			if (net.m_resultEventIncentiveList != null)
			{
				EventUtility.SetEventIncentiveListToEventManager(net.m_resultEventIncentiveList);
			}
			if (net.m_resultEventState != null)
			{
				EventUtility.SetEventStateToEventManager(net.m_resultEventState);
			}
			List<ServerMileageIncentive> mileageIncentiveList = net.m_resultMileageIncentive;
			List<ServerItemState> dailyIncentiveList = net.m_resultDailyMissionIncentiveList;
			MsgPostGameResultsSucceed msg2 = new MsgPostGameResultsSucceed
			{
				m_playerState = playerState,
				m_mileageMapState = interfaceMileageMapState,
				m_mileageIncentive = mileageIncentiveList,
				m_dailyIncentive = dailyIncentiveList
			};
			if (monitor != null)
			{
				monitor.EndMonitorForward(msg2, callbackObject, "ServerPostGameResults_Succeeded");
			}
			if (callbackObject != null)
			{
				callbackObject.SendMessage("ServerPostGameResults_Succeeded", msg2, SendMessageOptions.DontRequireReceiver);
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
