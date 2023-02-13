using Message;
using System.Collections;
using UnityEngine;

public class ServerGetInformation
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
		ServerNoticeInfo instance = ServerInterface.NoticeInfo;
		NetServerGetNoticeInfo net = new NetServerGetNoticeInfo();
		net.Request();
		monitor.StartMonitor(new ServerGetInformationRetry(callbackObject));
		while (net.IsExecuting())
		{
			yield return null;
		}
		instance.Clear();
		if (net.IsSucceeded())
		{
			instance.m_isGetNoticeInfo = true;
			instance.LastUpdateInfoTime = NetUtil.GetCurrentTime();
			int num = net.GetInfoCount();
			for (int i = 0; i < num; i++)
			{
				NetNoticeItem info = net.GetInfo(i);
				if (NetUtil.IsServerTimeWithinPeriod(info.Start, info.End))
				{
					if (info.WindowType == 14)
					{
						instance.m_rouletteItems.Add(info);
					}
					else if (info.WindowType == 15)
					{
						instance.m_eventItems.Add(info);
					}
					else
					{
						instance.m_noticeItems.Add(info);
					}
				}
			}
			if (instance.m_noticeItems.Count > 1)
			{
				instance.m_noticeItems.Sort(PriorityComparer);
			}
			MsgGetInformationSucceed msg2 = new MsgGetInformationSucceed
			{
				m_information = instance
			};
			if (callbackObject != null)
			{
				callbackObject.SendMessage("ServerGetInformation_Succeeded", msg2, SendMessageOptions.DontRequireReceiver);
			}
		}
		else
		{
			MsgServerConnctFailed msg = new MsgServerConnctFailed(net.resultStCd);
			if (monitor != null)
			{
				monitor.EndMonitorForward(msg, callbackObject, "ServerGetInformation_Failed");
			}
		}
		if (monitor != null)
		{
			monitor.EndMonitorBackward();
		}
	}

	private static int PriorityComparer(NetNoticeItem itemA, NetNoticeItem itemB)
	{
		if (itemA != null && itemB != null)
		{
			if (itemA.Id >= NetNoticeItem.OPERATORINFO_START_ID)
			{
				if (itemB.Id >= NetNoticeItem.OPERATORINFO_START_ID)
				{
					return itemA.Priority - itemB.Priority;
				}
				return 1;
			}
			if (itemB.Id >= NetNoticeItem.OPERATORINFO_START_ID)
			{
				return -1;
			}
			return itemA.Priority - itemB.Priority;
		}
		return 0;
	}
}
