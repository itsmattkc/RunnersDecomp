using Message;
using System.Collections;
using UnityEngine;

public class ServerGetEventState
{
	public static IEnumerator Process(int eventId, GameObject callbackObject)
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
		NetServerGetEventState net = new NetServerGetEventState(eventId);
		net.Request();
		monitor.StartMonitor(new ServerGetEventStateRetry(eventId, callbackObject), -1f, HudNetworkConnect.DisplayType.ONLY_ICON);
		while (net.IsExecuting())
		{
			yield return null;
		}
		if (net.IsSucceeded())
		{
			ServerEventState eventState = ServerInterface.EventState;
			net.resultEventState.CopyTo(eventState);
			MsgGetEventStateSucceed msg2 = new MsgGetEventStateSucceed
			{
				m_eventState = net.resultEventState
			};
			if (EventManager.Instance != null)
			{
				EventManager.Instance.SynchServerEventState();
			}
			if (monitor != null)
			{
				monitor.EndMonitorForward(msg2, callbackObject, "ServerGetEventState_Succeeded");
			}
			if (callbackObject != null)
			{
				callbackObject.SendMessage("ServerGetEventState_Succeeded", msg2, SendMessageOptions.DontRequireReceiver);
			}
		}
		else
		{
			MsgServerConnctFailed msg = new MsgServerConnctFailed(net.resultStCd);
			if (monitor != null)
			{
				monitor.EndMonitorForward(msg, callbackObject, "ServerGetEventState_Failed");
			}
		}
		if (monitor != null)
		{
			monitor.EndMonitorBackward();
		}
	}
}
