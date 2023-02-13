using Message;
using System.Collections;
using UnityEngine;

public class ServerGetWeeklyLeaderboardOptions
{
	public static IEnumerator Process(int mode, GameObject callbackObject)
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
		NetServerGetWeeklyLeaderboardOptions net = new NetServerGetWeeklyLeaderboardOptions(mode);
		net.Request();
		monitor.StartMonitor(new ServerGetWeeklyLeaderboardOptionsRetry(mode, callbackObject));
		while (net.IsExecuting())
		{
			yield return null;
		}
		if (net.IsSucceeded())
		{
			MsgGetWeeklyLeaderboardOptions msg2 = new MsgGetWeeklyLeaderboardOptions
			{
				m_weeklyLeaderboardOptions = net.weeklyLeaderboardOptions
			};
			if (SingletonGameObject<RankingManager>.Instance != null)
			{
				SingletonGameObject<RankingManager>.Instance.SetRankingDataSet(net.weeklyLeaderboardOptions);
			}
			if (monitor != null)
			{
				monitor.EndMonitorForward(msg2, callbackObject, "ServerGetWeeklyLeaderboardOptions_Succeeded");
			}
			if (callbackObject != null)
			{
				callbackObject.SendMessage("ServerGetWeeklyLeaderboardOptions_Succeeded", msg2, SendMessageOptions.DontRequireReceiver);
			}
		}
		else
		{
			MsgServerConnctFailed msg = new MsgServerConnctFailed(net.resultStCd);
			if (monitor != null)
			{
				monitor.EndMonitorForward(msg, callbackObject, "ServerWeeklyLeaderboardOptions_Failed");
			}
		}
		if (monitor != null)
		{
			monitor.EndMonitorBackward();
		}
	}
}
