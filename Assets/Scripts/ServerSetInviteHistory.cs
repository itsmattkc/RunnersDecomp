using Message;
using System.Collections;
using UnityEngine;

public class ServerSetInviteHistory
{
	public enum IncentiveType
	{
		LOGIN,
		REVIEW,
		FEED,
		ACHIEVEMENT,
		PUSH_NOLOGIN
	}

	public static IEnumerator Process(string facebookIdHash, GameObject callbackObject)
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
		NetServerSetInviteHistory net = new NetServerSetInviteHistory(facebookIdHash);
		net.Request();
		monitor.StartMonitor(new ServerSetInviteHistoryRetry(facebookIdHash, callbackObject));
		while (net.IsExecuting())
		{
			yield return null;
		}
		if (net.IsSucceeded())
		{
			MsgSetInviteHistorySucceed msg2 = new MsgSetInviteHistorySucceed();
			if (monitor != null)
			{
				monitor.EndMonitorForward(msg2, callbackObject, "ServerSetInviteHistory_Succeeded");
			}
			if (callbackObject != null)
			{
				callbackObject.SendMessage("ServerSetInviteHistory_Succeeded", msg2, SendMessageOptions.DontRequireReceiver);
			}
		}
		else
		{
			MsgServerConnctFailed msg = new MsgServerConnctFailed(net.resultStCd);
			if (monitor != null)
			{
				monitor.EndMonitorForward(msg, callbackObject, "ServerSetInviteHistory_Failed");
			}
		}
		if (monitor != null)
		{
			monitor.EndMonitorBackward();
		}
	}
}
