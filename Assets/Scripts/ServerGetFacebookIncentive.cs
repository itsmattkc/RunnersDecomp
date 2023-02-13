using Message;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ServerGetFacebookIncentive
{
	public enum IncentiveType
	{
		LOGIN,
		REVIEW,
		FEED,
		ACHIEVEMENT,
		PUSH_NOLOGIN
	}

	public static IEnumerator Process(int incentiveType, int achievementCount, GameObject callbackObject)
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
		NetServerGetFacebookIncentive net = new NetServerGetFacebookIncentive(incentiveType, achievementCount);
		net.Request();
		monitor.StartMonitor(new ServerGetFacebookIncentiveRetry(incentiveType, achievementCount, callbackObject));
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
				monitor.EndMonitorForward(msg2, callbackObject, "ServerGetFacebookIncentive_Succeeded");
			}
			if (callbackObject != null)
			{
				callbackObject.SendMessage("ServerGetFacebookIncentive_Succeeded", msg2, SendMessageOptions.DontRequireReceiver);
			}
		}
		else
		{
			MsgServerConnctFailed msg = new MsgServerConnctFailed(net.resultStCd);
			if (monitor != null)
			{
				monitor.EndMonitorForward(msg, callbackObject, "ServerGetFacebookIncentive_Failed");
			}
		}
		if (monitor != null)
		{
			monitor.EndMonitorBackward();
		}
	}
}
