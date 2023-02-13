using Message;
using System.Collections;
using UnityEngine;

public class ServerActRetry
{
	public static IEnumerator Process(GameObject callbackObject)
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
		NetServerActRetry net = new NetServerActRetry();
		net.Request();
		monitor.StartMonitor(new ServerActRetryRetry(callbackObject));
		while (net.IsExecuting())
		{
			yield return null;
		}
		if (net.IsSucceeded())
		{
			MsgActRetrySucceed msg2 = new MsgActRetrySucceed();
			if (monitor != null)
			{
				monitor.EndMonitorForward(msg2, callbackObject, "ServerActRetry_Succeeded");
			}
			if (callbackObject != null)
			{
				callbackObject.SendMessage("ServerActRetry_Succeeded", msg2, SendMessageOptions.DontRequireReceiver);
			}
		}
		else
		{
			MsgServerConnctFailed msg = new MsgServerConnctFailed(net.resultStCd);
			if (monitor != null)
			{
				monitor.EndMonitorForward(msg, callbackObject, "ServerActRetry_Failed");
			}
		}
		if (monitor != null)
		{
			monitor.EndMonitorBackward();
		}
	}
}
