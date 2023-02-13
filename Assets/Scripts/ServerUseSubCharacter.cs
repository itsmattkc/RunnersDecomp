using Message;
using System.Collections;
using UnityEngine;

public class ServerUseSubCharacter
{
	public static IEnumerator Process(bool useFlag, GameObject callbackObject)
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
		NetServerUseSubCharacter net = new NetServerUseSubCharacter(useFlag);
		net.Request();
		monitor.StartMonitor(new ServerUseSubCharacterRetry(useFlag, callbackObject));
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
				monitor.EndMonitorForward(msg2, callbackObject, "ServerUseSubCharacter_Succeeded");
			}
			if (callbackObject != null)
			{
				callbackObject.SendMessage("ServerUseSubCharacter_Succeeded", msg2, SendMessageOptions.DontRequireReceiver);
			}
		}
		else
		{
			MsgServerConnctFailed msg = new MsgServerConnctFailed(net.resultStCd);
			if (monitor != null)
			{
				monitor.EndMonitorForward(msg, callbackObject, "ServerUseSubCharacter_Failed");
			}
		}
		if (monitor != null)
		{
			monitor.EndMonitorBackward();
		}
	}
}
