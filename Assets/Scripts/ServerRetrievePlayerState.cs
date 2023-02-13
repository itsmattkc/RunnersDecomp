using Message;
using System.Collections;
using UnityEngine;

public class ServerRetrievePlayerState
{
	public static IEnumerator Process(GameObject callbackObject)
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
		NetServerRetrievePlayerState net = new NetServerRetrievePlayerState();
		net.Request();
		monitor.StartMonitor(new ServerRetrievePlayerStateRetry(callbackObject));
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
				m_playerState = playerState
			};
			if (monitor != null)
			{
				monitor.EndMonitorForward(msg2, callbackObject, "ServerRetrievePlayerState_Succeeded");
			}
			if (callbackObject != null)
			{
				callbackObject.SendMessage("ServerRetrievePlayerState_Succeeded", msg2, SendMessageOptions.DontRequireReceiver);
			}
		}
		else
		{
			MsgServerConnctFailed msg = new MsgServerConnctFailed(net.resultStCd);
			if (monitor != null)
			{
				monitor.EndMonitorForward(msg, callbackObject, "ServerRetrievePlayerState_Failed");
			}
		}
		if (monitor != null)
		{
			monitor.EndMonitorBackward();
		}
	}
}
