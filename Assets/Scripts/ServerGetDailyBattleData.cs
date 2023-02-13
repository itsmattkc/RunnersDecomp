using Message;
using System.Collections;
using UnityEngine;

public class ServerGetDailyBattleData
{
	private const string SuccessEvent = "ServerGetDailyBattleData_Succeeded";

	private const string FailEvent = "ServerGetDailyBattleData_Failed";

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
		NetServerGetDailyBattleData net = new NetServerGetDailyBattleData();
		net.Request();
		monitor.StartMonitor(new ServerGetDailyBattleDataRetry(callbackObject));
		while (net.IsExecuting())
		{
			yield return null;
		}
		if (net.IsSucceeded())
		{
			MsgGetDailyBattleDataSucceed msg2 = new MsgGetDailyBattleDataSucceed();
			net.battleDataPair.CopyTo(msg2.battleDataPair);
			if (monitor != null)
			{
				monitor.EndMonitorForward(msg2, callbackObject, "ServerGetDailyBattleData_Succeeded");
			}
			if (callbackObject != null)
			{
				callbackObject.SendMessage("ServerGetDailyBattleData_Succeeded", msg2, SendMessageOptions.DontRequireReceiver);
			}
		}
		else
		{
			MsgServerConnctFailed msg = new MsgServerConnctFailed(net.resultStCd);
			if (callbackObject != null)
			{
				callbackObject.SendMessage("ServerGetDailyBattleData_Failed", msg, SendMessageOptions.DontRequireReceiver);
			}
		}
		if (monitor != null)
		{
			monitor.EndMonitorBackward();
		}
	}
}
