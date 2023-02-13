using Message;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ServerGetItemStockNum
{
	public static IEnumerator Process(int eventId, List<int> itemId, GameObject callbackObject)
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
		NetServerGetItemStockNum net = new NetServerGetItemStockNum(eventId, itemId);
		net.Request();
		monitor.StartMonitor(new ServerGetItemStockNumRetry(eventId, itemId, callbackObject));
		while (net.IsExecuting())
		{
			yield return null;
		}
		if (net.IsSucceeded())
		{
			List<ServerItemState> itemStockList = net.m_itemStockList;
			if (itemStockList != null && itemStockList.Count > 0)
			{
				foreach (ServerItemState item in itemStockList)
				{
					GeneralUtil.SetItemCount((ServerItem.Id)item.m_itemId, item.m_num);
				}
			}
			MsgGetItemStockNumSucceed msg2 = new MsgGetItemStockNumSucceed
			{
				m_itemStockList = itemStockList
			};
			if (monitor != null)
			{
				monitor.EndMonitorForward(msg2, callbackObject, "ServerGetItemStockNum_Succeeded");
			}
			if (callbackObject != null)
			{
				callbackObject.SendMessage("ServerGetItemStockNum_Succeeded", msg2, SendMessageOptions.DontRequireReceiver);
			}
		}
		else
		{
			MsgServerConnctFailed msg = new MsgServerConnctFailed(net.resultStCd);
			if (monitor != null)
			{
				monitor.EndMonitorForward(msg, callbackObject, "ServerGetItemStockNum_Failed");
			}
		}
		if (monitor != null)
		{
			monitor.EndMonitorBackward();
		}
	}
}
