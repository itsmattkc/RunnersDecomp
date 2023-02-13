using Message;
using System.Collections;
using UnityEngine;

public class ServerGetLeaderboardEntries
{
	public static IEnumerator Process(int mode, int first, int count, int index, int rankingType, int eventId, string[] friendIdList, GameObject callbackObject)
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
		NetServerGetLeaderboardEntries net = new NetServerGetLeaderboardEntries(mode, first, count, index, rankingType, eventId, friendIdList);
		net.Request();
		monitor.StartMonitor(new ServerGetLeaderboardEntriesRetry(mode, first, count, index, rankingType, eventId, friendIdList, callbackObject));
		while (net.IsExecuting())
		{
			yield return null;
		}
		if (net.IsSucceeded())
		{
			MsgGetLeaderboardEntriesSucceed msg2 = new MsgGetLeaderboardEntriesSucceed
			{
				m_leaderboardEntries = ServerInterface.LeaderboardEntries
			};
			if (monitor != null)
			{
				monitor.EndMonitorForward(msg2, callbackObject, "ServerGetLeaderboardEntries_Succeeded");
			}
			if (callbackObject != null)
			{
				callbackObject.SendMessage("ServerGetLeaderboardEntries_Succeeded", msg2, SendMessageOptions.DontRequireReceiver);
			}
		}
		else
		{
			MsgServerConnctFailed msg = new MsgServerConnctFailed(net.resultStCd);
			if (monitor != null)
			{
				monitor.EndMonitorForward(msg, callbackObject, "ServerGetLeaderboardEntries_Failed");
			}
		}
		if (monitor != null)
		{
			monitor.EndMonitorBackward();
		}
	}
}
