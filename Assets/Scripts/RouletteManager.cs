using DataTable;
using Message;
using System;
using System.Collections.Generic;
using Text;
using UnityEngine;

public class RouletteManager : CustomGameObject
{
	public delegate void CallbackRouletteInit(int specialEggNum);

	public const string REQUEST_GET_SUCCEEDED = "RequestGetRoulette_Succeeded";

	public const string REQUEST_GET_FAILED = "RequestGetRoulette_Failed";

	public const string REQUEST_COMMIT_SUCCEEDED = "RequestCommitRoulette_Succeeded";

	public const string REQUEST_COMMIT_FAILED = "RequestCommitRoulette_Failed";

	public const string REQUEST_PRIZE_SUCCEEDED = "RequestRoulettePrize_Succeeded";

	public const string REQUEST_PRIZE_FAILED = "RequestRoulettePrize_Failed";

	public const string REQUEST_BASIC_INFO_SUCCEEDED = "RequestBasicInfo_Succeeded";

	public const string REQUEST_BASIC_INFO_FAILED = "RequestBasicInfo_Failed";

	private const float DUMMY_COMMIT_DELAY = 2f;

	private Dictionary<int, GameObject> m_callbackList;

	private GameObject m_prizeCallback;

	private GameObject m_basicInfoCallback;

	private Dictionary<RouletteCategory, ServerWheelOptionsData> m_rouletteList;

	private Dictionary<RouletteCategory, ServerPrizeState> m_prizeList;

	private ServerWheelOptions m_rouletteItemBak;

	private ServerWheelOptionsGeneral m_rouletteGeneralBak;

	private ServerChaoWheelOptions m_rouletteChaoBak;

	private ServerChaoSpinResult m_resultData;

	private ServerSpinResultGeneral m_resultGeneral;

	private RouletteCategory m_rouletteGeneralBakCategory;

	private RouletteCategory m_rouletteChaoBakCategory;

	private Dictionary<RouletteCategory, float> m_loadingList;

	private RouletteCategory m_isCurrentPrizeLoading;

	private RouletteCategory m_isCurrentPrizeLoadingAuto;

	private RouletteCategory m_lastCommitCategory;

	private RouletteTop m_rouletteTop;

	private EasySnsFeed m_easySnsFeed;

	private List<RouletteCategory> m_basicRouletteCategorys;

	private DateTime m_basicRouletteCategorysGetLastTime;

	private int m_requestRouletteId;

	private bool m_currentRankup;

	private bool m_networkError;

	private static bool s_multiGetWindow;

	private float m_updateRouletteDelay;

	private static RouletteUtility.AchievementType m_achievementType;

	private static RouletteUtility.NextType m_nextType;

	private static bool s_isShowGetWindow;

	private static int s_numJackpotRing;

	private List<ServerItem.Id> m_rouletteCostItemIdList;

	private float m_rouletteCostItemIdListGetTime = -1f;

	private GameObject m_dummyCallback;

	private ServerWheelOptionsData m_dummyData;

	private float m_dummyTime;

	private Dictionary<RouletteCategory, List<CharaType>> m_picupCharaList;

	private bool m_isPicupCharaListInit;

	private DateTime m_picupCharaListTime;

	private bool m_initReq;

	private CallbackRouletteInit m_initReqCallback;

	private string m_oldBgm;

	private bool m_bgmReset;

	private static RouletteManager s_instance;

	public static int numJackpotRing
	{
		get
		{
			return s_numJackpotRing;
		}
		set
		{
			s_numJackpotRing = value;
		}
	}

	public static bool isShowGetWindow
	{
		get
		{
			return s_isShowGetWindow;
		}
		set
		{
			s_isShowGetWindow = value;
		}
	}

	public static bool isMultiGetWindow
	{
		get
		{
			return s_multiGetWindow;
		}
		set
		{
			s_multiGetWindow = value;
		}
	}

	public bool isCurrentPrizeLoading
	{
		get
		{
			if (m_isCurrentPrizeLoading != 0)
			{
				return true;
			}
			if (m_isCurrentPrizeLoadingAuto != 0)
			{
				return true;
			}
			return false;
		}
	}

	public List<RouletteCategory> rouletteCategorys
	{
		get
		{
			return m_basicRouletteCategorys;
		}
	}

	public int specialEgg
	{
		get
		{
			return (int)GeneralUtil.GetItemCount(ServerItem.Id.SPECIAL_EGG);
		}
		set
		{
			GeneralUtil.SetItemCount(ServerItem.Id.SPECIAL_EGG, value);
		}
	}

	public bool currentRankup
	{
		get
		{
			return m_currentRankup;
		}
	}

	public List<ServerItem.Id> rouletteCostItemIdList
	{
		get
		{
			return m_rouletteCostItemIdList;
		}
	}

	public string oldBgmName
	{
		get
		{
			return m_oldBgm;
		}
	}

	public static RouletteManager Instance
	{
		get
		{
			return s_instance;
		}
	}

	public void InitRouletteRequest(CallbackRouletteInit callback = null)
	{
		ServerInterface loggedInServerInterface = ServerInterface.LoggedInServerInterface;
		if (loggedInServerInterface != null)
		{
			m_initReq = true;
			m_initReqCallback = callback;
			if (!RequestRouletteOrg(RouletteCategory.PREMIUM))
			{
				callback(0);
				m_initReq = false;
				m_initReqCallback = null;
			}
		}
		else if (callback != null)
		{
			callback(0);
		}
	}

	private bool RouletteOpenOrg(RouletteCategory category)
	{
		m_isCurrentPrizeLoadingAuto = RouletteCategory.NONE;
		bool flag = GeneralUtil.IsNetwork();
		if (m_networkError && flag)
		{
			m_networkError = false;
		}
		if (!flag)
		{
			if (RouletteTop.Instance != null)
			{
				RouletteTop.Instance.BtnInit();
			}
			m_networkError = true;
		}
		m_rouletteTop = RouletteTop.RouletteTopPageCreate();
		return m_rouletteTop != null;
	}

	private bool RouletteCloseOrg()
	{
		m_isCurrentPrizeLoadingAuto = RouletteCategory.NONE;
		if (m_rouletteTop != null && IsRouletteEnabled())
		{
			return m_rouletteTop.Close();
		}
		return false;
	}

	private void GetWindowClose(RouletteUtility.AchievementType achievement, RouletteUtility.NextType nextType = RouletteUtility.NextType.NONE)
	{
		if (m_rouletteTop != null)
		{
			if (achievement != RouletteUtility.AchievementType.Multi)
			{
				UpdateChangeBotton();
			}
			m_rouletteTop.CloseGetWindow(achievement, nextType);
		}
	}

	public void UpdateChangeBotton(RouletteCategory target = RouletteCategory.NONE)
	{
		if (m_rouletteTop != null)
		{
			if (target != 0)
			{
				Debug.Log(string.Concat("UpdateChangeBotton target:", target, " !!!!!!!!!!!!!!!!!! "));
				m_rouletteTop.UpdateChangeBotton(target);
			}
			else if (m_lastCommitCategory != 0)
			{
				m_rouletteTop.UpdateChangeBotton(m_lastCommitCategory);
			}
		}
	}

	private bool IsRouletteEnabledOrg()
	{
		bool flag = false;
		if (m_rouletteTop != null)
		{
			flag = m_rouletteTop.gameObject.activeSelf;
			if (flag)
			{
				float panelsAlpha = m_rouletteTop.GetPanelsAlpha();
				if (panelsAlpha == 0f)
				{
					flag = false;
				}
			}
		}
		return flag;
	}

	private bool OpenRouletteWindowOrg()
	{
		bool result = false;
		if (m_rouletteTop != null)
		{
			m_rouletteTop.OpenRouletteWindow();
			result = true;
		}
		return result;
	}

	private bool CloseRouletteWindowOrg()
	{
		bool result = false;
		if (m_rouletteTop != null)
		{
			m_rouletteTop.CloseRouletteWindow();
			result = true;
		}
		return result;
	}

	public static bool RouletteOpen(RouletteCategory category = RouletteCategory.NONE)
	{
		if (category == RouletteCategory.NONE || category == RouletteCategory.ALL)
		{
			category = RouletteUtility.rouletteDefault;
		}
		if (category == RouletteCategory.RAID && EventManager.Instance != null)
		{
			EventManager.EventType typeInTime = EventManager.Instance.TypeInTime;
			if (typeInTime != EventManager.EventType.RAID_BOSS)
			{
				RouletteUtility.rouletteDefault = RouletteCategory.PREMIUM;
				category = RouletteUtility.rouletteDefault;
			}
		}
		if (s_instance != null)
		{
			return s_instance.RouletteOpenOrg(category);
		}
		return false;
	}

	public static bool RouletteClose()
	{
		bool result = false;
		if (s_instance != null)
		{
			s_instance.RouletteCloseOrg();
		}
		return result;
	}

	public static bool IsRouletteClose()
	{
		bool result = false;
		if (s_instance != null && s_instance.m_rouletteTop != null)
		{
			return s_instance.m_rouletteTop.IsClose();
		}
		return result;
	}

	public static void RouletteGetWindowClose(RouletteUtility.AchievementType achievement, RouletteUtility.NextType nextType = RouletteUtility.NextType.NONE)
	{
		if (achievement == RouletteUtility.AchievementType.NONE)
		{
			return;
		}
		if (IsRouletteEnabled())
		{
			if (achievement == RouletteUtility.AchievementType.Multi)
			{
				if (!RouletteUtility.IsGetOtomoOrCharaWindow())
				{
					if (!RouletteUtility.ShowGetAllOtomoAndChara())
					{
						RouletteUtility.ShowGetAllListEnd();
						s_multiGetWindow = false;
						if (s_instance != null)
						{
							s_instance.UpdateChangeBotton();
						}
					}
				}
				else
				{
					s_multiGetWindow = true;
				}
			}
			else
			{
				m_achievementType = achievement;
				m_nextType = nextType;
			}
		}
		else
		{
			m_achievementType = RouletteUtility.AchievementType.NONE;
			m_nextType = RouletteUtility.NextType.NONE;
		}
		AchievementManager.RequestUpdate();
	}

	public static bool IsRouletteEnabled()
	{
		bool result = false;
		if (s_instance != null)
		{
			result = s_instance.IsRouletteEnabledOrg();
		}
		return result;
	}

	public static bool OpenRouletteWindow()
	{
		bool result = false;
		if (s_instance != null)
		{
			result = s_instance.OpenRouletteWindowOrg();
		}
		return result;
	}

	public static bool CloseRouletteWindow()
	{
		bool result = false;
		if (s_instance != null)
		{
			result = s_instance.CloseRouletteWindowOrg();
		}
		return result;
	}

	public void Init()
	{
		AddUpdateCustom(OnUpdateCustom, "Check", 0.1f);
		m_currentRankup = false;
	}

	protected override void UpdateStd(float deltaTime, float timeRate)
	{
		UpdateLoading(deltaTime);
		if (m_achievementType != 0 && AchievementManager.IsRequestEnd())
		{
			if (IsRouletteEnabled())
			{
				GetWindowClose(m_achievementType, m_nextType);
			}
			m_achievementType = RouletteUtility.AchievementType.NONE;
			m_nextType = RouletteUtility.NextType.NONE;
		}
		if (m_dummyTime > 0f)
		{
			m_dummyTime -= Time.deltaTime;
			if (m_dummyTime <= 0f && m_dummyCallback != null && m_dummyData != null)
			{
				m_dummyCallback.SendMessage("RequestCommitRoulette_Succeeded", m_dummyData, SendMessageOptions.DontRequireReceiver);
				m_dummyTime = 0f;
				m_dummyCallback = null;
			}
		}
		if (m_updateRouletteDelay > 0f)
		{
			m_updateRouletteDelay -= Time.deltaTime;
			if (m_updateRouletteDelay <= 0f)
			{
				if (m_rouletteItemBak != null && m_rouletteList != null && m_rouletteList.ContainsKey(RouletteCategory.ITEM) && m_rouletteGeneralBakCategory == RouletteCategory.NONE)
				{
					m_rouletteList[RouletteCategory.ITEM].Setup(m_rouletteItemBak);
					m_rouletteItemBak = null;
					if (m_rouletteTop != null)
					{
						m_rouletteTop.UpdateWheel(m_rouletteList[RouletteCategory.ITEM], false);
					}
				}
				else if (m_rouletteGeneralBak != null && m_rouletteGeneralBakCategory != 0)
				{
					RouletteCategory rouletteGeneralBakCategory = m_rouletteGeneralBakCategory;
					if (m_rouletteList != null && m_rouletteList.ContainsKey(rouletteGeneralBakCategory))
					{
						m_rouletteList[rouletteGeneralBakCategory].Setup(m_rouletteGeneralBak);
						m_rouletteGeneralBak = null;
						m_rouletteGeneralBakCategory = RouletteCategory.NONE;
						if (m_rouletteTop != null)
						{
							m_rouletteTop.UpdateWheel(m_rouletteList[rouletteGeneralBakCategory], false);
						}
					}
				}
				else if (m_rouletteChaoBak != null && m_rouletteChaoBakCategory != 0)
				{
					RouletteCategory rouletteChaoBakCategory = m_rouletteChaoBakCategory;
					if (m_rouletteList != null && m_rouletteList.ContainsKey(rouletteChaoBakCategory))
					{
						m_rouletteList[rouletteChaoBakCategory].Setup(m_rouletteChaoBak);
						m_rouletteChaoBak = null;
						m_rouletteChaoBakCategory = RouletteCategory.NONE;
						if (m_rouletteTop != null)
						{
							m_rouletteTop.UpdateWheel(m_rouletteList[rouletteChaoBakCategory], false);
						}
					}
				}
				m_updateRouletteDelay = 0f;
			}
		}
		if (!(m_rouletteCostItemIdListGetTime < 0f) || !GeneralWindow.IsCreated("ShowNoCommunicationCostItem") || !GeneralWindow.IsOkButtonPressed)
		{
			return;
		}
		if (GeneralUtil.IsNetwork())
		{
			if (m_rouletteCostItemIdList == null || m_rouletteCostItemIdList.Count <= 0)
			{
				return;
			}
			List<int> list = new List<int>();
			foreach (ServerItem.Id rouletteCostItemId in m_rouletteCostItemIdList)
			{
				list.Add((int)rouletteCostItemId);
			}
			ServerInterface.LoggedInServerInterface.RequestServerGetItemStockNum(EventManager.Instance.Id, list, base.gameObject);
		}
		else
		{
			GeneralUtil.ShowNoCommunication("ShowNoCommunicationCostItem");
		}
	}

	private void OnUpdateCustom(string updateName, float timeRate)
	{
		if (isShowGetWindow)
		{
			ItemGetWindow itemGetWindow = ItemGetWindowUtil.GetItemGetWindow();
			if (itemGetWindow != null && itemGetWindow.IsCreated("Jackpot") && itemGetWindow.IsEnd)
			{
				if (itemGetWindow.IsYesButtonPressed)
				{
					m_easySnsFeed = new EasySnsFeed(m_rouletteTop.gameObject, "Camera/menu_Anim/RouletteTopUI/Anchor_5_MC", GetText("feed_jackpot_caption"), RouletteUtility.jackpotFeedText);
					Debug.Log("Jackpot feed text:" + RouletteUtility.jackpotFeedText + " !!!!!");
				}
				else if (IsRouletteEnabled())
				{
					GetWindowClose(RouletteUtility.AchievementType.NONE);
				}
				isShowGetWindow = false;
				itemGetWindow.Reset();
			}
			if (GeneralWindow.IsCreated("RouletteGetAllList"))
			{
				if (GeneralWindow.IsButtonPressed)
				{
					s_multiGetWindow = false;
					isShowGetWindow = false;
					GeneralWindow.Close();
				}
			}
			else if (GeneralWindow.IsCreated("RouletteGetAllListEnd"))
			{
				if (GeneralWindow.IsButtonPressed)
				{
					isShowGetWindow = false;
					GeneralWindow.Close();
				}
			}
			else if (GeneralWindow.IsCreated("RouletteGetAllListEndChara"))
			{
				if (GeneralWindow.IsButtonPressed)
				{
					if (GeneralWindow.IsYesButtonPressed)
					{
						HudMenuUtility.SendMenuButtonClicked(ButtonInfoTable.ButtonType.CHARA_MAIN, true);
					}
					isShowGetWindow = false;
					GeneralWindow.Close();
				}
			}
			else if (GeneralWindow.IsCreated("RouletteGetAllListEndChao") && GeneralWindow.IsButtonPressed)
			{
				if (GeneralWindow.IsYesButtonPressed)
				{
					HudMenuUtility.SendMenuButtonClicked(ButtonInfoTable.ButtonType.CHAO, true);
				}
				isShowGetWindow = false;
				GeneralWindow.Close();
			}
		}
		if (m_easySnsFeed != null)
		{
			EasySnsFeed.Result result = m_easySnsFeed.Update();
			if (result == EasySnsFeed.Result.COMPLETED || result == EasySnsFeed.Result.FAILED)
			{
				if (IsRouletteEnabled())
				{
					GetWindowClose(RouletteUtility.AchievementType.NONE);
				}
				isShowGetWindow = false;
				m_easySnsFeed = null;
			}
		}
		if (s_multiGetWindow && !RouletteUtility.IsGetOtomoOrCharaWindow() && !RouletteUtility.ShowGetAllOtomoAndChara())
		{
			RouletteUtility.ShowGetAllListEnd();
			s_multiGetWindow = false;
		}
		if (m_isCurrentPrizeLoadingAuto == RouletteCategory.NONE)
		{
			return;
		}
		switch (m_isCurrentPrizeLoadingAuto)
		{
		case RouletteCategory.PREMIUM:
			if (ServerInterface.PremiumRoulettePrizeList != null && ServerInterface.PremiumRoulettePrizeList.IsData())
			{
				SetPrizeList(m_isCurrentPrizeLoadingAuto, ServerInterface.PremiumRoulettePrizeList);
				m_isCurrentPrizeLoadingAuto = RouletteCategory.NONE;
			}
			break;
		case RouletteCategory.SPECIAL:
			if (ServerInterface.SpecialRoulettePrizeList != null && ServerInterface.SpecialRoulettePrizeList.IsData())
			{
				SetPrizeList(m_isCurrentPrizeLoadingAuto, ServerInterface.SpecialRoulettePrizeList);
				m_isCurrentPrizeLoadingAuto = RouletteCategory.NONE;
			}
			break;
		case RouletteCategory.RAID:
			if (ServerInterface.RaidRoulettePrizeList != null && ServerInterface.RaidRoulettePrizeList.IsData())
			{
				SetPrizeList(m_isCurrentPrizeLoadingAuto, ServerInterface.RaidRoulettePrizeList);
				m_isCurrentPrizeLoadingAuto = RouletteCategory.NONE;
			}
			break;
		default:
			m_isCurrentPrizeLoadingAuto = RouletteCategory.NONE;
			break;
		}
	}

	public void RouletteBgmResetOrg()
	{
		SoundManager.BgmStop();
		m_oldBgm = null;
		m_bgmReset = false;
		RemoveCallback();
	}

	public void PlayBgmOrg(string soundName, float delay = 0f, string cueSheetName = "BGM", bool bgmReset = false)
	{
		if (string.IsNullOrEmpty(soundName) || !IsRouletteEnabled())
		{
			return;
		}
		string text = soundName + ":" + cueSheetName;
		if (delay <= 0f)
		{
			bool flag = false;
			if (bgmReset)
			{
				flag = true;
			}
			else if (!string.IsNullOrEmpty(m_oldBgm))
			{
				if (m_oldBgm != text)
				{
					flag = true;
				}
				string text2 = m_oldBgm;
				if (text2.IndexOf(":") >= 0)
				{
					string[] array = text2.Split(':');
					if (array != null && array.Length > 1)
					{
						text2 = array[0];
					}
				}
				if (text2 == soundName)
				{
					return;
				}
			}
			m_oldBgm = text;
			if (flag)
			{
				SoundManager.BgmChange(soundName, cueSheetName);
			}
			else
			{
				SoundManager.BgmChange(soundName, cueSheetName);
			}
			return;
		}
		bool flag2 = true;
		if (!bgmReset)
		{
			if (!string.IsNullOrEmpty(m_oldBgm) && m_oldBgm == text)
			{
				flag2 = false;
			}
		}
		else
		{
			m_oldBgm = null;
		}
		if (flag2)
		{
			m_bgmReset = bgmReset;
			RemoveCallbackPartialMatch("bgm_sys_");
			AddCallback(OnCallbackBgm, text, delay);
		}
		else
		{
			m_bgmReset = false;
			m_oldBgm = null;
		}
	}

	public void PlaySeOrg(string soundName, float delay = 0f, string cueSheetName = "SE")
	{
		if (!string.IsNullOrEmpty(soundName) && IsRouletteEnabled())
		{
			if (delay <= 0f)
			{
				SoundManager.SePlay(soundName, cueSheetName);
			}
			else
			{
				AddCallback(OnCallbackSe, soundName + ":" + cueSheetName, delay);
			}
		}
	}

	private void OnCallbackBgm(string callbackName)
	{
		if (string.IsNullOrEmpty(callbackName) || !IsRouletteEnabled())
		{
			return;
		}
		string cueName = callbackName;
		string cueSheetName = "BGM";
		if (callbackName.IndexOf(":") >= 0)
		{
			string[] array = callbackName.Split(':');
			if (array.Length > 1)
			{
				cueSheetName = array[1];
				cueName = array[0];
			}
		}
		bool flag = false;
		if (m_bgmReset)
		{
			flag = true;
		}
		else if (!string.IsNullOrEmpty(m_oldBgm) && m_oldBgm != callbackName)
		{
			flag = true;
		}
		m_oldBgm = callbackName;
		if (flag)
		{
			SoundManager.BgmChange(cueName, cueSheetName);
		}
		else
		{
			SoundManager.BgmChange(cueName, cueSheetName);
		}
	}

	private void OnCallbackSe(string callbackName)
	{
		if (string.IsNullOrEmpty(callbackName) || !IsRouletteEnabled())
		{
			return;
		}
		string cueName = callbackName;
		string cueSheetName = "SE";
		if (callbackName.IndexOf(":") >= 0)
		{
			string[] array = callbackName.Split(':');
			if (array.Length > 1)
			{
				cueSheetName = array[1];
				cueName = array[0];
			}
		}
		SoundManager.SePlay(cueName, cueSheetName);
	}

	public static void RouletteBgmReset()
	{
		if (s_instance != null)
		{
			s_instance.RouletteBgmResetOrg();
		}
	}

	public static void PlayBgm(string soundName, float delay = 0f, string cueSheetName = "BGM", bool bgmReset = false)
	{
		if (s_instance != null)
		{
			s_instance.PlayBgmOrg(soundName, delay, cueSheetName, bgmReset);
		}
	}

	public static void PlaySe(string soundName, float delay = 0f, string cueSheetName = "SE")
	{
		if (s_instance != null)
		{
			s_instance.PlaySeOrg(soundName, delay, cueSheetName);
		}
	}

	public Dictionary<RouletteCategory, float> GetCurrentLoadingOrg()
	{
		if (m_loadingList != null && m_loadingList.Count > 0)
		{
			return m_loadingList;
		}
		return null;
	}

	public bool IsLoadingOrg(RouletteCategory category)
	{
		bool result = false;
		if (category != RouletteCategory.ALL)
		{
			if (m_loadingList != null && m_loadingList.ContainsKey(category))
			{
				result = true;
			}
		}
		else if (m_loadingList != null && m_loadingList.Count > 0)
		{
			result = true;
		}
		return result;
	}

	public bool IsPrizeLoadingOrg(RouletteCategory category)
	{
		if (category == RouletteCategory.ALL)
		{
			if (m_isCurrentPrizeLoading != 0)
			{
				return true;
			}
			return false;
		}
		return m_isCurrentPrizeLoading == category;
	}

	private bool StartLoading(RouletteCategory category)
	{
		bool result = false;
		if (category != 0 && category != RouletteCategory.ALL)
		{
			if (m_loadingList == null)
			{
				m_loadingList = new Dictionary<RouletteCategory, float>();
				m_loadingList.Add(category, 0f);
				result = true;
			}
			else if (m_loadingList.ContainsKey(category))
			{
				if (m_loadingList[category] < 0f)
				{
					m_loadingList[category] = 0f;
					result = true;
				}
				else
				{
					result = false;
				}
			}
			else
			{
				m_loadingList.Add(category, 0f);
				result = true;
			}
		}
		m_currentRankup = false;
		return result;
	}

	private void UpdateLoading(float deltaTime)
	{
		if (m_loadingList == null || m_loadingList.Count <= 0)
		{
			return;
		}
		List<RouletteCategory> list = new List<RouletteCategory>(m_loadingList.Keys);
		RouletteCategory rouletteCategory = RouletteCategory.NONE;
		foreach (RouletteCategory item in list)
		{
			if (m_loadingList[item] < 0f)
			{
				rouletteCategory = item;
				continue;
			}
			Dictionary<RouletteCategory, float> loadingList;
			Dictionary<RouletteCategory, float> dictionary = loadingList = m_loadingList;
			RouletteCategory key;
			RouletteCategory key2 = key = item;
			float num = loadingList[key];
			dictionary[key2] = num + deltaTime;
		}
		if (rouletteCategory != 0)
		{
			m_loadingList.Remove(rouletteCategory);
		}
	}

	private float GetLoadingTime(RouletteCategory category)
	{
		float result = -1f;
		if (category != 0 && category != RouletteCategory.ALL && m_loadingList != null && m_loadingList.Count > 0 && m_loadingList.ContainsKey(category))
		{
			result = m_loadingList[category];
		}
		return result;
	}

	private bool EndLoading(RouletteCategory category)
	{
		bool result = false;
		if (category != 0 && category != RouletteCategory.ALL && m_loadingList != null && m_loadingList.Count > 0 && m_loadingList.ContainsKey(category))
		{
			m_loadingList[category] = -1f;
			result = true;
		}
		return result;
	}

	public bool RequestRouletteOrg(RouletteCategory category, GameObject callbackObject = null)
	{
		bool result = false;
		m_isCurrentPrizeLoadingAuto = RouletteCategory.NONE;
		if (category != 0 && category != RouletteCategory.ALL)
		{
			if (category == RouletteCategory.SPECIAL)
			{
				category = RouletteCategory.PREMIUM;
			}
			ServerInterface loggedInServerInterface = ServerInterface.LoggedInServerInterface;
			if (loggedInServerInterface != null)
			{
				if (StartLoading(category))
				{
					int id = EventManager.Instance.Id;
					switch (category)
					{
					case RouletteCategory.PREMIUM:
						Debug.Log("RequestRouletteOrg : RouletteCategory.PREMIUM");
						SetCallbackObject((int)(1000 + category), callbackObject);
						loggedInServerInterface.RequestServerGetChaoWheelOptions(base.gameObject);
						break;
					case RouletteCategory.ITEM:
						Debug.Log("RequestRouletteOrg : RouletteCategory.ITEM");
						SetCallbackObject((int)(1000 + category), callbackObject);
						loggedInServerInterface.RequestServerGetWheelOptions(base.gameObject);
						break;
					default:
						EndLoading(category);
						StartLoading(RouletteCategory.GENERAL);
						m_requestRouletteId = (int)category;
						m_requestRouletteId = 0;
						Debug.Log("RequestRouletteOrg : RouletteCategory.RAID");
						SetCallbackObject(m_requestRouletteId, callbackObject);
						loggedInServerInterface.RequestServerGetWheelOptionsGeneral(id, 0, base.gameObject);
						break;
					}
					result = true;
				}
				else
				{
					result = false;
				}
			}
		}
		return result;
	}

	private ServerWheelOptionsData UpdateRouletteOrg(RouletteCategory category = RouletteCategory.NONE, float delay = 0f)
	{
		if (delay <= 0f)
		{
			if (category != 0 && category != RouletteCategory.ALL)
			{
				if (m_rouletteItemBak != null && category == RouletteCategory.ITEM && m_rouletteList != null && m_rouletteList.ContainsKey(RouletteCategory.ITEM) && m_rouletteGeneralBakCategory == RouletteCategory.NONE)
				{
					m_rouletteList[RouletteCategory.ITEM].Setup(m_rouletteItemBak);
					m_rouletteItemBak = null;
					return m_rouletteList[RouletteCategory.ITEM];
				}
				if (m_rouletteGeneralBak != null && m_rouletteGeneralBakCategory != 0)
				{
					RouletteCategory rouletteGeneralBakCategory = m_rouletteGeneralBakCategory;
					if (m_rouletteList != null && m_rouletteList.ContainsKey(rouletteGeneralBakCategory))
					{
						m_rouletteList[rouletteGeneralBakCategory].Setup(m_rouletteGeneralBak);
						m_rouletteGeneralBak = null;
						m_rouletteGeneralBakCategory = RouletteCategory.NONE;
						return m_rouletteList[rouletteGeneralBakCategory];
					}
				}
				else if (m_rouletteChaoBak != null && m_rouletteChaoBakCategory != 0)
				{
					RouletteCategory rouletteChaoBakCategory = m_rouletteChaoBakCategory;
					if (m_rouletteList != null && m_rouletteList.ContainsKey(rouletteChaoBakCategory))
					{
						m_rouletteList[rouletteChaoBakCategory].Setup(m_rouletteChaoBak);
						m_rouletteChaoBak = null;
						m_rouletteChaoBakCategory = RouletteCategory.NONE;
						return m_rouletteList[rouletteChaoBakCategory];
					}
				}
			}
		}
		else if (category != 0 && category != RouletteCategory.ALL)
		{
			if (m_rouletteItemBak != null && category == RouletteCategory.ITEM && m_rouletteList != null && m_rouletteList.ContainsKey(RouletteCategory.ITEM))
			{
				m_updateRouletteDelay = delay;
			}
			else if (m_rouletteGeneralBak != null && m_rouletteGeneralBakCategory != 0)
			{
				if (m_rouletteList != null && m_rouletteList.ContainsKey(m_rouletteGeneralBakCategory))
				{
					m_updateRouletteDelay = delay;
				}
			}
			else if (m_rouletteChaoBak != null && m_rouletteChaoBakCategory != 0 && m_rouletteList != null && m_rouletteList.ContainsKey(m_rouletteChaoBakCategory))
			{
				m_updateRouletteDelay = delay;
			}
		}
		return null;
	}

	public bool IsRequestPicupCharaList()
	{
		bool result = false;
		if (m_isPicupCharaListInit && (NetBase.GetCurrentTime() - m_picupCharaListTime).TotalHours <= 1.0)
		{
			result = true;
		}
		return result;
	}

	public bool RequestPicupCharaList(bool isImmediatelyUpdate = false)
	{
		bool result = false;
		bool flag = false;
		if (!m_isPicupCharaListInit)
		{
			flag = true;
		}
		else if (isImmediatelyUpdate)
		{
			if ((NetBase.GetCurrentTime() - m_picupCharaListTime).TotalMinutes > 1.0)
			{
				flag = true;
			}
		}
		else if ((NetBase.GetCurrentTime() - m_picupCharaListTime).TotalHours > 1.0)
		{
			flag = true;
		}
		if (flag)
		{
			m_picupCharaListTime = NetBase.GetCurrentTime();
			ServerInterface loggedInServerInterface = ServerInterface.LoggedInServerInterface;
			if (loggedInServerInterface != null)
			{
				if (GetPrizeList(RouletteCategory.PREMIUM) == null)
				{
					loggedInServerInterface.RequestServerGetPrizeChaoWheelSpin(0, base.gameObject);
				}
				if (GetPrizeList(RouletteCategory.SPECIAL) == null)
				{
					loggedInServerInterface.RequestServerGetPrizeChaoWheelSpin(1, base.gameObject);
				}
				if (EventManager.Instance != null && EventManager.Instance.TypeInTime == EventManager.EventType.RAID_BOSS && GetPrizeList(RouletteCategory.RAID) == null)
				{
					int id = EventManager.Instance.Id;
					int spinType = 0;
					loggedInServerInterface.RequestServerGetPrizeWheelSpinGeneral(id, spinType, base.gameObject);
				}
				result = true;
			}
		}
		m_isPicupCharaListInit = true;
		return result;
	}

	public bool RequestRoulettePrizeOrg(RouletteCategory category, GameObject callbackObject = null)
	{
		bool result = false;
		if (category != 0 && category != RouletteCategory.ALL)
		{
			m_isCurrentPrizeLoading = RouletteCategory.NONE;
			ServerPrizeState prizeList = GetPrizeList(category);
			m_prizeCallback = callbackObject;
			if (category == RouletteCategory.PREMIUM && specialEgg >= 10)
			{
				category = RouletteCategory.SPECIAL;
			}
			if (prizeList == null && m_rouletteList != null && m_rouletteList.ContainsKey(category))
			{
				ServerWheelOptionsData serverWheelOptionsData = m_rouletteList[category];
				if (serverWheelOptionsData != null)
				{
					if (serverWheelOptionsData.IsPrizeDataList())
					{
						if (!IsPrizeLoadingOrg(category))
						{
							ServerInterface loggedInServerInterface = ServerInterface.LoggedInServerInterface;
							if (loggedInServerInterface != null)
							{
								if (serverWheelOptionsData.isGeneral)
								{
									int eventId = 0;
									int spinType = 0;
									if (EventManager.Instance != null)
									{
										eventId = EventManager.Instance.Id;
									}
									loggedInServerInterface.RequestServerGetPrizeWheelSpinGeneral(eventId, spinType, base.gameObject);
								}
								else
								{
									int chaoWheelSpinType = (category == RouletteCategory.SPECIAL) ? 1 : 0;
									loggedInServerInterface.RequestServerGetPrizeChaoWheelSpin(chaoWheelSpinType, base.gameObject);
								}
								m_isCurrentPrizeLoading = category;
							}
						}
					}
					else
					{
						prizeList = CreatePrizeList(category);
						if (m_prizeCallback != null && prizeList != null)
						{
							if (IsRouletteEnabled())
							{
								m_prizeCallback.SendMessage("RequestRoulettePrize_Succeeded", prizeList, SendMessageOptions.DontRequireReceiver);
								m_prizeCallback = null;
								m_isCurrentPrizeLoading = RouletteCategory.NONE;
							}
							else
							{
								Debug.Log("RouletteManager RequestRoulettePrizeOrg RouletteTop:false");
							}
						}
					}
				}
			}
			else if (m_prizeCallback != null)
			{
				if (IsRouletteEnabled())
				{
					if (prizeList != null)
					{
						m_prizeCallback.SendMessage("RequestRoulettePrize_Succeeded", prizeList, SendMessageOptions.DontRequireReceiver);
						m_prizeCallback = null;
						m_isCurrentPrizeLoading = RouletteCategory.NONE;
					}
					else
					{
						ServerInterface loggedInServerInterface2 = ServerInterface.LoggedInServerInterface;
						if (loggedInServerInterface2 != null)
						{
							switch (category)
							{
							case RouletteCategory.ITEM:
								prizeList = CreatePrizeList(category);
								if (prizeList == null)
								{
									m_isCurrentPrizeLoading = category;
									loggedInServerInterface2.RequestServerGetWheelOptions(base.gameObject);
								}
								else
								{
									m_prizeCallback.SendMessage("RequestRoulettePrize_Succeeded", prizeList, SendMessageOptions.DontRequireReceiver);
									m_prizeCallback = null;
									m_isCurrentPrizeLoading = RouletteCategory.NONE;
								}
								break;
							case RouletteCategory.PREMIUM:
							case RouletteCategory.SPECIAL:
							{
								m_isCurrentPrizeLoading = category;
								int chaoWheelSpinType2 = (category == RouletteCategory.SPECIAL) ? 1 : 0;
								loggedInServerInterface2.RequestServerGetPrizeChaoWheelSpin(chaoWheelSpinType2, base.gameObject);
								break;
							}
							case RouletteCategory.RAID:
							{
								int eventId2 = 0;
								int spinType2 = 0;
								if (EventManager.Instance != null)
								{
									eventId2 = EventManager.Instance.Id;
								}
								m_isCurrentPrizeLoading = category;
								loggedInServerInterface2.RequestServerGetPrizeWheelSpinGeneral(eventId2, spinType2, base.gameObject);
								break;
							}
							default:
								m_isCurrentPrizeLoading = RouletteCategory.NONE;
								Debug.Log("RouletteManager RequestRoulettePrizeOrg RouletteTop:false");
								break;
							}
						}
						else
						{
							m_isCurrentPrizeLoading = RouletteCategory.NONE;
							Debug.Log("RouletteManager RequestRoulettePrizeOrg RouletteTop:false");
						}
					}
				}
				else
				{
					m_isCurrentPrizeLoading = RouletteCategory.NONE;
					Debug.Log("RouletteManager RequestRoulettePrizeOrg RouletteTop:false");
				}
			}
		}
		return result;
	}

	public void SetPrizeList(RouletteCategory category, ServerPrizeState prizeState)
	{
		if (category != 0 && category != RouletteCategory.ALL && prizeState != null && prizeState.IsData())
		{
			if (m_prizeList == null)
			{
				m_prizeList = new Dictionary<RouletteCategory, ServerPrizeState>();
				m_prizeList.Add(category, prizeState);
			}
			else if (m_prizeList.ContainsKey(category))
			{
				m_prizeList[category] = prizeState;
			}
			else
			{
				m_prizeList.Add(category, prizeState);
			}
			SetPicupCharaList(category, prizeState);
		}
	}

	private void SetPicupCharaList(RouletteCategory category, ServerPrizeState prizeState)
	{
		if (category == RouletteCategory.NONE || category == RouletteCategory.ALL || prizeState == null)
		{
			return;
		}
		if (m_picupCharaList == null)
		{
			m_picupCharaList = new Dictionary<RouletteCategory, List<CharaType>>();
		}
		if (m_picupCharaList.ContainsKey(category))
		{
			m_picupCharaList[category].Clear();
			if (!prizeState.IsData())
			{
				return;
			}
			foreach (ServerPrizeData prize in prizeState.prizeList)
			{
				if (prize.itemId >= 300000 && prize.itemId < 400000)
				{
					ServerItem serverItem = new ServerItem((ServerItem.Id)prize.itemId);
					if (serverItem.idType == ServerItem.IdType.CHARA && !m_picupCharaList[category].Contains(serverItem.charaType))
					{
						m_picupCharaList[category].Add(serverItem.charaType);
					}
				}
			}
			return;
		}
		List<CharaType> list = new List<CharaType>();
		if (prizeState.IsData())
		{
			foreach (ServerPrizeData prize2 in prizeState.prizeList)
			{
				if (prize2.itemId >= 300000 && prize2.itemId < 400000)
				{
					ServerItem serverItem2 = new ServerItem((ServerItem.Id)prize2.itemId);
					if (serverItem2.idType == ServerItem.IdType.CHARA && !list.Contains(serverItem2.charaType))
					{
						list.Add(serverItem2.charaType);
					}
				}
			}
		}
		m_picupCharaList.Add(category, list);
	}

	public ServerPrizeState GetPrizeList(RouletteCategory category)
	{
		ServerPrizeState result = null;
		if (category != 0 && category != RouletteCategory.ALL)
		{
			if (m_prizeList == null)
			{
				m_prizeList = new Dictionary<RouletteCategory, ServerPrizeState>();
				result = null;
			}
			else if (m_prizeList.ContainsKey(category) && !m_prizeList[category].IsExpired())
			{
				result = m_prizeList[category];
			}
			switch (category)
			{
			case RouletteCategory.PREMIUM:
				if (ServerInterface.PremiumRoulettePrizeList != null && ServerInterface.PremiumRoulettePrizeList.IsData())
				{
					SetPrizeList(category, ServerInterface.PremiumRoulettePrizeList);
					result = ServerInterface.PremiumRoulettePrizeList;
				}
				break;
			case RouletteCategory.SPECIAL:
				if (ServerInterface.SpecialRoulettePrizeList != null && ServerInterface.SpecialRoulettePrizeList.IsData())
				{
					SetPrizeList(category, ServerInterface.SpecialRoulettePrizeList);
					result = ServerInterface.SpecialRoulettePrizeList;
				}
				break;
			case RouletteCategory.RAID:
				if (ServerInterface.RaidRoulettePrizeList != null && ServerInterface.RaidRoulettePrizeList.IsData())
				{
					SetPrizeList(category, ServerInterface.RaidRoulettePrizeList);
					result = ServerInterface.RaidRoulettePrizeList;
				}
				break;
			}
		}
		return result;
	}

	private ServerPrizeState CreatePrizeList(RouletteCategory category)
	{
		ServerPrizeState serverPrizeState = null;
		if (category != 0 && category != RouletteCategory.ALL && m_rouletteList != null && m_rouletteList.ContainsKey(category))
		{
			ServerWheelOptionsData data = m_rouletteList[category];
			serverPrizeState = new ServerPrizeState(data);
			if (m_prizeList == null)
			{
				m_prizeList = new Dictionary<RouletteCategory, ServerPrizeState>();
			}
			if (m_prizeList.ContainsKey(category))
			{
				m_prizeList[category] = serverPrizeState;
			}
			else
			{
				m_prizeList.Add(category, serverPrizeState);
			}
		}
		return serverPrizeState;
	}

	public void DummyCommit(ServerWheelOptionsData data, GameObject callbackObject = null)
	{
		Debug.Log("DummyCommit !!!!!!!!!!!!!!!!!!!!!!!!!");
		if (m_rouletteList == null || !m_rouletteList.ContainsKey(RouletteCategory.PREMIUM))
		{
			return;
		}
		ServerWheelOptionsData serverWheelOptionsData = m_rouletteList[RouletteCategory.PREMIUM];
		if (serverWheelOptionsData == null)
		{
			return;
		}
		ServerSpinResultGeneral serverSpinResultGeneral = null;
		ServerChaoSpinResult serverChaoSpinResult = null;
		if (data.isGeneral)
		{
			serverSpinResultGeneral = new ServerSpinResultGeneral();
			m_dummyData = new ServerWheelOptionsData(data.GetOrgGeneralData());
		}
		else
		{
			serverChaoSpinResult = new ServerChaoSpinResult();
			m_dummyData = new ServerWheelOptionsData(data.GetOrgNormalData());
		}
		int num = 0;
		ServerChaoData serverChaoData = null;
		ChaoData[] dataTable = ChaoTable.GetDataTable();
		if (dataTable != null)
		{
			for (int i = 0; i < dataTable.Length; i++)
			{
				if (dataTable[i].level >= 0)
				{
					serverChaoData = new ServerChaoData();
					serverChaoData.Id = dataTable[i].id + 400000;
					serverChaoData.Level = dataTable[i].level;
					serverChaoData.Rarity = (int)dataTable[i].rarity;
					break;
				}
			}
		}
		List<int> list = new List<int>();
		if (serverChaoData != null)
		{
			for (int j = 0; j < 16; j++)
			{
				int cellEgg = data.GetCellEgg(j);
				if (cellEgg == serverChaoData.Rarity)
				{
					list.Add(j);
				}
			}
			if (list.Count <= 0)
			{
				list.Add(1);
			}
			num = list[UnityEngine.Random.Range(0, list.Count)];
			if (serverChaoSpinResult != null)
			{
				serverChaoSpinResult.AcquiredChaoData = serverChaoData;
				serverChaoSpinResult.IsRequiredChao = true;
				serverChaoSpinResult.ItemWon = num;
			}
			if (serverSpinResultGeneral != null)
			{
				serverSpinResultGeneral.AddChaoState(serverChaoData);
				serverSpinResultGeneral.ItemWon = num;
			}
			m_resultData = serverChaoSpinResult;
			m_resultGeneral = serverSpinResultGeneral;
		}
		m_dummyCallback = callbackObject;
		m_dummyTime = 2f;
	}

	public bool RequestCommitRouletteOrg(ServerWheelOptionsData data, int num, GameObject callbackObject = null)
	{
		if (data == null)
		{
			return false;
		}
		bool result = false;
		RouletteCategory rouletteCategory = data.category;
		if (rouletteCategory != 0 && rouletteCategory != RouletteCategory.ALL)
		{
			if (rouletteCategory == RouletteCategory.SPECIAL)
			{
				rouletteCategory = RouletteCategory.PREMIUM;
			}
			ServerInterface loggedInServerInterface = ServerInterface.LoggedInServerInterface;
			if (loggedInServerInterface != null)
			{
				ResetResult();
				m_lastCommitCategory = RouletteCategory.NONE;
				if (StartLoading(rouletteCategory))
				{
					int id = EventManager.Instance.Id;
					switch (rouletteCategory)
					{
					case RouletteCategory.PREMIUM:
					{
						Debug.Log("RequestCommitRouletteOrg : RouletteCategory.PREMIUM");
						int multi2 = data.multi;
						SetCallbackObject((int)(1000 + rouletteCategory), callbackObject);
						loggedInServerInterface.RequestServerCommitChaoWheelSpin(multi2, base.gameObject);
						break;
					}
					case RouletteCategory.ITEM:
						Debug.Log("RequestCommitRouletteOrg : RouletteCategory.ITEM");
						SetCallbackObject((int)(1000 + rouletteCategory), callbackObject);
						loggedInServerInterface.RequestServerCommitWheelSpin(1, base.gameObject);
						break;
					default:
					{
						m_requestRouletteId = (int)rouletteCategory;
						m_requestRouletteId = 0;
						Debug.Log("RequestCommitRouletteOrg : RouletteCategory.RAID");
						EndLoading(rouletteCategory);
						StartLoading(RouletteCategory.GENERAL);
						int spinCostItemId = data.GetSpinCostItemId();
						int multi = data.multi;
						SetCallbackObject(m_requestRouletteId, callbackObject);
						loggedInServerInterface.RequestServerCommitWheelSpinGeneral(id, m_requestRouletteId, spinCostItemId, multi, base.gameObject);
						break;
					}
					}
					result = true;
				}
				else
				{
					result = false;
				}
			}
		}
		return result;
	}

	public void ResetRouletteOrg(RouletteCategory category)
	{
		if (m_rouletteList == null || m_rouletteList.Count <= 0)
		{
			return;
		}
		switch (category)
		{
		case RouletteCategory.NONE:
			return;
		case RouletteCategory.SPECIAL:
			category = RouletteCategory.PREMIUM;
			break;
		}
		if (category != RouletteCategory.ALL)
		{
			if (m_rouletteList.ContainsKey(category))
			{
				m_rouletteList.Remove(category);
			}
		}
		else
		{
			m_rouletteList.Clear();
		}
	}

	public void RequestRouletteBasicInformation(GameObject callback = null)
	{
		bool flag = false;
		m_basicInfoCallback = callback;
		if (m_basicRouletteCategorys == null || m_basicRouletteCategorys.Count <= 0 || m_basicInfoCallback == null)
		{
			flag = true;
			m_basicRouletteCategorysGetLastTime = NetUtil.GetCurrentTime();
		}
		else if ((NetUtil.GetCurrentTime() - m_basicRouletteCategorysGetLastTime).TotalMinutes > 5.0)
		{
			flag = true;
		}
		if (flag)
		{
			ServerGetRouletteList_Succeeded(null);
			return;
		}
		if (m_basicInfoCallback != null)
		{
			m_basicInfoCallback.SendMessage("RequestBasicInfo_Succeeded", m_basicRouletteCategorys, SendMessageOptions.DontRequireReceiver);
		}
		else
		{
			GeneralUtil.SetRouletteBtnIcon();
		}
		if (m_rouletteCostItemIdList != null && m_rouletteCostItemIdList.Count > 0)
		{
			RouletteTop.Instance.UpdateCostItemList(m_rouletteCostItemIdList);
			m_rouletteCostItemIdListGetTime = Time.realtimeSinceStartup;
		}
	}

	private void ServerGetRouletteList_Succeeded(MsgGetItemStockNumSucceed msg)
	{
		m_basicRouletteCategorysGetLastTime = NetUtil.GetCurrentTime();
		EventManager.EventType typeInTime = EventManager.Instance.TypeInTime;
		if (m_basicRouletteCategorys != null && m_basicRouletteCategorys.Contains(RouletteCategory.RAID) && typeInTime != EventManager.EventType.RAID_BOSS)
		{
			m_rouletteCostItemIdListGetTime = -1f;
		}
		m_basicRouletteCategorys = new List<RouletteCategory>();
		m_rouletteCostItemIdList = new List<ServerItem.Id>();
		m_basicRouletteCategorys.Add(RouletteCategory.PREMIUM);
		m_basicRouletteCategorys.Add(RouletteCategory.ITEM);
		m_rouletteCostItemIdList.Add(ServerItem.Id.ROULLETE_TICKET_PREMIAM);
		m_rouletteCostItemIdList.Add(ServerItem.Id.ROULLETE_TICKET_ITEM);
		m_rouletteCostItemIdList.Add(ServerItem.Id.SPECIAL_EGG);
		if (typeInTime == EventManager.EventType.RAID_BOSS)
		{
			m_basicRouletteCategorys.Add(RouletteCategory.RAID);
			m_rouletteCostItemIdList.Add(ServerItem.Id.RAIDRING);
		}
		if (m_rouletteCostItemIdListGetTime <= 0f || Time.realtimeSinceStartup - m_rouletteCostItemIdListGetTime > 1000f)
		{
			List<int> list = new List<int>();
			foreach (ServerItem.Id rouletteCostItemId in m_rouletteCostItemIdList)
			{
				list.Add((int)rouletteCostItemId);
			}
			if (GeneralUtil.IsNetwork())
			{
				ServerInterface.LoggedInServerInterface.RequestServerGetItemStockNum(EventManager.Instance.Id, list, base.gameObject);
			}
			else
			{
				GeneralUtil.ShowNoCommunication("ShowNoCommunicationCostItem");
			}
		}
		GeneralUtil.SetRouletteBtnIcon();
		if (m_basicInfoCallback != null)
		{
			m_basicInfoCallback.SendMessage("RequestBasicInfo_Succeeded", m_basicRouletteCategorys, SendMessageOptions.DontRequireReceiver);
		}
	}

	private void ServerGetItemStockNum_Succeeded(MsgGetItemStockNumSucceed msg)
	{
		if (RouletteTop.Instance != null)
		{
			RouletteTop.Instance.UpdateCostItemList(m_rouletteCostItemIdList);
			m_rouletteCostItemIdListGetTime = Time.realtimeSinceStartup;
		}
	}

	public static Dictionary<RouletteCategory, float> GetCurrentLoading()
	{
		Dictionary<RouletteCategory, float> result = null;
		if (s_instance != null)
		{
			result = s_instance.GetCurrentLoadingOrg();
		}
		return result;
	}

	public static bool IsLoading(RouletteCategory category)
	{
		bool result = false;
		if (s_instance != null)
		{
			result = s_instance.IsLoadingOrg(category);
		}
		return result;
	}

	public static bool IsPrizeLoading(RouletteCategory category)
	{
		bool result = false;
		if (s_instance != null)
		{
			result = s_instance.IsPrizeLoadingOrg(category);
		}
		return result;
	}

	public static bool RequestRoulettePrize(RouletteCategory category, GameObject callbackObject = null)
	{
		bool result = false;
		if (s_instance != null)
		{
			result = s_instance.RequestRoulettePrizeOrg(category, callbackObject);
		}
		return result;
	}

	public static bool RequestRoulette(RouletteCategory category, GameObject callbackObject = null)
	{
		bool result = false;
		if (s_instance != null)
		{
			result = s_instance.RequestRouletteOrg(category, callbackObject);
		}
		return result;
	}

	public static bool RequestCommitRoulette(ServerWheelOptionsData data, int num, GameObject callbackObject = null)
	{
		bool result = false;
		if (s_instance != null)
		{
			result = s_instance.RequestCommitRouletteOrg(data, num, callbackObject);
		}
		return result;
	}

	public static void ResetRoulette(RouletteCategory category = RouletteCategory.ALL)
	{
		if (s_instance != null)
		{
			s_instance.ResetRouletteOrg(category);
		}
	}

	public static ServerWheelOptionsData UpdateRoulette(RouletteCategory category, float delay = 0f)
	{
		if (s_instance != null)
		{
			return s_instance.UpdateRouletteOrg(category, delay);
		}
		return null;
	}

	public static void Remove()
	{
		if (s_instance != null)
		{
			UnityEngine.Object.Destroy(s_instance.gameObject);
			s_instance = null;
		}
	}

	private void ResetResult()
	{
		m_resultData = null;
		m_resultGeneral = null;
	}

	public bool IsResult()
	{
		bool result = false;
		if (m_resultData != null || m_resultGeneral != null)
		{
			result = true;
		}
		return result;
	}

	public bool IsPicupChara(CharaType charaType)
	{
		bool flag = false;
		if (m_picupCharaList != null && m_picupCharaList.Count > 0)
		{
			Dictionary<RouletteCategory, List<CharaType>>.KeyCollection keys = m_picupCharaList.Keys;
			{
				foreach (RouletteCategory item in keys)
				{
					if (m_picupCharaList[item].Count > 0)
					{
						foreach (CharaType item2 in m_picupCharaList[item])
						{
							if (item2 == charaType)
							{
								flag = true;
								break;
							}
						}
					}
					if (flag)
					{
						return flag;
					}
				}
				return flag;
			}
		}
		return flag;
	}

	public RouletteCategory GetPicupCharaCategry(CharaType charaType)
	{
		RouletteCategory rouletteCategory = RouletteCategory.NONE;
		if (m_picupCharaList != null && m_picupCharaList.Count > 0)
		{
			Dictionary<RouletteCategory, List<CharaType>>.KeyCollection keys = m_picupCharaList.Keys;
			{
				foreach (RouletteCategory item in keys)
				{
					if (m_picupCharaList[item].Count > 0)
					{
						foreach (CharaType item2 in m_picupCharaList[item])
						{
							if (item2 == charaType)
							{
								rouletteCategory = item;
								break;
							}
						}
					}
					if (rouletteCategory != 0)
					{
						return rouletteCategory;
					}
				}
				return rouletteCategory;
			}
		}
		return rouletteCategory;
	}

	public ServerSpinResultGeneral GetResult()
	{
		return m_resultGeneral;
	}

	public ServerChaoSpinResult GetResultChao()
	{
		return m_resultData;
	}

	private void ServerGetWheelOptionsGeneral_Succeeded(MsgGetWheelOptionsGeneralSucceed msg)
	{
		if (msg == null)
		{
			return;
		}
		ServerWheelOptionsGeneral wheelOptionsGeneral = msg.m_wheelOptionsGeneral;
		RouletteCategory rouletteCategory = RouletteUtility.GetRouletteCategory(wheelOptionsGeneral);
		Debug.Log("ServerGetWheelOptionsGeneral_Succeeded Category:" + rouletteCategory);
		if (wheelOptionsGeneral == null)
		{
			return;
		}
		if (wheelOptionsGeneral.jackpotRing >= 30000)
		{
			s_numJackpotRing = wheelOptionsGeneral.jackpotRing;
		}
		if (m_rouletteList == null)
		{
			m_rouletteList = new Dictionary<RouletteCategory, ServerWheelOptionsData>();
			m_rouletteList.Add(rouletteCategory, new ServerWheelOptionsData(wheelOptionsGeneral));
		}
		else if (m_rouletteList.ContainsKey(rouletteCategory))
		{
			m_rouletteList[rouletteCategory].Setup(wheelOptionsGeneral);
		}
		else
		{
			m_rouletteList.Add(rouletteCategory, new ServerWheelOptionsData(wheelOptionsGeneral));
		}
		EndLoading(RouletteCategory.GENERAL);
		Debug.Log("rouletteId:" + wheelOptionsGeneral.rouletteId);
		RequestPrizeAuto(m_rouletteList[rouletteCategory]);
		if (GetCallbackObject(wheelOptionsGeneral.rouletteId) != null && m_rouletteList.ContainsKey(rouletteCategory))
		{
			if (IsRouletteEnabled())
			{
				GetCallbackObject(wheelOptionsGeneral.rouletteId).SendMessage("RequestGetRoulette_Succeeded", m_rouletteList[rouletteCategory], SendMessageOptions.DontRequireReceiver);
				UpdateChangeBotton(rouletteCategory);
			}
			else
			{
				Debug.Log("RouletteManager ServerGetWheelOptionsGeneral_Succeeded RouletteTop:false");
			}
		}
	}

	private void ServerGetChaoWheelOptions_Succeeded(MsgGetChaoWheelOptionsSucceed msg)
	{
		if (msg == null)
		{
			return;
		}
		ServerChaoWheelOptions options = msg.m_options;
		RouletteCategory rouletteCategory = RouletteCategory.PREMIUM;
		if (options.SpinType == ServerChaoWheelOptions.ChaoSpinType.Special)
		{
			rouletteCategory = RouletteCategory.SPECIAL;
		}
		if (m_rouletteList == null)
		{
			m_rouletteList = new Dictionary<RouletteCategory, ServerWheelOptionsData>();
			m_rouletteList.Add(rouletteCategory, new ServerWheelOptionsData(options));
		}
		else if (m_rouletteList.ContainsKey(rouletteCategory))
		{
			m_rouletteList[rouletteCategory].Setup(options);
		}
		else
		{
			m_rouletteList.Add(rouletteCategory, new ServerWheelOptionsData(options));
		}
		RequestPrizeAuto(m_rouletteList[rouletteCategory]);
		EndLoading(RouletteCategory.PREMIUM);
		if (!m_initReq)
		{
			if (GetCallbackObject(1001) != null && m_rouletteList.ContainsKey(rouletteCategory))
			{
				if (IsRouletteEnabled())
				{
					GetCallbackObject(1001).SendMessage("RequestGetRoulette_Succeeded", m_rouletteList[rouletteCategory], SendMessageOptions.DontRequireReceiver);
					UpdateChangeBotton(rouletteCategory);
				}
				else
				{
					Debug.Log("RouletteManager ServerGetChaoWheelOptions_Succeeded RouletteTop:false");
				}
			}
		}
		else if (m_initReqCallback != null)
		{
			m_initReqCallback(specialEgg);
			m_initReqCallback = null;
			m_initReq = false;
		}
	}

	private void ServerGetWheelOptions_Succeeded(MsgGetWheelOptionsSucceed msg)
	{
		if (msg == null)
		{
			return;
		}
		ServerWheelOptions wheelOptions = msg.m_wheelOptions;
		if (wheelOptions == null)
		{
			return;
		}
		if (wheelOptions.m_numJackpotRing >= 30000)
		{
			s_numJackpotRing = wheelOptions.m_numJackpotRing;
		}
		if (m_rouletteList == null)
		{
			m_rouletteList = new Dictionary<RouletteCategory, ServerWheelOptionsData>();
			m_rouletteList.Add(RouletteCategory.ITEM, new ServerWheelOptionsData(wheelOptions));
		}
		else if (m_rouletteList.ContainsKey(RouletteCategory.ITEM))
		{
			m_rouletteList[RouletteCategory.ITEM].Setup(wheelOptions);
		}
		else
		{
			m_rouletteList.Add(RouletteCategory.ITEM, new ServerWheelOptionsData(wheelOptions));
		}
		EndLoading(RouletteCategory.ITEM);
		if (m_isCurrentPrizeLoading == RouletteCategory.ITEM)
		{
			ServerPrizeState value = CreatePrizeList(RouletteCategory.ITEM);
			if (m_prizeCallback != null)
			{
				m_prizeCallback.SendMessage("RequestRoulettePrize_Succeeded", value, SendMessageOptions.DontRequireReceiver);
			}
			m_prizeCallback = null;
			m_isCurrentPrizeLoading = RouletteCategory.NONE;
			return;
		}
		RequestPrizeAuto(m_rouletteList[RouletteCategory.ITEM]);
		if (GetCallbackObject(1002) != null && m_rouletteList.ContainsKey(RouletteCategory.ITEM))
		{
			if (IsRouletteEnabled() || RouletteUtility.rouletteDefault == RouletteCategory.ITEM)
			{
				GetCallbackObject(1002).SendMessage("RequestGetRoulette_Succeeded", m_rouletteList[RouletteCategory.ITEM], SendMessageOptions.DontRequireReceiver);
				UpdateChangeBotton(RouletteCategory.ITEM);
			}
			else
			{
				Debug.Log("RouletteManager ServerGetWheelOptions_Succeeded RouletteTop:false");
			}
		}
	}

	private void ServerCommitWheelSpinGeneral_Succeeded(MsgCommitWheelSpinGeneralSucceed msg)
	{
		Debug.Log("RouletteManager ServerCommitWheelSpinGeneral_Succeeded !!!");
		if (msg == null)
		{
			return;
		}
		ServerWheelOptionsData.SPIN_BUTTON sPIN_BUTTON = ServerWheelOptionsData.SPIN_BUTTON.FREE;
		RouletteCategory rouletteCategory = RouletteCategory.NONE;
		ServerSpinResultGeneral resultSpinResultGeneral = msg.m_resultSpinResultGeneral;
		m_currentRankup = false;
		int num = 0;
		ServerWheelOptionsData serverWheelOptionsData = new ServerWheelOptionsData(msg.m_wheelOptionsGeneral);
		if (serverWheelOptionsData != null)
		{
			rouletteCategory = serverWheelOptionsData.category;
		}
		if (m_rouletteList != null && m_rouletteList.ContainsKey(rouletteCategory))
		{
			sPIN_BUTTON = m_rouletteList[rouletteCategory].GetSpinButtonSeting();
		}
		if (rouletteCategory == RouletteCategory.NONE)
		{
			return;
		}
		ServerWheelOptionsData serverWheelOptionsData2 = null;
		if (m_rouletteList != null && m_rouletteList.ContainsKey(rouletteCategory))
		{
			serverWheelOptionsData2 = m_rouletteList[rouletteCategory];
		}
		num = resultSpinResultGeneral.ItemWon;
		if (serverWheelOptionsData2 != null)
		{
			if (resultSpinResultGeneral.ItemWon >= 0 && serverWheelOptionsData2.GetRouletteRank() != RouletteUtility.WheelRank.Super)
			{
				int num2 = 0;
				if (serverWheelOptionsData2.GetCellItem(resultSpinResultGeneral.ItemWon, out num2).idType == ServerItem.IdType.ITEM_ROULLETE_WIN)
				{
					m_currentRankup = true;
				}
				else
				{
					m_currentRankup = false;
				}
			}
			else
			{
				m_currentRankup = false;
			}
		}
		else
		{
			if (m_rouletteList != null)
			{
				m_rouletteList.Add(rouletteCategory, serverWheelOptionsData);
			}
			Debug.Log("RouletteManager ServerCommitWheelSpinGeneral_Succeeded error?");
		}
		m_rouletteItemBak = null;
		m_rouletteChaoBak = null;
		m_rouletteGeneralBakCategory = RouletteCategory.NONE;
		m_rouletteGeneralBak = msg.m_wheelOptionsGeneral;
		if (m_rouletteGeneralBak != null)
		{
			m_rouletteGeneralBakCategory = rouletteCategory;
		}
		EndLoading(RouletteCategory.GENERAL);
		if (rouletteCategory == RouletteCategory.PREMIUM)
		{
		}
		if (GetCallbackObject(serverWheelOptionsData.rouletteId) != null)
		{
			if (IsRouletteEnabled())
			{
				m_resultGeneral = resultSpinResultGeneral;
				m_resultData = null;
				m_resultGeneral.ItemWon = num;
				m_lastCommitCategory = rouletteCategory;
				GetCallbackObject(serverWheelOptionsData.rouletteId).SendMessage("RequestCommitRoulette_Succeeded", serverWheelOptionsData, SendMessageOptions.DontRequireReceiver);
			}
			else
			{
				Debug.Log("RouletteManager ServerCommitWheelSpinGeneral_Succeeded RouletteTop:false");
			}
		}
	}

	private void ServerCommitWheelSpinGeneral_Failed(MsgServerConnctFailed msg)
	{
		if (GetCallbackObject(9) != null)
		{
			if (IsRouletteEnabled())
			{
				GetCallbackObject(9).SendMessage("RequestCommitRoulette_Failed", SendMessageOptions.DontRequireReceiver);
			}
			else
			{
				Debug.Log("RouletteManager ServerCommitWheelSpinGeneral_Failed RouletteTop:false");
			}
		}
	}

	private void ServerCommitChaoWheelSpin_Succeeded(MsgCommitChaoWheelSpicSucceed msg)
	{
		if (msg == null)
		{
			return;
		}
		ServerSpinResultGeneral resultSpinResultGeneral = msg.m_resultSpinResultGeneral;
		m_currentRankup = false;
		bool flag = false;
		ServerChaoWheelOptions.ChaoSpinType spinType = msg.m_chaoWheelOptions.SpinType;
		RouletteCategory rouletteCategory = RouletteCategory.PREMIUM;
		ServerWheelOptionsData serverWheelOptionsData = null;
		ServerWheelOptionsData.SPIN_BUTTON sPIN_BUTTON = ServerWheelOptionsData.SPIN_BUTTON.FREE;
		if (m_rouletteList != null && m_rouletteList.ContainsKey(RouletteCategory.PREMIUM))
		{
			sPIN_BUTTON = m_rouletteList[RouletteCategory.PREMIUM].GetSpinButtonSeting();
		}
		if (spinType == ServerChaoWheelOptions.ChaoSpinType.Special)
		{
			if (m_rouletteList != null && m_rouletteList.ContainsKey(RouletteCategory.SPECIAL))
			{
				serverWheelOptionsData = m_rouletteList[RouletteCategory.SPECIAL];
			}
			rouletteCategory = RouletteCategory.SPECIAL;
		}
		else
		{
			if (m_rouletteList != null && m_rouletteList.ContainsKey(RouletteCategory.PREMIUM))
			{
				serverWheelOptionsData = m_rouletteList[RouletteCategory.PREMIUM];
				if (serverWheelOptionsData.category == RouletteCategory.PREMIUM)
				{
					flag = true;
				}
			}
			rouletteCategory = RouletteCategory.PREMIUM;
		}
		if (m_rouletteList == null || !m_rouletteList.ContainsKey(rouletteCategory))
		{
			serverWheelOptionsData = new ServerWheelOptionsData(msg.m_chaoWheelOptions);
			if (m_rouletteList != null)
			{
				m_rouletteList.Add(rouletteCategory, serverWheelOptionsData);
			}
		}
		m_currentRankup = false;
		m_rouletteItemBak = null;
		m_rouletteChaoBak = msg.m_chaoWheelOptions;
		m_rouletteGeneralBakCategory = RouletteCategory.NONE;
		m_rouletteChaoBakCategory = RouletteCategory.NONE;
		m_rouletteGeneralBak = null;
		if (m_rouletteChaoBak != null)
		{
			m_rouletteChaoBakCategory = rouletteCategory;
		}
		EndLoading(RouletteCategory.PREMIUM);
		if (flag)
		{
		}
		if (!(GetCallbackObject(1001) != null))
		{
			return;
		}
		if (IsRouletteEnabled())
		{
			if (resultSpinResultGeneral != null)
			{
				m_resultGeneral = resultSpinResultGeneral;
				m_resultData = null;
			}
			else
			{
				m_resultGeneral = null;
			}
			m_lastCommitCategory = rouletteCategory;
			GetCallbackObject(1001).SendMessage("RequestCommitRoulette_Succeeded", m_rouletteList[rouletteCategory], SendMessageOptions.DontRequireReceiver);
		}
		else
		{
			Debug.Log("RouletteManager ServerCommitChaoWheelSpin_Succeeded RouletteTop:false");
		}
	}

	private void ServerCommitChaoWheelSpin_Failed(MsgServerConnctFailed msg)
	{
		if (GetCallbackObject(1001) != null)
		{
			if (IsRouletteEnabled())
			{
				GetCallbackObject(1001).SendMessage("RequestCommitRoulette_Failed", SendMessageOptions.DontRequireReceiver);
			}
			else
			{
				Debug.Log("RouletteManager ServerCommitChaoWheelSpin_Failed RouletteTop:false");
			}
		}
	}

	private void ServerCommitWheelSpin_Succeeded(MsgCommitWheelSpinSucceed msg)
	{
		if (msg == null)
		{
			return;
		}
		ServerWheelOptionsData serverWheelOptionsData = null;
		if (m_rouletteList != null && m_rouletteList.ContainsKey(RouletteCategory.ITEM))
		{
			serverWheelOptionsData = m_rouletteList[RouletteCategory.ITEM];
		}
		if (serverWheelOptionsData != null)
		{
			if (serverWheelOptionsData.itemWonData.idType == ServerItem.IdType.ITEM_ROULLETE_WIN && serverWheelOptionsData.GetRouletteRank() != RouletteUtility.WheelRank.Super)
			{
				m_currentRankup = true;
			}
			else
			{
				m_currentRankup = false;
			}
		}
		else
		{
			if (m_rouletteList != null)
			{
				serverWheelOptionsData = new ServerWheelOptionsData(msg.m_wheelOptions);
				m_rouletteList.Add(RouletteCategory.ITEM, serverWheelOptionsData);
			}
			Debug.Log("RouletteManager ServerCommitWheelSpin_Succeeded error?");
		}
		m_rouletteItemBak = msg.m_wheelOptions;
		EndLoading(RouletteCategory.ITEM);
		if (!(GetCallbackObject(1002) != null))
		{
			return;
		}
		if (IsRouletteEnabled() && m_rouletteList != null && m_rouletteList.ContainsKey(RouletteCategory.ITEM) && m_rouletteItemBak != null)
		{
			serverWheelOptionsData = new ServerWheelOptionsData(m_rouletteItemBak);
			if (msg.m_resultSpinResultGeneral != null)
			{
				m_resultData = null;
				m_resultGeneral = msg.m_resultSpinResultGeneral;
			}
			else
			{
				m_resultData = null;
				m_resultGeneral = new ServerSpinResultGeneral(m_rouletteItemBak, m_rouletteList[RouletteCategory.ITEM].GetOrgRankupData());
			}
			m_lastCommitCategory = RouletteCategory.ITEM;
			GetCallbackObject(1002).SendMessage("RequestCommitRoulette_Succeeded", serverWheelOptionsData, SendMessageOptions.DontRequireReceiver);
		}
		else
		{
			Debug.Log("RouletteManager ServerCommitWheelSpin_Succeeded RouletteTop:false");
		}
	}

	private void ServerCommitWheelSpin_Failed(MsgServerConnctFailed msg)
	{
		if (GetCallbackObject(1002) != null)
		{
			if (IsRouletteEnabled())
			{
				GetCallbackObject(1002).SendMessage("RequestCommitRoulette_Failed", SendMessageOptions.DontRequireReceiver);
			}
			else
			{
				Debug.Log("RouletteManager ServerCommitWheelSpin_Failed RouletteTop:false");
			}
		}
	}

	private void ServerGetPrizeChaoWheelSpin_Succeeded(MsgGetPrizeChaoWheelSpinSucceed msg)
	{
		RouletteCategory rouletteCategory = (msg.m_type == 0) ? RouletteCategory.PREMIUM : RouletteCategory.SPECIAL;
		if (m_prizeList == null)
		{
			m_prizeList = new Dictionary<RouletteCategory, ServerPrizeState>();
		}
		SetPrizeList(rouletteCategory, msg.m_prizeState);
		if (rouletteCategory != 0 && m_prizeCallback != null)
		{
			m_isCurrentPrizeLoading = RouletteCategory.NONE;
			if (IsRouletteEnabled())
			{
				m_prizeCallback.SendMessage("RequestRoulettePrize_Succeeded", m_prizeList[rouletteCategory], SendMessageOptions.DontRequireReceiver);
			}
			else
			{
				Debug.Log("RouletteManager ServerGetPrizeChaoWheelSpin_Succeeded RouletteTop:false");
			}
		}
		m_isCurrentPrizeLoading = RouletteCategory.NONE;
		m_prizeCallback = null;
	}

	private void ServerGetPrizeWheelSpinGeneral_Succeeded(MsgGetPrizeWheelSpinGeneralSucceed msg)
	{
		RouletteCategory rouletteCategory = RouletteCategory.RAID;
		if (m_prizeList == null)
		{
			m_prizeList = new Dictionary<RouletteCategory, ServerPrizeState>();
		}
		SetPrizeList(rouletteCategory, msg.prizeState);
		if (rouletteCategory != 0 && m_prizeCallback != null)
		{
			m_isCurrentPrizeLoading = RouletteCategory.NONE;
			if (IsRouletteEnabled())
			{
				m_prizeCallback.SendMessage("RequestRoulettePrize_Succeeded", m_prizeList[rouletteCategory], SendMessageOptions.DontRequireReceiver);
			}
			else
			{
				Debug.Log("RouletteManager ServerGetPrizeWheelSpinGeneral_Succeeded RouletteTop:false");
			}
		}
		m_isCurrentPrizeLoading = RouletteCategory.NONE;
		m_prizeCallback = null;
	}

	private void ServerGetPrizeChaoWheelSpin_Failed()
	{
		if (m_isCurrentPrizeLoading != 0 && m_prizeCallback != null)
		{
			m_isCurrentPrizeLoading = RouletteCategory.NONE;
			if (IsRouletteEnabled())
			{
				m_prizeCallback.SendMessage("RequestRoulettePrize_Failed", SendMessageOptions.DontRequireReceiver);
			}
			else
			{
				Debug.Log("RouletteManager ServerGetPrizeChaoWheelSpin_Failed RouletteTop:false");
			}
		}
		m_isCurrentPrizeLoading = RouletteCategory.NONE;
		m_prizeCallback = null;
	}

	private void ServerGetPrizeWheelSpinGeneral_Failed()
	{
		if (m_isCurrentPrizeLoading != 0 && m_prizeCallback != null)
		{
			m_isCurrentPrizeLoading = RouletteCategory.NONE;
			if (IsRouletteEnabled())
			{
				m_prizeCallback.SendMessage("RequestRoulettePrize_Failed", SendMessageOptions.DontRequireReceiver);
			}
			else
			{
				Debug.Log("RouletteManager ServerGetPrizeWheelSpinGeneral_Failed RouletteTop:false");
			}
		}
		m_isCurrentPrizeLoading = RouletteCategory.NONE;
		m_prizeCallback = null;
	}

	private void ServerAddSpecialEgg_Succeeded(MsgAddSpecialEggSucceed msg)
	{
	}

	private void ServerAddSpecialEgg_Failed(MsgServerConnctFailed msg)
	{
	}

	private void ServerRetrievePlayerState_Succeeded(MsgGetPlayerStateSucceed msg)
	{
		specialEgg = msg.m_playerState.GetNumItemById(220000);
	}

	public ServerWheelOptionsData GetRouletteDataOrg(RouletteCategory category)
	{
		ServerWheelOptionsData serverWheelOptionsData = null;
		if (category != 0 && category != RouletteCategory.ALL && m_rouletteList != null && m_rouletteList.ContainsKey(category))
		{
			ServerWheelOptionsData serverWheelOptionsData2 = m_rouletteList[category];
			if (serverWheelOptionsData2.isValid)
			{
				serverWheelOptionsData = serverWheelOptionsData2;
				if (category == RouletteCategory.PREMIUM && specialEgg >= 10 && !RouletteUtility.isTutorial && serverWheelOptionsData != null && serverWheelOptionsData.category == RouletteCategory.PREMIUM)
				{
					serverWheelOptionsData = null;
				}
			}
		}
		return serverWheelOptionsData;
	}

	public Dictionary<RouletteCategory, ServerWheelOptionsData> GetRouletteDataAllOrg()
	{
		Dictionary<RouletteCategory, ServerWheelOptionsData> dictionary = null;
		if (m_rouletteList != null && m_rouletteList.Count > 0)
		{
			Dictionary<RouletteCategory, ServerWheelOptionsData>.KeyCollection keys = m_rouletteList.Keys;
			{
				foreach (RouletteCategory item in keys)
				{
					if (m_rouletteList[item].isValid)
					{
						if (dictionary == null)
						{
							dictionary = new Dictionary<RouletteCategory, ServerWheelOptionsData>();
						}
						dictionary.Add(item, m_rouletteList[item]);
					}
				}
				return dictionary;
			}
		}
		return dictionary;
	}

	public static ServerWheelOptionsData GetRouletteData(RouletteCategory category)
	{
		ServerWheelOptionsData result = null;
		if (s_instance != null)
		{
			result = s_instance.GetRouletteDataOrg(category);
		}
		return result;
	}

	public static Dictionary<RouletteCategory, ServerWheelOptionsData> GetRouletteDataAll()
	{
		Dictionary<RouletteCategory, ServerWheelOptionsData> result = null;
		if (s_instance != null)
		{
			result = s_instance.GetRouletteDataAllOrg();
		}
		return result;
	}

	public GameObject GetCallbackObject(int key)
	{
		GameObject result = null;
		if (m_callbackList != null)
		{
			if (m_callbackList.ContainsKey(key))
			{
				result = m_callbackList[key];
			}
			else if (key == 8 && m_callbackList.ContainsKey(1))
			{
				result = m_callbackList[1];
			}
		}
		return result;
	}

	public void SetCallbackObject(int key, GameObject obj)
	{
		if (m_callbackList == null)
		{
			m_callbackList = new Dictionary<int, GameObject>();
			m_callbackList.Add(key, obj);
		}
		else if (m_callbackList.ContainsKey(key))
		{
			m_callbackList[key] = obj;
		}
		else
		{
			m_callbackList.Add(key, obj);
		}
	}

	private void RequestPrizeAuto(ServerWheelOptionsData data)
	{
		m_isCurrentPrizeLoadingAuto = RouletteCategory.NONE;
		ServerInterface loggedInServerInterface = ServerInterface.LoggedInServerInterface;
		if (data == null || !(loggedInServerInterface != null))
		{
			return;
		}
		RouletteCategory category = data.category;
		if (m_prizeList != null && m_prizeList.ContainsKey(category) && m_prizeList[category] != null && m_prizeList[category].IsData())
		{
			return;
		}
		if (category != 0 && category != RouletteCategory.ALL && category != RouletteCategory.ITEM)
		{
			if (data.isGeneral)
			{
				int eventId = 0;
				int spinType = 0;
				if (EventManager.Instance != null)
				{
					eventId = EventManager.Instance.Id;
				}
				loggedInServerInterface.RequestServerGetPrizeWheelSpinGeneral(eventId, spinType, null);
			}
			else
			{
				int chaoWheelSpinType = (category == RouletteCategory.SPECIAL) ? 1 : 0;
				loggedInServerInterface.RequestServerGetPrizeChaoWheelSpin(chaoWheelSpinType, null);
			}
			m_isCurrentPrizeLoadingAuto = category;
			Debug.Log(string.Concat("RequestPrizeAuto category:", data.category, " isReq:true"));
		}
		else
		{
			Debug.Log(string.Concat("RequestPrizeAuto category:", data.category, " isReq:false"));
		}
	}

	private void Awake()
	{
		SetInstance();
	}

	private void OnDestroy()
	{
		if (s_instance == this)
		{
			s_instance = null;
		}
	}

	private void SetInstance()
	{
		if (s_instance == null)
		{
			UnityEngine.Object.DontDestroyOnLoad(base.gameObject);
			s_instance = this;
			s_instance.Init();
		}
		else
		{
			UnityEngine.Object.Destroy(base.gameObject);
		}
	}

	private static string GetText(string cellName, Dictionary<string, string> dicReplaces = null)
	{
		string text = TextManager.GetText(TextManager.TextType.TEXTTYPE_COMMON_TEXT, "ItemRoulette", cellName).text;
		if (dicReplaces != null)
		{
			text = TextUtility.Replaces(text, dicReplaces);
		}
		return text;
	}

	private static string GetText(string cellName, string srcText, string dstText)
	{
		return GetText(cellName, new Dictionary<string, string>
		{
			{
				srcText,
				dstText
			}
		});
	}
}
