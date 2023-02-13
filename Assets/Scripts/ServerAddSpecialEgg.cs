using Message;
using System.Collections;
using UnityEngine;

public class ServerAddSpecialEgg
{
	public static IEnumerator Process(int numSpecialEgg, GameObject callbackObject)
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
		NetServerAddSpecialEgg net = new NetServerAddSpecialEgg(numSpecialEgg);
		net.Request();
		monitor.StartMonitor(new ServerAddSpecialEggRetry(numSpecialEgg, callbackObject));
		while (net.IsExecuting())
		{
			yield return null;
		}
		if (net.IsSucceeded())
		{
			ServerChaoWheelOptions serverChaoWheelOptions = ServerInterface.ChaoWheelOptions;
			if (serverChaoWheelOptions != null)
			{
				serverChaoWheelOptions.NumSpecialEggs = net.resultSpecialEgg;
			}
			if (RouletteManager.Instance != null)
			{
				RouletteManager.Instance.specialEgg = net.resultSpecialEgg;
				GeneralUtil.SetRouletteBtnIcon();
			}
			MsgAddSpecialEggSucceed msg2 = new MsgAddSpecialEggSucceed();
			if (monitor != null)
			{
				monitor.EndMonitorForward(msg2, callbackObject, "ServerAddSpecialEgg_Succeeded");
			}
			if (callbackObject != null)
			{
				callbackObject.SendMessage("ServerAddSpecialEgg_Succeeded", msg2, SendMessageOptions.DontRequireReceiver);
			}
		}
		else
		{
			MsgServerConnctFailed msg = new MsgServerConnctFailed(net.resultStCd);
			if (monitor != null)
			{
				monitor.EndMonitorForward(msg, callbackObject, "ServerAddSpecialEgg_Failed");
			}
		}
		if (monitor != null)
		{
			monitor.EndMonitorBackward();
		}
	}
}
