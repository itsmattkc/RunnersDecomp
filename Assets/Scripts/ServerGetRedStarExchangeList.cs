using Message;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ServerGetRedStarExchangeList
{
	public static IEnumerator Process(int itemType, GameObject callbackObject)
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
		NetServerGetRedStarExchangeList net = new NetServerGetRedStarExchangeList(itemType);
		net.Request();
		monitor.StartMonitor(new ServerGetRedStarExchangeListRetry(itemType, callbackObject));
		while (net.IsExecuting())
		{
			yield return null;
		}
		if (net.IsSucceeded())
		{
			int resultItems = net.resultItems;
			List<ServerRedStarItemState> exchangeList = new List<ServerRedStarItemState>(net.resultItems);
			List<ServerRedStarItemState> serverExachangeList = null;
			switch (itemType)
			{
			case 0:
			{
				serverExachangeList = ServerInterface.RedStarItemList;
				ServerSettingState settingState = ServerInterface.SettingState;
				if (settingState != null)
				{
					settingState.m_birthday = net.resultBirthDay;
					settingState.m_monthPurchase = net.resultMonthPurchase;
				}
				break;
			}
			case 1:
				serverExachangeList = ServerInterface.RedStarExchangeRingItemList;
				break;
			case 2:
				serverExachangeList = ServerInterface.RedStarExchangeEnergyItemList;
				break;
			case 4:
				serverExachangeList = ServerInterface.RedStarExchangeRaidbossEnergyItemList;
				break;
			}
			if (serverExachangeList != null)
			{
				serverExachangeList.Clear();
			}
			for (int i = 0; i < resultItems; i++)
			{
				ServerRedStarItemState result = net.GetResultRedStarItemState(i);
				if (result != null)
				{
					ServerRedStarItemState item = new ServerRedStarItemState();
					result.CopyTo(item);
					exchangeList.Add(item);
					if (serverExachangeList != null)
					{
						serverExachangeList.Add(item);
					}
				}
			}
			MsgGetRedStarExchangeListSucceed msg2 = new MsgGetRedStarExchangeListSucceed
			{
				m_itemList = exchangeList,
				m_totalItems = resultItems
			};
			if (monitor != null)
			{
				monitor.EndMonitorForward(msg2, callbackObject, "ServerGetRedStarExchangeList_Succeeded");
			}
			if (callbackObject != null)
			{
				callbackObject.SendMessage("ServerGetRedStarExchangeList_Succeeded", msg2, SendMessageOptions.DontRequireReceiver);
			}
		}
		else
		{
			MsgServerConnctFailed msg = new MsgServerConnctFailed(net.resultStCd);
			if (monitor != null)
			{
				monitor.EndMonitorForward(msg, callbackObject, "ServerGetRedStarExchangeList_Failed");
			}
		}
		if (monitor != null)
		{
			monitor.EndMonitorBackward();
		}
	}
}
