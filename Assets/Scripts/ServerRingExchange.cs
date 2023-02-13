using Message;
using System.Collections;
using UnityEngine;

public class ServerRingExchange
{
	public static IEnumerator Process(int itemId, int itemNum, GameObject callbackObject)
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
		NetServerRingExchange net = new NetServerRingExchange(itemId, itemNum);
		net.Request();
		monitor.StartMonitor(new ServerRingExchangeRetry(itemId, itemNum, callbackObject));
		while (net.IsExecuting())
		{
			yield return null;
		}
		if (net.IsSucceeded())
		{
			ServerPlayerState playerState = ServerInterface.PlayerState;
			ServerPlayerState resultPlayerState = net.resultPlayerState;
			resultPlayerState.CopyTo(playerState);
			MsgGetPlayerStateSucceed msg2 = new MsgGetPlayerStateSucceed
			{
				m_playerState = resultPlayerState
			};
			if (monitor != null)
			{
				monitor.EndMonitorForward(msg2, callbackObject, "ServerRingExchange_Succeeded");
			}
			if (callbackObject != null)
			{
				callbackObject.SendMessage("ServerRingExchange_Succeeded", msg2, SendMessageOptions.DontRequireReceiver);
			}
		}
		else
		{
			MsgServerConnctFailed msg = new MsgServerConnctFailed(net.resultStCd);
			if (monitor != null)
			{
				monitor.EndMonitorForward(msg, callbackObject, "ServerRingExchange_Failed");
			}
		}
		if (monitor != null)
		{
			monitor.EndMonitorBackward();
		}
	}
}
