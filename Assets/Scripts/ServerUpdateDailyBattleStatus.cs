using Message;
using System.Collections;
using UnityEngine;

public class ServerUpdateDailyBattleStatus
{
	private const string SuccessEvent = "ServerUpdateDailyBattleStatus_Succeeded";

	private const string FailEvent = "ServerUpdateDailyBattleStatus_Failed";

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
		NetServerUpdateDailyBattleStatus net = new NetServerUpdateDailyBattleStatus();
		net.Request();
		monitor.StartMonitor(new ServerUpdateDailyBattleStatusRetry(callbackObject));
		while (net.IsExecuting())
		{
			yield return null;
		}
		if (net.IsSucceeded())
		{
			MsgUpdateDailyBattleStatusSucceed msg2 = new MsgUpdateDailyBattleStatusSucceed();
			net.battleDataStatus.CopyTo(msg2.battleStatus);
			msg2.endTime = net.endTime;
			msg2.rewardFlag = net.rewardFlag;
			if (msg2.rewardFlag && net.rewardBattleDataPair != null)
			{
				msg2.rewardBattleDataPair = new ServerDailyBattleDataPair();
				net.rewardBattleDataPair.CopyTo(msg2.rewardBattleDataPair);
			}
			if (monitor != null)
			{
				monitor.EndMonitorForward(msg2, callbackObject, "ServerUpdateDailyBattleStatus_Succeeded");
			}
			if (callbackObject != null)
			{
				callbackObject.SendMessage("ServerUpdateDailyBattleStatus_Succeeded", msg2, SendMessageOptions.DontRequireReceiver);
			}
		}
		else
		{
			MsgServerConnctFailed msg = new MsgServerConnctFailed(net.resultStCd);
			if (callbackObject != null)
			{
				callbackObject.SendMessage("ServerUpdateDailyBattleStatus_Failed", msg, SendMessageOptions.DontRequireReceiver);
			}
		}
		if (monitor != null)
		{
			monitor.EndMonitorBackward();
		}
	}
}
