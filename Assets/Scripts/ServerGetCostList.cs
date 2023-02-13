using Message;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ServerGetCostList
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
		NetServerGetCostList net = new NetServerGetCostList();
		net.Request();
		monitor.StartMonitor(new ServerGetCostListRetry(callbackObject));
		while (net.IsExecuting())
		{
			yield return null;
		}
		if (net.IsSucceeded())
		{
			List<ServerConsumedCostData> costData = ServerInterface.CostList;
			if (costData != null)
			{
				costData.Clear();
				List<ServerConsumedCostData> resultCostData = net.resultCostList;
				if (resultCostData != null)
				{
					foreach (ServerConsumedCostData data in resultCostData)
					{
						if (data != null)
						{
							costData.Add(data);
						}
					}
				}
			}
			MsgGetCostListSucceed msg2 = new MsgGetCostListSucceed
			{
				m_costList = costData
			};
			if (monitor != null)
			{
				monitor.EndMonitorForward(msg2, callbackObject, "GetCostList_Succeeded");
			}
			if (callbackObject != null)
			{
				callbackObject.SendMessage("GetCostList_Succeeded", msg2, SendMessageOptions.DontRequireReceiver);
			}
		}
		else
		{
			MsgServerConnctFailed msg = new MsgServerConnctFailed(net.resultStCd);
			if (monitor != null)
			{
				monitor.EndMonitorForward(msg, callbackObject, "GetCostList_Failed");
			}
		}
		if (monitor != null)
		{
			monitor.EndMonitorBackward();
		}
	}
}
