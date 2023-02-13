using Message;
using System.Collections;
using UnityEngine;

public class ServerGetChaoWheelOptions
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
		NetServerGetChaoWheelOptions net = new NetServerGetChaoWheelOptions();
		net.Request();
		monitor.StartMonitor(new ServerGetChaoWheelOptionsRetry(callbackObject), 0f, HudNetworkConnect.DisplayType.NO_BG);
		while (net.IsExecuting())
		{
			yield return null;
		}
		if (net.IsSucceeded())
		{
			ServerChaoWheelOptions chaoWheelOptions = ServerInterface.ChaoWheelOptions;
			ServerChaoWheelOptions resultChaoWheelOptions = net.resultChaoWheelOptions;
			net.resultChaoWheelOptions.IsConnected = true;
			resultChaoWheelOptions.CopyTo(chaoWheelOptions);
			MsgGetChaoWheelOptionsSucceed msg2 = new MsgGetChaoWheelOptionsSucceed
			{
				m_options = chaoWheelOptions
			};
			if (monitor != null)
			{
				monitor.EndMonitorForward(msg2, callbackObject, "ServerGetChaoWheelOptions_Succeeded");
			}
			if (callbackObject != null)
			{
				callbackObject.SendMessage("ServerGetChaoWheelOptions_Succeeded", msg2, SendMessageOptions.DontRequireReceiver);
			}
		}
		else
		{
			MsgServerConnctFailed msg = new MsgServerConnctFailed(net.resultStCd);
			if (monitor != null)
			{
				monitor.EndMonitorForward(msg, callbackObject, "ServerGetChaoWheelOptions_Failed");
			}
		}
		if (monitor != null)
		{
			monitor.EndMonitorBackward();
		}
	}
}
