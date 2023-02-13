using Message;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ServerCommitChaoWheelSpin
{
	public static IEnumerator Process(int count, GameObject callbackObject)
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
		NetServerCommitChaoWheelSpin net = new NetServerCommitChaoWheelSpin(count);
		net.Request();
		monitor.StartMonitor(new ServerCommitChaoWheelSpinRetry(count, callbackObject), -1f, HudNetworkConnect.DisplayType.ONLY_ICON);
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
			ServerCharacterState[] resultCharacterState = net.resultCharacterState;
			if (resultCharacterState != null)
			{
				playerState.SetCharacterState(resultCharacterState);
			}
			List<ServerChaoState> resultChaoState = net.resultChaoState;
			if (resultChaoState != null)
			{
				playerState.SetChaoState(resultChaoState);
			}
			ServerChaoWheelOptions chaoWheelOptions = ServerInterface.ChaoWheelOptions;
			ServerChaoWheelOptions resultChaoWheelOptions = net.resultChaoWheelOptions;
			resultChaoWheelOptions.CopyTo(chaoWheelOptions);
			ServerSpinResultGeneral resultSpinResultGeneral = null;
			if (net.resultSpinResultGeneral != null)
			{
				resultSpinResultGeneral = new ServerSpinResultGeneral();
				net.resultSpinResultGeneral.CopyTo(resultSpinResultGeneral);
			}
			MsgCommitChaoWheelSpicSucceed msg2 = new MsgCommitChaoWheelSpicSucceed
			{
				m_playerState = playerState,
				m_chaoWheelOptions = chaoWheelOptions,
				m_resultSpinResultGeneral = resultSpinResultGeneral
			};
			if (monitor != null)
			{
				monitor.EndMonitorForward(msg2, callbackObject, "ServerCommitChaoWheelSpin_Succeeded");
			}
			if (callbackObject != null)
			{
				callbackObject.SendMessage("ServerCommitChaoWheelSpin_Succeeded", msg2, SendMessageOptions.DontRequireReceiver);
			}
		}
		else
		{
			MsgServerConnctFailed msg = new MsgServerConnctFailed(net.resultStCd);
			if (monitor != null)
			{
				monitor.EndMonitorForward(msg, callbackObject, "ServerCommitChaoWheelSpin_Failed");
			}
		}
		if (monitor != null)
		{
			monitor.EndMonitorBackward();
		}
	}
}
