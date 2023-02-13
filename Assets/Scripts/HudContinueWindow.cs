using AnimationOrTween;
using System.Collections.Generic;
using Text;
using UnityEngine;

public class HudContinueWindow : MonoBehaviour
{
	private enum PressedButton
	{
		NO_PRESSING,
		YES,
		NO,
		VIDEO
	}

	private enum State
	{
		IDLE,
		START,
		WAIT_TOUCH_BUTTON,
		TOUCHED_BUTTON
	}

	private const float WAIT_TIME = 0.5f;

	private bool m_debugInfo;

	private PressedButton m_pressedButton;

	private State m_state;

	private GameObject m_parentPanel;

	private GameObject m_timeUpObj;

	private bool m_videoEnabled;

	private bool m_bossStage;

	private float m_waitTime;

	private string m_scoreText;

	private string m_dailyBattleText;

	public bool IsYesButtonPressed
	{
		get
		{
			if (m_state == State.TOUCHED_BUTTON && m_pressedButton == PressedButton.YES)
			{
				return true;
			}
			return false;
		}
		private set
		{
		}
	}

	public bool IsNoButtonPressed
	{
		get
		{
			if (m_state == State.TOUCHED_BUTTON && m_pressedButton == PressedButton.NO)
			{
				return true;
			}
			return false;
		}
		private set
		{
		}
	}

	public bool IsVideoButtonPressed
	{
		get
		{
			if (m_state == State.TOUCHED_BUTTON && m_pressedButton == PressedButton.VIDEO)
			{
				return true;
			}
			return false;
		}
		private set
		{
		}
	}

	public void Setup(bool bossStage)
	{
		m_bossStage = bossStage;
		m_parentPanel = base.gameObject;
		if (m_timeUpObj == null)
		{
			m_timeUpObj = GameObjectUtil.FindChildGameObject(base.gameObject, "timesup");
			SetTimeUpObj(false);
		}
		bool flag = false;
		GameObject gameObject = GameObjectUtil.FindChildGameObject(base.gameObject, "HL-AdsEnabled");
		GameObject gameObject2 = GameObjectUtil.FindChildGameObject(base.gameObject, "HL-AdsDisabled");
		if (gameObject2 != null && gameObject != null)
		{
			if (flag)
			{
				m_parentPanel = gameObject;
				gameObject.SetActive(true);
				gameObject2.SetActive(false);
			}
			else
			{
				m_parentPanel = gameObject2;
				gameObject2.SetActive(true);
				gameObject.SetActive(false);
			}
		}
		UIButtonMessage uIButtonMessage = GameObjectUtil.FindChildGameObjectComponent<UIButtonMessage>(m_parentPanel, "Btn_yes");
		if (uIButtonMessage != null)
		{
			uIButtonMessage.target = base.gameObject;
			uIButtonMessage.functionName = "OnClickYesButton";
		}
		UIButtonMessage uIButtonMessage2 = GameObjectUtil.FindChildGameObjectComponent<UIButtonMessage>(m_parentPanel, "Btn_no");
		if (uIButtonMessage2 != null)
		{
			uIButtonMessage2.target = base.gameObject;
			uIButtonMessage2.functionName = "OnClickNoButton";
		}
		if (flag)
		{
			UIButtonMessage uIButtonMessage3 = GameObjectUtil.FindChildGameObjectComponent<UIButtonMessage>(m_parentPanel, "Btn_video");
			if (uIButtonMessage3 != null)
			{
				uIButtonMessage3.target = base.gameObject;
				uIButtonMessage3.functionName = "OnClickVideoButton";
			}
		}
		UILabel uILabel = GameObjectUtil.FindChildGameObjectComponent<UILabel>(base.gameObject, "Lbt_caption");
		if (uILabel != null)
		{
			string text2 = uILabel.text = TextManager.GetText(TextManager.TextType.TEXTTYPE_COMMON_TEXT, "Continue", "rsring_continue_caption").text;
		}
	}

	public void SetTimeUpObj(bool enablFlag)
	{
		if (m_timeUpObj != null)
		{
			m_timeUpObj.SetActive(enablFlag);
		}
	}

	public void SetVideoButton(bool enablFlag)
	{
		m_videoEnabled = enablFlag;
		Setup(m_bossStage);
	}

	private void UpdateRedStarRingCount()
	{
		GameObject gameObject = GameObjectUtil.FindChildGameObject(base.gameObject, "textView");
		if (gameObject == null)
		{
			return;
		}
		UILabel uILabel = GameObjectUtil.FindChildGameObjectComponent<UILabel>(gameObject, "Lbl_body");
		if (!(uILabel == null))
		{
			TextObject text = TextManager.GetText(TextManager.TextType.TEXTTYPE_COMMON_TEXT, "Continue", "label_rsring_continue");
			text.ReplaceTag("{RING_NUM}", HudContinueUtility.GetContinueCostString());
			text.ReplaceTag("{OWNED_RSRING}", HudContinueUtility.GetRedStarRingCountString());
			if (string.IsNullOrEmpty(m_scoreText))
			{
				m_scoreText = GetScoreText();
			}
			if (string.IsNullOrEmpty(m_dailyBattleText))
			{
				m_dailyBattleText = GetDailyBattleText();
			}
			uILabel.text = m_scoreText + m_dailyBattleText + text.text;
		}
	}

	private string GetScoreText()
	{
		if (m_bossStage)
		{
			LevelInformation levelInformation = ObjUtil.GetLevelInformation();
			if (levelInformation != null)
			{
				int num = levelInformation.NumBossHpMax - levelInformation.NumBossAttack;
				int num2 = GameModeStage.ContinueRestCount();
				if (EventManager.Instance != null && EventManager.Instance.IsRaidBossStage())
				{
					TextObject text = TextManager.GetText(TextManager.TextType.TEXTTYPE_COMMON_TEXT, "Continue", "label_rsring_continue_RAID");
					text.ReplaceTag("{PARAM_LIFE}", num.ToString());
					text.ReplaceTag("{PARAM}", num2.ToString());
					return text.text;
				}
				TextObject text2 = TextManager.GetText(TextManager.TextType.TEXTTYPE_COMMON_TEXT, "Continue", "label_rsring_continue_E");
				text2.ReplaceTag("{PARAM_LIFE}", num.ToString());
				text2.ReplaceTag("{PARAM}", num2.ToString());
				return text2.text;
			}
			return string.Empty;
		}
		bool flag = false;
		if (EventManager.Instance != null && EventManager.Instance.IsSpecialStage())
		{
			flag = true;
		}
		ObjUtil.SendMessageFinalScore();
		StageScoreManager instance = StageScoreManager.Instance;
		if (instance == null)
		{
			return string.Empty;
		}
		RankingManager instance2 = SingletonGameObject<RankingManager>.Instance;
		if (instance2 == null)
		{
			return string.Empty;
		}
		long num3 = 0L;
		num3 = (flag ? instance.FinalCountData.sp_crystal : instance.FinalScore);
		RankingUtil.RankingMode rankingMode = RankingUtil.RankingMode.ENDLESS;
		if (StageModeManager.Instance != null && StageModeManager.Instance.IsQuickMode())
		{
			rankingMode = RankingUtil.RankingMode.QUICK;
		}
		long myScore = 0L;
		long myHiScore = 0L;
		int myLeague = 0;
		RankingManager.GetCurrentRankingStatus(rankingMode, flag, out myScore, out myHiScore, out myLeague);
		long currentScore = num3;
		bool isHighScore = false;
		long nextRankScore = 0L;
		long prveRankScore = 0L;
		int nextRank = 0;
		RankingUtil.RankingScoreType currentRankingScoreType = RankingManager.GetCurrentRankingScoreType(rankingMode, flag);
		int currentHighScoreRank = RankingManager.GetCurrentHighScoreRank(rankingMode, flag, ref currentScore, out isHighScore, out nextRankScore, out prveRankScore, out nextRank);
		if (currentRankingScoreType == RankingUtil.RankingScoreType.TOTAL_SCORE)
		{
			isHighScore = true;
		}
		LeagueType league = (LeagueType)myLeague;
		if (currentHighScoreRank == 1 && isHighScore && nextRankScore == 0L && prveRankScore == 0L)
		{
			if (!flag)
			{
				return GetTextObject("label_rsring_continue_F", league, 0, 0, 0L, currentRankingScoreType);
			}
			return GetSpStageTextObject("label_rsring_continue_SP_A", 1, 0, currentScore, 0L, currentRankingScoreType);
		}
		if (!isHighScore)
		{
			if (!flag)
			{
				long score = myHiScore - num3 + 1;
				return GetTextObject("label_rsring_continue_A", league, currentHighScoreRank, 0, score, currentRankingScoreType);
			}
			return GetSpStageTextObject("label_rsring_continue_SP_A", currentHighScoreRank, 0, currentScore, 0L, currentRankingScoreType);
		}
		if (currentHighScoreRank == 1 && nextRankScore == 0L && prveRankScore == 0L)
		{
			if (!flag)
			{
				return GetTextObject("label_rsring_continue_F", league, 0, 0, 0L, currentRankingScoreType);
			}
			return GetSpStageTextObject("label_rsring_continue_SP_A", 1, 0, currentScore, 0L, currentRankingScoreType);
		}
		if (currentHighScoreRank == 1 && nextRankScore == 0L && prveRankScore >= 0)
		{
			if (!flag)
			{
				return GetTextObject("label_rsring_continue_C", league, 0, 0, prveRankScore, currentRankingScoreType);
			}
			return GetSpStageTextObject("label_rsring_continue_SP_C", 0, 0, currentScore, prveRankScore, currentRankingScoreType);
		}
		if (currentHighScoreRank > 1)
		{
			if (!flag)
			{
				return GetTextObject("label_rsring_continue_B", league, currentHighScoreRank, nextRank, nextRankScore, currentRankingScoreType);
			}
			return GetSpStageTextObject("label_rsring_continue_SP_B", currentHighScoreRank, nextRank, currentScore, nextRankScore, currentRankingScoreType);
		}
		return string.Empty;
	}

	private string GetTextObject(string cellName, LeagueType league, int rank1, int rank2, long score, RankingUtil.RankingScoreType scoreType)
	{
		string empty = string.Empty;
		empty = ((scoreType != 0) ? TextManager.GetText(TextManager.TextType.TEXTTYPE_COMMON_TEXT, "Ranking", "ui_Lbl_total_score").text : TextManager.GetText(TextManager.TextType.TEXTTYPE_COMMON_TEXT, "Ranking", "ui_Lbl_high_score").text);
		TextObject text = TextManager.GetText(TextManager.TextType.TEXTTYPE_COMMON_TEXT, "Continue", cellName);
		text.ReplaceTag("{PARAM_LEAGUE}", RankingUtil.GetLeagueName(league));
		text.ReplaceTag("{PARAM_RANK}", rank1.ToString());
		text.ReplaceTag("{PARAM_RANK2}", rank2.ToString());
		text.ReplaceTag("{PARAM_SCORE}", HudUtility.GetFormatNumString(score));
		text.ReplaceTag("{SCORE}", empty);
		return text.text;
	}

	private string GetSpStageTextObject(string cellName, int rank1, int rank2, long score, long score2, RankingUtil.RankingScoreType scoreType)
	{
		string empty = string.Empty;
		empty = ((scoreType != 0) ? TextManager.GetText(TextManager.TextType.TEXTTYPE_COMMON_TEXT, "Ranking", "ui_Lbl_total_score").text : TextManager.GetText(TextManager.TextType.TEXTTYPE_COMMON_TEXT, "Ranking", "ui_Lbl_high_score").text);
		TextObject text = TextManager.GetText(TextManager.TextType.TEXTTYPE_COMMON_TEXT, "Continue", cellName);
		text.ReplaceTag("{PARAM_STAGE}", HudUtility.GetEventStageName());
		text.ReplaceTag("{PARAM_OBJECT}", HudUtility.GetEventSpObjectName());
		text.ReplaceTag("{PARAM_RANK}", HudUtility.GetFormatNumString(rank1));
		text.ReplaceTag("{PARAM_RANK2}", HudUtility.GetFormatNumString(rank2));
		text.ReplaceTag("{PARAM_SCORE}", HudUtility.GetFormatNumString(score));
		text.ReplaceTag("{PARAM_SCORE2}", HudUtility.GetFormatNumString(score2));
		text.ReplaceTag("{SCORE}", empty);
		return text.text;
	}

	private bool IsRankerNone(ServerLeaderboardEntries serverLeaderboardEntries)
	{
		if (serverLeaderboardEntries.m_leaderboardEntries == null)
		{
			return true;
		}
		if (serverLeaderboardEntries.m_leaderboardEntries.Count <= 0)
		{
			return true;
		}
		return false;
	}

	private bool IsRankerAlone(ServerLeaderboardEntries serverLeaderboardEntries, ServerLeaderboardEntry myServerLeaderboardEntry)
	{
		if (serverLeaderboardEntries.m_leaderboardEntries.Count == 1 && serverLeaderboardEntries.m_leaderboardEntries[0].m_hspId == myServerLeaderboardEntry.m_hspId)
		{
			return true;
		}
		return false;
	}

	private ServerLeaderboardEntry GetOtherRanker(ServerLeaderboardEntries serverLeaderboardEntries, ServerLeaderboardEntry myServerLeaderboardEntry)
	{
		int num = serverLeaderboardEntries.m_leaderboardEntries.Count - 1;
		for (int num2 = num; num2 >= 0; num2--)
		{
			if (myServerLeaderboardEntry.m_hspId != serverLeaderboardEntries.m_leaderboardEntries[num2].m_hspId)
			{
				return serverLeaderboardEntries.m_leaderboardEntries[num2];
			}
		}
		return null;
	}

	private ServerLeaderboardEntry GetRankUpPlayerData(ServerLeaderboardEntries serverLeaderboardEntries, ServerLeaderboardEntry myServerLeaderboardEntry, long playScore)
	{
		List<ServerLeaderboardEntry> list = new List<ServerLeaderboardEntry>();
		foreach (ServerLeaderboardEntry leaderboardEntry in serverLeaderboardEntries.m_leaderboardEntries)
		{
			if (leaderboardEntry.m_score > playScore && leaderboardEntry.m_hspId != myServerLeaderboardEntry.m_hspId)
			{
				list.Add(leaderboardEntry);
				DebugInfo("GetRankUpPlayerData LIST rank=" + leaderboardEntry.m_grade + " score=" + leaderboardEntry.m_score);
			}
		}
		if (list.Count > 0)
		{
			list.Sort(RankComparer);
			return list[0];
		}
		return null;
	}

	private static int RankComparer(ServerLeaderboardEntry itemA, ServerLeaderboardEntry itemB)
	{
		if (itemA != null && itemB != null)
		{
			if (itemA.m_grade > itemB.m_grade)
			{
				return -1;
			}
			if (itemA.m_grade < itemB.m_grade)
			{
				return 1;
			}
		}
		return 0;
	}

	private string GetDailyBattleText()
	{
		string result = string.Empty;
		long num = 0L;
		if (StageModeManager.Instance != null && !StageModeManager.Instance.IsQuickMode())
		{
			return string.Empty;
		}
		DailyBattleManager instance = SingletonGameObject<DailyBattleManager>.Instance;
		if (instance != null && !m_bossStage)
		{
			string cellName = string.Empty;
			long num2 = 0L;
			long num3 = 0L;
			long num4 = 0L;
			if (instance.currentWinFlag > 0)
			{
				ServerDailyBattleDataPair currentDataPair = instance.currentDataPair;
				if (currentDataPair != null)
				{
					bool flag = true;
					StageScoreManager instance2 = StageScoreManager.Instance;
					if (instance2 != null)
					{
						num = (num4 = instance2.FinalScore);
						if (currentDataPair.myBattleData != null && !string.IsNullOrEmpty(currentDataPair.myBattleData.userId) && currentDataPair.myBattleData.maxScore > num)
						{
							num = currentDataPair.myBattleData.maxScore;
							flag = false;
						}
					}
					if (currentDataPair.rivalBattleData == null || string.IsNullOrEmpty(currentDataPair.rivalBattleData.userId))
					{
						if (flag)
						{
							cellName = "label_rsring_continue_DB_C";
						}
						else
						{
							num3 = num - num4;
							cellName = "label_rsring_continue_DB_F";
						}
					}
					else
					{
						long maxScore = currentDataPair.rivalBattleData.maxScore;
						if (maxScore > num)
						{
							num2 = maxScore - num;
							if (flag)
							{
								cellName = "label_rsring_continue_DB_B";
							}
							else
							{
								num3 = num - num4;
								cellName = "label_rsring_continue_DB_E";
							}
						}
						else
						{
							num2 = num - maxScore;
							if (flag)
							{
								cellName = "label_rsring_continue_DB_A";
							}
							else
							{
								num3 = num - num4;
								cellName = "label_rsring_continue_DB_D";
							}
						}
					}
				}
			}
			else
			{
				cellName = "label_rsring_continue_DB_C";
			}
			TextObject text = TextManager.GetText(TextManager.TextType.TEXTTYPE_COMMON_TEXT, "Continue", cellName);
			if (text != null)
			{
				text.ReplaceTag("{PARAM_SCORE}", HudUtility.GetFormatNumString(num2));
				text.ReplaceTag("{PARAM_HIGHSCORE}", HudUtility.GetFormatNumString(num3));
				result = text.text;
			}
		}
		return result;
	}

	public void PlayStart()
	{
		m_pressedButton = PressedButton.NO_PRESSING;
		m_state = State.START;
		m_waitTime = 0f;
		base.gameObject.SetActive(true);
		Animation component = base.gameObject.GetComponent<Animation>();
		if (component != null)
		{
			ActiveAnimation.Play(component, Direction.Forward);
		}
	}

	private void Start()
	{
	}

	private void Update()
	{
		if (m_state == State.IDLE)
		{
			return;
		}
		if (m_state == State.START)
		{
			m_waitTime += RealTime.deltaTime;
			if (m_waitTime > 0.5f)
			{
				m_state = State.WAIT_TOUCH_BUTTON;
				m_waitTime = 0f;
			}
		}
		if (m_parentPanel == null)
		{
			m_parentPanel = base.gameObject;
		}
		GameObject gameObject = GameObjectUtil.FindChildGameObject(m_parentPanel, "Btn_yes");
		if (gameObject != null)
		{
			UILabel uILabel = GameObjectUtil.FindChildGameObjectComponent<UILabel>(gameObject, "Lbl_rs_cost");
			if (uILabel != null)
			{
				uILabel.text = HudContinueUtility.GetContinueCostString();
			}
			GameObject gameObject2 = GameObjectUtil.FindChildGameObject(gameObject, "img_sale_icon");
			if (gameObject2 != null)
			{
				bool active = HudContinueUtility.IsInContinueCostCampaign();
				gameObject2.SetActive(active);
			}
		}
		UpdateRedStarRingCount();
	}

	private void OnClickYesButton()
	{
		if (m_state != State.WAIT_TOUCH_BUTTON)
		{
			return;
		}
		m_pressedButton = PressedButton.YES;
		SoundManager.SePlay("sys_menu_decide");
		Animation component = base.gameObject.GetComponent<Animation>();
		if (component != null)
		{
			ActiveAnimation activeAnimation = ActiveAnimation.Play(base.animation, Direction.Reverse);
			if (activeAnimation != null)
			{
				EventDelegate.Add(activeAnimation.onFinished, OnFinishedAnimationCallbck, true);
			}
		}
	}

	private void OnClickNoButton()
	{
		if (m_state != State.WAIT_TOUCH_BUTTON)
		{
			return;
		}
		m_pressedButton = PressedButton.NO;
		SoundManager.SePlay("sys_window_close");
		Animation component = base.gameObject.GetComponent<Animation>();
		if (component != null)
		{
			ActiveAnimation activeAnimation = ActiveAnimation.Play(base.animation, Direction.Reverse);
			if (activeAnimation != null)
			{
				EventDelegate.Add(activeAnimation.onFinished, OnFinishedAnimationCallbck, true);
			}
		}
	}

	private void OnClickVideoButton()
	{
		if (m_state != State.WAIT_TOUCH_BUTTON)
		{
			return;
		}
		m_pressedButton = PressedButton.VIDEO;
		SoundManager.SePlay("sys_menu_decide");
		Animation component = base.gameObject.GetComponent<Animation>();
		if (component != null)
		{
			ActiveAnimation activeAnimation = ActiveAnimation.Play(base.animation, Direction.Reverse);
			if (activeAnimation != null)
			{
				EventDelegate.Add(activeAnimation.onFinished, OnFinishedAnimationCallbck, true);
			}
		}
	}

	public void OnPushBackKey()
	{
		OnClickNoButton();
	}

	private void OnFinishedAnimationCallbck()
	{
		base.gameObject.SetActive(false);
		m_scoreText = string.Empty;
		m_dailyBattleText = string.Empty;
		m_state = State.TOUCHED_BUTTON;
	}

	private void DebugInfo(string msg)
	{
	}
}
