using Message;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ServerGetTicker
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
		List<ServerTickerData> instance = ServerInterface.TickerInfo.Data;
		NetServerGetTicker net = new NetServerGetTicker();
		net.Request();
		monitor.StartMonitor(new ServerGetTickerRetry(callbackObject));
		while (net.IsExecuting())
		{
			yield return null;
		}
		instance.Clear();
		if (net.IsSucceeded())
		{
			int num = net.GetInfoCount();
			for (int i = 0; i < num; i++)
			{
				ServerTickerData info = net.GetInfo(i);
				if (NetUtil.IsServerTimeWithinPeriod(info.Start, info.End))
				{
					instance.Add(info);
				}
			}
			MsgGetTickerDataSucceed msg2 = new MsgGetTickerDataSucceed
			{
				m_tickerData = instance
			};
			if (callbackObject != null)
			{
				callbackObject.SendMessage("ServerGetTicker_Succeeded", msg2, SendMessageOptions.DontRequireReceiver);
			}
		}
		else
		{
			MsgServerConnctFailed msg = new MsgServerConnctFailed(net.resultStCd);
			if (monitor != null)
			{
				monitor.EndMonitorForward(msg, callbackObject, "ServerGetTicker_Failed");
			}
		}
		if (monitor != null)
		{
			monitor.EndMonitorBackward();
		}
	}
}
