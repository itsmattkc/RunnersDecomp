using Message;
using System.Collections;
using UnityEngine;

public class ServerGetDailyBattleStatus
{
	private const string SuccessEvent = "ServerGetDailyBattleStatus_Succeeded";

	private const string FailEvent = "ServerGetDailyBattleStatus_Failed";

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
		NetServerGetDailyBattleStatus net = new NetServerGetDailyBattleStatus();
		net.Request();
		monitor.StartMonitor(new ServerGetDailyBattleStatusRetry(callbackObject));
		while (net.IsExecuting())
		{
			yield return null;
		}
		if (net.IsSucceeded())
		{
			MsgGetDailyBattleStatusSucceed msg2 = new MsgGetDailyBattleStatusSucceed();
			net.battleStatus.CopyTo(msg2.battleStatus);
			msg2.endTime = net.endTime;
			if (monitor != null)
			{
				monitor.EndMonitorForward(msg2, callbackObject, "ServerGetDailyBattleStatus_Succeeded");
			}
			if (callbackObject != null)
			{
				callbackObject.SendMessage("ServerGetDailyBattleStatus_Succeeded", msg2, SendMessageOptions.DontRequireReceiver);
			}
		}
		else
		{
			MsgServerConnctFailed msg = new MsgServerConnctFailed(net.resultStCd);
			if (callbackObject != null)
			{
				callbackObject.SendMessage("ServerGetDailyBattleStatus_Failed", msg, SendMessageOptions.DontRequireReceiver);
			}
		}
		if (monitor != null)
		{
			monitor.EndMonitorBackward();
		}
	}
}
