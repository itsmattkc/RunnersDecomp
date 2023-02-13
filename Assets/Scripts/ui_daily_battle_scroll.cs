using UnityEngine;

public class ui_daily_battle_scroll : MonoBehaviour
{
	public const float IMAGE_DELAY = 0.2f;

	private static float s_updateLastTime;

	[SerializeField]
	private UIButtonMessage m_buttonMessage;

	[SerializeField]
	private UILabel m_date;

	[SerializeField]
	private GameObject m_winSet;

	[SerializeField]
	private GameObject m_loseSet;

	[SerializeField]
	private GameObject m_playerL;

	[SerializeField]
	private GameObject m_playerR;

	[SerializeField]
	private GameObject m_noMatchingL;

	[SerializeField]
	private GameObject m_noMatchingR;

	private GameObject m_nonParticipating;

	private ServerDailyBattleDataPair m_dailyBattleData;

	private RankingUtil.Ranker m_rankerL;

	private RankingUtil.Ranker m_rankerR;

	private bool m_isImgLoad;

	private BoxCollider m_collider;

	private UITexture m_playerTexL;

	private UISprite m_playerLeagueIconMainL;

	private UISprite m_playerLeagueIconSubL;

	private UISprite m_playerFriendIconL;

	private UILabel m_playerScoreL;

	private UILabel m_playerNameL;

	private UITexture m_playerTexR;

	private UISprite m_playerLeagueIconMainR;

	private UISprite m_playerLeagueIconSubR;

	private UISprite m_playerFriendIconR;

	private UILabel m_playerScoreR;

	private UILabel m_playerNameR;

	private void Start()
	{
	}

	public void Update()
	{
	}

	public void DrawClear()
	{
		if (m_playerTexL != null && m_playerTexL.alpha > 0f)
		{
			Object.DestroyImmediate(m_playerTexL.mainTexture);
			m_playerTexL.alpha = 0f;
		}
		if (m_playerTexR != null && m_playerTexR.alpha > 0f)
		{
			Object.DestroyImmediate(m_playerTexR.mainTexture);
			m_playerTexR.alpha = 0f;
		}
		m_isImgLoad = false;
	}

	public void InitSetupObject()
	{
		if (m_buttonMessage != null)
		{
			m_buttonMessage.target = base.gameObject;
			m_buttonMessage.functionName = "OnClickDailyBattelScroll";
		}
		if (m_nonParticipating == null)
		{
			m_nonParticipating = GameObjectUtil.FindChildGameObject(base.gameObject, "duel_lose_default_set");
			if (m_nonParticipating != null)
			{
				m_nonParticipating.SetActive(false);
			}
		}
		if (m_collider == null)
		{
			m_collider = base.gameObject.GetComponent<BoxCollider>();
		}
		if (m_collider != null)
		{
			m_collider.enabled = false;
		}
		if (m_date != null)
		{
			m_date.text = string.Empty;
		}
		if (m_winSet != null)
		{
			m_winSet.SetActive(false);
		}
		if (m_loseSet != null)
		{
			m_loseSet.SetActive(false);
		}
		if (m_noMatchingL != null)
		{
			m_noMatchingL.SetActive(false);
		}
		if (m_noMatchingR != null)
		{
			m_noMatchingR.SetActive(false);
		}
		if (m_playerL != null)
		{
			m_playerTexL = GameObjectUtil.FindChildGameObjectComponent<UITexture>(m_playerL, "img_icon_friends");
			m_playerLeagueIconMainL = GameObjectUtil.FindChildGameObjectComponent<UISprite>(m_playerL, "img_icon_league");
			m_playerLeagueIconSubL = GameObjectUtil.FindChildGameObjectComponent<UISprite>(m_playerL, "img_icon_league_sub");
			m_playerFriendIconL = GameObjectUtil.FindChildGameObjectComponent<UISprite>(m_playerL, "img_friend_icon");
			m_playerScoreL = GameObjectUtil.FindChildGameObjectComponent<UILabel>(m_playerL, "Lbl_score");
			m_playerNameL = GameObjectUtil.FindChildGameObjectComponent<UILabel>(m_playerL, "Lbl_username");
			m_playerL.SetActive(false);
		}
		if (m_playerR != null)
		{
			m_playerTexR = GameObjectUtil.FindChildGameObjectComponent<UITexture>(m_playerR, "img_icon_friends");
			m_playerLeagueIconMainR = GameObjectUtil.FindChildGameObjectComponent<UISprite>(m_playerR, "img_icon_league");
			m_playerLeagueIconSubR = GameObjectUtil.FindChildGameObjectComponent<UISprite>(m_playerR, "img_icon_league_sub");
			m_playerFriendIconR = GameObjectUtil.FindChildGameObjectComponent<UISprite>(m_playerR, "img_friend_icon");
			m_playerScoreR = GameObjectUtil.FindChildGameObjectComponent<UILabel>(m_playerR, "Lbl_score");
			m_playerNameR = GameObjectUtil.FindChildGameObjectComponent<UILabel>(m_playerR, "Lbl_username");
			m_playerR.SetActive(false);
		}
		if (m_playerTexL != null && m_playerTexR != null)
		{
			m_playerTexL.alpha = 0f;
			m_playerTexR.alpha = 0f;
		}
		if (m_playerLeagueIconMainL != null && m_playerLeagueIconSubL != null && m_playerLeagueIconMainR != null && m_playerLeagueIconSubR != null)
		{
			m_playerLeagueIconMainL.spriteName = string.Empty;
			m_playerLeagueIconSubL.spriteName = string.Empty;
			m_playerLeagueIconMainR.spriteName = string.Empty;
			m_playerLeagueIconSubR.spriteName = string.Empty;
		}
		if (m_playerFriendIconL != null && m_playerFriendIconR != null)
		{
			m_playerFriendIconL.spriteName = string.Empty;
			m_playerFriendIconR.spriteName = string.Empty;
		}
		if (m_playerScoreL != null && m_playerNameL != null && m_playerScoreR != null && m_playerNameR != null)
		{
			m_playerScoreL.text = "0";
			m_playerScoreR.text = "0";
			m_playerNameL.text = string.Empty;
			m_playerNameR.text = string.Empty;
		}
	}

	public void UpdateView(ServerDailyBattleDataPair data)
	{
		m_dailyBattleData = data;
		s_updateLastTime = Time.realtimeSinceStartup;
		base.enabled = true;
		InitSetupObject();
		m_rankerL = null;
		m_rankerR = null;
		if (m_dailyBattleData != null)
		{
			if (m_collider != null && !m_dailyBattleData.isDummyData)
			{
				m_collider.enabled = true;
			}
			if (m_date != null)
			{
				if (!m_dailyBattleData.isDummyData && m_dailyBattleData.winFlag > 0)
				{
					m_date.text = m_dailyBattleData.starDateString;
					if (m_nonParticipating != null)
					{
						m_nonParticipating.SetActive(false);
					}
				}
				else
				{
					string starDateString = m_dailyBattleData.starDateString;
					string dateStringHour = GeneralUtil.GetDateStringHour(m_dailyBattleData.endTime, -24);
					if (starDateString == dateStringHour)
					{
						m_date.text = starDateString;
					}
					else
					{
						m_date.text = starDateString + " - " + dateStringHour;
					}
					if (m_nonParticipating != null)
					{
						m_nonParticipating.SetActive(true);
					}
				}
			}
			if (m_winSet != null && m_loseSet != null)
			{
				if (m_dailyBattleData.winFlag >= 2)
				{
					m_winSet.SetActive(true);
					m_loseSet.SetActive(false);
				}
				else
				{
					m_winSet.SetActive(false);
					m_loseSet.SetActive(true);
				}
			}
			if (m_dailyBattleData.myBattleData != null && !string.IsNullOrEmpty(m_dailyBattleData.myBattleData.userId))
			{
				m_rankerL = new RankingUtil.Ranker(m_dailyBattleData.myBattleData);
			}
			if (m_dailyBattleData.rivalBattleData != null && !string.IsNullOrEmpty(m_dailyBattleData.rivalBattleData.userId))
			{
				m_rankerR = new RankingUtil.Ranker(m_dailyBattleData.rivalBattleData);
			}
			if (m_rankerL != null)
			{
				if (m_playerLeagueIconMainL != null && m_playerLeagueIconSubL != null)
				{
					m_playerLeagueIconMainL.spriteName = RankingUtil.GetLeagueIconName((LeagueType)m_rankerL.leagueIndex);
					m_playerLeagueIconSubL.spriteName = RankingUtil.GetLeagueIconName2((LeagueType)m_rankerL.leagueIndex);
				}
				if (m_playerFriendIconL != null)
				{
					m_playerFriendIconL.spriteName = RankingUtil.GetFriendIconSpriteName(m_rankerL);
				}
				if (m_playerScoreL != null && m_playerNameL != null)
				{
					m_playerNameL.text = m_rankerL.userName;
					m_playerScoreL.text = HudUtility.GetFormatNumString(m_rankerL.score);
				}
				m_playerL.SetActive(true);
			}
			else
			{
				m_playerL.SetActive(false);
				if (m_noMatchingL != null)
				{
					m_noMatchingL.SetActive(true);
				}
			}
			if (m_rankerR != null)
			{
				if (m_playerLeagueIconMainR != null && m_playerLeagueIconSubR != null)
				{
					m_playerLeagueIconMainR.spriteName = RankingUtil.GetLeagueIconName((LeagueType)m_rankerR.leagueIndex);
					m_playerLeagueIconSubR.spriteName = RankingUtil.GetLeagueIconName2((LeagueType)m_rankerR.leagueIndex);
				}
				if (m_playerFriendIconR != null)
				{
					m_playerFriendIconR.spriteName = RankingUtil.GetFriendIconSpriteName(m_rankerR);
				}
				if (m_playerScoreR != null && m_playerNameR != null)
				{
					m_playerNameR.text = m_rankerR.userName;
					m_playerScoreR.text = HudUtility.GetFormatNumString(m_rankerR.score);
				}
				m_playerR.SetActive(true);
			}
			else
			{
				m_playerR.SetActive(false);
				if (m_noMatchingR != null)
				{
					m_noMatchingR.SetActive(true);
				}
			}
		}
		LoadImage();
	}

	private void LoadImage()
	{
		if (!m_isImgLoad)
		{
			if (m_rankerL != null)
			{
				LoadUserFaceTexture(m_rankerL, m_playerTexL);
			}
			if (m_rankerR != null)
			{
				LoadUserFaceTexture(m_rankerR, m_playerTexR);
			}
		}
		m_isImgLoad = true;
	}

	private void LoadUserFaceTexture(RankingUtil.Ranker ranker, UITexture uiTex)
	{
		if (ranker != null && (ranker.isFriend || ranker.isMy) && uiTex != null)
		{
			uiTex.mainTexture = RankingUtil.GetProfilePictureTexture(ranker, delegate(Texture2D _faceTexture)
			{
				uiTex.mainTexture = _faceTexture;
				uiTex.alpha = 1f;
			});
			if (uiTex.mainTexture != null)
			{
				uiTex.alpha = 1f;
			}
		}
	}

	private void OnClickDailyBattelScroll()
	{
		if (m_dailyBattleData != null && Mathf.Abs(s_updateLastTime - Time.realtimeSinceStartup) > 1f)
		{
			SoundManager.SePlay("sys_menu_decide");
			DailyBattleDetailWindow.Open(m_dailyBattleData);
		}
	}
}
