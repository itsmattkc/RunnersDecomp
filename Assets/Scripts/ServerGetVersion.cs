using Message;
using System.Collections;
using UnityEngine;

public class ServerGetVersion
{
	public static IEnumerator Process(GameObject callbackObject)
	{
		NetMonitor monitor = NetMonitor.Instance;
		if (!(monitor != null))
		{
			yield break;
		}
		NetServerGetVersion net = new NetServerGetVersion();
		net.Request();
		monitor.StartMonitor(new ServerGetVersionRetry(callbackObject));
		while (net.IsExecuting())
		{
			yield return null;
		}
		if (net.IsSucceeded())
		{
			MsgGetVersionSucceed msg2 = new MsgGetVersionSucceed();
			if (monitor != null)
			{
				monitor.EndMonitorForward(msg2, callbackObject, "ServerGetVersion_Succeeded");
			}
			if (callbackObject != null)
			{
				callbackObject.SendMessage("ServerGetVersion_Succeeded", msg2, SendMessageOptions.DontRequireReceiver);
			}
		}
		else
		{
			MsgServerConnctFailed msg = new MsgServerConnctFailed(net.resultStCd);
			if (monitor != null)
			{
				monitor.EndMonitorForward(msg, callbackObject, "ServerGetVersion_Failed");
			}
		}
		if (monitor != null)
		{
			monitor.EndMonitorBackward();
		}
	}
}
