using AnimationOrTween;
using System.Collections;
using Text;
using UnityEngine;

public class DailyBattleDetailWindow : WindowBase
{
	private const float OPEN_EFFECT_START = 0.5f;

	private const float OPEN_EFFECT_TIME = 1.5f;

	private static bool s_isActive;

	[SerializeField]
	private Animation m_animation;

	[SerializeField]
	private UIPanel m_mainPanel;

	private bool m_isClickClose;

	private bool m_isEnd;

	private bool m_isOpenEffectStart;

	private bool m_isOpenEffectIssue;

	private bool m_isOpenEffectEnd;

	private float m_time;

	private long m_targetScore;

	private int m_winFlag;

	private GameObject m_mineSet;

	private GameObject m_adversarySet;

	private UILabel m_result;

	private ui_ranking_scroll m_mineData;

	private ui_ranking_scroll m_adversaryData;

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
		if (!base.gameObject.activeSelf || m_isOpenEffectEnd)
		{
			return;
		}
		if (!m_isOpenEffectStart && m_time >= 0.5f)
		{
			m_isOpenEffectStart = true;
			if (m_winFlag > 1)
			{
				SoundManager.SePlay("sys_rank_up");
			}
			else
			{
				SoundManager.SePlay("sys_league_down");
			}
		}
		else if (m_isOpenEffectStart)
		{
			float num = (m_time - 0.5f) / 1.5f;
			if (num < 0f)
			{
				num = 0f;
			}
			else if (num > 1f)
			{
				num = 1f;
			}
			long num2 = (long)((float)m_targetScore * num);
			if (m_mineData != null)
			{
				if (num2 < m_mineData.rankerScore)
				{
					m_mineData.UpdateViewScore(num2);
				}
				else
				{
					m_mineData.UpdateViewScore(-1L);
					if (!m_isOpenEffectIssue)
					{
						m_isOpenEffectIssue = true;
						SetupUserData(m_winFlag);
					}
				}
			}
			if (m_adversaryData != null)
			{
				if (num2 < m_adversaryData.rankerScore)
				{
					m_adversaryData.UpdateViewScore(num2);
				}
				else
				{
					m_adversaryData.UpdateViewScore(-1L);
					if (!m_isOpenEffectIssue)
					{
						m_isOpenEffectIssue = true;
						SetupUserData(m_winFlag);
					}
				}
			}
			if (num >= 1f)
			{
				m_isOpenEffectEnd = true;
				if (m_mineData != null)
				{
					m_mineData.UpdateViewScore(-1L);
				}
				if (m_adversaryData != null)
				{
					m_adversaryData.UpdateViewScore(-1L);
				}
				SetupUserData(m_winFlag);
			}
		}
		m_time += Time.deltaTime;
	}

	public static bool Open(ServerDailyBattleDataPair data)
	{
		bool result = false;
		GameObject menuAnimUIObject = HudMenuUtility.GetMenuAnimUIObject();
		if (menuAnimUIObject != null)
		{
			DailyBattleDetailWindow dailyBattleDetailWindow = GameObjectUtil.FindChildGameObjectComponent<DailyBattleDetailWindow>(menuAnimUIObject, "DailyBattleDetailWindow");
			if (dailyBattleDetailWindow == null)
			{
				GameObject gameObject = GameObjectUtil.FindChildGameObject(menuAnimUIObject, "DailyBattleDetailWindow");
				if (gameObject != null)
				{
					dailyBattleDetailWindow = gameObject.AddComponent<DailyBattleDetailWindow>();
				}
			}
			if (dailyBattleDetailWindow != null)
			{
				dailyBattleDetailWindow.Setup(data);
				result = true;
			}
		}
		return result;
	}

	private IEnumerator SetupObject()
	{
		yield return null;
		if (m_mainPanel != null)
		{
			m_mainPanel.alpha = 1f;
		}
		if (m_animation != null)
		{
			ActiveAnimation.Play(m_animation, "ui_cmn_window_Anim2", Direction.Forward);
		}
		UIPlayAnimation btnClose = GameObjectUtil.FindChildGameObjectComponent<UIPlayAnimation>(base.gameObject, "Btn_close");
		if (btnClose != null && !EventDelegate.IsValid(btnClose.onFinished))
		{
			EventDelegate.Add(btnClose.onFinished, OnFinished, true);
		}
		m_targetScore = 0L;
		m_time = 0f;
		m_winFlag = 0;
		if (m_battleData != null)
		{
			m_winFlag = m_battleData.winFlag;
			if (m_battleData != null)
			{
				if (m_battleData.myBattleData != null)
				{
					m_targetScore = m_battleData.myBattleData.maxScore;
				}
				if (m_battleData.rivalBattleData != null && m_targetScore < m_battleData.rivalBattleData.maxScore)
				{
					m_targetScore = m_battleData.rivalBattleData.maxScore;
				}
			}
		}
		GameObject root = GameObjectUtil.FindChildGameObject(base.gameObject, "window_contents");
		m_mineData = null;
		m_adversaryData = null;
		if (root != null)
		{
			GameObject body = GameObjectUtil.FindChildGameObject(root, "body");
			if (body != null)
			{
				if (m_battleData != null)
				{
					body.SetActive(true);
					m_mineSet = GameObjectUtil.FindChildGameObject(body, "duel_mine_set");
					m_adversarySet = GameObjectUtil.FindChildGameObject(body, "duel_adversary_set");
					GameObject vsObj = GameObjectUtil.FindChildGameObject(body, "img_word_vs");
					if (vsObj != null)
					{
						bool flg = false;
						if (m_battleData.myBattleData != null && !string.IsNullOrEmpty(m_battleData.myBattleData.userId) && m_battleData.rivalBattleData != null && !string.IsNullOrEmpty(m_battleData.rivalBattleData.userId))
						{
							flg = true;
						}
						vsObj.SetActive(flg);
					}
					if (m_mineSet != null)
					{
						UIRectItemStorage storage = GameObjectUtil.FindChildGameObjectComponent<UIRectItemStorage>(m_mineSet, "score_mine");
						if (storage != null)
						{
							if (m_battleData.myBattleData != null && !string.IsNullOrEmpty(m_battleData.myBattleData.userId))
							{
								storage.maxItemCount = (storage.maxRows = 1);
								storage.Restart();
								m_mineData = m_mineSet.GetComponentInChildren<ui_ranking_scroll>();
								if (m_mineData != null)
								{
									RankingUtil.Ranker ranker2 = new RankingUtil.Ranker(m_battleData.myBattleData)
									{
										isBoxCollider = false
									};
									m_mineData.UpdateView(RankingUtil.RankingScoreType.HIGH_SCORE, RankingUtil.RankingRankerType.RIVAL, ranker2, true);
									m_mineData.UpdateViewScore(0L);
									m_mineData.SetMyRanker(true);
								}
							}
							else
							{
								storage.maxItemCount = (storage.maxRows = 0);
								storage.Restart();
							}
						}
					}
					if (m_adversarySet != null)
					{
						UIRectItemStorage storage2 = GameObjectUtil.FindChildGameObjectComponent<UIRectItemStorage>(m_adversarySet, "score_adversary");
						if (storage2 != null)
						{
							if (m_battleData.rivalBattleData != null && !string.IsNullOrEmpty(m_battleData.rivalBattleData.userId))
							{
								storage2.maxItemCount = (storage2.maxRows = 1);
								storage2.Restart();
								m_adversaryData = m_adversarySet.GetComponentInChildren<ui_ranking_scroll>();
								if (m_adversaryData != null)
								{
									RankingUtil.Ranker ranker = new RankingUtil.Ranker(m_battleData.rivalBattleData)
									{
										isBoxCollider = false
									};
									if (ranker.isFriend)
									{
										ranker.isSentEnergy = true;
									}
									m_adversaryData.UpdateView(RankingUtil.RankingScoreType.HIGH_SCORE, RankingUtil.RankingRankerType.RIVAL, ranker, true);
									m_adversaryData.UpdateViewScore(0L);
									m_adversaryData.SetMyRanker(false);
								}
							}
							else
							{
								storage2.maxItemCount = (storage2.maxRows = 0);
								storage2.Restart();
							}
						}
					}
					m_result = GameObjectUtil.FindChildGameObjectComponent<UILabel>(body, "Lbl_result");
					SetupUserData();
				}
				else
				{
					body.SetActive(false);
				}
			}
			UILabel caption = GameObjectUtil.FindChildGameObjectComponent<UILabel>(root, "Lbl_caption");
			if (caption != null)
			{
				caption.text = TextUtility.GetText(TextManager.TextType.TEXTTYPE_COMMON_TEXT, "DailyMission", "battle_detail_caption");
			}
		}
		m_isEnd = false;
		m_isClickClose = false;
	}

	private void Setup(ServerDailyBattleDataPair data)
	{
		s_isActive = true;
		base.gameObject.SetActive(true);
		m_battleData = data;
		m_isEnd = false;
		m_isClickClose = false;
		base.enabled = true;
		m_isOpenEffectStart = false;
		m_isOpenEffectIssue = false;
		m_isOpenEffectEnd = false;
		StartCoroutine(SetupObject());
	}

	private void SetupUserData(int winOrLose = 0)
	{
		if (m_battleData == null)
		{
			return;
		}
		if (m_mineSet != null)
		{
			GameObject gameObject = GameObjectUtil.FindChildGameObject(m_mineSet, "duel_win_set");
			GameObject gameObject2 = GameObjectUtil.FindChildGameObject(m_mineSet, "duel_lose_set");
			if (winOrLose <= 0 || m_battleData.myBattleData == null || string.IsNullOrEmpty(m_battleData.myBattleData.userId))
			{
				gameObject.SetActive(false);
				gameObject2.SetActive(false);
			}
			else if (winOrLose == 1)
			{
				gameObject.SetActive(false);
				gameObject2.SetActive(true);
			}
			else
			{
				gameObject.SetActive(true);
				gameObject2.SetActive(false);
			}
		}
		if (m_adversarySet != null)
		{
			GameObject gameObject3 = GameObjectUtil.FindChildGameObject(m_adversarySet, "duel_win_set");
			GameObject gameObject4 = GameObjectUtil.FindChildGameObject(m_adversarySet, "duel_lose_set");
			if (winOrLose <= 0 || m_battleData.rivalBattleData == null || string.IsNullOrEmpty(m_battleData.rivalBattleData.userId))
			{
				gameObject3.SetActive(false);
				gameObject4.SetActive(false);
			}
			else if (winOrLose == 1)
			{
				gameObject3.SetActive(true);
				gameObject4.SetActive(false);
			}
			else
			{
				gameObject3.SetActive(false);
				gameObject4.SetActive(true);
			}
		}
		if (m_result != null)
		{
			if (winOrLose <= 0)
			{
				m_result.text = string.Empty;
			}
			else if (winOrLose == 4 && !GeneralUtil.IsOverTime(m_battleData.endTime))
			{
				m_result.text = TextUtility.GetText(TextManager.TextType.TEXTTYPE_COMMON_TEXT, "DailyMission", "battle_result_still");
			}
			else if (winOrLose == 1)
			{
				m_result.text = TextUtility.GetText(TextManager.TextType.TEXTTYPE_COMMON_TEXT, "DailyMission", "battle_result_lose");
			}
			else
			{
				m_result.text = TextUtility.GetText(TextManager.TextType.TEXTTYPE_COMMON_TEXT, "DailyMission", "battle_result_win");
			}
		}
		if (m_winFlag == 0)
		{
			m_isOpenEffectStart = true;
			m_isOpenEffectIssue = true;
			m_isOpenEffectEnd = true;
			if (m_mineData != null)
			{
				m_mineData.UpdateViewScore(-1L);
			}
			if (m_adversaryData != null)
			{
				m_adversaryData.UpdateViewScore(-1L);
			}
			if (m_result != null)
			{
				m_result.text = TextUtility.GetText(TextManager.TextType.TEXTTYPE_COMMON_TEXT, "DailyMission", "battle_result_failure");
			}
		}
	}

	public void OnOpen()
	{
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
			ActiveAnimation activeAnimation = ActiveAnimation.Play(m_animation, "ui_cmn_window_Anim2", Direction.Reverse);
			if (activeAnimation != null)
			{
				EventDelegate.Add(activeAnimation.onFinished, OnFinished, true);
			}
		}
	}
}
