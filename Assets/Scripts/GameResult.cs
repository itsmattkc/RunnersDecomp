using AnimationOrTween;
using App.Utility;
using SaveData;
using Text;
using UnityEngine;

public class GameResult : MonoBehaviour
{
	public enum ResultType
	{
		NONE = -1,
		NORMAL,
		BOSS
	}

	private enum EventSignal
	{
		SIG_BG_START_IN_ANIM = 100,
		SIG_END_BG_IN_ANIM,
		SIG_END_SCORE_IN_ANIM,
		SIG_NEXT_BUTTON_PRESSED,
		SIG_DETAILS_BUTTON_PRESSED,
		SIG_DETAILS_END_BUTTON_PRESSED,
		SIG_END_SCORE_OUT_ANIM,
		SIG_END_OUT_ANIM
	}

	private enum Status
	{
		PRESS_NEXT,
		END_OUT_ANIM
	}

	private TinyFsmBehavior m_fsm;

	private GameObject m_AnimationObject;

	private GameObject m_resultRoot;

	private GameObject m_eventRoot;

	private UIImageButton m_imageButtonNext;

	private UIImageButton m_imageButtonDetails;

	private GameResultScores m_scores;

	private bool m_isNomiss;

	private bool m_isBossTutorialClear;

	private bool m_isReplay;

	private bool m_isBossDestroyed;

	private ResultType m_resultType;

	private bool m_isScoreStart;

	private bool m_eventTimeup;

	private EventResultState m_eventResultState;

	private HudEventResult m_eventResult;

	private Bitset32 m_status;

	public bool IsPressNext
	{
		get
		{
			return m_status.Test(0);
		}
		private set
		{
			m_status.Set(0, value);
		}
	}

	public bool IsEndOutAnimation
	{
		get
		{
			return m_status.Test(1);
		}
		private set
		{
			m_status.Set(1, value);
		}
	}

	public void PlayBGStart(ResultType resultType, bool isNoMiss, bool isBossTutorialClear, bool isBossDestroy, EventResultState eventResultState)
	{
		m_isBossDestroyed = isBossDestroy;
		SetupResultType(resultType, isNoMiss, isBossTutorialClear, m_isBossDestroyed);
		m_resultType = resultType;
		m_eventResultState = eventResultState;
		IsPressNext = false;
		IsEndOutAnimation = false;
		GameResultUtility.SetRaidbossBeatBonus(0);
		Debug.Log("GameResult:PlayBGStart >>> eventResultState=" + eventResultState);
		m_eventTimeup = false;
		switch (m_eventResultState)
		{
		case EventResultState.TIMEUP:
			m_eventTimeup = true;
			break;
		case EventResultState.TIMEUP_RESULT:
			m_eventTimeup = true;
			break;
		default:
			m_eventTimeup = false;
			break;
		}
		TinyFsmEvent signal = TinyFsmEvent.CreateUserEvent(100);
		if (m_fsm != null)
		{
			m_fsm.Dispatch(signal);
		}
	}

	public void PlayScoreStart()
	{
		m_isScoreStart = true;
	}

	public void SetRaidbossBeatBonus(int value)
	{
		GameResultUtility.SetRaidbossBeatBonus(value);
	}

	private void Start()
	{
		m_fsm = (base.gameObject.AddComponent(typeof(TinyFsmBehavior)) as TinyFsmBehavior);
		if (!(m_fsm == null))
		{
			TinyFsmBehavior.Description description = new TinyFsmBehavior.Description(this);
			description.initState = new TinyFsmState(StateIdle);
			description.onFixedUpdate = true;
			m_fsm.SetUp(description);
			GameObject gameObject = base.transform.Find("result_Anim").gameObject;
			GameObject gameObject2 = base.transform.Find("result_boss_Anim").gameObject;
			if (gameObject != null)
			{
				gameObject.SetActive(false);
			}
			if (gameObject2 != null)
			{
				gameObject2.SetActive(false);
			}
			GameResultUtility.SaveOldBestScore();
			m_isNomiss = false;
			m_isBossTutorialClear = false;
		}
	}

	private void Update()
	{
	}

	private void SetupHudResultWindow(bool isCampaignBonus)
	{
		string windowName = "HudResultWindow";
		string windowName2 = "HudResultWindow2";
		if (isCampaignBonus)
		{
			windowName = "HudResultWindow2";
			windowName2 = "HudResultWindow";
		}
		HudResultWindow hudResultWindow = GetHudResultWindow(windowName);
		if (hudResultWindow != null)
		{
			hudResultWindow.Setup(base.gameObject, m_resultType == ResultType.BOSS);
			hudResultWindow.gameObject.SetActive(false);
		}
		HudResultWindow hudResultWindow2 = GetHudResultWindow(windowName2);
		if (hudResultWindow2 != null)
		{
			hudResultWindow2.gameObject.SetActive(false);
		}
	}

	private HudResultWindow GetHudResultWindow(string windowName)
	{
		GameObject gameObject = base.transform.Find(windowName).gameObject;
		if (gameObject != null)
		{
			return gameObject.GetComponent<HudResultWindow>();
		}
		return null;
	}

	private TinyFsmState StateIdle(TinyFsmEvent e)
	{
		switch (e.Signal)
		{
		case -3:
			return TinyFsmState.End();
		case -4:
			return TinyFsmState.End();
		case 0:
			return TinyFsmState.End();
		case 100:
			m_fsm.ChangeState(new TinyFsmState(StateInAnimation));
			return TinyFsmState.End();
		default:
			return TinyFsmState.End();
		}
	}

	private TinyFsmState StateInAnimation(TinyFsmEvent e)
	{
		switch (e.Signal)
		{
		case -3:
		{
			m_AnimationObject.SetActive(true);
			bool isCampaignBonus = false;
			if (m_scores != null)
			{
				m_scores.Setup(base.gameObject, m_resultRoot, m_eventRoot);
				isCampaignBonus = m_scores.IsCampaignBonus();
			}
			SetupHudResultWindow(isCampaignBonus);
			if (m_isReplay)
			{
				return TinyFsmState.End();
			}
			Animation anim = GameResultUtility.SearchAnimation(m_AnimationObject);
			string clipName = string.Empty;
			switch (m_resultType)
			{
			case ResultType.NORMAL:
				clipName = "ui_result_intro_bg_Anim";
				break;
			case ResultType.BOSS:
				clipName = "ui_result_boss_intro_bg_Anim";
				break;
			}
			ActiveAnimation activeAnimation = ActiveAnimation.Play(anim, clipName, Direction.Forward);
			EventDelegate.Add(activeAnimation.onFinished, InAnimationEndCallback, true);
			BoxCollider boxCollider = GameObjectUtil.FindChildGameObjectComponent<BoxCollider>(m_resultRoot, "Btn_next");
			if (boxCollider != null)
			{
				boxCollider.isTrigger = false;
			}
			BoxCollider boxCollider2 = GameObjectUtil.FindChildGameObjectComponent<BoxCollider>(m_resultRoot, "Btn_details");
			if (boxCollider2 != null)
			{
				boxCollider2.isTrigger = false;
			}
			SoundManager.SePlay("sys_window_open");
			return TinyFsmState.End();
		}
		case -4:
			return TinyFsmState.End();
		case 0:
			if (m_isReplay)
			{
				OnSetEnableDetailsButton(true);
				m_fsm.ChangeState(new TinyFsmState(StateScoreInAnimation));
			}
			return TinyFsmState.End();
		case 101:
			m_fsm.ChangeState(new TinyFsmState(StateScoreInWait));
			return TinyFsmState.End();
		default:
			return TinyFsmState.End();
		}
	}

	private TinyFsmState StateScoreInWait(TinyFsmEvent e)
	{
		switch (e.Signal)
		{
		case -3:
			return TinyFsmState.End();
		case -4:
			return TinyFsmState.End();
		case 0:
			if (m_isScoreStart)
			{
				bool flag = false;
				EventManager instance = EventManager.Instance;
				if (instance != null)
				{
					flag = (instance.EventStage || instance.IsCollectEvent());
				}
				if (m_resultType == ResultType.BOSS)
				{
					flag = (EventManager.Instance.IsRaidBossStage() ? true : false);
				}
				if (flag)
				{
					m_fsm.ChangeState(new TinyFsmState(StateEventScoreDisplaying));
				}
				else
				{
					m_fsm.ChangeState(new TinyFsmState(StateScoreInAnimation));
				}
				if (m_isReplay)
				{
					OnSetEnableDetailsButton(true);
				}
			}
			return TinyFsmState.End();
		default:
			return TinyFsmState.End();
		}
	}

	private TinyFsmState StateEventScoreDisplaying(TinyFsmEvent e)
	{
		switch (e.Signal)
		{
		case -3:
			if (EventManager.Instance.IsSpecialStage())
			{
				m_eventResult = GameObjectUtil.FindChildGameObjectComponent<HudEventResult>(base.gameObject, "EventResult_spstage");
			}
			else if (EventManager.Instance.IsRaidBossStage())
			{
				m_eventResult = GameObjectUtil.FindChildGameObjectComponent<HudEventResult>(base.gameObject, "EventResult_raidboss");
			}
			else
			{
				m_eventResult = GameObjectUtil.FindChildGameObjectComponent<HudEventResult>(base.gameObject, "EventResult_animal");
			}
			if (m_eventResult != null)
			{
				m_eventResult.Setup(m_eventTimeup);
				m_eventResult.PlayStart();
			}
			return TinyFsmState.End();
		case -4:
			return TinyFsmState.End();
		case 0:
			if (m_eventResult != null)
			{
				if (m_eventResult.IsEndOutAnim)
				{
					bool flag = false;
					EventManager instance = EventManager.Instance;
					if (instance != null)
					{
						if (instance.Type == EventManager.EventType.COLLECT_OBJECT)
						{
							flag = true;
						}
						switch (m_eventResultState)
						{
						default:
							flag = true;
							break;
						case EventResultState.TIMEUP:
							break;
						}
						if (instance.IsRaidBossStage())
						{
							flag = false;
						}
					}
					if (!m_eventTimeup || flag)
					{
						m_fsm.ChangeState(new TinyFsmState(StateScoreInAnimation));
					}
					else
					{
						if (m_scores != null)
						{
							m_scores.AllSkip();
						}
						IsPressNext = true;
						m_fsm.ChangeState(new TinyFsmState(StateOutScoreAnimation));
					}
				}
			}
			else
			{
				m_fsm.ChangeState(new TinyFsmState(StateScoreInAnimation));
			}
			return TinyFsmState.End();
		case 103:
			if (m_eventResult != null)
			{
				m_eventResult.PlayOutAnimation();
			}
			return TinyFsmState.End();
		default:
			return TinyFsmState.End();
		}
	}

	private TinyFsmState StateScoreInAnimation(TinyFsmEvent e)
	{
		switch (e.Signal)
		{
		case -3:
			if (m_scores != null)
			{
				m_scores.PlayScoreInAnimation(ScoreInAnimationEndCallback);
			}
			return TinyFsmState.End();
		case -4:
			return TinyFsmState.End();
		case 0:
			return TinyFsmState.End();
		case 102:
			m_fsm.ChangeState(new TinyFsmState(StateScoreChanging));
			return TinyFsmState.End();
		default:
			return TinyFsmState.End();
		}
	}

	private TinyFsmState StateScoreChanging(TinyFsmEvent e)
	{
		switch (e.Signal)
		{
		case -3:
			if (m_scores != null)
			{
				m_scores.PlayStart();
			}
			return TinyFsmState.End();
		case -4:
			return TinyFsmState.End();
		case 0:
			if (m_scores != null && m_scores.IsEnd)
			{
				bool flag = false;
				if (!m_isReplay)
				{
					if (m_isBossTutorialClear)
					{
						flag = true;
					}
					else if (m_isNomiss && RouletteManager.Instance != null && RouletteManager.Instance.specialEgg >= 10)
					{
						flag = true;
					}
				}
				if (flag)
				{
					m_fsm.ChangeState(new TinyFsmState(StateSpEggMessage));
				}
				else
				{
					m_fsm.ChangeState(new TinyFsmState(StateWaitButtonPressed));
				}
			}
			return TinyFsmState.End();
		case 103:
			if (m_scores != null)
			{
				m_scores.AllSkip();
			}
			return TinyFsmState.End();
		default:
			return TinyFsmState.End();
		}
	}

	private TinyFsmState StateSpEggMessage(TinyFsmEvent e)
	{
		switch (e.Signal)
		{
		case -3:
		{
			GeneralWindow.CInfo info = default(GeneralWindow.CInfo);
			info.name = "SpEggMessage";
			info.buttonType = GeneralWindow.ButtonType.Ok;
			if (m_isBossTutorialClear)
			{
				info.caption = TextUtility.GetCommonText("ChaoRoulette", "sp_egg_get_caption");
				info.message = TextUtility.GetCommonText("ChaoRoulette", "sp_egg_get_text");
			}
			else
			{
				info.caption = TextUtility.GetCommonText("ChaoRoulette", "sp_egg_max_caption");
				info.message = TextUtility.GetCommonText("ChaoRoulette", "sp_egg_max_text");
			}
			GeneralWindow.Create(info);
			return TinyFsmState.End();
		}
		case -4:
			return TinyFsmState.End();
		case 0:
			if (GeneralWindow.IsCreated("SpEggMessage") && GeneralWindow.IsButtonPressed)
			{
				GeneralWindow.Close();
				m_fsm.ChangeState(new TinyFsmState(StateWaitButtonPressed));
			}
			return TinyFsmState.End();
		default:
			return TinyFsmState.End();
		}
	}

	private TinyFsmState StateWaitButtonPressed(TinyFsmEvent e)
	{
		switch (e.Signal)
		{
		case -3:
			OnSetEnableNextButton(true);
			OnSetEnableDetailsButton(true);
			if (!m_scores.IsBonusEvent())
			{
				OnSetEnableDetailsButton(false);
			}
			return TinyFsmState.End();
		case -4:
			return TinyFsmState.End();
		case 0:
			return TinyFsmState.End();
		case 103:
			if (m_scores != null)
			{
				m_scores.AllSkip();
			}
			IsPressNext = true;
			m_fsm.ChangeState(new TinyFsmState(StateOutScoreAnimation));
			return TinyFsmState.End();
		case 104:
			OnSetEnableNextButton(false);
			OnSetEnableDetailsButton(false);
			m_isReplay = true;
			m_fsm.ChangeState(new TinyFsmState(StateInAnimation));
			return TinyFsmState.End();
		case 105:
			OnSetEnableNextButton(true);
			OnSetEnableDetailsButton(true);
			return TinyFsmState.End();
		default:
			return TinyFsmState.End();
		}
	}

	private TinyFsmState StateOutScoreAnimation(TinyFsmEvent e)
	{
		switch (e.Signal)
		{
		case -3:
			if (m_scores != null)
			{
				m_scores.PlayScoreOutAnimation(ScoreOutAnimationCallback);
			}
			return TinyFsmState.End();
		case -4:
			return TinyFsmState.End();
		case 0:
			return TinyFsmState.End();
		case 106:
			m_fsm.ChangeState(new TinyFsmState(StateShowCharacterGlowUp));
			return TinyFsmState.End();
		default:
			return TinyFsmState.End();
		}
	}

	private TinyFsmState StateShowCharacterGlowUp(TinyFsmEvent e)
	{
		switch (e.Signal)
		{
		case -3:
		{
			GlowUpWindow glowUpWindow2 = GameObjectUtil.FindChildGameObjectComponent<GlowUpWindow>(base.gameObject, "ResultPlayerExpWindow");
			if (glowUpWindow2 != null)
			{
				GlowUpWindow.ExpType expType = GlowUpWindow.ExpType.RUN_STAGE;
				expType = ((m_resultType == ResultType.BOSS) ? (m_isBossDestroyed ? GlowUpWindow.ExpType.BOSS_SUCCESS : GlowUpWindow.ExpType.BOSS_FAILED) : GlowUpWindow.ExpType.RUN_STAGE);
				glowUpWindow2.PlayStart(expType);
			}
			return TinyFsmState.End();
		}
		case -4:
			return TinyFsmState.End();
		case 0:
		{
			bool flag = false;
			GlowUpWindow glowUpWindow = GameObjectUtil.FindChildGameObjectComponent<GlowUpWindow>(base.gameObject, "ResultPlayerExpWindow");
			if (glowUpWindow != null)
			{
				if (glowUpWindow.IsPlayEnd)
				{
					flag = true;
				}
			}
			else
			{
				flag = true;
			}
			if (flag)
			{
				m_fsm.ChangeState(new TinyFsmState(StateOutAnimation));
			}
			return TinyFsmState.End();
		}
		default:
			return TinyFsmState.End();
		}
	}

	private TinyFsmState StateOutAnimation(TinyFsmEvent e)
	{
		switch (e.Signal)
		{
		case -3:
			if (m_scores != null)
			{
				m_scores.OnFinishScore();
				m_scores.PlayScoreOutAnimation(OutAnimationEndCallback);
			}
			OnSetEnableDetailsButton(false);
			return TinyFsmState.End();
		case -4:
			return TinyFsmState.End();
		case 0:
			return TinyFsmState.End();
		case 107:
			m_fsm.ChangeState(new TinyFsmState(StateFinished));
			return TinyFsmState.End();
		default:
			return TinyFsmState.End();
		}
	}

	private TinyFsmState StateFinished(TinyFsmEvent e)
	{
		switch (e.Signal)
		{
		case -3:
			if (m_resultRoot != null)
			{
				m_resultRoot.SetActive(true);
			}
			return TinyFsmState.End();
		case -4:
			return TinyFsmState.End();
		case 0:
			return TinyFsmState.End();
		default:
			return TinyFsmState.End();
		}
	}

	private void OnClickNextButton()
	{
		SoundManager.SePlay("sys_menu_decide");
		TinyFsmEvent signal = TinyFsmEvent.CreateUserEvent(103);
		if (m_fsm != null)
		{
			m_fsm.Dispatch(signal);
		}
	}

	private void OnClickDetailsButton()
	{
		SoundManager.SePlay("sys_menu_decide");
		TinyFsmEvent signal = TinyFsmEvent.CreateUserEvent(104);
		if (m_fsm != null)
		{
			m_fsm.Dispatch(signal);
		}
		if (m_scores != null)
		{
			if (m_isReplay)
			{
				m_scores.PlaySkip();
			}
			else
			{
				m_scores.AllSkip();
			}
		}
		if (m_scores != null)
		{
			m_scores.SetActiveDetailsButton(true);
		}
	}

	private void OnClickSkipButton()
	{
		if (m_scores != null)
		{
			m_scores.PlaySkip();
		}
	}

	private void OnClickDetailsEndButton()
	{
		TinyFsmEvent signal = TinyFsmEvent.CreateUserEvent(105);
		if (m_fsm != null)
		{
			m_fsm.Dispatch(signal);
		}
		if (m_scores != null)
		{
			m_scores.SetActiveDetailsButton(false);
		}
	}

	private void SetupResultType(ResultType resultType, bool isNoMiss, bool isBossTutorialClear, bool isRaidBossDestroy)
	{
		GameObject gameObject = base.transform.Find("result_Anim").gameObject;
		GameObject gameObject2 = base.transform.Find("result_boss_Anim").gameObject;
		GameObject gameObject3 = base.transform.Find("result_word_Anim").gameObject;
		GameResultUtility.SetBossDestroyFlag(isRaidBossDestroy);
		switch (resultType)
		{
		case ResultType.NORMAL:
			m_isNomiss = false;
			m_isBossTutorialClear = false;
			m_scores = base.gameObject.AddComponent<GameResultScoresNormal>();
			m_resultRoot = gameObject;
			m_eventRoot = gameObject3;
			break;
		case ResultType.BOSS:
			if (EventManager.Instance.IsRaidBossStage())
			{
				GameResultScoresRaidBoss gameResultScoresRaidBoss = base.gameObject.AddComponent<GameResultScoresRaidBoss>();
				gameResultScoresRaidBoss.SetBossDestroyFlag(isRaidBossDestroy);
				m_isNomiss = false;
				m_isBossTutorialClear = false;
				m_scores = gameResultScoresRaidBoss;
				m_resultRoot = gameObject2;
			}
			else
			{
				GameResultScoresBoss gameResultScoresBoss = base.gameObject.AddComponent<GameResultScoresBoss>();
				gameResultScoresBoss.SetNoMissFlag(isNoMiss);
				m_isNomiss = isNoMiss;
				m_isBossTutorialClear = isBossTutorialClear;
				m_scores = gameResultScoresBoss;
				m_resultRoot = gameObject2;
			}
			m_eventRoot = gameObject3;
			break;
		}
		BackKeyManager.AddWindowCallBack(base.gameObject);
		m_AnimationObject = m_resultRoot;
		Animation animation = GameResultUtility.SearchAnimation(m_AnimationObject);
		if (animation != null)
		{
			animation.Stop();
		}
		m_imageButtonNext = GameObjectUtil.FindChildGameObjectComponent<UIImageButton>(m_resultRoot, "Btn_next");
		if (m_imageButtonNext != null)
		{
			m_imageButtonNext.isEnabled = false;
		}
		m_imageButtonDetails = GameObjectUtil.FindChildGameObjectComponent<UIImageButton>(m_resultRoot, "Btn_details");
		if (m_imageButtonDetails != null)
		{
			m_imageButtonDetails.isEnabled = false;
			UIButtonMessage uIButtonMessage = m_imageButtonDetails.gameObject.GetComponent<UIButtonMessage>();
			if (uIButtonMessage == null)
			{
				uIButtonMessage = m_imageButtonDetails.gameObject.AddComponent<UIButtonMessage>();
			}
			if (uIButtonMessage != null)
			{
				uIButtonMessage.enabled = true;
				uIButtonMessage.trigger = UIButtonMessage.Trigger.OnClick;
				uIButtonMessage.target = base.gameObject;
				uIButtonMessage.functionName = "OnClickDetailsButton";
			}
		}
		SystemSaveManager instance = SystemSaveManager.Instance;
		if (instance != null)
		{
			instance.AddPlayCount();
		}
	}

	private void InAnimationEndCallback()
	{
		TinyFsmEvent signal = TinyFsmEvent.CreateUserEvent(101);
		if (m_fsm != null)
		{
			m_fsm.Dispatch(signal);
		}
	}

	private void ScoreInAnimationEndCallback()
	{
		TinyFsmEvent signal = TinyFsmEvent.CreateUserEvent(102);
		if (m_fsm != null)
		{
			m_fsm.Dispatch(signal);
		}
	}

	private void ScoreOutAnimationCallback()
	{
		TinyFsmEvent signal = TinyFsmEvent.CreateUserEvent(106);
		if (m_fsm != null)
		{
			m_fsm.Dispatch(signal);
		}
	}

	private void OutAnimationEndCallback()
	{
		IsEndOutAnimation = true;
		TinyFsmEvent signal = TinyFsmEvent.CreateUserEvent(107);
		if (m_fsm != null)
		{
			m_fsm.Dispatch(signal);
		}
	}

	private void OnSetEnableNextButton(bool enabled)
	{
		if (m_imageButtonNext != null)
		{
			m_imageButtonNext.isEnabled = enabled;
		}
	}

	private void OnSetEnableDetailsButton(bool enabled)
	{
		if (m_imageButtonDetails != null)
		{
			m_imageButtonDetails.isEnabled = enabled;
		}
	}

	public void OnClickPlatformBackButton()
	{
		if (base.gameObject.activeSelf && (!(m_eventResult != null) || m_eventResult.IsBackkeyEnable()) && m_imageButtonNext != null && m_imageButtonNext.isEnabled)
		{
			OnClickNextButton();
		}
	}
}
