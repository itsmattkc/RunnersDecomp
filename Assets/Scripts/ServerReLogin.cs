using Message;
using System.Collections;
using UnityEngine;

public class ServerReLogin
{
	private const string SuccessEvent = "ServerReLogin_Succeeded";

	private const string FailEvent = "ServerReLogin_Failed";

	public static IEnumerator Process(GameObject callbackObject)
	{
		NetMonitor monitor = NetMonitor.Instance;
		if (!(monitor != null))
		{
			yield break;
		}
		NetServerReLogin net = new NetServerReLogin();
		net.Request();
		monitor.StartMonitor(new ServerReLoginRetry(callbackObject));
		while (net.IsExecuting())
		{
			yield return null;
		}
		if (net.IsSucceeded())
		{
			ServerLoginState loginState = ServerInterface.LoginState;
			loginState.sessionId = net.resultSessionId;
			loginState.sessionTimeLimit = net.sessionTimeLimit;
			if (monitor != null)
			{
				monitor.EndMonitorForward(null, callbackObject, "ServerReLogin_Succeeded");
			}
			if (callbackObject != null)
			{
				callbackObject.SendMessage("ServerReLogin_Succeeded", null, SendMessageOptions.DontRequireReceiver);
			}
		}
		else
		{
			MsgServerConnctFailed msg = new MsgServerConnctFailed(net.resultStCd);
			if (monitor != null)
			{
				monitor.EndMonitorForward(msg, callbackObject, "ServerReLogin_Failed");
			}
		}
		if (monitor != null)
		{
			monitor.EndMonitorBackward();
		}
	}
}
