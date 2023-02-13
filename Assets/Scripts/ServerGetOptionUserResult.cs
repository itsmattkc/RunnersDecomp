using Message;
using System.Collections;
using UnityEngine;

public class ServerGetOptionUserResult
{
	private const string SuccessEvent = "ServerGetOptionUserResult_Succeeded";

	private const string FailEvent = "ServerGetOptionUserResult_Failed";

	public static IEnumerator Process(GameObject callbackObject)
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
		NetServerGetOptionUserResult net = new NetServerGetOptionUserResult();
		net.Request();
		monitor.StartMonitor(new ServerGetOptionUserResultRetry(callbackObject));
		while (net.IsExecuting())
		{
			yield return null;
		}
		if (net.IsSucceeded())
		{
			MsgGetOptionUserResultSucceed msg2 = new MsgGetOptionUserResultSucceed
			{
				m_serverOptionUserResult = net.UserResult
			};
			if (monitor != null)
			{
				monitor.EndMonitorForward(msg2, callbackObject, "ServerGetOptionUserResult_Succeeded");
			}
			if (callbackObject != null)
			{
				callbackObject.SendMessage("ServerGetOptionUserResult_Succeeded", msg2, SendMessageOptions.DontRequireReceiver);
			}
		}
		else
		{
			MsgServerConnctFailed msg = new MsgServerConnctFailed(net.resultStCd);
			if (monitor != null)
			{
				monitor.EndMonitorForward(msg, callbackObject, "ServerGetOptionUserResult_Failed");
			}
		}
		if (monitor != null)
		{
			monitor.EndMonitorBackward();
		}
	}
}
