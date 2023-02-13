using Message;
using System.Collections;
using UnityEngine;

public class ServerGetDailyMissionData
{
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
		NetServerGetDailyMissionData net = new NetServerGetDailyMissionData();
		net.Request();
		monitor.StartMonitor(new ServerGetDailyMissionDataRetry(callbackObject));
		while (net.IsExecuting())
		{
			yield return null;
		}
		if (net.IsSucceeded())
		{
			int numContinue = net.resultNumContinue;
			int numIncentive = net.resultIncentives;
			ServerDailyChallengeState dailyIncentive = ServerInterface.DailyChallengeState;
			dailyIncentive.m_incentiveList.Clear();
			for (int i = 0; i < numIncentive; i++)
			{
				ServerDailyChallengeIncentive incentive = net.GetResultDailyMissionIncentive(i);
				dailyIncentive.m_incentiveList.Add(incentive);
			}
			dailyIncentive.m_numIncentiveCont = numContinue;
			dailyIncentive.m_numDailyChalDay = net.resultNumDailyChalDay;
			dailyIncentive.m_maxDailyChalDay = net.resultMaxDailyChalDay;
			dailyIncentive.m_maxIncentive = net.resultMaxIncentive;
			dailyIncentive.m_chalEndTime = net.resultChalEndTime;
			NetUtil.SyncSaveDataAndDailyMission(dailyIncentive);
			MsgGetDailyMissionDataSucceed msg2 = new MsgGetDailyMissionDataSucceed
			{
				m_dailyChallengeState = dailyIncentive
			};
			if (monitor != null)
			{
				monitor.EndMonitorForward(msg2, callbackObject, "ServerGetDailyMissionData_Succeeded");
			}
			if (callbackObject != null)
			{
				callbackObject.SendMessage("ServerGetDailyMissionData_Succeeded", msg2, SendMessageOptions.DontRequireReceiver);
			}
		}
		else
		{
			MsgServerConnctFailed msg = new MsgServerConnctFailed(net.resultStCd);
			if (monitor != null)
			{
				monitor.EndMonitorForward(msg, callbackObject, "ServerGetDailyMissionData_Failed");
			}
		}
		if (monitor != null)
		{
			monitor.EndMonitorBackward();
		}
	}
}
