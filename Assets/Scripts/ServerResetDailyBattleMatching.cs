using Message;
using System.Collections;
using UnityEngine;

public class ServerResetDailyBattleMatching
{
	private const string SuccessEvent = "ServerResetDailyBattleMatching_Succeeded";

	private const string FailEvent = "ServerResetDailyBattleMatching_Failed";

	public static IEnumerator Process(int type, GameObject callbackObject)
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
		NetServerResetDailyBattleMatching net = new NetServerResetDailyBattleMatching(type);
		net.Request();
		monitor.StartMonitor(new ServerResetDailyBattleMatchingRetry(type, callbackObject));
		while (net.IsExecuting())
		{
			yield return null;
		}
		if (net.IsSucceeded())
		{
			MsgResetDailyBattleMatchingSucceed msg2 = new MsgResetDailyBattleMatchingSucceed();
			net.playerState.CopyTo(msg2.playerState);
			net.playerState.CopyTo(ServerInterface.PlayerState);
			net.battleDataPair.CopyTo(msg2.battleDataPair);
			msg2.endTime = net.endTime;
			if (monitor != null)
			{
				monitor.EndMonitorForward(msg2, callbackObject, "ServerResetDailyBattleMatching_Succeeded");
			}
			if (callbackObject != null)
			{
				callbackObject.SendMessage("ServerResetDailyBattleMatching_Succeeded", msg2, SendMessageOptions.DontRequireReceiver);
			}
		}
		else
		{
			MsgServerConnctFailed msg = new MsgServerConnctFailed(net.resultStCd);
			if (callbackObject != null)
			{
				callbackObject.SendMessage("ServerResetDailyBattleMatching_Failed", msg, SendMessageOptions.DontRequireReceiver);
			}
		}
		if (monitor != null)
		{
			monitor.EndMonitorBackward();
		}
	}
}
