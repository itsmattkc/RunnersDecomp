using Message;
using System.Collections;
using UnityEngine;

public class ServerSetFacebookScopedId
{
	public static IEnumerator Process(string userId, GameObject callbackObject)
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
		NetServerSetFacebookScopedId net = new NetServerSetFacebookScopedId(userId);
		net.Request();
		monitor.StartMonitor(new ServerSetFacebookScopedIdRetry(userId, callbackObject));
		while (net.IsExecuting())
		{
			yield return null;
		}
		if (net.IsSucceeded())
		{
			MsgSetFacebookScopedIdSucceed msg2 = new MsgSetFacebookScopedIdSucceed();
			if (monitor != null)
			{
				monitor.EndMonitorForward(msg2, callbackObject, "ServerSetFacebookScopedId_Succeeded");
			}
			if (callbackObject != null)
			{
				callbackObject.SendMessage("ServerSetFacebookScopedId_Succeeded", msg2, SendMessageOptions.DontRequireReceiver);
			}
		}
		else
		{
			MsgServerConnctFailed msg = new MsgServerConnctFailed(net.resultStCd);
			if (monitor != null)
			{
				monitor.EndMonitorForward(msg, callbackObject, "ServerSetFacebookScopedId_Failed");
			}
		}
		if (monitor != null)
		{
			monitor.EndMonitorBackward();
		}
	}
}
