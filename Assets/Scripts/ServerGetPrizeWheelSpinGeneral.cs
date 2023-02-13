using Message;
using System.Collections;
using UnityEngine;

public class ServerGetPrizeWheelSpinGeneral
{
	public static IEnumerator Process(int eventId, int spinType, GameObject callbackObject)
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
		NetServerGetPrizeWheelSpinGeneral net = new NetServerGetPrizeWheelSpinGeneral(eventId, spinType);
		net.Request();
		monitor.StartMonitor(new ServerGetPrizeWheelSpinGeneralRetry(eventId, spinType, callbackObject), 0f, HudNetworkConnect.DisplayType.NO_BG);
		while (net.IsExecuting())
		{
			yield return null;
		}
		if (net.IsSucceeded())
		{
			if (net.resultPrizeState != null)
			{
				net.resultPrizeState.CopyTo(ServerInterface.RaidRoulettePrizeList);
			}
			MsgGetPrizeWheelSpinGeneralSucceed msg2 = new MsgGetPrizeWheelSpinGeneralSucceed
			{
				prizeState = ServerInterface.RaidRoulettePrizeList
			};
			if (monitor != null)
			{
				monitor.EndMonitorForward(msg2, callbackObject, "ServerGetPrizeWheelSpinGeneral_Succeeded");
			}
			if (callbackObject != null)
			{
				callbackObject.SendMessage("ServerGetPrizeWheelSpinGeneral_Succeeded", msg2, SendMessageOptions.DontRequireReceiver);
			}
		}
		else
		{
			MsgServerConnctFailed msg = new MsgServerConnctFailed(net.resultStCd);
			if (monitor != null)
			{
				monitor.EndMonitorForward(msg, callbackObject, "ServerGetPrizeWheelSpinGeneral_Failed");
			}
			if (callbackObject != null)
			{
				callbackObject.SendMessage("ServerGetPrizeWheelSpinGeneral_Failed", SendMessageOptions.DontRequireReceiver);
			}
		}
		if (monitor != null)
		{
			monitor.EndMonitorBackward();
		}
	}
}
