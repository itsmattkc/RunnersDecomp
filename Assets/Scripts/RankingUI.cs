using System;
using System.Collections.Generic;
using System.Diagnostics;
using UnityEngine;

public class RankingUI : MonoBehaviour
{
	private const float RANKING_INIT_LOAD_DELAY = 0.25f;

	private const RankingUtil.RankingScoreType DEFAULT_SCORE_TYPE = RankingUtil.RankingScoreType.HIGH_SCORE;

	private const RankingUtil.RankingRankerType DEFAULT_RANKER_TYPE = RankingUtil.RankingRankerType.RIVAL;

	public const int BTN_NOT_COUNT = 5;

	[Header("モードごとのカラー設定")]
	[SerializeField]
	private Color m_quickModeColor1;

	[SerializeField]
	private Color m_quickModeColor2;

	[SerializeField]
	private Color m_endlessModeColor1;

	[SerializeField]
	private Color m_endlessModeColor2;

	[SerializeField]
	private List<UISprite> m_colorObjects1;

	[SerializeField]
	private List<UISprite> m_colorObjects2;

	[Header("読み込み中表示")]
	[SerializeField]
	private GameObject m_loading;

	[SerializeField]
	[Header("SNSログインページ")]
	private GameObject m_facebook;

	[Header("ランキング初期ページ(自分と上位3人)")]
	[SerializeField]
	private GameObject m_pattern0;

	[SerializeField]
	private UIRectItemStorage m_pattern0MyDataArea;

	[SerializeField]
	private UIRectItemStorage m_pattern0TopRankerArea;

	[SerializeField]
	private GameObject m_pattern0More;

	[Header("ランキング一覧")]
	[SerializeField]
	private GameObject m_pattern1;

	[SerializeField]
	private UIRectItemStorageRanking m_pattern1ListArea;

	[SerializeField]
	private UIDraggablePanel m_pattern1MainListPanel;

	[SerializeField]
	[Header("ランキング一覧(リーグ)")]
	private GameObject m_pattern2;

	[SerializeField]
	private UIRectItemStorageRanking m_pattern2ListArea;

	[SerializeField]
	private UIDraggablePanel m_pattern2MainListPanel;

	[Header("ボタン類などのオブジェクト")]
	[SerializeField]
	private GameObject m_parts;

	[SerializeField]
	private GameObject m_partsTabNormal;

	[SerializeField]
	private GameObject m_partsTabRival;

	[SerializeField]
	private GameObject m_partsTabFriend;

	[SerializeField]
	private UILabel m_partsInfo;

	[SerializeField]
	private UIImageButton[] m_partsBtns;

	[SerializeField]
	private UISprite m_partsRankIcon0;

	[SerializeField]
	private UISprite m_partsRankIcon1;

	[SerializeField]
	private UILabel m_partsRankText;

	[SerializeField]
	private UILabel m_partsRankTextEx;

	[SerializeField]
	private GameObject m_tallying;

	[SerializeField]
	private UIToggle m_partsTabNormalTogglH;

	[SerializeField]
	private UIToggle m_partsTabNormalTogglT;

	[SerializeField]
	private UIToggle m_partsTabRivalTogglH;

	[SerializeField]
	private UIToggle m_partsTabRivalTogglT;

	[SerializeField]
	private UIToggle m_partsTabFriendTogglH;

	[SerializeField]
	private UIToggle m_partsTabFriendTogglT;

	[SerializeField]
	private List<UIToggle> m_partsBtnToggls;

	[SerializeField]
	[Header("ヘルプウインド")]
	private ranking_help m_help;

	public static SocialInterface s_socialInterface;

	private bool m_isInitilalized;

	private bool m_isHelp;

	private bool m_isDrawInit;

	private bool m_rankingInitDraw;

	private RankingUtil.RankChange m_rankingChange;

	private float m_rankingInitloadingTime;

	private float m_rankingChangeTime;

	private RankingUtil.RankingScoreType m_currentScoreType;

	private RankingUtil.RankingRankerType m_currentRankerType;

	private List<RankingUtil.Ranker> m_currentRankerList;

	private int m_page;

	private bool m_pageNext;

	private bool m_pagePrev;

	private bool m_toggleLock;

	private bool m_facebookLock;

	private bool m_facebookLockInit;

	private bool m_snsCompGetRanking;

	private bool m_snsLogin;

	private bool m_first;

	private float m_snsCompGetRankingTime;

	private RankingCallbackTemporarilySaved m_callbackTemporarilySaved;

	private float m_callbackTemporarilySavedDelay;

	private TimeSpan m_currentResetTimeSpan;

	private float m_resetTimeSpanSec;

	private int m_btnDelay = 5;

	private EasySnsFeed m_easySnsFeed;

	private RankingUtil.RankingMode m_currentMode = RankingUtil.RankingMode.COUNT;

	private RankingUtil.RankingScoreType m_lastSelectedScoreType;

	private bool m_displayFlag;

	private static RankingUI s_instance;

	private static RankingUI Instance
	{
		get
		{
			return s_instance;
		}
	}

	private void SetRankingMode(RankingUtil.RankingMode mode)
	{
		RankerToggleChange(RankingUtil.RankingRankerType.RIVAL);
		Debug.Log(string.Concat("RankingUI SetRankingMode mode:", mode, " old:", m_currentMode));
		if (mode != m_currentMode || m_currentMode == RankingUtil.RankingMode.COUNT)
		{
			m_currentMode = mode;
			m_isDrawInit = false;
			m_isHelp = false;
			if (m_loading != null)
			{
				m_loading.SetActive(true);
			}
			m_callbackTemporarilySavedDelay = 0.25f;
		}
		Init();
		RankingUtil.SetCurrentRankingMode(mode);
	}

	public void SetDisplayEndlessModeOn()
	{
		if (!m_isInitilalized)
		{
			Init();
		}
		SetDisplay(true, RankingUtil.RankingMode.ENDLESS);
	}

	public void SetDisplayEndlessModeOff()
	{
		SetDisplay(false, RankingUtil.RankingMode.ENDLESS);
	}

	public void SetDisplayQuickModeOn()
	{
		if (!m_isInitilalized)
		{
			Init();
		}
		SetDisplay(true, RankingUtil.RankingMode.QUICK);
	}

	public void SetDisplayQuickModeOff()
	{
		SetDisplay(false, RankingUtil.RankingMode.QUICK);
	}

	public void SetDisplay(bool displayFlag, RankingUtil.RankingMode mode)
	{
		m_displayFlag = displayFlag;
		base.gameObject.SetActive(m_displayFlag);
		if (m_displayFlag)
		{
			SetRankingMode(mode);
		}
		if (m_first && displayFlag && m_loading != null)
		{
			m_loading.SetActive(true);
		}
		if (m_parts != null)
		{
			m_parts.SetActive(m_displayFlag);
		}
		UISprite uISprite = GameObjectUtil.FindChildGameObjectComponent<UISprite>(base.gameObject, "img_ranking_word");
		if (uISprite != null)
		{
			if (mode == RankingUtil.RankingMode.QUICK)
			{
				uISprite.spriteName = "ui_mm_mode_word_quick";
			}
			else
			{
				uISprite.spriteName = "ui_mm_mode_word_endless";
			}
		}
		if (m_colorObjects1 == null || m_colorObjects2 == null)
		{
			return;
		}
		Color color;
		Color color2;
		if (mode == RankingUtil.RankingMode.QUICK)
		{
			color = m_quickModeColor1;
			color2 = m_quickModeColor2;
		}
		else
		{
			color = m_endlessModeColor1;
			color2 = m_endlessModeColor2;
		}
		foreach (UISprite item in m_colorObjects1)
		{
			if (item != null)
			{
				item.color = color;
			}
		}
		foreach (UISprite item2 in m_colorObjects2)
		{
			if (item2 != null)
			{
				item2.color = color2;
			}
		}
	}

	public void SetLoadingObject()
	{
		if (m_tallying != null)
		{
			m_tallying.SetActive(false);
		}
	}

	public bool IsInitLoading()
	{
		if (m_loading != null)
		{
			return m_loading.activeSelf;
		}
		return false;
	}

	public RankingUtil.Ranker GetCurrentRanker(int slot)
	{
		RankingUtil.Ranker result = null;
		if (m_currentRankerList != null && slot >= 0 && m_currentRankerList.Count > slot + 1)
		{
			result = m_currentRankerList[slot + 1];
		}
		return result;
	}

	private void Start()
	{
		foreach (UIToggle partsBtnToggl in m_partsBtnToggls)
		{
			if (partsBtnToggl != null)
			{
				if (partsBtnToggl.gameObject.name == "Btn_all")
				{
					EventDelegate.Add(partsBtnToggl.onChange, OnAllToggleChange);
				}
				else if (partsBtnToggl.gameObject.name == "Btn_friend")
				{
					EventDelegate.Add(partsBtnToggl.onChange, OnFriendToggleChange);
				}
				else if (partsBtnToggl.gameObject.name == "Btn_history")
				{
					EventDelegate.Add(partsBtnToggl.onChange, OnHistoryToggleChange);
				}
				else if (partsBtnToggl.gameObject.name == "Btn_rival")
				{
					EventDelegate.Add(partsBtnToggl.onChange, OnRivalToggleChange);
				}
			}
		}
	}

	public static void DebugInitRankingChange()
	{
		if (s_instance != null)
		{
			s_instance.InitRankingChange();
		}
	}

	private void InitRankingChange()
	{
		m_rankingChange = SingletonGameObject<RankingManager>.Instance.GetRankingRankChange(RankingUtil.currentRankingMode);
		if (m_rankingChange != RankingUtil.RankChange.UP)
		{
			m_rankingChange = RankingUtil.RankChange.NONE;
		}
		m_rankingChangeTime = 0f;
	}

	private void Init()
	{
		if (!m_snsLogin)
		{
			m_snsCompGetRanking = false;
			m_snsCompGetRankingTime = 0f;
			m_first = true;
			m_isInitilalized = false;
			m_rankingInitloadingTime = 0f;
		}
	}

	private void InitSetting()
	{
		m_callbackTemporarilySavedDelay = 0.25f;
		m_facebookLockInit = false;
		if (m_loading != null)
		{
			m_loading.SetActive(true);
		}
		if (m_tallying != null)
		{
			m_tallying.SetActive(false);
		}
		if (m_rankingInitDraw)
		{
			m_rankingChange = SingletonGameObject<RankingManager>.Instance.GetRankingRankChange(RankingUtil.currentRankingMode);
			if (m_rankingChange != RankingUtil.RankChange.UP)
			{
				m_rankingChange = RankingUtil.RankChange.NONE;
			}
			m_rankingChangeTime = 0f;
		}
		SetupLeague();
		ServerInterface loggedInServerInterface = ServerInterface.LoggedInServerInterface;
		if (loggedInServerInterface == null)
		{
			ServerInterface.DebugInit();
		}
		s_socialInterface = GameObjectUtil.FindGameObjectComponent<SocialInterface>("SocialInterface");
		Debug.Log("RankingUI  Init !!!!!");
		m_currentRankerType = RankingUtil.RankingRankerType.RIVAL;
		m_currentScoreType = RankingManager.EndlessRivalRankingScoreType;
		RankingManager instance = SingletonGameObject<RankingManager>.Instance;
		if (instance != null && !instance.isLoading && instance.IsRankingTop(RankingUtil.currentRankingMode, RankingManager.EndlessRivalRankingScoreType, RankingUtil.RankingRankerType.RIVAL))
		{
			SetRanking(RankingUtil.RankingRankerType.RIVAL, RankingManager.EndlessRivalRankingScoreType, 0);
		}
		m_snsCompGetRankingTime = 0f;
		m_snsCompGetRanking = false;
		m_isDrawInit = false;
		m_isInitilalized = true;
		m_rankingInitloadingTime = 0f;
		m_btnDelay = 5;
		m_facebookLock = true;
		if (RegionManager.Instance != null)
		{
			m_facebookLock = !RegionManager.Instance.IsUseSNS();
		}
		UIImageButton[] partsBtns = m_partsBtns;
		foreach (UIImageButton uIImageButton in partsBtns)
		{
			if (uIImageButton.name.IndexOf("friend") != -1)
			{
				uIImageButton.isEnabled = !m_facebookLock;
			}
		}
		m_first = false;
	}

	private bool SetRanking(RankingUtil.RankingRankerType type, RankingUtil.RankingScoreType score, int page)
	{
		bool flag = false;
		if (page == -1 || m_currentRankerType != type || m_currentScoreType != score || m_page != page)
		{
			RankingManager instance = SingletonGameObject<RankingManager>.Instance;
			if (instance != null && !instance.isLoading)
			{
				if (page <= 0)
				{
					ResetRankerList(0, type);
					ResetRankerList(1, type);
				}
				m_page = page;
				m_currentRankerType = type;
				m_currentScoreType = score;
				if (m_page < 0)
				{
					m_page = 0;
				}
				if (s_socialInterface != null)
				{
					if (type == RankingUtil.RankingRankerType.FRIEND && !s_socialInterface.IsLoggedIn)
					{
						if (m_facebook != null)
						{
							m_facebook.SetActive(true);
							ResetRankerList(0, type);
							if (m_partsInfo != null)
							{
								m_partsInfo.text = string.Empty;
							}
							return true;
						}
					}
					else if (type == RankingUtil.RankingRankerType.FRIEND && s_socialInterface.IsLoggedIn)
					{
						if (m_facebook != null)
						{
							m_facebook.SetActive(false);
						}
					}
					else if (m_facebook != null)
					{
						m_facebook.SetActive(false);
					}
				}
				else
				{
					if (type == RankingUtil.RankingRankerType.SP_FRIEND)
					{
						m_facebook.SetActive(true);
						ResetRankerList(0, type);
						if (m_partsInfo != null)
						{
							m_partsInfo.text = string.Empty;
						}
						return true;
					}
					if (m_facebook != null)
					{
						m_facebook.SetActive(false);
					}
				}
				if (m_pattern0More != null)
				{
					m_pattern0More.SetActive(false);
				}
				if (m_loading != null)
				{
					m_loading.SetActive(true);
				}
				return instance.GetRanking(RankingUtil.currentRankingMode, score, type, m_page, CallbackRanking);
			}
		}
		return true;
	}

	private void ResetRankerList(int page, RankingUtil.RankingRankerType type)
	{
		if (m_page > 1)
		{
			return;
		}
		if (m_parts != null)
		{
			m_parts.SetActive(true);
		}
		if (page > 0)
		{
			if (type == RankingUtil.RankingRankerType.RIVAL)
			{
				if (m_pattern0 != null)
				{
					m_pattern0.SetActive(false);
				}
				if (m_pattern1 != null)
				{
					m_pattern1.SetActive(false);
				}
				if (m_pattern2 != null)
				{
					m_pattern2.SetActive(true);
				}
			}
			else
			{
				if (m_pattern0 != null)
				{
					m_pattern0.SetActive(false);
				}
				if (m_pattern1 != null)
				{
					m_pattern1.SetActive(true);
				}
				if (m_pattern2 != null)
				{
					m_pattern2.SetActive(false);
				}
			}
		}
		else
		{
			if (m_pattern0 != null)
			{
				m_pattern0.SetActive(true);
			}
			if (m_pattern1 != null)
			{
				m_pattern1.SetActive(false);
			}
			if (m_pattern2 != null)
			{
				m_pattern2.SetActive(false);
			}
		}
		if (m_pattern1ListArea != null)
		{
			m_pattern1ListArea.Reset();
		}
		if (m_pattern2ListArea != null)
		{
			m_pattern2ListArea.Reset();
		}
		if (m_pattern0MyDataArea != null)
		{
			m_pattern0MyDataArea.maxItemCount = (m_pattern0MyDataArea.maxRows = 0);
			m_pattern0MyDataArea.Restart();
		}
		if (m_pattern0TopRankerArea != null)
		{
			m_pattern0TopRankerArea.maxItemCount = (m_pattern0TopRankerArea.maxRows = 0);
			m_pattern0TopRankerArea.Restart();
		}
	}

	private void CallbackRanking(List<RankingUtil.Ranker> rankerList, RankingUtil.RankingScoreType score, RankingUtil.RankingRankerType type, int page, bool isNext, bool isPrev, bool isCashData)
	{
		if (m_callbackTemporarilySavedDelay > 0f)
		{
			m_callbackTemporarilySaved = new RankingCallbackTemporarilySaved(rankerList, score, type, page, isNext, isPrev, isCashData, CallbackRanking);
			return;
		}
		m_callbackTemporarilySavedDelay = 0f;
		Debug.Log(string.Concat("RankingUI:CallbackRanking  type:", type, "  score", score, "  num:", rankerList.Count, " isNext:", isNext, " !!!!"));
		if (m_currentRankerType != type || m_currentScoreType != score)
		{
			return;
		}
		switch (type)
		{
		case RankingUtil.RankingRankerType.RIVAL:
			m_partsTabRival.SetActive(true);
			m_partsTabNormal.SetActive(false);
			m_partsTabFriend.SetActive(false);
			break;
		case RankingUtil.RankingRankerType.FRIEND:
			m_partsTabRival.SetActive(false);
			m_partsTabNormal.SetActive(false);
			m_partsTabFriend.SetActive(true);
			m_snsLogin = false;
			break;
		default:
			m_partsTabRival.SetActive(false);
			m_partsTabNormal.SetActive(true);
			m_partsTabFriend.SetActive(false);
			break;
		}
		m_snsCompGetRankingTime = 0f;
		m_snsCompGetRanking = false;
		m_pageNext = isNext;
		m_pagePrev = isPrev;
		if (m_pattern1ListArea != null)
		{
			m_pattern1ListArea.rankingType = type;
		}
		if (m_pattern2ListArea != null)
		{
			m_pattern2ListArea.rankingType = type;
		}
		SetRankerList(rankerList, type, page);
		if (m_pattern1MainListPanel != null && page <= 1)
		{
			if (type == RankingUtil.RankingRankerType.RIVAL)
			{
				GameObject myDataGameObject = m_pattern2ListArea.GetMyDataGameObject();
				if (myDataGameObject != null)
				{
					Vector3 localPosition = myDataGameObject.transform.localPosition;
					float num = localPosition.y * -1f - 166f;
					if (num < -36f)
					{
						num = -36f;
					}
					Transform transform = m_pattern2MainListPanel.transform;
					Vector3 localPosition2 = m_pattern2MainListPanel.transform.localPosition;
					float x = localPosition2.x;
					float y = num;
					Vector3 localPosition3 = m_pattern2MainListPanel.transform.localPosition;
					transform.localPosition = new Vector3(x, y, localPosition3.z);
					UIPanel panel = m_pattern2MainListPanel.panel;
					Vector4 clipRange = m_pattern2MainListPanel.panel.clipRange;
					float x2 = clipRange.x;
					float y2 = 0f - num;
					Vector4 clipRange2 = m_pattern2MainListPanel.panel.clipRange;
					float z = clipRange2.z;
					Vector4 clipRange3 = m_pattern2MainListPanel.panel.clipRange;
					panel.clipRange = new Vector4(x2, y2, z, clipRange3.w);
					m_pattern2ListArea.CheckItemDrawAll(true);
				}
				else
				{
					m_pattern2MainListPanel.Scroll(0f);
					m_pattern2MainListPanel.ResetPosition();
				}
			}
			else
			{
				m_pattern1MainListPanel.Scroll(0f);
				m_pattern1MainListPanel.ResetPosition();
			}
		}
		if (isNext && m_pattern0More != null)
		{
			m_pattern0More.SetActive(true);
		}
		m_currentResetTimeSpan = SingletonGameObject<RankingManager>.Instance.GetRankigResetTimeSpan(RankingUtil.currentRankingMode, m_currentScoreType, m_currentRankerType);
		m_resetTimeSpanSec = (float)m_currentResetTimeSpan.Seconds + 0.1f;
		if (m_loading != null && m_rankingChange == RankingUtil.RankChange.NONE)
		{
			m_loading.SetActive(false);
		}
		SetTogglBtn();
		if (m_currentResetTimeSpan.Ticks <= 0 && rankerList.Count <= 1)
		{
			if (type != 0 || (type == RankingUtil.RankingRankerType.FRIEND && IsActiveSnsLoginGameObject()))
			{
				if (m_tallying != null)
				{
					m_tallying.SetActive(true);
				}
			}
			else if (m_tallying != null)
			{
				m_tallying.SetActive(false);
			}
		}
		else if (m_tallying != null)
		{
			m_tallying.SetActive(false);
		}
		SetupRankingReset(m_currentResetTimeSpan);
	}

	private void SetTogglBtn()
	{
		m_toggleLock = true;
		if (m_partsBtnToggls != null && m_partsBtnToggls.Count > 0)
		{
			UIToggle uIToggle = null;
			switch (m_currentRankerType)
			{
			case RankingUtil.RankingRankerType.FRIEND:
				foreach (UIToggle partsBtnToggl in m_partsBtnToggls)
				{
					if (partsBtnToggl != null && (partsBtnToggl.gameObject.name.IndexOf("friend") != -1 || partsBtnToggl.gameObject.name.IndexOf("Friend") != -1))
					{
						uIToggle = partsBtnToggl;
						break;
					}
				}
				break;
			case RankingUtil.RankingRankerType.RIVAL:
				foreach (UIToggle partsBtnToggl2 in m_partsBtnToggls)
				{
					if (partsBtnToggl2 != null && (partsBtnToggl2.gameObject.name.IndexOf("rival") != -1 || partsBtnToggl2.gameObject.name.IndexOf("Rival") != -1))
					{
						uIToggle = partsBtnToggl2;
						break;
					}
				}
				break;
			}
			if (uIToggle != null)
			{
				uIToggle.SendMessage("OnClick", SendMessageOptions.DontRequireReceiver);
			}
		}
		if (m_currentRankerType == RankingUtil.RankingRankerType.RIVAL)
		{
			if (m_partsTabRivalTogglH != null && m_partsTabRivalTogglT != null)
			{
				if (m_currentScoreType == RankingUtil.RankingScoreType.HIGH_SCORE)
				{
					m_partsTabRivalTogglH.SendMessage("OnClick", SendMessageOptions.DontRequireReceiver);
				}
				else
				{
					m_partsTabRivalTogglT.SendMessage("OnClick", SendMessageOptions.DontRequireReceiver);
				}
			}
		}
		else if ((m_currentRankerType == RankingUtil.RankingRankerType.FRIEND && IsActiveSnsLoginGameObject()) || m_currentRankerType != 0)
		{
			if (m_partsTabNormalTogglH != null && m_partsTabNormalTogglT != null)
			{
				if (m_currentScoreType == RankingUtil.RankingScoreType.HIGH_SCORE)
				{
					m_partsTabNormalTogglH.SendMessage("OnClick", SendMessageOptions.DontRequireReceiver);
				}
				else
				{
					m_partsTabNormalTogglT.SendMessage("OnClick", SendMessageOptions.DontRequireReceiver);
				}
			}
		}
		else if (m_partsTabFriendTogglH != null && m_partsTabFriendTogglT != null)
		{
			if (m_currentScoreType == RankingUtil.RankingScoreType.HIGH_SCORE)
			{
				m_partsTabFriendTogglH.SendMessage("OnClick", SendMessageOptions.DontRequireReceiver);
			}
			else
			{
				m_partsTabFriendTogglT.SendMessage("OnClick", SendMessageOptions.DontRequireReceiver);
			}
		}
		m_toggleLock = false;
	}

	private void SetRankerList(List<RankingUtil.Ranker> rankers, RankingUtil.RankingRankerType type, int page)
	{
		if (page > 0 || type == RankingUtil.RankingRankerType.RIVAL)
		{
			m_currentRankerList = rankers;
		}
		if (type != 0 && m_facebook != null)
		{
			m_facebook.SetActive(false);
		}
		if (page > 0 || type == RankingUtil.RankingRankerType.RIVAL)
		{
			if (m_pattern0 != null)
			{
				m_pattern0.SetActive(false);
			}
			if (m_pattern1 != null)
			{
				m_pattern1.SetActive(type != RankingUtil.RankingRankerType.RIVAL);
			}
			if (m_pattern2 != null)
			{
				m_pattern2.SetActive(type == RankingUtil.RankingRankerType.RIVAL);
			}
			if (type == RankingUtil.RankingRankerType.RIVAL)
			{
				if (m_pattern2ListArea != null)
				{
					if (page < 1)
					{
						m_pattern2ListArea.Reset();
						AddRectItemStorageRanking(m_pattern2ListArea, rankers, type);
					}
					else
					{
						if (page == 1)
						{
							m_pattern2ListArea.Reset();
						}
						AddRectItemStorageRanking(m_pattern2ListArea, rankers, type);
					}
				}
			}
			else if (m_pattern1ListArea != null)
			{
				if (page == 1)
				{
					m_pattern1ListArea.Reset();
				}
				AddRectItemStorageRanking(m_pattern1ListArea, rankers, type);
			}
			if (m_pattern0MyDataArea != null)
			{
				m_pattern0MyDataArea.maxItemCount = (m_pattern0MyDataArea.maxRows = 0);
				m_pattern0MyDataArea.Restart();
			}
			if (m_pattern0TopRankerArea != null)
			{
				m_pattern0TopRankerArea.maxItemCount = (m_pattern0TopRankerArea.maxRows = 0);
				m_pattern0TopRankerArea.Restart();
			}
			return;
		}
		if (m_pattern0 != null)
		{
			m_pattern0.SetActive(true);
		}
		if (m_pattern1 != null)
		{
			m_pattern1.SetActive(false);
		}
		if (m_pattern2 != null)
		{
			m_pattern2.SetActive(false);
		}
		if (m_pattern1ListArea != null)
		{
			m_pattern1ListArea.Reset();
		}
		if (m_pattern2ListArea != null)
		{
			m_pattern2ListArea.Reset();
		}
		if (m_pattern0MyDataArea != null)
		{
			if (rankers != null && rankers.Count > 0)
			{
				if (rankers[0] != null)
				{
					m_pattern0MyDataArea.maxItemCount = (m_pattern0MyDataArea.maxRows = 1);
					UpdateRectItemStorage(m_pattern0MyDataArea, rankers, 0);
				}
				else
				{
					m_pattern0MyDataArea.maxItemCount = (m_pattern0MyDataArea.maxRows = 0);
					m_pattern0MyDataArea.Restart();
				}
			}
			else
			{
				m_pattern0MyDataArea.maxItemCount = (m_pattern0MyDataArea.maxRows = 0);
				m_pattern0MyDataArea.Restart();
			}
		}
		if (m_pattern0TopRankerArea != null && rankers != null)
		{
			if (rankers.Count - 1 >= RankingManager.GetRankingMax(type, page))
			{
				m_pattern0TopRankerArea.maxItemCount = (m_pattern0TopRankerArea.maxRows = 3);
			}
			else
			{
				m_pattern0TopRankerArea.maxItemCount = (m_pattern0TopRankerArea.maxRows = rankers.Count - 1);
			}
			UpdateRectItemStorage(m_pattern0TopRankerArea, rankers);
		}
	}

	private void AddRectItemStorageRanking(UIRectItemStorageRanking ui_rankers, List<RankingUtil.Ranker> rankerList, RankingUtil.RankingRankerType type)
	{
		int childCount = ui_rankers.childCount;
		int num = rankerList.Count - childCount;
		if (m_pageNext)
		{
			num--;
		}
		if (ui_rankers.callback == null)
		{
			ui_rankers.callback = CallbackItemStorageRanking;
			ui_rankers.callbackTopOrLast = CallbackItemStorageRankingTopOrLast;
		}
		GameObject mainMenuUIObject = HudMenuUtility.GetMainMenuUIObject();
		if (mainMenuUIObject != null && mainMenuUIObject.activeSelf)
		{
			ui_rankers.AddItem(num);
		}
		else
		{
			ui_rankers.AddItem(num, 0f);
		}
	}

	private bool CallbackItemStorageRankingTopOrLast(bool isTop)
	{
		bool result = false;
		RankingManager instance = SingletonGameObject<RankingManager>.Instance;
		if (instance != null && !instance.isLoading)
		{
			if (isTop)
			{
				if (m_pagePrev)
				{
					result = false;
				}
			}
			else if (m_pageNext)
			{
				result = instance.GetRankingScroll(RankingUtil.currentRankingMode, true, CallbackRanking);
			}
		}
		return result;
	}

	private void CallbackItemStorageRanking(ui_ranking_scroll_dummy obj, UIRectItemStorageRanking storage)
	{
		if (!(obj != null) || m_currentRankerList == null)
		{
			return;
		}
		int num = obj.slot + 1;
		if (num > 0 && m_currentRankerList.Count > num)
		{
			RankingUtil.Ranker rankerData = m_currentRankerList[num];
			if (obj.myRankerData == null)
			{
				obj.myRankerData = m_currentRankerList[0];
			}
			obj.spWindow = null;
			obj.rankingUI = this;
			obj.rankerData = rankerData;
			obj.rankerType = m_currentRankerType;
			obj.scoreType = m_currentScoreType;
			obj.SetActiveObject(storage.CheckItemDraw(obj.slot), 0f);
			obj.end = (obj.slot + 1 == m_currentRankerList.Count);
		}
		else
		{
			UnityEngine.Object.Destroy(obj.gameObject);
		}
	}

	private void UpdateRectItemStorage(UIRectItemStorage ui_rankers, List<RankingUtil.Ranker> rankerList, int head = 1)
	{
		ui_rankers.Restart();
		ui_ranking_scroll[] componentsInChildren = ui_rankers.GetComponentsInChildren<ui_ranking_scroll>(true);
		for (int i = 0; i < ui_rankers.maxItemCount && i + head < rankerList.Count; i++)
		{
			RankingUtil.Ranker ranker = rankerList[i + head];
			if (ranker != null)
			{
				componentsInChildren[i].UpdateView(m_currentScoreType, m_currentRankerType, ranker, i == ui_rankers.maxItemCount - 1);
				bool myRanker = false;
				if (rankerList[0] != null && ranker.id == rankerList[0].id)
				{
					myRanker = true;
				}
				componentsInChildren[i].SetMyRanker(myRanker);
			}
		}
	}

	private static string[] GetFriendIdList(RankingUtil.RankingRankerType rankerType)
	{
		string[] result = null;
		if (rankerType == RankingUtil.RankingRankerType.FRIEND)
		{
			result = RankingUtil.GetFriendIdList();
		}
		return result;
	}

	private void Update()
	{
		if (m_isInitilalized)
		{
			if (m_easySnsFeed != null)
			{
				switch (m_easySnsFeed.Update())
				{
				case EasySnsFeed.Result.COMPLETED:
					m_snsCompGetRanking = true;
					m_snsCompGetRankingTime = 0f;
					m_currentRankerList = null;
					ResetRankerList(0, m_currentRankerType);
					Debug.Log("SetRanking m_easySnsFeed");
					SetRanking(RankingUtil.RankingRankerType.FRIEND, m_lastSelectedScoreType, -1);
					m_easySnsFeed = null;
					break;
				case EasySnsFeed.Result.FAILED:
					m_snsLogin = false;
					m_snsCompGetRankingTime = 0f;
					m_snsCompGetRanking = false;
					m_easySnsFeed = null;
					break;
				}
			}
			else if (m_snsCompGetRanking)
			{
				m_snsCompGetRankingTime += Time.deltaTime;
				if (m_snsCompGetRankingTime > 5f)
				{
					Debug.Log("SetRanking m_easySnsFeed reload !");
					SetRanking(RankingUtil.RankingRankerType.FRIEND, m_lastSelectedScoreType, -1);
					m_snsCompGetRankingTime = -5f;
				}
			}
			else
			{
				m_snsCompGetRankingTime = 0f;
			}
		}
		if (m_isInitilalized && !m_isDrawInit)
		{
			Debug.Log("m_isInitilalized");
			RankingManager instance = SingletonGameObject<RankingManager>.Instance;
			if (instance != null && !instance.isLoading && instance.IsRankingTop(RankingUtil.currentRankingMode, RankingManager.EndlessRivalRankingScoreType, RankingUtil.RankingRankerType.RIVAL))
			{
				SetRanking(m_currentRankerType, m_currentScoreType, -1);
			}
			m_isDrawInit = true;
		}
		if (m_resetTimeSpanSec <= 0f)
		{
			m_currentResetTimeSpan = SingletonGameObject<RankingManager>.Instance.GetRankigResetTimeSpan(RankingUtil.currentRankingMode, m_currentScoreType, m_currentRankerType);
			m_resetTimeSpanSec = (float)m_currentResetTimeSpan.Seconds + 0.1f;
			if (m_currentResetTimeSpan.Ticks > 0)
			{
				if (m_currentResetTimeSpan.Days < 1 && m_currentResetTimeSpan.Hours < 1 && m_currentResetTimeSpan.Minutes < 1)
				{
					m_resetTimeSpanSec = (float)m_currentResetTimeSpan.Milliseconds / 1000f + 0.005f;
				}
			}
			else
			{
				m_resetTimeSpanSec = 300f;
			}
			SetupRankingReset(m_currentResetTimeSpan);
		}
		else
		{
			m_resetTimeSpanSec -= Time.deltaTime;
		}
		if (m_btnDelay > 0)
		{
			m_btnDelay--;
		}
		if (m_rankingChange != 0 && m_rankingInitDraw)
		{
			if (IsRankingActive())
			{
				m_rankingChangeTime += Time.deltaTime;
			}
			else
			{
				m_rankingChangeTime = 0f;
			}
			if (m_rankingChangeTime > 0.25f)
			{
				RankingUtil.ShowRankingChangeWindow(RankingUtil.currentRankingMode);
				m_rankingChange = RankingUtil.RankChange.NONE;
				if (m_loading != null)
				{
					m_loading.SetActive(false);
				}
			}
		}
		if (m_isInitilalized && !m_facebookLockInit && !m_facebookLock && IsRankingActive())
		{
			if (RegionManager.Instance != null)
			{
				m_facebookLock = !RegionManager.Instance.IsUseSNS();
			}
			UIImageButton[] partsBtns = m_partsBtns;
			foreach (UIImageButton uIImageButton in partsBtns)
			{
				if (uIImageButton.name.IndexOf("friend") != -1)
				{
					uIImageButton.isEnabled = !m_facebookLock;
					break;
				}
			}
			m_facebookLockInit = true;
		}
		if (!m_rankingInitDraw)
		{
			GameObject x = GameObject.Find("UI Root (2D)");
			if (x != null)
			{
				m_rankingInitDraw = true;
			}
		}
		else if (m_loading != null && m_loading.activeSelf)
		{
			if (IsRankingActive())
			{
				m_rankingInitloadingTime += Time.deltaTime;
			}
			else
			{
				m_rankingInitloadingTime = 0f;
			}
			if (m_first && !m_isInitilalized && m_rankingInitloadingTime > 0.5f)
			{
				InitSetting();
				m_rankingInitloadingTime = -10f;
			}
			else if (m_rankingInitloadingTime > 5f)
			{
				if (SingletonGameObject<RankingManager>.Instance != null)
				{
					RankingManager instance2 = SingletonGameObject<RankingManager>.Instance;
					if (!instance2.isLoading)
					{
						if (m_currentRankerType == RankingUtil.RankingRankerType.RIVAL && !instance2.IsRankingTop(RankingUtil.currentRankingMode, RankingUtil.RankingScoreType.HIGH_SCORE, RankingUtil.RankingRankerType.RIVAL))
						{
							SingletonGameObject<RankingManager>.Instance.InitNormal(RankingUtil.currentRankingMode, null);
							m_rankingInitloadingTime = -30f;
						}
						else
						{
							m_rankingInitloadingTime = -60f;
						}
					}
					else
					{
						m_rankingInitloadingTime = -40f;
					}
				}
				else
				{
					m_rankingInitloadingTime = -60f;
				}
			}
		}
		if (m_callbackTemporarilySaved == null || !base.gameObject.activeSelf)
		{
			return;
		}
		if (m_callbackTemporarilySavedDelay <= 0f)
		{
			m_callbackTemporarilySaved.SendCallback();
			m_callbackTemporarilySaved = null;
		}
		else if (IsRankingActive() && base.gameObject.activeSelf)
		{
			if (Time.deltaTime <= 0f)
			{
				m_callbackTemporarilySavedDelay -= 1f / (float)Application.targetFrameRate;
			}
			else
			{
				m_callbackTemporarilySavedDelay -= Time.deltaTime;
			}
		}
		else
		{
			m_callbackTemporarilySavedDelay = 0.25f;
		}
	}

	private void OnClickNextButton()
	{
	}

	public void OnHighScoreToggleChange()
	{
		if (!m_toggleLock)
		{
			scoreToggleChange(RankingUtil.RankingScoreType.HIGH_SCORE);
		}
	}

	public void OnWeeklyToggleChange()
	{
		if (!m_toggleLock)
		{
			scoreToggleChange(RankingUtil.RankingScoreType.TOTAL_SCORE);
		}
	}

	private void OnClickHelpButton()
	{
		if (m_help != null)
		{
			m_help.Open(!m_isHelp);
			m_isHelp = true;
		}
		SoundManager.SePlay("sys_menu_decide");
	}

	private void OnClickScoreType()
	{
		SoundManager.SePlay("sys_page_skip");
	}

	public void scoreToggleChange(RankingUtil.RankingScoreType scoreType)
	{
		if (m_isInitilalized && scoreType != m_currentScoreType)
		{
			SoundManager.SePlay("sys_page_skip");
			Debug.Log("SetRanking scoreToggleChange");
			SetRanking(m_currentRankerType, scoreType, 0);
			if (m_currentRankerType != RankingUtil.RankingRankerType.RIVAL)
			{
				m_lastSelectedScoreType = scoreType;
			}
		}
	}

	public void OnFriendToggleChange()
	{
		if (!m_toggleLock && m_currentRankerType != 0)
		{
			RankerToggleChange(RankingUtil.RankingRankerType.FRIEND);
		}
	}

	public void OnAllToggleChange()
	{
		if (!m_toggleLock && m_currentRankerType != RankingUtil.RankingRankerType.ALL)
		{
			RankerToggleChange(RankingUtil.RankingRankerType.ALL);
		}
	}

	public void OnRivalToggleChange()
	{
		if (!m_toggleLock && m_currentRankerType != RankingUtil.RankingRankerType.RIVAL)
		{
			RankerToggleChange(RankingUtil.RankingRankerType.RIVAL);
		}
	}

	public void OnHistoryToggleChange()
	{
		if (!m_toggleLock && m_currentRankerType != RankingUtil.RankingRankerType.HISTORY)
		{
			RankerToggleChange(RankingUtil.RankingRankerType.HISTORY);
		}
	}

	private void RankerToggleChange(RankingUtil.RankingRankerType rankerType)
	{
		if (!m_isInitilalized)
		{
			return;
		}
		SoundManager.SePlay("sys_page_skip");
		if (rankerType == RankingUtil.RankingRankerType.RIVAL)
		{
			m_currentScoreType = ((!m_partsTabRivalTogglH.value) ? RankingUtil.RankingScoreType.TOTAL_SCORE : RankingUtil.RankingScoreType.HIGH_SCORE);
		}
		else
		{
			m_currentScoreType = m_lastSelectedScoreType;
		}
		if (!SetRanking(rankerType, m_currentScoreType, 0))
		{
			if (m_loading != null)
			{
				m_loading.SetActive(true);
			}
			if (m_tallying != null)
			{
				m_tallying.SetActive(false);
			}
		}
	}

	private void OnClickMoreButton()
	{
		if (m_isInitilalized)
		{
			SoundManager.SePlay("sys_page_skip");
			SetRanking(m_currentRankerType, m_currentScoreType, 1);
		}
	}

	private void OnClickSnsLogin()
	{
		m_snsLogin = true;
		m_snsCompGetRankingTime = 0f;
		m_snsCompGetRanking = false;
		m_easySnsFeed = new EasySnsFeed(base.gameObject, "Camera/menu_Anim/ui_mm_ranking_page/Anchor_5_MC");
	}

	private void OnClickFriendOption()
	{
		SoundManager.SePlay("sys_menu_decide");
		GameObject loadMenuChildObject = HudMenuUtility.GetLoadMenuChildObject("RankingFriendOptionWindow", true);
		if (loadMenuChildObject != null)
		{
			RankingFriendOptionWindow component = loadMenuChildObject.GetComponent<RankingFriendOptionWindow>();
			if (component != null)
			{
				component.StartCoroutine("SetUp");
			}
		}
	}

	private void OnClickFriendOptionOk()
	{
		SetRanking(RankingUtil.RankingRankerType.FRIEND, m_currentScoreType, -1);
		if (SingletonGameObject<RankingManager>.Instance != null && EventManager.Instance != null && EventManager.Instance.Type == EventManager.EventType.SPECIAL_STAGE)
		{
			SingletonGameObject<RankingManager>.Instance.Reset(RankingUtil.RankingMode.ENDLESS, RankingUtil.RankingRankerType.SP_FRIEND);
		}
	}

	private void SetupRankingReset(TimeSpan span)
	{
		if (!(m_partsInfo != null))
		{
			return;
		}
		RankingManager instance = SingletonGameObject<RankingManager>.Instance;
		if (instance != null)
		{
			if (m_currentRankerType == RankingUtil.RankingRankerType.FRIEND && s_socialInterface != null && !s_socialInterface.IsLoggedIn)
			{
				m_partsInfo.text = string.Empty;
			}
			else
			{
				m_partsInfo.text = RankingUtil.GetResetTime(span);
			}
		}
	}

	private void SetupLeague()
	{
		RankingUtil.SetLeagueObject(RankingUtil.currentRankingMode, ref m_partsRankIcon0, ref m_partsRankIcon1, ref m_partsRankText, ref m_partsRankTextEx);
	}

	private bool IsActiveSnsLoginGameObject()
	{
		return m_currentRankerType == RankingUtil.RankingRankerType.FRIEND && s_socialInterface != null && !s_socialInterface.IsLoggedIn;
	}

	private void OnSettingPartsSnsAdditional()
	{
	}

	[Conditional("DEBUG_INFO")]
	private static void DebugLog(string s)
	{
		Debug.Log("@ms DEBUG_INFO " + s);
	}

	[Conditional("DEBUG_INFO2")]
	private static void DebugLog2(string s)
	{
		Debug.Log("@ms DEBUG_INFO2" + s);
	}

	[Conditional("DEBUG_INFO3")]
	private static void DebugLog3(string s)
	{
		Debug.Log("@ms DEBUG_INFO3" + s);
	}

	[Conditional("DEBUG_INFO")]
	private static void DebugLogWarning(string s)
	{
		Debug.LogWarning("@ms " + s);
	}

	public static RankingUI Setup()
	{
		if (s_instance != null)
		{
			s_instance.Init();
			return s_instance;
		}
		return null;
	}

	public static void CheckSnsUse()
	{
		if (s_instance != null)
		{
			s_instance.CheckSns();
		}
	}

	public void CheckSns()
	{
		if (RegionManager.Instance != null)
		{
			m_facebookLock = !RegionManager.Instance.IsUseSNS();
		}
		UIImageButton[] partsBtns = m_partsBtns;
		foreach (UIImageButton uIImageButton in partsBtns)
		{
			if (uIImageButton.name.IndexOf("friend") != -1)
			{
				uIImageButton.isEnabled = !m_facebookLock;
				break;
			}
		}
		m_facebookLockInit = true;
	}

	public void UpdateSendChallengeOrg(RankingUtil.RankingRankerType type, string id)
	{
		Debug.Log("RankingUI:UpdateSendChallengeOrg type:" + type);
		if (m_currentRankerType != type)
		{
			return;
		}
		if (type == RankingUtil.RankingRankerType.RIVAL)
		{
			if (!(m_pattern2 != null) || !m_pattern2.activeSelf || !(m_pattern2ListArea != null))
			{
				return;
			}
			ui_ranking_scroll[] componentsInChildren = m_pattern2ListArea.GetComponentsInChildren<ui_ranking_scroll>();
			if (componentsInChildren != null && componentsInChildren.Length > 0)
			{
				ui_ranking_scroll[] array = componentsInChildren;
				foreach (ui_ranking_scroll ui_ranking_scroll in array)
				{
					ui_ranking_scroll.UpdateSendChallenge(id);
				}
			}
		}
		else if (m_pattern0 != null && m_pattern0.activeSelf && m_pattern0TopRankerArea != null)
		{
			ui_ranking_scroll[] componentsInChildren2 = m_pattern0TopRankerArea.GetComponentsInChildren<ui_ranking_scroll>();
			if (componentsInChildren2 != null && componentsInChildren2.Length > 0)
			{
				ui_ranking_scroll[] array2 = componentsInChildren2;
				foreach (ui_ranking_scroll ui_ranking_scroll2 in array2)
				{
					ui_ranking_scroll2.UpdateSendChallenge(id);
				}
			}
		}
		else
		{
			if (!(m_pattern1 != null) || !m_pattern1.activeSelf || !(m_pattern1ListArea != null))
			{
				return;
			}
			ui_ranking_scroll[] componentsInChildren3 = m_pattern1ListArea.GetComponentsInChildren<ui_ranking_scroll>();
			if (componentsInChildren3 != null && componentsInChildren3.Length > 0)
			{
				ui_ranking_scroll[] array3 = componentsInChildren3;
				foreach (ui_ranking_scroll ui_ranking_scroll3 in array3)
				{
					ui_ranking_scroll3.UpdateSendChallenge(id);
				}
			}
		}
	}

	private bool IsRankingActive()
	{
		if (m_displayFlag)
		{
			return true;
		}
		return false;
	}

	public static void UpdateSendChallenge(RankingUtil.RankingRankerType type, string id)
	{
		if (s_instance != null)
		{
			s_instance.UpdateSendChallengeOrg(type, id);
		}
	}

	public static void SetLoading()
	{
		if (s_instance != null)
		{
			s_instance.SetLoadingObject();
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
			s_instance = this;
		}
		else
		{
			UnityEngine.Object.Destroy(base.gameObject);
		}
	}
}
