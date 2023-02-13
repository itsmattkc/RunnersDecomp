using Message;
using System.Collections;
using UnityEngine;

public class ServerChangeCharacter
{
	public static IEnumerator Process(int mainCharaId, int subCharaId, GameObject callbackObject)
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
		NetServerChangeCharacter net = new NetServerChangeCharacter(mainCharaId, subCharaId);
		net.Request();
		monitor.StartMonitor(new ServerChangeCharacterRetry(mainCharaId, subCharaId, callbackObject));
		while (net.IsExecuting())
		{
			yield return null;
		}
		if (net.IsSucceeded())
		{
			ServerPlayerState playerState = ServerInterface.PlayerState;
			ServerPlayerState resultPlayerState = net.resultPlayerState;
			resultPlayerState.CopyTo(playerState);
			DeckUtil.CharaSetSaveAuto(playerState.m_mainCharaId, playerState.m_subCharaId);
			MsgGetPlayerStateSucceed msg2 = new MsgGetPlayerStateSucceed
			{
				m_playerState = resultPlayerState
			};
			if (monitor != null)
			{
				monitor.EndMonitorForward(msg2, callbackObject, "ServerChangeCharacter_Succeeded");
			}
			if (callbackObject != null)
			{
				callbackObject.SendMessage("ServerChangeCharacter_Succeeded", msg2, SendMessageOptions.DontRequireReceiver);
			}
		}
		else
		{
			MsgServerConnctFailed msg = new MsgServerConnctFailed(net.resultStCd);
			if (monitor != null)
			{
				monitor.EndMonitorForward(msg, callbackObject, "ServerChangeCharacter_Failed");
			}
		}
		if (monitor != null)
		{
			monitor.EndMonitorBackward();
		}
	}
}
