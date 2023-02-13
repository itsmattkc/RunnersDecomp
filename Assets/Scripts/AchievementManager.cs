using DataTable;
using SaveData;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SocialPlatforms;
using UnityEngine.SocialPlatforms.GameCenter;

public class AchievementManager : MonoBehaviour
{
	private enum State
	{
		Idle,
		Authenticate,
		AuthenticateError,
		AuthenticateSkip,
		LoadAchievement,
		LoadAchievementError,
		Report,
		RequestIncentive,
		RequestEnd
	}

	public bool m_debugInfo = true;

	private bool m_debugInfo2;

	public bool m_debugAllOpen;

	private State m_state;

	private int m_reportCount;

	private int m_reportSuccessCount;

	private List<AchievementTempData> m_loadData = new List<AchievementTempData>();

	private List<AchievementTempData> m_clearData = new List<AchievementTempData>();

	private List<AchievementData> m_data = new List<AchievementData>();

	private bool m_setupDataTable;

	private float m_waitTime;

	private static float WAIT_TIME = 10f;

	private bool m_connectAnim;

	private AchievementIncentive m_achievementIncentive;

	private static AchievementManager m_instance;

	public static AchievementManager Instance
	{
		get
		{
			return m_instance;
		}
	}

	private void Awake()
	{
		if (m_instance == null)
		{
			Object.DontDestroyOnLoad(base.gameObject);
			m_instance = this;
			base.gameObject.AddComponent<HudNetworkConnect>();
		}
		else
		{
			Object.Destroy(base.gameObject);
			DestroyDataTable();
		}
	}

	private void OnDestroy()
	{
		if (m_instance == this)
		{
			m_instance = null;
		}
	}

	private void Update()
	{
		if (m_state == State.Report && (m_reportCount == m_reportSuccessCount || IsRequestEndAchievement()))
		{
			SetDebugDraw("ReportResult m_reportCount=" + m_reportCount + " m_reportSuccessCount=" + m_reportSuccessCount);
			SetNetworkConnect(false);
			RequestAchievementIncentive(m_reportSuccessCount);
			m_state = State.RequestIncentive;
		}
		if (m_state == State.RequestIncentive && IsRequestEndIncentive())
		{
			m_state = State.RequestEnd;
		}
		if (m_waitTime > 0f)
		{
			m_waitTime -= Time.deltaTime;
			if (m_waitTime < 0f)
			{
				SetNetworkConnect(false);
				m_waitTime = 0f;
			}
		}
	}

	public void RequestUpdateAchievement()
	{
		if (!IsSetupEndAchievement())
		{
			SetDebugDraw("RequestUpdateAchievement Not Setup m_state=" + m_state);
			return;
		}
		SetDebugDraw("RequestUpdateAchievement m_state=" + m_state);
		switch (m_state)
		{
		case State.Authenticate:
		case State.LoadAchievement:
		case State.Report:
		case State.RequestIncentive:
			break;
		case State.AuthenticateError:
		case State.AuthenticateSkip:
			break;
		case State.Idle:
			SetNetworkConnect(true);
			Authenticate();
			break;
		case State.LoadAchievementError:
			SetNetworkConnect(true);
			LoadAchievements();
			break;
		case State.RequestEnd:
			SetNetworkConnect(true);
			LoadAchievements2();
			break;
		}
	}

	public void RequestResetAchievement()
	{
		SetDebugDraw("RequestResetAchievement m_state=" + m_state);
		ResetAchievements();
	}

	public void ShowAchievementsUI()
	{
		SetDebugDraw("ShowAchievementsUI");
		Social.ShowAchievementsUI();
	}

	public bool IsSetupEndAchievement()
	{
		return m_setupDataTable;
	}

	public bool IsRequestEndAchievement()
	{
		if (IsSetupEndAchievement())
		{
			switch (m_state)
			{
			case State.Authenticate:
			case State.LoadAchievement:
			case State.Report:
			case State.RequestIncentive:
				if (m_waitTime.Equals(0f))
				{
					return true;
				}
				return false;
			}
		}
		return true;
	}

	private void Authenticate()
	{
		SetDebugDraw("Authenticate");
		m_state = State.Authenticate;
		SetWaitTime();
		if (!Social.localUser.authenticated)
		{
			Social.localUser.Authenticate(ProcessAuthentication);
		}
		else
		{
			ProcessAuthentication(false);
		}
	}

	private void LoadAchievements()
	{
		SetDebugDraw("LoadAchievements");
		m_state = State.LoadAchievement;
		SetWaitTime();
		ProcessLoadedAchievements1(null);
	}

	private void LoadAchievements2()
	{
		ProcessLoadedAchievements2(null);
	}

	private void ReportProgress()
	{
		SetDebugDraw("ReportProgress");
		m_state = State.Report;
		SetWaitTime();
		m_reportCount = 0;
		m_reportSuccessCount = 0;
		if (m_loadData.Count <= 0)
		{
			return;
		}
		UpdateAchievement();
		SetDebugDraw("m_loadData=" + m_loadData.Count + " m_clearData=" + m_clearData.Count);
		if (m_clearData.Count <= 0)
		{
			return;
		}
		foreach (AchievementTempData loadDatum in m_loadData)
		{
			if (!loadDatum.m_reportEnd && IsClearAchievement(loadDatum.m_id))
			{
				m_reportCount++;
				Social.ReportProgress(loadDatum.m_id, 100.0, ProcessReportProgress);
			}
		}
	}

	private void ResetAchievements()
	{
		SetDebugDraw("ResetAchievements");
		m_state = State.Idle;
		GameCenterPlatform.ResetAllAchievements(ProcessResetAllAchievements);
	}

	private void ProcessAuthentication(bool success)
	{
		if (success)
		{
			LoadAchievements();
			SystemSaveManager instance = SystemSaveManager.Instance;
			if (instance != null)
			{
				SystemData systemdata = SystemSaveManager.Instance.GetSystemdata();
				if (systemdata != null && systemdata.achievementCancelCount != 0)
				{
					systemdata.achievementCancelCount = 0;
					instance.SaveSystemData();
				}
			}
			return;
		}
		SetNetworkConnect(false);
		m_state = State.AuthenticateError;
		SetDebugDraw("Authenticate ERROR");
		SystemSaveManager instance2 = SystemSaveManager.Instance;
		if (instance2 != null && Application.loadedLevelName == TitleDefine.TitleSceneName)
		{
			SystemData systemdata2 = SystemSaveManager.Instance.GetSystemdata();
			if (systemdata2 != null)
			{
				systemdata2.achievementCancelCount++;
				instance2.SaveSystemData();
			}
		}
	}

	private void SetLoadedAchievements(string[] achievementsIDList)
	{
		if (achievementsIDList == null || achievementsIDList.Length == 0)
		{
			SetNetworkConnect(false);
			m_state = State.LoadAchievementError;
			SetDebugDraw("LoadAchievements ERROR");
			return;
		}
		SetDebugDraw("LoadAchievements1 OK " + achievementsIDList.Length + " achievements");
		m_loadData.Clear();
		foreach (string text in achievementsIDList)
		{
			if (text != null)
			{
				SetDebugDraw("Load achievementID=" + text);
				m_loadData.Add(new AchievementTempData(text));
			}
		}
		LoadAchievements2();
	}

	private void ProcessLoadedAchievements1(IAchievement[] achievements)
	{
		if (achievements == null || achievements.Length == 0)
		{
			SetLoadedAchievements(null);
			return;
		}
		string[] array = new string[achievements.Length];
		for (int i = 0; i < achievements.Length; i++)
		{
			array[i] = achievements[i].id;
		}
		SetLoadedAchievements(array);
	}

	private void ProcessLoadedAchievements2(IAchievement[] achievements)
	{
		if (achievements != null && achievements.Length > 0)
		{
			SetDebugDraw("LoadAchievements2 " + achievements.Length + " achievements");
			foreach (IAchievement achievement in achievements)
			{
				if (achievement != null)
				{
					SetReporteEnd(achievement.id);
				}
			}
		}
		ReportProgress();
	}

	private void ProcessReportProgress(bool result)
	{
		if (result)
		{
			m_reportSuccessCount++;
			SetDebugDraw("ReportProgress OK");
		}
		else
		{
			SetDebugDraw("ReportProgress ERROR");
		}
	}

	private void ProcessResetAllAchievements(bool result)
	{
		if (result)
		{
			SetDebugDraw("ProcessResetAllAchievements OK");
		}
		else
		{
			SetDebugDraw("ProcessResetAllAchievements ERROR");
		}
	}

	private void UpdateAchievement()
	{
		m_clearData.Clear();
		ServerPlayerState playerState = ServerInterface.PlayerState;
		if (playerState == null)
		{
			return;
		}
		SetDebugDraw2("DataTable m_data.Count=" + m_data.Count);
		for (int i = 0; i < m_data.Count; i++)
		{
			AchievementData achievementData = m_data[i];
			if (achievementData == null)
			{
				continue;
			}
			SetDebugDraw2("number=" + achievementData.number + " explanation=" + achievementData.explanation + " type=" + achievementData.type.ToString() + " itemID=" + achievementData.itemID + " value=" + achievementData.value + " id=" + achievementData.GetID());
			if (achievementData.GetID() != null && achievementData.GetID() != string.Empty)
			{
				bool flag = false;
				switch (achievementData.type)
				{
				case AchievementData.Type.ANIMAL:
					flag = CheckClearAnimal(achievementData, playerState.m_numAnimals);
					break;
				case AchievementData.Type.DISTANCE:
					flag = CheckClearDistance(achievementData, (uint)playerState.m_totalDistance);
					break;
				case AchievementData.Type.PLAYER_OPEN:
					flag = CheckClearPlayerOpen(achievementData, playerState.CharacterStateByItemID(achievementData.itemID));
					break;
				case AchievementData.Type.PLAYER_LEVEL:
					flag = CheckClearPlayerLevel(achievementData, playerState.CharacterStateByItemID(achievementData.itemID));
					break;
				case AchievementData.Type.CHAO_OPEN:
					flag = CheckClearChaoOpen(achievementData, playerState.ChaoStateByItemID(achievementData.itemID));
					break;
				case AchievementData.Type.CHAO_LEVEL:
					flag = CheckClearChaoLevel(achievementData, playerState.ChaoStateByItemID(achievementData.itemID));
					break;
				}
				string text = string.Empty;
				if (IsDebugAllOpen())
				{
					flag = true;
					text = "DebugAllOpen ";
				}
				if (flag)
				{
					SetDebugDraw2(text + "Clear!! ID=" + achievementData.GetID() + " / " + achievementData.explanation);
					m_clearData.Add(new AchievementTempData(achievementData.GetID()));
				}
			}
		}
	}

	private bool CheckClearAnimal(AchievementData data, int animal)
	{
		if (data != null)
		{
			SetDebugDraw2("animal=" + animal + " / data.value=" + data.value);
			if (IsClear((uint)animal, (uint)data.value))
			{
				return true;
			}
		}
		return false;
	}

	private bool CheckClearDistance(AchievementData data, uint distance)
	{
		if (data != null)
		{
			SetDebugDraw2("distance=" + distance + " / data.value=" + data.value);
			if (IsClear(distance, (uint)data.value))
			{
				return true;
			}
		}
		return false;
	}

	private bool CheckClearPlayerOpen(AchievementData data, ServerCharacterState state)
	{
		if (data != null && state != null)
		{
			SetDebugDraw2("player IsUnlocked=" + state.IsUnlocked);
			if (state.IsUnlocked)
			{
				return true;
			}
		}
		return false;
	}

	private bool CheckClearPlayerLevel(AchievementData data, ServerCharacterState state)
	{
		if (data != null && state != null)
		{
			SetDebugDraw2("player IsUnlocked=" + state.IsUnlocked.ToString() + " level=" + state.Level + " / data.value=" + data.value);
			if (state.IsUnlocked && IsClear((uint)state.Level, (uint)data.value))
			{
				return true;
			}
		}
		return false;
	}

	private bool CheckClearChaoOpen(AchievementData data, ServerChaoState state)
	{
		if (data != null && state != null)
		{
			SetDebugDraw2("chao IsOwned=" + state.IsOwned);
			if (state.IsOwned)
			{
				return true;
			}
		}
		return false;
	}

	private bool CheckClearChaoLevel(AchievementData data, ServerChaoState state)
	{
		if (data != null && state != null)
		{
			SetDebugDraw2("chao IsOwned=" + state.IsOwned.ToString() + " level=" + state.Level + " / data.value=" + data.value);
			if (state.IsOwned && IsClear((uint)state.Level, (uint)data.value))
			{
				return true;
			}
		}
		return false;
	}

	private bool IsClear(uint myParam, uint cmpParam)
	{
		if (myParam >= cmpParam)
		{
			return true;
		}
		return false;
	}

	private bool IsClearAchievement(string id)
	{
		foreach (AchievementTempData clearDatum in m_clearData)
		{
			if (clearDatum.m_id == id)
			{
				return true;
			}
		}
		return false;
	}

	private void SetReporteEnd(string id)
	{
		foreach (AchievementTempData loadDatum in m_loadData)
		{
			if (loadDatum.m_id == id)
			{
				loadDatum.m_reportEnd = true;
				break;
			}
		}
	}

	public void SetupDataAchievementTable()
	{
		if (m_setupDataTable || m_data.Count != 0)
		{
			return;
		}
		AchievementTable achievementTable = GameObjectUtil.FindGameObjectComponent<AchievementTable>("AchievementTable");
		if (achievementTable != null)
		{
			m_data.Clear();
			AchievementData[] dataTable = AchievementTable.GetDataTable();
			if (dataTable != null)
			{
				AchievementData[] array = dataTable;
				foreach (AchievementData item in array)
				{
					m_data.Add(item);
				}
			}
			SetDebugDraw("SetupDataAchievementTable m_data=" + m_data.Count);
			Object.Destroy(achievementTable.gameObject);
		}
		else
		{
			SetDebugDraw("SetupDataAchievementTable Error");
		}
		m_setupDataTable = true;
	}

	public void SkipAuthenticate()
	{
		m_state = State.AuthenticateSkip;
	}

	private void DestroyDataTable()
	{
		AchievementTable achievementTable = GameObjectUtil.FindGameObjectComponent<AchievementTable>("AchievementTable");
		if (achievementTable != null)
		{
			Object.Destroy(achievementTable.gameObject);
		}
	}

	private void SetWaitTime()
	{
		m_waitTime = WAIT_TIME;
	}

	private void RequestAchievementIncentive(int incentivCount)
	{
		if (incentivCount > 0)
		{
			AchievementIncentive.AddAchievementIncentiveCount(incentivCount);
		}
		if (m_achievementIncentive == null)
		{
			SetWaitTime();
			GameObject gameObject = new GameObject("AchievementIncentive");
			m_achievementIncentive = gameObject.AddComponent<AchievementIncentive>();
			if (m_achievementIncentive != null)
			{
				m_achievementIncentive.RequestServer();
			}
			SetDebugDraw("RequestAchievementIncentive RequestServer");
		}
	}

	private bool IsRequestEndIncentive()
	{
		if (m_achievementIncentive != null)
		{
			AchievementIncentive.State state = m_achievementIncentive.GetState();
			if (state == AchievementIncentive.State.Request)
			{
				return false;
			}
			Object.Destroy(m_achievementIncentive.gameObject);
			m_achievementIncentive = null;
			SetDebugDraw("IsRequestEndIncentive Destroy state=" + state);
		}
		return true;
	}

	private void SetNetworkConnect(bool on)
	{
		if (m_connectAnim == on)
		{
			return;
		}
		HudNetworkConnect component = base.gameObject.GetComponent<HudNetworkConnect>();
		if (component != null)
		{
			if (on)
			{
				SetDebugDraw("SetNetworkConnect PlayStart");
				component.Setup();
				component.PlayStart(HudNetworkConnect.DisplayType.ALL);
				m_connectAnim = true;
			}
			else
			{
				SetDebugDraw("SetNetworkConnect PlayEnd");
				component.PlayEnd();
				m_connectAnim = false;
			}
		}
	}

	private void SetDebugDraw(string msg)
	{
	}

	private void SetDebugDraw2(string msg)
	{
	}

	private bool IsDebugAllOpen()
	{
		return false;
	}

	public static AchievementManager GetAchievementManager()
	{
		AchievementManager achievementManager = Instance;
		if (achievementManager == null)
		{
			GameObject gameObject = new GameObject("AchievementManager");
			achievementManager = gameObject.AddComponent<AchievementManager>();
		}
		return achievementManager;
	}

	public static void Setup()
	{
		AchievementManager instance = Instance;
		if (instance != null)
		{
			instance.SetupDataAchievementTable();
		}
	}

	public static bool IsSetupEnd()
	{
		AchievementManager instance = Instance;
		if (instance != null)
		{
			return instance.IsSetupEndAchievement();
		}
		return true;
	}

	public static void RequestSkipAuthenticate()
	{
		AchievementManager instance = Instance;
		if (instance != null)
		{
			instance.SkipAuthenticate();
		}
	}

	public static void RequestUpdate()
	{
		AchievementManager achievementManager = GetAchievementManager();
		if (achievementManager != null)
		{
			achievementManager.RequestUpdateAchievement();
		}
	}

	public static bool IsRequestEnd()
	{
		AchievementManager instance = Instance;
		if (instance != null)
		{
			return instance.IsRequestEndAchievement();
		}
		return true;
	}

	public static void RequestReset()
	{
		AchievementManager achievementManager = GetAchievementManager();
		if (achievementManager != null)
		{
			achievementManager.RequestResetAchievement();
		}
	}

	public static void RequestShowAchievementsUI()
	{
		AchievementManager achievementManager = GetAchievementManager();
		if (achievementManager != null)
		{
			if (!Social.localUser.authenticated)
			{
				achievementManager.Authenticate();
			}
			achievementManager.ShowAchievementsUI();
		}
	}

	public static void RequestDebugInfo(bool flag)
	{
		AchievementManager achievementManager = GetAchievementManager();
		if (achievementManager != null)
		{
			achievementManager.m_debugInfo = flag;
		}
	}

	public static void RequestDebugAllOpen(bool flag)
	{
		AchievementManager achievementManager = GetAchievementManager();
		if (achievementManager != null)
		{
			achievementManager.m_debugAllOpen = flag;
		}
	}
}
