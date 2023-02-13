using AnimationOrTween;
using Message;
using System;
using System.Collections.Generic;
using Text;
using UnityEngine;

public class RouletteTop : CustomGameObject
{
	public enum ROULETTE_EFFECT_TYPE
	{
		BG_PARTICLE,
		SPIN,
		BOARD,
		NUM
	}

	[SerializeField]
	[Header("ルーレット種別ごとのカラー設定")]
	private Color m_premiumColor;

	[SerializeField]
	private Color m_specialColor;

	[SerializeField]
	private Color m_defaultColor;

	[SerializeField]
	private RouletteBoard m_orgRouletteBoard;

	[SerializeField]
	private RouletteStandardPart m_orgStdPartsBoard;

	[SerializeField]
	private Animation m_animation;

	[SerializeField]
	private List<UIPanel> m_panels;

	[SerializeField]
	private GameObject m_topPageObject;

	[SerializeField]
	private GameObject m_rouletteBase;

	[SerializeField]
	private GameObject m_stdPartsBase;

	[SerializeField]
	private GameObject m_buttonsBase;

	[SerializeField]
	private GameObject m_buttonsBaseBg;

	[SerializeField]
	private window_odds m_odds;

	[SerializeField]
	public Texture m_itemRouletteDefaultTexture;

	private bool m_updateRequest;

	private bool m_close;

	private bool m_change;

	private bool m_opened;

	private ServerWheelOptionsData m_wheelData;

	private ServerWheelOptionsData m_wheelDataAfter;

	private List<RoulettePartsBase> m_parts;

	private List<UIButtonMessage> m_buttons;

	private bool m_tutorial;

	private bool m_tutorialSpin;

	private bool m_addSpecialEgg;

	private bool m_word;

	private bool m_spin;

	private long m_spinCount;

	private bool m_spinSkip;

	private bool m_spinDecision;

	private int m_spinDecisionIndex = -1;

	private float m_spinTime;

	private float m_multiGetDelayTime;

	private float m_closeTime;

	private float m_removeTime;

	private bool m_wheelSetup;

	private List<RouletteCategory> m_rouletteList;

	private List<RouletteCategory> m_rouletteCostItemLoadedList;

	private RouletteCategory m_requestCostItemCategory;

	private ServerChaoSpinResult m_spinResult;

	private ServerSpinResultGeneral m_spinResultGeneral;

	private RouletteUtility.NextType m_nextType;

	private RouletteCategory m_requestCategory;

	private RouletteCategory m_setupNoCommunicationCategory;

	private UIImageButton m_backButtonImg;

	private List<ROULETTE_EFFECT_TYPE> m_notEffectList;

	private bool m_clickBack;

	private UIRectItemStorage m_topPageStorage;

	private List<GameObject> m_topPageRouletteList;

	private List<GameObject> m_topPageHeaderList;

	private RouletteCategory m_topPageOddsSelect;

	private bool m_topPageWheelData;

	private Dictionary<RouletteCategory, InformationWindow.Information> m_rouletteInfoList;

	private UILabel m_premiumRouletteLabel;

	private UILabel m_premiumRouletteShLabel;

	private SendApollo m_sendApollo;

	private float m_inputLimitTime;

	private bool m_isWindow;

	private bool m_isTopPage;

	private static RouletteTop s_instance;

	public bool addSpecialEgg
	{
		get
		{
			return m_addSpecialEgg;
		}
	}

	public bool isWindow
	{
		get
		{
			return m_isWindow;
		}
	}

	public bool isEnabled
	{
		get
		{
			bool result = false;
			if (base.gameObject.activeSelf && m_parts != null && m_parts.Count > 0)
			{
				result = true;
			}
			return result;
		}
	}

	public ServerWheelOptionsData wheelData
	{
		get
		{
			return m_wheelData;
		}
	}

	public RouletteCategory category
	{
		get
		{
			if (m_wheelData == null)
			{
				return RouletteCategory.NONE;
			}
			return m_wheelData.category;
		}
	}

	public float spinTime
	{
		get
		{
			if (!m_spin)
			{
				return 0f;
			}
			return m_spinTime;
		}
	}

	public bool isSpin
	{
		get
		{
			return m_spin;
		}
	}

	public bool isSpinSkip
	{
		get
		{
			return m_spinSkip;
		}
	}

	public bool isSpinDecision
	{
		get
		{
			return m_spinDecision;
		}
	}

	public int spinDecisionIndex
	{
		get
		{
			return m_spinDecisionIndex;
		}
	}

	public bool isSpinGetWindow
	{
		get
		{
			bool result = false;
			if (m_wheelDataAfter != null)
			{
				result = true;
			}
			return result;
		}
	}

	public bool isWordAnime
	{
		get
		{
			return m_word;
		}
	}

	public static RouletteTop Instance
	{
		get
		{
			return s_instance;
		}
	}

	public bool IsClose()
	{
		return !m_opened;
	}

	public Color GetBtnColor(RouletteCategory category)
	{
		Color result = m_defaultColor;
		switch (category)
		{
		case RouletteCategory.PREMIUM:
			result = m_premiumColor;
			break;
		case RouletteCategory.SPECIAL:
			result = m_specialColor;
			break;
		}
		return result;
	}

	public bool IsEffect(ROULETTE_EFFECT_TYPE type)
	{
		bool result = false;
		if (m_notEffectList != null && m_notEffectList.Count > 0)
		{
			if (!m_notEffectList.Contains(type))
			{
				result = true;
			}
		}
		else
		{
			result = true;
		}
		return result;
	}

	public void OnRouletteOpenItem()
	{
		if (m_removeTime == 0f || Mathf.Abs(Time.realtimeSinceStartup - m_removeTime) > 0.5f)
		{
			if (m_rouletteCostItemLoadedList != null)
			{
				m_rouletteCostItemLoadedList.Clear();
			}
			else
			{
				m_rouletteCostItemLoadedList = new List<RouletteCategory>();
			}
			m_isTopPage = false;
			m_tutorialSpin = false;
			m_opened = true;
			Debug.Log("RouletteTop:OnRouletteOpenItem!");
			RouletteManager.RouletteBgmReset();
			RouletteManager.RouletteOpen(RouletteCategory.ITEM);
		}
	}

	public void OnRouletteOpenPremium()
	{
		if (m_removeTime == 0f || Mathf.Abs(Time.realtimeSinceStartup - m_removeTime) > 0.5f)
		{
			if (m_rouletteCostItemLoadedList != null)
			{
				m_rouletteCostItemLoadedList.Clear();
			}
			else
			{
				m_rouletteCostItemLoadedList = new List<RouletteCategory>();
			}
			m_isTopPage = false;
			m_tutorialSpin = false;
			m_opened = true;
			Debug.Log("RouletteTop:OnRouletteOpenPremium!");
			RouletteManager.RouletteBgmReset();
			RouletteManager.RouletteOpen(RouletteCategory.PREMIUM);
		}
	}

	public void OnRouletteOpenRaid()
	{
		if (m_removeTime == 0f || Mathf.Abs(Time.realtimeSinceStartup - m_removeTime) > 0.5f)
		{
			if (m_rouletteCostItemLoadedList != null)
			{
				m_rouletteCostItemLoadedList.Clear();
			}
			else
			{
				m_rouletteCostItemLoadedList = new List<RouletteCategory>();
			}
			m_isTopPage = false;
			m_tutorialSpin = false;
			m_opened = true;
			Debug.Log("RouletteTop:OnRouletteOpenRaid!");
			RouletteManager.RouletteBgmReset();
			RouletteManager.RouletteOpen(RouletteCategory.RAID);
		}
	}

	public void OnRouletteOpenDefault()
	{
		if (m_removeTime == 0f || Mathf.Abs(Time.realtimeSinceStartup - m_removeTime) > 0.5f)
		{
			if (m_rouletteCostItemLoadedList != null)
			{
				m_rouletteCostItemLoadedList.Clear();
			}
			else
			{
				m_rouletteCostItemLoadedList = new List<RouletteCategory>();
			}
			m_isTopPage = false;
			m_tutorialSpin = false;
			m_opened = true;
			Debug.Log("RouletteTop:OnRouletteOpenDefault!  rouletteDefault:" + RouletteUtility.rouletteDefault);
			RouletteManager.RouletteBgmReset();
			RouletteManager.RouletteOpen();
		}
	}

	public void OnRouletteEnd()
	{
		RouletteManager.RouletteClose();
	}

	public void UpdateCostItemList(List<ServerItem.Id> costItemList)
	{
		SetTopPageHeaderObject();
	}

	protected override void UpdateStd(float deltaTime, float timeRate)
	{
		if (m_setupNoCommunicationCategory != 0)
		{
			if (GeneralWindow.IsCreated("SetupNoCommunication") && GeneralWindow.IsButtonPressed)
			{
				if (!GeneralUtil.IsNetwork())
				{
					GeneralUtil.ShowNoCommunication("SetupNoCommunication");
				}
				else
				{
					Setup(m_setupNoCommunicationCategory);
				}
			}
			return;
		}
		if (m_tutorial)
		{
			if (!GeneralWindow.Created)
			{
				if (GeneralWindow.IsCreated("RouletteTutorial") && GeneralWindow.IsOkButtonPressed && !m_tutorialSpin)
				{
					TutorialCursor.StartTutorialCursor(TutorialCursor.Type.ROULETTE_SPIN);
					m_tutorialSpin = true;
				}
				else if (GeneralWindow.IsCreated("RouletteTutorialEnd") && GeneralWindow.IsButtonPressed)
				{
					RouletteUtility.rouletteTurtorialEnd = true;
					m_tutorial = false;
					ItemGetWindow itemGetWindow = ItemGetWindowUtil.GetItemGetWindow();
					if (itemGetWindow != null)
					{
						itemGetWindow.Create(new ItemGetWindow.CInfo
						{
							name = "TutorialEndAddSp",
							caption = TextUtility.GetCommonText("MainMenu", "tutorial_sp_egg1_caption"),
							serverItemId = 220000,
							imageCount = TextUtility.GetCommonText("MainMenu", "tutorial_sp_egg1_text", "{COUNT}", 10.ToString())
						});
						SoundManager.SePlay("sys_specialegg");
					}
					Debug.Log("TurtorialEnd:" + RouletteUtility.rouletteTurtorialEnd + " !!!!!!!!!!!!!!!!!!!! ");
				}
				else if (GeneralWindow.IsCreated("RouletteTutorialError") && GeneralWindow.IsButtonPressed)
				{
					if (GeneralUtil.IsNetwork())
					{
						GeneralWindow.CInfo info = default(GeneralWindow.CInfo);
						info.name = "RouletteTutorialEnd";
						info.buttonType = GeneralWindow.ButtonType.Ok;
						info.caption = TextManager.GetText(TextManager.TextType.TEXTTYPE_COMMON_TEXT, "MainMenu", "end_of_tutorial_caption").text;
						info.message = TextManager.GetText(TextManager.TextType.TEXTTYPE_COMMON_TEXT, "MainMenu", "end_of_tutorial_text").text;
						GeneralWindow.Create(info);
						string[] value = new string[1];
						SendApollo.GetTutorialValue(ApolloTutorialIndex.START_STEP6, ref value);
						m_sendApollo = SendApollo.CreateRequest(ApolloType.TUTORIAL_END, value);
					}
					else
					{
						GeneralUtil.ShowNoCommunication("RouletteTutorialError");
					}
				}
			}
		}
		else if (RouletteUtility.rouletteTurtorialEnd)
		{
			ItemGetWindow itemGetWindow2 = ItemGetWindowUtil.GetItemGetWindow();
			if (itemGetWindow2 != null && itemGetWindow2.IsCreated("TutorialEndAddSp") && itemGetWindow2.IsEnd)
			{
				itemGetWindow2.Reset();
				if (RouletteManager.Instance != null && RouletteManager.Instance.specialEgg >= 10)
				{
					GeneralWindow.CInfo info2 = default(GeneralWindow.CInfo);
					info2.name = "SpEggMax";
					info2.buttonType = GeneralWindow.ButtonType.Ok;
					info2.caption = TextManager.GetText(TextManager.TextType.TEXTTYPE_COMMON_TEXT, "ChaoRoulette", "sp_egg_max_caption").text;
					info2.message = TextManager.GetText(TextManager.TextType.TEXTTYPE_COMMON_TEXT, "ChaoRoulette", "sp_egg_max_text").text;
					GeneralWindow.Create(info2);
				}
				RouletteManager.RequestRoulette(category, base.gameObject);
				RouletteUtility.rouletteTurtorialEnd = false;
				m_addSpecialEgg = false;
				Debug.Log("TurtorialEnd:" + RouletteUtility.rouletteTurtorialEnd + " !!!!!!!!!!!!!!!!!!!! ");
			}
		}
		if (m_spin)
		{
			m_spinTime += deltaTime;
			if (m_multiGetDelayTime > 0f && m_spinDecision)
			{
				m_multiGetDelayTime -= deltaTime;
				if (m_multiGetDelayTime <= 0f)
				{
					OnRouletteSpinEnd();
					m_multiGetDelayTime = 0f;
				}
			}
			m_spinCount++;
		}
		if (m_closeTime > 0f)
		{
			m_closeTime -= deltaTime;
			if (m_closeTime <= 0f)
			{
				m_clickBack = true;
				Close();
				m_closeTime = 0f;
			}
		}
		if (m_sendApollo != null && m_sendApollo.IsEnd())
		{
			UnityEngine.Object.Destroy(m_sendApollo.gameObject);
			m_sendApollo = null;
		}
		if (m_inputLimitTime > 0f)
		{
			m_inputLimitTime -= Time.deltaTime;
			if (m_inputLimitTime <= 0f)
			{
				m_inputLimitTime = 0f;
				HudMenuUtility.SetConnectAlertSimpleUI(false);
			}
		}
		if (m_topPageWheelData && m_topPageOddsSelect != 0 && RouletteManager.Instance != null && !RouletteManager.Instance.isCurrentPrizeLoading)
		{
			ServerPrizeState prizeList = RouletteManager.Instance.GetPrizeList(m_topPageOddsSelect);
			ServerWheelOptionsData rouletteDataOrg = RouletteManager.Instance.GetRouletteDataOrg(m_topPageOddsSelect);
			if (prizeList != null && rouletteDataOrg != null)
			{
				OpenOdds(prizeList, rouletteDataOrg);
				m_topPageWheelData = false;
				m_topPageOddsSelect = RouletteCategory.NONE;
			}
		}
	}

	public void SetPanelsAlpha(float alpha)
	{
		if (m_panels == null || m_panels.Count <= 0)
		{
			return;
		}
		foreach (UIPanel panel in m_panels)
		{
			panel.alpha = alpha;
		}
	}

	public float GetPanelsAlpha()
	{
		float num = -1f;
		if (m_parts != null && m_parts.Count > 0)
		{
			num = 0f;
			foreach (UIPanel panel in m_panels)
			{
				float num2 = panel.alpha;
				if (num2 > 1f)
				{
					num2 = 1f;
				}
				num += num2;
			}
			num /= (float)m_panels.Count;
			if (num > 1f)
			{
				num = 1f;
			}
			else if (num < 0.01f)
			{
				num = 0f;
			}
		}
		return num;
	}

	public bool OpenOdds(ServerPrizeState prize, ServerWheelOptionsData wheelOptionsData = null)
	{
		bool result = false;
		if (m_odds != null)
		{
			if (wheelOptionsData != null)
			{
				m_odds.Open(prize, wheelOptionsData);
			}
			else
			{
				m_odds.Open(prize, wheelData);
			}
			result = true;
		}
		return result;
	}

	public bool OnRouletteSpinStart(ServerWheelOptionsData data, int num)
	{
		if (data != null && data.category == RouletteCategory.RAID && EventManager.Instance != null && EventManager.Instance.TypeInTime != EventManager.EventType.RAID_BOSS)
		{
			GeneralUtil.ShowEventEnd();
			Setup(m_rouletteList[0]);
			return false;
		}
		bool result = false;
		m_nextType = RouletteUtility.NextType.NONE;
		m_closeTime = 0f;
		if (!isSpin)
		{
			m_spinCount = 0L;
			GC.Collect();
			if (m_tutorial)
			{
				TutorialCursor.EndTutorialCursor(TutorialCursor.Type.ROULETTE_SPIN);
			}
			if (m_backButtonImg != null)
			{
				m_backButtonImg.isEnabled = false;
			}
			m_spinDecisionIndex = -1;
			if (m_parts != null && m_parts.Count > 0)
			{
				foreach (RoulettePartsBase part in m_parts)
				{
					if (part != null)
					{
						part.OnSpinStart();
					}
				}
			}
			m_wheelSetup = false;
			m_spinTime = 0f;
			m_word = false;
			m_spin = true;
			m_spinSkip = false;
			m_spinDecision = false;
			result = RouletteManager.RequestCommitRoulette(data, num, base.gameObject);
		}
		return result;
	}

	public bool OnRouletteSpinSkip()
	{
		bool result = false;
		m_closeTime = 0f;
		if (isSpin && !isSpinSkip && m_spinDecisionIndex >= 0 && m_spinTime > 0.1f)
		{
			if (m_parts != null && m_parts.Count > 0)
			{
				foreach (RoulettePartsBase part in m_parts)
				{
					if (part != null)
					{
						part.OnSpinSkip();
					}
				}
			}
			m_spinSkip = true;
			m_spinDecision = true;
		}
		return result;
	}

	public bool OnRouletteSpinDecision(int decIndex)
	{
		bool result = false;
		m_closeTime = 0f;
		if (isSpin && !isSpinDecision)
		{
			if (decIndex >= 0)
			{
				if (m_backButtonImg != null)
				{
					m_backButtonImg.isEnabled = true;
				}
				m_spinDecisionIndex = decIndex;
				if (m_parts != null && m_parts.Count > 0)
				{
					foreach (RoulettePartsBase part in m_parts)
					{
						if (part != null)
						{
							part.OnSpinDecision();
						}
					}
				}
				m_spinSkip = false;
			}
			else
			{
				if (m_backButtonImg != null)
				{
					m_backButtonImg.isEnabled = false;
				}
				m_multiGetDelayTime = 5f;
				if (m_parts != null && m_parts.Count > 0)
				{
					foreach (RoulettePartsBase part2 in m_parts)
					{
						if (part2 != null)
						{
							part2.OnSpinDecisionMulti();
						}
					}
				}
				m_spinSkip = true;
			}
			m_spinDecision = true;
			result = true;
		}
		return result;
	}

	public bool OnRouletteSpinEnd()
	{
		HudMenuUtility.SetConnectAlertSimpleUI(true);
		m_inputLimitTime = 5f;
		bool result = false;
		m_closeTime = 0f;
		m_multiGetDelayTime = 0f;
		if (isSpin && isSpinDecision)
		{
			GC.Collect();
			SetDelayTime(0.25f);
			if (m_backButtonImg != null)
			{
				m_backButtonImg.isEnabled = true;
			}
			m_word = true;
			if (m_parts != null && m_parts.Count > 0)
			{
				foreach (RoulettePartsBase part in m_parts)
				{
					if (part != null)
					{
						part.OnSpinEnd();
					}
				}
			}
			m_spin = false;
			m_spinSkip = false;
			m_spinDecision = false;
			result = true;
		}
		return result;
	}

	public bool OnRouletteWordAnimeEnd()
	{
		m_inputLimitTime = 0.25f;
		bool result = false;
		float delay = 0f;
		m_closeTime = 0f;
		m_multiGetDelayTime = 0f;
		if (m_word)
		{
			SetDelayTime(0.25f);
			m_word = false;
			HudMenuUtility.SendMsgUpdateSaveDataDisplay();
			if (m_spinResultGeneral != null)
			{
				if (m_spinResultGeneral.ItemWon >= 0 && m_wheelData.GetRouletteRank() != RouletteUtility.WheelRank.Super)
				{
					int num = 0;
					if (m_wheelData.GetCellItem(m_spinResultGeneral.ItemWon, out num).idType == ServerItem.IdType.ITEM_ROULLETE_WIN)
					{
						CloseGetWindow(RouletteUtility.AchievementType.NONE);
					}
					else
					{
						RouletteUtility.ShowGetWindow(m_spinResultGeneral);
						delay = 0.5f;
					}
				}
				else
				{
					RouletteUtility.ShowGetWindow(m_spinResultGeneral);
					delay = 0.5f;
				}
				m_spinResultGeneral = null;
			}
			else if (m_spinResult != null)
			{
				RouletteUtility.ShowGetWindow(m_spinResult);
				m_spinResult = null;
				delay = 0.5f;
			}
			else
			{
				Debug.Log("OnRouletteWordAnimeEnd error?");
				if (m_wheelData.itemWonData.idType == ServerItem.IdType.ITEM_ROULLETE_WIN && m_wheelData.GetRouletteRank() != RouletteUtility.WheelRank.Super)
				{
					CloseGetWindow(RouletteUtility.AchievementType.NONE);
				}
				else
				{
					RouletteUtility.ShowGetWindow(m_wheelData.GetOrgRankupData());
					delay = 0.5f;
				}
			}
			result = true;
		}
		ServerWheelOptionsData serverWheelOptionsData = RouletteManager.UpdateRoulette(m_wheelData.category, delay);
		if (serverWheelOptionsData != null)
		{
			UpdateWheelData(serverWheelOptionsData, false);
		}
		return result;
	}

	public void OnRouletteGetError(MsgServerConnctFailed msg)
	{
		m_spin = false;
		m_spinSkip = false;
		m_spinDecision = false;
		SetDelayTime(0.5f);
		m_closeTime = 0.1f;
		m_multiGetDelayTime = 0f;
	}

	public bool OnRouletteSpinError(MsgServerConnctFailed msg)
	{
		Debug.Log("RouletteTop  OnRouletteSpinError !!!!!!!");
		bool result = false;
		SetDelayTime(0.5f);
		m_closeTime = 0.1f;
		m_multiGetDelayTime = 0f;
		if (isSpin)
		{
			if (m_backButtonImg != null)
			{
				m_backButtonImg.isEnabled = true;
			}
			if (m_parts != null && m_parts.Count > 0)
			{
				foreach (RoulettePartsBase part in m_parts)
				{
					if (part != null)
					{
						part.OnSpinError();
					}
				}
			}
			m_spin = false;
			m_spinSkip = false;
			m_spinDecision = false;
			result = true;
		}
		return result;
	}

	public void UpdateEffectSetting()
	{
		if (m_parts == null || m_parts.Count <= 0)
		{
			return;
		}
		foreach (RoulettePartsBase part in m_parts)
		{
			if (part != null)
			{
				part.UpdateEffectSetting();
			}
		}
	}

	public void OpenRouletteWindow()
	{
		if (!base.gameObject.activeSelf || m_parts == null || m_parts.Count <= 0)
		{
			return;
		}
		m_isWindow = true;
		foreach (RoulettePartsBase part in m_parts)
		{
			if (part != null)
			{
				part.windowOpen();
			}
		}
	}

	public void CloseRouletteWindow()
	{
		if (!base.gameObject.activeSelf)
		{
			return;
		}
		if (m_parts != null && m_parts.Count > 0)
		{
			m_isWindow = false;
			foreach (RoulettePartsBase part in m_parts)
			{
				if (part != null)
				{
					part.windowClose();
				}
			}
		}
		if (m_tutorial)
		{
			if (GeneralUtil.IsNetwork())
			{
				GeneralWindow.CInfo info = default(GeneralWindow.CInfo);
				info.name = "RouletteTutorialEnd";
				info.buttonType = GeneralWindow.ButtonType.Ok;
				info.caption = TextManager.GetText(TextManager.TextType.TEXTTYPE_COMMON_TEXT, "MainMenu", "end_of_tutorial_caption").text;
				info.message = TextManager.GetText(TextManager.TextType.TEXTTYPE_COMMON_TEXT, "MainMenu", "end_of_tutorial_text").text;
				GeneralWindow.Create(info);
				string[] value = new string[1];
				SendApollo.GetTutorialValue(ApolloTutorialIndex.START_STEP6, ref value);
				m_sendApollo = SendApollo.CreateRequest(ApolloType.TUTORIAL_END, value);
			}
			else
			{
				GeneralUtil.ShowNoCommunication("RouletteTutorialError");
			}
			TutorialCursor.EndTutorialCursor(TutorialCursor.Type.ROULETTE_OK);
		}
	}

	public void CloseGetWindow(RouletteUtility.AchievementType achievement, RouletteUtility.NextType nextType = RouletteUtility.NextType.NONE)
	{
		CloseRouletteWindow();
		if (nextType == RouletteUtility.NextType.NONE)
		{
			if (m_wheelDataAfter != null && !m_change)
			{
				UpdateWheelData(m_wheelDataAfter);
				if (!m_change)
				{
					m_wheelDataAfter = null;
				}
			}
		}
		else
		{
			Close(nextType);
		}
	}

	public void SetDelayTime(float delay = 0.2f)
	{
		if (m_parts == null || m_parts.Count <= 0)
		{
			return;
		}
		foreach (RoulettePartsBase part in m_parts)
		{
			if (part != null)
			{
				part.SetDelayTime(delay);
			}
		}
	}

	public void BtnInit()
	{
		if (!(m_buttonsBase != null))
		{
			return;
		}
		if (m_buttons == null)
		{
			bool activeSelf = m_buttonsBase.activeSelf;
			m_buttonsBase.SetActive(true);
			for (int i = 0; i < 10; i++)
			{
				UIButtonMessage uIButtonMessage = GameObjectUtil.FindChildGameObjectComponent<UIButtonMessage>(m_buttonsBase, "Btn_" + i);
				if (uIButtonMessage != null)
				{
					if (m_buttons == null)
					{
						m_buttons = new List<UIButtonMessage>();
					}
					uIButtonMessage.gameObject.SetActive(false);
					m_buttons.Add(uIButtonMessage);
					continue;
				}
				break;
			}
			m_buttonsBase.SetActive(activeSelf);
		}
		else
		{
			if (m_buttons.Count <= 0)
			{
				return;
			}
			foreach (UIButtonMessage button in m_buttons)
			{
				button.gameObject.SetActive(false);
			}
		}
	}

	public bool SetupTopPage(bool init = true)
	{
		if (m_close)
		{
			return false;
		}
		if (RouletteManager.Instance == null)
		{
			return false;
		}
		ResetParts();
		if (m_topPageObject != null)
		{
			m_topPageObject.SetActive(true);
		}
		if (HudMenuUtility.IsNumPlayingRouletteTutorial() && RouletteUtility.isTutorial)
		{
			TutorialCursor.StartTutorialCursor(TutorialCursor.Type.ROULETTE_TOP_PAGE);
		}
		if (m_topPageStorage == null && m_topPageObject != null)
		{
			m_topPageStorage = GameObjectUtil.FindChildGameObjectComponent<UIRectItemStorage>(m_topPageObject, "list");
		}
		if (m_topPageHeaderList == null && m_topPageObject != null)
		{
			GameObject gameObject = GameObjectUtil.FindChildGameObject(m_topPageObject, "window_header");
			if (gameObject != null)
			{
				string text = "img_{PARAM}_bg";
				for (int i = 0; i < 10; i++)
				{
					string name = text.Replace("{PARAM}", i.ToString());
					GameObject gameObject2 = GameObjectUtil.FindChildGameObject(gameObject, name);
					if (gameObject2 != null)
					{
						if (m_topPageHeaderList == null)
						{
							m_topPageHeaderList = new List<GameObject>();
						}
						gameObject2.SetActive(false);
						m_topPageHeaderList.Add(gameObject2);
					}
				}
			}
		}
		else
		{
			SetTopPageHeaderObject();
		}
		if (m_topPageRouletteList != null)
		{
			m_topPageRouletteList.Clear();
		}
		if (m_topPageStorage != null)
		{
			m_topPageStorage.maxItemCount = (m_topPageStorage.maxRows = 0);
			m_topPageStorage.Restart();
		}
		m_isTopPage = true;
		RouletteUtility.ChangeRouletteHeader(RouletteCategory.ALL);
		if (m_backButtonImg == null)
		{
			m_backButtonImg = GameObjectUtil.FindChildGameObjectComponent<UIImageButton>(base.gameObject, "Btn_cmn_back");
		}
		if (m_buttonsBase != null)
		{
			if (m_buttons == null)
			{
				for (int j = 0; j < 10; j++)
				{
					UIButtonMessage uIButtonMessage = GameObjectUtil.FindChildGameObjectComponent<UIButtonMessage>(m_buttonsBase, "Btn_" + j);
					if (uIButtonMessage != null)
					{
						if (m_buttons == null)
						{
							m_buttons = new List<UIButtonMessage>();
						}
						uIButtonMessage.gameObject.SetActive(false);
						m_buttons.Add(uIButtonMessage);
						continue;
					}
					break;
				}
			}
			else if (m_buttons.Count > 0)
			{
				foreach (UIButtonMessage button in m_buttons)
				{
					button.gameObject.SetActive(false);
				}
			}
		}
		m_isWindow = false;
		m_requestCategory = RouletteCategory.NONE;
		RouletteUtility.rouletteDefault = RouletteCategory.NONE;
		if (RouletteUtility.rouletteDefault != RouletteCategory.ITEM && RouletteUtility.rouletteDefault != RouletteCategory.PREMIUM)
		{
			RouletteUtility.rouletteDefault = RouletteCategory.PREMIUM;
		}
		base.gameObject.SetActive(true);
		if (init)
		{
			if (m_buttonsBase != null)
			{
				m_buttonsBase.SetActive(false);
			}
			if (m_buttonsBaseBg != null)
			{
				m_buttonsBaseBg.SetActive(false);
			}
			RouletteManager.Instance.RequestRouletteBasicInformation(base.gameObject);
		}
		else
		{
			if (m_buttonsBase != null)
			{
				m_buttonsBase.SetActive(false);
			}
			if (m_buttonsBaseBg != null)
			{
				m_buttonsBaseBg.SetActive(false);
			}
			SetTopPageHeaderObject();
			UpdateChangeBotton(RouletteCategory.ALL);
		}
		EventManager.EventType type = EventManager.Instance.Type;
		string text2 = null;
		string cueSheetName = "BGM";
		if (type != EventManager.EventType.NUM && type != EventManager.EventType.UNKNOWN && type != EventManager.EventType.ADVERT && EventManager.Instance.IsInEvent() && EventCommonDataTable.Instance != null)
		{
			string data = EventCommonDataTable.Instance.GetData(EventCommonDataItem.Roulette_BgmName);
			if (!string.IsNullOrEmpty(data))
			{
				cueSheetName = "BGM_" + EventManager.GetEventTypeName(EventManager.Instance.Type);
				text2 = data;
			}
		}
		if (string.IsNullOrEmpty(text2))
		{
			text2 = "bgm_sys_roulette";
		}
		if (!string.IsNullOrEmpty(text2))
		{
			SoundManager.BgmChange(text2, cueSheetName);
		}
		return true;
	}

	public bool Setup(RouletteCategory category)
	{
		if (m_close)
		{
			return false;
		}
		if (RouletteManager.Instance == null)
		{
			return false;
		}
		if (m_topPageObject != null)
		{
			m_topPageObject.SetActive(false);
		}
		if (HudMenuUtility.IsNumPlayingRouletteTutorial() && RouletteUtility.isTutorial)
		{
			TutorialCursor.EndTutorialCursor(TutorialCursor.Type.ROULETTE_TOP_PAGE);
		}
		m_setupNoCommunicationCategory = RouletteCategory.NONE;
		if (!GeneralUtil.IsNetwork())
		{
			m_setupNoCommunicationCategory = category;
			GeneralUtil.ShowNoCommunication("SetupNoCommunication");
			return false;
		}
		bool isTopPage = m_isTopPage;
		m_isTopPage = false;
		if (m_backButtonImg == null)
		{
			m_backButtonImg = GameObjectUtil.FindChildGameObjectComponent<UIImageButton>(base.gameObject, "Btn_cmn_back");
		}
		if (m_buttonsBase != null)
		{
			if (m_buttons == null)
			{
				for (int i = 0; i < 10; i++)
				{
					UIButtonMessage uIButtonMessage = GameObjectUtil.FindChildGameObjectComponent<UIButtonMessage>(m_buttonsBase, "Btn_" + i);
					if (uIButtonMessage != null)
					{
						if (m_buttons == null)
						{
							m_buttons = new List<UIButtonMessage>();
						}
						uIButtonMessage.gameObject.SetActive(false);
						m_buttons.Add(uIButtonMessage);
						continue;
					}
					break;
				}
			}
			else if (m_buttons.Count > 0)
			{
				foreach (UIButtonMessage button in m_buttons)
				{
					button.gameObject.SetActive(false);
				}
			}
		}
		if (category == RouletteCategory.SPECIAL)
		{
			category = RouletteCategory.PREMIUM;
		}
		m_isWindow = false;
		m_requestCategory = category;
		RouletteUtility.rouletteDefault = category;
		if (RouletteUtility.rouletteDefault != RouletteCategory.ITEM && RouletteUtility.rouletteDefault != RouletteCategory.PREMIUM)
		{
			RouletteUtility.rouletteDefault = RouletteCategory.PREMIUM;
		}
		if (isEnabled && !isTopPage)
		{
			if (m_buttonsBase != null)
			{
				m_buttonsBase.SetActive(false);
			}
			if (m_buttonsBaseBg != null)
			{
				m_buttonsBaseBg.SetActive(false);
			}
			SetDelayTime(1f);
			ServerWheelOptionsData rouletteData = RouletteManager.GetRouletteData(category);
			if (rouletteData != null)
			{
				UpdateWheelData(rouletteData);
				if (m_buttons.Count > 0)
				{
					int num = 0;
					foreach (RouletteCategory roulette in m_rouletteList)
					{
						if (m_buttons.Count > num)
						{
							UpdateChangeBottonIcon(roulette, m_buttons[num], num, roulette != category);
							num++;
							continue;
						}
						break;
					}
				}
			}
			else
			{
				m_updateRequest = true;
				RouletteManager.RequestRoulette(category, base.gameObject);
			}
		}
		else
		{
			if (m_buttonsBase != null)
			{
				m_buttonsBase.SetActive(false);
			}
			if (m_buttonsBaseBg != null)
			{
				m_buttonsBaseBg.SetActive(false);
			}
			if (category != RouletteCategory.ITEM)
			{
				RouletteManager.RouletteBgmReset();
			}
			RouletteManager.ResetRoulette();
			if (!isTopPage)
			{
				SetPanelsAlpha(0f);
			}
			base.gameObject.SetActive(true);
			m_updateRequest = false;
			RouletteManager.RequestRoulette(category, base.gameObject);
		}
		if (!isTopPage)
		{
			RouletteManager.Instance.RequestRouletteBasicInformation(base.gameObject);
		}
		else
		{
			SetTopPageHeaderObject();
			UpdateChangeBotton(m_requestCategory);
		}
		return true;
	}

	private void SetupWheelData(ServerWheelOptionsData wheelData)
	{
		m_setupNoCommunicationCategory = RouletteCategory.NONE;
		if (wheelData == null)
		{
			return;
		}
		m_isTopPage = false;
		m_wheelSetup = true;
		m_wheelDataAfter = null;
		m_closeTime = 0f;
		m_nextType = RouletteUtility.NextType.NONE;
		m_spinTime = 0f;
		m_multiGetDelayTime = 0f;
		m_word = false;
		m_spin = false;
		m_spinSkip = false;
		m_spinDecision = false;
		m_spinDecisionIndex = -1;
		m_wheelData = wheelData;
		m_close = false;
		m_clickBack = false;
		SetPanelsAlpha(1f);
		base.gameObject.SetActive(true);
		if (m_animation != null)
		{
			ActiveAnimation activeAnimation = ActiveAnimation.Play(m_animation, "ui_mm_roulette_intro_Anim", Direction.Forward);
			EventDelegate.Add(activeAnimation.onFinished, AnimationFinishCallback, true);
			if (m_wheelData.category == RouletteCategory.SPECIAL)
			{
				SoundManager.BgmStop();
				m_wheelData.PlaySe(ServerWheelOptionsData.SE_TYPE.Change, 0f);
				m_wheelData.PlayBgm(2.2f);
			}
			else
			{
				m_wheelData.PlaySe(ServerWheelOptionsData.SE_TYPE.Open, 0f);
				m_wheelData.PlayBgm(0.3f);
			}
		}
		ResetParts();
		if (m_rouletteBase != null && m_orgRouletteBoard != null)
		{
			CreateParts(m_orgRouletteBoard.gameObject, m_rouletteBase);
		}
		if (m_stdPartsBase != null && m_orgStdPartsBoard != null)
		{
			CreateParts(m_orgStdPartsBoard.gameObject, m_stdPartsBase);
		}
		RouletteUtility.ChangeRouletteHeader(m_wheelData.category);
		if (m_buttonsBase != null)
		{
			m_buttonsBase.SetActive(false);
		}
		if (m_buttonsBaseBg != null)
		{
			m_buttonsBaseBg.SetActive(false);
		}
		UpdateChangeBotton(m_wheelData.category);
	}

	public void UpdateWheel(ServerWheelOptionsData wheelData, bool changeEffect)
	{
		m_isTopPage = false;
		if (base.gameObject.activeSelf)
		{
			UpdateWheelData(wheelData, changeEffect);
		}
	}

	private void UpdateWheelData(ServerWheelOptionsData wheelData, bool changeEffect = true)
	{
		m_setupNoCommunicationCategory = RouletteCategory.NONE;
		if (wheelData == null)
		{
			return;
		}
		m_isTopPage = false;
		if (wheelData.isGeneral && (m_rouletteCostItemLoadedList == null || !m_rouletteCostItemLoadedList.Contains(wheelData.category)) && ServerInterface.LoggedInServerInterface != null)
		{
			List<int> spinCostItemIdList = wheelData.GetSpinCostItemIdList();
			if (spinCostItemIdList != null && spinCostItemIdList.Count > 0)
			{
				List<int> list = new List<int>();
				foreach (int item in spinCostItemIdList)
				{
					if (item != 910000 && item != 900000)
					{
						list.Add(item);
					}
				}
				if (list.Count > 0)
				{
					m_requestCostItemCategory = wheelData.category;
					ServerInterface.LoggedInServerInterface.RequestServerGetItemStockNum(EventManager.Instance.Id, list, base.gameObject);
				}
			}
		}
		m_closeTime = 0f;
		m_nextType = RouletteUtility.NextType.NONE;
		m_spinTime = 0f;
		m_multiGetDelayTime = 0f;
		m_spin = false;
		m_spinSkip = false;
		m_spinDecision = false;
		m_spinDecisionIndex = -1;
		if (m_wheelData.category == RouletteCategory.PREMIUM && wheelData.category == RouletteCategory.SPECIAL)
		{
			SoundManager.BgmStop();
			wheelData.PlaySe(ServerWheelOptionsData.SE_TYPE.Change, 0f);
			wheelData.PlayBgm(2.2f);
			changeEffect = false;
		}
		else if (m_wheelData.category != RouletteCategory.SPECIAL && wheelData.category == RouletteCategory.SPECIAL)
		{
			SoundManager.BgmStop();
			wheelData.PlaySe(ServerWheelOptionsData.SE_TYPE.Change, 0f);
			wheelData.PlayBgm(2.2f);
			changeEffect = false;
		}
		else if (m_wheelData.category == RouletteCategory.SPECIAL && wheelData.category == RouletteCategory.PREMIUM)
		{
			wheelData.PlayBgm(0.3f);
			changeEffect = false;
		}
		else
		{
			bool flag = false;
			if (m_wheelData.category == RouletteCategory.SPECIAL && wheelData.category == RouletteCategory.SPECIAL && RouletteManager.Instance != null && string.IsNullOrEmpty(RouletteManager.Instance.oldBgmName))
			{
				flag = true;
			}
			if (flag)
			{
				wheelData.PlaySe(ServerWheelOptionsData.SE_TYPE.Change, 0f);
				wheelData.PlayBgm(2.2f);
				changeEffect = false;
			}
			else
			{
				wheelData.PlayBgm(0.3f);
				if (changeEffect)
				{
					wheelData.PlaySe(ServerWheelOptionsData.SE_TYPE.Open, 0f);
				}
				changeEffect = false;
			}
		}
		if (changeEffect && m_wheelData.category != wheelData.category)
		{
			m_wheelDataAfter = wheelData;
			m_close = false;
			m_clickBack = false;
			m_change = true;
			if (m_animation != null)
			{
				ActiveAnimation activeAnimation = ActiveAnimation.Play(m_animation, "ui_mm_roulette_intro_Anim", Direction.Reverse);
				EventDelegate.Add(activeAnimation.onFinished, AnimationFinishCallback, true);
			}
			return;
		}
		int multi = m_wheelData.multi;
		m_wheelData = new ServerWheelOptionsData(wheelData);
		m_wheelData.ChangeMulti(multi);
		UpdateChangeBotton(m_wheelData.category);
		m_wheelDataAfter = null;
		m_close = false;
		m_clickBack = false;
		m_change = false;
		if (m_parts != null && m_parts.Count > 0)
		{
			foreach (RoulettePartsBase part in m_parts)
			{
				if (part != null)
				{
					part.OnUpdateWheelData(m_wheelData);
				}
			}
		}
		RouletteUtility.ChangeRouletteHeader(m_wheelData.category);
		m_wheelSetup = true;
	}

	private void ServerAddSpecialEgg_Succeeded(MsgAddSpecialEggSucceed msg)
	{
	}

	private void ServerAddSpecialEgg_Failed(MsgServerConnctFailed msg)
	{
	}

	private void ServerGetItemStockNum_Succeeded(MsgGetItemStockNumSucceed msg)
	{
		if (m_rouletteCostItemLoadedList == null)
		{
			m_rouletteCostItemLoadedList = new List<RouletteCategory>();
		}
		if (!m_rouletteCostItemLoadedList.Contains(m_requestCostItemCategory))
		{
			m_rouletteCostItemLoadedList.Add(m_requestCostItemCategory);
		}
		if (m_parts != null && m_parts.Count > 0)
		{
			foreach (RoulettePartsBase part in m_parts)
			{
				if (part != null)
				{
					part.PartsSendMessage("CostItemUpdate");
				}
			}
		}
		m_requestCostItemCategory = RouletteCategory.NONE;
	}

	public bool Close(RouletteUtility.NextType nextType = RouletteUtility.NextType.NONE)
	{
		if (m_close)
		{
			return false;
		}
		RouletteUtility.loginRoulette = false;
		if (m_rouletteCostItemLoadedList != null)
		{
			m_rouletteCostItemLoadedList.Clear();
		}
		SetDelayTime(0.25f);
		m_nextType = nextType;
		m_spinTime = 0f;
		m_multiGetDelayTime = 0f;
		m_spin = false;
		m_spinSkip = false;
		m_spinDecision = false;
		m_spinDecisionIndex = -1;
		if (m_nextType != 0)
		{
			switch (m_nextType)
			{
			case RouletteUtility.NextType.CHARA_EQUIP:
				HudMenuUtility.SendMenuButtonClicked(ButtonInfoTable.ButtonType.CHARA_MAIN, true);
				break;
			case RouletteUtility.NextType.EQUIP:
				HudMenuUtility.SendMenuButtonClicked(ButtonInfoTable.ButtonType.CHAO, true);
				break;
			}
		}
		else
		{
			m_close = true;
			if (m_animation != null)
			{
				ActiveAnimation activeAnimation = ActiveAnimation.Play(m_animation, "ui_mm_roulette_intro_Anim", Direction.Reverse);
				EventDelegate.Add(activeAnimation.onFinished, AnimationFinishCallback, true);
				if (m_isTopPage)
				{
					SoundManager.SePlay("sys_window_close");
				}
				else if (m_wheelData != null)
				{
					m_wheelData.PlaySe(ServerWheelOptionsData.SE_TYPE.Close, 0f);
				}
			}
		}
		return true;
	}

	public void Remove()
	{
		Debug.Log("RouletteTop Remove!");
		m_closeTime = 0f;
		m_spinTime = 0f;
		m_multiGetDelayTime = 0f;
		m_spin = false;
		m_spinSkip = false;
		m_spinDecision = false;
		m_spinDecisionIndex = -1;
		m_opened = false;
		m_close = false;
		SetPanelsAlpha(0f);
		HudMenuUtility.SendEnableShopButton(true);
		ChaoTextureManager.Instance.RemoveChaoTexture();
		ResetParts();
		RouletteManager.RouletteBgmReset();
		HudMenuUtility.ChangeMainMenuBGM();
		m_removeTime = Time.realtimeSinceStartup;
		if (m_clickBack)
		{
			HudMenuUtility.SendMenuButtonClicked(ButtonInfoTable.ButtonType.ROULETTE_BACK);
		}
		m_clickBack = false;
	}

	private void CreateParts(GameObject org, GameObject parent)
	{
		GameObject gameObject = UnityEngine.Object.Instantiate(org, Vector3.zero, Quaternion.identity) as GameObject;
		gameObject.gameObject.transform.parent = parent.transform;
		gameObject.gameObject.transform.localPosition = new Vector3(0f, 0f, 0f);
		gameObject.gameObject.transform.localScale = new Vector3(1f, 1f, 1f);
		RoulettePartsBase component = gameObject.gameObject.GetComponent<RoulettePartsBase>();
		if (component != null)
		{
			component.Setup(this);
			component.SetDelayTime(0f);
			if (m_parts == null)
			{
				m_parts = new List<RoulettePartsBase>();
			}
			m_parts.Add(component);
		}
	}

	private void ResetParts()
	{
		if (m_parts == null || m_parts.Count <= 0)
		{
			return;
		}
		for (int i = 0; i < m_parts.Count; i++)
		{
			if (m_parts[i] != null)
			{
				m_parts[i].DestroyParts();
			}
		}
		m_parts.Clear();
		m_parts = null;
	}

	public static RouletteTop RouletteTopPageCreate()
	{
		if (s_instance != null)
		{
			s_instance.SetupTopPage();
			return s_instance;
		}
		return null;
	}

	public static RouletteTop RouletteCreate(RouletteCategory category)
	{
		if (s_instance != null && category != 0 && category != RouletteCategory.ALL)
		{
			s_instance.Setup(category);
			return s_instance;
		}
		return null;
	}

	public void UpdateChangeBotton(RouletteCategory current)
	{
		if (current == RouletteCategory.SPECIAL)
		{
			current = RouletteCategory.PREMIUM;
		}
		if (m_rouletteList == null || m_rouletteList.Count <= 0)
		{
			return;
		}
		if (m_rouletteList.Contains(RouletteCategory.RAID) && EventManager.Instance != null && EventManager.Instance.TypeInTime != EventManager.EventType.RAID_BOSS)
		{
			m_rouletteList.Remove(RouletteCategory.RAID);
		}
		SetTopPageObject();
		if (m_buttons == null)
		{
			return;
		}
		switch (current)
		{
		case RouletteCategory.NONE:
		case RouletteCategory.GENERAL:
			break;
		default:
		{
			for (int j = 0; j < m_buttons.Count; j++)
			{
				m_buttons[j].gameObject.SetActive(false);
			}
			break;
		}
		case RouletteCategory.ALL:
		{
			for (int i = 0; i < m_buttons.Count; i++)
			{
				m_buttons[i].gameObject.SetActive(false);
			}
			break;
		}
		}
	}

	private void ResetChangeBotton()
	{
		if (m_rouletteList != null && m_rouletteList.Count > 0 && m_buttons != null)
		{
			for (int i = 0; i < m_buttons.Count; i++)
			{
				m_buttons[i].gameObject.SetActive(false);
			}
		}
	}

	private void UpdateChangeBottonIcon(RouletteCategory category, UIButtonMessage button, int idx, bool enabled)
	{
		UIImageButton component = button.GetComponent<UIImageButton>();
		if (component != null)
		{
			component.isEnabled = enabled;
		}
		UISprite uISprite = null;
		UISprite uISprite2 = null;
		if (category == RouletteCategory.PREMIUM || category == RouletteCategory.SPECIAL)
		{
			button.functionName = "OnClickChangeBtn_" + idx;
			uISprite = GameObjectUtil.FindChildGameObjectComponent<UISprite>(button.gameObject, "img_btn_icon");
			uISprite2 = GameObjectUtil.FindChildGameObjectComponent<UISprite>(button.gameObject, "img_move_icon");
			if (RouletteManager.Instance.specialEgg >= 10 && !m_tutorial && !RouletteUtility.rouletteTurtorialEnd)
			{
				if (uISprite != null)
				{
					uISprite.gameObject.SetActive(false);
				}
				if (uISprite2 != null)
				{
					uISprite2.spriteName = RouletteUtility.GetRouletteChangeIconSpriteName(RouletteCategory.SPECIAL);
					uISprite2.gameObject.SetActive(true);
				}
			}
			else
			{
				if (uISprite != null)
				{
					uISprite.gameObject.SetActive(true);
					uISprite.spriteName = RouletteUtility.GetRouletteChangeIconSpriteName(category);
				}
				if (uISprite2 != null)
				{
					uISprite2.gameObject.SetActive(false);
				}
			}
		}
		else
		{
			button.functionName = "OnClickChangeBtn_" + idx;
			uISprite = GameObjectUtil.FindChildGameObjectComponent<UISprite>(button.gameObject, "img_btn_icon");
			uISprite2 = GameObjectUtil.FindChildGameObjectComponent<UISprite>(button.gameObject, "img_move_icon");
			if (uISprite != null)
			{
				uISprite.gameObject.SetActive(true);
				uISprite.spriteName = RouletteUtility.GetRouletteChangeIconSpriteName(category);
			}
			if (uISprite2 != null)
			{
				uISprite2.gameObject.SetActive(false);
			}
		}
	}

	private void UpdateChangeBottonIcon(RouletteCategory category, UIButtonMessage button, int idx)
	{
		UISprite uISprite = null;
		UISprite uISprite2 = null;
		if (category == RouletteCategory.PREMIUM || category == RouletteCategory.SPECIAL)
		{
			button.functionName = "OnClickChangeBtn_" + idx;
			uISprite = GameObjectUtil.FindChildGameObjectComponent<UISprite>(button.gameObject, "img_btn_icon");
			uISprite2 = GameObjectUtil.FindChildGameObjectComponent<UISprite>(button.gameObject, "img_move_icon");
			if (RouletteManager.Instance.specialEgg >= 10 && !m_tutorial && !RouletteUtility.rouletteTurtorialEnd)
			{
				if (uISprite != null)
				{
					uISprite.gameObject.SetActive(false);
				}
				if (uISprite2 != null)
				{
					uISprite2.spriteName = RouletteUtility.GetRouletteChangeIconSpriteName(RouletteCategory.SPECIAL);
					uISprite2.gameObject.SetActive(true);
				}
			}
			else
			{
				if (uISprite != null)
				{
					uISprite.gameObject.SetActive(true);
					uISprite.spriteName = RouletteUtility.GetRouletteChangeIconSpriteName(category);
				}
				if (uISprite2 != null)
				{
					uISprite2.gameObject.SetActive(false);
				}
			}
		}
		else
		{
			button.functionName = "OnClickChangeBtn_" + idx;
			uISprite = GameObjectUtil.FindChildGameObjectComponent<UISprite>(button.gameObject, "img_btn_icon");
			uISprite2 = GameObjectUtil.FindChildGameObjectComponent<UISprite>(button.gameObject, "img_move_icon");
			if (uISprite != null)
			{
				uISprite.gameObject.SetActive(true);
				uISprite.spriteName = RouletteUtility.GetRouletteChangeIconSpriteName(category);
			}
			if (uISprite2 != null)
			{
				uISprite2.gameObject.SetActive(false);
			}
		}
	}

	public void RequestBasicInfo_Succeeded(List<RouletteCategory> rouletteList)
	{
		if (rouletteList == null || rouletteList.Count <= 0)
		{
			return;
		}
		m_rouletteList = rouletteList;
		if (m_buttons != null && m_buttons.Count > 0)
		{
			for (int i = 0; i < m_buttons.Count; i++)
			{
				if (rouletteList.Count > i)
				{
					m_buttons[i].gameObject.SetActive(true);
					bool enabled = m_requestCategory != m_rouletteList[i];
					if (m_isTopPage)
					{
						enabled = true;
					}
					UpdateChangeBottonIcon(m_rouletteList[i], m_buttons[i], i, enabled);
				}
				else
				{
					m_buttons[i].gameObject.SetActive(false);
				}
			}
		}
		SetTopPageObject();
		if (m_buttonsBase != null)
		{
			m_buttonsBase.SetActive(false);
		}
		if (m_buttonsBaseBg != null)
		{
			m_buttonsBaseBg.SetActive(false);
		}
	}

	public void RequestBasicInfo_Failed()
	{
	}

	private void SetTopPageObject()
	{
		if (!(m_topPageStorage != null))
		{
			return;
		}
		m_topPageStorage.maxItemCount = (m_topPageStorage.maxRows = m_rouletteList.Count);
		m_topPageStorage.Restart();
		m_topPageRouletteList = GameObjectUtil.FindChildGameObjects(m_topPageStorage.gameObject, "ui_rouletteTop_scroll(Clone)");
		int specialEgg = RouletteManager.Instance.specialEgg;
		if (m_rouletteInfoList != null)
		{
			m_rouletteInfoList.Clear();
		}
		if (RouletteInformationManager.Instance != null)
		{
			RouletteInformationManager.Instance.GetCurrentInfoParam(out m_rouletteInfoList);
		}
		if (m_topPageRouletteList == null)
		{
			return;
		}
		UIDraggablePanel uIDraggablePanel = GameObjectUtil.FindChildGameObjectComponent<UIDraggablePanel>(m_topPageObject, "ScrollView");
		if (uIDraggablePanel != null)
		{
			uIDraggablePanel.enabled = (!HudMenuUtility.IsNumPlayingRouletteTutorial() || !RouletteUtility.isTutorial);
		}
		for (int i = 0; i < m_topPageRouletteList.Count; i++)
		{
			UISprite uISprite = GameObjectUtil.FindChildGameObjectComponent<UISprite>(m_topPageRouletteList[i], "base");
			UILabel uILabel = GameObjectUtil.FindChildGameObjectComponent<UILabel>(m_topPageRouletteList[i], "Lbl_btn_roulette");
			UILabel uILabel2 = GameObjectUtil.FindChildGameObjectComponent<UILabel>(m_topPageRouletteList[i], "Lbl_btn_roulette_sh");
			UITexture uITexture = GameObjectUtil.FindChildGameObjectComponent<UITexture>(m_topPageRouletteList[i], "img_ad_tex");
			UIImageButton uIImageButton = GameObjectUtil.FindChildGameObjectComponent<UIImageButton>(m_topPageRouletteList[i], "Btn_information");
			RouletteCategory rouletteCategory = m_rouletteList[i];
			if (m_rouletteList[i] == RouletteCategory.PREMIUM && specialEgg >= 10 && !RouletteUtility.isTutorial)
			{
				rouletteCategory = RouletteCategory.SPECIAL;
			}
			ServerWheelOptionsData rouletteDataOrg = RouletteManager.Instance.GetRouletteDataOrg(rouletteCategory);
			if (rouletteDataOrg != null && m_isTopPage)
			{
				rouletteDataOrg.ChangeMulti(1);
			}
			if (uISprite != null)
			{
				uISprite.color = GetBtnColor(rouletteCategory);
			}
			if (uILabel != null && uILabel2 != null)
			{
				string text2 = uILabel.text = (uILabel2.text = RouletteUtility.GetRouletteCategoryHeaderText(rouletteCategory));
			}
			if (rouletteCategory == RouletteCategory.PREMIUM || rouletteCategory == RouletteCategory.SPECIAL)
			{
				m_premiumRouletteLabel = uILabel;
				m_premiumRouletteShLabel = uILabel2;
			}
			if (rouletteCategory != RouletteCategory.ITEM)
			{
				RouletteCategory rouletteCategory2 = RouletteCategory.NONE;
				switch (rouletteCategory)
				{
				case RouletteCategory.PREMIUM:
				case RouletteCategory.SPECIAL:
					if (m_rouletteInfoList.ContainsKey(RouletteCategory.PREMIUM))
					{
						rouletteCategory2 = RouletteCategory.PREMIUM;
					}
					break;
				case RouletteCategory.RAID:
					if (m_rouletteInfoList.ContainsKey(RouletteCategory.RAID))
					{
						rouletteCategory2 = RouletteCategory.RAID;
					}
					break;
				}
				if (rouletteCategory2 != 0)
				{
					if (uITexture != null && RouletteInformationManager.Instance != null)
					{
						RouletteInformationManager.InfoBannerRequest bannerRequest = new RouletteInformationManager.InfoBannerRequest(uITexture);
						RouletteInformationManager.Instance.LoadInfoBaner(bannerRequest, rouletteCategory2);
					}
					GeneralUtil.SetButtonFunc(m_topPageRouletteList[i], "Btn_information", base.gameObject, "OnClickInfoBtn_" + rouletteCategory2);
					if (uIImageButton != null)
					{
						uIImageButton.isEnabled = true;
					}
				}
				else
				{
					GeneralUtil.SetButtonFunc(m_topPageRouletteList[i], "Btn_information", base.gameObject, "OnClickInfoBtn_" + RouletteCategory.ITEM);
					if (uIImageButton != null)
					{
						uIImageButton.isEnabled = false;
					}
				}
			}
			else
			{
				if (uITexture != null && m_itemRouletteDefaultTexture != null)
				{
					uITexture.mainTexture = m_itemRouletteDefaultTexture;
				}
				GeneralUtil.SetButtonFunc(m_topPageRouletteList[i], "Btn_information", base.gameObject, "OnClickDummy");
				if (uIImageButton != null)
				{
					uIImageButton.isEnabled = false;
				}
			}
			GeneralUtil.SetButtonFunc(m_topPageRouletteList[i], "Btn_all_item", base.gameObject, "OnClickOddsBtn_" + i);
			GeneralUtil.SetButtonFunc(m_topPageRouletteList[i], "Btn_roulette", base.gameObject, "OnClickChangeBtn_" + i);
		}
	}

	private void SetTopPageHeaderObject()
	{
		if (m_topPageHeaderList == null || m_topPageHeaderList.Count <= 0 || !(RouletteManager.Instance != null))
		{
			return;
		}
		List<ServerItem.Id> rouletteCostItemIdList = RouletteManager.Instance.rouletteCostItemIdList;
		Dictionary<ServerItem.Id, string> dictionary = new Dictionary<ServerItem.Id, string>();
		dictionary.Add(ServerItem.Id.ROULLETE_TICKET_ITEM, "ui_roulette_ticket_2");
		dictionary.Add(ServerItem.Id.ROULLETE_TICKET_PREMIAM, "ui_roulette_ticket_1");
		dictionary.Add(ServerItem.Id.SPECIAL_EGG, "ui_result_special_egg");
		dictionary.Add(ServerItem.Id.RAIDRING, "ui_event_ring_icon");
		int num = 88;
		int num2 = 108;
		float num3 = Mathf.Sqrt(num * num2);
		for (int i = 0; i < m_topPageHeaderList.Count; i++)
		{
			GameObject gameObject = m_topPageHeaderList[i];
			if (gameObject != null && rouletteCostItemIdList.Count > i)
			{
				ServerItem.Id id = rouletteCostItemIdList[i];
				long itemCount = GeneralUtil.GetItemCount(id);
				if (id == ServerItem.Id.SPECIAL_EGG && m_premiumRouletteLabel != null && m_premiumRouletteShLabel != null)
				{
					if (itemCount >= 10 && !RouletteUtility.isTutorial)
					{
						UILabel premiumRouletteLabel = m_premiumRouletteLabel;
						string rouletteCategoryHeaderText = RouletteUtility.GetRouletteCategoryHeaderText(RouletteCategory.SPECIAL);
						m_premiumRouletteShLabel.text = rouletteCategoryHeaderText;
						premiumRouletteLabel.text = rouletteCategoryHeaderText;
					}
					else
					{
						UILabel premiumRouletteLabel2 = m_premiumRouletteLabel;
						string rouletteCategoryHeaderText = RouletteUtility.GetRouletteCategoryHeaderText(RouletteCategory.PREMIUM);
						m_premiumRouletteShLabel.text = rouletteCategoryHeaderText;
						premiumRouletteLabel2.text = rouletteCategoryHeaderText;
					}
				}
				UISprite uISprite = GameObjectUtil.FindChildGameObjectComponent<UISprite>(gameObject, "img_icon");
				UILabel uILabel = GameObjectUtil.FindChildGameObjectComponent<UILabel>(gameObject, "Lbl_num");
				if (uISprite != null)
				{
					if (dictionary.ContainsKey(id))
					{
						uISprite.spriteName = dictionary[id];
						UISpriteData atlasSprite = uISprite.GetAtlasSprite();
						if (atlasSprite != null)
						{
							int width = atlasSprite.width;
							int height = atlasSprite.height;
							float num4 = Mathf.Sqrt(width * height);
							float num5 = num3 / num4;
							uISprite.width = (int)((float)width * num5);
							uISprite.height = (int)((float)height * num5);
						}
					}
					else
					{
						uISprite.spriteName = string.Empty;
					}
				}
				if (uILabel != null)
				{
					uILabel.text = HudUtility.GetFormatNumString(itemCount);
				}
				gameObject.SetActive(true);
			}
			else
			{
				gameObject.SetActive(false);
			}
		}
	}

	public void RequestGetRoulette_Succeeded(ServerWheelOptionsData wheelData)
	{
		m_tutorial = false;
		if (m_topPageOddsSelect == RouletteCategory.NONE)
		{
			if (wheelData != null)
			{
				if (RouletteUtility.isTutorial && wheelData.category == RouletteCategory.PREMIUM)
				{
					m_tutorial = true;
					GeneralWindow.CInfo info = default(GeneralWindow.CInfo);
					info.name = "RouletteTutorial";
					info.buttonType = GeneralWindow.ButtonType.Ok;
					info.caption = TextManager.GetText(TextManager.TextType.TEXTTYPE_COMMON_TEXT, "MainMenu", "roulette_move_explan_caption").text;
					info.message = TextManager.GetText(TextManager.TextType.TEXTTYPE_COMMON_TEXT, "MainMenu", "roulette_move_explan").text;
					GeneralWindow.Create(info);
					string[] value = new string[1];
					SendApollo.GetTutorialValue(ApolloTutorialIndex.START_STEP6, ref value);
					m_sendApollo = SendApollo.CreateRequest(ApolloType.TUTORIAL_START, value);
				}
				if (m_updateRequest)
				{
					SetDelayTime(0.25f);
					UpdateWheelData(wheelData);
					m_updateRequest = false;
				}
				else
				{
					SetupWheelData(wheelData);
				}
			}
		}
		else if (wheelData.category == RouletteCategory.ITEM)
		{
			RouletteManager.Instance.RequestRoulettePrizeOrg(m_topPageOddsSelect, base.gameObject);
		}
		else
		{
			m_topPageOddsSelect = wheelData.category;
			m_topPageWheelData = true;
		}
	}

	public void RequestCommitRoulette_Succeeded(ServerWheelOptionsData wheelData)
	{
		if (wheelData == null)
		{
			return;
		}
		if (m_tutorial)
		{
			m_addSpecialEgg = true;
			ServerInterface loggedInServerInterface = ServerInterface.LoggedInServerInterface;
			if (loggedInServerInterface != null)
			{
				loggedInServerInterface.RequestServerAddSpecialEgg(10, base.gameObject);
			}
		}
		RouletteManager instance = RouletteManager.Instance;
		m_wheelDataAfter = wheelData;
		if (instance != null)
		{
			m_spinResultGeneral = instance.GetResult();
			if (m_spinResultGeneral != null)
			{
				m_spinResult = null;
				OnRouletteSpinDecision(m_spinResultGeneral.ItemWon);
			}
			else
			{
				m_spinResult = instance.GetResultChao();
				OnRouletteSpinDecision(m_spinResult.ItemWon);
			}
		}
	}

	private void OnClickBack()
	{
		if (m_isTopPage)
		{
			if (!m_spin && m_closeTime <= 0f)
			{
				HudMenuUtility.SendMenuButtonClicked(ButtonInfoTable.ButtonType.ROULETTE_BACK);
			}
		}
		else
		{
			SoundManager.SePlay("sys_window_close");
			SetupTopPage(false);
		}
	}

	public void OnClickOddsBtn_0()
	{
		OnClickOddsBtn(0);
	}

	public void OnClickOddsBtn_1()
	{
		OnClickOddsBtn(1);
	}

	public void OnClickOddsBtn_2()
	{
		OnClickOddsBtn(2);
	}

	public void OnClickOddsBtn_3()
	{
		OnClickOddsBtn(3);
	}

	public void OnClickOddsBtn_4()
	{
		OnClickOddsBtn(4);
	}

	public void OnClickOddsBtn_5()
	{
		OnClickOddsBtn(5);
	}

	public void OnClickOddsBtn_6()
	{
		OnClickOddsBtn(6);
	}

	public void OnClickOddsBtn_7()
	{
		OnClickOddsBtn(7);
	}

	public void OnClickOddsBtn_8()
	{
		OnClickOddsBtn(8);
	}

	public void OnClickOddsBtn_9()
	{
		OnClickOddsBtn(9);
	}

	private void OnClickOddsBtn(int index)
	{
		Debug.Log("OnClickOddsBtn  " + index);
		if (RouletteManager.Instance != null && m_rouletteList != null && m_rouletteList.Count > index)
		{
			RouletteCategory rouletteCategory = m_rouletteList[index];
			if (rouletteCategory == RouletteCategory.SPECIAL && RouletteUtility.isTutorial)
			{
				rouletteCategory = RouletteCategory.PREMIUM;
			}
			m_topPageOddsSelect = rouletteCategory;
			m_topPageWheelData = false;
			ServerWheelOptionsData rouletteDataOrg = RouletteManager.Instance.GetRouletteDataOrg(rouletteCategory);
			Debug.Log(string.Concat("OnClickOddsBtn data:", rouletteDataOrg != null, " categ:", rouletteCategory, " !!!!!!"));
			if (rouletteDataOrg != null)
			{
				RouletteManager.Instance.RequestRoulettePrizeOrg(m_topPageOddsSelect, base.gameObject);
				return;
			}
			if (GeneralUtil.IsNetwork())
			{
				RouletteManager.Instance.RequestRouletteOrg(m_topPageOddsSelect, base.gameObject);
				return;
			}
			m_topPageOddsSelect = RouletteCategory.NONE;
			m_topPageWheelData = false;
			GeneralUtil.ShowNoCommunication();
		}
	}

	private void RequestRoulettePrize_Succeeded(ServerPrizeState prize)
	{
		ServerWheelOptionsData rouletteDataOrg = RouletteManager.Instance.GetRouletteDataOrg(m_topPageOddsSelect);
		OpenOdds(prize, rouletteDataOrg);
		m_topPageOddsSelect = RouletteCategory.NONE;
	}

	public void OnClickCurrentRouletteBanner()
	{
		RouletteCategory rouletteCategory = m_wheelData.category;
		if (rouletteCategory == RouletteCategory.SPECIAL)
		{
			rouletteCategory = RouletteCategory.PREMIUM;
		}
		OnClickInfoBtn(rouletteCategory);
	}

	public void OnClickInfoBtn_ITEM()
	{
		OnClickInfoBtn(RouletteCategory.ITEM);
		Debug.Log("OnClickInfoBtn_ITEM  !!!!!!!!!");
	}

	public void OnClickInfoBtn_PREMIUM()
	{
		OnClickInfoBtn(RouletteCategory.PREMIUM);
	}

	public void OnClickInfoBtn_SPECIAL()
	{
		OnClickInfoBtn(RouletteCategory.PREMIUM);
	}

	public void OnClickInfoBtn_RAID()
	{
		OnClickInfoBtn(RouletteCategory.RAID);
	}

	public void OnClickInfoBtn_EVENT()
	{
		OnClickInfoBtn(RouletteCategory.EVENT);
	}

	private void OnClickInfoBtn(RouletteCategory category)
	{
		if (m_rouletteInfoList != null && m_rouletteInfoList.ContainsKey(category))
		{
			RouletteInformationUtility.ShowNewsWindow(m_rouletteInfoList[category]);
		}
		Debug.Log("OnClickInfoBtn  " + category);
	}

	public void OnClickChangeBtn_0()
	{
		OnClickChangeBtn(0);
	}

	public void OnClickChangeBtn_1()
	{
		OnClickChangeBtn(1);
	}

	public void OnClickChangeBtn_2()
	{
		OnClickChangeBtn(2);
	}

	public void OnClickChangeBtn_3()
	{
		OnClickChangeBtn(3);
	}

	public void OnClickChangeBtn_4()
	{
		OnClickChangeBtn(4);
	}

	public void OnClickChangeBtn_5()
	{
		OnClickChangeBtn(5);
	}

	public void OnClickChangeBtn_6()
	{
		OnClickChangeBtn(6);
	}

	public void OnClickChangeBtn_7()
	{
		OnClickChangeBtn(7);
	}

	public void OnClickChangeBtn_8()
	{
		OnClickChangeBtn(8);
	}

	public void OnClickChangeBtn_9()
	{
		OnClickChangeBtn(9);
	}

	private void OnClickChangeBtn(int index)
	{
		if (GeneralUtil.IsNetwork())
		{
			if (m_rouletteList == null || m_rouletteList.Count <= index)
			{
				return;
			}
			if (m_rouletteList[index] == RouletteCategory.RAID)
			{
				if (EventManager.Instance != null)
				{
					EventManager.EventType typeInTime = EventManager.Instance.TypeInTime;
					if (typeInTime != EventManager.EventType.RAID_BOSS)
					{
						m_rouletteList.Remove(RouletteCategory.RAID);
						GeneralUtil.ShowEventEnd();
						if (m_requestCategory == RouletteCategory.RAID)
						{
							Setup(m_rouletteList[0]);
						}
						else
						{
							ResetChangeBotton();
						}
					}
					else
					{
						Setup(m_rouletteList[index]);
					}
				}
				else
				{
					Setup(m_rouletteList[index]);
				}
			}
			else
			{
				Setup(m_rouletteList[index]);
			}
		}
		else
		{
			GeneralUtil.ShowNoCommunication();
		}
	}

	private void AnimationFinishCallback()
	{
		if (m_close)
		{
			Remove();
		}
		else if (m_change && m_wheelDataAfter != null)
		{
			UpdateWheelData(m_wheelDataAfter, false);
			m_wheelDataAfter = null;
			m_change = false;
			if (m_animation != null)
			{
				ActiveAnimation activeAnimation = ActiveAnimation.Play(m_animation, "ui_mm_roulette_intro_Anim", Direction.Forward);
				EventDelegate.Add(activeAnimation.onFinished, AnimationFinishCallback, true);
				m_wheelData.PlaySe(ServerWheelOptionsData.SE_TYPE.Open, 0f);
			}
		}
	}

	private void Awake()
	{
		SetInstance();
		if (s_instance != null)
		{
			SetPanelsAlpha(0f);
		}
	}

	private void OnDestroy()
	{
		if (s_instance == this)
		{
			RemoveBackKeyCallBack();
			s_instance = null;
		}
	}

	private void SetInstance()
	{
		if (s_instance == null)
		{
			EntryBackKeyCallBack();
			s_instance = this;
		}
		else
		{
			UnityEngine.Object.Destroy(base.gameObject);
		}
	}

	private void EntryBackKeyCallBack()
	{
		BackKeyManager.AddWindowCallBack(base.gameObject);
	}

	private void RemoveBackKeyCallBack()
	{
		BackKeyManager.RemoveWindowCallBack(base.gameObject);
	}

	public void OnClickPlatformBackButton(WindowBase.BackButtonMessage msg)
	{
		if (!base.gameObject.activeSelf)
		{
			return;
		}
		bool flag = false;
		if (m_panels != null && m_parts != null && m_parts.Count > 0)
		{
			foreach (UIPanel panel in m_panels)
			{
				if (panel.alpha > 0.1f)
				{
					flag = true;
					break;
				}
			}
		}
		if (!flag)
		{
			return;
		}
		if (msg != null)
		{
			msg.StaySequence();
		}
		if (m_spin || !m_wheelSetup || GeneralWindow.Created)
		{
			return;
		}
		ItemGetWindow itemGetWindow = ItemGetWindowUtil.GetItemGetWindow();
		if (((itemGetWindow != null && itemGetWindow.IsEnd) || itemGetWindow == null) && !GeneralWindow.Created && !EventBestChaoWindow.Created && !m_isWindow && !m_tutorial)
		{
			if (m_isTopPage)
			{
				HudMenuUtility.SendMenuButtonClicked(ButtonInfoTable.ButtonType.ROULETTE_BACK);
				return;
			}
			SetupTopPage(false);
			SoundManager.SePlay("sys_window_close");
		}
	}

	private void UpdateDebug()
	{
	}
}
