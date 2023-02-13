using Message;
using System.Collections;
using UnityEngine;

public class ServerPostDailyBattleResult
{
	private const string SuccessEvent = "ServerPostDailyBattleResult_Succeeded";

	private const string FailEvent = "ServerPostDailyBattleResult_Failed";

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
		NetServerPostDailyBattleResult net = new NetServerPostDailyBattleResult();
		net.Request();
		monitor.StartMonitor(new ServerPostDailyBattleResultRetry(callbackObject));
		while (net.IsExecuting())
		{
			yield return null;
		}
		if (net.IsSucceeded())
		{
			MsgPostDailyBattleResultSucceed msg2 = new MsgPostDailyBattleResultSucceed();
			net.battleStatus.CopyTo(msg2.battleStatus);
			net.battleDataPair.CopyTo(msg2.battleDataPair);
			msg2.rewardFlag = net.rewardFlag;
			if (msg2.rewardFlag && net.rewardBattleDataPair != null)
			{
				msg2.rewardBattleDataPair = new ServerDailyBattleDataPair();
				net.rewardBattleDataPair.CopyTo(msg2.rewardBattleDataPair);
			}
			if (monitor != null)
			{
				monitor.EndMonitorForward(msg2, callbackObject, "ServerPostDailyBattleResult_Succeeded");
			}
			if (callbackObject != null)
			{
				callbackObject.SendMessage("ServerPostDailyBattleResult_Succeeded", msg2, SendMessageOptions.DontRequireReceiver);
			}
		}
		else
		{
			MsgServerConnctFailed msg = new MsgServerConnctFailed(net.resultStCd);
			if (callbackObject != null)
			{
				callbackObject.SendMessage("ServerPostDailyBattleResult_Failed", msg, SendMessageOptions.DontRequireReceiver);
			}
		}
		if (monitor != null)
		{
			monitor.EndMonitorBackward();
		}
	}
}
