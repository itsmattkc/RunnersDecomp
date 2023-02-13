using Message;
using System.Collections;
using UnityEngine;

public class ServerGetWheelOptionsGeneral
{
	private const string SuccessEvent = "ServerGetWheelOptionsGeneral_Succeeded";

	private const string FailEvent = "ServerGetWheelOptionsGeneral_Failed";

	public static IEnumerator Process(int eventId, int spinId, GameObject callbackObject)
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
		NetServerGetWheelOptionsGeneral net = new NetServerGetWheelOptionsGeneral(eventId, spinId);
		net.Request();
		monitor.StartMonitor(new ServerGetWheelOptionsGeneralRetry(spinId, eventId, callbackObject));
		while (net.IsExecuting())
		{
			yield return null;
		}
		if (net.IsSucceeded())
		{
			ServerWheelOptionsGeneral resultWheelOptions = net.resultWheelOptionsGeneral;
			MsgGetWheelOptionsGeneralSucceed msg2 = new MsgGetWheelOptionsGeneralSucceed
			{
				m_wheelOptionsGeneral = resultWheelOptions
			};
			if (monitor != null)
			{
				monitor.EndMonitorForward(msg2, callbackObject, "ServerGetWheelOptionsGeneral_Succeeded");
			}
			if (callbackObject != null)
			{
				callbackObject.SendMessage("ServerGetWheelOptionsGeneral_Succeeded", msg2, SendMessageOptions.DontRequireReceiver);
			}
		}
		else
		{
			MsgServerConnctFailed msg = new MsgServerConnctFailed(net.resultStCd);
			if (monitor != null)
			{
				monitor.EndMonitorForward(msg, callbackObject, "ServerGetWheelOptionsGeneral_Failed");
			}
		}
		if (monitor != null)
		{
			monitor.EndMonitorBackward();
		}
	}
}
