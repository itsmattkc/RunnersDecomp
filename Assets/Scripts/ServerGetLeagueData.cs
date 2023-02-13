using Message;
using System.Collections;
using UnityEngine;

public class ServerGetLeagueData
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
		NetServerGetLeagueData net = new NetServerGetLeagueData(mode);
		net.Request();
		monitor.StartMonitor(new ServerGetLeagueDataRetry(mode, callbackObject));
		while (net.IsExecuting())
		{
			yield return null;
		}
		if (net.IsSucceeded())
		{
			ServerLeagueData resultLeagueData = net.leagueData;
			if (resultLeagueData == null)
			{
				resultLeagueData = new ServerLeagueData();
			}
			if (SingletonGameObject<RankingManager>.Instance != null)
			{
				SingletonGameObject<RankingManager>.Instance.SetLeagueData(resultLeagueData);
			}
			MsgGetLeagueDataSucceed msg2 = new MsgGetLeagueDataSucceed();
			if (monitor != null)
			{
				monitor.EndMonitorForward(msg2, callbackObject, "ServerGetLeagueData_Succeeded");
			}
			if (callbackObject != null)
			{
				callbackObject.SendMessage("ServerGetLeagueData_Succeeded", msg2, SendMessageOptions.DontRequireReceiver);
			}
		}
		else
		{
			MsgServerConnctFailed msg = new MsgServerConnctFailed(net.resultStCd);
			if (monitor != null)
			{
				monitor.EndMonitorForward(msg, callbackObject, "ServerGetLeagueData_Failed");
			}
		}
		if (monitor != null)
		{
			monitor.EndMonitorBackward();
		}
	}
}
