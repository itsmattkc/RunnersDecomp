using Message;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ServerCommitWheelSpin
{
	private const string SuccessEvent = "ServerCommitWheelSpin_Succeeded";

	private const string FailEvent = "ServerCommitWheelSpin_Failed";

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
		NetServerCommitWheelSpin net = new NetServerCommitWheelSpin(count);
		net.Request();
		monitor.StartMonitor(new ServerCommitWheelSpinRetry(count, callbackObject), -1f, HudNetworkConnect.DisplayType.ONLY_ICON);
		while (net.IsExecuting())
		{
			yield return null;
		}
		if (net.IsSucceeded())
		{
			ServerPlayerState playerState = ServerInterface.PlayerState;
			ServerPlayerState resultPlayerState = net.resultPlayerState;
			ServerSpinResultGeneral resultSpinResultGeneral = null;
			if (net.resultSpinResultGeneral != null)
			{
				resultSpinResultGeneral = new ServerSpinResultGeneral();
				net.resultSpinResultGeneral.CopyTo(resultSpinResultGeneral);
			}
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
			ServerWheelOptions wheelOptions = ServerInterface.WheelOptions;
			ServerWheelOptions resultWheelOptions = net.resultWheelOptions;
			resultWheelOptions.CopyTo(wheelOptions);
			MsgCommitWheelSpinSucceed msg2 = new MsgCommitWheelSpinSucceed
			{
				m_playerState = playerState,
				m_wheelOptions = wheelOptions,
				m_resultSpinResultGeneral = resultSpinResultGeneral
			};
			if (msg2.m_resultSpinResultGeneral == null && RouletteManager.Instance != null)
			{
				ServerWheelOptionsData currentWheelData = RouletteManager.Instance.GetRouletteDataOrg(RouletteCategory.ITEM);
				if (currentWheelData != null && currentWheelData.GetOrgRankupData() != null)
				{
					ServerSpinResultGeneral res = msg2.m_resultSpinResultGeneral = new ServerSpinResultGeneral(wheelOptions, currentWheelData.GetOrgRankupData());
				}
			}
			if (monitor != null)
			{
				monitor.EndMonitorForward(msg2, callbackObject, "ServerCommitWheelSpin_Succeeded");
			}
			if (callbackObject != null)
			{
				callbackObject.SendMessage("ServerCommitWheelSpin_Succeeded", msg2, SendMessageOptions.DontRequireReceiver);
			}
		}
		else
		{
			MsgServerConnctFailed msg = new MsgServerConnctFailed(net.resultStCd);
			if (net.resultStCd == ServerInterface.StatusCode.RouletteBoardReset)
			{
				if (callbackObject != null)
				{
					callbackObject.SendMessage("ServerCommitWheelSpin_Failed", msg, SendMessageOptions.DontRequireReceiver);
				}
			}
			else if (monitor != null)
			{
				monitor.EndMonitorForward(msg, callbackObject, "ServerCommitWheelSpin_Failed");
			}
		}
		if (monitor != null)
		{
			monitor.EndMonitorBackward();
		}
	}
}
