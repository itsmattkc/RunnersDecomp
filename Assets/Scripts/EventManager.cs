using SaveData;
using System;
using System.Collections.Generic;
using System.IO;
using System.Xml.Serialization;
using UnityEngine;

public class EventManager : MonoBehaviour
{
	public enum EventType
	{
		SPECIAL_STAGE = 0,
		RAID_BOSS = 1,
		COLLECT_OBJECT = 2,
		GACHA = 3,
		ADVERT = 4,
		QUICK = 5,
		BGM = 6,
		NUM = 7,
		UNKNOWN = -1
	}

	public enum CollectEventType
	{
		GET_ANIMALS = 0,
		GET_RING = 1,
		RUN_DISTANCE = 2,
		NUM = 3,
		UNKNOWN = -1
	}

	public enum AdvertEventType
	{
		ROULETTE = 0,
		CHARACTER = 1,
		SHOP = 2,
		NUM = 3,
		UNKNOWN = -1
	}

	[Serializable]
	public class DebugRaidBoss
	{
		[SerializeField]
		public int m_id;

		[SerializeField]
		public int m_level;

		[SerializeField]
		public int m_rarity;

		[SerializeField]
		public int m_hp = 1;

		[SerializeField]
		public int m_hpMax = 1;

		[SerializeField]
		public string m_discovererName;

		[SerializeField]
		public int m_endTimeMinutes = 60;

		[SerializeField]
		public int m_state;

		[SerializeField]
		public bool m_findMyself = true;

		[SerializeField]
		public bool m_validFlag;
	}

	[Serializable]
	public class DebugRaidBossInfo
	{
		[Header("レイドボスの基本データ")]
		[SerializeField]
		public int m_raidBossRingNum;

		[SerializeField]
		public int m_raidBossKillNum;

		[SerializeField]
		public int m_raidBossChallengeNum;

		[SerializeField]
		[Header("レイドボスに挑むときのチャレンジ使用数")]
		public int m_raidBossUseChallengeNum = 1;

		[Header("レイドボス情報リスト(データが足りないようなら、クラスの変数を足してください)")]
		[SerializeField]
		public DebugRaidBoss[] m_debugRaidBossDatas = new DebugRaidBoss[6];

		[SerializeField]
		[Header("レイドボス情報リストの指定番号を使用して、レイドボス戦(レイドボスステージ用)")]
		public int m_debugCurrentRaidBossDataIndex;

		[SerializeField]
		[Header("レイドボスの襲来フラグ(通常ステージ用)")]
		public bool m_debugRaidBossDescentFlag;
	}

	private const int EVENT_TYPE_COFFI = 100000000;

	private const int SPECIFIC_TYPE_COFFI = 10000;

	private const int NUMBER_OF_TIMES_COFFI = 100;

	private const int DEBUG_RAID_BOSS_COUNT = 6;

	private static readonly string[] EventTypeName = new string[7]
	{
		"SpecialStage",
		"RaidBoss",
		"CollectObject",
		"Gacha",
		"Advert",
		"Quick",
		"BGM"
	};

	private static readonly int[] COLLECT_EVENT_SPECIFIC_ID = new int[3]
	{
		1,
		2,
		3
	};

	private static EventManager instance = null;

	[Header("debugFlag にチェックを入れると、指定した時間で始められます")]
	[SerializeField]
	private bool m_debugFlag;

	[SerializeField]
	[Header("eventId の詳細はwiki[イベントIDルール]を見てください。")]
	private int m_eventId = -1;

	[Header("イベントスタートまでの時間を設定")]
	[SerializeField]
	private int m_startTimeHours;

	[SerializeField]
	private int m_startTimeMinutes;

	[SerializeField]
	private int m_startTimeSeconds;

	[SerializeField]
	[Header("イベントの残り時間を設定")]
	private int m_endTimeHours;

	[SerializeField]
	private int m_endTimeMinutes;

	[SerializeField]
	private int m_endTimeSeconds;

	[Header("イベントのクローズまでの時間を設定")]
	[SerializeField]
	private int m_closeTimeHours;

	[SerializeField]
	private int m_closeTimeMinutes;

	[SerializeField]
	private int m_closeTimeSeconds;

	[Header("イベントのステージプレイ猶予時間を設定")]
	[SerializeField]
	private int m_endPlayingTimeMinutes = 25;

	[SerializeField]
	[Header("イベントのステージリザルト猶予時間を設定")]
	private int m_endResultTimeMinutes = 30;

	[Header("Debug レイドボスインフォ(データが足りないようなら、変数を足してください)")]
	[SerializeField]
	private DebugRaidBossInfo m_debugRaidBossInfo = new DebugRaidBossInfo();

	private int m_specificId = -1;

	private int m_numberOfTimes = -1;

	private int m_reservedId;

	private int m_useRaidBossEnergy;

	private long m_collectCount;

	private EventType m_eventType = EventType.UNKNOWN;

	private EventType m_standbyType = EventType.UNKNOWN;

	private CollectEventType m_collectType = CollectEventType.UNKNOWN;

	private AdvertEventType m_advertType = AdvertEventType.UNKNOWN;

	private bool m_eventStage;

	private bool m_setEventInfo;

	private bool m_appearRaidBoss;

	private bool m_synchFlag;

	private List<EventMenuData> m_datas;

	private List<RaidBossAttackRateTable> m_raidBossAttackRateList;

	private SpecialStageInfo m_specialStageInfo;

	private RaidBossInfo m_raidBossInfo;

	private EtcEventInfo m_etcEventInfo;

	private DateTime m_startTime = DateTime.MinValue;

	private DateTime m_endTime = DateTime.MinValue;

	private DateTime m_endPlayTime = DateTime.MinValue;

	private DateTime m_endResultTime = DateTime.MinValue;

	private DateTime m_closeTime = DateTime.MinValue;

	private List<ServerEventEntry> m_entryList = new List<ServerEventEntry>();

	private List<ServerEventReward> m_rewardList = new List<ServerEventReward>();

	private List<ServerEventRaidBossState> m_userRaidBossList = new List<ServerEventRaidBossState>();

	private ServerEventState m_state = new ServerEventState();

	private ServerEventUserRaidBossState m_raidState = new ServerEventUserRaidBossState();

	private ServerEventRaidBossBonus m_raidBossBonus = new ServerEventRaidBossBonus();

	public static EventManager Instance
	{
		get
		{
			return instance;
		}
	}

	public ServerEventState State
	{
		get
		{
			return m_state;
		}
	}

	public List<ServerEventReward> RewardList
	{
		get
		{
			return m_rewardList;
		}
	}

	public DateTime EventEndTime
	{
		get
		{
			return m_endTime;
		}
	}

	public List<ServerEventRaidBossState> UserRaidBossList
	{
		get
		{
			return m_userRaidBossList;
		}
	}

	public bool AppearRaidBoss
	{
		get
		{
			return m_appearRaidBoss;
		}
		set
		{
			m_appearRaidBoss = value;
		}
	}

	public ServerEventRaidBossBonus RaidBossBonus
	{
		get
		{
			return m_raidBossBonus;
		}
		set
		{
			m_raidBossBonus = value;
		}
	}

	public DateTime EventCloseTime
	{
		get
		{
			return m_closeTime;
		}
	}

	public int Id
	{
		get
		{
			return m_eventId;
		}
		set
		{
			m_eventId = value;
		}
	}

	public EventType Type
	{
		get
		{
			return m_eventType;
		}
	}

	public EventType StandbyType
	{
		get
		{
			return m_standbyType;
		}
	}

	public EventType TypeInTime
	{
		get
		{
			if (m_eventType != EventType.UNKNOWN && IsInEvent())
			{
				return m_eventType;
			}
			return EventType.UNKNOWN;
		}
	}

	public AdvertEventType AdvertType
	{
		get
		{
			return m_advertType;
		}
	}

	public int NumberOfTimes
	{
		get
		{
			return m_numberOfTimes;
		}
	}

	public int ReservedId
	{
		get
		{
			return m_reservedId;
		}
	}

	public CollectEventType CollectType
	{
		get
		{
			return m_collectType;
		}
	}

	public long CollectCount
	{
		get
		{
			return m_collectCount;
		}
		set
		{
			m_collectCount = value;
		}
	}

	public bool EventStage
	{
		get
		{
			return m_eventStage;
		}
		set
		{
			m_eventStage = value;
		}
	}

	public SpecialStageInfo SpecialStageInfo
	{
		get
		{
			return m_specialStageInfo;
		}
	}

	public RaidBossInfo RaidBossInfo
	{
		get
		{
			return m_raidBossInfo;
		}
	}

	public EtcEventInfo EtcEventInfo
	{
		get
		{
			return m_etcEventInfo;
		}
	}

	public bool IsSetEventStateInfo
	{
		get
		{
			return m_setEventInfo;
		}
	}

	public List<ServerChaoData> RecommendedChaos
	{
		get
		{
			return null;
		}
	}

	public ServerEventUserRaidBossState RaidBossState
	{
		get
		{
			return m_raidState;
		}
	}

	public int RaidbossChallengeCount
	{
		get
		{
			if (m_raidState != null && m_raidState.RaidBossEnergyCount >= 0)
			{
				return m_raidState.RaidBossEnergyCount;
			}
			return 0;
		}
	}

	public int UseRaidbossChallengeCount
	{
		get
		{
			return m_useRaidBossEnergy;
		}
		set
		{
			m_useRaidBossEnergy = value;
		}
	}

	public bool IsStandby()
	{
		return m_standbyType != EventType.UNKNOWN;
	}

	public bool IsInEvent()
	{
		return CheckCloseTime();
	}

	public bool IsChallengeEvent()
	{
		if (IsInEvent())
		{
			return CheckEndTime();
		}
		return false;
	}

	public bool IsPlayEventForStage()
	{
		return CheckPlayingTime();
	}

	public bool IsResultEvent()
	{
		return CheckResultTime();
	}

	public bool IsCautionPlayEvent()
	{
		if (IsChallengeEvent())
		{
			return (m_endTime - NetBase.GetCurrentTime()).TotalSeconds < 1800.0;
		}
		return false;
	}

	public static EventType GetType(int id)
	{
		if (id > 0)
		{
			switch (id / 100000000)
			{
			case 1:
				return EventType.SPECIAL_STAGE;
			case 2:
				return EventType.RAID_BOSS;
			case 3:
				return EventType.COLLECT_OBJECT;
			case 4:
				return EventType.GACHA;
			case 5:
				return EventType.ADVERT;
			case 6:
				return EventType.QUICK;
			case 7:
				return EventType.BGM;
			}
		}
		return EventType.UNKNOWN;
	}

	public static CollectEventType GetCollectEventType(int id)
	{
		EventType type = GetType(id);
		if (type == EventType.COLLECT_OBJECT)
		{
			int num = id % 100000000;
			num /= 10000;
			for (int i = 0; i < 3; i++)
			{
				if (num == COLLECT_EVENT_SPECIFIC_ID[i])
				{
					return (CollectEventType)i;
				}
			}
		}
		return CollectEventType.UNKNOWN;
	}

	public static bool IsVaildEvent(int id)
	{
		if (id > 0)
		{
			int num = id / 10000;
			if (instance != null)
			{
				int num2 = instance.Id / 10000;
				return num2 == num;
			}
		}
		return false;
	}

	public static int GetSpecificId()
	{
		if (instance != null)
		{
			return instance.Id / 10000;
		}
		return -1;
	}

	public static int GetSpecificId(int eventId)
	{
		return eventId / 10000;
	}

	public static string GetResourceName()
	{
		int specificId = GetSpecificId();
		if (instance != null)
		{
			return instance.GetEventTypeName() + "_" + specificId;
		}
		return string.Empty;
	}

	public bool IsQuickEvent()
	{
		return m_eventType == EventType.QUICK && IsInEvent();
	}

	public bool IsBGMEvent()
	{
		return m_eventType == EventType.BGM && IsInEvent();
	}

	public bool IsSpecialStage()
	{
		return m_eventStage && m_eventType == EventType.SPECIAL_STAGE;
	}

	public bool IsRaidBossStage()
	{
		return m_eventStage && m_eventType == EventType.RAID_BOSS;
	}

	public bool IsCollectEvent()
	{
		return m_eventStage && m_eventType == EventType.COLLECT_OBJECT;
	}

	public bool IsGetAnimalStage()
	{
		if (IsCollectEvent())
		{
			return m_collectType == CollectEventType.GET_ANIMALS;
		}
		return false;
	}

	public bool IsEncounterRaidBoss()
	{
		foreach (ServerEventRaidBossState userRaidBoss in m_userRaidBossList)
		{
			if (userRaidBoss.Encounter && userRaidBoss.GetStatusType() != ServerEventRaidBossState.StatusType.PROCESS_END)
			{
				return true;
			}
		}
		return false;
	}

	public string GetEventTypeName()
	{
		return GetEventTypeName(m_eventType);
	}

	public static string GetEventTypeName(EventType type)
	{
		if ((uint)type < 7u)
		{
			return EventTypeName[(int)type];
		}
		return string.Empty;
	}

	public int GetCollectEventSpecificId(CollectEventType type)
	{
		if ((uint)type < 3u)
		{
			return COLLECT_EVENT_SPECIFIC_ID[(int)type];
		}
		return -1;
	}

	public WindowEventData GetWindowEvenData(int texWindowId)
	{
		if (m_datas != null && m_datas.Count > 0)
		{
			WindowEventData[] window_data = m_datas[0].window_data;
			foreach (WindowEventData windowEventData in window_data)
			{
				if (windowEventData.id == texWindowId)
				{
					return windowEventData;
				}
			}
		}
		return null;
	}

	public EventStageData GetStageData()
	{
		if (m_datas != null && m_datas.Count > 0)
		{
			return m_datas[0].stage_data;
		}
		return null;
	}

	public EyeCatcherChaoData[] GetEyeCatcherChaoDatas()
	{
		if (m_datas != null && m_datas.Count > 0 && m_datas[0].chao_data != null)
		{
			return m_datas[0].chao_data.eyeCatchers;
		}
		return null;
	}

	public RewardChaoData GetRewardChaoData()
	{
		if (m_datas != null && m_datas.Count > 0 && m_datas[0].chao_data != null && m_datas[0].chao_data.rewards != null && m_datas[0].chao_data.rewards.Length > 0)
		{
			return m_datas[0].chao_data.rewards[0];
		}
		return null;
	}

	public EyeCatcherCharaData[] GetEyeCatcherCharaDatas()
	{
		if (m_datas != null && m_datas.Count > 0 && m_datas[0].chao_data != null)
		{
			return m_datas[0].chao_data.charaEyeCatchers;
		}
		return null;
	}

	public EventAvertData GetAvertData()
	{
		if (m_datas != null && m_datas.Count > 0)
		{
			return m_datas[0].advert_data;
		}
		return null;
	}

	public float GetRaidAttackRate(int useChallengeCount)
	{
		if (useChallengeCount > 0)
		{
			int num = useChallengeCount - 1;
			if (m_raidBossAttackRateList != null && m_raidBossAttackRateList[0].attackRate != null && num < m_raidBossAttackRateList[0].attackRate.Length)
			{
				return m_raidBossAttackRateList[0].attackRate[num];
			}
		}
		return 1f;
	}

	public void SetEventMenuData(TextAsset xml_data)
	{
		if (m_datas == null)
		{
			m_datas = new List<EventMenuData>();
		}
		else
		{
			m_datas.Clear();
		}
		string s = AESCrypt.Decrypt(xml_data.text);
		XmlSerializer xmlSerializer = new XmlSerializer(typeof(EventMenuData[]));
		StringReader textReader = new StringReader(s);
		EventMenuData[] array = (EventMenuData[])xmlSerializer.Deserialize(textReader);
		if (array != null && array.Length > 0 && m_datas != null)
		{
			m_datas.Add(array[0]);
		}
	}

	public void SetRaidBossAttacRate(TextAsset xml_data)
	{
		if (m_raidBossAttackRateList == null)
		{
			m_raidBossAttackRateList = new List<RaidBossAttackRateTable>();
		}
		else
		{
			m_raidBossAttackRateList.Clear();
		}
		string s = AESCrypt.Decrypt(xml_data.text);
		XmlSerializer xmlSerializer = new XmlSerializer(typeof(RaidBossAttackRateTable[]));
		StringReader textReader = new StringReader(s);
		RaidBossAttackRateTable[] array = (RaidBossAttackRateTable[])xmlSerializer.Deserialize(textReader);
		if (array != null && array.Length > 0 && m_raidBossAttackRateList != null)
		{
			m_raidBossAttackRateList.Add(array[0]);
		}
	}

	public void SetParameter()
	{
		m_eventType = EventType.UNKNOWN;
		m_standbyType = EventType.UNKNOWN;
		SetCurrentEvent();
		CalcParameter();
		m_synchFlag = true;
	}

	public void SetState(ServerEventState state)
	{
		if (state != null)
		{
			state.CopyTo(m_state);
			m_collectCount = m_state.Param;
			m_setEventInfo = true;
		}
	}

	public void ReCalcEndPlayTime()
	{
		if (m_eventType == EventType.RAID_BOSS)
		{
			m_endPlayingTimeMinutes = UnityEngine.Random.Range(4, 9);
			m_endPlayTime = m_endTime + new TimeSpan(0, m_endPlayingTimeMinutes, 0);
		}
		else
		{
			m_endPlayingTimeMinutes = UnityEngine.Random.Range(24, 29);
			m_endPlayTime = m_endTime + new TimeSpan(0, m_endPlayingTimeMinutes, 0);
		}
	}

	public void SetDebugParameter()
	{
		m_endTimeHours = 72;
		m_debugFlag = true;
		SetParameter();
	}

	public void SetEventInfo()
	{
		switch (m_eventType)
		{
		case EventType.SPECIAL_STAGE:
			if (m_debugFlag)
			{
				if (m_specialStageInfo == null)
				{
					m_specialStageInfo = SpecialStageInfo.CreateDummyData();
				}
			}
			else
			{
				m_specialStageInfo = SpecialStageInfo.CreateData();
			}
			break;
		case EventType.RAID_BOSS:
			if (m_debugFlag)
			{
				if (m_raidBossInfo == null)
				{
					DebugSetRaidBossData();
				}
			}
			else
			{
				SetRaidBossData();
			}
			break;
		case EventType.COLLECT_OBJECT:
			if (m_debugFlag)
			{
				if (m_etcEventInfo == null)
				{
					m_etcEventInfo = EtcEventInfo.CreateDummyData();
				}
			}
			else
			{
				m_etcEventInfo = EtcEventInfo.CreateData();
			}
			break;
		}
	}

	public void CheckEvent()
	{
		if (IsStandby())
		{
			if (IsInEvent())
			{
				SetParameter();
			}
			else if (m_closeTime < NetBase.GetCurrentTime())
			{
				ResetParameter();
				SetParameter();
			}
		}
		else
		{
			if (m_eventType == EventType.UNKNOWN)
			{
				return;
			}
			if (IsInEvent())
			{
				if (m_eventType == EventType.ADVERT && IsStartOtherEvent())
				{
					ResetParameter();
					SetParameter();
				}
			}
			else
			{
				ResetParameter();
				SetParameter();
			}
		}
	}

	public void ResetData()
	{
		if (m_datas != null)
		{
			m_datas.Clear();
			m_datas = null;
		}
		if (m_raidBossInfo != null)
		{
			m_raidBossInfo = null;
		}
		if (m_etcEventInfo != null)
		{
			m_etcEventInfo = null;
		}
		if (m_specialStageInfo != null)
		{
			m_specialStageInfo = null;
		}
		m_setEventInfo = false;
		m_eventStage = false;
		m_appearRaidBoss = false;
	}

	protected void Awake()
	{
		SetInstance();
	}

	private void Start()
	{
		base.enabled = false;
		SynchServerEntryList();
	}

	private void OnDestroy()
	{
		if (instance == this)
		{
			instance = null;
		}
	}

	public void SynchServerEntryList()
	{
		m_synchFlag = false;
		m_entryList.Clear();
		List<ServerEventEntry> eventEntryList = ServerInterface.EventEntryList;
		if (eventEntryList != null)
		{
			int count = eventEntryList.Count;
			for (int i = 0; i < count; i++)
			{
				m_entryList.Add(eventEntryList[i]);
			}
		}
		SetParameter();
	}

	public void SynchServerEventState()
	{
		ServerEventState eventState = ServerInterface.EventState;
		if (eventState != null)
		{
			eventState.CopyTo(m_state);
			m_collectCount = m_state.Param;
			m_setEventInfo = true;
		}
	}

	public void SynchServerRewardList()
	{
		m_rewardList.Clear();
		List<ServerEventReward> eventRewardList = ServerInterface.EventRewardList;
		if (eventRewardList != null)
		{
			int count = eventRewardList.Count;
			for (int i = 0; i < count; i++)
			{
				m_rewardList.Add(eventRewardList[i]);
			}
		}
	}

	public void SynchServerEventRaidBossList(List<ServerEventRaidBossState> raidBossList)
	{
		m_userRaidBossList.Clear();
		if (raidBossList != null)
		{
			int count = raidBossList.Count;
			bool flag = false;
			for (int i = 0; i < count; i++)
			{
				m_userRaidBossList.Add(raidBossList[i]);
				EventUtility.SetRaidbossEntry(raidBossList[i]);
				if (raidBossList[i] != null && RaidBossInfo.currentRaidData != null && RaidBossInfo.currentRaidData.id == raidBossList[i].Id)
				{
					RaidBossInfo.currentRaidData = new RaidBossData(raidBossList[i]);
					flag = true;
				}
			}
			if (!flag && RaidBossInfo.currentRaidData != null)
			{
				RaidBossInfo.currentRaidData = null;
			}
		}
		SetRaidBossData();
	}

	public void SynchServerEventRaidBossList(ServerEventRaidBossState raidBossState)
	{
		if (raidBossState != null)
		{
			bool flag = true;
			foreach (ServerEventRaidBossState userRaidBoss in m_userRaidBossList)
			{
				if (userRaidBoss.Id == raidBossState.Id)
				{
					raidBossState.CopyTo(userRaidBoss);
					flag = false;
					break;
				}
			}
			if (flag)
			{
				m_userRaidBossList.Add(raidBossState);
				EventUtility.SetRaidbossEntry(raidBossState);
			}
		}
		SetRaidBossData();
	}

	public void SynchServerEventUserRaidBossState(ServerEventUserRaidBossState state)
	{
		if (state != null)
		{
			state.CopyTo(m_raidState);
		}
		if (m_raidBossInfo != null)
		{
			m_raidBossInfo.raidRing = m_raidState.NumRaidbossRings;
			m_raidBossInfo.totalDestroyCount = m_raidState.NumBeatedEnterprise;
		}
	}

	public void SynchServerEventRaidBossUserList(List<ServerEventRaidBossUserState> userList, long raidBossId, ServerEventRaidBossBonus bonus)
	{
		RaidBossData raidBossData = null;
		if (m_raidBossInfo != null)
		{
			List<RaidBossData> raidData = m_raidBossInfo.raidData;
			if (raidData != null)
			{
				foreach (RaidBossData item in raidData)
				{
					if (item.id == raidBossId)
					{
						item.SetUserList(userList);
						item.SetReward(bonus);
						raidBossData = item;
						break;
					}
				}
			}
		}
		if (raidBossData != null && RaidBossInfo.currentRaidData != null && RaidBossInfo.currentRaidData.id == raidBossId)
		{
			RaidBossInfo.currentRaidData = raidBossData;
		}
	}

	private void SetInstance()
	{
		if (instance == null)
		{
			UnityEngine.Object.DontDestroyOnLoad(base.gameObject);
			instance = this;
		}
		else
		{
			UnityEngine.Object.Destroy(base.gameObject);
		}
	}

	private void CalcParameter()
	{
		if (m_eventType == EventType.UNKNOWN || m_eventType >= EventType.NUM)
		{
			return;
		}
		m_specificId = m_eventId % 100000000;
		m_specificId /= 10000;
		m_numberOfTimes = m_eventId % 10000;
		m_numberOfTimes /= 100;
		m_reservedId = m_eventId % 100;
		if (m_eventType == EventType.COLLECT_OBJECT)
		{
			SetCollectEventType();
		}
		else if (m_eventType == EventType.SPECIAL_STAGE || m_eventType == EventType.RAID_BOSS)
		{
			if (SystemSaveManager.Instance != null)
			{
				SystemData systemdata = SystemSaveManager.Instance.GetSystemdata();
				if (systemdata != null && systemdata.pictureShowEventId != m_eventId)
				{
					systemdata.pictureShowEventId = m_eventId;
					systemdata.pictureShowProgress = -1;
					systemdata.pictureShowEmergeRaidBossProgress = -1;
					systemdata.pictureShowRaidBossFirstBattle = -1;
					SystemSaveManager.Instance.SaveSystemData();
				}
			}
		}
		else if (m_eventType == EventType.ADVERT)
		{
			SetAdvertEventType();
		}
	}

	private void SetCurrentEvent()
	{
		ServerEventEntry serverEventEntry = null;
		ServerEventEntry serverEventEntry2 = null;
		DateTime currentTime = NetBase.GetCurrentTime();
		foreach (ServerEventEntry entry in m_entryList)
		{
			DateTime eventCloseTime = entry.EventCloseTime;
			if (!(eventCloseTime > currentTime))
			{
				continue;
			}
			EventType type = GetType(entry.EventId);
			if (type == EventType.ADVERT)
			{
				if (serverEventEntry2 == null)
				{
					serverEventEntry2 = entry;
				}
				else if (entry.EventStartTime < serverEventEntry2.EventStartTime)
				{
					serverEventEntry2 = entry;
				}
			}
			else if (serverEventEntry == null)
			{
				serverEventEntry = entry;
			}
			else if (entry.EventStartTime < serverEventEntry.EventStartTime)
			{
				serverEventEntry = entry;
			}
		}
		bool flag = false;
		if (serverEventEntry != null && serverEventEntry2 != null)
		{
			if (serverEventEntry2.EventStartTime < serverEventEntry.EventStartTime)
			{
				DateTime eventStartTime = serverEventEntry.EventStartTime;
				if (eventStartTime > currentTime)
				{
					flag = true;
				}
			}
		}
		else if (serverEventEntry == null && serverEventEntry2 != null)
		{
			flag = true;
		}
		ServerEventEntry serverEventEntry3 = (!flag) ? serverEventEntry : serverEventEntry2;
		if (serverEventEntry3 == null)
		{
			return;
		}
		m_startTime = serverEventEntry3.EventStartTime;
		m_endTime = serverEventEntry3.EventEndTime;
		m_closeTime = serverEventEntry3.EventCloseTime;
		m_eventId = serverEventEntry3.EventId;
		if (IsInEvent())
		{
			m_eventType = GetType(m_eventId);
			ReCalcEndPlayTime();
			if (m_eventType == EventType.RAID_BOSS)
			{
				m_endResultTimeMinutes = 10;
				m_endResultTime = m_endTime + new TimeSpan(0, m_endResultTimeMinutes, 0);
			}
			else
			{
				m_endResultTimeMinutes = 30;
				m_endResultTime = m_endTime + new TimeSpan(0, m_endResultTimeMinutes, 0);
			}
		}
		else
		{
			m_standbyType = GetType(m_eventId);
		}
	}

	private bool IsStartOtherEvent()
	{
		ServerEventEntry serverEventEntry = null;
		DateTime currentTime = NetBase.GetCurrentTime();
		foreach (ServerEventEntry entry in m_entryList)
		{
			if (GetType(entry.EventId) == EventType.ADVERT)
			{
				continue;
			}
			DateTime eventEndTime = entry.EventEndTime;
			if (eventEndTime > currentTime)
			{
				if (serverEventEntry == null)
				{
					serverEventEntry = entry;
				}
				else if (entry.EventStartTime < serverEventEntry.EventStartTime)
				{
					serverEventEntry = entry;
				}
			}
		}
		if (serverEventEntry != null)
		{
			DateTime eventStartTime = serverEventEntry.EventStartTime;
			if (eventStartTime <= currentTime)
			{
				return true;
			}
		}
		return false;
	}

	private void DebugSetCurrentEvent()
	{
		if (!m_debugFlag || m_eventId <= 0)
		{
			return;
		}
		if (!m_synchFlag && m_endTimeHours + m_endTimeMinutes + m_endTimeSeconds > 0)
		{
			TimeSpan timeSpan = new TimeSpan(m_endTimeHours, m_endTimeMinutes, m_endTimeSeconds);
			TimeSpan timeSpan2 = new TimeSpan(m_closeTimeHours, m_closeTimeMinutes, m_closeTimeSeconds);
			if (timeSpan > timeSpan2)
			{
				timeSpan2 = timeSpan;
			}
			TimeSpan t = new TimeSpan(m_startTimeHours, m_startTimeMinutes, m_startTimeSeconds);
			m_startTime = NetBase.GetCurrentTime() + t;
			m_endTime = m_startTime + timeSpan;
			m_closeTime = m_startTime + timeSpan2;
			m_endPlayTime = m_endTime + new TimeSpan(0, m_endPlayingTimeMinutes, 0);
			m_endResultTime = m_endTime + new TimeSpan(0, m_endResultTimeMinutes, 0);
		}
		if (IsInEvent())
		{
			m_eventType = GetType(m_eventId);
		}
		else
		{
			m_standbyType = GetType(m_eventId);
		}
	}

	private void SetRaidBossData()
	{
		if (m_eventType != EventType.RAID_BOSS)
		{
			return;
		}
		if (m_raidBossInfo != null)
		{
			if (m_raidBossInfo.raidData != null)
			{
				m_raidBossInfo.raidData.Clear();
				foreach (ServerEventRaidBossState userRaidBoss in m_userRaidBossList)
				{
					m_raidBossInfo.raidData.Add(new RaidBossData(userRaidBoss));
				}
			}
		}
		else
		{
			List<RaidBossData> list = new List<RaidBossData>();
			foreach (ServerEventRaidBossState userRaidBoss2 in m_userRaidBossList)
			{
				list.Add(new RaidBossData(userRaidBoss2));
			}
			m_raidBossInfo = RaidBossInfo.CreateData(list);
		}
		if (m_raidBossInfo != null)
		{
			m_raidBossInfo.raidRing = m_raidState.NumRaidbossRings;
			m_raidBossInfo.totalDestroyCount = m_raidState.NumBeatedEnterprise;
		}
		m_setEventInfo = true;
	}

	private void DebugSetRaidBossData()
	{
		if (m_eventType != EventType.RAID_BOSS)
		{
			return;
		}
		List<RaidBossData> list = new List<RaidBossData>();
		DebugRaidBoss[] debugRaidBossDatas = m_debugRaidBossInfo.m_debugRaidBossDatas;
		foreach (DebugRaidBoss debugRaidBoss in debugRaidBossDatas)
		{
			if (debugRaidBoss.m_validFlag)
			{
				ServerEventRaidBossState serverEventRaidBossState = new ServerEventRaidBossState();
				DateTime escapeAt = NetBase.GetCurrentTime() + new TimeSpan(0, debugRaidBoss.m_endTimeMinutes, 0);
				serverEventRaidBossState.Id = debugRaidBoss.m_id;
				serverEventRaidBossState.Rarity = debugRaidBoss.m_rarity;
				serverEventRaidBossState.Level = debugRaidBoss.m_level;
				serverEventRaidBossState.EncounterName = debugRaidBoss.m_discovererName;
				serverEventRaidBossState.Encounter = debugRaidBoss.m_findMyself;
				serverEventRaidBossState.Status = debugRaidBoss.m_state;
				serverEventRaidBossState.EscapeAt = escapeAt;
				serverEventRaidBossState.HitPoint = debugRaidBoss.m_hp;
				serverEventRaidBossState.MaxHitPoint = debugRaidBoss.m_hpMax;
				list.Add(new RaidBossData(serverEventRaidBossState));
			}
		}
		m_setEventInfo = true;
		m_raidBossInfo = RaidBossInfo.CreateDataForDebugData(list);
		if (m_raidBossInfo != null)
		{
			int debugCurrentRaidBossDataIndex = m_debugRaidBossInfo.m_debugCurrentRaidBossDataIndex;
			if (m_raidBossInfo.raidData.Count > debugCurrentRaidBossDataIndex && debugCurrentRaidBossDataIndex >= 0)
			{
				RaidBossInfo.currentRaidData = m_raidBossInfo.raidData[debugCurrentRaidBossDataIndex];
			}
			m_appearRaidBoss = m_debugRaidBossInfo.m_debugRaidBossDescentFlag;
			m_raidBossInfo.raidRing = m_debugRaidBossInfo.m_raidBossRingNum;
			m_raidBossInfo.totalDestroyCount = m_debugRaidBossInfo.m_raidBossKillNum;
		}
		if (m_raidState != null)
		{
			m_raidState.RaidBossEnergy = 20;
		}
	}

	private void ResetParameter()
	{
		ResetData();
		m_eventType = EventType.UNKNOWN;
		m_standbyType = EventType.UNKNOWN;
		m_collectType = CollectEventType.UNKNOWN;
		m_advertType = AdvertEventType.UNKNOWN;
		m_eventId = -1;
		m_specificId = -1;
		m_numberOfTimes = -1;
		m_reservedId = -1;
		m_useRaidBossEnergy = 0;
		m_startTime = DateTime.MinValue;
		m_endTime = DateTime.MinValue;
		m_closeTime = DateTime.MinValue;
		m_endPlayTime = DateTime.MinValue;
		m_endResultTime = DateTime.MinValue;
	}

	private bool CheckCloseTime()
	{
		DateTime currentTime = NetBase.GetCurrentTime();
		if (currentTime >= m_startTime)
		{
			return m_closeTime > currentTime;
		}
		return false;
	}

	private bool CheckEndTime()
	{
		DateTime currentTime = NetBase.GetCurrentTime();
		if (currentTime >= m_startTime)
		{
			return m_endTime > currentTime;
		}
		return false;
	}

	private bool CheckPlayingTime()
	{
		if (m_eventType != EventType.UNKNOWN)
		{
			DateTime currentTime = NetBase.GetCurrentTime();
			if (currentTime >= m_startTime)
			{
				return m_endPlayTime > currentTime;
			}
		}
		return false;
	}

	private bool CheckResultTime()
	{
		if (m_eventType != EventType.UNKNOWN)
		{
			DateTime currentTime = NetBase.GetCurrentTime();
			if (currentTime >= m_startTime)
			{
				return m_endResultTime > currentTime;
			}
		}
		return false;
	}

	private void SetCollectEventType()
	{
		m_collectType = CollectEventType.GET_ANIMALS;
		for (int i = 0; i < 3; i++)
		{
			if (COLLECT_EVENT_SPECIFIC_ID[i] == m_specificId)
			{
				m_collectType = (CollectEventType)i;
				break;
			}
		}
	}

	private void SetAdvertEventType()
	{
		m_advertType = AdvertEventType.UNKNOWN;
		if (m_specificId < 1000)
		{
			m_advertType = AdvertEventType.ROULETTE;
		}
		else if (m_specificId < 2000)
		{
			m_advertType = AdvertEventType.CHARACTER;
		}
		else if (m_specificId < 3000)
		{
			m_advertType = AdvertEventType.SHOP;
		}
	}

	public EventProductionData GetPuductionData()
	{
		if (m_datas != null && m_datas.Count > 0)
		{
			return m_datas[0].puduction_data;
		}
		return null;
	}

	public EventRaidProductionData GetRaidProductionData()
	{
		if (m_datas != null && m_datas.Count > 0)
		{
			return m_datas[0].raid_data;
		}
		return null;
	}
}
