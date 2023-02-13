using Message;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ServerGetMenuData
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
		NetServerGetMenuData net = new NetServerGetMenuData();
		net.Request();
		monitor.StartMonitor(new ServerGetMenuDataRetry(callbackObject));
		while (net.IsExecuting())
		{
			yield return null;
		}
		if (net.IsSucceeded())
		{
			SetPlayerState(net.PlayerState);
			SetWheelOptions(net.WheelOptions);
			SetChaoWheelOptions(net.ChaoWheelOptions);
			SetSubCharaRingPayment(net.SubCharaRingExchange);
			SetDailyChallenge(net.DailyChallengeState);
			SetMileageMap(net.MileageMapState);
			SetMessage(net.MessageEntryList, net.TotalMessage);
			SetOperatorMessage(net.OperatorMessageEntryList, net.TotalOperatorMessage);
			SetRedStarExchangeList(ServerInterface.RedStarItemList, net.RedstarItemStateList, net.RedstarTotalItems);
			SetRedStarExchangeList(ServerInterface.RedStarExchangeRingItemList, net.RingItemStateList, net.RingTotalItems);
			SetRedStarExchangeList(ServerInterface.RedStarExchangeEnergyItemList, net.EnergyItemStateList, net.EnergyTotalItems);
			SetMonthPurchase(net.MonthPurchase);
			SetBirthday(net.BirthDay);
			SetConsumedCostList(net.ConsumedCostList);
			MsgGetMenuDataSucceed msg2 = new MsgGetMenuDataSucceed();
			if (monitor != null)
			{
				monitor.EndMonitorForward(msg2, callbackObject, "ServerGetMenuData_Succeeded");
			}
			if (callbackObject != null)
			{
				callbackObject.SendMessage("ServerGetMenuData_Succeeded", msg2, SendMessageOptions.DontRequireReceiver);
			}
		}
		else
		{
			MsgServerConnctFailed msg = new MsgServerConnctFailed(net.resultStCd);
			if (monitor != null)
			{
				monitor.EndMonitorForward(msg, callbackObject, "ServerGetMenuData_Failed");
			}
		}
		if (monitor != null)
		{
			monitor.EndMonitorBackward();
		}
	}

	private static void SetPlayerState(ServerPlayerState resultPlayerState)
	{
		if (resultPlayerState != null)
		{
			ServerPlayerState playerState = ServerInterface.PlayerState;
			if (playerState != null)
			{
				resultPlayerState.CopyTo(playerState);
			}
		}
	}

	private static void SetWheelOptions(ServerWheelOptions resultWheelOptions)
	{
		if (resultWheelOptions != null)
		{
			ServerWheelOptions wheelOptions = ServerInterface.WheelOptions;
			if (wheelOptions != null)
			{
				resultWheelOptions.CopyTo(wheelOptions);
			}
		}
	}

	private static void SetChaoWheelOptions(ServerChaoWheelOptions resultChaoWheelOptions)
	{
		if (resultChaoWheelOptions != null)
		{
			ServerChaoWheelOptions chaoWheelOptions = ServerInterface.ChaoWheelOptions;
			if (chaoWheelOptions != null)
			{
				resultChaoWheelOptions.CopyTo(chaoWheelOptions);
			}
		}
	}

	private static void SetSubCharaRingPayment(int subCharaRingPayment)
	{
		ServerSettingState settingState = ServerInterface.SettingState;
		if (settingState != null)
		{
			settingState.m_subCharaRingPayment = subCharaRingPayment;
		}
	}

	private static void SetDailyChallenge(ServerDailyChallengeState dailyChallengeState)
	{
		if (dailyChallengeState == null)
		{
			return;
		}
		int count = dailyChallengeState.m_incentiveList.Count;
		ServerDailyChallengeState dailyChallengeState2 = ServerInterface.DailyChallengeState;
		if (dailyChallengeState2 != null)
		{
			dailyChallengeState2.m_incentiveList.Clear();
			for (int i = 0; i < count; i++)
			{
				dailyChallengeState2.m_incentiveList.Add(dailyChallengeState.m_incentiveList[i]);
			}
			dailyChallengeState2.m_numIncentiveCont = dailyChallengeState.m_numIncentiveCont;
			dailyChallengeState2.m_numDailyChalDay = dailyChallengeState.m_numDailyChalDay;
			dailyChallengeState2.m_maxDailyChalDay = dailyChallengeState.m_maxDailyChalDay;
			dailyChallengeState2.m_maxIncentive = dailyChallengeState.m_maxIncentive;
			NetUtil.SyncSaveDataAndDailyMission(dailyChallengeState2);
		}
	}

	private static void SetMileageMap(ServerMileageMapState resultMileageMapState)
	{
		if (resultMileageMapState != null)
		{
			ServerMileageMapState mileageMapState = ServerInterface.MileageMapState;
			if (mileageMapState != null)
			{
				resultMileageMapState.CopyTo(mileageMapState);
			}
		}
	}

	private static void SetMessage(List<ServerMessageEntry> resultMessageList, int totalMessage)
	{
		if (resultMessageList != null)
		{
			List<ServerMessageEntry> messageList = ServerInterface.MessageList;
			messageList.Clear();
			for (int i = 0; i < totalMessage; i++)
			{
				ServerMessageEntry item = resultMessageList[i];
				messageList.Add(item);
			}
		}
	}

	private static void SetOperatorMessage(List<ServerOperatorMessageEntry> resultMessageList, int totalMessage)
	{
		if (resultMessageList != null)
		{
			List<ServerOperatorMessageEntry> operatorMessageList = ServerInterface.OperatorMessageList;
			operatorMessageList.Clear();
			for (int i = 0; i < totalMessage; i++)
			{
				ServerOperatorMessageEntry item = resultMessageList[i];
				operatorMessageList.Add(item);
			}
		}
	}

	private static void SetRedStarExchangeList(List<ServerRedStarItemState> serverExachangeList, List<ServerRedStarItemState> exchangeList, int resultItems)
	{
		if (exchangeList == null || serverExachangeList == null)
		{
			return;
		}
		serverExachangeList.Clear();
		for (int i = 0; i < resultItems; i++)
		{
			ServerRedStarItemState serverRedStarItemState = exchangeList[i];
			if (serverRedStarItemState != null)
			{
				ServerRedStarItemState serverRedStarItemState2 = new ServerRedStarItemState();
				serverRedStarItemState.CopyTo(serverRedStarItemState2);
				if (serverExachangeList != null)
				{
					serverExachangeList.Add(serverRedStarItemState2);
				}
			}
		}
	}

	private static void SetMonthPurchase(int monthPurchase)
	{
		ServerSettingState settingState = ServerInterface.SettingState;
		if (settingState != null)
		{
			settingState.m_monthPurchase = monthPurchase;
		}
	}

	private static void SetBirthday(string birthday)
	{
		ServerSettingState settingState = ServerInterface.SettingState;
		if (settingState != null)
		{
			settingState.m_birthday = birthday;
		}
	}

	private static void SetConsumedCostList(List<ServerConsumedCostData> serverConsumedCostList)
	{
		List<ServerConsumedCostData> consumedCostList = ServerInterface.ConsumedCostList;
		if (consumedCostList == null)
		{
			return;
		}
		consumedCostList.Clear();
		foreach (ServerConsumedCostData serverConsumedCost in serverConsumedCostList)
		{
			if (serverConsumedCost != null)
			{
				ServerConsumedCostData serverConsumedCostData = new ServerConsumedCostData();
				serverConsumedCost.CopyTo(serverConsumedCostData);
				consumedCostList.Add(serverConsumedCostData);
			}
		}
	}
}
