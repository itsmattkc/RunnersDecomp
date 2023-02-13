using Message;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ServerGetFirstLaunchChao
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
		NetServerGetFirstLaunchChao net = new NetServerGetFirstLaunchChao();
		net.Request();
		monitor.StartMonitor(new ServerGetFirstLaunchChaoRetry(callbackObject));
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
			List<ServerChaoState> resultChaoState = net.resultChaoState;
			if (resultChaoState != null && resultChaoState.Count > 0)
			{
				playerState.SetChaoState(resultChaoState);
			}
			MsgGetPlayerStateSucceed msg2 = new MsgGetPlayerStateSucceed
			{
				m_playerState = playerState
			};
			if (monitor != null)
			{
				monitor.EndMonitorForward(msg2, callbackObject, "ServerGetFirstLaunchChao_Succeeded");
			}
			if (callbackObject != null)
			{
				callbackObject.SendMessage("ServerGetFirstLaunchChao_Succeeded", msg2, SendMessageOptions.DontRequireReceiver);
			}
		}
		else
		{
			MsgServerConnctFailed msg = new MsgServerConnctFailed(net.resultStCd);
			if (monitor != null)
			{
				monitor.EndMonitorForward(msg, callbackObject, "ServerGetFirstLaunchChao_Failed");
			}
		}
		if (monitor != null)
		{
			monitor.EndMonitorBackward();
		}
	}
}
