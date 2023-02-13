using AnimationOrTween;
using Message;
using System;
using System.Collections.Generic;
using System.Diagnostics;
using Text;
using UnityEngine;

public class ranking_window : WindowBase
{
	private static bool s_rankingWindowActive;

	[SerializeField]
	private Animation m_windowAnimation;

	[SerializeField]
	private GameObject[] m_scoreTypeObjects = new GameObject[2];

	[SerializeField]
	private UISprite m_rankingSprite;

	[SerializeField]
	private UILabel m_rankingLabel;

	[SerializeField]
	private UITexture m_faceTexture;

	[SerializeField]
	private UILabel m_userIdLabel;

	[SerializeField]
	private UISprite m_friendIconSprite;

	[SerializeField]
	private UILabel m_userNameLabel;

	[SerializeField]
	private UILabel m_mapRankLabel;

	[SerializeField]
	private UILabel m_daysLabel;

	[SerializeField]
	private UILabel m_charaNameLabel;

	[SerializeField]
	private UILabel m_charaLevelLabel;

	[SerializeField]
	private UISprite m_charaSprite;

	[SerializeField]
	private UISprite m_subCharaSprite;

	[SerializeField]
	private UISprite[] m_ChaoIconSprite = new UISprite[2];

	[SerializeField]
	private UISprite[] m_chaoBgSprite = new UISprite[2];

	[SerializeField]
	private UILabel[] m_chaoLevelLabel = new UILabel[2];

	[SerializeField]
	private UIImageButton m_sendButton;

	[SerializeField]
	private UISprite m_leagueIcon;

	[SerializeField]
	private UISprite m_leagueIcon2;

	private RankingUtil.RankingScoreType m_scoreType;

	private RankingUtil.RankingRankerType m_rankerType;

	private RankingUtil.Ranker m_ranker;

	private UITexture[] m_chaoIconTex = new UITexture[2];

	private UITexture m_mainCharaTex;

	private UITexture m_subCharaTex;

	public static bool isActive
	{
		get
		{
			return s_rankingWindowActive;
		}
	}

	private void Awake()
	{
		s_rankingWindowActive = false;
		if (m_charaSprite != null)
		{
			m_charaSprite.gameObject.SetActive(false);
		}
		if (m_subCharaSprite != null)
		{
			m_subCharaSprite.gameObject.SetActive(false);
		}
		GameObject gameObject = GameObjectUtil.FindChildGameObject(base.gameObject, "window_row_2");
		if (gameObject != null)
		{
			GameObject gameObject2 = GameObjectUtil.FindChildGameObject(gameObject, "img_main_charicon_tex");
			if (gameObject2 != null)
			{
				m_mainCharaTex = gameObject2.GetComponent<UITexture>();
			}
			GameObject gameObject3 = GameObjectUtil.FindChildGameObject(gameObject, "img_sub_charicon_tex");
			if (gameObject3 != null)
			{
				m_subCharaTex = gameObject3.GetComponent<UITexture>();
			}
		}
		m_chaoIconTex[0] = GameObjectUtil.FindChildGameObjectComponent<UITexture>(gameObject, "img_chao_icon_main");
		m_chaoIconTex[1] = GameObjectUtil.FindChildGameObjectComponent<UITexture>(gameObject, "img_chao_icon_sub");
	}

	private void Update()
	{
		if (GeneralWindow.IsCreated("SendChallenge") && GeneralWindow.IsButtonPressed)
		{
			GeneralWindow.Close();
		}
	}

	public void Open(RankingUtil.RankingScoreType scoreType, RankingUtil.RankingRankerType rankerType, RankingUtil.Ranker ranker, bool sendBtnDisable)
	{
		BackKeyManager.AddWindowCallBack(base.gameObject);
		SoundManager.SePlay("sys_window_open");
		base.gameObject.SetActive(true);
		s_rankingWindowActive = true;
		ActiveAnimation.Play(m_windowAnimation, "ui_menu_ranking_window_Anim", Direction.Forward, EnableCondition.DoNothing, DisableCondition.DoNotDisable);
		UpdateView(scoreType, rankerType, ranker, sendBtnDisable);
	}

	public void Open(RaidBossUser user)
	{
		BackKeyManager.AddWindowCallBack(base.gameObject);
		SoundManager.SePlay("sys_window_open");
		base.gameObject.SetActive(true);
		s_rankingWindowActive = true;
		ActiveAnimation.Play(m_windowAnimation, "ui_menu_ranking_window_Anim", Direction.Forward, EnableCondition.DoNothing, DisableCondition.DoNotDisable);
		UpdateView(user);
	}

	private void OnClose()
	{
		s_rankingWindowActive = false;
		SoundManager.SePlay("sys_window_close");
	}

	private void UpdateView()
	{
		bool sendBtnDisable = false;
		UpdateView(m_scoreType, m_rankerType, m_ranker, sendBtnDisable);
	}

	public void UpdateView(RankingUtil.RankingScoreType scoreType, RankingUtil.RankingRankerType rankerType, RankingUtil.Ranker ranker, bool sendBtnDisable)
	{
		m_scoreType = scoreType;
		m_rankerType = rankerType;
		m_ranker = ranker;
		if (m_ranker == null)
		{
			return;
		}
		m_scoreTypeObjects[0].SetActive(m_scoreType == RankingUtil.RankingScoreType.HIGH_SCORE && ranker.userDataType != RankingUtil.UserDataType.DAILY_BATTLE);
		m_scoreTypeObjects[1].SetActive(m_scoreType == RankingUtil.RankingScoreType.TOTAL_SCORE && ranker.userDataType != RankingUtil.UserDataType.DAILY_BATTLE);
		if (ranker.rankIndex < 3)
		{
			m_rankingSprite.spriteName = "ui_ranking_scroll_num_" + (ranker.rankIndex + 1);
		}
		else
		{
			m_rankingLabel.text = (ranker.rankIndex + 1).ToString();
		}
		m_rankingSprite.gameObject.SetActive(ranker.rankIndex < 3 && ranker.userDataType != RankingUtil.UserDataType.DAILY_BATTLE);
		m_rankingLabel.gameObject.SetActive(ranker.rankIndex >= 3 && ranker.userDataType != RankingUtil.UserDataType.DAILY_BATTLE);
		m_faceTexture.mainTexture = RankingUtil.GetProfilePictureTexture(ranker, delegate(Texture2D _faceTexture)
		{
			m_faceTexture.mainTexture = _faceTexture;
		});
		m_userIdLabel.text = TextManager.GetText(TextManager.TextType.TEXTTYPE_COMMON_TEXT, "Ranking", "ui_Lbl_id").text + " " + ranker.id;
		m_friendIconSprite.spriteName = RankingUtil.GetFriendIconSpriteName(ranker);
		m_userNameLabel.text = ranker.userName;
		m_mapRankLabel.text = TextManager.GetText(TextManager.TextType.TEXTTYPE_COMMON_TEXT, "MainMenu", "ui_Lbl_word_rank").text + " " + ranker.dispMapRank;
		m_leagueIcon.spriteName = RankingUtil.GetLeagueIconName((LeagueType)ranker.leagueIndex);
		m_leagueIcon2.spriteName = RankingUtil.GetLeagueIconName2((LeagueType)ranker.leagueIndex);
		TimeSpan timeSpan = NetUtil.GetCurrentTime() - ranker.loginTime;
		m_daysLabel.text = TextUtility.Replaces(TextManager.GetText(TextManager.TextType.TEXTTYPE_COMMON_TEXT, "Ranking", (timeSpan.Days > 0) ? "login_days" : ((timeSpan.Hours <= 0) ? "login_minutes" : "login_hours")).text, new Dictionary<string, string>
		{
			{
				"{DAYS}",
				timeSpan.Days.ToString()
			},
			{
				"{HOURS}",
				timeSpan.Hours.ToString()
			},
			{
				"{MINUTES}",
				timeSpan.Minutes.ToString()
			}
		});
		m_charaNameLabel.text = TextManager.GetText(TextManager.TextType.TEXTTYPE_COMMON_TEXT, "CharaName", CharaName.Name[(int)ranker.charaType]).text;
		m_charaLevelLabel.text = TextUtility.GetTextLevel(ranker.charaLevel.ToString("D3"));
		SetChara(m_mainCharaTex, ranker.charaType);
		SetChara(m_subCharaTex, ranker.charaSubType);
		SetChao(0, ranker.mainChaoId, ranker.mainChaoRarity, ranker.mainChaoLevel);
		SetChao(1, ranker.subChaoId, ranker.subChaoRarity, ranker.subChaoLevel);
		bool activeSelf = m_sendButton.gameObject.activeSelf;
		if (sendBtnDisable)
		{
			if (activeSelf)
			{
				m_sendButton.gameObject.SetActive(false);
			}
			return;
		}
		if (!activeSelf)
		{
			m_sendButton.gameObject.SetActive(true);
		}
		m_sendButton.isEnabled = (m_ranker.isFriend && !m_ranker.isSentEnergy);
	}

	public void UpdateView(RaidBossUser user)
	{
		m_ranker = user.ConvertRankerData();
		if (m_ranker != null)
		{
			m_scoreTypeObjects[0].SetActive(false);
			m_scoreTypeObjects[1].SetActive(false);
			m_rankingSprite.gameObject.SetActive(false);
			m_rankingLabel.gameObject.SetActive(false);
			m_faceTexture.mainTexture = RankingUtil.GetProfilePictureTexture(m_ranker, delegate(Texture2D _faceTexture)
			{
				m_faceTexture.mainTexture = _faceTexture;
			});
			m_userIdLabel.text = TextManager.GetText(TextManager.TextType.TEXTTYPE_COMMON_TEXT, "Ranking", "ui_Lbl_id").text + " " + m_ranker.id;
			m_friendIconSprite.spriteName = RankingUtil.GetFriendIconSpriteName(m_ranker);
			m_userNameLabel.text = m_ranker.userName;
			m_mapRankLabel.text = TextManager.GetText(TextManager.TextType.TEXTTYPE_COMMON_TEXT, "MainMenu", "ui_Lbl_word_rank").text + " " + m_ranker.dispMapRank;
			m_leagueIcon.spriteName = RankingUtil.GetLeagueIconName((LeagueType)m_ranker.leagueIndex);
			m_leagueIcon2.spriteName = RankingUtil.GetLeagueIconName2((LeagueType)m_ranker.leagueIndex);
			TimeSpan timeSpan = NetUtil.GetCurrentTime() - m_ranker.loginTime;
			m_daysLabel.text = TextUtility.Replaces(TextManager.GetText(TextManager.TextType.TEXTTYPE_COMMON_TEXT, "Ranking", (timeSpan.Days > 0) ? "login_days" : ((timeSpan.Hours <= 0) ? "login_minutes" : "login_hours")).text, new Dictionary<string, string>
			{
				{
					"{DAYS}",
					timeSpan.Days.ToString()
				},
				{
					"{HOURS}",
					timeSpan.Hours.ToString()
				},
				{
					"{MINUTES}",
					timeSpan.Minutes.ToString()
				}
			});
			m_charaNameLabel.text = TextManager.GetText(TextManager.TextType.TEXTTYPE_COMMON_TEXT, "CharaName", CharaName.Name[(int)m_ranker.charaType]).text;
			m_charaLevelLabel.text = TextUtility.GetTextLevel(m_ranker.charaLevel.ToString("D3"));
			SetChara(m_mainCharaTex, m_ranker.charaType);
			SetChara(m_subCharaTex, m_ranker.charaSubType);
			SetChao(0, m_ranker.mainChaoId, m_ranker.mainChaoRarity, m_ranker.mainChaoLevel);
			SetChao(1, m_ranker.subChaoId, m_ranker.subChaoRarity, m_ranker.subChaoLevel);
			m_sendButton.isEnabled = (m_ranker.isFriend && !m_ranker.isSentEnergy);
		}
	}

	private void OnClickSend()
	{
		SoundManager.SePlay("sys_menu_decide");
		ServerInterface loggedInServerInterface = ServerInterface.LoggedInServerInterface;
		if (loggedInServerInterface != null)
		{
			loggedInServerInterface.RequestServerSendEnergy(m_ranker.id, base.gameObject);
		}
		else
		{
			ServerSendEnergy_Succeeded(null);
		}
	}

	private void ServerSendEnergy_Succeeded(MsgSendEnergySucceed msg)
	{
		GeneralWindow.CInfo info = default(GeneralWindow.CInfo);
		info.name = "SendChallenge";
		info.buttonType = GeneralWindow.ButtonType.Ok;
		info.caption = TextManager.GetText(TextManager.TextType.TEXTTYPE_COMMON_TEXT, "Ranking", "gw_send_challenge_caption").text;
		info.message = TextManager.GetText(TextManager.TextType.TEXTTYPE_COMMON_TEXT, "Ranking", "gw_send_challenge_text").text;
		GeneralWindow.Create(info);
		GameObjectUtil.SendMessageFindGameObject("ui_mm_ranking_page(Clone)", "OnUpdateSentEnergy", m_ranker.id, SendMessageOptions.DontRequireReceiver);
		string cellName = "get_present_" + m_ranker.language;
		string text = TextManager.GetText(TextManager.TextType.TEXTTYPE_FIXATION_TEXT, "PushNotice", cellName).text;
		if (!string.IsNullOrEmpty(text))
		{
			PnoteNotification.SendMessage(text, m_ranker.id, PnoteNotification.LaunchOption.SendEnergy);
		}
		RankingManager.UpdateSendChallenge(m_ranker.id);
		SocialInterface socialInterface = GameObjectUtil.FindGameObjectComponent<SocialInterface>("SocialInterface");
		if (socialInterface != null)
		{
			SocialUserData socialUserData = null;
			List<SocialUserData> friendList = socialInterface.FriendList;
			foreach (SocialUserData item in friendList)
			{
				if (item == null || !(item.CustomData.GameId == m_ranker.id))
				{
					continue;
				}
				socialUserData = item;
				break;
			}
			if (socialUserData == null)
			{
			}
		}
		if (m_windowAnimation != null)
		{
			ActiveAnimation.Play(m_windowAnimation, "ui_menu_ranking_window_Anim", Direction.Reverse, EnableCondition.DoNothing, DisableCondition.DisableAfterReverse);
		}
	}

	private void ServerSendEnergy_Failed(ServerInterface.StatusCode status)
	{
		GeneralWindow.CInfo info = default(GeneralWindow.CInfo);
		info.name = "SendChallenge";
		info.buttonType = GeneralWindow.ButtonType.Ok;
		info.caption = TextManager.GetText(TextManager.TextType.TEXTTYPE_COMMON_TEXT, "Ranking", "gw_send_challenge_caption").text;
		info.message = TextManager.GetText(TextManager.TextType.TEXTTYPE_COMMON_TEXT, "Ranking", "gw_send_challenge_error").text;
		GeneralWindow.Create(info);
		if (m_windowAnimation != null)
		{
			ActiveAnimation.Play(m_windowAnimation, "ui_menu_ranking_window_Anim", Direction.Reverse, EnableCondition.DoNothing, DisableCondition.DisableAfterReverse);
		}
	}

	public static void Open(GameObject parent, RankingUtil.RankingScoreType scoreType, RankingUtil.RankingRankerType rankerType, RankingUtil.Ranker ranker, bool sendBtnDisable)
	{
		ranking_window ranking_window = GameObjectUtil.FindChildGameObjectComponent<ranking_window>(parent, "RankingWindowUI");
		if (ranking_window != null)
		{
			ranking_window.Open(scoreType, rankerType, ranker, sendBtnDisable);
		}
	}

	public static void RaidOpen(RaidBossUser user)
	{
		GameObject parent = GameObject.Find("UI Root (2D)");
		ranking_window ranking_window = GameObjectUtil.FindChildGameObjectComponent<ranking_window>(parent, "RankingWindowUI");
		if (ranking_window != null)
		{
			ranking_window.Open(user);
		}
	}

	private void SetChara(UITexture tex, CharaType charaType)
	{
		if (!(tex == null))
		{
			if (charaType == CharaType.UNKNOWN)
			{
				tex.gameObject.SetActive(false);
				return;
			}
			tex.gameObject.SetActive(true);
			TextureRequestChara request = new TextureRequestChara(charaType, tex);
			TextureAsyncLoadManager.Instance.Request(request);
		}
	}

	private void SetChao(int index, int chaoId, int chaoRarity, int chaoLevel)
	{
		SetChaoTexture(m_chaoIconTex[index], chaoId);
		if (chaoId >= 0)
		{
			m_chaoBgSprite[index].gameObject.SetActive(true);
			m_chaoLevelLabel[index].gameObject.SetActive(true);
			m_chaoBgSprite[index].spriteName = "ui_tex_chao_bg_" + chaoRarity;
			m_chaoLevelLabel[index].text = TextUtility.GetTextLevel(chaoLevel.ToString());
		}
		else
		{
			m_chaoBgSprite[index].gameObject.SetActive(true);
			m_chaoLevelLabel[index].gameObject.SetActive(false);
			m_chaoBgSprite[index].spriteName = "ui_tex_chao_bg_x";
		}
		Debug.Log("SetChao m_ChaoIconSprite:" + (m_ChaoIconSprite != null));
	}

	private void SetChaoTexture(UITexture uiTex, int chaoId)
	{
		if (uiTex != null)
		{
			if (chaoId >= 0)
			{
				ChaoTextureManager.CallbackInfo info = new ChaoTextureManager.CallbackInfo(uiTex, null, true);
				ChaoTextureManager.Instance.GetTexture(chaoId, info);
				uiTex.gameObject.SetActive(true);
			}
			else
			{
				uiTex.gameObject.SetActive(false);
			}
		}
	}

	[Conditional("DEBUG_INFO")]
	private static void DebugLog(string s)
	{
		Debug.Log("@ms " + s);
	}

	[Conditional("DEBUG_INFO")]
	private static void DebugLogWarning(string s)
	{
		Debug.LogWarning("@ms " + s);
	}

	public override void OnClickPlatformBackButton(BackButtonMessage msg)
	{
		if (msg != null)
		{
			msg.StaySequence();
		}
		GameObject gameObject = GameObjectUtil.FindChildGameObject(base.gameObject, "Btn_close");
		if (gameObject != null)
		{
			UIButtonMessage component = gameObject.GetComponent<UIButtonMessage>();
			if (component != null)
			{
				component.SendMessage("OnClick");
			}
		}
	}
}
