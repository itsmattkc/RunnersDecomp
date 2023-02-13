using AnimationOrTween;
using Text;
using UnityEngine;

public class PlayerMergeWindow : WindowBase
{
	private bool m_isSetup;

	private int m_playerId;

	private int m_level;

	private int m_rarity;

	private int m_star;

	private bool m_backKeyVaild;

	private bool m_isEnd;

	private RouletteUtility.AchievementType m_achievementType;

	private static readonly string[] PlayerIconNameList = new string[3]
	{
		"img_player_1",
		"img_player_2",
		"img_player_3"
	};

	private HudFlagWatcher m_SeFlagFog;

	private CharacterDataNameInfo.Info m_charaInfo;

	[SerializeField]
	private Color m_starLabelColor = new Color32(239, 236, 0, byte.MaxValue);

	[SerializeField]
	private Color m_maxStarLabelColor = new Color32(246, 116, 0, byte.MaxValue);

	public bool isSetuped
	{
		get
		{
			return m_isSetup;
		}
		set
		{
			m_isSetup = false;
		}
	}

	public bool IsPlayEnd
	{
		get
		{
			return m_isEnd;
		}
	}

	public void PlayStart(int playerId, RouletteUtility.AchievementType achievement = RouletteUtility.AchievementType.NONE)
	{
		m_charaInfo = CharacterDataNameInfo.Instance.GetDataByServerID(playerId);
		RouletteManager.OpenRouletteWindow();
		ServerPlayerState playerState = ServerInterface.PlayerState;
		ServerCharacterState serverCharacterState = playerState.CharacterState(m_charaInfo.m_ID);
		int totalLevel = MenuPlayerSetUtil.GetTotalLevel(m_charaInfo.m_ID);
		int star = serverCharacterState.star;
		int starMax = serverCharacterState.starMax;
		int num = star - 1;
		m_backKeyVaild = false;
		m_isEnd = false;
		m_achievementType = achievement;
		if (!m_isSetup)
		{
			GameObject gameObject = GameObjectUtil.FindChildGameObject(base.gameObject, "pattern_btn_use");
			if (gameObject != null)
			{
				GameObject gameObject2 = GameObjectUtil.FindChildGameObject(gameObject, "pattern_0");
				if (gameObject2 != null)
				{
					UIButtonMessage uIButtonMessage = GameObjectUtil.FindChildGameObjectComponent<UIButtonMessage>(gameObject2, "Btn_ok");
					if (uIButtonMessage != null)
					{
						uIButtonMessage.target = base.gameObject;
						uIButtonMessage.functionName = "OkButtonClickedCallback";
					}
				}
			}
			UIButtonMessage uIButtonMessage2 = GameObjectUtil.FindChildGameObjectComponent<UIButtonMessage>(base.gameObject, "skip_collider");
			if (uIButtonMessage2 != null)
			{
				uIButtonMessage2.target = base.gameObject;
				uIButtonMessage2.functionName = "SkipButtonClickedCallback";
			}
			m_SeFlagFog = new HudFlagWatcher();
			GameObject watchObject = GameObjectUtil.FindChildGameObject(base.gameObject, "SE_flag_fog");
			m_SeFlagFog.Setup(watchObject, SeFlagFogCallback);
			m_isSetup = true;
		}
		base.gameObject.SetActive(true);
		GameObject gameObject3 = GameObjectUtil.FindChildGameObject(base.gameObject, "skip_collider");
		if (gameObject3 != null)
		{
			gameObject3.SetActive(true);
		}
		Animation component = base.gameObject.GetComponent<Animation>();
		if (component != null)
		{
			component.Stop();
			ActiveAnimation activeAnimation = ActiveAnimation.Play(component, "ui_menu_player_merge_Window_Anim", Direction.Forward);
			EventDelegate.Add(activeAnimation.onFinished, InAnimationEndCallback, true);
			UIImageButton uIImageButton = GameObjectUtil.FindChildGameObjectComponent<UIImageButton>(base.gameObject, "Btn_ok");
			if (uIImageButton != null)
			{
				uIImageButton.isEnabled = false;
			}
		}
		string[] playerIconNameList = PlayerIconNameList;
		foreach (string name in playerIconNameList)
		{
			UITexture uITexture = GameObjectUtil.FindChildGameObjectComponent<UITexture>(base.gameObject, name);
			if (uITexture != null)
			{
				TextureRequestChara request = new TextureRequestChara(m_charaInfo.m_ID, uITexture);
				TextureAsyncLoadManager.Instance.Request(request);
			}
		}
		UILabel uILabel = GameObjectUtil.FindChildGameObjectComponent<UILabel>(base.gameObject, "Lbl_player_name");
		if (uILabel != null && m_charaInfo != null)
		{
			uILabel.gameObject.SetActive(true);
			uILabel.text = TextManager.GetText(TextManager.TextType.TEXTTYPE_COMMON_TEXT, "CharaName", m_charaInfo.m_name.ToLower()).text;
		}
		UILabel uILabel2 = GameObjectUtil.FindChildGameObjectComponent<UILabel>(base.gameObject, "Lbl_player_lv");
		if (uILabel2 != null && m_charaInfo != null)
		{
			uILabel2.gameObject.SetActive(true);
			uILabel2.text = string.Format("Lv.{0:D3}", totalLevel);
		}
		UISprite uISprite = GameObjectUtil.FindChildGameObjectComponent<UISprite>(base.gameObject, "img_player_speacies");
		if (uISprite != null && m_charaInfo != null)
		{
			uISprite.gameObject.SetActive(true);
			switch (m_charaInfo.m_attribute)
			{
			case CharacterAttribute.FLY:
				uISprite.spriteName = "ui_mm_player_species_1";
				break;
			case CharacterAttribute.POWER:
				uISprite.spriteName = "ui_mm_player_species_2";
				break;
			case CharacterAttribute.SPEED:
				uISprite.spriteName = "ui_mm_player_species_0";
				break;
			}
		}
		UISprite uISprite2 = GameObjectUtil.FindChildGameObjectComponent<UISprite>(base.gameObject, "img_player_genus");
		if (uISprite2 != null && m_charaInfo != null)
		{
			uISprite2.gameObject.SetActive(true);
			uISprite2.spriteName = HudUtility.GetTeamAttributeSpriteName(m_charaInfo.m_ID);
		}
		GameObject gameObject4 = GameObjectUtil.FindChildGameObject(base.gameObject, "star_lv_before");
		if (gameObject4 != null)
		{
			UILabel uILabel3 = GameObjectUtil.FindChildGameObjectComponent<UILabel>(gameObject4, "Lbl_player_star_lv");
			if (uILabel3 != null && m_charaInfo != null)
			{
				uILabel3.gameObject.SetActive(true);
				uILabel3.text = string.Format("{0:D2}", num);
				uILabel3.color = m_starLabelColor;
			}
		}
		GameObject gameObject5 = GameObjectUtil.FindChildGameObject(base.gameObject, "star_lv_after");
		if (gameObject5 != null)
		{
			UILabel uILabel4 = GameObjectUtil.FindChildGameObjectComponent<UILabel>(gameObject5, "Lbl_player_star_lv");
			if (uILabel4 != null && m_charaInfo != null)
			{
				uILabel4.gameObject.SetActive(true);
				uILabel4.text = string.Format("{0:D2}", star);
				if (star >= starMax)
				{
					uILabel4.color = m_maxStarLabelColor;
				}
				else
				{
					uILabel4.color = m_starLabelColor;
				}
			}
		}
		UILabel uILabel5 = GameObjectUtil.FindChildGameObjectComponent<UILabel>(base.gameObject, "Lbl_player_attribute");
		if (uILabel5 != null && m_charaInfo != null)
		{
			GameObject gameObject6 = ResourceManager.Instance.GetGameObject(ResourceCategory.QUICK_MODE, "StageTimeTable");
			GameObject gameObject7 = ResourceManager.Instance.GetGameObject(ResourceCategory.ETC, "OverlapBonusTable");
			if (gameObject6 != null && gameObject7 != null)
			{
				StageTimeTable component2 = gameObject6.GetComponent<StageTimeTable>();
				OverlapBonusTable component3 = gameObject7.GetComponent<OverlapBonusTable>();
				if (component2 != null && component3 != null)
				{
					int tableItemData = component2.GetTableItemData(StageTimeTableItem.OverlapBonus);
					uILabel5.gameObject.SetActive(true);
					TextObject text = TextManager.GetText(TextManager.TextType.TEXTTYPE_COMMON_TEXT, "WindowText", "chara_overlap_text");
					text.ReplaceTag("{BEFORE_TIME}", (num * tableItemData).ToString());
					text.ReplaceTag("{AFTER_TIME}", (star * tableItemData).ToString());
					if (component3.GetStarBonusList(m_charaInfo.m_ID, star).ContainsKey(BonusParam.BonusType.SCORE))
					{
						text.ReplaceTag("{BEFORE_PARAM}", component3.GetStarBonusList(m_charaInfo.m_ID, star - 1)[BonusParam.BonusType.SCORE].ToString());
						text.ReplaceTag("{AFTER_PARAM}", component3.GetStarBonusList(m_charaInfo.m_ID, star)[BonusParam.BonusType.SCORE].ToString());
					}
					uILabel5.text = text.text;
				}
			}
		}
		UIDraggablePanel uIDraggablePanel = GameObjectUtil.FindChildGameObjectComponent<UIDraggablePanel>(base.gameObject, "window_chaoset_alpha_clip");
		if (uIDraggablePanel != null)
		{
			uIDraggablePanel.ResetPosition();
		}
		GameObject gameObject8 = GameObjectUtil.FindChildGameObject(base.gameObject, "eff_4");
		GameObject gameObject9 = GameObjectUtil.FindChildGameObject(base.gameObject, "eff_6");
		GameObject gameObject10 = GameObjectUtil.FindChildGameObject(base.gameObject, "eff_7");
		if (gameObject8 != null)
		{
			gameObject8.SetActive(true);
		}
		if (gameObject9 != null)
		{
			gameObject9.SetActive(true);
		}
		if (gameObject10 != null)
		{
			gameObject10.SetActive(true);
		}
	}

	private void Update()
	{
		if (m_SeFlagFog != null)
		{
			m_SeFlagFog.Update();
		}
	}

	private void SeFlagFogCallback(float newValue, float prevValue)
	{
		if (newValue == 1f)
		{
			ChaoWindowUtility.PlaySEChaoUnite(m_playerId);
		}
	}

	private void OkButtonClickedCallback()
	{
		RouletteManager.CloseRouletteWindow();
		SoundManager.SePlay("sys_menu_decide");
		if (m_achievementType != 0)
		{
			RouletteManager.RouletteGetWindowClose(m_achievementType);
			m_achievementType = RouletteUtility.AchievementType.NONE;
		}
		Animation component = base.gameObject.GetComponent<Animation>();
		if (component != null)
		{
			ActiveAnimation activeAnimation = ActiveAnimation.Play(component, "ui_menu_player_merge_Window_outro_Anim", Direction.Forward);
			EventDelegate.Add(activeAnimation.onFinished, OutAnimationEndCallback, true);
		}
		m_backKeyVaild = false;
	}

	private void InAnimationEndCallback()
	{
		GameObject gameObject = GameObjectUtil.FindChildGameObject(base.gameObject, "skip_collider");
		if (gameObject != null)
		{
			gameObject.SetActive(false);
		}
		UIImageButton uIImageButton = GameObjectUtil.FindChildGameObjectComponent<UIImageButton>(base.gameObject, "Btn_ok");
		if (uIImageButton != null)
		{
			uIImageButton.isEnabled = true;
		}
		m_backKeyVaild = true;
	}

	private void OutAnimationEndCallback()
	{
		base.gameObject.SetActive(false);
		m_isEnd = true;
	}

	private void SkipButtonClickedCallback()
	{
		Animation component = base.gameObject.GetComponent<Animation>();
		if (!(component != null))
		{
			return;
		}
		foreach (AnimationState item in component)
		{
			if (!(item == null))
			{
				item.time = item.length * 0.99f;
			}
		}
	}

	public override void OnClickPlatformBackButton(BackButtonMessage msg)
	{
		if (msg != null)
		{
			msg.StaySequence();
		}
		if (!m_backKeyVaild)
		{
			return;
		}
		GameObject gameObject = GameObjectUtil.FindChildGameObject(base.gameObject, "pattern_0");
		if (!(gameObject != null))
		{
			return;
		}
		GameObject gameObject2 = GameObjectUtil.FindChildGameObject(gameObject, "Btn_ok");
		if (gameObject2 != null)
		{
			UIButtonMessage component = gameObject2.GetComponent<UIButtonMessage>();
			if (component != null)
			{
				component.SendMessage("OnClick");
			}
		}
	}
}
