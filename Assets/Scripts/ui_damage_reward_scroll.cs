using Text;
using UnityEngine;

public class ui_damage_reward_scroll : MonoBehaviour
{
	[SerializeField]
	private UITexture m_faceTexture;

	[SerializeField]
	private UISprite m_scoreRankIconSprite;

	[SerializeField]
	private UISprite m_friendIconSprite;

	[SerializeField]
	private UISprite m_charaSprite;

	[SerializeField]
	private UITexture m_mainChaoIcon;

	[SerializeField]
	private UITexture m_subChaoIcon;

	[SerializeField]
	private UISprite m_chao1BgSprite;

	[SerializeField]
	private UISprite m_chao2BgSprite;

	[SerializeField]
	private UILabel m_nameLabel;

	[SerializeField]
	private UILabel m_damageLabel;

	[SerializeField]
	private UILabel m_damageRateLabel;

	[SerializeField]
	private UISlider m_damage;

	[SerializeField]
	private UISprite m_destroyIcon;

	[SerializeField]
	private UILabel m_destroyCountLabel;

	[SerializeField]
	private TweenColor[] m_myRanker = new TweenColor[2];

	private UILabel m_lvLabel;

	private RaidBossData m_bossData;

	private RaidBossUser m_user;

	private bool m_isImgLoad;

	private bool m_myCell;

	private static float s_updateLastTime;

	public void Start()
	{
	}

	private void LoadImage()
	{
		if (m_user != null && !m_isImgLoad && (m_myCell || m_user.isFriend))
		{
			m_faceTexture.mainTexture = RankingUtil.GetProfilePictureTexture(m_user.id, delegate(Texture2D _faceTexture)
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

	public void UpdateView(RaidBossUser user, RaidBossData bossData)
	{
		if (user == null)
		{
			return;
		}
		m_myCell = false;
		m_isImgLoad = false;
		m_bossData = bossData;
		m_user = user;
		if (m_lvLabel == null)
		{
			m_lvLabel = GameObjectUtil.FindChildGameObjectComponent<UILabel>(base.gameObject, "Lbl_player_lv");
		}
		SetChaoTexture(m_mainChaoIcon, m_user.mainChaoId);
		SetChaoTexture(m_subChaoIcon, m_user.subChaoId);
		if (m_user.isFriend)
		{
			LoadImage();
		}
		else
		{
			m_faceTexture.mainTexture = PlayerImageManager.GetPlayerDefaultImage();
		}
		m_scoreRankIconSprite.enabled = m_user.isRankTop;
		if (m_friendIconSprite != null)
		{
			m_friendIconSprite.spriteName = RankingUtil.GetFriendIconSpriteName(m_user);
		}
		if (m_user.charaType != CharaType.UNKNOWN)
		{
			if (AtlasManager.Instance != null)
			{
				AtlasManager.Instance.ReplacePlayerAtlasForRaidResult(m_charaSprite.atlas);
			}
			m_charaSprite.spriteName = "ui_tex_player_set_" + CharaTypeUtil.GetCharaSpriteNameSuffix(m_user.charaType);
		}
		if (m_lvLabel != null)
		{
			string text = TextManager.GetText(TextManager.TextType.TEXTTYPE_COMMON_TEXT, "MainMenu", "ui_LevelNumber").text;
			m_lvLabel.text = text.Replace("{PARAM}", user.charaLevel.ToString());
		}
		if (m_chao1BgSprite != null)
		{
			if (m_user.mainChaoId == -1)
			{
				m_chao1BgSprite.gameObject.SetActive(false);
			}
			else
			{
				m_chao1BgSprite.gameObject.SetActive(true);
				m_chao1BgSprite.spriteName = "ui_ranking_scroll_char_bg_" + m_user.mainChaoRarity;
			}
		}
		if (m_chao2BgSprite != null)
		{
			if (m_user.subChaoId == -1)
			{
				m_chao2BgSprite.gameObject.SetActive(false);
			}
			else
			{
				m_chao2BgSprite.gameObject.SetActive(true);
				m_chao2BgSprite.spriteName = "ui_ranking_scroll_char_bg_S" + m_user.subChaoRarity;
			}
		}
		m_nameLabel.text = m_user.userName;
		if (m_damage != null && m_damageLabel != null && m_damageRateLabel != null)
		{
			m_damageLabel.text = HudUtility.GetFormatNumString(m_user.damage);
			float num = (float)m_user.damage / (float)m_bossData.hpMax;
			int num2 = 0;
			if (num < 0f)
			{
				num = 0f;
				num2 = 0;
			}
			else if (num > 1f)
			{
				num = 1f;
				num2 = 100;
			}
			else
			{
				int num3 = Mathf.FloorToInt(num * 10000f);
				if (num3 > 1)
				{
					num3--;
				}
				num = (float)num3 / 10000f;
				num2 = Mathf.CeilToInt(100f * num);
			}
			if (num2 < 0)
			{
				num2 = 0;
			}
			else if (num2 >= 100)
			{
				num2 = 100;
				if (m_user.damage < m_bossData.hpMax)
				{
					num2 = 99;
				}
			}
			string text2 = TextManager.GetText(TextManager.TextType.TEXTTYPE_COMMON_TEXT, "ItemRoulette", "odds").text;
			m_damageRateLabel.text = TextUtility.Replace(text2, "{ODDS}", string.Empty + num2);
			m_damage.value = num;
			m_damage.ForceUpdate();
		}
		if (m_destroyCountLabel != null)
		{
			m_destroyCountLabel.text = HudUtility.GetFormatNumString(m_user.destroyCount);
		}
		if (m_destroyIcon != null)
		{
			m_destroyIcon.enabled = m_user.isDestroy;
		}
	}

	public void SetMyRanker(bool myCell)
	{
		m_myCell = myCell;
		UISprite uISprite = GameObjectUtil.FindChildGameObjectComponent<UISprite>(base.gameObject, "Btn_damage_reward_top");
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
		if (myCell)
		{
			m_isImgLoad = false;
			LoadImage();
		}
		return;
		IL_008c:
		uISprite.spriteName = ((!myCell) ? "ui_event_raidboss_damage_reward_bar1" : "ui_event_raidboss_damage_reward_bar2");
		goto IL_00a7;
	}

	public void UpdateSendChallenge(string id)
	{
		if (base.gameObject.activeSelf && m_user != null && m_user.id == id)
		{
			m_user.isSentEnergy = true;
			if (m_friendIconSprite != null)
			{
				m_friendIconSprite.spriteName = RankingUtil.GetFriendIconSpriteName(m_user);
			}
		}
	}

	private void SetChaoTexture(UITexture uiTex, int chaoId)
	{
		if (uiTex != null)
		{
			if (chaoId >= 0 && SingletonGameObject<RankingManager>.Instance != null)
			{
				SingletonGameObject<RankingManager>.Instance.GetChaoTexture(chaoId, uiTex);
				uiTex.gameObject.SetActive(true);
			}
			else
			{
				uiTex.gameObject.SetActive(false);
			}
		}
	}

	public void SetClickCollision(bool flag)
	{
		UIButtonOffset uIButtonOffset = GameObjectUtil.FindChildGameObjectComponent<UIButtonOffset>(base.gameObject, "Btn_damage_reward_top");
		if (uIButtonOffset != null)
		{
			uIButtonOffset.enabled = flag;
		}
		UIButtonMessage uIButtonMessage = GameObjectUtil.FindChildGameObjectComponent<UIButtonMessage>(base.gameObject, "Btn_damage_reward_top");
		if (uIButtonMessage != null)
		{
			uIButtonMessage.enabled = flag;
		}
	}

	private void OnClickUserScroll()
	{
		if (Mathf.Abs(s_updateLastTime - Time.realtimeSinceStartup) > 1.5f)
		{
			SoundManager.SePlay("sys_menu_decide");
			ranking_window.RaidOpen(m_user);
		}
	}
}
