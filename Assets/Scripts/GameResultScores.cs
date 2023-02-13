using AnimationOrTween;
using Text;
using UnityEngine;

public abstract class GameResultScores : MonoBehaviour
{
	protected enum Category
	{
		CHAO,
		CAMPAIGN,
		CHARA,
		NUM,
		NONE
	}

	private enum EventSignal
	{
		PLAY_START = 100,
		SKIP,
		ALL_SKIP
	}

	private enum EventUpdateState
	{
		Idle,
		Start,
		Wait
	}

	private bool m_debugInfo;

	protected StageScoreManager m_scoreManager;

	protected GameObject m_resultRoot;

	private ResultObjParam[] ResultObjParamTable = new ResultObjParam[7]
	{
		new ResultObjParam("Lbl_score", "Lbl_chao_score", "img_chao_score", "Lbl_player_score", "img_player_score", string.Empty, "Lbl_chaototal_score", "Lbl_player_rank_score1"),
		new ResultObjParam("Lbl_ring", "Lbl_chao_ring", "img_chao_ring", "Lbl_player_ring", "img_player_ring", "Lbl_campaign_ring", string.Empty, string.Empty),
		new ResultObjParam("Lbl_rsring", string.Empty, string.Empty, string.Empty, string.Empty, string.Empty, string.Empty, string.Empty),
		new ResultObjParam("Lbl_animal", "Lbl_chao_animal", "img_chao_animal", "Lbl_player_animal", "img_player_animal", string.Empty, string.Empty, string.Empty),
		new ResultObjParam("Lbl_distance", "Lbl_chao_distance", "img_chao_distance", "Lbl_player_distance", "img_player_distance", string.Empty, string.Empty, string.Empty),
		new ResultObjParam("Lbl_totalscore", string.Empty, string.Empty, string.Empty, string.Empty, string.Empty, string.Empty, string.Empty),
		new ResultObjParam(string.Empty, string.Empty, string.Empty, string.Empty, string.Empty, string.Empty, string.Empty, string.Empty)
	};

	private string[] BonusCategoryData = new string[3]
	{
		"chao_bonus",
		"campaign_bonus",
		"player_bonus"
	};

	private string[] BonusCategoryButtonLabel = new string[4]
	{
		"ui_Lbl_word_chao_bonus",
		"ui_Lbl_word_campaign_bonus",
		"ui_Lbl_word_player_bonus",
		"ui_Lbl_word_bonus_details_button"
	};

	private TinyFsmBehavior m_fsm;

	private Category m_category;

	private GameResultScoreInterporate[] m_scores = new GameResultScoreInterporate[7];

	private BonusEventScore[] m_bonusEventScores = new BonusEventScore[3];

	private BonusEventScore m_chaoCountBonusEventScores = new BonusEventScore();

	private BonusEventScore m_RankBonusEventScores = new BonusEventScore();

	private BonusEventInfo[] m_bonusEventInfos = new BonusEventInfo[3];

	private Animation m_bonusEventAnim;

	private GameObject m_eventRoot;

	private GameObject m_resultObj;

	private bool m_finished;

	private bool m_skip;

	private bool m_allSkip;

	protected bool m_isReplay;

	private float m_timer;

	private EventUpdateState m_eventUpdateState;

	private Category m_addScore;

	private UILabel m_DetailButtonLabel;

	private UILabel m_DetailButtonLabel_Sh;

	protected bool m_isBossResult;

	private bool m_isQuickMode;

	public bool IsEnd
	{
		get
		{
			return m_finished;
		}
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
			m_finished = false;
			m_isReplay = false;
		}
	}

	public void Setup(GameObject resultObj, GameObject resultRoot, GameObject eventRoot)
	{
		DebugScoreLog();
		m_finished = false;
		m_isBossResult = false;
		if (resultObj == null)
		{
			return;
		}
		m_resultObj = resultObj;
		m_scoreManager = StageScoreManager.Instance;
		if (m_scoreManager == null)
		{
			return;
		}
		SaveDataManager instance = SaveDataManager.Instance;
		if (instance == null)
		{
			return;
		}
		StageAbilityManager instance2 = StageAbilityManager.Instance;
		if (instance2 == null || resultRoot == null)
		{
			return;
		}
		m_resultRoot = resultRoot;
		if (eventRoot == null)
		{
			return;
		}
		m_eventRoot = eventRoot;
		if (StageModeManager.Instance != null)
		{
			m_isQuickMode = StageModeManager.Instance.IsQuickMode();
		}
		m_bonusEventAnim = m_eventRoot.GetComponent<Animation>();
		GameObject gameObject = GameObjectUtil.FindChildGameObject(m_resultRoot, "window_result");
		if (gameObject == null)
		{
			return;
		}
		GameObject gameObject2 = GameObjectUtil.FindChildGameObject(m_resultRoot, "window_bonus");
		if (gameObject2 == null)
		{
			return;
		}
		gameObject2.SetActive(true);
		m_DetailButtonLabel = GameObjectUtil.FindChildGameObjectComponent<UILabel>(m_resultRoot, "Lbl_word_bonus_details");
		m_DetailButtonLabel_Sh = GameObjectUtil.FindChildGameObjectComponent<UILabel>(m_resultRoot, "Lbl_word_bonus_details_sh");
		for (int i = 0; i < 3; i++)
		{
			m_bonusEventScores[i] = new BonusEventScore();
			m_bonusEventScores[i].obj = GameObjectUtil.FindChildGameObject(gameObject2, BonusCategoryData[i]);
			if (m_bonusEventScores[i].obj != null)
			{
				m_bonusEventScores[i].obj.SetActive(false);
			}
		}
		m_chaoCountBonusEventScores.obj = GameObjectUtil.FindChildGameObject(gameObject2, "chaototal_bonus");
		if (m_chaoCountBonusEventScores.obj != null)
		{
			m_chaoCountBonusEventScores.obj.SetActive(false);
		}
		m_RankBonusEventScores.obj = GameObjectUtil.FindChildGameObject(gameObject2, "player_bonus");
		if (m_RankBonusEventScores.obj != null)
		{
			m_RankBonusEventScores.obj.SetActive(false);
		}
		for (int j = 0; j < 7; j++)
		{
			UILabel uILabel = GameObjectUtil.FindChildGameObjectComponent<UILabel>(gameObject, ResultObjParamTable[j].scoreLabel);
			if (uILabel != null)
			{
				m_scores[j] = new GameResultScoreInterporate();
				m_scores[j].Setup(uILabel);
				m_scores[j].SetLabelStartValue(0L);
			}
			int num = 0;
			GameObject obj = m_bonusEventScores[num].obj;
			if (obj != null)
			{
				m_bonusEventScores[num].bonusData[j] = new BonusData();
				m_bonusEventScores[num].bonusData[j].labelScore1 = GameObjectUtil.FindChildGameObjectComponent<UILabel>(obj, ResultObjParamTable[j].chaoBonusLabel + "1");
				m_bonusEventScores[num].bonusData[j].uiTexture1 = GameObjectUtil.FindChildGameObjectComponent<UITexture>(obj, ResultObjParamTable[j].chaoBonusTexture + "1");
				m_bonusEventScores[num].bonusData[j].labelScore2 = GameObjectUtil.FindChildGameObjectComponent<UILabel>(obj, ResultObjParamTable[j].chaoBonusLabel + "2");
				m_bonusEventScores[num].bonusData[j].uiTexture2 = GameObjectUtil.FindChildGameObjectComponent<UITexture>(obj, ResultObjParamTable[j].chaoBonusTexture + "2");
				int num2 = (!IsBonusRate(instance2.MainChaoBonusValueRate, (ScoreDataType)j)) ? (-1) : instance.PlayerData.MainChaoID;
				if (num2 >= 0)
				{
					SetActiveBonusEventScore1(num, j, true);
					HudUtility.SetChaoTexture(m_bonusEventScores[num].bonusData[j].uiTexture1, num2, true);
				}
				else
				{
					SetActiveBonusEventScore1(num, j, false);
				}
				int num3 = (!IsBonusRate(instance2.SubChaoBonusValueRate, (ScoreDataType)j)) ? (-1) : instance.PlayerData.SubChaoID;
				if (num3 >= 0)
				{
					SetActiveBonusEventScore2(num, j, true);
					HudUtility.SetChaoTexture(m_bonusEventScores[num].bonusData[j].uiTexture2, num3, true);
				}
				else
				{
					SetActiveBonusEventScore2(num, j, false);
				}
			}
			GameObject obj2 = m_chaoCountBonusEventScores.obj;
			if (obj2 != null)
			{
				m_chaoCountBonusEventScores.bonusData[j] = new BonusData();
				m_chaoCountBonusEventScores.bonusData[j].labelScore1 = GameObjectUtil.FindChildGameObjectComponent<UILabel>(obj2, ResultObjParamTable[j].chaoCountBonusLabel);
				SetActiveBonusEventScore(m_chaoCountBonusEventScores.bonusData[j].labelScore1, IsBonusRate(instance2.CountChaoBonusValueRate, (ScoreDataType)j));
			}
			int num4 = 1;
			GameObject obj3 = m_bonusEventScores[num4].obj;
			if (obj3 != null)
			{
				m_bonusEventScores[num4].bonusData[j] = new BonusData();
				m_bonusEventScores[num4].bonusData[j].labelScore1 = GameObjectUtil.FindChildGameObjectComponent<UILabel>(obj3, ResultObjParamTable[j].campaignBonusLabel);
				SetActiveBonusEventScore(m_bonusEventScores[num4].bonusData[j].labelScore1, IsBonusRate(GameResultUtility.GetCampaignBonusRate(instance2), (ScoreDataType)j));
			}
			int num5 = 2;
			GameObject obj4 = m_bonusEventScores[num5].obj;
			if (!(obj4 != null))
			{
				continue;
			}
			m_bonusEventScores[num5].bonusData[j] = new BonusData();
			m_bonusEventScores[num5].bonusData[j].labelScore1 = GameObjectUtil.FindChildGameObjectComponent<UILabel>(obj4, ResultObjParamTable[j].charaBonusLabel + "1");
			m_bonusEventScores[num5].bonusData[j].uiSprite1 = GameObjectUtil.FindChildGameObjectComponent<UISprite>(obj4, ResultObjParamTable[j].charaBonusTexture + "1");
			m_bonusEventScores[num5].bonusData[j].labelScore2 = GameObjectUtil.FindChildGameObjectComponent<UILabel>(obj4, ResultObjParamTable[j].charaBonusLabel + "2");
			m_bonusEventScores[num5].bonusData[j].uiSprite2 = GameObjectUtil.FindChildGameObjectComponent<UISprite>(obj4, ResultObjParamTable[j].charaBonusTexture + "2");
			CharaType charaType = (!IsBonusRate(instance2.MainCharaBonusValueRate, (ScoreDataType)j)) ? CharaType.UNKNOWN : instance.PlayerData.MainChara;
			if (charaType != CharaType.UNKNOWN)
			{
				SetActiveBonusEventScore1(num5, j, true);
				GameResultUtility.SetCharaTexture(m_bonusEventScores[num5].bonusData[j].uiSprite1, charaType);
			}
			else
			{
				SetActiveBonusEventScore1(num5, j, false);
			}
			CharaType charaType2 = (!IsBonusRate(instance2.SubCharaBonusValueRate, (ScoreDataType)j)) ? CharaType.UNKNOWN : instance.PlayerData.SubChara;
			if (charaType2 != CharaType.UNKNOWN)
			{
				SetActiveBonusEventScore2(num5, j, true);
				GameResultUtility.SetCharaTexture(m_bonusEventScores[num5].bonusData[j].uiSprite2, charaType2);
			}
			else
			{
				SetActiveBonusEventScore2(num5, j, false);
			}
			GameObject obj5 = m_RankBonusEventScores.obj;
			if (!(obj5 != null))
			{
				continue;
			}
			m_RankBonusEventScores.bonusData[j] = new BonusData();
			m_RankBonusEventScores.bonusData[j].labelScore1 = GameObjectUtil.FindChildGameObjectComponent<UILabel>(obj5, ResultObjParamTable[j].charaBonusRankScore);
			SetActiveBonusEventScore(m_RankBonusEventScores.bonusData[j].labelScore1, true);
			if (m_isQuickMode && m_RankBonusEventScores.bonusData[j].labelScore1 != null)
			{
				m_RankBonusEventScores.bonusData[j].labelScore1.gameObject.SetActive(false);
				GameObject gameObject3 = GameObjectUtil.FindChildGameObject(obj5, "player_rank_bonus_title");
				if (gameObject3 != null)
				{
					gameObject3.SetActive(false);
				}
			}
		}
		SetResultScore(m_scoreManager.CountData);
		for (int k = 0; k < 3; k++)
		{
			m_bonusEventInfos[k] = new BonusEventInfo();
		}
		if (m_bonusEventAnim != null)
		{
			int num6 = 0;
			m_bonusEventInfos[num6].obj = GameObjectUtil.FindChildGameObject(m_eventRoot, "chaobonus");
			m_bonusEventInfos[num6].animClipName = "ui_result_word_chaobonus_Anim";
			m_bonusEventInfos[num6].viewTime = m_bonusEventAnim[m_bonusEventInfos[num6].animClipName].length * 0.3f;
			m_bonusEventInfos[num6].valueText = GameObjectUtil.FindChildGameObjectComponent<UILabel>(m_bonusEventInfos[num6].obj, "Lbl_num_chaobonus_score");
			StageScoreManager.ResultData bonusCountMainChaoData = m_scoreManager.BonusCountMainChaoData;
			StageScoreManager.ResultData bonusCountSubChaoData = m_scoreManager.BonusCountSubChaoData;
			StageScoreManager.ResultData bonusCountChaoCountData = m_scoreManager.BonusCountChaoCountData;
			if (IsBonus(bonusCountMainChaoData, bonusCountSubChaoData, bonusCountChaoCountData))
			{
				SetupBonus(bonusCountMainChaoData, bonusCountSubChaoData, ref m_bonusEventScores[num6].bonusData);
				SetupBonus(bonusCountChaoCountData, null, ref m_chaoCountBonusEventScores.bonusData);
			}
			else
			{
				m_bonusEventInfos[num6].obj = null;
			}
			int num7 = 1;
			m_bonusEventInfos[num7].obj = GameObjectUtil.FindChildGameObject(m_eventRoot, "campaignbonus");
			m_bonusEventInfos[num7].animClipName = "ui_result_word_campaignbonus_Anim";
			m_bonusEventInfos[num7].viewTime = m_bonusEventAnim[m_bonusEventInfos[num7].animClipName].length * 0.3f;
			m_bonusEventInfos[num7].valueText = GameObjectUtil.FindChildGameObjectComponent<UILabel>(m_bonusEventInfos[num7].obj, "Lbl_num_campaignbonus_score");
			StageScoreManager.ResultData bonusCountCampaignData = m_scoreManager.BonusCountCampaignData;
			if (IsBonus(bonusCountCampaignData, null, null))
			{
				SetupBonus(bonusCountCampaignData, null, ref m_bonusEventScores[num7].bonusData);
			}
			else
			{
				m_bonusEventInfos[num7].obj = null;
			}
			int num8 = 2;
			m_bonusEventInfos[num8].obj = GameObjectUtil.FindChildGameObject(m_eventRoot, "playerbonus");
			m_bonusEventInfos[num8].animClipName = "ui_result_word_playerbonus_Anim";
			m_bonusEventInfos[num8].viewTime = m_bonusEventAnim[m_bonusEventInfos[num8].animClipName].length * 0.3f;
			m_bonusEventInfos[num8].valueText = GameObjectUtil.FindChildGameObjectComponent<UILabel>(m_bonusEventInfos[num8].obj, "Lbl_num_playerbonus_score");
			StageScoreManager.ResultData bonusCountMainCharaData = m_scoreManager.BonusCountMainCharaData;
			StageScoreManager.ResultData bonusCountSubCharaData = m_scoreManager.BonusCountSubCharaData;
			StageScoreManager.ResultData bonusCountRankData = m_scoreManager.BonusCountRankData;
			if (IsBonus(bonusCountMainCharaData, bonusCountSubCharaData, bonusCountRankData))
			{
				SetupBonus(bonusCountMainCharaData, bonusCountSubCharaData, ref m_bonusEventScores[num8].bonusData);
				SetupBonus(bonusCountRankData, null, ref m_RankBonusEventScores.bonusData);
			}
			else
			{
				m_bonusEventInfos[num8].obj = null;
			}
		}
		m_addScore = Category.NONE;
		OnSetup(resultRoot);
	}

	public bool IsCampaignBonus()
	{
		if (m_bonusEventInfos[1].obj != null)
		{
			return true;
		}
		return false;
	}

	private void SetResultScore(StageScoreManager.ResultData resultDataScore)
	{
		if (m_scores[0] != null)
		{
			m_scores[0].AddScore(resultDataScore.score);
		}
		if (m_scores[1] != null)
		{
			m_scores[1].AddScore(resultDataScore.ring);
		}
		if (m_scores[2] != null)
		{
			m_scores[2].AddScore(resultDataScore.red_ring);
		}
		if (m_scores[3] != null)
		{
			m_scores[3].AddScore(resultDataScore.animal);
		}
		if (m_scores[4] != null)
		{
			m_scores[4].AddScore(resultDataScore.distance);
		}
		if (EventManager.Instance != null && EventManager.Instance.IsRaidBossStage() && m_scores[6] != null)
		{
			m_scores[6].AddScore(resultDataScore.raid_boss_ring);
		}
	}

	private void SetActiveBonusEventScore1(int category, int scoreDataType, bool on)
	{
		if (m_bonusEventScores[category].bonusData[scoreDataType].labelScore1 != null)
		{
			m_bonusEventScores[category].bonusData[scoreDataType].labelScore1.gameObject.SetActive(on);
		}
		if (m_bonusEventScores[category].bonusData[scoreDataType].uiSprite1 != null)
		{
			m_bonusEventScores[category].bonusData[scoreDataType].uiSprite1.gameObject.SetActive(on);
		}
		if (m_bonusEventScores[category].bonusData[scoreDataType].uiTexture1 != null)
		{
			m_bonusEventScores[category].bonusData[scoreDataType].uiTexture1.gameObject.SetActive(on);
		}
	}

	private void SetActiveBonusEventScore2(int category, int scoreDataType, bool on)
	{
		if (m_bonusEventScores[category].bonusData[scoreDataType].labelScore2 != null)
		{
			m_bonusEventScores[category].bonusData[scoreDataType].labelScore2.gameObject.SetActive(on);
		}
		if (m_bonusEventScores[category].bonusData[scoreDataType].uiSprite2 != null)
		{
			m_bonusEventScores[category].bonusData[scoreDataType].uiSprite2.gameObject.SetActive(on);
		}
		if (m_bonusEventScores[category].bonusData[scoreDataType].uiTexture2 != null)
		{
			m_bonusEventScores[category].bonusData[scoreDataType].uiTexture2.gameObject.SetActive(on);
		}
	}

	private void SetActiveBonusEventScore(UILabel uiLavel, bool on)
	{
		if (uiLavel != null)
		{
			uiLavel.gameObject.SetActive(on);
		}
	}

	private void SetupBonus(StageScoreManager.ResultData resultData1, StageScoreManager.ResultData resultData2, ref BonusData[] bonusScore)
	{
		if (bonusScore == null)
		{
			return;
		}
		if (resultData1 != null)
		{
			int num = 0;
			if (bonusScore[num] != null && bonusScore[num].labelScore1 != null)
			{
				bonusScore[num].labelScore1.text = GameResultUtility.GetScoreFormat(resultData1.score);
			}
			int num2 = 1;
			if (bonusScore[num2] != null && bonusScore[num2].labelScore1 != null)
			{
				bonusScore[num2].labelScore1.text = GameResultUtility.GetScoreFormat(resultData1.ring);
			}
			int num3 = 3;
			if (bonusScore[num3] != null && bonusScore[num3].labelScore1 != null)
			{
				bonusScore[num3].labelScore1.text = GameResultUtility.GetScoreFormat(resultData1.animal);
			}
			int num4 = 4;
			if (bonusScore[num4] != null && bonusScore[num4].labelScore1 != null)
			{
				bonusScore[num4].labelScore1.text = GameResultUtility.GetScoreFormat(resultData1.distance);
			}
			if (EventManager.Instance != null && EventManager.Instance.IsRaidBossStage())
			{
				int num5 = 6;
				if (bonusScore[num5] != null && bonusScore[num5].labelScore1 != null)
				{
					bonusScore[num5].labelScore1.text = GameResultUtility.GetScoreFormat(resultData1.raid_boss_ring);
				}
			}
		}
		if (resultData2 == null)
		{
			return;
		}
		int num6 = 0;
		if (bonusScore[num6] != null && bonusScore[num6].labelScore2 != null)
		{
			bonusScore[num6].labelScore2.text = GameResultUtility.GetScoreFormat(resultData2.score);
		}
		int num7 = 1;
		if (bonusScore[num7] != null && bonusScore[num7].labelScore2 != null)
		{
			bonusScore[num7].labelScore2.text = GameResultUtility.GetScoreFormat(resultData2.ring);
		}
		int num8 = 3;
		if (bonusScore[num8] != null && bonusScore[num8].labelScore2 != null)
		{
			bonusScore[num8].labelScore2.text = GameResultUtility.GetScoreFormat(resultData2.animal);
		}
		int num9 = 4;
		if (bonusScore[num9] != null && bonusScore[num9].labelScore2 != null)
		{
			bonusScore[num9].labelScore2.text = GameResultUtility.GetScoreFormat(resultData2.distance);
		}
		if (EventManager.Instance != null && EventManager.Instance.IsRaidBossStage())
		{
			int num10 = 6;
			if (bonusScore[num10] != null && bonusScore[num10].labelScore2 != null)
			{
				bonusScore[num10].labelScore2.text = GameResultUtility.GetScoreFormat(resultData2.raid_boss_ring);
			}
		}
	}

	private bool IsBonusRate(StageAbilityManager.BonusRate bonusRate, ScoreDataType scoreDataType)
	{
		switch (scoreDataType)
		{
		case ScoreDataType.SCORE:
			if (bonusRate.score > 0f)
			{
				return true;
			}
			break;
		case ScoreDataType.RING:
			if (bonusRate.ring > 0f)
			{
				return true;
			}
			break;
		case ScoreDataType.REDSTAR_RING:
			if (bonusRate.red_ring > 0f)
			{
				return true;
			}
			break;
		case ScoreDataType.ANIMAL:
			if (bonusRate.animal > 0f)
			{
				return true;
			}
			break;
		case ScoreDataType.DISTANCE:
			if (bonusRate.distance > 0f)
			{
				return true;
			}
			break;
		case ScoreDataType.RAIDBOSS_RING:
			if (bonusRate.raid_boss_ring > 0f)
			{
				return true;
			}
			break;
		}
		return false;
	}

	protected void SetBonusEventScoreActive(Category category)
	{
		for (int i = 0; i < 3; i++)
		{
			if (m_bonusEventScores[i].obj != null)
			{
				m_bonusEventScores[i].obj.SetActive(category == (Category)i);
			}
		}
		if (m_chaoCountBonusEventScores.obj != null)
		{
			m_chaoCountBonusEventScores.obj.SetActive(Category.CHAO == category);
		}
		if (m_RankBonusEventScores.obj != null)
		{
			m_RankBonusEventScores.obj.SetActive(Category.CHARA == category);
		}
		AddScore(category);
		if (m_scores[5] != null)
		{
			long bonusTotalScore = GetBonusTotalScore(category);
			m_scores[5].SetLabelStartValue(bonusTotalScore);
		}
	}

	private long GetBonusTotalScore(Category category)
	{
		long result = 0L;
		switch (category)
		{
		case Category.CHAO:
			result = m_scoreManager.ResultChaoBonusTotal;
			break;
		case Category.CAMPAIGN:
			result = m_scoreManager.ResultCampaignBonusTotal;
			break;
		case Category.CHARA:
			result = m_scoreManager.ResultPlayerBonusTotal;
			break;
		}
		return result;
	}

	private void AddScore(Category category)
	{
		switch (category)
		{
		case Category.CHAO:
			if (m_addScore == Category.NONE)
			{
				SetResultScore(m_scoreManager.BonusCountMainChaoData);
				SetResultScore(m_scoreManager.BonusCountSubChaoData);
				SetResultScore(m_scoreManager.BonusCountChaoCountData);
				m_addScore = Category.CHAO;
			}
			break;
		case Category.CAMPAIGN:
			if (m_addScore == Category.CHAO)
			{
				SetResultScore(m_scoreManager.BonusCountCampaignData);
				m_addScore = Category.CAMPAIGN;
			}
			break;
		case Category.CHARA:
			if (m_addScore == Category.CAMPAIGN)
			{
				SetResultScore(m_scoreManager.BonusCountMainCharaData);
				SetResultScore(m_scoreManager.BonusCountSubCharaData);
				SetResultScore(m_scoreManager.BonusCountRankData);
				m_addScore = Category.CHARA;
			}
			break;
		}
	}

	public bool IsBonusEvent()
	{
		for (int i = 0; i < 3; i++)
		{
			if (m_bonusEventInfos[i].obj != null)
			{
				return true;
			}
		}
		return false;
	}

	public void PlayStart()
	{
		TinyFsmEvent signal = TinyFsmEvent.CreateUserEvent(100);
		if (m_fsm != null)
		{
			m_fsm.Dispatch(signal);
		}
	}

	public void PlaySkip()
	{
		TinyFsmEvent signal = TinyFsmEvent.CreateUserEvent(101);
		if (m_fsm != null)
		{
			m_fsm.Dispatch(signal);
		}
	}

	public void SetActiveDetailsButton(bool on)
	{
		OnSetActiveDetailsButton(on);
	}

	public void AllSkip()
	{
		TinyFsmEvent signal = TinyFsmEvent.CreateUserEvent(102);
		if (m_fsm != null)
		{
			m_fsm.Dispatch(signal);
		}
	}

	public void OnFinishScore()
	{
		OnFinish();
	}

	private void OnStartBonusScore(Category category)
	{
		GameObject obj = m_bonusEventInfos[(int)category].obj;
		if (m_isReplay)
		{
			if (obj != null)
			{
				obj.SetActive(false);
			}
			m_eventUpdateState = EventUpdateState.Idle;
			Category nextCategory = GetNextCategory(category);
			ReplaceDitailButtonLabel(nextCategory);
			if (m_bonusEventInfos[(int)category].obj != null)
			{
				SetBonusEventScoreActive(category);
			}
			else
			{
				AddScore(category);
			}
		}
		else if (obj != null && m_bonusEventAnim != null)
		{
			obj.SetActive(true);
			m_bonusEventAnim.Rewind(m_bonusEventInfos[(int)category].animClipName);
			if (m_isBossResult)
			{
				m_bonusEventInfos[(int)category].valueText.text = string.Empty;
			}
			else
			{
				m_bonusEventInfos[(int)category].valueText.text = GetBonusTotalScore(category).ToString();
			}
			ActiveAnimation.Play(m_bonusEventAnim, m_bonusEventInfos[(int)category].animClipName, Direction.Forward, false);
			SoundManager.SePlay("sys_bonus");
			m_eventUpdateState = EventUpdateState.Start;
		}
		else
		{
			m_eventUpdateState = EventUpdateState.Idle;
		}
	}

	private Category GetNextCategory(Category nowCategory)
	{
		Category category = Category.CHAO;
		switch (nowCategory)
		{
		case Category.CHAO:
			category = Category.CAMPAIGN;
			break;
		case Category.CAMPAIGN:
			category = Category.CHARA;
			break;
		case Category.CHARA:
			category = Category.NUM;
			break;
		}
		if (category != Category.NUM && m_bonusEventInfos[(int)category].obj == null)
		{
			category = GetNextCategory(category);
		}
		return category;
	}

	private void ReplaceDitailButtonLabel(Category category)
	{
		if (m_DetailButtonLabel != null)
		{
			m_DetailButtonLabel.text = TextManager.GetText(TextManager.TextType.TEXTTYPE_COMMON_TEXT, "Result", BonusCategoryButtonLabel[(int)category]).text;
		}
		if (m_DetailButtonLabel_Sh != null)
		{
			m_DetailButtonLabel_Sh.text = TextManager.GetText(TextManager.TextType.TEXTTYPE_COMMON_TEXT, "Result", BonusCategoryButtonLabel[(int)category]).text;
		}
	}

	private void OnUpdateBonusScore(Category category)
	{
		GameObject obj = m_bonusEventInfos[(int)category].obj;
		if (!(obj != null) || !(m_bonusEventAnim != null))
		{
			return;
		}
		switch (m_eventUpdateState)
		{
		case EventUpdateState.Start:
		{
			float time = m_bonusEventAnim[m_bonusEventInfos[(int)category].animClipName].time;
			if (time > m_bonusEventInfos[(int)category].viewTime)
			{
				SetBonusEventScoreActive(category);
				m_timer = 2f;
				m_eventUpdateState = EventUpdateState.Wait;
			}
			break;
		}
		case EventUpdateState.Wait:
			m_timer -= Time.deltaTime;
			if (m_timer < 0f)
			{
				m_eventUpdateState = EventUpdateState.Idle;
			}
			break;
		}
	}

	private void OnSkipBonusScore(Category category)
	{
		AddScore(category);
	}

	private void OnEndBonusScore(Category category)
	{
		GameObject obj = m_bonusEventInfos[(int)category].obj;
		if (obj != null && m_bonusEventAnim != null)
		{
			SetBonusEventScoreActive(category);
			m_bonusEventAnim.Play();
			float length = m_bonusEventAnim[m_bonusEventInfos[(int)category].animClipName].length;
			m_bonusEventAnim[m_bonusEventInfos[(int)category].animClipName].time = length;
			m_bonusEventAnim.Sample();
			m_bonusEventAnim.Stop();
			if (m_isReplay)
			{
				obj.SetActive(false);
			}
		}
		else
		{
			AddScore(category);
		}
	}

	private bool IsEndBonusScore(Category category)
	{
		if (m_eventUpdateState == EventUpdateState.Idle)
		{
			if (m_bonusEventInfos[(int)category].obj != null && m_isReplay)
			{
				return false;
			}
			return true;
		}
		return false;
	}

	protected void SetEnableNextButton(bool enabled)
	{
		if (m_resultObj != null)
		{
			m_resultObj.SendMessage("OnSetEnableNextButton", enabled);
		}
	}

	protected void SetEnableDetailsButton(bool enabled)
	{
		if (m_resultObj != null)
		{
			m_resultObj.SendMessage("OnSetEnableDetailsButton", enabled);
		}
	}

	protected virtual void OnSetActiveDetailsButton(bool on)
	{
	}

	protected abstract bool IsBonus(StageScoreManager.ResultData data1, StageScoreManager.ResultData data2, StageScoreManager.ResultData data3);

	protected abstract void OnSetup(GameObject resultRoot);

	protected abstract void OnFinish();

	protected virtual void OnStartMileageBonusScore()
	{
	}

	protected virtual void OnUpdateMileageBonusScore()
	{
	}

	protected virtual void OnEndMileageBonusScore()
	{
	}

	protected virtual void OnSkipMileageBonusScore()
	{
	}

	protected virtual bool IsEndMileageBonusScore()
	{
		return true;
	}

	protected virtual void OnStartFinished()
	{
	}

	protected virtual void OnUpdateFinished()
	{
	}

	protected virtual void OnEndFinished()
	{
	}

	protected virtual void OnSkipFinished()
	{
	}

	protected virtual bool IsEndFinished()
	{
		return true;
	}

	protected virtual void OnStartBeginning()
	{
	}

	protected virtual void OnUpdateBeginning()
	{
	}

	protected virtual void OnEndBeginning()
	{
	}

	protected virtual void OnSkipBeginning()
	{
	}

	protected virtual bool IsEndBeginning()
	{
		return true;
	}

	protected virtual void OnScoreInAnimation(EventDelegate.Callback callback)
	{
	}

	protected virtual void OnScoreOutAnimation(EventDelegate.Callback callback)
	{
	}

	public void PlayScoreInAnimation(EventDelegate.Callback callback)
	{
		OnScoreInAnimation(callback);
	}

	public void PlayScoreOutAnimation(EventDelegate.Callback callback)
	{
		OnScoreOutAnimation(callback);
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
			m_skip = false;
			m_allSkip = false;
			m_category = Category.CHAO;
			m_fsm.ChangeState(new TinyFsmState(StateBeginning));
			return TinyFsmState.End();
		default:
			return TinyFsmState.End();
		}
	}

	private TinyFsmState StateBeginning(TinyFsmEvent e)
	{
		switch (e.Signal)
		{
		case -3:
			OnStartBeginning();
			return TinyFsmState.End();
		case -4:
			return TinyFsmState.End();
		case 0:
			OnUpdateBeginning();
			if (IsEndBeginning())
			{
				OnEndBeginning();
				m_fsm.ChangeState(new TinyFsmState(StateBonusUpdate));
			}
			return TinyFsmState.End();
		case 101:
		case 102:
			OnSkipBeginning();
			return TinyFsmState.End();
		default:
			return TinyFsmState.End();
		}
	}

	private TinyFsmState StateBonusUpdate(TinyFsmEvent e)
	{
		switch (e.Signal)
		{
		case -3:
			m_skip = false;
			OnStartBonusScore(m_category);
			return TinyFsmState.End();
		case -4:
			return TinyFsmState.End();
		case 0:
			OnUpdateBonusScore(m_category);
			if (IsEndBonusScore(m_category) || m_skip || m_allSkip)
			{
				OnEndBonusScore(m_category);
				if (m_allSkip)
				{
					m_fsm.ChangeState(new TinyFsmState(StateMileageBonusUpdate));
				}
				else if (m_category == Category.CHAO)
				{
					m_category = Category.CAMPAIGN;
					m_fsm.ChangeState(new TinyFsmState(StateBonusUpdate));
				}
				else if (m_category == Category.CAMPAIGN)
				{
					m_category = Category.CHARA;
					m_fsm.ChangeState(new TinyFsmState(StateBonusUpdate));
				}
				else
				{
					m_fsm.ChangeState(new TinyFsmState(StateMileageBonusUpdate));
				}
			}
			return TinyFsmState.End();
		case 101:
			OnSkipBonusScore(m_category);
			m_skip = true;
			return TinyFsmState.End();
		case 102:
			OnSkipBonusScore(m_category);
			m_allSkip = true;
			return TinyFsmState.End();
		default:
			return TinyFsmState.End();
		}
	}

	private TinyFsmState StateMileageBonusUpdate(TinyFsmEvent e)
	{
		switch (e.Signal)
		{
		case -3:
			AddScore(Category.CHAO);
			AddScore(Category.CAMPAIGN);
			AddScore(Category.CHARA);
			m_skip = false;
			OnStartMileageBonusScore();
			return TinyFsmState.End();
		case -4:
			return TinyFsmState.End();
		case 0:
			OnUpdateMileageBonusScore();
			if (IsEndMileageBonusScore() || m_skip || m_allSkip)
			{
				if (m_allSkip)
				{
					OnSkipMileageBonusScore();
				}
				OnEndMileageBonusScore();
				m_fsm.ChangeState(new TinyFsmState(StateFinished));
			}
			return TinyFsmState.End();
		case 101:
		case 102:
			OnSkipMileageBonusScore();
			m_skip = true;
			m_allSkip = true;
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
			OnStartFinished();
			return TinyFsmState.End();
		case -4:
			return TinyFsmState.End();
		case 0:
			if (!m_finished)
			{
				OnUpdateFinished();
				if (IsEndFinished())
				{
					OnEndFinished();
					m_finished = true;
					m_fsm.ChangeState(new TinyFsmState(StateIdle));
					m_isReplay = true;
					Category nextCategory = GetNextCategory(Category.NUM);
					ReplaceDitailButtonLabel(nextCategory);
				}
			}
			return TinyFsmState.End();
		case 101:
		case 102:
			if (!m_finished)
			{
				OnSkipFinished();
			}
			return TinyFsmState.End();
		default:
			return TinyFsmState.End();
		}
	}

	private void DebugScoreLog()
	{
	}

	private void DebugScoreLogResultData(string msg, StageScoreManager.ResultData resultData)
	{
	}
}
