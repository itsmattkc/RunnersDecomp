using Message;
using System.Collections;
using UnityEngine;

public class ServerSendApollo
{
	public static IEnumerator Process(int type, string[] value, GameObject callbackObject)
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
		NetServerSendApollo net = new NetServerSendApollo(type, value);
		net.Request();
		monitor.StartMonitor(new ServerSendApolloRetry(type, value, callbackObject));
		while (net.IsExecuting())
		{
			yield return null;
		}
		if (net.IsSucceeded())
		{
			MsgSendApolloSucceed msg2 = new MsgSendApolloSucceed();
			if (monitor != null)
			{
				monitor.EndMonitorForward(msg2, callbackObject, "ServerSendApollo_Succeeded");
			}
			if (callbackObject != null)
			{
				callbackObject.SendMessage("ServerSendApollo_Succeeded", msg2, SendMessageOptions.DontRequireReceiver);
			}
		}
		else
		{
			Debug.Log("ServerSendApollo: connectIsFailded");
			MsgServerConnctFailed msg = new MsgServerConnctFailed(net.resultStCd);
			if (monitor != null)
			{
				monitor.EndMonitorForward(msg, callbackObject, "ServerSendApollo_Failed");
			}
		}
		if (monitor != null)
		{
			monitor.EndMonitorBackward();
		}
	}
}
