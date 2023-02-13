using Message;
using System.Collections;
using UnityEngine;

public class ServerGetPrizeDailyBattle
{
	private const string SuccessEvent = "ServerGetPrizeDailyBattle_Succeeded";

	private const string FailEvent = "ServerGetPrizeDailyBattle_Failed";

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
		NetServerGetPrizeDailyBattle net = new NetServerGetPrizeDailyBattle();
		net.Request();
		monitor.StartMonitor(new ServerGetPrizeDailyBattleRetry(callbackObject));
		while (net.IsExecuting())
		{
			yield return null;
		}
		if (net.IsSucceeded())
		{
			MsgGetPrizeDailyBattleSucceed msg2 = new MsgGetPrizeDailyBattleSucceed();
			if (net.battleDataPrizeList != null && net.battleDataPrizeList.Count > 0)
			{
				foreach (ServerDailyBattlePrizeData prize in net.battleDataPrizeList)
				{
					ServerDailyBattlePrizeData setPrize = new ServerDailyBattlePrizeData();
					prize.CopyTo(setPrize);
					msg2.battlePrizeDataList.Add(setPrize);
				}
			}
			if (monitor != null)
			{
				monitor.EndMonitorForward(msg2, callbackObject, "ServerGetPrizeDailyBattle_Succeeded");
			}
			if (callbackObject != null)
			{
				callbackObject.SendMessage("ServerGetPrizeDailyBattle_Succeeded", msg2, SendMessageOptions.DontRequireReceiver);
			}
		}
		else
		{
			MsgServerConnctFailed msg = new MsgServerConnctFailed(net.resultStCd);
			if (callbackObject != null)
			{
				callbackObject.SendMessage("ServerGetPrizeDailyBattle_Failed", msg, SendMessageOptions.DontRequireReceiver);
			}
		}
		if (monitor != null)
		{
			monitor.EndMonitorBackward();
		}
	}
}
