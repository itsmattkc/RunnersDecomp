using Message;
using System.Collections;
using UnityEngine;

public class ServerBuyIos
{
	public static IEnumerator Process(string receiptData, GameObject callbackObject)
	{
		NetMonitor monitor = NetMonitor.Instance;
		if (!(monitor != null))
		{
			yield break;
		}
		monitor.PrepareConnect(ServerSessionWatcher.ValidateType.LOGIN_OR_RELOGIN);
		while (!monitor.IsEndPrepare())
		{
			yield return null;
		}
		if (!monitor.IsSuccessPrepare())
		{
			yield break;
		}
		NetServerBuyIos net = new NetServerBuyIos(receiptData);
		net.Request();
		monitor.StartMonitor(new ServerBuyIosRetry(receiptData, callbackObject));
		while (net.IsExecuting())
		{
			yield return null;
		}
		if (net.IsSucceeded())
		{
			Debug.Log("ServerBuyIos: connectIsSucceed");
			ServerPlayerState playerState = ServerInterface.PlayerState;
			ServerPlayerState resultPlayerState = net.resultPlayerState;
			resultPlayerState.CopyTo(playerState);
			ServerInterface.SettingState.m_isPurchased = true;
			MsgGetPlayerStateSucceed msg2 = new MsgGetPlayerStateSucceed
			{
				m_playerState = resultPlayerState
			};
			if (monitor != null)
			{
				monitor.EndMonitorForward(msg2, callbackObject, "ServerBuyIos_Succeeded");
			}
			if (callbackObject != null)
			{
				callbackObject.SendMessage("ServerBuyIos_Succeeded", msg2, SendMessageOptions.DontRequireReceiver);
			}
		}
		else
		{
			Debug.Log("ServerBuyIos: connectIsFailded");
			MsgServerConnctFailed msg = new MsgServerConnctFailed(net.resultStCd);
			if (net.resultStCd == ServerInterface.StatusCode.AlreadyProcessedReceipt)
			{
				if (callbackObject != null)
				{
					callbackObject.SendMessage("ServerBuyIos_Failed", msg, SendMessageOptions.DontRequireReceiver);
				}
			}
			else if (monitor != null)
			{
				monitor.EndMonitorForward(msg, callbackObject, "ServerBuyIos_Failed");
			}
		}
		if (monitor != null)
		{
			monitor.EndMonitorBackward();
		}
	}
}
