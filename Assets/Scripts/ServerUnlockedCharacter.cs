using Message;
using System.Collections;
using UnityEngine;

public class ServerUnlockedCharacter
{
	public static IEnumerator Process(CharaType charaType, ServerItem serverItem, GameObject callbackObject)
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
		NetServerUnlockedCharacter net = new NetServerUnlockedCharacter(charaType, serverItem);
		net.Request();
		monitor.StartMonitor(new ServerUnlockedCharacterRetry(charaType, serverItem, callbackObject));
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
			ServerCharacterState[] characterState = net.resultCharacterState;
			if (characterState != null)
			{
				playerState.SetCharacterState(characterState);
			}
			MsgGetPlayerStateSucceed msg2 = new MsgGetPlayerStateSucceed
			{
				m_playerState = resultPlayerState
			};
			if (monitor != null)
			{
				monitor.EndMonitorForward(msg2, callbackObject, "ServerUnlockedCharacter_Succeeded");
			}
			if (callbackObject != null)
			{
				callbackObject.SendMessage("ServerUnlockedCharacter_Succeeded", msg2, SendMessageOptions.DontRequireReceiver);
			}
		}
		else
		{
			MsgServerConnctFailed msg = new MsgServerConnctFailed(net.resultStCd);
			if (monitor != null)
			{
				monitor.EndMonitorForward(msg, callbackObject, "ServerUnlockedCharacter_Failed");
			}
		}
		if (monitor != null)
		{
			monitor.EndMonitorBackward();
		}
	}
}
