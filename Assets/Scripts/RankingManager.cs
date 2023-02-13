using DataTable;
using Message;
using System;
using System.Collections.Generic;
using UnityEngine;

public class RankingManager : SingletonGameObject<RankingManager>
{
	private class CallbackData
	{
		public CallbackRankingData callback
		{
			get;
			set;
		}

		public int rankingType
		{
			get;
			set;
		}

		public int getPage
		{
			get;
			set;
		}

		public float startTime
		{
			get;
			set;
		}

		public CallbackData(CallbackRankingData target, int ranking, int page)
		{
			callback = target;
			rankingType = ranking;
			getPage = page;
			startTime = Time.realtimeSinceStartup;
		}

		public bool Check(int ranking, int page)
		{
			bool result = false;
			if (ranking == rankingType && getPage == page)
			{
				result = true;
			}
			return result;
		}
	}

	public delegate void CallbackRankingData(List<RankingUtil.Ranker> rankerList, RankingUtil.RankingScoreType score, RankingUtil.RankingRankerType type, int page, bool isNext, bool isPrev, bool isCashData);

	public const long CHAO_ID_OFFSET = 1000000L;

	public const float CHAO_TEX_LOAD_DELAY = 0.25f;

	public const float AUTO_RELOAD_TIME = 5f;

	private const int CALLBACK_STACK_MAX = 256;

	public const int PAGE0_RANKER_COUNT = 3;

	private const int PAGE_RANKER_COUNT_INIT = 70;

	private const int PAGE_RANKER_COUNT_MARGIN = 20;

	private const int PAGE_RANKER_COUNT_SAME = 100;

	private Dictionary<RankingUtil.RankingMode, RankingUtil.RankingDataSet> m_rankingDataSet;

	private RankingUtil.RankingMode m_mode;

	private RankingUtil.RankingScoreType m_scoreType;

	private RankingUtil.RankingRankerType m_rankerType = RankingUtil.RankingRankerType.ALL;

	private bool m_isLoading;

	private float m_getRankingLastTime;

	private int m_page;

	private int m_eventId;

	private bool m_isSpRankingInit;

	private bool m_isReset = true;

	private bool m_isRankingInit;

	private bool m_isRankingPageCheck;

	private List<CallbackData> m_callbacks = new List<CallbackData>();

	private CallbackRankingData m_callbackBakNormalAll;

	private CallbackRankingData m_callbackBakEventAll;

	private float m_chaoTextureLoadTime = -1f;

	private float m_chaoTextureLoadEndTime = -1f;

	private Dictionary<int, float> m_chaoTextureLoad;

	private Dictionary<int, Texture> m_chaoTextureList;

	private Dictionary<int, List<UITexture>> m_chaoTextureObject;

	private int m_initLoadCount;

	private List<int> m_chainGetRankingCodeList;

	public int eventId
	{
		get
		{
			return m_eventId;
		}
	}

	public static RankingUtil.RankingScoreType EndlessRivalRankingScoreType
	{
		get
		{
			RankingUtil.RankingScoreType result = RankingUtil.RankingScoreType.HIGH_SCORE;
			if (SingletonGameObject<RankingManager>.Instance != null)
			{
				RankingUtil.RankingDataSet rankingDataSet = SingletonGameObject<RankingManager>.Instance.GetRankingDataSet(RankingUtil.RankingMode.ENDLESS);
				if (rankingDataSet != null)
				{
					result = rankingDataSet.targetRivalScoreType;
				}
			}
			return result;
		}
	}

	public static RankingUtil.RankingScoreType QuickRivalRankingScoreType
	{
		get
		{
			RankingUtil.RankingScoreType result = RankingUtil.RankingScoreType.HIGH_SCORE;
			if (SingletonGameObject<RankingManager>.Instance != null)
			{
				RankingUtil.RankingDataSet rankingDataSet = SingletonGameObject<RankingManager>.Instance.GetRankingDataSet(RankingUtil.RankingMode.QUICK);
				if (rankingDataSet != null)
				{
					result = rankingDataSet.targetRivalScoreType;
				}
			}
			return result;
		}
	}

	public static RankingUtil.RankingScoreType EndlessSpecialRankingScoreType
	{
		get
		{
			RankingUtil.RankingScoreType result = RankingUtil.RankingScoreType.TOTAL_SCORE;
			if (SingletonGameObject<RankingManager>.Instance != null)
			{
				RankingUtil.RankingDataSet rankingDataSet = SingletonGameObject<RankingManager>.Instance.GetRankingDataSet(RankingUtil.RankingMode.ENDLESS);
				if (rankingDataSet != null)
				{
					result = rankingDataSet.targetSpScoreType;
				}
			}
			return result;
		}
	}

	public RankingUtil.RankingMode mode
	{
		get
		{
			return m_mode;
		}
	}

	public RankingUtil.RankingScoreType scoreType
	{
		get
		{
			return m_scoreType;
		}
	}

	public RankingUtil.RankingRankerType rankerType
	{
		get
		{
			return m_rankerType;
		}
	}

	public bool isSpRankingInit
	{
		get
		{
			return m_isSpRankingInit;
		}
	}

	public bool isLoading
	{
		get
		{
			if (m_isLoading)
			{
				float num = Mathf.Abs(m_getRankingLastTime - Time.realtimeSinceStartup);
				return num > 0.15f;
			}
			return false;
		}
	}

	public bool isChaoTextureLoading
	{
		get
		{
			if (m_chaoTextureObject != null && m_chaoTextureObject.Count > 0)
			{
				return true;
			}
			return false;
		}
	}

	public bool isRankingInit
	{
		get
		{
			return m_isRankingInit;
		}
	}

	public bool isRankingPageCheck
	{
		get
		{
			return m_isRankingPageCheck;
		}
		set
		{
			m_isRankingPageCheck = value;
		}
	}

	public bool isReset
	{
		get
		{
			return m_isReset;
		}
	}

	public void SetRankingDataSet(ServerWeeklyLeaderboardOptions leaderboardOptions)
	{
		if (m_rankingDataSet == null)
		{
			m_rankingDataSet = new Dictionary<RankingUtil.RankingMode, RankingUtil.RankingDataSet>();
		}
		int num = leaderboardOptions.mode;
		if (num < 0 || num >= 2)
		{
			num = 0;
		}
		if (m_rankingDataSet.ContainsKey((RankingUtil.RankingMode)num))
		{
			m_rankingDataSet[(RankingUtil.RankingMode)num].Setup(leaderboardOptions);
			return;
		}
		RankingUtil.RankingDataSet value = new RankingUtil.RankingDataSet(leaderboardOptions);
		m_rankingDataSet.Add((RankingUtil.RankingMode)num, value);
	}

	public RankingUtil.RankingDataSet GetRankingDataSet(RankingUtil.RankingMode rankingMode)
	{
		RankingUtil.RankingDataSet result = null;
		if (m_rankingDataSet != null && m_rankingDataSet.ContainsKey(rankingMode))
		{
			result = m_rankingDataSet[rankingMode];
		}
		return result;
	}

	private RankingDataContainer GetRankingDataContainer(RankingUtil.RankingMode rankingMode)
	{
		RankingDataContainer result = null;
		if (m_rankingDataSet != null && m_rankingDataSet.ContainsKey(rankingMode))
		{
			result = m_rankingDataSet[rankingMode].dataContainer;
		}
		return result;
	}

	public bool Init(CallbackRankingData callbackNormalAll, CallbackRankingData callbackEventAll = null)
	{
		Debug.Log("! RankingManager:Init isLoading:" + isLoading);
		m_initLoadCount = 0;
		if (isLoading)
		{
			return false;
		}
		m_callbackBakNormalAll = null;
		m_callbackBakEventAll = null;
		if (m_rankingDataSet != null && m_rankingDataSet.Count >= 2)
		{
			return RankingInit(callbackNormalAll, callbackEventAll);
		}
		m_callbackBakNormalAll = callbackNormalAll;
		m_callbackBakEventAll = callbackEventAll;
		ServerInterface.LoggedInServerInterface.RequestServerGetWeeklyLeaderboardOptions(0, base.gameObject);
		return true;
	}

	private void ServerGetWeeklyLeaderboardOptions_Succeeded(MsgGetWeeklyLeaderboardOptions msg)
	{
		Debug.Log("RankingManager: ServerGetWeeklyLeaderboardOptions_Succeeded  mode:" + msg.m_weeklyLeaderboardOptions.mode);
		m_initLoadCount++;
		if (m_rankingDataSet != null && m_rankingDataSet.Count >= 2)
		{
			m_initLoadCount = 0;
			ServerInterface.LoggedInServerInterface.RequestServerGetLeagueData(0, base.gameObject);
		}
		else if (m_initLoadCount > 2)
		{
			Debug.Log("RankingManager: ServerGetWeeklyLeaderboardOptions_Succeeded error !!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!");
			ServerWeeklyLeaderboardOptions serverWeeklyLeaderboardOptions = new ServerWeeklyLeaderboardOptions();
			msg.m_weeklyLeaderboardOptions.CopyTo(serverWeeklyLeaderboardOptions);
			serverWeeklyLeaderboardOptions.mode = 1;
			SetRankingDataSet(serverWeeklyLeaderboardOptions);
			m_initLoadCount = 0;
			ServerInterface.LoggedInServerInterface.RequestServerGetLeagueData(0, base.gameObject);
		}
		else
		{
			ServerInterface.LoggedInServerInterface.RequestServerGetWeeklyLeaderboardOptions(msg.m_weeklyLeaderboardOptions.mode + 1, base.gameObject);
		}
	}

	private void ServerGetLeagueData_Succeeded(MsgGetLeagueDataSucceed msg)
	{
		Debug.Log("RankingManager: ServerGetLeagueData_Succeeded count:" + m_initLoadCount);
		m_initLoadCount++;
		int nextLeagueDataMode = GetNextLeagueDataMode();
		if (nextLeagueDataMode < 0)
		{
			RankingLeagueTable.SetupRankingLeagueTable();
			RankingInit(null, m_callbackBakEventAll);
		}
		else if (m_initLoadCount > 2)
		{
			Debug.Log("RankingManager: ServerGetLeagueData_Succeeded error !!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!");
			if (m_rankingDataSet != null && m_rankingDataSet.Count > 0)
			{
				Dictionary<RankingUtil.RankingMode, RankingUtil.RankingDataSet>.KeyCollection keys = m_rankingDataSet.Keys;
				foreach (RankingUtil.RankingMode item in keys)
				{
					if (m_rankingDataSet[item].leagueData == null)
					{
						m_rankingDataSet[item].SetLeagueData(m_rankingDataSet[RankingUtil.RankingMode.ENDLESS].leagueData);
					}
				}
			}
			RankingLeagueTable.SetupRankingLeagueTable();
			RankingInit(null, m_callbackBakEventAll);
		}
		else
		{
			ServerInterface.LoggedInServerInterface.RequestServerGetLeagueData(nextLeagueDataMode, base.gameObject);
		}
	}

	private bool RankingInit(CallbackRankingData callbackNormalAll, CallbackRankingData callbackEventAll)
	{
		Debug.Log("! RankingManager:RankingInit isLoading:" + isLoading);
		if (isLoading)
		{
			return false;
		}
		m_isRankingPageCheck = false;
		m_page = -1;
		m_eventId = 0;
		m_getRankingLastTime = 0f;
		EventManager.EventType type = EventManager.Instance.Type;
		m_isRankingInit = true;
		ResetRankingData(RankingUtil.RankingMode.ENDLESS);
		ResetRankingData(RankingUtil.RankingMode.QUICK);
		m_isSpRankingInit = false;
		RankingUtil.RankingMode rankingMode = RankingUtil.RankingMode.ENDLESS;
		RankingUtil.RankingScoreType scoreType = RankingUtil.RankingScoreType.HIGH_SCORE;
		m_chainGetRankingCodeList = new List<int>();
		if (m_rankingDataSet != null && m_rankingDataSet.Count > 0)
		{
			int num = 0;
			Dictionary<RankingUtil.RankingMode, RankingUtil.RankingDataSet>.KeyCollection keys = m_rankingDataSet.Keys;
			foreach (RankingUtil.RankingMode item in keys)
			{
				if (num == 0)
				{
					rankingMode = item;
					scoreType = m_rankingDataSet[item].targetRivalScoreType;
				}
				int rankingCode = RankingUtil.GetRankingCode(item, m_rankingDataSet[item].targetRivalScoreType, RankingUtil.RankingRankerType.RIVAL);
				if (rankingCode >= 0)
				{
					m_chainGetRankingCodeList.Add(rankingCode);
				}
				num++;
			}
		}
		if (callbackNormalAll == null)
		{
			callbackNormalAll = DefaultCallback;
		}
		GetRanking(rankingMode, scoreType, RankingUtil.RankingRankerType.RIVAL, 0, callbackNormalAll);
		if (type == EventManager.EventType.SPECIAL_STAGE)
		{
			if (callbackEventAll == null)
			{
				callbackEventAll = EventRankingInitCallback;
			}
			GetRanking(RankingUtil.RankingMode.ENDLESS, EndlessSpecialRankingScoreType, RankingUtil.RankingRankerType.SP_ALL, 0, callbackEventAll);
		}
		else if (callbackEventAll != null)
		{
			List<RankingUtil.Ranker> rankerList = new List<RankingUtil.Ranker>();
			callbackEventAll(rankerList, EndlessSpecialRankingScoreType, RankingUtil.RankingRankerType.SP_ALL, 0, false, false, true);
		}
		return true;
	}

	public bool InitNormal(RankingUtil.RankingMode rankingMode, CallbackRankingData callback)
	{
		RankingUtil.RankingScoreType rankingScoreType = RankingUtil.RankingScoreType.HIGH_SCORE;
		rankingScoreType = ((rankingMode != 0) ? QuickRivalRankingScoreType : EndlessRivalRankingScoreType);
		Debug.Log("! RankingManager:InitNormal isLoading:" + isLoading);
		if (isLoading)
		{
			return false;
		}
		m_isRankingPageCheck = false;
		m_page = -1;
		m_eventId = 0;
		m_getRankingLastTime = 0f;
		m_isRankingInit = true;
		ResetRankingData(RankingUtil.RankingMode.ENDLESS);
		m_isSpRankingInit = false;
		if (callback == null)
		{
			callback = DefaultCallback;
		}
		GetRanking(rankingMode, rankingScoreType, RankingUtil.RankingRankerType.RIVAL, 0, callback);
		return true;
	}

	public bool InitSp(CallbackRankingData callback)
	{
		Debug.Log("! RankingManager:InitSp isLoading:" + isLoading);
		if (isLoading)
		{
			return false;
		}
		m_page = -1;
		m_eventId = 0;
		m_getRankingLastTime = 0f;
		EventManager.EventType type = EventManager.Instance.Type;
		m_isRankingInit = true;
		ResetRankingData(RankingUtil.RankingMode.ENDLESS);
		m_isSpRankingInit = false;
		if (type == EventManager.EventType.SPECIAL_STAGE)
		{
			if (callback == null)
			{
				callback = EventRankingInitCallback;
			}
			GetRanking(RankingUtil.RankingMode.ENDLESS, EndlessSpecialRankingScoreType, RankingUtil.RankingRankerType.SP_ALL, 0, callback);
		}
		else if (callback != null)
		{
			callback(null, EndlessSpecialRankingScoreType, RankingUtil.RankingRankerType.SP_ALL, 0, false, false, true);
		}
		return true;
	}

	private void Update()
	{
		if (!(m_chaoTextureLoadTime >= 0f))
		{
			return;
		}
		float deltaTime = Time.deltaTime;
		if (m_chaoTextureObject != null && m_chaoTextureObject.Count > 0)
		{
			if (m_chaoTextureLoad != null && m_chaoTextureLoad.Count > 0)
			{
				int[] array = new int[m_chaoTextureLoad.Count];
				m_chaoTextureLoad.Keys.CopyTo(array, 0);
				List<int> list = null;
				int[] array2 = array;
				foreach (int num in array2)
				{
					Dictionary<int, float> chaoTextureLoad;
					Dictionary<int, float> dictionary = chaoTextureLoad = m_chaoTextureLoad;
					int key;
					int key2 = key = num;
					float num2 = chaoTextureLoad[key];
					dictionary[key2] = num2 - deltaTime;
					if (!(m_chaoTextureLoad[num] <= 0f) || !m_chaoTextureObject.ContainsKey(num) || m_chaoTextureList == null || !m_chaoTextureList.ContainsKey(num))
					{
						continue;
					}
					List<UITexture> list2 = m_chaoTextureObject[num];
					if (list2 != null && list2.Count > 0)
					{
						foreach (UITexture item in list2)
						{
							item.mainTexture = m_chaoTextureList[num];
							item.alpha = 1f;
						}
					}
					if (list == null)
					{
						list = new List<int>();
					}
					list.Add(num);
				}
				if (list != null)
				{
					foreach (int item2 in list)
					{
						m_chaoTextureLoad.Remove(item2);
					}
				}
			}
		}
		else
		{
			m_chaoTextureLoadEndTime += deltaTime;
		}
		m_chaoTextureLoadTime += deltaTime;
		if (m_chaoTextureLoadEndTime > 5f && m_chaoTextureLoad != null && m_chaoTextureLoad.Count <= 0)
		{
			m_chaoTextureLoadTime = -1f;
		}
	}

	public ServerLeagueData GetLeagueData(RankingUtil.RankingMode rankingMode)
	{
		ServerLeagueData result = null;
		if (m_rankingDataSet != null && m_rankingDataSet.Count > 0 && m_rankingDataSet.ContainsKey(rankingMode))
		{
			result = m_rankingDataSet[rankingMode].leagueData;
		}
		return result;
	}

	public bool SetLeagueData(ServerLeagueData data)
	{
		bool result = false;
		if (m_rankingDataSet != null && m_rankingDataSet.Count > 0)
		{
			Dictionary<RankingUtil.RankingMode, RankingUtil.RankingDataSet>.KeyCollection keys = m_rankingDataSet.Keys;
			{
				foreach (RankingUtil.RankingMode item in keys)
				{
					if (item == data.rankinMode)
					{
						m_rankingDataSet[item].SetLeagueData(data);
						return true;
					}
				}
				return result;
			}
		}
		return result;
	}

	public int GetNextLeagueDataMode()
	{
		int result = -1;
		if (m_rankingDataSet != null && m_rankingDataSet.Count > 0)
		{
			Dictionary<RankingUtil.RankingMode, RankingUtil.RankingDataSet>.KeyCollection keys = m_rankingDataSet.Keys;
			{
				foreach (RankingUtil.RankingMode item in keys)
				{
					if (m_rankingDataSet[item].leagueData == null)
					{
						return (int)item;
					}
				}
				return result;
			}
		}
		return result;
	}

	public void Reset(RankingUtil.RankingMode mode, RankingUtil.RankingRankerType type)
	{
		if (m_rankingDataSet != null && m_rankingDataSet.ContainsKey(mode))
		{
			m_rankingDataSet[mode].Reset(type);
		}
	}

	private void ResetRankingData(RankingUtil.RankingMode mode)
	{
		if (m_rankingDataSet != null)
		{
			if (m_rankingDataSet.ContainsKey(mode))
			{
				m_rankingDataSet[mode].Reset();
			}
		}
		else
		{
			m_rankingDataSet = new Dictionary<RankingUtil.RankingMode, RankingUtil.RankingDataSet>();
		}
		RankingUI.SetLoading();
		if (EventManager.Instance.Type == EventManager.EventType.SPECIAL_STAGE)
		{
			SpecialStageWindow.SetLoading();
		}
		m_callbacks = null;
		m_callbacks = new List<CallbackData>();
		m_isReset = true;
		ResetChaoTexture();
	}

	public RankingUtil.RankChange GetRankingRankChange(RankingUtil.RankingMode rankingMode, RankingUtil.RankingScoreType scoreType, RankingUtil.RankingRankerType rankerType)
	{
		RankingUtil.RankChange result = RankingUtil.RankChange.NONE;
		RankingDataContainer rankingDataContainer = GetRankingDataContainer(rankingMode);
		if (rankingDataContainer != null)
		{
			result = rankingDataContainer.GetRankChange(scoreType, rankerType);
		}
		return result;
	}

	public RankingUtil.RankChange GetRankingRankChange(RankingUtil.RankingMode rankingMode, RankingUtil.RankingScoreType scoreType, RankingUtil.RankingRankerType rankerType, out int currentRank, out int oldRank)
	{
		RankingUtil.RankChange result = RankingUtil.RankChange.NONE;
		currentRank = -1;
		oldRank = -1;
		RankingDataContainer rankingDataContainer = GetRankingDataContainer(rankingMode);
		if (rankingDataContainer != null)
		{
			result = rankingDataContainer.GetRankChange(scoreType, rankerType, out currentRank, out oldRank);
		}
		return result;
	}

	public RankingUtil.RankChange GetRankingRankChange(RankingUtil.RankingMode rankingMode)
	{
		RankingUtil.RankingDataSet rankingDataSet = GetRankingDataSet(rankingMode);
		RankingUtil.RankingRankerType rankerType = RankingUtil.RankingRankerType.RIVAL;
		RankingUtil.RankingScoreType targetRivalScoreType = rankingDataSet.targetRivalScoreType;
		return GetRankingRankChange(rankingMode, targetRivalScoreType, rankerType);
	}

	public RankingUtil.RankChange GetRankingRankChange(RankingUtil.RankingMode rankingMode, out int currentRank, out int oldRank)
	{
		RankingUtil.RankingDataSet rankingDataSet = GetRankingDataSet(rankingMode);
		RankingUtil.RankingRankerType rankerType = RankingUtil.RankingRankerType.RIVAL;
		RankingUtil.RankingScoreType targetRivalScoreType = rankingDataSet.targetRivalScoreType;
		return GetRankingRankChange(rankingMode, targetRivalScoreType, rankerType, out currentRank, out oldRank);
	}

	public void ResetRankingRankChange(RankingUtil.RankingMode rankingMode)
	{
		RankingUtil.RankingDataSet rankingDataSet = GetRankingDataSet(rankingMode);
		if (rankingDataSet != null)
		{
			RankingUtil.RankingRankerType rankerType = RankingUtil.RankingRankerType.RIVAL;
			RankingUtil.RankingScoreType targetRivalScoreType = rankingDataSet.targetRivalScoreType;
			rankingDataSet.ResetRankChange(targetRivalScoreType, rankerType);
		}
	}

	public bool GetRanking(RankingUtil.RankingMode rankingMode, RankingUtil.RankingScoreType scoreType, RankingUtil.RankingRankerType rankerType, int page, CallbackRankingData callback)
	{
		if (!m_isRankingInit)
		{
			return false;
		}
		RankingDataContainer rankingDataContainer = GetRankingDataContainer(rankingMode);
		m_isReset = false;
		if (rankerType == RankingUtil.RankingRankerType.RIVAL)
		{
			page = 0;
		}
		bool flag = false;
		if (isLoading && m_scoreType == scoreType && m_rankerType == rankerType && (m_page == page || page == 0) && m_callbacks != null)
		{
			if (m_callbacks.Count > 0)
			{
				flag = true;
				foreach (CallbackData callback2 in m_callbacks)
				{
					if (callback2.rankingType == RankingUtil.GetRankingType(scoreType, rankerType) && (callback2.getPage == page || page == 0))
					{
						flag = false;
						callback2.getPage = page;
						callback2.callback = callback;
						return false;
					}
				}
			}
			else
			{
				flag = true;
			}
			if (flag && callback != null)
			{
				List<RankingUtil.Ranker> list = null;
				if (page > 1)
				{
					page--;
				}
				if (IsRankingList(rankingMode, scoreType, rankerType, page))
				{
					list = GetRankerList(rankingMode, scoreType, rankerType, page);
				}
				if (list != null && rankingDataContainer != null)
				{
					MsgGetLeaderboardEntriesSucceed rankerListOrg = rankingDataContainer.GetRankerListOrg(rankerType, scoreType, page);
					callback(list, scoreType, rankerType, page, rankerListOrg.m_leaderboardEntries.IsNext(), rankerListOrg.m_leaderboardEntries.IsPrev(), true);
					return true;
				}
				return false;
			}
		}
		if (page < 0)
		{
			return false;
		}
		bool flag2 = IsRankingList(rankingMode, scoreType, rankerType, page);
		int rankingType = RankingUtil.GetRankingType(scoreType, rankerType);
		ServerInterface loggedInServerInterface = ServerInterface.LoggedInServerInterface;
		bool flag3 = true;
		MsgGetLeaderboardEntriesSucceed msgGetLeaderboardEntriesSucceed = null;
		if (flag2)
		{
			msgGetLeaderboardEntriesSucceed = rankingDataContainer.GetRankerListOrg(rankerType, scoreType, page);
			if (msgGetLeaderboardEntriesSucceed == null)
			{
				flag3 = true;
			}
			else if (page == 0)
			{
				flag3 = rankingDataContainer.IsRankerListReload(rankerType, scoreType);
			}
			else if (page > 0)
			{
				flag3 = true;
			}
		}
		if (page >= 2)
		{
			flag3 = true;
		}
		if (flag3)
		{
			m_isLoading = true;
			m_scoreType = scoreType;
			m_rankerType = rankerType;
			m_page = page;
			m_eventId = EventManager.Instance.Id;
			if (callback != null && m_callbacks != null)
			{
				CallbackData item = new CallbackData(callback, rankingType, page);
				m_callbacks.Add(item);
				if (m_callbacks.Count > 256)
				{
					m_callbacks.RemoveAt(0);
				}
			}
			int rankingTop = GetRankingTop(rankingMode, rankerType, scoreType, page);
			int rankingSize = GetRankingSize(rankerType, rankingTop, page);
			string[] friendIdList = RankingUtil.GetFriendIdList();
			if (loggedInServerInterface != null)
			{
				loggedInServerInterface.RequestServerGetLeaderboardEntries((int)rankingMode, rankingTop, rankingSize, page, rankingType, m_eventId, friendIdList, base.gameObject);
			}
		}
		else if (flag2 && callback != null)
		{
			List<RankingUtil.Ranker> rankerList = GetRankerList(rankingMode, scoreType, rankerType, page);
			if (rankerList != null)
			{
				callback(rankerList, scoreType, rankerType, page, msgGetLeaderboardEntriesSucceed.m_leaderboardEntries.IsNext(), msgGetLeaderboardEntriesSucceed.m_leaderboardEntries.IsPrev(), true);
				return true;
			}
		}
		return false;
	}

	public bool GetRankingScroll(RankingUtil.RankingMode rankingMode, bool isNext, CallbackRankingData callback)
	{
		if (!m_isRankingInit)
		{
			return false;
		}
		RankingDataContainer rankingDataContainer = GetRankingDataContainer(rankingMode);
		m_isReset = false;
		bool result = false;
		if (!isLoading && m_page > 0)
		{
			RankingUtil.RankingScoreType scoreType = m_scoreType;
			RankingUtil.RankingRankerType rankerType = m_rankerType;
			if (rankingDataContainer != null)
			{
				bool flag = false;
				int top = 1;
				int count = 70;
				int rankingType = RankingUtil.GetRankingType(scoreType, rankerType);
				Dictionary<RankingUtil.RankingScoreType, List<MsgGetLeaderboardEntriesSucceed>> data;
				rankingDataContainer.IsRankerType(rankerType, out data);
				if (data != null && data.Count > 0 && data.ContainsKey(scoreType) && data[scoreType].Count > 1)
				{
					MsgGetLeaderboardEntriesSucceed msgGetLeaderboardEntriesSucceed = data[scoreType][1];
					if (msgGetLeaderboardEntriesSucceed != null && msgGetLeaderboardEntriesSucceed.m_leaderboardEntries != null)
					{
						if (isNext)
						{
							flag = msgGetLeaderboardEntriesSucceed.m_leaderboardEntries.IsNext();
							msgGetLeaderboardEntriesSucceed.m_leaderboardEntries.GetNextRanking(ref top, ref count, 20);
						}
						else
						{
							flag = msgGetLeaderboardEntriesSucceed.m_leaderboardEntries.IsPrev();
							msgGetLeaderboardEntriesSucceed.m_leaderboardEntries.GetPrevRanking(ref top, ref count, 20);
						}
						if (flag)
						{
							if (msgGetLeaderboardEntriesSucceed.m_leaderboardEntries.m_count > 30000)
							{
								return false;
							}
							if (top + count > 30000)
							{
								int num = top + count - 30000;
								count = count - num + 2;
							}
						}
					}
				}
				if (flag)
				{
					if (callback != null)
					{
						CallbackData item = new CallbackData(callback, rankingType, 2);
						m_callbacks.Add(item);
						if (m_callbacks.Count > 256)
						{
							m_callbacks.RemoveAt(0);
						}
					}
					result = true;
					m_isLoading = true;
					ServerInterface loggedInServerInterface = ServerInterface.LoggedInServerInterface;
					string[] friendIdList = RankingUtil.GetFriendIdList();
					if (loggedInServerInterface != null)
					{
						Debug.Log("RankingManager:RequestServerGetLeaderboardEntries   rankTop:" + top + "  rankSize:" + count + "  type:" + rankingType + " eventId:" + m_eventId);
						loggedInServerInterface.RequestServerGetLeaderboardEntries((int)rankingMode, top, count, 2, rankingType, m_eventId, friendIdList, base.gameObject);
					}
				}
			}
		}
		return result;
	}

	public static void SavePlayerRankingData(RankingUtil.RankingMode rankingMode)
	{
		if (SingletonGameObject<RankingManager>.Instance != null)
		{
			SingletonGameObject<RankingManager>.Instance.SavePlayerRankingDataOrg(rankingMode);
		}
	}

	private void SavePlayerRankingDataOrg(RankingUtil.RankingMode rankingMode)
	{
		if (m_rankingDataSet == null)
		{
			return;
		}
		Dictionary<RankingUtil.RankingMode, RankingUtil.RankingDataSet>.KeyCollection keys = m_rankingDataSet.Keys;
		foreach (RankingUtil.RankingMode item in keys)
		{
			if (item == rankingMode)
			{
				m_rankingDataSet[item].SaveRanking();
			}
			else
			{
				m_rankingDataSet[item].Reset();
			}
		}
	}

	public void SavePlayerRankingDataDummy(RankingUtil.RankingMode rankingMode, RankingUtil.RankingRankerType rankType, RankingUtil.RankingScoreType scoreType, int dammyRank)
	{
		RankingDataContainer rankingDataContainer = GetRankingDataContainer(rankingMode);
		if (rankingDataContainer != null)
		{
			rankingDataContainer.SavePlayerRankingDummy(rankType, scoreType, dammyRank);
		}
	}

	public TimeSpan GetRankigResetTimeSpan(RankingUtil.RankingMode rankingMode, RankingUtil.RankingScoreType scoreType, RankingUtil.RankingRankerType rankerType)
	{
		RankingDataContainer rankingDataContainer = GetRankingDataContainer(rankingMode);
		if (rankingDataContainer == null)
		{
			return NetUtil.GetCurrentTime().AddMinutes(1.0) - NetUtil.GetCurrentTime();
		}
		return rankingDataContainer.GetResetTimeSpan(rankerType, scoreType);
	}

	public static int GetRankingMax(RankingUtil.RankingRankerType rankerType, int page = 0)
	{
		return GetRankingSize(rankerType, 1, page) - 1;
	}

	public static RankingUtil.Ranker GetMyRank(RankingUtil.RankingMode rankingMode, RankingUtil.RankingRankerType rankType, RankingUtil.RankingScoreType scoreType)
	{
		if (SingletonGameObject<RankingManager>.Instance != null)
		{
			return SingletonGameObject<RankingManager>.Instance.GetMyRankOrg(rankingMode, rankType, scoreType);
		}
		return null;
	}

	private RankingUtil.Ranker GetMyRankOrg(RankingUtil.RankingMode rankingMode, RankingUtil.RankingRankerType rankType, RankingUtil.RankingScoreType scoreType)
	{
		RankingUtil.Ranker result = null;
		List<RankingUtil.Ranker> rankerList = GetRankerList(rankingMode, scoreType, rankType);
		if (rankerList != null && rankerList.Count > 0)
		{
			RankingUtil.Ranker ranker = rankerList[0];
			if (ranker != null)
			{
				result = ranker;
			}
		}
		return result;
	}

	public static long GetMyHiScore(RankingUtil.RankingMode rankingMode, bool isEvent)
	{
		long result = 0L;
		if (SingletonGameObject<RankingManager>.Instance != null)
		{
			RankingUtil.RankingRankerType rankType;
			RankingUtil.RankingScoreType scoreType;
			if (isEvent)
			{
				rankType = RankingUtil.RankingRankerType.SP_ALL;
				scoreType = EndlessSpecialRankingScoreType;
			}
			else
			{
				rankType = RankingUtil.RankingRankerType.RIVAL;
				scoreType = EndlessRivalRankingScoreType;
			}
			RankingUtil.Ranker myRankOrg = SingletonGameObject<RankingManager>.Instance.GetMyRankOrg(rankingMode, rankType, scoreType);
			if (myRankOrg != null)
			{
				result = myRankOrg.hiScore;
			}
		}
		return result;
	}

	public static RankingUtil.RankingScoreType GetCurrentRankingScoreType(RankingUtil.RankingMode rankingMode, bool isEvent)
	{
		RankingUtil.RankingScoreType result = RankingUtil.RankingScoreType.HIGH_SCORE;
		if (SingletonGameObject<RankingManager>.Instance != null)
		{
			RankingUtil.RankingDataSet rankingDataSet = SingletonGameObject<RankingManager>.Instance.GetRankingDataSet(rankingMode);
			if (rankingDataSet != null)
			{
				result = ((!isEvent) ? rankingDataSet.targetRivalScoreType : rankingDataSet.targetSpScoreType);
			}
		}
		return result;
	}

	public static int GetCurrentMyLeagueMax(RankingUtil.RankingMode rankingMode)
	{
		if (SingletonGameObject<RankingManager>.Instance != null)
		{
			return SingletonGameObject<RankingManager>.Instance.GetCurrentMyLeagueMaxOrg(rankingMode);
		}
		return 0;
	}

	private int GetCurrentMyLeagueMaxOrg(RankingUtil.RankingMode rankingMode)
	{
		int result = 0;
		List<RankingUtil.Ranker> rankerList = GetRankerList(rankingMode, EndlessRivalRankingScoreType, RankingUtil.RankingRankerType.RIVAL);
		if (rankerList != null)
		{
			result = rankerList.Count - 1;
		}
		return result;
	}

	public static bool GetCurrentRankingStatus(RankingUtil.RankingMode rankingMode, bool isEvent, out long myScore, out long myHiScore, out int myLeague)
	{
		if (SingletonGameObject<RankingManager>.Instance != null)
		{
			return SingletonGameObject<RankingManager>.Instance.GetCurrentRankingStatusOrg(rankingMode, isEvent, out myScore, out myHiScore, out myLeague);
		}
		myScore = 0L;
		myLeague = 0;
		myHiScore = 0L;
		return false;
	}

	private bool GetCurrentRankingStatusOrg(RankingUtil.RankingMode rankingMode, bool isEvent, out long myScore, out long myHiScore, out int myLeague)
	{
		bool result = false;
		myScore = 0L;
		myHiScore = 0L;
		myLeague = 0;
		List<RankingUtil.Ranker> list = null;
		list = ((!isEvent) ? GetRankerList(rankingMode, EndlessRivalRankingScoreType, RankingUtil.RankingRankerType.RIVAL) : GetRankerList(rankingMode, EndlessSpecialRankingScoreType, RankingUtil.RankingRankerType.SP_ALL));
		if (list != null && list.Count > 0)
		{
			RankingUtil.Ranker ranker = list[0];
			if (ranker != null)
			{
				myScore = ranker.score;
				myHiScore = ranker.hiScore;
				myLeague = ranker.leagueIndex;
				result = true;
			}
		}
		return result;
	}

	public static int GetCurrentHighScoreRank(RankingUtil.RankingMode rankingMode, bool isEvent, ref long currentScore, out bool isHighScore, out long nextRankScore, out long prveRankScore, out int nextRank)
	{
		if (SingletonGameObject<RankingManager>.Instance != null)
		{
			return SingletonGameObject<RankingManager>.Instance.GetCurrentHighScoreRankOrg(rankingMode, isEvent, ref currentScore, out isHighScore, out nextRankScore, out prveRankScore, out nextRank);
		}
		isHighScore = false;
		nextRankScore = 0L;
		prveRankScore = 0L;
		nextRank = -1;
		return -1;
	}

	private int GetCurrentHighScoreRankOrg(RankingUtil.RankingMode rankingMode, bool isEvent, ref long currentScore, out bool isHighScore, out long nextRankScore, out long prveRankScore, out int nextRank)
	{
		RankingDataContainer rankingDataContainer = GetRankingDataContainer(rankingMode);
		if (rankingDataContainer != null)
		{
			return rankingDataContainer.GetCurrentHighScoreRank(isEvent, ref currentScore, out isHighScore, out nextRankScore, out prveRankScore, out nextRank);
		}
		isHighScore = false;
		nextRankScore = 0L;
		prveRankScore = 0L;
		nextRank = -1;
		return -1;
	}

	public static bool IsRankingInAggregate(RankingUtil.RankingMode rankingMode, RankingUtil.RankingRankerType rankType, RankingUtil.RankingScoreType scoreType)
	{
		bool result = false;
		if (SingletonGameObject<RankingManager>.Instance != null)
		{
			result = SingletonGameObject<RankingManager>.Instance.IsRankingInAggregateOrg(rankingMode, rankType, scoreType);
		}
		return result;
	}

	private bool IsRankingInAggregateOrg(RankingUtil.RankingMode rankingMode, RankingUtil.RankingRankerType rankType, RankingUtil.RankingScoreType scoreType)
	{
		bool result = false;
		if (GetRankigResetTimeSpan(rankingMode, scoreType, rankType).Ticks <= 0)
		{
			result = true;
		}
		return result;
	}

	public static void UpdateSendChallenge(string id)
	{
		if (SingletonGameObject<RankingManager>.Instance != null)
		{
			SingletonGameObject<RankingManager>.Instance.UpdateSendChallengeOrg(id);
		}
	}

	private void UpdateSendChallengeOrg(string id)
	{
		EventManager.EventType type = EventManager.Instance.Type;
		bool flag = false;
		if (UpdateSendChallengeRankingList(RankingUtil.RankingRankerType.RIVAL, id))
		{
			RankingUI.UpdateSendChallenge(RankingUtil.RankingRankerType.RIVAL, id);
		}
		if (UpdateSendChallengeRankingList(RankingUtil.RankingRankerType.FRIEND, id))
		{
			RankingUI.UpdateSendChallenge(RankingUtil.RankingRankerType.FRIEND, id);
		}
		if (UpdateSendChallengeRankingList(RankingUtil.RankingRankerType.ALL, id))
		{
			RankingUI.UpdateSendChallenge(RankingUtil.RankingRankerType.ALL, id);
		}
		if (UpdateSendChallengeRankingList(RankingUtil.RankingRankerType.HISTORY, id))
		{
			RankingUI.UpdateSendChallenge(RankingUtil.RankingRankerType.HISTORY, id);
		}
		if (type == EventManager.EventType.SPECIAL_STAGE)
		{
			if (UpdateSendChallengeRankingList(RankingUtil.RankingRankerType.SP_FRIEND, id))
			{
				SpecialStageWindow.UpdateSendChallenge(RankingUtil.RankingRankerType.SP_FRIEND, id);
			}
			if (UpdateSendChallengeRankingList(RankingUtil.RankingRankerType.SP_ALL, id))
			{
				SpecialStageWindow.UpdateSendChallenge(RankingUtil.RankingRankerType.SP_ALL, id);
			}
		}
	}

	private bool UpdateSendChallengeRankingList(RankingUtil.RankingRankerType type, string id)
	{
		bool result = false;
		if (m_rankingDataSet != null)
		{
			Dictionary<RankingUtil.RankingMode, RankingUtil.RankingDataSet>.KeyCollection keys = m_rankingDataSet.Keys;
			{
				foreach (RankingUtil.RankingMode item in keys)
				{
					if (m_rankingDataSet[item].UpdateSendChallengeList(type, id))
					{
						result = true;
					}
				}
				return result;
			}
		}
		return result;
	}

	private void DefaultCallback(List<RankingUtil.Ranker> rankerList, RankingUtil.RankingScoreType score, RankingUtil.RankingRankerType type, int page, bool isNext, bool isPrev, bool isCashData)
	{
		RankingUI.Setup();
	}

	private void EventRankingInitCallback(List<RankingUtil.Ranker> rankerList, RankingUtil.RankingScoreType score, RankingUtil.RankingRankerType type, int page, bool isNext, bool isPrev, bool isCashData)
	{
		Debug.Log(string.Concat(" ! RankingManager:NullCallback   type:", type, "  page:", page, "  isNext:", isNext, "  isPrev:", isPrev, "  num:", rankerList.Count));
		m_isSpRankingInit = true;
		SpecialStageWindow.RankingSetup();
	}

	public void EventRankingSameRankCallback(List<RankingUtil.Ranker> rankerList, RankingUtil.RankingScoreType score, RankingUtil.RankingRankerType type, int page, bool isNext, bool isPrev, bool isCashData)
	{
		Debug.Log(string.Concat(" ! RankingManager:EventRankingSameRankCallback   type:", type, "  page:", page, "  isNext:", isNext, "  isPrev:", isPrev, "  num:", rankerList.Count));
	}

	public void RankingSameRankCallback(List<RankingUtil.Ranker> rankerList, RankingUtil.RankingScoreType score, RankingUtil.RankingRankerType type, int page, bool isNext, bool isPrev, bool isCashData)
	{
		Debug.Log(string.Concat(" ! RankingManager:RankingSameRankCallback   type:", type, "  page:", page, "  isNext:", isNext, "  isPrev:", isPrev, "  num:", rankerList.Count));
	}

	private void ServerGetLeaderboardEntries_Succeeded(MsgGetLeaderboardEntriesSucceed msg)
	{
		ServerLeaderboardEntries leaderboardEntries = msg.m_leaderboardEntries;
		int num = msg.m_leaderboardEntries.m_mode;
		int rankerPage = GetRankerPage(msg);
		int rankingType = leaderboardEntries.m_rankingType;
		if (num < 0 || num >= 2)
		{
			num = 0;
		}
		RankingUtil.RankingMode rankingMode = (RankingUtil.RankingMode)num;
		Debug.Log(string.Concat(" RankingManager:ServerGetLeaderboardEntries_Succeeded mode:", rankingMode, " Count:", msg.m_leaderboardEntries.m_leaderboardEntries.Count));
		if (m_rankingDataSet != null && m_rankingDataSet.ContainsKey(rankingMode))
		{
			m_rankingDataSet[rankingMode].AddRankerList(msg);
			if (RankingUtil.IsRankingUserFrontAndBack(scoreType, this.rankerType, rankerPage))
			{
				CallbackRankingData callbackRankingData = null;
				GetRanking(callback: (this.rankerType != RankingUtil.RankingRankerType.SP_ALL && this.rankerType != RankingUtil.RankingRankerType.SP_FRIEND) ? new CallbackRankingData(RankingSameRankCallback) : new CallbackRankingData(EventRankingSameRankCallback), rankingMode: rankingMode, scoreType: scoreType, rankerType: this.rankerType, page: 3);
			}
			if (m_callbacks == null)
			{
				m_callbacks = new List<CallbackData>();
			}
			if (m_callbacks.Count > 0)
			{
				CallbackData callbackData = null;
				for (int i = 0; i < m_callbacks.Count; i++)
				{
					if (m_callbacks[i].Check(rankingType, rankerPage))
					{
						callbackData = m_callbacks[i];
						break;
					}
				}
				if (callbackData != null && callbackData.callback != null)
				{
					leaderboardEntries = msg.m_leaderboardEntries;
					List<RankingUtil.Ranker> list = null;
					list = ((this.rankerType != RankingUtil.RankingRankerType.RIVAL && rankerPage != 0) ? m_rankingDataSet[rankingMode].GetRankerList(this.rankerType, scoreType, 1) : m_rankingDataSet[rankingMode].GetRankerList(this.rankerType, scoreType, 0));
					callbackData.callback(list, scoreType, this.rankerType, rankerPage, leaderboardEntries.IsNext(), leaderboardEntries.IsPrev(), false);
					m_callbacks.Remove(callbackData);
				}
			}
			Debug.Log(" RankingManager:ServerGetLeaderboardEntries_Succeeded  chainGetRankingCodeList:" + m_chainGetRankingCodeList.Count);
			if (m_chainGetRankingCodeList != null && m_chainGetRankingCodeList.Count > 0)
			{
				RankingUtil.RankingScoreType rankerScoreType = RankingUtil.GetRankerScoreType(rankingType);
				RankingUtil.RankingRankerType rankerType = RankingUtil.GetRankerType(rankingType);
				int rankingCode = RankingUtil.GetRankingCode(rankingMode, rankerScoreType, rankerType);
				if (m_chainGetRankingCodeList.Contains(rankingCode))
				{
					m_chainGetRankingCodeList.Remove(rankingCode);
					if (m_chainGetRankingCodeList.Count > 0)
					{
						m_isLoading = false;
						int rankingType2 = m_chainGetRankingCodeList[0];
						RankingUtil.RankingMode rankerMode = RankingUtil.GetRankerMode(rankingType2);
						RankingUtil.RankingScoreType rankerScoreType2 = RankingUtil.GetRankerScoreType(rankingType2);
						RankingUtil.RankingRankerType rankerType2 = RankingUtil.GetRankerType(rankingType2);
						GetRanking(rankerMode, rankerScoreType2, rankerType2, 0, DefaultCallback);
					}
					else
					{
						Debug.Log(" RankingManager:ServerGetLeaderboardEntries_Succeeded  chain end " + (m_callbackBakNormalAll != null));
						if (m_callbackBakNormalAll != null)
						{
							List<RankingUtil.Ranker> list2 = null;
							list2 = ((this.rankerType != RankingUtil.RankingRankerType.RIVAL && rankerPage != 0) ? m_rankingDataSet[rankingMode].GetRankerList(this.rankerType, scoreType, 1) : m_rankingDataSet[rankingMode].GetRankerList(this.rankerType, scoreType, 0));
							m_callbackBakNormalAll(list2, scoreType, this.rankerType, rankerPage, leaderboardEntries.IsNext(), leaderboardEntries.IsPrev(), false);
							m_callbackBakNormalAll = null;
						}
						m_chainGetRankingCodeList.Clear();
					}
				}
			}
		}
		m_getRankingLastTime = Time.realtimeSinceStartup;
		m_isLoading = false;
	}

	private void ServerGetLeaderboardEntries_Failed()
	{
		Debug.Log(" RankingManager:ServerGetLeaderboardEntries_Failed()");
		m_getRankingLastTime = Time.realtimeSinceStartup;
		m_isLoading = false;
	}

	private List<RankingUtil.Ranker> GetRankerList(RankingUtil.RankingMode rankingMode, RankingUtil.RankingScoreType scoreType, RankingUtil.RankingRankerType rankerType, int page = 0)
	{
		List<RankingUtil.Ranker> result = null;
		if (rankerType == RankingUtil.RankingRankerType.RIVAL)
		{
			page = 0;
		}
		if (m_rankingDataSet != null && m_rankingDataSet.ContainsKey(rankingMode))
		{
			result = m_rankingDataSet[rankingMode].GetRankerList(rankerType, scoreType, page);
		}
		return result;
	}

	private int GetRankerPage(MsgGetLeaderboardEntriesSucceed msg)
	{
		int result = 0;
		if (msg != null && msg.m_leaderboardEntries != null)
		{
			result = msg.m_leaderboardEntries.m_index;
			if (msg.m_leaderboardEntries.IsRivalRanking())
			{
				result = 0;
			}
		}
		return result;
	}

	private int GetRankingTop(RankingUtil.RankingMode rankingMode, RankingUtil.RankingRankerType rankerType, RankingUtil.RankingScoreType scoreType, int page = 0)
	{
		int num = 1;
		if (page >= 3)
		{
			RankingDataContainer rankingDataContainer = GetRankingDataContainer(rankingMode);
			if (rankingDataContainer != null)
			{
				Dictionary<RankingUtil.RankingScoreType, List<MsgGetLeaderboardEntriesSucceed>> data;
				rankingDataContainer.IsRankerType(rankerType, out data);
				if (data != null && data.ContainsKey(scoreType))
				{
					List<MsgGetLeaderboardEntriesSucceed> list = data[scoreType];
					if (list != null && list.Count > 0)
					{
						ServerLeaderboardEntry serverLeaderboardEntry = null;
						if (list[0] != null)
						{
							serverLeaderboardEntry = list[0].m_leaderboardEntries.m_myLeaderboardEntry;
						}
						else if (list.Count > 1 && list[1] != null)
						{
							serverLeaderboardEntry = list[1].m_leaderboardEntries.m_myLeaderboardEntry;
						}
						if (serverLeaderboardEntry != null)
						{
							num = serverLeaderboardEntry.m_grade - 50;
							if (num < 1)
							{
								num = 1;
							}
						}
					}
				}
			}
		}
		return num;
	}

	private static int GetRankingSize(RankingUtil.RankingRankerType rankerType, int top, int page)
	{
		if (rankerType == RankingUtil.RankingRankerType.COUNT || page < 0)
		{
			return -1;
		}
		int result = 0;
		switch (rankerType)
		{
		case RankingUtil.RankingRankerType.FRIEND:
		case RankingUtil.RankingRankerType.ALL:
		case RankingUtil.RankingRankerType.HISTORY:
		case RankingUtil.RankingRankerType.SP_ALL:
		case RankingUtil.RankingRankerType.SP_FRIEND:
			result = 4;
			break;
		case RankingUtil.RankingRankerType.RIVAL:
			result = 71;
			break;
		}
		if (page > 0)
		{
			result = 71;
			if (page >= 3)
			{
				result = 100;
			}
		}
		return result;
	}

	public bool IsRankingTop(RankingUtil.RankingMode rankingMode, RankingUtil.RankingScoreType scoreType, RankingUtil.RankingRankerType rankerType)
	{
		bool result = false;
		if (scoreType == RankingUtil.RankingScoreType.NONE || rankerType == RankingUtil.RankingRankerType.COUNT)
		{
			return false;
		}
		RankingDataContainer rankingDataContainer = GetRankingDataContainer(rankingMode);
		if (rankingDataContainer != null)
		{
			result = rankingDataContainer.IsRankerAndScoreType(rankerType, scoreType);
		}
		return result;
	}

	private bool IsRankingList(RankingUtil.RankingMode rankingMode, RankingUtil.RankingScoreType scoreType, RankingUtil.RankingRankerType rankerType, int page = 0)
	{
		bool result = false;
		if (scoreType == RankingUtil.RankingScoreType.NONE || rankerType == RankingUtil.RankingRankerType.COUNT || page < 0)
		{
			return false;
		}
		RankingDataContainer rankingDataContainer = GetRankingDataContainer(rankingMode);
		if (rankingDataContainer != null)
		{
			result = rankingDataContainer.IsRankerAndScoreType(rankerType, scoreType, page);
		}
		return result;
	}

	public List<RankingUtil.Ranker> GetCacheRankingList(RankingUtil.RankingMode rankingMode, RankingUtil.RankingScoreType scoreType, RankingUtil.RankingRankerType rankerType)
	{
		List<RankingUtil.Ranker> list = null;
		RankingDataContainer rankingDataContainer = GetRankingDataContainer(rankingMode);
		if (rankingDataContainer != null)
		{
			if (rankerType == RankingUtil.RankingRankerType.RIVAL)
			{
				list = rankingDataContainer.GetRankerList(rankerType, scoreType, 0);
			}
			else
			{
				list = rankingDataContainer.GetRankerList(rankerType, scoreType, 1);
				List<RankingUtil.Ranker> rankerList = rankingDataContainer.GetRankerList(rankerType, scoreType, 2);
				if (list != null && rankerList != null && rankerList.Count > 1)
				{
					for (int i = 1; i < rankerList.Count; i++)
					{
						list.Add(rankerList[i]);
					}
				}
			}
		}
		return list;
	}

	public Texture GetChaoTexture(int chaoId, UITexture chaoTexture, float delay = 0.2f, bool isDefaultTexture = false)
	{
		Texture result = null;
		if (chaoTexture == null)
		{
			return null;
		}
		if (m_chaoTextureList != null && m_chaoTextureList.ContainsKey(chaoId))
		{
			result = m_chaoTextureList[chaoId];
			chaoTexture.mainTexture = m_chaoTextureList[chaoId];
			chaoTexture.alpha = 1f;
		}
		else
		{
			Texture loadedTexture = ChaoTextureManager.Instance.GetLoadedTexture(chaoId);
			if (loadedTexture != null)
			{
				if (m_chaoTextureList == null)
				{
					m_chaoTextureList = new Dictionary<int, Texture>();
				}
				m_chaoTextureList.Add(chaoId, loadedTexture);
				result = m_chaoTextureList[chaoId];
				chaoTexture.mainTexture = m_chaoTextureList[chaoId];
				chaoTexture.alpha = 1f;
			}
			else
			{
				if (isDefaultTexture || chaoTexture.alpha > 0f)
				{
					chaoTexture.mainTexture = ChaoTextureManager.Instance.m_defaultTexture;
				}
				if (m_chaoTextureLoad == null)
				{
					m_chaoTextureLoad = new Dictionary<int, float>();
				}
				if (m_chaoTextureList == null)
				{
					m_chaoTextureList = new Dictionary<int, Texture>();
				}
				if (m_chaoTextureObject == null)
				{
					m_chaoTextureObject = new Dictionary<int, List<UITexture>>();
				}
				if (!m_chaoTextureObject.ContainsKey(chaoId))
				{
					List<UITexture> value = new List<UITexture>();
					m_chaoTextureObject.Add(chaoId, value);
				}
				m_chaoTextureObject[chaoId].Add(chaoTexture);
				if (m_chaoTextureLoad.Count <= 0)
				{
					ChaoTextureManager.CallbackInfo.LoadFinishCallback callback = ChaoLoadFinishCallback;
					ChaoTextureManager.CallbackInfo info = new ChaoTextureManager.CallbackInfo(null, callback);
					ChaoTextureManager.Instance.GetTexture(chaoId, info);
					m_chaoTextureLoadTime = 0f;
				}
				else if (!m_chaoTextureLoad.ContainsKey(chaoId))
				{
					m_chaoTextureLoad.Add(chaoId, delay);
				}
				else if (delay > 0f && m_chaoTextureLoad[chaoId] < delay * 0.15f)
				{
					m_chaoTextureLoad[chaoId] = delay * 0.15f;
				}
				m_chaoTextureLoadEndTime = 0f;
			}
		}
		return result;
	}

	public void ResetChaoTexture()
	{
		if (m_chaoTextureLoad != null)
		{
			m_chaoTextureLoad.Clear();
		}
		if (m_chaoTextureList != null)
		{
			m_chaoTextureList.Clear();
		}
		if (m_chaoTextureObject != null)
		{
			m_chaoTextureObject.Clear();
		}
		m_chaoTextureLoadTime = -1f;
		m_chaoTextureLoadEndTime = 0f;
		m_chaoTextureLoad = new Dictionary<int, float>();
		m_chaoTextureList = new Dictionary<int, Texture>();
		m_chaoTextureObject = new Dictionary<int, List<UITexture>>();
	}

	private void ChaoLoadFinishCallback(Texture tex)
	{
		if (tex == null)
		{
			return;
		}
		string[] array = tex.name.Split('_');
		int num = int.Parse(array[array.Length - 1]);
		if (m_chaoTextureObject != null && m_chaoTextureLoad != null && m_chaoTextureObject.ContainsKey(num))
		{
			bool flag = true;
			if (m_chaoTextureLoad.ContainsKey(num))
			{
				if (m_chaoTextureLoad[num] > 0f)
				{
					flag = false;
				}
				else
				{
					m_chaoTextureLoad.Remove(num);
				}
			}
			if (flag)
			{
				List<UITexture> list = m_chaoTextureObject[num];
				if (list != null && list.Count > 0)
				{
					foreach (UITexture item in list)
					{
						if (item != null)
						{
							item.mainTexture = tex;
							item.alpha = 1f;
						}
					}
				}
				m_chaoTextureObject.Remove(num);
			}
		}
		if (num >= 0 && m_chaoTextureList != null && !m_chaoTextureList.ContainsKey(num))
		{
			m_chaoTextureList.Add(num, tex);
		}
		int num2 = -1;
		if (m_chaoTextureLoad == null || m_chaoTextureLoad.Count <= 0)
		{
			return;
		}
		int[] array2 = new int[m_chaoTextureLoad.Count];
		m_chaoTextureLoad.Keys.CopyTo(array2, 0);
		int[] array3 = array2;
		foreach (int num3 in array3)
		{
			if (!m_chaoTextureList.ContainsKey(num3))
			{
				num2 = num3;
				break;
			}
		}
		if (num2 >= 0)
		{
			ChaoTextureManager.CallbackInfo.LoadFinishCallback callback = ChaoLoadFinishCallback;
			ChaoTextureManager.CallbackInfo info = new ChaoTextureManager.CallbackInfo(null, callback);
			ChaoTextureManager.Instance.GetTexture(num, info);
		}
	}
}
