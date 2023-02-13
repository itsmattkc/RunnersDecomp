using AnimationOrTween;
using Text;
using UnityEngine;

public class DailyInfo : MonoBehaviour
{
	[SerializeField]
	private Animation m_animation;

	[SerializeField]
	private UIPanel m_mainPanel;

	[SerializeField]
	private GameObject m_battleObject;

	[SerializeField]
	private GameObject m_historyObject;

	private bool m_isClickClose;

	private bool m_isEnd;

	private bool m_isHistory;

	private bool m_isToggleLock;

	private void Start()
	{
	}

	public static bool Open()
	{
		bool result = false;
		GameObject menuAnimUIObject = HudMenuUtility.GetMenuAnimUIObject();
		if (menuAnimUIObject != null)
		{
			DailyInfo dailyInfo = GameObjectUtil.FindChildGameObjectComponent<DailyInfo>(menuAnimUIObject, "DailyInfoUI");
			if (dailyInfo != null)
			{
				dailyInfo.Setup();
				result = true;
			}
		}
		return result;
	}

	private void Setup()
	{
		if (m_mainPanel != null)
		{
			m_mainPanel.alpha = 0.1f;
		}
		m_isEnd = false;
		m_isClickClose = false;
		m_isHistory = false;
		if (m_animation != null)
		{
			ActiveAnimation.Play(m_animation, "ui_daily_challenge_infomation_intro_Anim", Direction.Forward);
		}
		GeneralUtil.SetButtonFunc(base.gameObject, "Btn_today", base.gameObject, "OnClickToggleToday");
		GeneralUtil.SetButtonFunc(base.gameObject, "Btn_history", base.gameObject, "OnClickToggleHistory");
		GeneralUtil.SetButtonFunc(base.gameObject, "Btn_help", base.gameObject, "OnClickHelp");
		SetupToggleBtn();
		ChangeInfo();
		base.gameObject.SetActive(true);
	}

	private void SetupToggleBtn()
	{
		m_isToggleLock = true;
		UIToggle uIToggle = null;
		if (m_isHistory)
		{
			uIToggle = GameObjectUtil.FindChildGameObjectComponent<UIToggle>(base.gameObject, "Btn_history");
			UIToggle uIToggle2 = GameObjectUtil.FindChildGameObjectComponent<UIToggle>(base.gameObject, "Btn_today");
			if (uIToggle2 != null)
			{
				uIToggle2.startsActive = false;
			}
		}
		else
		{
			uIToggle = GameObjectUtil.FindChildGameObjectComponent<UIToggle>(base.gameObject, "Btn_today");
			UIToggle uIToggle3 = GameObjectUtil.FindChildGameObjectComponent<UIToggle>(base.gameObject, "Btn_history");
			if (uIToggle3 != null)
			{
				uIToggle3.startsActive = false;
			}
		}
		uIToggle.startsActive = true;
		if (uIToggle != null)
		{
			uIToggle.SendMessage("Start");
		}
		m_isToggleLock = false;
	}

	private void OnClickBack()
	{
		m_isClickClose = true;
		SoundManager.SePlay("sys_menu_decide");
	}

	private void OnClickToggleToday()
	{
		if (!m_isToggleLock && m_isHistory)
		{
			if (GeneralUtil.IsNetwork())
			{
				m_isHistory = false;
				ChangeInfo();
			}
			else
			{
				SetupToggleBtn();
				GeneralUtil.ShowNoCommunication("ShowNoCommunicationDailyInfo");
			}
			SoundManager.SePlay("sys_menu_decide");
		}
	}

	private void OnClickToggleHistory()
	{
		if (!m_isToggleLock && !m_isHistory)
		{
			if (GeneralUtil.IsNetwork())
			{
				m_isHistory = true;
				ChangeInfo();
			}
			else
			{
				SetupToggleBtn();
				GeneralUtil.ShowNoCommunication("ShowNoCommunicationDailyInfo");
			}
			SoundManager.SePlay("sys_menu_decide");
		}
	}

	private void OnClickHelp()
	{
		GeneralWindow.CInfo info = default(GeneralWindow.CInfo);
		info.name = "ShowDailyBattleHelp";
		info.buttonType = GeneralWindow.ButtonType.Ok;
		info.caption = TextUtility.GetText(TextManager.TextType.TEXTTYPE_COMMON_TEXT, "DailyMission", "battle_help_caption");
		info.message = TextUtility.GetText(TextManager.TextType.TEXTTYPE_COMMON_TEXT, "DailyMission", "battle_help_text");
		GeneralWindow.Create(info);
		SoundManager.SePlay("sys_menu_decide");
	}

	private void ChangeInfo()
	{
		if (!m_isHistory)
		{
			SetupMainObject();
		}
		else
		{
			SetupHistoryObject();
		}
	}

	private void SetupMainObject()
	{
		if (m_battleObject == null)
		{
			m_battleObject = GameObjectUtil.FindChildGameObject(base.gameObject, "battle_set");
		}
		if (m_historyObject == null)
		{
			m_historyObject = GameObjectUtil.FindChildGameObject(base.gameObject, "history_set");
		}
		if (m_battleObject != null)
		{
			m_battleObject.SetActive(true);
			DailyInfoBattle dailyInfoBattle = GameObjectUtil.FindChildGameObjectComponent<DailyInfoBattle>(m_battleObject, "battle_set");
			if (dailyInfoBattle == null)
			{
				dailyInfoBattle = m_battleObject.AddComponent<DailyInfoBattle>();
			}
			if (dailyInfoBattle != null)
			{
				dailyInfoBattle.Setup(this);
			}
		}
		if (m_historyObject != null)
		{
			m_historyObject.SetActive(false);
		}
	}

	private void SetupHistoryObject()
	{
		if (m_battleObject == null)
		{
			m_battleObject = GameObjectUtil.FindChildGameObject(base.gameObject, "battle_set");
		}
		if (m_historyObject == null)
		{
			m_historyObject = GameObjectUtil.FindChildGameObject(base.gameObject, "history_set");
		}
		if (m_battleObject != null)
		{
			m_battleObject.SetActive(false);
		}
		if (m_historyObject != null)
		{
			m_historyObject.SetActive(true);
			DailyInfoHistory dailyInfoHistory = GameObjectUtil.FindChildGameObjectComponent<DailyInfoHistory>(m_historyObject, "history_set");
			if (dailyInfoHistory == null)
			{
				dailyInfoHistory = m_historyObject.AddComponent<DailyInfoHistory>();
			}
			if (dailyInfoHistory != null)
			{
				dailyInfoHistory.Setup(this);
			}
		}
	}

	public void OnClosedWindowAnim()
	{
		m_isEnd = true;
		m_isHistory = false;
		if (m_mainPanel != null)
		{
			m_mainPanel.alpha = 0f;
		}
		ChangeInfo();
		base.gameObject.SetActive(false);
	}

	public void OnClickBackButton()
	{
		if (!m_isEnd && !m_isClickClose)
		{
			m_isClickClose = true;
			ActiveAnimation activeAnimation = ActiveAnimation.Play(m_animation, "ui_daily_challenge_infomation_intro_Anim", Direction.Reverse);
			if (activeAnimation != null)
			{
				EventDelegate.Add(activeAnimation.onFinished, OnClosedWindowAnim, true);
			}
		}
	}
}
