using Message;
using System.Collections;
using UnityEngine;

public class ServerLoginBonusSelect
{
	public static IEnumerator Process(int rewardId, int rewardDays, int rewardSelect, int firstRewardDays, int firstRewardSelect, GameObject callbackObject)
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
		NetServerLoginBonusSelect net = new NetServerLoginBonusSelect(rewardId, rewardDays, rewardSelect, firstRewardDays, firstRewardSelect);
		net.Request();
		monitor.StartMonitor(new ServerLoginBonusSelectRetry(rewardId, rewardDays, rewardSelect, firstRewardDays, firstRewardSelect, callbackObject));
		while (net.IsExecuting())
		{
			yield return null;
		}
		if (net.IsSucceeded())
		{
			ServerLoginBonusData loginBonusData = ServerInterface.LoginBonusData;
			if (loginBonusData != null)
			{
				loginBonusData.setLoginBonusList(net.loginBonusReward, net.firstLoginBonusReward);
			}
			if (net.loginBonusReward != null)
			{
				loginBonusData.m_loginBonusState.m_numBonus++;
				loginBonusData.m_loginBonusState.m_numLogin++;
			}
			MsgLoginBonusSelectSucceed msg2 = new MsgLoginBonusSelectSucceed
			{
				m_loginBonusReward = net.loginBonusReward,
				m_firstLoginBonusReward = net.firstLoginBonusReward
			};
			if (monitor != null)
			{
				monitor.EndMonitorForward(msg2, callbackObject, "ServerLoginBonusSelect_Succeeded");
			}
			if (callbackObject != null)
			{
				callbackObject.SendMessage("ServerLoginBonusSelect_Succeeded", msg2, SendMessageOptions.DontRequireReceiver);
			}
		}
		else
		{
			MsgServerConnctFailed msg = new MsgServerConnctFailed(net.resultStCd);
			if (monitor != null)
			{
				monitor.EndMonitorForward(msg, callbackObject, "ServerLoginBonusSelect_Failed");
			}
		}
		if (monitor != null)
		{
			monitor.EndMonitorBackward();
		}
	}
}
