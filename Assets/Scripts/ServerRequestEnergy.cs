using Message;
using System.Collections;
using UnityEngine;

public class ServerRequestEnergy
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
		NetServerRequestEnergy net = new NetServerRequestEnergy(friendId);
		net.Request();
		monitor.StartMonitor(new ServerRequestEnergyRetry(friendId, callbackObject));
		while (net.IsExecuting())
		{
			yield return null;
		}
		if (net.IsSucceeded())
		{
			long resultExpireTime = net.resultExpireTime;
			ServerPlayerState playerState = ServerInterface.PlayerState;
			ServerPlayerState resultPlayerState = net.resultPlayerState;
			resultPlayerState.CopyTo(playerState);
			MsgRequestEnergySucceed msg2 = new MsgRequestEnergySucceed
			{
				m_playerState = playerState,
				m_resultExpireTime = resultExpireTime
			};
			if (monitor != null)
			{
				monitor.EndMonitorForward(msg2, callbackObject, "ServerRequestEnergy_Succeeded");
			}
			if (callbackObject != null)
			{
				callbackObject.SendMessage("ServerRequestEnergy_Succeeded", msg2, SendMessageOptions.DontRequireReceiver);
			}
		}
		else
		{
			MsgServerConnctFailed msg = new MsgServerConnctFailed(net.resultStCd);
			if (monitor != null)
			{
				monitor.EndMonitorForward(msg, callbackObject, "ServerRequestEnergy_Failed");
			}
		}
		if (monitor != null)
		{
			monitor.EndMonitorBackward();
		}
	}
}
