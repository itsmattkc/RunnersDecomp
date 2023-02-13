using Message;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ServerCommitWheelSpinGeneral
{
	private const string SuccessEvent = "ServerCommitWheelSpinGeneral_Succeeded";

	private const string FailEvent = "ServerCommitWheelSpinGeneral_Failed";

	public static IEnumerator Process(int eventId, int spinId, int spinCostItemId, int spinNum, GameObject callbackObject)
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
		NetServerCommitWheelSpinGeneral net = new NetServerCommitWheelSpinGeneral(eventId, spinId, spinCostItemId, spinNum);
		net.Request();
		monitor.StartMonitor(new ServerCommitWheelSpinGeneralRetry(eventId, spinId, spinCostItemId, spinNum, callbackObject), -1f, HudNetworkConnect.DisplayType.ONLY_ICON);
		while (net.IsExecuting())
		{
			yield return null;
		}
		if (net.IsSucceeded())
		{
			ServerPlayerState playerState = ServerInterface.PlayerState;
			ServerPlayerState resultPlayerState = net.resultPlayerState;
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
			MsgCommitWheelSpinGeneralSucceed msg2 = new MsgCommitWheelSpinGeneralSucceed
			{
				m_playerState = playerState,
				m_wheelOptionsGeneral = net.resultWheelOptionsGen,
				m_resultSpinResultGeneral = net.resultWheelResultGen
			};
			if (monitor != null)
			{
				monitor.EndMonitorForward(msg2, callbackObject, "ServerCommitWheelSpinGeneral_Succeeded");
			}
			if (callbackObject != null)
			{
				callbackObject.SendMessage("ServerCommitWheelSpinGeneral_Succeeded", msg2, SendMessageOptions.DontRequireReceiver);
			}
		}
		else
		{
			MsgServerConnctFailed msg = new MsgServerConnctFailed(net.resultStCd);
			if (net.resultStCd == ServerInterface.StatusCode.RouletteBoardReset)
			{
				if (callbackObject != null)
				{
					callbackObject.SendMessage("ServerCommitWheelSpinGeneral_Failed", msg, SendMessageOptions.DontRequireReceiver);
				}
			}
			else if (monitor != null)
			{
				monitor.EndMonitorForward(msg, callbackObject, "ServerCommitWheelSpinGeneral_Failed");
			}
		}
		if (monitor != null)
		{
			monitor.EndMonitorBackward();
		}
	}
}
