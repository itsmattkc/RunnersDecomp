using Message;
using System.Collections;
using UnityEngine;

public class ServerEquipChao
{
	public static IEnumerator Process(int mainChaoId, int subChaoId, GameObject callbackObject)
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
		NetServerEquipChao net = new NetServerEquipChao(mainChaoId, subChaoId);
		net.Request();
		monitor.StartMonitor(new ServerEquipChaoRetry(mainChaoId, subChaoId, callbackObject));
		while (net.IsExecuting())
		{
			yield return null;
		}
		if (net.IsSucceeded())
		{
			DeckUtil.ChaoSetSaveAuto(mainChaoId, subChaoId);
			ServerPlayerState playerState = ServerInterface.PlayerState;
			ServerPlayerState resultPlayerState = net.resultPlayerState;
			resultPlayerState.CopyTo(playerState);
			MsgGetPlayerStateSucceed msg2 = new MsgGetPlayerStateSucceed
			{
				m_playerState = playerState
			};
			if (monitor != null)
			{
				monitor.EndMonitorForward(msg2, callbackObject, "ServerEquipChao_Succeeded");
			}
			if (callbackObject != null)
			{
				callbackObject.SendMessage("ServerEquipChao_Succeeded", msg2, SendMessageOptions.DontRequireReceiver);
			}
		}
		else
		{
			MsgServerConnctFailed msg = new MsgServerConnctFailed(net.resultStCd);
			if (monitor != null)
			{
				monitor.EndMonitorForward(msg, callbackObject, "ServerEquipChao_Failed");
			}
		}
		if (monitor != null)
		{
			monitor.EndMonitorBackward();
		}
	}
}
