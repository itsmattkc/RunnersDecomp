using Message;
using System.Collections;
using UnityEngine;

public class ServerSetNoahId
{
	public static IEnumerator Process(string noahId, GameObject callbackObject)
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
		NetServerSetNoahId net = new NetServerSetNoahId(noahId);
		net.Request();
		monitor.StartMonitor(new ServerSetNoahIdRetry(noahId, callbackObject));
		while (net.IsExecuting())
		{
			yield return null;
		}
		if (net.IsSucceeded())
		{
			MsgSetNoahIdSucceed msg2 = new MsgSetNoahIdSucceed();
			if (monitor != null)
			{
				monitor.EndMonitorForward(msg2, callbackObject, "ServerSetNoahId_Succeeded");
			}
			if (callbackObject != null)
			{
				callbackObject.SendMessage("ServerSetNoahId_Succeeded", msg2, SendMessageOptions.DontRequireReceiver);
			}
		}
		else
		{
			Debug.Log("ServerSetNoahId: connectIsFailded");
			MsgServerConnctFailed msg = new MsgServerConnctFailed(net.resultStCd);
			if (monitor != null)
			{
				monitor.EndMonitorForward(msg, callbackObject, "ServerSetNoahId_Failed");
			}
		}
		if (monitor != null)
		{
			monitor.EndMonitorBackward();
		}
	}
}
