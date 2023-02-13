using Message;
using System.Collections;
using UnityEngine;

public class ServerGetDailyBattleDataHistory
{
	private const string SuccessEvent = "ServerGetDailyBattleDataHistory_Succeeded";

	private const string FailEvent = "ServerGetDailyBattleDataHistory_Failed";

	public static IEnumerator Process(int count, GameObject callbackObject)
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
		NetServerGetDailyBattleDataHistory net = new NetServerGetDailyBattleDataHistory(count);
		net.Request();
		monitor.StartMonitor(new ServerGetDailyBattleDataHistoryRetry(count, callbackObject));
		while (net.IsExecuting())
		{
			yield return null;
		}
		if (net.IsSucceeded())
		{
			MsgGetDailyBattleDataHistorySucceed msg2 = new MsgGetDailyBattleDataHistorySucceed();
			if (net.battleDataPairList != null && net.battleDataPairList.Count > 0)
			{
				foreach (ServerDailyBattleDataPair pair in net.battleDataPairList)
				{
					ServerDailyBattleDataPair setPair = new ServerDailyBattleDataPair();
					pair.CopyTo(setPair);
					msg2.battleDataPairList.Add(setPair);
				}
			}
			if (monitor != null)
			{
				monitor.EndMonitorForward(msg2, callbackObject, "ServerGetDailyBattleDataHistory_Succeeded");
			}
			if (callbackObject != null)
			{
				callbackObject.SendMessage("ServerGetDailyBattleDataHistory_Succeeded", msg2, SendMessageOptions.DontRequireReceiver);
			}
		}
		else
		{
			MsgServerConnctFailed msg = new MsgServerConnctFailed(net.resultStCd);
			if (callbackObject != null)
			{
				callbackObject.SendMessage("ServerGetDailyBattleDataHistory_Failed", msg, SendMessageOptions.DontRequireReceiver);
			}
		}
		if (monitor != null)
		{
			monitor.EndMonitorBackward();
		}
	}
}
