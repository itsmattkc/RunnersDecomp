using Message;
using System.Collections;
using UnityEngine;

public class ServerGetWheelSpinInfo
{
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
		NetServerGetWheelSpinInfo net = new NetServerGetWheelSpinInfo();
		net.Request();
		monitor.StartMonitor(new ServerGetWheelSpinInfoRetry(callbackObject));
		while (net.IsExecuting())
		{
			yield return null;
		}
		if (net.IsSucceeded())
		{
			MsgGetChaoWheelSpinInfoSucceed msg2 = new MsgGetChaoWheelSpinInfoSucceed
			{
				m_wheelSpinInfos = net.resultWheelSpinInfos
			};
			if (monitor != null)
			{
				monitor.EndMonitorForward(msg2, callbackObject, "ServerGetWheelSpinInfo_Succeeded");
			}
			if (callbackObject != null)
			{
				callbackObject.SendMessage("ServerGetWheelSpinInfo_Succeeded", msg2, SendMessageOptions.DontRequireReceiver);
			}
		}
		else
		{
			MsgServerConnctFailed msg = new MsgServerConnctFailed(net.resultStCd);
			if (monitor != null)
			{
				monitor.EndMonitorForward(msg, callbackObject, "ServerGetWheelSpinInfo_Failed");
			}
		}
		if (monitor != null)
		{
			monitor.EndMonitorBackward();
		}
	}
}
