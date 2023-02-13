using Message;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ServerGetChaoState
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
		NetServerGetChaoState net = new NetServerGetChaoState();
		net.Request();
		monitor.StartMonitor(new ServerGetChaoStateRetry(callbackObject));
		while (net.IsExecuting())
		{
			yield return null;
		}
		if (net.IsSucceeded())
		{
			ServerPlayerState playerState = ServerInterface.PlayerState;
			List<ServerChaoState> resultChaoState = net.resultChaoState;
			playerState.SetChaoState(resultChaoState);
			MsgGetChaoStateSucceed msg2 = new MsgGetChaoStateSucceed
			{
				m_playerState = playerState
			};
			if (monitor != null)
			{
				monitor.EndMonitorForward(msg2, callbackObject, "ServerGetChaoState_Succeeded");
			}
			if (callbackObject != null)
			{
				callbackObject.SendMessage("ServerGetChaoState_Succeeded", msg2, SendMessageOptions.DontRequireReceiver);
			}
		}
		else
		{
			MsgServerConnctFailed msg = new MsgServerConnctFailed(net.resultStCd);
			if (monitor != null)
			{
				monitor.EndMonitorForward(msg, callbackObject, "ServerGetChaoState_Failed");
			}
		}
		if (monitor != null)
		{
			monitor.EndMonitorBackward();
		}
	}
}
