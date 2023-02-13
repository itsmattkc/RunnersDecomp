using Message;
using System.Collections;
using UnityEngine;

public class ServerGetWheelOptions
{
	private const string SuccessEvent = "ServerGetWheelOptions_Succeeded";

	private const string FailEvent = "ServerGetWheelOptions_Failed";

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
		NetServerGetWheelOptions net = new NetServerGetWheelOptions();
		net.Request();
		monitor.StartMonitor(new ServerGetWheelOptionsRetry(callbackObject));
		while (net.IsExecuting())
		{
			yield return null;
		}
		if (net.IsSucceeded())
		{
			ServerWheelOptions wheelOptions = ServerInterface.WheelOptions;
			ServerWheelOptions resultWheelOptions = net.resultWheelOptions;
			resultWheelOptions.CopyTo(wheelOptions);
			MsgGetWheelOptionsSucceed msg2 = new MsgGetWheelOptionsSucceed
			{
				m_wheelOptions = wheelOptions
			};
			if (monitor != null)
			{
				monitor.EndMonitorForward(msg2, callbackObject, "ServerGetWheelOptions_Succeeded");
			}
			if (callbackObject != null)
			{
				callbackObject.SendMessage("ServerGetWheelOptions_Succeeded", msg2, SendMessageOptions.DontRequireReceiver);
			}
		}
		else
		{
			MsgServerConnctFailed msg = new MsgServerConnctFailed(net.resultStCd);
			if (monitor != null)
			{
				monitor.EndMonitorForward(msg, callbackObject, "ServerGetWheelOptions_Failed");
			}
		}
		if (monitor != null)
		{
			monitor.EndMonitorBackward();
		}
	}
}
