using Message;
using System.Collections;
using UnityEngine;

public class ServerUpgradeCharacter
{
	public static IEnumerator Process(int characterId, int abilityId, GameObject callbackObject)
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
		NetServerUpgradeCharacter net = new NetServerUpgradeCharacter(characterId, abilityId);
		net.Request();
		monitor.StartMonitor(new ServerUpgradeCharacterRetry(characterId, abilityId, callbackObject));
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
			ServerCharacterState[] resultCharacterState = net.resultCharacterState;
			if (resultCharacterState != null)
			{
				playerState.SetCharacterState(resultCharacterState);
			}
			MsgGetPlayerStateSucceed msg2 = new MsgGetPlayerStateSucceed
			{
				m_playerState = playerState
			};
			if (monitor != null)
			{
				monitor.EndMonitorForward(msg2, callbackObject, "ServerUpgradeCharacter_Succeeded");
			}
			if (callbackObject != null)
			{
				callbackObject.SendMessage("ServerUpgradeCharacter_Succeeded", msg2, SendMessageOptions.DontRequireReceiver);
			}
		}
		else
		{
			MsgServerConnctFailed msg = new MsgServerConnctFailed(net.resultStCd)
			{
				m_status = net.resultStCd
			};
			if (monitor != null)
			{
				monitor.EndMonitorForward(msg, callbackObject, "ServerUpgradeCharacter_Failed");
			}
		}
		if (monitor != null)
		{
			monitor.EndMonitorBackward();
		}
	}
}
