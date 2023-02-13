using AnimationOrTween;
using System.Collections;
using Text;
using UnityEngine;

public class DailyBattleRewardWindow : WindowBase
{
	private const float OPEN_EFFECT_START = 0.5f;

	private const float OPEN_EFFECT_TIME = 2f;

	private static bool s_isActive;

	[SerializeField]
	private Animation m_animation;

	[SerializeField]
	private UIPanel m_mainPanel;

	private bool m_isClickClose;

	private bool m_isEnd;

	private ServerDailyBattleDataPair m_battleData;

	public static bool isActive
	{
		get
		{
			return s_isActive;
		}
	}

	public bool IsEnd
	{
		get
		{
			return m_isEnd;
		}
	}

	private void Start()
	{
	}

	private void OnDestroy()
	{
		Destroy();
	}

	private void Update()
	{
	}

	private IEnumerator SetupObject()
	{
		yield return null;
		if (m_battleData != null)
		{
			GameObject loseWindow = GameObjectUtil.FindChildGameObject(base.gameObject, "lose");
			GameObject winWindow = GameObjectUtil.FindChildGameObject(base.gameObject, "win");
			if (winWindow != null && loseWindow != null)
			{
				switch (m_battleData.winFlag)
				{
				case 0:
				case 1:
				{
					winWindow.SetActive(false);
					loseWindow.SetActive(true);
					string loseText = TextUtility.GetCommonText("DailyMission", "battle_vsreward_text3");
					UILabel loseLabel = GameObjectUtil.FindChildGameObjectComponent<UILabel>(loseWindow, "Lbl_daily_battle_lose");
					if (loseLabel != null)
					{
						loseLabel.text = loseText;
					}
					UILabel loseLabel_sh = GameObjectUtil.FindChildGameObjectComponent<UILabel>(loseWindow, "Lbl_daily_battle_lose_sh");
					if (loseLabel_sh != null)
					{
						loseLabel_sh.text = loseText;
					}
					break;
				}
				case 2:
				case 3:
				case 4:
				{
					winWindow.SetActive(true);
					loseWindow.SetActive(false);
					int winCount = m_battleData.goOnWin;
					TextObject textObject;
					if (winCount < 2)
					{
						textObject = TextManager.GetText(TextManager.TextType.TEXTTYPE_COMMON_TEXT, "DailyMission", "battle_vsreward_text1");
					}
					else
					{
						textObject = TextManager.GetText(TextManager.TextType.TEXTTYPE_COMMON_TEXT, "DailyMission", "battle_vsreward_text2");
						textObject.ReplaceTag("{PARAM_WIN}", winCount.ToString());
					}
					UILabel winLabel = GameObjectUtil.FindChildGameObjectComponent<UILabel>(winWindow, "Lbl_daily_battle_win");
					if (winLabel != null)
					{
						winLabel.text = textObject.text;
					}
					UILabel winLabel_sh = GameObjectUtil.FindChildGameObjectComponent<UILabel>(winWindow, "Lbl_daily_battle_win_sh");
					if (winLabel_sh != null)
					{
						winLabel_sh.text = textObject.text;
					}
					break;
				}
				}
			}
		}
		if (m_mainPanel != null)
		{
			m_mainPanel.alpha = 1f;
		}
		GameObject window = GameObjectUtil.FindChildGameObject(base.gameObject, "ranking_window");
		if (window != null)
		{
			window.SetActive(true);
		}
		if (m_animation != null)
		{
			ActiveAnimation.Play(m_animation, "ui_cmn_window_Anim", Direction.Forward);
		}
		UIPlayAnimation btnClose = GameObjectUtil.FindChildGameObjectComponent<UIPlayAnimation>(base.gameObject, "Btn_close");
		if (btnClose != null && !EventDelegate.IsValid(btnClose.onFinished))
		{
			EventDelegate.Add(btnClose.onFinished, OnFinished, true);
		}
		UIPlayAnimation blinder = GameObjectUtil.FindChildGameObjectComponent<UIPlayAnimation>(base.gameObject, "blinder");
		if (blinder != null && !EventDelegate.IsValid(blinder.onFinished))
		{
			EventDelegate.Add(blinder.onFinished, OnFinished, true);
		}
		m_isEnd = false;
		m_isClickClose = false;
		if (m_battleData != null && m_battleData.winFlag >= 2)
		{
			SoundManager.SePlay("sys_league_up");
		}
		else
		{
			SoundManager.SePlay("sys_league_down");
		}
	}

	private void Setup(ServerDailyBattleDataPair data)
	{
		s_isActive = true;
		base.gameObject.SetActive(true);
		m_battleData = data;
		m_isEnd = false;
		m_isClickClose = false;
		base.enabled = true;
		StartCoroutine(SetupObject());
	}

	public static DailyBattleRewardWindow Open(ServerDailyBattleDataPair data)
	{
		DailyBattleRewardWindow dailyBattleRewardWindow = null;
		GameObject menuAnimUIObject = HudMenuUtility.GetMenuAnimUIObject();
		if (menuAnimUIObject != null)
		{
			dailyBattleRewardWindow = GameObjectUtil.FindChildGameObjectComponent<DailyBattleRewardWindow>(menuAnimUIObject, "DailybattleRewardWindowUI");
			if (dailyBattleRewardWindow == null)
			{
				GameObject gameObject = GameObjectUtil.FindChildGameObject(menuAnimUIObject, "DailybattleRewardWindowUI");
				if (gameObject != null)
				{
					dailyBattleRewardWindow = gameObject.AddComponent<DailyBattleRewardWindow>();
				}
			}
			if (dailyBattleRewardWindow != null)
			{
				dailyBattleRewardWindow.Setup(data);
			}
		}
		return dailyBattleRewardWindow;
	}

	public void OnFinished()
	{
		s_isActive = false;
		m_isEnd = true;
		if (m_mainPanel != null)
		{
			m_mainPanel.alpha = 0f;
		}
		base.gameObject.SetActive(false);
		base.enabled = false;
		SoundManager.SePlay("sys_window_close");
	}

	public override void OnClickPlatformBackButton(BackButtonMessage msg)
	{
		if (m_isEnd)
		{
			return;
		}
		if (msg != null)
		{
			msg.StaySequence();
		}
		if (!ranking_window.isActive && !m_isClickClose)
		{
			m_isClickClose = true;
			ActiveAnimation activeAnimation = ActiveAnimation.Play(m_animation, "ui_cmn_window_Anim", Direction.Reverse);
			if (activeAnimation != null)
			{
				EventDelegate.Add(activeAnimation.onFinished, OnFinished, true);
			}
		}
	}
}
