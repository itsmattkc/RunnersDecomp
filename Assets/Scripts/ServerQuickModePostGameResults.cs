using Message;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ServerQuickModePostGameResults
{
	public static IEnumerator Process(ServerQuickModeGameResults results, GameObject callbackObject)
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
		NetServerQuickModePostGameResults net = new NetServerQuickModePostGameResults(results);
		net.Request();
		monitor.StartMonitor(new ServerQuickModePostGameResultsRetry(results, callbackObject));
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
			ServerCharacterState[] characterState = net.m_resultCharacterState;
			if (characterState != null)
			{
				playerState.SetCharacterState(characterState);
			}
			List<ServerChaoState> chaoState = net.m_resultChaoState;
			if (chaoState != null)
			{
				playerState.SetChaoState(chaoState);
			}
			ServerPlayCharacterState[] playCharacterState = net.m_resultPlayCharacterState;
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
			if (net.m_messageEntryList != null)
			{
				List<ServerMessageEntry> messageEntries = ServerInterface.MessageList;
				messageEntries.Clear();
				int resultMessageEntries = net.totalMessage;
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
				int resultOperatorMessageEntries = net.totalOperatorMessage;
				for (int index = 0; index < resultOperatorMessageEntries; index++)
				{
					ServerOperatorMessageEntry messageEntry = net.m_operatorMessageEntryList[index];
					operatorMessageEntries.Add(messageEntry);
				}
			}
			List<ServerItemState> dailyIncentiveList = net.m_dailyMissionIncentiveList;
			MsgQuickModePostGameResultsSucceed msg2 = new MsgQuickModePostGameResultsSucceed
			{
				m_playerState = playerState,
				m_dailyIncentive = dailyIncentiveList
			};
			if (monitor != null)
			{
				monitor.EndMonitorForward(msg2, callbackObject, "ServerQuickModePostGameResults_Succeeded");
			}
			if (callbackObject != null)
			{
				callbackObject.SendMessage("ServerQuickModePostGameResults_Succeeded", msg2, SendMessageOptions.DontRequireReceiver);
			}
		}
		else
		{
			MsgServerConnctFailed msg = new MsgServerConnctFailed(net.resultStCd);
			if (monitor != null)
			{
				monitor.EndMonitorForward(msg, callbackObject, "ServerQuickModePostGameResults_Failed");
			}
		}
		if (monitor != null)
		{
			monitor.EndMonitorBackward();
		}
	}
}
