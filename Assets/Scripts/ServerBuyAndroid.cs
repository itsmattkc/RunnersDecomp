using Message;
using System.Collections;
using UnityEngine;

public class ServerBuyAndroid
{
	public static IEnumerator Process(string receiptData, string signature, GameObject callbackObject)
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
		NetServerBuyAndroid net = new NetServerBuyAndroid(receiptData, signature);
		net.Request();
		monitor.StartMonitor(new ServerBuyAndroidRetry(receiptData, signature, callbackObject));
		while (net.IsExecuting())
		{
			yield return null;
		}
		if (net.IsSucceeded())
		{
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
				monitor.EndMonitorForward(msg2, callbackObject, "ServerBuyAndroid_Succeeded");
			}
			if (callbackObject != null)
			{
				callbackObject.SendMessage("ServerBuyAndroid_Succeeded", msg2, SendMessageOptions.DontRequireReceiver);
			}
		}
		else
		{
			MsgServerConnctFailed msg = new MsgServerConnctFailed(net.resultStCd);
			if (net.resultStCd == ServerInterface.StatusCode.AlreadyProcessedReceipt)
			{
				if (callbackObject != null)
				{
					callbackObject.SendMessage("ServerBuyAndroid_Failed", msg, SendMessageOptions.DontRequireReceiver);
				}
			}
			else if (monitor != null)
			{
				monitor.EndMonitorForward(msg, callbackObject, "ServerBuyAndroid_Failed");
			}
		}
		if (monitor != null)
		{
			monitor.EndMonitorBackward();
		}
	}
}
