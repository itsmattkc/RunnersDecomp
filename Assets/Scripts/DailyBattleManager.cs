using Message;
using System;
using System.Collections.Generic;
using UnityEngine;

public class DailyBattleManager : SingletonGameObject<DailyBattleManager>
{
	public enum REQ_TYPE
	{
		GET_STATUS,
		UPD_STATUS,
		POST_RESULT,
		GET_PRIZE,
		GET_DATA,
		GET_DATA_HISTORY,
		RESET_MATCHING,
		NUM
	}

	public enum DATA_TYPE
	{
		STATUS,
		DATA_PAIR,
		DATA_PAIR_LIST,
		PRIZE_LIST,
		END_TIME,
		REWARD_FLAG,
		REWARD_DATA_PAIR,
		NUM
	}

	public delegate void CallbackGetStatus(ServerDailyBattleStatus status, DateTime endTime, bool rewardFlag, ServerDailyBattleDataPair rewardDataPair);

	public delegate void CallbackPostResult(ServerDailyBattleDataPair dataPair, bool rewardFlag, ServerDailyBattleDataPair rewardDataPair);

	public delegate void CallbackGetData(ServerDailyBattleDataPair dataPair);

	public delegate void CallbackGetDataHistory(List<ServerDailyBattleDataPair> dataPairList);

	public delegate void CallbackGetPrize(List<ServerDailyBattlePrizeData> prizeList);

	public delegate void CallbackResetMatching(ServerPlayerState playerStatus, ServerDailyBattleDataPair dataPair, DateTime endTime);

	public delegate void CallbackSetupInfo(ServerDailyBattleStatus status, ServerDailyBattleDataPair dataPair, DateTime endTime, bool rewardFlag, int winFlag);

	public delegate void CallbackSetup();

	private const int REQUEST_LOAD_DELAY = 2;

	private const int REQUEST_RELOAD_DELAY = 60;

	private const int DATA_AUTO_RELOAD_TIME = 600;

	[SerializeField]
	private bool m_showLog;

	private CallbackGetStatus m_callbackGetStatus;

	private CallbackPostResult m_callbackPostResult;

	private CallbackGetData m_callbackGetData;

	private CallbackGetDataHistory m_callbackGetDataHistory;

	private CallbackGetPrize m_callbackGetPrize;

	private CallbackResetMatching m_callbackResetMatching;

	private CallbackSetupInfo m_callbackSetupInfo;

	private CallbackSetup m_callbackSetup;

	private Dictionary<REQ_TYPE, bool> m_isRequestList;

	private Dictionary<REQ_TYPE, DateTime> m_requestTimeList;

	private bool m_isDataInit;

	private bool m_isFirstSetupReq;

	private bool m_isResultSetupReq;

	private bool m_isDispUpdate;

	private DateTime m_dataInitTime;

	private ServerDailyBattleStatus m_currentStatus;

	private ServerDailyBattleDataPair m_currentDataPair;

	private List<ServerDailyBattleDataPair> m_currentDataPairList;

	private List<ServerDailyBattlePrizeData> m_currentPrizeList;

	private DateTime m_currentEndTime;

	private bool m_currentRewardFlag;

	private ServerDailyBattleDataPair m_currentRewardDataPair;

	private Dictionary<int, ServerConsumedCostData> m_resetCostList;

	private Dictionary<REQ_TYPE, int> m_chainRequestList;

	private List<REQ_TYPE> m_chainRequestKeys;

	private Dictionary<DATA_TYPE, DateTime> m_currentDataLastUpdateTimeList;

	public ServerDailyBattleStatus currentStatus
	{
		get
		{
			return m_currentStatus;
		}
	}

	public ServerDailyBattleDataPair currentDataPair
	{
		get
		{
			return m_currentDataPair;
		}
	}

	public List<ServerDailyBattleDataPair> currentDataPairList
	{
		get
		{
			return m_currentDataPairList;
		}
	}

	public List<ServerDailyBattlePrizeData> currentPrizeList
	{
		get
		{
			return m_currentPrizeList;
		}
	}

	public DateTime currentEndTime
	{
		get
		{
			return m_currentEndTime;
		}
	}

	public bool currentRewardFlag
	{
		get
		{
			return m_currentRewardFlag;
		}
	}

	public int currentWinFlag
	{
		get
		{
			int result = 0;
			if (m_currentDataPair != null && m_currentDataPair.myBattleData != null && !string.IsNullOrEmpty(m_currentDataPair.myBattleData.userId))
			{
				result = ((m_currentDataPair.rivalBattleData == null || string.IsNullOrEmpty(m_currentDataPair.rivalBattleData.userId)) ? 4 : ((m_currentDataPair.myBattleData.maxScore > m_currentDataPair.rivalBattleData.maxScore) ? 3 : ((m_currentDataPair.myBattleData.maxScore != m_currentDataPair.rivalBattleData.maxScore) ? 1 : 2)));
			}
			return result;
		}
	}

	public Dictionary<int, ServerConsumedCostData> resetCostList
	{
		get
		{
			if (m_resetCostList == null)
			{
				List<ServerConsumedCostData> costList = ServerInterface.CostList;
				if (costList != null)
				{
					m_resetCostList = new Dictionary<int, ServerConsumedCostData>();
					foreach (ServerConsumedCostData item in costList)
					{
						switch (item.consumedItemId)
						{
						case 980000:
							if (!m_resetCostList.ContainsKey(0))
							{
								m_resetCostList.Add(0, item);
							}
							break;
						case 980001:
							if (!m_resetCostList.ContainsKey(1))
							{
								m_resetCostList.Add(1, item);
							}
							break;
						case 980002:
							if (!m_resetCostList.ContainsKey(2))
							{
								m_resetCostList.Add(2, item);
							}
							break;
						}
					}
				}
			}
			return m_resetCostList;
		}
	}

	public bool isLoading
	{
		get
		{
			if (m_isFirstSetupReq || m_isResultSetupReq)
			{
				return true;
			}
			return false;
		}
	}

	public static bool isDailyBattleDispUpdateFlag
	{
		get
		{
			if (SingletonGameObject<DailyBattleManager>.Instance != null)
			{
				return SingletonGameObject<DailyBattleManager>.Instance.isDispUpdateFlag;
			}
			return false;
		}
		set
		{
			if (SingletonGameObject<DailyBattleManager>.Instance != null)
			{
				SingletonGameObject<DailyBattleManager>.Instance.isDispUpdateFlag = value;
			}
		}
	}

	public bool isDispUpdateFlag
	{
		get
		{
			return m_isDispUpdate;
		}
		set
		{
			if (!value)
			{
				m_isDispUpdate = value;
			}
		}
	}

	public TimeSpan GetLimitTimeSpan()
	{
		return m_currentEndTime - NetBase.GetCurrentTime();
	}

	private void Init()
	{
		m_dataInitTime = NetBase.GetCurrentTime();
		m_isFirstSetupReq = false;
		m_isResultSetupReq = false;
		m_isDispUpdate = false;
		DateTime currentEndTime = m_dataInitTime.AddHours(1.0);
		m_currentStatus = null;
		m_currentDataPair = null;
		m_currentDataPairList = null;
		m_currentPrizeList = null;
		m_currentEndTime = currentEndTime;
		if (m_chainRequestList != null)
		{
			m_chainRequestList.Clear();
		}
		if (m_chainRequestKeys != null)
		{
			m_chainRequestKeys.Clear();
		}
		DateTime time = m_dataInitTime.AddSeconds(-606.0);
		for (int i = 0; i < 7; i++)
		{
			UpdateDataLastTime((DATA_TYPE)i, time);
		}
		m_isDataInit = true;
	}

	public ServerDailyBattleDataPair GetRewardDataPair(bool reset = false)
	{
		ServerDailyBattleDataPair result = null;
		if (m_currentRewardFlag && m_currentRewardDataPair != null)
		{
			result = new ServerDailyBattleDataPair(m_currentRewardDataPair);
			if (reset)
			{
				m_currentRewardDataPair = null;
				m_currentRewardFlag = false;
			}
		}
		return result;
	}

	public bool RestReward()
	{
		bool result = false;
		if (m_currentRewardFlag && m_currentRewardDataPair != null)
		{
			result = true;
		}
		m_currentRewardDataPair = null;
		m_currentRewardFlag = false;
		return result;
	}

	public void FirstSetup(CallbackSetupInfo callback)
	{
		Init();
		m_isFirstSetupReq = true;
		m_isDispUpdate = false;
		m_callbackSetup = null;
		m_callbackSetupInfo = callback;
		SetChainRequest(REQ_TYPE.UPD_STATUS, REQ_TYPE.GET_DATA);
	}

	public void FirstSetup(CallbackSetup callback)
	{
		Init();
		m_isFirstSetupReq = true;
		m_isDispUpdate = false;
		m_callbackSetup = callback;
		m_callbackSetupInfo = null;
		SetChainRequest(REQ_TYPE.UPD_STATUS, REQ_TYPE.GET_DATA);
	}

	public void FirstSetup()
	{
		Init();
		m_isFirstSetupReq = true;
		m_isDispUpdate = false;
		m_callbackSetup = null;
		m_callbackSetupInfo = null;
		SetChainRequest(REQ_TYPE.UPD_STATUS, REQ_TYPE.GET_DATA);
	}

	public void ResultSetup(CallbackSetupInfo callback)
	{
		m_isResultSetupReq = true;
		m_isDispUpdate = false;
		m_callbackSetup = null;
		m_callbackSetupInfo = callback;
		SetChainRequest(REQ_TYPE.POST_RESULT, REQ_TYPE.GET_STATUS);
	}

	public void ResultSetup(CallbackSetup callback)
	{
		m_isResultSetupReq = true;
		m_isDispUpdate = false;
		m_callbackSetup = callback;
		m_callbackSetupInfo = null;
		SetChainRequest(REQ_TYPE.POST_RESULT, REQ_TYPE.GET_STATUS);
	}

	public void ResultSetup()
	{
		m_isResultSetupReq = true;
		m_isDispUpdate = false;
		m_callbackSetup = null;
		m_callbackSetupInfo = null;
		SetChainRequest(REQ_TYPE.POST_RESULT, REQ_TYPE.GET_STATUS);
	}

	private bool CheckChainOrMultiRequestType(REQ_TYPE reqType)
	{
		bool result = true;
		if (reqType == REQ_TYPE.NUM || reqType == REQ_TYPE.GET_DATA_HISTORY || reqType == REQ_TYPE.RESET_MATCHING)
		{
			result = false;
		}
		return result;
	}

	private bool IsChainRequest(REQ_TYPE reqType)
	{
		bool result = false;
		if (m_chainRequestKeys != null && m_chainRequestList.Count > 0 && m_chainRequestList.ContainsKey(reqType) && m_chainRequestList[reqType] <= 1)
		{
			result = true;
		}
		return result;
	}

	private bool NextChainRequest()
	{
		bool flag = false;
		if (m_chainRequestKeys != null && m_chainRequestKeys.Count > 0)
		{
			int num = 0;
			for (int i = 0; i < m_chainRequestKeys.Count; i++)
			{
				REQ_TYPE rEQ_TYPE = m_chainRequestKeys[i];
				int num2 = m_chainRequestList[rEQ_TYPE];
				if (num2 >= 2)
				{
					num++;
				}
				if (num2 <= 0)
				{
					m_chainRequestList[rEQ_TYPE] = 1;
					Request(rEQ_TYPE);
					flag = true;
					break;
				}
			}
			if (!flag && num >= m_chainRequestKeys.Count)
			{
				if (m_isFirstSetupReq)
				{
					Debug.Log(" FirstSetup end !!!!!!!!!!!!!!!!!!!!!!!!!!!");
				}
				if (m_isResultSetupReq)
				{
					Debug.Log(" ResultSetup end !!!!!!!!!!!!!!!!!!!!!!!!!!!");
					if (currentDataPair != null && currentDataPair.myBattleData != null && currentDataPair.rivalBattleData != null && !string.IsNullOrEmpty(currentDataPair.myBattleData.userId))
					{
						Debug.Log(" ResultSetup end   starTime:" + currentDataPair.starDateString + " endTime:" + currentDataPair.endDateString);
					}
				}
				m_isDispUpdate = true;
				m_isResultSetupReq = false;
				m_isFirstSetupReq = false;
				m_chainRequestList.Clear();
				m_chainRequestKeys.Clear();
				if (m_callbackSetupInfo != null)
				{
					m_callbackSetupInfo(currentStatus, currentDataPair, currentEndTime, currentRewardFlag, currentWinFlag);
				}
				if (m_callbackSetup != null)
				{
					m_callbackSetup();
				}
			}
		}
		return flag;
	}

	private bool SetChainRequest(REQ_TYPE req0, REQ_TYPE req1)
	{
		List<REQ_TYPE> list = new List<REQ_TYPE>();
		list.Add(req0);
		list.Add(req1);
		return SetChainRequest(list);
	}

	private bool SetChainRequest(REQ_TYPE req0, REQ_TYPE req1, REQ_TYPE req2)
	{
		List<REQ_TYPE> list = new List<REQ_TYPE>();
		list.Add(req0);
		list.Add(req1);
		list.Add(req2);
		return SetChainRequest(list);
	}

	private bool SetChainRequest(List<REQ_TYPE> reqList)
	{
		if (reqList == null)
		{
			return false;
		}
		if (reqList.Count <= 0)
		{
			return false;
		}
		for (int i = 0; i < reqList.Count; i++)
		{
			if (!CheckChainOrMultiRequestType(reqList[i]))
			{
				return false;
			}
		}
		bool result = false;
		if (m_chainRequestList == null || m_chainRequestList.Count <= 0)
		{
			if (m_chainRequestList == null)
			{
				m_chainRequestList = new Dictionary<REQ_TYPE, int>();
			}
			if (m_chainRequestKeys == null)
			{
				m_chainRequestKeys = new List<REQ_TYPE>();
			}
			for (int j = 0; j < reqList.Count; j++)
			{
				m_chainRequestList.Add(reqList[j], 0);
				m_chainRequestKeys.Add(reqList[j]);
			}
			m_chainRequestList[m_chainRequestKeys[0]] = 1;
			Request(m_chainRequestKeys[0]);
			result = true;
		}
		return result;
	}

	private void SetCurrentStatus(ServerDailyBattleStatus status)
	{
		if (m_currentStatus == null)
		{
			m_currentStatus = new ServerDailyBattleStatus();
		}
		status.CopyTo(m_currentStatus);
		UpdateDataLastTime(DATA_TYPE.STATUS, NetBase.GetCurrentTime());
	}

	private void SetCurrentDataPair(ServerDailyBattleDataPair data)
	{
		if (m_currentDataPair == null)
		{
			m_currentDataPair = new ServerDailyBattleDataPair();
		}
		data.CopyTo(m_currentDataPair);
		UpdateDataLastTime(DATA_TYPE.DATA_PAIR, NetBase.GetCurrentTime());
	}

	private void SetCurrentDataPairList(List<ServerDailyBattleDataPair> list)
	{
		if (m_currentDataPairList == null)
		{
			m_currentDataPairList = new List<ServerDailyBattleDataPair>();
		}
		else
		{
			m_currentDataPairList.Clear();
		}
		if (list != null && list.Count > 0)
		{
			DateTime dateTime = NetBase.GetCurrentTime();
			TimeSpan t = list[0].endTime - list[0].starTime;
			for (int i = 0; i < list.Count; i++)
			{
				ServerDailyBattleDataPair serverDailyBattleDataPair = list[i];
				if (!serverDailyBattleDataPair.isToday)
				{
					ServerDailyBattleDataPair serverDailyBattleDataPair2 = new ServerDailyBattleDataPair();
					serverDailyBattleDataPair.CopyTo(serverDailyBattleDataPair2);
					if (dateTime.Ticks > serverDailyBattleDataPair2.endTime.Ticks && t.TotalSeconds > 0.0)
					{
						TimeSpan t2 = dateTime - serverDailyBattleDataPair2.endTime;
						if (t2 >= t)
						{
							Debug.Log(string.Empty + i + " span:" + t2.TotalHours + "h  currentEnd:" + dateTime.ToString() + " end:" + serverDailyBattleDataPair2.endTime);
							ServerDailyBattleDataPair item = new ServerDailyBattleDataPair(serverDailyBattleDataPair2.endTime, dateTime);
							m_currentDataPairList.Add(item);
						}
					}
					m_currentDataPairList.Add(serverDailyBattleDataPair2);
				}
				dateTime = serverDailyBattleDataPair.starTime;
			}
		}
		UpdateDataLastTime(DATA_TYPE.DATA_PAIR_LIST, NetBase.GetCurrentTime());
	}

	private void SetCurrentPrizeList(List<ServerDailyBattlePrizeData> list)
	{
		if (m_currentPrizeList == null)
		{
			m_currentPrizeList = new List<ServerDailyBattlePrizeData>();
		}
		else
		{
			m_currentPrizeList.Clear();
		}
		if (list != null && list.Count > 0)
		{
			foreach (ServerDailyBattlePrizeData item in list)
			{
				ServerDailyBattlePrizeData serverDailyBattlePrizeData = new ServerDailyBattlePrizeData();
				item.CopyTo(serverDailyBattlePrizeData);
				m_currentPrizeList.Add(serverDailyBattlePrizeData);
			}
		}
		UpdateDataLastTime(DATA_TYPE.PRIZE_LIST, NetBase.GetCurrentTime());
	}

	private void SetCurrentEndTime(DateTime time)
	{
		m_currentEndTime = time;
		UpdateDataLastTime(DATA_TYPE.END_TIME, NetBase.GetCurrentTime());
	}

	private void SetCurrentRewardFlag(bool flg)
	{
		if (flg || !m_currentRewardFlag)
		{
			m_currentRewardFlag = flg;
			UpdateDataLastTime(DATA_TYPE.REWARD_FLAG, NetBase.GetCurrentTime());
		}
	}

	private void SetCurrentRewardDataPair(ServerDailyBattleDataPair rewardDataPair)
	{
		if (rewardDataPair != null || m_currentRewardDataPair == null)
		{
			m_currentRewardDataPair = rewardDataPair;
			UpdateDataLastTime(DATA_TYPE.REWARD_DATA_PAIR, NetBase.GetCurrentTime());
		}
	}

	private void Update()
	{
		if (!m_isDataInit)
		{
			Init();
		}
	}

	private void UpdateDataLastTime(DATA_TYPE type, DateTime time)
	{
		if (m_currentDataLastUpdateTimeList == null)
		{
			m_currentDataLastUpdateTimeList = new Dictionary<DATA_TYPE, DateTime>();
		}
		if (m_currentDataLastUpdateTimeList.ContainsKey(type))
		{
			m_currentDataLastUpdateTimeList[type] = time;
		}
		else
		{
			m_currentDataLastUpdateTimeList.Add(type, time);
		}
	}

	public bool IsExpirationData(DATA_TYPE type)
	{
		bool result = false;
		if (m_currentDataLastUpdateTimeList == null || type == DATA_TYPE.NUM)
		{
			return false;
		}
		if (m_currentDataLastUpdateTimeList.ContainsKey(type) && !GeneralUtil.IsOverTimeMinute(m_currentDataLastUpdateTimeList[type], 3))
		{
			result = true;
		}
		return result;
	}

	private void StartRequest(REQ_TYPE type)
	{
		if (m_isRequestList == null)
		{
			m_isRequestList = new Dictionary<REQ_TYPE, bool>();
		}
		if (m_requestTimeList == null)
		{
			m_requestTimeList = new Dictionary<REQ_TYPE, DateTime>();
		}
		if (type != REQ_TYPE.NUM)
		{
			if (!m_isRequestList.ContainsKey(type))
			{
				m_isRequestList.Add(type, true);
			}
			else
			{
				m_isRequestList[type] = true;
			}
			if (!m_requestTimeList.ContainsKey(type))
			{
				m_requestTimeList.Add(type, NetBase.GetCurrentTime());
			}
			else
			{
				m_requestTimeList[type] = NetBase.GetCurrentTime();
			}
		}
	}

	private void EndRequest(REQ_TYPE type)
	{
		if (m_isRequestList == null)
		{
			m_isRequestList = new Dictionary<REQ_TYPE, bool>();
		}
		if (m_requestTimeList == null)
		{
			m_requestTimeList = new Dictionary<REQ_TYPE, DateTime>();
		}
		if (type != REQ_TYPE.NUM)
		{
			if (!m_isRequestList.ContainsKey(type))
			{
				m_isRequestList.Add(type, false);
			}
			else
			{
				m_isRequestList[type] = false;
			}
		}
	}

	private bool IsRequestPossible(REQ_TYPE type)
	{
		bool result = false;
		if (m_isRequestList == null || m_requestTimeList == null)
		{
			return true;
		}
		if (type != REQ_TYPE.NUM)
		{
			if (m_isRequestList.ContainsKey(type) && m_requestTimeList.ContainsKey(type))
			{
				if (!m_isRequestList[type])
				{
					if (type != REQ_TYPE.POST_RESULT && type != REQ_TYPE.RESET_MATCHING)
					{
						if (GeneralUtil.IsOverTimeSecond(m_requestTimeList[type], 2))
						{
							result = true;
						}
					}
					else
					{
						result = true;
					}
				}
				else if (type != REQ_TYPE.POST_RESULT && type != REQ_TYPE.RESET_MATCHING)
				{
					if (GeneralUtil.IsOverTimeSecond(m_requestTimeList[type], 60))
					{
						EndRequest(type);
						result = true;
					}
				}
				else
				{
					result = true;
				}
			}
			else
			{
				result = true;
			}
		}
		return result;
	}

	public bool IsDataReload(DATA_TYPE type)
	{
		bool result = true;
		if (m_currentDataLastUpdateTimeList != null && m_currentDataLastUpdateTimeList.ContainsKey(type))
		{
			DateTime baseTime = m_currentDataLastUpdateTimeList[type];
			if (!GeneralUtil.IsOverTimeSecond(baseTime, 600))
			{
				result = false;
				if (type != DATA_TYPE.END_TIME)
				{
					DateTime currentTime = NetBase.GetCurrentTime();
					DateTime currentEndTime = this.currentEndTime;
					if (currentTime.Ticks > currentEndTime.Ticks)
					{
						result = true;
					}
				}
			}
		}
		return result;
	}

	private bool Request(REQ_TYPE type)
	{
		bool result = false;
		if (CheckChainOrMultiRequestType(type))
		{
			result = true;
			switch (type)
			{
			case REQ_TYPE.GET_STATUS:
				RequestGetStatus(null);
				break;
			case REQ_TYPE.UPD_STATUS:
				RequestUpdateStatus(null);
				break;
			case REQ_TYPE.POST_RESULT:
				RequestPostResult(null);
				break;
			case REQ_TYPE.GET_PRIZE:
				RequestGetPrize(null);
				break;
			case REQ_TYPE.GET_DATA:
				RequestGetData(null);
				break;
			default:
				result = false;
				break;
			}
		}
		return result;
	}

	public bool RequestGetStatus(CallbackGetStatus callback)
	{
		if (IsRequestGetStatus())
		{
			StartRequest(REQ_TYPE.GET_STATUS);
			ServerInterface loggedInServerInterface = ServerInterface.LoggedInServerInterface;
			if (loggedInServerInterface != null)
			{
				m_callbackGetStatus = callback;
				loggedInServerInterface.RequestServerGetDailyBattleStatus(base.gameObject);
				return true;
			}
		}
		return false;
	}

	public bool IsRequestGetStatus()
	{
		ServerInterface loggedInServerInterface = ServerInterface.LoggedInServerInterface;
		if (loggedInServerInterface != null)
		{
			return IsRequestPossible(REQ_TYPE.GET_STATUS);
		}
		return false;
	}

	public bool RequestUpdateStatus(CallbackGetStatus callback)
	{
		if (IsRequestUpdateStatus())
		{
			StartRequest(REQ_TYPE.UPD_STATUS);
			ServerInterface loggedInServerInterface = ServerInterface.LoggedInServerInterface;
			if (loggedInServerInterface != null)
			{
				m_callbackGetStatus = callback;
				loggedInServerInterface.RequestServerUpdateDailyBattleStatus(base.gameObject);
				return true;
			}
		}
		return false;
	}

	public bool IsRequestUpdateStatus()
	{
		ServerInterface loggedInServerInterface = ServerInterface.LoggedInServerInterface;
		if (loggedInServerInterface != null)
		{
			return IsRequestPossible(REQ_TYPE.UPD_STATUS);
		}
		return false;
	}

	public bool RequestPostResult(CallbackPostResult callback)
	{
		if (IsRequestPostResult())
		{
			StartRequest(REQ_TYPE.POST_RESULT);
			ServerInterface loggedInServerInterface = ServerInterface.LoggedInServerInterface;
			if (loggedInServerInterface != null)
			{
				m_callbackPostResult = callback;
				loggedInServerInterface.RequestServerPostDailyBattleResult(base.gameObject);
				return true;
			}
		}
		return false;
	}

	public bool IsRequestPostResult()
	{
		ServerInterface loggedInServerInterface = ServerInterface.LoggedInServerInterface;
		if (loggedInServerInterface != null)
		{
			return IsRequestPossible(REQ_TYPE.POST_RESULT);
		}
		return false;
	}

	public bool RequestGetData(CallbackGetData callback)
	{
		if (IsRequestGetData())
		{
			StartRequest(REQ_TYPE.GET_DATA);
			ServerInterface loggedInServerInterface = ServerInterface.LoggedInServerInterface;
			if (loggedInServerInterface != null)
			{
				m_callbackGetData = callback;
				loggedInServerInterface.RequestServerGetDailyBattleData(base.gameObject);
				return true;
			}
		}
		return false;
	}

	public bool IsEndTimeOver()
	{
		if (NetBase.GetCurrentTime().Ticks >= m_currentEndTime.Ticks)
		{
			return true;
		}
		return false;
	}

	public bool IsReload(REQ_TYPE reqType, double waitMinutes = 1.0)
	{
		bool result = true;
		if (m_requestTimeList != null && m_requestTimeList.ContainsKey(reqType))
		{
			DateTime currentTime = NetBase.GetCurrentTime();
			DateTime d = m_requestTimeList[reqType];
			if ((currentTime - d).TotalMinutes < waitMinutes)
			{
				result = false;
			}
		}
		return result;
	}

	public bool IsDataInit(REQ_TYPE reqType)
	{
		bool result = false;
		if (m_requestTimeList != null && m_requestTimeList.ContainsKey(reqType))
		{
			result = true;
		}
		return result;
	}

	public bool IsRequestGetData()
	{
		ServerInterface loggedInServerInterface = ServerInterface.LoggedInServerInterface;
		if (loggedInServerInterface != null)
		{
			return IsRequestPossible(REQ_TYPE.GET_DATA);
		}
		return false;
	}

	public bool RequestGetPrize(CallbackGetPrize callback)
	{
		if (IsRequestGetPrize())
		{
			StartRequest(REQ_TYPE.GET_PRIZE);
			ServerInterface loggedInServerInterface = ServerInterface.LoggedInServerInterface;
			if (loggedInServerInterface != null)
			{
				m_callbackGetPrize = callback;
				loggedInServerInterface.RequestServerGetPrizeDailyBattle(base.gameObject);
				return true;
			}
		}
		return false;
	}

	public bool IsRequestGetPrize()
	{
		ServerInterface loggedInServerInterface = ServerInterface.LoggedInServerInterface;
		if (loggedInServerInterface != null)
		{
			return IsRequestPossible(REQ_TYPE.GET_PRIZE);
		}
		return false;
	}

	public bool RequestGetDataHistory(int count, CallbackGetDataHistory callback)
	{
		if (IsRequestGetDataHistory())
		{
			StartRequest(REQ_TYPE.GET_DATA_HISTORY);
			ServerInterface loggedInServerInterface = ServerInterface.LoggedInServerInterface;
			if (loggedInServerInterface != null)
			{
				m_callbackGetDataHistory = callback;
				loggedInServerInterface.RequestServerGetDailyBattleDataHistory(count, base.gameObject);
				return true;
			}
		}
		return false;
	}

	public bool IsRequestGetDataHistory()
	{
		ServerInterface loggedInServerInterface = ServerInterface.LoggedInServerInterface;
		if (loggedInServerInterface != null)
		{
			return IsRequestPossible(REQ_TYPE.GET_DATA_HISTORY);
		}
		return false;
	}

	public bool RequestResetMatching(int type, CallbackResetMatching callback)
	{
		if (IsRequestResetMatching())
		{
			StartRequest(REQ_TYPE.RESET_MATCHING);
			ServerInterface loggedInServerInterface = ServerInterface.LoggedInServerInterface;
			if (loggedInServerInterface != null)
			{
				m_callbackResetMatching = callback;
				loggedInServerInterface.RequestServerResetDailyBattleMatching(type, base.gameObject);
				return true;
			}
		}
		return false;
	}

	public bool IsRequestResetMatching()
	{
		ServerInterface loggedInServerInterface = ServerInterface.LoggedInServerInterface;
		if (loggedInServerInterface != null)
		{
			return IsRequestPossible(REQ_TYPE.RESET_MATCHING);
		}
		return false;
	}

	public void Dump()
	{
		Debug.Log("DailyBattleManager  Dump ============================================================================================");
		if (m_currentStatus != null)
		{
			m_currentStatus.Dump();
		}
		else
		{
			Debug.Log("ServerDailyBattleStatus  null");
		}
		if (m_currentDataPair != null)
		{
			m_currentDataPair.Dump();
		}
		else
		{
			Debug.Log("ServerDailyBattleDataPair  null");
		}
		if (m_currentDataPairList != null)
		{
			Debug.Log(string.Format("dataPairList:{0}", m_currentDataPairList.Count));
		}
		else
		{
			Debug.Log(string.Format("dataPairList:{0}", 0));
		}
		if (m_currentPrizeList != null)
		{
			Debug.Log(string.Format("prizeList:{0}", m_currentPrizeList.Count));
		}
		else
		{
			Debug.Log(string.Format("prizeList:{0}", 0));
		}
		Debug.Log(string.Format("rewardFlag:{0}", m_currentRewardFlag));
		Debug.Log(string.Format("endTime:{0}", m_currentEndTime.ToString()));
		Debug.Log("DailyBattleManager  Dump --------------------------------------------------------------------------------------------");
	}

	private void ServerGetDailyBattleStatus_Succeeded(MsgGetDailyBattleStatusSucceed msg)
	{
		if (msg != null)
		{
			SetCurrentStatus(msg.battleStatus);
			SetCurrentEndTime(msg.endTime);
			EndRequest(REQ_TYPE.GET_STATUS);
			if (IsChainRequest(REQ_TYPE.GET_STATUS))
			{
				m_chainRequestList[REQ_TYPE.GET_STATUS] = 2;
				NextChainRequest();
			}
			if (m_callbackGetStatus != null)
			{
				m_callbackGetStatus(msg.battleStatus, msg.endTime, false, null);
			}
		}
		m_callbackGetStatus = null;
		if (m_showLog)
		{
			Debug.Log("DailyBattleManager ServerGetDailyBattleStatus_Succeeded !!!!!");
			Dump();
		}
	}

	private void ServerGetDailyBattleStatus_Failed(MsgServerConnctFailed msg)
	{
		Debug.Log("DailyBattleManager ServerGetDailyBattleStatus_Failed !!!!!");
		EndRequest(REQ_TYPE.GET_STATUS);
		if (IsChainRequest(REQ_TYPE.GET_STATUS))
		{
			m_chainRequestList[REQ_TYPE.GET_STATUS] = 3;
			NextChainRequest();
		}
		if (m_callbackGetStatus != null)
		{
			m_callbackGetStatus(null, NetBase.GetCurrentTime(), false, null);
		}
		m_callbackGetStatus = null;
	}

	private void ServerUpdateDailyBattleStatus_Succeeded(MsgUpdateDailyBattleStatusSucceed msg)
	{
		if (msg != null)
		{
			SetCurrentStatus(msg.battleStatus);
			SetCurrentEndTime(msg.endTime);
			SetCurrentRewardFlag(msg.rewardFlag);
			SetCurrentRewardDataPair(msg.rewardBattleDataPair);
			EndRequest(REQ_TYPE.UPD_STATUS);
			if (IsChainRequest(REQ_TYPE.UPD_STATUS))
			{
				m_chainRequestList[REQ_TYPE.UPD_STATUS] = 2;
				NextChainRequest();
			}
			if (m_callbackGetStatus != null)
			{
				m_callbackGetStatus(msg.battleStatus, msg.endTime, msg.rewardFlag, msg.rewardBattleDataPair);
			}
		}
		m_callbackGetStatus = null;
		if (m_showLog)
		{
			Debug.Log("DailyBattleManager ServerUpdateDailyBattleStatus_Succeeded !!!!!");
			Dump();
		}
	}

	private void ServerUpdateDailyBattleStatus_Failed(MsgServerConnctFailed msg)
	{
		Debug.Log("DailyBattleManager ServerUpdateDailyBattleStatus_Failed !!!!!");
		EndRequest(REQ_TYPE.UPD_STATUS);
		if (IsChainRequest(REQ_TYPE.UPD_STATUS))
		{
			m_chainRequestList[REQ_TYPE.UPD_STATUS] = 3;
			NextChainRequest();
		}
		if (m_callbackGetStatus != null)
		{
			m_callbackGetStatus(null, NetBase.GetCurrentTime(), false, null);
		}
		m_callbackGetStatus = null;
	}

	private void ServerPostDailyBattleResult_Succeeded(MsgPostDailyBattleResultSucceed msg)
	{
		if (msg != null)
		{
			SetCurrentDataPair(msg.battleDataPair);
			SetCurrentRewardFlag(msg.rewardFlag);
			SetCurrentRewardDataPair(msg.rewardBattleDataPair);
			EndRequest(REQ_TYPE.POST_RESULT);
			if (IsChainRequest(REQ_TYPE.POST_RESULT))
			{
				m_chainRequestList[REQ_TYPE.POST_RESULT] = 2;
				NextChainRequest();
			}
			if (m_callbackPostResult != null)
			{
				m_callbackPostResult(msg.battleDataPair, msg.rewardFlag, msg.rewardBattleDataPair);
			}
		}
		m_callbackPostResult = null;
		if (m_showLog)
		{
			Debug.Log("DailyBattleManager ServerPostDailyBattleResult_Succeeded !!!!!");
			Dump();
		}
	}

	private void ServerPostDailyBattleResult_Failed(MsgServerConnctFailed msg)
	{
		Debug.Log("DailyBattleManager ServerPostDailyBattleResult_Failed !!!!!");
		EndRequest(REQ_TYPE.POST_RESULT);
		if (IsChainRequest(REQ_TYPE.POST_RESULT))
		{
			m_chainRequestList[REQ_TYPE.POST_RESULT] = 3;
			NextChainRequest();
		}
		if (m_callbackPostResult != null)
		{
			m_callbackPostResult(null, false, null);
		}
		m_callbackPostResult = null;
	}

	private void ServerGetDailyBattleData_Succeeded(MsgGetDailyBattleDataSucceed msg)
	{
		if (msg != null)
		{
			SetCurrentDataPair(msg.battleDataPair);
			EndRequest(REQ_TYPE.GET_DATA);
			if (IsChainRequest(REQ_TYPE.GET_DATA))
			{
				m_chainRequestList[REQ_TYPE.GET_DATA] = 2;
				NextChainRequest();
			}
			if (m_callbackGetData != null)
			{
				m_callbackGetData(msg.battleDataPair);
			}
		}
		m_callbackGetData = null;
		if (m_showLog)
		{
			Debug.Log("DailyBattleManager ServerGetDailyBattleData_Succeeded !!!!!");
			Dump();
		}
	}

	private void ServerGetDailyBattleData_Failed(MsgServerConnctFailed msg)
	{
		Debug.Log("DailyBattleManager ServerGetDailyBattleData_Failed !!!!!");
		EndRequest(REQ_TYPE.GET_DATA);
		if (IsChainRequest(REQ_TYPE.GET_DATA))
		{
			m_chainRequestList[REQ_TYPE.GET_DATA] = 3;
			NextChainRequest();
		}
		if (m_callbackGetData != null)
		{
			m_callbackGetData(null);
		}
		m_callbackGetData = null;
	}

	private void ServerGetPrizeDailyBattle_Succeeded(MsgGetPrizeDailyBattleSucceed msg)
	{
		if (msg != null)
		{
			SetCurrentPrizeList(msg.battlePrizeDataList);
			EndRequest(REQ_TYPE.GET_PRIZE);
			if (IsChainRequest(REQ_TYPE.GET_PRIZE))
			{
				m_chainRequestList[REQ_TYPE.GET_PRIZE] = 2;
				NextChainRequest();
			}
			if (m_callbackGetPrize != null)
			{
				m_callbackGetPrize(msg.battlePrizeDataList);
			}
		}
		m_callbackGetPrize = null;
		if (m_showLog)
		{
			Debug.Log("DailyBattleManager ServerGetPrizeDailyBattle_Succeeded !!!!!");
			Dump();
		}
	}

	private void ServerGetPrizeDailyBattle_Failed(MsgServerConnctFailed msg)
	{
		Debug.Log("DailyBattleManager ServerGetPrizeDailyBattle_Failed !!!!!");
		EndRequest(REQ_TYPE.GET_PRIZE);
		if (IsChainRequest(REQ_TYPE.GET_PRIZE))
		{
			m_chainRequestList[REQ_TYPE.GET_PRIZE] = 3;
			NextChainRequest();
		}
		if (m_callbackGetPrize != null)
		{
			m_callbackGetPrize(null);
		}
		m_callbackGetPrize = null;
	}

	private void ServerGetDailyBattleDataHistory_Succeeded(MsgGetDailyBattleDataHistorySucceed msg)
	{
		if (msg != null)
		{
			SetCurrentDataPairList(msg.battleDataPairList);
			EndRequest(REQ_TYPE.GET_DATA_HISTORY);
			if (m_callbackGetDataHistory != null)
			{
				m_callbackGetDataHistory(msg.battleDataPairList);
			}
		}
		m_callbackGetDataHistory = null;
		if (m_showLog)
		{
			Debug.Log("DailyBattleManager ServerGetDailyBattleDataHistory_Succeeded !!!!!");
			Dump();
		}
	}

	private void ServerGetDailyBattleDataHistory_Failed(MsgServerConnctFailed msg)
	{
		Debug.Log("DailyBattleManager ServerGetDailyBattleDataHistory_Failed !!!!!");
		EndRequest(REQ_TYPE.GET_DATA_HISTORY);
		if (m_callbackGetDataHistory != null)
		{
			m_callbackGetDataHistory(null);
		}
		m_callbackGetDataHistory = null;
	}

	private void ServerResetDailyBattleMatching_Succeeded(MsgResetDailyBattleMatchingSucceed msg)
	{
		if (msg != null)
		{
			SetCurrentDataPair(msg.battleDataPair);
			SetCurrentEndTime(msg.endTime);
			EndRequest(REQ_TYPE.RESET_MATCHING);
			if (m_callbackResetMatching != null)
			{
				m_callbackResetMatching(msg.playerState, msg.battleDataPair, msg.endTime);
			}
		}
		m_callbackResetMatching = null;
		if (m_showLog)
		{
			Debug.Log("DailyBattleManager ServerResetDailyBattleMatching_Succeeded !!!!!");
			Dump();
		}
	}

	private void ServerResetDailyBattleMatching_Failed(MsgServerConnctFailed msg)
	{
		Debug.Log("DailyBattleManager ServerResetDailyBattleMatching_Failed !!!!!");
		EndRequest(REQ_TYPE.RESET_MATCHING);
		if (m_callbackResetMatching != null)
		{
			m_callbackResetMatching(null, null, NetBase.GetCurrentTime());
		}
		m_callbackResetMatching = null;
	}
}
