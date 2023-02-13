using Message;
using System.Collections;
using UnityEngine;

public class ServerLoginBonus
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
		NetServerLoginBonus net = new NetServerLoginBonus();
		net.Request();
		monitor.StartMonitor(new ServerLoginBonusRetry(callbackObject));
		while (net.IsExecuting())
		{
			yield return null;
		}
		if (net.IsSucceeded())
		{
			ServerLoginBonusData loginBonusData = ServerInterface.LoginBonusData;
			net.loginBonusState.CopyTo(loginBonusData.m_loginBonusState);
			loginBonusData.m_startTime = net.startTime;
			loginBonusData.m_endTime = net.endTime;
			foreach (ServerLoginBonusReward data2 in net.loginBonusRewardList)
			{
				loginBonusData.m_loginBonusRewardList.Add(data2);
			}
			foreach (ServerLoginBonusReward data in net.firstLoginBonusRewardList)
			{
				loginBonusData.m_firstLoginBonusRewardList.Add(data);
			}
			loginBonusData.m_rewardId = net.rewardId;
			loginBonusData.m_rewardDays = net.rewardDays;
			loginBonusData.m_firstRewardDays = net.firstRewardDays;
			MsgLoginBonusSucceed msg2 = new MsgLoginBonusSucceed
			{
				m_loginBonusData = loginBonusData
			};
			if (monitor != null)
			{
				monitor.EndMonitorForward(msg2, callbackObject, "ServerLoginBonus_Succeeded");
			}
			if (callbackObject != null)
			{
				callbackObject.SendMessage("ServerLoginBonus_Succeeded", msg2, SendMessageOptions.DontRequireReceiver);
			}
		}
		else
		{
			MsgServerConnctFailed msg = new MsgServerConnctFailed(net.resultStCd);
			if (monitor != null)
			{
				monitor.EndMonitorForward(msg, callbackObject, "ServerLoginBonus_Failed");
			}
		}
		if (monitor != null)
		{
			monitor.EndMonitorBackward();
		}
	}
}
