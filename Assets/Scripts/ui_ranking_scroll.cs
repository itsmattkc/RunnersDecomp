using Text;
using UnityEngine;

public class ui_ranking_scroll : MonoBehaviour
{
	public const float IMAGE_DELAY = 0.2f;

	private static float s_updateLastTime;

	[SerializeField]
	private UITexture m_faceTexture;

	[SerializeField]
	private UISprite m_rankingSprite;

	[SerializeField]
	private UILabel m_rankingLabel;

	[SerializeField]
	private UISprite m_scoreRankIconSprite;

	[SerializeField]
	private UISprite m_friendIconSprite;

	[SerializeField]
	private UISprite m_charaSprite;

	[SerializeField]
	private UISprite m_mainChaoIcon;

	[SerializeField]
	private UISprite m_subChaoIcon;

	[SerializeField]
	private UISprite m_chao1BgSprite;

	[SerializeField]
	private UISprite m_chao2BgSprite;

	[SerializeField]
	private UILabel m_nameLabel;

	[SerializeField]
	private UILabel m_scodeLabel;

	[SerializeField]
	private UIDragPanelContents m_dragPanelContents;

	[SerializeField]
	private TweenColor[] m_myRanker = new TweenColor[2];

	[SerializeField]
	private UISprite m_leagueIcon;

	[SerializeField]
	private UISprite m_leagueIcon2;

	private RankingUtil.RankingScoreType m_scoreType;

	private RankingUtil.RankingRankerType m_rankerType;

	private RankingUtil.Ranker m_ranker;

	private UILabel m_lvLabel;

	private bool m_myCell;

	private bool m_isImgLoad;

	private bool m_sendBtnDisable;

	private int m_startCount;

	private BoxCollider m_boxCollider;

	private ui_ranking_scroll_dummy m_dummy;

	private float m_updTime;

	private bool m_isDraw;

	private bool m_isDrawChao;

	private float m_chaoDrawDelay;

	private float m_nfPanelTime;

	private GameObject m_rankingSet;

	private UITexture m_mainChaoTex;

	private UITexture m_subChaoTex;

	private float m_mainChaoTexTime;

	private float m_subChaoTexTime;

	public bool SendButtonDisable
	{
		get
		{
			return m_sendBtnDisable;
		}
		set
		{
			m_sendBtnDisable = value;
		}
	}

	public long rankerScore
	{
		get
		{
			long result = 0L;
			if (m_ranker != null)
			{
				result = m_ranker.score;
			}
			return result;
		}
	}

	private void Start()
	{
	}

	public void Update()
	{
		if (m_startCount < 30)
		{
			m_startCount++;
			if (m_dragPanelContents != null && m_dragPanelContents.draggablePanel != null && !string.IsNullOrEmpty(m_dragPanelContents.draggablePanel.name) && m_dragPanelContents.draggablePanel.name.IndexOf("mainmenu_contents") != -1)
			{
				m_dragPanelContents.draggablePanel = null;
			}
		}
		float deltaTime = Time.deltaTime;
		if (m_chaoDrawDelay > 0f)
		{
			if (IsDraw())
			{
				m_chaoDrawDelay -= deltaTime;
				if (m_chaoDrawDelay <= 0f && m_ranker != null)
				{
					m_mainChaoTexTime = -1f;
					m_subChaoTexTime = -1f;
					if (m_ranker.mainChaoId >= 0)
					{
						SingletonGameObject<RankingManager>.Instance.GetChaoTexture(m_ranker.mainChaoId, m_mainChaoTex);
						m_mainChaoTexTime = 0f;
					}
					if (m_ranker.subChaoId >= 0)
					{
						SingletonGameObject<RankingManager>.Instance.GetChaoTexture(m_ranker.subChaoId, m_subChaoTex);
						m_subChaoTexTime = 0f;
					}
					m_chaoDrawDelay = 0f;
					if (m_mainChaoTexTime < 0f && m_subChaoTexTime < 0f)
					{
						m_isDrawChao = true;
					}
				}
			}
		}
		else if (!m_isDrawChao)
		{
			if (m_mainChaoIcon != null && m_mainChaoIcon.gameObject.activeSelf && m_mainChaoTex != null && m_mainChaoTexTime >= 0f)
			{
				if (m_mainChaoTex.alpha > 0f)
				{
					m_mainChaoIcon.gameObject.SetActive(false);
					m_mainChaoTexTime = -1f;
				}
				else
				{
					m_mainChaoTexTime += deltaTime;
					if (m_mainChaoTexTime > 0.75f)
					{
						if (m_ranker.mainChaoId >= 0)
						{
							SingletonGameObject<RankingManager>.Instance.GetChaoTexture(m_ranker.mainChaoId, m_mainChaoTex);
							m_mainChaoTexTime = 0f;
						}
						else
						{
							m_mainChaoTexTime = -1f;
						}
					}
				}
			}
			if (m_subChaoIcon != null && m_subChaoIcon.gameObject.activeSelf && m_subChaoTex != null && m_subChaoTexTime >= 0f)
			{
				if (m_subChaoTex.alpha > 0f)
				{
					m_subChaoIcon.gameObject.SetActive(false);
					m_subChaoTexTime = -1f;
				}
				else
				{
					m_subChaoTexTime += deltaTime;
					if (m_subChaoTexTime > 0.75f)
					{
						if (m_ranker.subChaoId >= 0)
						{
							SingletonGameObject<RankingManager>.Instance.GetChaoTexture(m_ranker.subChaoId, m_subChaoTex);
							m_subChaoTexTime = 0f;
						}
						else
						{
							m_subChaoTexTime = -1f;
						}
					}
				}
			}
			if (m_mainChaoTexTime < 0f && m_subChaoTexTime < 0f)
			{
				m_isDrawChao = true;
			}
		}
		if (m_dummy != null)
		{
			if (m_updTime <= 0f)
			{
				m_boxCollider.enabled = !m_dummy.IsCreating(0f);
				m_updTime = 0.15f;
			}
			m_updTime -= deltaTime;
		}
	}

	private bool IsDraw()
	{
		bool result = false;
		if (m_dragPanelContents != null)
		{
			if (m_dragPanelContents.draggablePanel != null && m_dragPanelContents.draggablePanel.panel != null)
			{
				if (!m_isDraw)
				{
					Vector3 localPosition = m_dragPanelContents.draggablePanel.panel.transform.localPosition;
					float num = localPosition.y * -1f;
					Vector4 clipRange = m_dragPanelContents.draggablePanel.panel.clipRange;
					float w = clipRange.w;
					float num2 = num - w;
					Vector3 localPosition2 = base.gameObject.transform.localPosition;
					float y = localPosition2.y;
					if (y > num2)
					{
						m_isDraw = true;
					}
				}
				result = m_isDraw;
				m_nfPanelTime = 0f;
			}
			else
			{
				m_nfPanelTime += Time.deltaTime;
				if (m_nfPanelTime > 0.5f)
				{
					m_isDraw = true;
					result = m_isDraw;
				}
			}
		}
		return result;
	}

	public void UpdateViewAsync(RankingUtil.RankingScoreType scoreType, RankingUtil.RankingRankerType rankerType, RankingUtil.Ranker ranker, bool end, bool myCell, float delay, ui_ranking_scroll_dummy target)
	{
		m_isImgLoad = false;
		m_myCell = false;
		s_updateLastTime = Time.realtimeSinceStartup;
		m_updTime = 0f;
		m_dummy = target;
		if (m_boxCollider == null)
		{
			m_boxCollider = base.gameObject.GetComponentInChildren<BoxCollider>();
		}
		if (m_boxCollider != null)
		{
			m_boxCollider.enabled = false;
		}
		SetMyRanker(myCell);
		UpdateView(scoreType, rankerType, ranker, end);
		if (target != null)
		{
			target.SetMask(0.98f);
		}
	}

	public void DrawClear()
	{
	}

	public void UpdateViewScore(long score)
	{
		if (m_scodeLabel != null)
		{
			if (score >= 0)
			{
				m_scodeLabel.text = HudUtility.GetFormatNumString(score);
			}
			else if (m_ranker != null)
			{
				m_scodeLabel.text = HudUtility.GetFormatNumString(m_ranker.score);
			}
		}
	}

	public void UpdateViewRank(int rank)
	{
		if (rank > 0)
		{
			if (rank <= 3)
			{
				m_rankingLabel.alpha = 0f;
				m_rankingSprite.gameObject.SetActive(true);
				m_rankingSprite.spriteName = "ui_ranking_scroll_num_" + rank;
				m_rankingSprite.alpha = 1f;
			}
			else
			{
				m_rankingSprite.alpha = 0f;
				m_rankingLabel.gameObject.SetActive(true);
				m_rankingLabel.text = rank.ToString();
				m_rankingLabel.alpha = 1f;
			}
			m_scoreRankIconSprite.spriteName = "ui_ranking_scroll_crown_" + (rank - 1);
		}
		else
		{
			if (m_ranker.rankIndex < 3)
			{
				m_rankingLabel.alpha = 0f;
				m_rankingSprite.gameObject.SetActive(true);
				m_rankingSprite.spriteName = "ui_ranking_scroll_num_" + (m_ranker.rankIndex + 1);
				m_rankingSprite.alpha = 1f;
			}
			else
			{
				m_rankingSprite.alpha = 0f;
				m_rankingLabel.gameObject.SetActive(true);
				m_rankingLabel.text = (m_ranker.rankIndex + 1).ToString();
				m_rankingLabel.alpha = 1f;
			}
			m_scoreRankIconSprite.spriteName = "ui_ranking_scroll_crown_" + m_ranker.rankIconIndex;
		}
	}

	public void UpdateView(RankingUtil.RankingScoreType scoreType, RankingUtil.RankingRankerType rankerType, RankingUtil.Ranker ranker, bool end)
	{
		m_isDraw = false;
		m_isDrawChao = false;
		m_chaoDrawDelay = 0f;
		m_nfPanelTime = 0f;
		m_sendBtnDisable = false;
		m_isImgLoad = false;
		m_updTime = 0f;
		m_startCount = 0;
		base.enabled = true;
		if (m_boxCollider == null)
		{
			m_boxCollider = base.gameObject.GetComponentInChildren<BoxCollider>();
		}
		if (m_boxCollider != null)
		{
			m_boxCollider.enabled = ranker.isBoxCollider;
		}
		if (m_rankingSet == null)
		{
			m_rankingSet = GameObjectUtil.FindChildGameObject(base.gameObject, "ranking_set");
		}
		if (m_rankingSet != null)
		{
			m_rankingSet.SetActive(true);
		}
		m_lvLabel = GameObjectUtil.FindChildGameObjectComponent<UILabel>(base.gameObject, "Lbl_player_lv");
		m_scoreType = scoreType;
		m_rankerType = rankerType;
		m_ranker = ranker;
		if (m_mainChaoTex == null)
		{
			m_mainChaoTex = GameObjectUtil.FindChildGameObjectComponent<UITexture>(base.gameObject, "img_chao_main_icon");
		}
		if (m_subChaoTex == null)
		{
			m_subChaoTex = GameObjectUtil.FindChildGameObjectComponent<UITexture>(base.gameObject, "img_chao_sub_icon");
		}
		if (m_mainChaoTex != null)
		{
			m_mainChaoTex.enabled = (m_ranker.mainChaoId >= 0);
		}
		if (m_subChaoTex != null)
		{
			m_subChaoTex.enabled = (m_ranker.subChaoId >= 0);
		}
		if (m_ranker.mainChaoId >= 0)
		{
			m_chaoDrawDelay = 0.2f;
			m_mainChaoTex.alpha = 0f;
		}
		if (m_ranker.subChaoId >= 0)
		{
			m_chaoDrawDelay = 0.2f;
			m_subChaoTex.alpha = 0f;
		}
		if (m_chaoDrawDelay <= 0f)
		{
			m_isDrawChao = true;
		}
		if (m_mainChaoIcon != null)
		{
			if (m_ranker.mainChaoId >= 0)
			{
				m_mainChaoIcon.gameObject.SetActive(true);
			}
			else
			{
				m_mainChaoIcon.gameObject.SetActive(false);
			}
		}
		if (m_subChaoIcon != null)
		{
			if (m_ranker.subChaoId >= 0)
			{
				m_subChaoIcon.gameObject.SetActive(true);
			}
			else
			{
				m_subChaoIcon.gameObject.SetActive(false);
			}
		}
		m_faceTexture.alpha = 0f;
		LoadImage();
		if (ranker.userDataType != RankingUtil.UserDataType.DAILY_BATTLE)
		{
			if (ranker.rankIndex < 3)
			{
				m_rankingSprite.spriteName = "ui_ranking_scroll_num_" + (ranker.rankIndex + 1);
			}
			else
			{
				m_rankingLabel.text = (ranker.rankIndex + 1).ToString();
			}
			m_rankingSprite.alpha = ((ranker.rankIndex >= 3) ? 0f : 1f);
			m_rankingLabel.alpha = ((ranker.rankIndex >= 3) ? 1f : 0f);
		}
		else
		{
			string text = TextUtility.GetText(TextManager.TextType.TEXTTYPE_COMMON_TEXT, "DailyMission", "battle_continuous_win");
			if (ranker.rankIndex > 1)
			{
				m_rankingLabel.text = text.Replace("{PARAM}", ranker.rankIndex.ToString());
			}
			else
			{
				m_rankingLabel.text = string.Empty;
				if (m_rankingSet != null)
				{
					m_rankingSet.SetActive(false);
				}
			}
			m_rankingSprite.alpha = 0f;
			m_rankingLabel.alpha = 1f;
		}
		m_rankingSprite.gameObject.SetActive(true);
		m_rankingLabel.gameObject.SetActive(true);
		if (m_lvLabel != null)
		{
			string text2 = TextManager.GetText(TextManager.TextType.TEXTTYPE_COMMON_TEXT, "MainMenu", "ui_LevelNumber").text;
			m_lvLabel.text = text2.Replace("{PARAM}", ranker.charaLevel.ToString());
		}
		if (ranker.userDataType != RankingUtil.UserDataType.DAILY_BATTLE)
		{
			m_scoreRankIconSprite.spriteName = "ui_ranking_scroll_crown_" + ranker.rankIconIndex;
		}
		else
		{
			m_scoreRankIconSprite.spriteName = string.Empty;
		}
		m_friendIconSprite.spriteName = RankingUtil.GetFriendIconSpriteName(ranker);
		if (ranker.charaType != CharaType.UNKNOWN)
		{
			m_charaSprite.spriteName = "ui_tex_player_set_" + CharaTypeUtil.GetCharaSpriteNameSuffix(ranker.charaType);
		}
		if (m_chao1BgSprite != null)
		{
			if (ranker.mainChaoId == -1)
			{
				m_chao1BgSprite.gameObject.SetActive(false);
			}
			else
			{
				m_chao1BgSprite.gameObject.SetActive(true);
				m_chao1BgSprite.spriteName = "ui_ranking_scroll_char_bg_" + ranker.mainChaoRarity;
			}
		}
		if (m_chao2BgSprite != null)
		{
			if (ranker.subChaoId == -1)
			{
				m_chao2BgSprite.gameObject.SetActive(false);
			}
			else
			{
				m_chao2BgSprite.gameObject.SetActive(true);
				m_chao2BgSprite.spriteName = "ui_ranking_scroll_char_bg_S" + ranker.subChaoRarity;
			}
		}
		m_nameLabel.text = ranker.userName;
		m_scodeLabel.text = HudUtility.GetFormatNumString(ranker.score);
		m_leagueIcon.spriteName = RankingUtil.GetLeagueIconName((LeagueType)ranker.leagueIndex);
		m_leagueIcon2.spriteName = RankingUtil.GetLeagueIconName2((LeagueType)ranker.leagueIndex);
	}

	public void UpdateViewForRaidbossDesired(ServerEventRaidBossDesiredState desiredState)
	{
		m_isDraw = false;
		m_isDrawChao = false;
		m_chaoDrawDelay = 0f;
		m_nfPanelTime = 0f;
		m_isImgLoad = false;
		m_updTime = 0f;
		m_startCount = 0;
		base.enabled = true;
		m_lvLabel = GameObjectUtil.FindChildGameObjectComponent<UILabel>(base.gameObject, "Lbl_player_lv");
		m_scoreType = RankingUtil.RankingScoreType.HIGH_SCORE;
		m_rankerType = RankingUtil.RankingRankerType.ALL;
		RaidBossUser raidBossUser = new RaidBossUser(desiredState);
		m_ranker = raidBossUser.ConvertRankerData();
		if (m_mainChaoTex == null)
		{
			m_mainChaoTex = GameObjectUtil.FindChildGameObjectComponent<UITexture>(base.gameObject, "img_chao_main_icon");
		}
		if (m_subChaoTex == null)
		{
			m_subChaoTex = GameObjectUtil.FindChildGameObjectComponent<UITexture>(base.gameObject, "img_chao_sub_icon");
		}
		if (m_mainChaoTex != null)
		{
			m_mainChaoTex.enabled = (m_ranker.mainChaoId >= 0);
		}
		if (m_subChaoTex != null)
		{
			m_subChaoTex.enabled = (m_ranker.subChaoId >= 0);
		}
		if (m_ranker.mainChaoId >= 0)
		{
			m_chaoDrawDelay = 0.2f;
			m_mainChaoTex.alpha = 0f;
		}
		if (m_ranker.subChaoId >= 0)
		{
			m_chaoDrawDelay = 0.2f;
			m_subChaoTex.alpha = 0f;
		}
		if (m_chaoDrawDelay <= 0f)
		{
			m_isDrawChao = true;
		}
		if (m_mainChaoIcon != null)
		{
			if (m_ranker.mainChaoId >= 0)
			{
				m_mainChaoIcon.gameObject.SetActive(true);
			}
			else
			{
				m_mainChaoIcon.gameObject.SetActive(false);
			}
		}
		if (m_subChaoIcon != null)
		{
			if (m_ranker.subChaoId >= 0)
			{
				m_subChaoIcon.gameObject.SetActive(true);
			}
			else
			{
				m_subChaoIcon.gameObject.SetActive(false);
			}
		}
		m_faceTexture.alpha = 0f;
		LoadImage();
		if (m_lvLabel != null)
		{
			string text = TextManager.GetText(TextManager.TextType.TEXTTYPE_COMMON_TEXT, "MainMenu", "ui_LevelNumber").text;
			m_lvLabel.text = text.Replace("{PARAM}", m_ranker.charaLevel.ToString());
		}
		m_friendIconSprite.spriteName = RankingUtil.GetFriendIconSpriteName(m_ranker);
		if (m_ranker.charaType != CharaType.UNKNOWN)
		{
			if (AtlasManager.Instance != null)
			{
				AtlasManager.Instance.ReplacePlayerAtlasForRaidResult(m_charaSprite.atlas);
			}
			m_charaSprite.spriteName = "ui_tex_player_set_" + CharaTypeUtil.GetCharaSpriteNameSuffix(m_ranker.charaType);
		}
		if (m_chao1BgSprite != null)
		{
			if (m_ranker.mainChaoId == -1)
			{
				m_chao1BgSprite.gameObject.SetActive(false);
			}
			else
			{
				m_chao1BgSprite.gameObject.SetActive(true);
				m_chao1BgSprite.spriteName = "ui_ranking_scroll_char_bg_" + m_ranker.mainChaoRarity;
			}
		}
		if (m_chao2BgSprite != null)
		{
			if (m_ranker.subChaoId == -1)
			{
				m_chao2BgSprite.gameObject.SetActive(false);
			}
			else
			{
				m_chao2BgSprite.gameObject.SetActive(true);
				m_chao2BgSprite.spriteName = "ui_ranking_scroll_char_bg_S" + m_ranker.subChaoRarity;
			}
		}
		GameObject gameObject = GameObjectUtil.FindChildGameObject(base.gameObject, "ranking_set");
		if (gameObject != null)
		{
			gameObject.SetActive(false);
		}
		GameObject gameObject2 = GameObjectUtil.FindChildGameObject(base.gameObject, "event_ui");
		if (gameObject2 != null)
		{
			gameObject2.SetActive(true);
			UILabel uILabel = GameObjectUtil.FindChildGameObjectComponent<UILabel>(gameObject2, "Lbl_event_score");
			if (uILabel != null)
			{
				uILabel.text = HudUtility.GetFormatNumString((long)desiredState.NumBeatedEnterprise);
			}
		}
		m_scodeLabel.gameObject.SetActive(false);
		m_nameLabel.text = m_ranker.userName;
		m_leagueIcon.spriteName = RankingUtil.GetLeagueIconName((LeagueType)m_ranker.leagueIndex);
		m_leagueIcon2.spriteName = RankingUtil.GetLeagueIconName2((LeagueType)m_ranker.leagueIndex);
		UIButtonOffset uIButtonOffset = GameObjectUtil.FindChildGameObjectComponent<UIButtonOffset>(base.gameObject, "Btn_ranking_top");
		if (uIButtonOffset != null)
		{
			uIButtonOffset.enabled = false;
		}
		UIButtonMessage uIButtonMessage = GameObjectUtil.FindChildGameObjectComponent<UIButtonMessage>(base.gameObject, "Btn_ranking_top");
		if (uIButtonMessage != null)
		{
			uIButtonMessage.enabled = false;
		}
	}

	public void SetMyRanker(bool myCell)
	{
		m_myCell = myCell;
		UISprite uISprite = GameObjectUtil.FindChildGameObjectComponent<UISprite>(base.gameObject, "Btn_ranking_top");
		if (uISprite != null)
		{
			Color color = uISprite.color;
			if (!(color.r < 1f))
			{
				Color color2 = uISprite.color;
				if (!(color2.g < 1f))
				{
					Color color3 = uISprite.color;
					if (!(color3.b < 1f))
					{
						goto IL_008c;
					}
				}
			}
			uISprite.color = new Color(1f, 1f, 1f, 1f);
			goto IL_008c;
		}
		goto IL_00a7;
		IL_00a7:
		for (int i = 0; i < m_myRanker.Length; i++)
		{
			if (m_myRanker[i] != null)
			{
				m_myRanker[i].enabled = myCell;
			}
		}
		if (m_mainChaoTex != null)
		{
			m_mainChaoTex.depth += 50;
		}
		if (m_subChaoTex != null)
		{
			m_subChaoTex.depth += 50;
		}
		if (myCell)
		{
			m_isImgLoad = false;
			LoadImage();
		}
		return;
		IL_008c:
		uISprite.spriteName = ((!myCell) ? "ui_ranking_bg_8" : "ui_ranking_bg_8m");
		goto IL_00a7;
	}

	public void UpdateSendChallenge(string id)
	{
		if (base.gameObject.activeSelf && m_ranker != null && m_ranker.id == id)
		{
			m_ranker.isSentEnergy = true;
			m_friendIconSprite.spriteName = RankingUtil.GetFriendIconSpriteName(m_ranker);
		}
	}

	private void LoadImage()
	{
		if (m_ranker != null && !m_isImgLoad && (m_myCell || m_ranker.isFriend))
		{
			m_faceTexture.mainTexture = RankingUtil.GetProfilePictureTexture(m_ranker, delegate(Texture2D _faceTexture)
			{
				m_faceTexture.mainTexture = _faceTexture;
				m_faceTexture.alpha = 1f;
			});
			if (m_faceTexture.mainTexture != null)
			{
				m_faceTexture.alpha = 1f;
			}
			m_isImgLoad = true;
		}
	}

	private void OnClickRankingScroll()
	{
		if (m_ranker.userDataType != RankingUtil.UserDataType.RANK_UP && Mathf.Abs(s_updateLastTime - Time.realtimeSinceStartup) > 1.5f)
		{
			SoundManager.SePlay("sys_menu_decide");
			ranking_window.Open(base.gameObject.transform.root.gameObject, m_scoreType, m_rankerType, m_ranker, m_sendBtnDisable || m_ranker.isMy);
		}
	}
}
