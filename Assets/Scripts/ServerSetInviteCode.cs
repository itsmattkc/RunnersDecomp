using Message;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ServerSetInviteCode
{
	public static IEnumerator Process(string friendId, GameObject callbackObject)
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
		NetServerSetInviteCode net = new NetServerSetInviteCode(friendId);
		net.Request();
		monitor.StartMonitor(new ServerSetInviteCodeRetry(friendId, callbackObject));
		while (net.IsExecuting())
		{
			yield return null;
		}
		if (net.IsSucceeded())
		{
			List<ServerPresentState> incentive = net.incentive;
			ServerPlayerState playerState = ServerInterface.PlayerState;
			ServerPlayerState resultPlayerState = net.playerState;
			resultPlayerState.CopyTo(playerState);
			MsgGetNormalIncentiveSucceed msg2 = new MsgGetNormalIncentiveSucceed
			{
				m_playerState = playerState,
				m_incentive = incentive
			};
			if (monitor != null)
			{
				monitor.EndMonitorForward(msg2, callbackObject, "ServerSetInviteCode_Succeeded");
			}
			if (callbackObject != null)
			{
				callbackObject.SendMessage("ServerSetInviteCode_Succeeded", msg2, SendMessageOptions.DontRequireReceiver);
			}
		}
		else
		{
			MsgServerConnctFailed msg = new MsgServerConnctFailed(net.resultStCd);
			if (monitor != null)
			{
				monitor.EndMonitorForward(msg, callbackObject, "ServerSetInviteCode_Failed");
			}
		}
		if (monitor != null)
		{
			monitor.EndMonitorBackward();
		}
	}
}
