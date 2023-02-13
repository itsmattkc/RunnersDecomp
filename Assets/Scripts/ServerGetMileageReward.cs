using Message;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ServerGetMileageReward
{
	public static IEnumerator Process(int episode, int chapter, GameObject callbackObject)
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
		NetServerGetMileageReward net = new NetServerGetMileageReward(episode, chapter);
		net.Request();
		monitor.StartMonitor(new ServerGetMileageRewardRetry(episode, chapter, callbackObject));
		while (net.IsExecuting())
		{
			yield return null;
		}
		if (net.IsSucceeded())
		{
			List<ServerMileageReward> mileageRewardList = ServerInterface.MileageRewardList;
			List<ServerMileageReward> allList = new List<ServerMileageReward>();
			foreach (ServerMileageReward reward2 in mileageRewardList)
			{
				allList.Add(reward2);
			}
			if (net.m_rewardList != null)
			{
				mileageRewardList.Clear();
				foreach (ServerMileageReward reward in net.m_rewardList)
				{
					if (!mileageRewardList.Contains(reward))
					{
						mileageRewardList.Add(reward);
					}
					if (!allList.Contains(reward))
					{
						allList.Add(reward);
					}
				}
			}
			MsgGetMileageRewardSucceed msg2 = new MsgGetMileageRewardSucceed
			{
				m_mileageRewardList = allList
			};
			if (monitor != null)
			{
				monitor.EndMonitorForward(msg2, callbackObject, "ServerGetMileageReward_Succeeded");
			}
			if (callbackObject != null)
			{
				callbackObject.SendMessage("ServerGetMileageReward_Succeeded", msg2, SendMessageOptions.DontRequireReceiver);
			}
		}
		else
		{
			MsgServerConnctFailed msg = new MsgServerConnctFailed(net.resultStCd);
			if (monitor != null)
			{
				monitor.EndMonitorForward(msg, callbackObject, "ServerGetMileageReward_Failed");
			}
		}
		if (monitor != null)
		{
			monitor.EndMonitorBackward();
		}
	}
}
