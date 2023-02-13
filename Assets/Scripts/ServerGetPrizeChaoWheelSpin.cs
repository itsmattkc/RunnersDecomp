using Message;
using System.Collections;
using UnityEngine;

public class ServerGetPrizeChaoWheelSpin
{
	public static IEnumerator Process(int chaoWheelSpinType, GameObject callbackObject)
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
		NetServerGetPrizeChaoWheelSpin net = new NetServerGetPrizeChaoWheelSpin(chaoWheelSpinType);
		net.Request();
		monitor.StartMonitor(new ServerGetPrizeChaoWheelSpinRetry(chaoWheelSpinType, callbackObject), 0f, HudNetworkConnect.DisplayType.NO_BG);
		while (net.IsExecuting())
		{
			yield return null;
		}
		if (net.IsSucceeded())
		{
			ServerPrizeState prizeState = null;
			switch (chaoWheelSpinType)
			{
			case 0:
				prizeState = ServerInterface.PremiumRoulettePrizeList;
				break;
			case 1:
				prizeState = ServerInterface.SpecialRoulettePrizeList;
				break;
			}
			if (net.resultPrizeState != null)
			{
				net.resultPrizeState.CopyTo(prizeState);
			}
			MsgGetPrizeChaoWheelSpinSucceed msg2 = new MsgGetPrizeChaoWheelSpinSucceed
			{
				m_prizeState = prizeState,
				m_type = chaoWheelSpinType
			};
			if (monitor != null)
			{
				monitor.EndMonitorForward(msg2, callbackObject, "ServerGetPrizeChaoWheelSpin_Succeeded");
			}
			if (callbackObject != null)
			{
				callbackObject.SendMessage("ServerGetPrizeChaoWheelSpin_Succeeded", msg2, SendMessageOptions.DontRequireReceiver);
			}
		}
		else
		{
			MsgServerConnctFailed msg = new MsgServerConnctFailed(net.resultStCd);
			if (monitor != null)
			{
				monitor.EndMonitorForward(msg, callbackObject, "ServerGetPrizeChaoWheelSpin_Failed");
			}
			if (callbackObject != null)
			{
				callbackObject.SendMessage("ServerGetPrizeChaoWheelSpin_Failed", SendMessageOptions.DontRequireReceiver);
			}
		}
		if (monitor != null)
		{
			monitor.EndMonitorBackward();
		}
	}
}
