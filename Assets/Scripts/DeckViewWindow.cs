using AnimationOrTween;
using DataTable;
using Message;
using SaveData;
using System.Collections.Generic;
using Text;
using UnityEngine;

public class DeckViewWindow : WindowBase
{
	private enum BTN_TYPE
	{
		UP,
		DOWN
	}

	private enum SELECT_TYPE
	{
		CHARA_MAIN = 0,
		CHARA_SUB = 1,
		CHAO_MAIN = 2,
		CHAO_SUB = 3,
		NUM = 4,
		UNKNOWN = -1
	}

	private const float BTN_TIME_LIMIT = 0.5f;

	private const float BTN_DELAY_TIME = 0.5f;

	private GameObject m_parent;

	private GameObject m_windowRoot;

	private Animation m_windowAnimation;

	private BoxCollider m_bgCollider;

	private bool m_init;

	private bool m_change;

	private bool m_close;

	private int m_chaoMainId = -1;

	private int m_chaoSubId = -1;

	private float m_chaoSpIconTime;

	private int m_direction = 1;

	private float m_pressTime;

	private SELECT_TYPE m_pressBtnType = SELECT_TYPE.UNKNOWN;

	private float m_changeDelayCheckTime;

	private Dictionary<SELECT_TYPE, List<UIImageButton>> m_changeBtnList;

	private int m_currentChaoSetStock;

	private List<bool> m_isSaveData;

	private CharaType m_playerMain = CharaType.UNKNOWN;

	private CharaType m_playerSub = CharaType.UNKNOWN;

	private List<CharaType> m_charaList;

	private List<DataTable.ChaoData> m_chaoList;

	private bool m_isLastInputTime;

	private Dictionary<SELECT_TYPE, float> m_lastInputTime;

	private UISprite m_detailTextBg;

	private UILabel m_detailTextLabel;

	private ChaoSort m_currentChaoSort = ChaoSort.NONE;

	private int m_initChaoSetStock;

	private CharaType m_initPlayerMain = CharaType.UNKNOWN;

	private CharaType m_initPlayerSub = CharaType.UNKNOWN;

	private int m_initChaoMain = -1;

	private int m_initChaoSub = -1;

	private int m_reqChaoSetStock;

	private CharaType m_reqPlayerMain = CharaType.UNKNOWN;

	private CharaType m_reqPlayerSub = CharaType.UNKNOWN;

	private int m_reqChaoMain = -1;

	private int m_reqChaoSub = -1;

	private bool m_btnLock;

	private static DeckViewWindow s_instance;

	public static DeckViewWindow instance
	{
		get
		{
			return s_instance;
		}
	}

	public static bool isActive
	{
		get
		{
			if (s_instance != null)
			{
				return s_instance.gameObject.activeSelf;
			}
			return false;
		}
	}

	private void OnDestroy()
	{
		Destroy();
	}

	public void Init()
	{
		m_btnLock = false;
		m_parent = null;
		UIPanel uIPanel = GameObjectUtil.FindChildGameObjectComponent<UIPanel>(base.gameObject, "DeckViewWindow");
		if (uIPanel != null)
		{
			uIPanel.alpha = 0f;
		}
		if (m_bgCollider != null)
		{
			m_bgCollider.enabled = false;
		}
		if (m_windowRoot == null)
		{
			m_windowRoot = GameObjectUtil.FindChildGameObject(base.gameObject, "window");
		}
		if (m_windowRoot != null)
		{
			m_windowRoot.SetActive(false);
		}
		m_change = false;
		m_chaoMainId = -1;
		m_chaoSubId = -1;
		m_playerMain = CharaType.UNKNOWN;
		m_playerSub = CharaType.UNKNOWN;
		m_init = false;
		m_close = false;
		m_chaoSpIconTime = 0f;
		m_initChaoSetStock = 0;
		m_initPlayerMain = CharaType.UNKNOWN;
		m_initPlayerSub = CharaType.UNKNOWN;
		m_initChaoMain = -1;
		m_initChaoSub = -1;
		SetChangeBtn();
		ResetBtnDelayTime();
		ResetLastInputTime();
	}

	public void SetChangeBtn()
	{
		if (m_changeBtnList == null)
		{
			m_changeBtnList = new Dictionary<SELECT_TYPE, List<UIImageButton>>();
		}
		else if (m_changeBtnList.Count > 0)
		{
			m_changeBtnList.Clear();
		}
		if (m_changeBtnList == null)
		{
			return;
		}
		GameObject parent = GameObjectUtil.FindChildGameObject(base.gameObject, "info_player");
		GameObject parent2 = GameObjectUtil.FindChildGameObject(base.gameObject, "info_chao");
		UIImageButton uIImageButton = null;
		UIImageButton uIImageButton2 = null;
		List<UIImageButton> list = null;
		int num = 4;
		for (int i = 0; i < num; i++)
		{
			SELECT_TYPE sELECT_TYPE = (SELECT_TYPE)i;
			list = new List<UIImageButton>();
			switch (sELECT_TYPE)
			{
			case SELECT_TYPE.CHARA_MAIN:
				uIImageButton = GameObjectUtil.FindChildGameObjectComponent<UIImageButton>(parent, "Btn_main_up");
				uIImageButton2 = GameObjectUtil.FindChildGameObjectComponent<UIImageButton>(parent, "Btn_main_down");
				if (uIImageButton != null)
				{
					uIImageButton.isEnabled = true;
				}
				if (uIImageButton2 != null)
				{
					uIImageButton2.isEnabled = true;
				}
				list.Add(uIImageButton);
				list.Add(uIImageButton2);
				break;
			case SELECT_TYPE.CHARA_SUB:
				uIImageButton = GameObjectUtil.FindChildGameObjectComponent<UIImageButton>(parent, "Btn_sub_up");
				uIImageButton2 = GameObjectUtil.FindChildGameObjectComponent<UIImageButton>(parent, "Btn_sub_down");
				if (uIImageButton != null)
				{
					uIImageButton.isEnabled = true;
				}
				if (uIImageButton2 != null)
				{
					uIImageButton2.isEnabled = true;
				}
				list.Add(uIImageButton);
				list.Add(uIImageButton2);
				break;
			case SELECT_TYPE.CHAO_MAIN:
				uIImageButton = GameObjectUtil.FindChildGameObjectComponent<UIImageButton>(parent2, "Btn_main_up");
				uIImageButton2 = GameObjectUtil.FindChildGameObjectComponent<UIImageButton>(parent2, "Btn_main_down");
				if (uIImageButton != null)
				{
					uIImageButton.isEnabled = true;
				}
				if (uIImageButton2 != null)
				{
					uIImageButton2.isEnabled = true;
				}
				list.Add(uIImageButton);
				list.Add(uIImageButton2);
				break;
			case SELECT_TYPE.CHAO_SUB:
				uIImageButton = GameObjectUtil.FindChildGameObjectComponent<UIImageButton>(parent2, "Btn_sub_up");
				uIImageButton2 = GameObjectUtil.FindChildGameObjectComponent<UIImageButton>(parent2, "Btn_sub_down");
				if (uIImageButton != null)
				{
					uIImageButton.isEnabled = true;
				}
				if (uIImageButton2 != null)
				{
					uIImageButton2.isEnabled = true;
				}
				list.Add(uIImageButton);
				list.Add(uIImageButton2);
				break;
			}
			if (list.Count > 0)
			{
				m_changeBtnList.Add(sELECT_TYPE, list);
			}
		}
	}

	public void ResetBtnDelayTime()
	{
		SetAllChangeBtnEnabled(true);
		m_changeDelayCheckTime = 0f;
	}

	public void Setup(int mainChaoId, int subChaoId, GameObject parent)
	{
		m_btnLock = false;
		s_instance = this;
		base.gameObject.SetActive(true);
		m_parent = parent;
		m_change = false;
		m_chaoMainId = mainChaoId;
		m_chaoSubId = subChaoId;
		m_init = true;
		m_close = false;
		m_chaoSpIconTime = 0f;
		m_windowAnimation = base.gameObject.GetComponentInChildren<Animation>();
		ActiveAnimation.Play(m_windowAnimation, Direction.Forward);
		SetChangeBtn();
		ResetBtnDelayTime();
		m_currentChaoSetStock = DeckUtil.GetDeckCurrentStockIndex();
		m_initChaoSetStock = m_currentChaoSetStock;
		m_isSaveData = new List<bool>();
		m_isSaveData.Add(true);
		m_isSaveData.Add(DeckUtil.IsChaoSetSave(1));
		m_isSaveData.Add(DeckUtil.IsChaoSetSave(2));
		ResetLastInputTime();
		GameObject gameObject = GameObjectUtil.FindChildGameObject(base.gameObject, "info_notice");
		if (gameObject != null)
		{
			m_detailTextBg = GameObjectUtil.FindChildGameObjectComponent<UISprite>(gameObject, "img_base_bg");
			m_detailTextLabel = GameObjectUtil.FindChildGameObjectComponent<UILabel>(gameObject, "Lbl_bonusnotice");
		}
		if (m_detailTextBg != null && m_detailTextLabel != null)
		{
			m_detailTextBg.alpha = 0f;
			m_detailTextLabel.alpha = 0f;
			TweenAlpha component = m_detailTextBg.GetComponent<TweenAlpha>();
			TweenAlpha component2 = m_detailTextLabel.GetComponent<TweenAlpha>();
			if (component != null && component2 != null)
			{
				component.enabled = false;
				component2.enabled = false;
			}
		}
		if (m_windowRoot == null)
		{
			m_windowRoot = GameObjectUtil.FindChildGameObject(base.gameObject, "window");
		}
		if (m_windowRoot != null)
		{
			m_windowRoot.SetActive(true);
		}
		m_bgCollider = GameObjectUtil.FindChildGameObjectComponent<BoxCollider>(base.gameObject, "blinder");
		if (m_bgCollider != null)
		{
			m_bgCollider.enabled = true;
		}
		UIPlayAnimation[] componentsInChildren = base.gameObject.GetComponentsInChildren<UIPlayAnimation>();
		if (componentsInChildren != null && componentsInChildren.Length > 0)
		{
			UIPlayAnimation[] array = componentsInChildren;
			foreach (UIPlayAnimation uIPlayAnimation in array)
			{
				uIPlayAnimation.enabled = true;
				if (uIPlayAnimation.onFinished == null || uIPlayAnimation.onFinished.Count == 0)
				{
					EventDelegate.Add(uIPlayAnimation.onFinished, OnFinished, false);
				}
			}
		}
		SaveDataManager instance = SaveDataManager.Instance;
		m_initPlayerMain = instance.PlayerData.MainChara;
		m_initPlayerSub = instance.PlayerData.SubChara;
		m_initChaoMain = mainChaoId;
		m_initChaoSub = subChaoId;
		SetupChaoView();
		SetupPlayerView(instance);
		SetupBonusView();
		SetupTabView();
		m_charaList = null;
		if (instance != null)
		{
			ServerPlayerState playerState = ServerInterface.PlayerState;
			if (playerState != null)
			{
				int num = 29;
				for (int j = 0; j < num; j++)
				{
					ServerCharacterState serverCharacterState = playerState.CharacterState((CharaType)j);
					if (serverCharacterState != null && serverCharacterState.IsUnlocked)
					{
						CharaType charaType = (CharaType)j;
						if (m_charaList == null)
						{
							m_charaList = new List<CharaType>();
							m_charaList.Add(charaType);
						}
						else
						{
							m_charaList.Add(charaType);
						}
						Debug.Log("use chara:" + charaType);
					}
				}
			}
		}
		m_currentChaoSort = ChaoSort.NONE;
		m_chaoList = ChaoTable.GetPossessionChaoData();
	}

	private void SetupBonusView()
	{
		GameObject gameObject = GameObjectUtil.FindChildGameObject(base.gameObject, "info_bonus");
		if (gameObject != null)
		{
			UILabel scoreBonus = GameObjectUtil.FindChildGameObjectComponent<UILabel>(gameObject, "Lbl_bonus_0");
			UILabel ringBonus = GameObjectUtil.FindChildGameObjectComponent<UILabel>(gameObject, "Lbl_bonus_1");
			UILabel animalBonus = GameObjectUtil.FindChildGameObjectComponent<UILabel>(gameObject, "Lbl_bonus_2");
			UILabel distanceBonus = GameObjectUtil.FindChildGameObjectComponent<UILabel>(gameObject, "Lbl_bonus_3");
			UILabel enemyBonus = GameObjectUtil.FindChildGameObjectComponent<UILabel>(gameObject, "Lbl_bonus_4");
			SetBonus(ref scoreBonus, ref ringBonus, ref animalBonus, ref distanceBonus, ref enemyBonus);
		}
	}

	private void SetBonus(ref UILabel scoreBonus, ref UILabel ringBonus, ref UILabel animalBonus, ref UILabel distanceBonus, ref UILabel enemyBonus)
	{
		BonusParamContainer currentBonusData = BonusUtil.GetCurrentBonusData(m_playerMain, m_playerSub, m_chaoMainId, m_chaoSubId);
		if (currentBonusData == null)
		{
			return;
		}
		int index = -1;
		Dictionary<BonusParam.BonusType, float> bonusInfo = currentBonusData.GetBonusInfo(index);
		SetupAbilityIcon(currentBonusData);
		SetupNoticeView(currentBonusData);
		if (bonusInfo != null)
		{
			if (bonusInfo.ContainsKey(BonusParam.BonusType.SCORE))
			{
				SetBonusParam(ref scoreBonus, bonusInfo[BonusParam.BonusType.SCORE]);
			}
			else
			{
				SetBonusParam(ref scoreBonus, 0f);
			}
			if (bonusInfo.ContainsKey(BonusParam.BonusType.RING))
			{
				SetBonusParam(ref ringBonus, bonusInfo[BonusParam.BonusType.RING]);
			}
			else
			{
				SetBonusParam(ref ringBonus, 0f);
			}
			if (bonusInfo.ContainsKey(BonusParam.BonusType.ANIMAL))
			{
				SetBonusParam(ref animalBonus, bonusInfo[BonusParam.BonusType.ANIMAL]);
			}
			else
			{
				SetBonusParam(ref animalBonus, 0f);
			}
			if (bonusInfo.ContainsKey(BonusParam.BonusType.DISTANCE))
			{
				SetBonusParam(ref distanceBonus, bonusInfo[BonusParam.BonusType.DISTANCE]);
			}
			else
			{
				SetBonusParam(ref distanceBonus, 0f);
			}
			if (bonusInfo.ContainsKey(BonusParam.BonusType.ENEMY_OBJBREAK))
			{
				SetBonusParam(ref enemyBonus, bonusInfo[BonusParam.BonusType.ENEMY_OBJBREAK]);
			}
			else
			{
				SetBonusParam(ref enemyBonus, 0f);
			}
		}
	}

	private void SetBonusParam(ref UILabel bonusLabel, float param)
	{
		if (bonusLabel != null)
		{
			bonusLabel.text = TextUtility.Replaces(TextManager.GetText(TextManager.TextType.TEXTTYPE_COMMON_TEXT, "DailyMission", "clear_percent").text, new Dictionary<string, string>
			{
				{
					"{PARAM}",
					param.ToString()
				}
			});
		}
	}

	private void SetupTabView()
	{
		GameObject gameObject = GameObjectUtil.FindChildGameObject(base.gameObject, "Deck_tab");
		if (gameObject != null)
		{
			List<string> list = new List<string>();
			List<string> list2 = new List<string>();
			list.Add("tab_1");
			list.Add("tab_2");
			list.Add("tab_3");
			list.Add("tab_4");
			list.Add("tab_5");
			list2.Add("OnClickTab1");
			list2.Add("OnClickTab2");
			list2.Add("OnClickTab3");
			list2.Add("OnClickTab4");
			list2.Add("OnClickTab5");
			GeneralUtil.SetToggleObject(base.gameObject, gameObject, list2, list, m_currentChaoSetStock);
		}
	}

	private void SetupChaoView()
	{
		GameObject gameObject = GameObjectUtil.FindChildGameObject(base.gameObject, "info_chao");
		if (gameObject != null)
		{
			GameObject gameObject2 = GameObjectUtil.FindChildGameObject(gameObject, "chao_main");
			GameObject gameObject3 = GameObjectUtil.FindChildGameObject(gameObject, "chao_sub");
			if (gameObject2 != null)
			{
				UILabel lv = GameObjectUtil.FindChildGameObjectComponent<UILabel>(gameObject2, "Lbl_chao_main_lv");
				UILabel name = GameObjectUtil.FindChildGameObjectComponent<UILabel>(gameObject2, "Lbl_chao_main_name");
				UISprite bg = GameObjectUtil.FindChildGameObjectComponent<UISprite>(gameObject2, "img_chao_rank_bg_main");
				UISprite type = GameObjectUtil.FindChildGameObjectComponent<UISprite>(gameObject2, "img_chao_type_main");
				UITexture tex = GameObjectUtil.FindChildGameObjectComponent<UITexture>(gameObject2, "img_chao_text_main");
				UISprite chao = GameObjectUtil.FindChildGameObjectComponent<UISprite>(gameObject2, "img_chao_main");
				SetChao(m_chaoMainId, ref name, ref lv, ref bg, ref type, ref tex, ref chao);
			}
			if (gameObject3 != null)
			{
				UILabel lv2 = GameObjectUtil.FindChildGameObjectComponent<UILabel>(gameObject3, "Lbl_chao_sub_lv");
				UILabel name2 = GameObjectUtil.FindChildGameObjectComponent<UILabel>(gameObject3, "Lbl_chao_sub_name");
				UISprite bg2 = GameObjectUtil.FindChildGameObjectComponent<UISprite>(gameObject3, "img_chao_rank_bg_sub");
				UISprite type2 = GameObjectUtil.FindChildGameObjectComponent<UISprite>(gameObject3, "img_chao_type_sub");
				UITexture tex2 = GameObjectUtil.FindChildGameObjectComponent<UITexture>(gameObject3, "img_chao_text_sub");
				UISprite chao2 = GameObjectUtil.FindChildGameObjectComponent<UISprite>(gameObject3, "img_chao_sub");
				SetChao(m_chaoSubId, ref name2, ref lv2, ref bg2, ref type2, ref tex2, ref chao2);
			}
		}
	}

	private void ResetupChaoView()
	{
		GameObject gameObject = GameObjectUtil.FindChildGameObject(base.gameObject, "info_chao");
		if (gameObject != null)
		{
			GameObject gameObject2 = GameObjectUtil.FindChildGameObject(gameObject, "chao_main");
			GameObject gameObject3 = GameObjectUtil.FindChildGameObject(gameObject, "chao_sub");
			if (gameObject2 != null)
			{
				UITexture tex = GameObjectUtil.FindChildGameObjectComponent<UITexture>(gameObject2, "img_chao_text_main");
				UISprite chao = GameObjectUtil.FindChildGameObjectComponent<UISprite>(gameObject2, "img_chao_main");
				ResetupChao(ref tex, ref chao);
			}
			if (gameObject3 != null)
			{
				UITexture tex2 = GameObjectUtil.FindChildGameObjectComponent<UITexture>(gameObject3, "img_chao_text_sub");
				UISprite chao2 = GameObjectUtil.FindChildGameObjectComponent<UISprite>(gameObject3, "img_chao_sub");
				ResetupChao(ref tex2, ref chao2);
			}
		}
	}

	private void SetupPlayerView(SaveDataManager saveMrg = null)
	{
		if (saveMrg == null)
		{
			saveMrg = SaveDataManager.Instance;
		}
		else
		{
			PlayerData playerData = saveMrg.PlayerData;
			m_playerMain = CharaType.UNKNOWN;
			m_playerSub = CharaType.UNKNOWN;
			if (playerData != null)
			{
				m_playerMain = playerData.MainChara;
				m_playerSub = playerData.SubChara;
			}
		}
		GameObject gameObject = GameObjectUtil.FindChildGameObject(base.gameObject, "info_player");
		if (!(gameObject != null))
		{
			return;
		}
		GameObject gameObject2 = GameObjectUtil.FindChildGameObject(gameObject, "player_main");
		GameObject gameObject3 = GameObjectUtil.FindChildGameObject(gameObject, "player_sub");
		GameObject parent = GameObjectUtil.FindChildGameObject(gameObject, "base_main");
		GameObject parent2 = GameObjectUtil.FindChildGameObject(gameObject, "base_sub");
		if (gameObject2 != null)
		{
			UILabel lv = GameObjectUtil.FindChildGameObjectComponent<UILabel>(gameObject2, "Lbl_player_main_lv");
			UILabel name = GameObjectUtil.FindChildGameObjectComponent<UILabel>(gameObject2, "Lbl_player_main_name");
			UITexture chara = GameObjectUtil.FindChildGameObjectComponent<UITexture>(gameObject2, "img_player_tex_main");
			UISprite genus = GameObjectUtil.FindChildGameObjectComponent<UISprite>(gameObject2, "img_player_main_genus");
			UISprite type = GameObjectUtil.FindChildGameObjectComponent<UISprite>(gameObject2, "img_player_main_speacies");
			UISprite star = GameObjectUtil.FindChildGameObjectComponent<UISprite>(parent, "base_star");
			UILabel starLv = GameObjectUtil.FindChildGameObjectComponent<UILabel>(gameObject2, "Lbl_player_main_star_lv");
			SetPlayer(m_playerMain, ref name, ref lv, ref chara, ref type, ref genus, ref star, ref starLv);
		}
		if (gameObject3 != null)
		{
			UILabel lv2 = GameObjectUtil.FindChildGameObjectComponent<UILabel>(gameObject3, "Lbl_player_sub_lv");
			UILabel name2 = GameObjectUtil.FindChildGameObjectComponent<UILabel>(gameObject3, "Lbl_player_sub_name");
			UITexture chara2 = GameObjectUtil.FindChildGameObjectComponent<UITexture>(gameObject3, "img_player_tex_sub");
			UISprite genus2 = GameObjectUtil.FindChildGameObjectComponent<UISprite>(gameObject3, "img_player_sub_genus");
			UISprite type2 = GameObjectUtil.FindChildGameObjectComponent<UISprite>(gameObject3, "img_player_sub_speacies");
			UISprite star2 = GameObjectUtil.FindChildGameObjectComponent<UISprite>(parent2, "base_star");
			UILabel starLv2 = GameObjectUtil.FindChildGameObjectComponent<UILabel>(gameObject3, "Lbl_player_sub_star_lv");
			SetPlayer(m_playerSub, ref name2, ref lv2, ref chara2, ref type2, ref genus2, ref star2, ref starLv2);
		}
		UIImageButton uIImageButton = GameObjectUtil.FindChildGameObjectComponent<UIImageButton>(gameObject, "Btn_change");
		if (uIImageButton != null)
		{
			if (m_playerMain == CharaType.UNKNOWN || m_playerSub == CharaType.UNKNOWN)
			{
				uIImageButton.gameObject.SetActive(false);
			}
			else
			{
				uIImageButton.gameObject.SetActive(true);
			}
		}
	}

	private void SetupNoticeView(BonusParamContainer bonus)
	{
		string a = string.Empty;
		string detailText;
		if (bonus.IsDetailInfo(out detailText) && m_detailTextLabel != null)
		{
			a = m_detailTextLabel.text;
			m_detailTextLabel.text = detailText;
		}
		if (!(m_detailTextBg != null) || !(m_detailTextLabel != null))
		{
			return;
		}
		TweenAlpha component = m_detailTextBg.GetComponent<TweenAlpha>();
		TweenAlpha component2 = m_detailTextLabel.GetComponent<TweenAlpha>();
		if (!string.IsNullOrEmpty(detailText))
		{
			if (component != null && component2 != null)
			{
				if (a != detailText)
				{
					component.Reset();
					component2.Reset();
					m_detailTextBg.alpha = 0f;
					m_detailTextLabel.alpha = 0f;
					component.enabled = true;
					component2.enabled = true;
					component.Play();
					component2.Play();
				}
				else if (!component.enabled)
				{
					m_detailTextBg.alpha = 0f;
					m_detailTextLabel.alpha = 0f;
					component.enabled = true;
					component2.enabled = true;
					component.Play();
					component2.Play();
				}
			}
		}
		else
		{
			m_detailTextBg.alpha = 0f;
			m_detailTextLabel.alpha = 0f;
			if (component != null && component2 != null)
			{
				component.Reset();
				component2.Reset();
				component.enabled = false;
				component2.enabled = false;
			}
		}
	}

	private void SetupAbilityIcon(BonusParamContainer bonus)
	{
		List<UISprite> icons = new List<UISprite>();
		List<UISprite> icons2 = new List<UISprite>();
		GameObject gameObject = GameObjectUtil.FindChildGameObject(base.gameObject, "info_player");
		if (gameObject != null)
		{
			GameObject gameObject2 = GameObjectUtil.FindChildGameObject(gameObject, "player_main");
			GameObject gameObject3 = GameObjectUtil.FindChildGameObject(gameObject, "player_sub");
			if (gameObject2 != null)
			{
				GameObject gameObject4 = GameObjectUtil.FindChildGameObject(gameObject2, "ability");
				if (gameObject4 != null)
				{
					for (int i = 0; i < 8; i++)
					{
						UISprite uISprite = GameObjectUtil.FindChildGameObjectComponent<UISprite>(gameObject4, "img_ability_icon_" + i);
						if (uISprite != null)
						{
							icons.Add(uISprite);
							continue;
						}
						break;
					}
				}
			}
			if (gameObject3 != null)
			{
				GameObject gameObject5 = GameObjectUtil.FindChildGameObject(gameObject3, "ability");
				if (gameObject5 != null)
				{
					for (int j = 0; j < 8; j++)
					{
						UISprite uISprite2 = GameObjectUtil.FindChildGameObjectComponent<UISprite>(gameObject5, "img_ability_icon_" + j);
						if (uISprite2 != null)
						{
							icons2.Add(uISprite2);
							continue;
						}
						break;
					}
				}
			}
		}
		if (bonus != null)
		{
			if (icons.Count > 0)
			{
				for (int k = 0; k < icons.Count; k++)
				{
					icons[k].enabled = false;
					icons[k].gameObject.SetActive(false);
				}
			}
			if (icons2.Count > 0)
			{
				for (int l = 0; l < icons2.Count; l++)
				{
					icons2[l].enabled = false;
					icons2[l].gameObject.SetActive(false);
				}
			}
			BonusParam bonusParam = bonus.GetBonusParam(0);
			BonusParam bonusParam2 = bonus.GetBonusParam(1);
			if (bonusParam != null && bonusParam.GetBonusInfo(BonusParam.BonusTarget.CHARA) != null)
			{
				Dictionary<BonusParam.BonusType, float> bonusInfo = bonusParam.GetBonusInfo(BonusParam.BonusTarget.CHARA);
				SetAbilityIconSprite(ref icons, bonusInfo, m_playerMain);
			}
			if (bonusParam2 != null && bonusParam2.GetBonusInfo(BonusParam.BonusTarget.CHARA) != null)
			{
				Dictionary<BonusParam.BonusType, float> bonusInfo2 = bonusParam2.GetBonusInfo(BonusParam.BonusTarget.CHARA);
				SetAbilityIconSprite(ref icons2, bonusInfo2, m_playerSub);
			}
			return;
		}
		if (icons.Count > 0)
		{
			for (int m = 0; m < icons.Count; m++)
			{
				icons[m].enabled = false;
			}
		}
		if (icons2.Count > 0)
		{
			for (int n = 0; n < icons2.Count; n++)
			{
				icons2[n].enabled = false;
			}
		}
	}

	private void SetAbilityIconSprite(ref List<UISprite> icons, Dictionary<BonusParam.BonusType, float> info, CharaType charaType)
	{
		if (info == null || icons == null || info.Count <= 0)
		{
			return;
		}
		int num = 0;
		Dictionary<BonusParam.BonusType, float>.KeyCollection keys = info.Keys;
		List<BonusParam.BonusType> list = new List<BonusParam.BonusType>();
		foreach (BonusParam.BonusType item in keys)
		{
			list.Add(item);
		}
		Dictionary<BonusParam.BonusType, bool> dictionary = BonusUtil.IsTeamBonus(charaType, list);
		foreach (BonusParam.BonusType item2 in keys)
		{
			if (info[item2] != 0f && dictionary != null && dictionary.ContainsKey(item2) && dictionary[item2])
			{
				string abilityIconSpriteName = BonusUtil.GetAbilityIconSpriteName(item2, info[item2]);
				if (!string.IsNullOrEmpty(abilityIconSpriteName))
				{
					icons[num].spriteName = BonusUtil.GetAbilityIconSpriteName(item2, info[item2]);
					icons[num].enabled = true;
					icons[num].gameObject.SetActive(true);
					num++;
				}
			}
		}
	}

	private void SetPlayer(CharaType charaType, ref UILabel name, ref UILabel lv, ref UITexture chara, ref UISprite type, ref UISprite genus, ref UISprite star, ref UILabel starLv)
	{
		bool flag = false;
		if (charaType != CharaType.NUM && charaType != CharaType.UNKNOWN && HudCharacterPanelUtil.CheckValidChara(charaType))
		{
			chara.gameObject.SetActive(true);
			TextureRequestChara request = new TextureRequestChara(charaType, chara);
			TextureAsyncLoadManager.Instance.Request(request);
			HudCharacterPanelUtil.SetName(charaType, name.gameObject);
			HudCharacterPanelUtil.SetLevel(charaType, lv.gameObject);
			HudCharacterPanelUtil.SetCharaType(charaType, type.gameObject);
			HudCharacterPanelUtil.SetTeamType(charaType, genus.gameObject);
			if (star != null && starLv != null && ServerInterface.PlayerState != null)
			{
				ServerCharacterState serverCharacterState = ServerInterface.PlayerState.CharacterState(charaType);
				if (serverCharacterState != null)
				{
					star.gameObject.SetActive(true);
					starLv.text = string.Empty + serverCharacterState.star;
					starLv.gameObject.SetActive(true);
				}
				else
				{
					star.gameObject.SetActive(false);
					starLv.gameObject.SetActive(false);
				}
			}
			flag = true;
		}
		if (!flag)
		{
			if (lv != null)
			{
				lv.text = string.Empty;
			}
			if (name != null)
			{
				name.text = string.Empty;
			}
			if (chara != null)
			{
				chara.gameObject.SetActive(false);
			}
			if (type != null)
			{
				type.spriteName = string.Empty;
			}
			if (genus != null)
			{
				genus.spriteName = string.Empty;
			}
			if (star != null && starLv != null)
			{
				star.gameObject.SetActive(false);
				starLv.gameObject.SetActive(false);
			}
		}
	}

	private void SetChao(int id, ref UILabel name, ref UILabel lv, ref UISprite bg, ref UISprite type, ref UITexture tex, ref UISprite chao)
	{
		DataTable.ChaoData chaoData = ChaoTable.GetChaoData(id);
		if (chaoData != null)
		{
			if (lv != null)
			{
				string text = TextManager.GetText(TextManager.TextType.TEXTTYPE_COMMON_TEXT, "MainMenu", "ui_LevelNumber").text;
				lv.text = TextUtility.Replace(text, "{PARAM}", chaoData.level.ToString());
			}
			if (name != null)
			{
				name.text = chaoData.nameTwolines;
			}
			if (bg != null)
			{
				bg.spriteName = "ui_chao_set_bg_ll_" + (int)chaoData.rarity;
			}
			if (type != null)
			{
				switch (chaoData.charaAtribute)
				{
				case CharacterAttribute.SPEED:
					type.spriteName = "ui_chao_set_type_icon_speed";
					break;
				case CharacterAttribute.FLY:
					type.spriteName = "ui_chao_set_type_icon_fly";
					break;
				case CharacterAttribute.POWER:
					type.spriteName = "ui_chao_set_type_icon_power";
					break;
				default:
					type.spriteName = string.Empty;
					break;
				}
			}
			if (tex != null)
			{
				ChaoTextureManager instance = ChaoTextureManager.Instance;
				if (instance != null)
				{
					Texture loadedTexture = ChaoTextureManager.Instance.GetLoadedTexture(id);
					if (loadedTexture != null)
					{
						tex.mainTexture = loadedTexture;
					}
					else
					{
						ChaoTextureManager.CallbackInfo info = new ChaoTextureManager.CallbackInfo(tex, null, true);
						ChaoTextureManager.Instance.GetTexture(id, info);
					}
					tex.alpha = 1f;
					chao.alpha = 0f;
					tex.enabled = true;
				}
			}
			else if (chao != null)
			{
				chao.enabled = true;
				chao.alpha = 1f;
			}
		}
		else
		{
			if (lv != null)
			{
				lv.text = string.Empty;
			}
			if (name != null)
			{
				name.text = string.Empty;
			}
			if (bg != null)
			{
				bg.spriteName = "ui_chao_set_ll_3";
			}
			if (type != null)
			{
				type.spriteName = string.Empty;
			}
			if (tex != null)
			{
				tex.alpha = 0f;
				tex.mainTexture = null;
			}
			if (chao != null)
			{
				chao.spriteName = string.Empty;
				chao.alpha = 0f;
			}
		}
	}

	private void ResetupChao(ref UITexture tex, ref UISprite chao)
	{
		if (!(chao != null) || !(tex != null) || !chao.enabled || !(chao.alpha >= 0.1f) || !(tex.alpha >= 0f) || string.IsNullOrEmpty(chao.spriteName))
		{
			return;
		}
		string s = chao.spriteName.Replace("ui_tex_chao_", string.Empty);
		int num = int.Parse(s);
		if (num < 0)
		{
			return;
		}
		ChaoTextureManager instance = ChaoTextureManager.Instance;
		if (instance != null)
		{
			Texture loadedTexture = ChaoTextureManager.Instance.GetLoadedTexture(num);
			if (loadedTexture != null)
			{
				tex.mainTexture = loadedTexture;
			}
			else
			{
				ChaoTextureManager.CallbackInfo info = new ChaoTextureManager.CallbackInfo(tex, null, true);
				ChaoTextureManager.Instance.GetTexture(num, info);
			}
			tex.alpha = 1f;
			chao.alpha = 0f;
			tex.enabled = true;
		}
	}

	private void Update()
	{
		if (m_pressBtnType != SELECT_TYPE.UNKNOWN)
		{
			m_pressTime += Time.deltaTime;
			if (m_pressTime > 0.5f)
			{
				switch (m_pressBtnType)
				{
				case SELECT_TYPE.CHARA_MAIN:
					OnReleasePlayerMain();
					break;
				case SELECT_TYPE.CHARA_SUB:
					OnReleasePlayerSub();
					break;
				case SELECT_TYPE.CHAO_MAIN:
					OnReleaseChaoMain();
					break;
				case SELECT_TYPE.CHAO_SUB:
					OnReleaseChaoSub();
					break;
				}
				m_pressTime = 0f;
				m_pressBtnType = SELECT_TYPE.UNKNOWN;
			}
		}
		if (m_chaoSpIconTime > 0f)
		{
			m_chaoSpIconTime += Time.deltaTime;
			if (m_chaoSpIconTime > 10f)
			{
				m_chaoSpIconTime = 0f;
				ResetupChaoView();
			}
		}
		float deltaTime = Time.deltaTime;
		UpdateChangeBtnDelay(deltaTime);
		UpdateLastInputTime(deltaTime);
	}

	private void ChangePlayer(SELECT_TYPE select, CharaType type)
	{
		SaveDataManager instance = SaveDataManager.Instance;
		if (instance != null && m_charaList != null && m_charaList.Count > 1 && m_charaList.Contains(type))
		{
			bool flag = false;
			switch (select)
			{
			case SELECT_TYPE.CHARA_MAIN:
				if (type != m_playerMain)
				{
					if (type == m_playerSub)
					{
						m_playerSub = m_playerMain;
					}
					m_playerMain = type;
					flag = true;
				}
				break;
			case SELECT_TYPE.CHARA_SUB:
				if (type != m_playerSub)
				{
					if (type == m_playerMain)
					{
						m_playerMain = m_playerSub;
					}
					m_playerSub = type;
					flag = true;
				}
				break;
			default:
				Debug.Log(string.Concat("ChangePlayer error select:", select, " !!!!!!"));
				break;
			}
			if (flag)
			{
				m_change = true;
				SetupPlayerView();
				SetupBonusView();
				SoundManager.SePlay("sys_menu_decide");
			}
			else
			{
				SoundManager.SePlay("sys_window_close");
			}
		}
		else
		{
			SoundManager.SePlay("sys_window_close");
		}
	}

	private void ChangePlayer(ref CharaType target, ref CharaType other, int param = 1)
	{
		SaveDataManager instance = SaveDataManager.Instance;
		if (instance != null && m_charaList != null && m_charaList.Count > 1)
		{
			CharaType charaType = target;
			ServerPlayerState playerState = ServerInterface.PlayerState;
			if (playerState != null)
			{
				int count = m_charaList.Count;
				int num = 0;
				if (target != CharaType.UNKNOWN)
				{
					for (int i = 0; i < count; i++)
					{
						if (m_charaList[i] == target)
						{
							num = i;
							break;
						}
					}
					if (m_charaList[(num + param + count) % count] == other)
					{
						num = (num + param + count) % count;
					}
				}
				else
				{
					num = 0;
					if (m_charaList[(num + param + count) % count] == other)
					{
						num = 1;
					}
				}
				int index = (num + param + count) % count;
				ServerCharacterState serverCharacterState = playerState.CharacterState(m_charaList[index]);
				if (serverCharacterState != null && serverCharacterState.IsUnlocked)
				{
					target = m_charaList[index];
				}
			}
			if (target != charaType)
			{
				SoundManager.SePlay("sys_menu_decide");
				if (target == other)
				{
					other = charaType;
				}
				m_change = true;
				SetupPlayerView();
				SetupBonusView();
			}
			else
			{
				SoundManager.SePlay("sys_window_close");
			}
		}
		else
		{
			SoundManager.SePlay("sys_window_close");
		}
	}

	private void OnPressPlayerMainUp()
	{
		if (!m_btnLock)
		{
			m_direction = -1;
			m_pressBtnType = SELECT_TYPE.CHARA_MAIN;
			m_pressTime = 0f;
		}
	}

	private void OnPressPlayerMainDown()
	{
		if (!m_btnLock)
		{
			m_direction = 1;
			m_pressBtnType = SELECT_TYPE.CHARA_MAIN;
			m_pressTime = 0f;
		}
	}

	private void OnReleasePlayerMain()
	{
		if (m_btnLock)
		{
			return;
		}
		if (m_init && m_pressBtnType == SELECT_TYPE.CHARA_MAIN)
		{
			if (m_pressTime > 0.5f)
			{
				PlayerSetWindowUI.Create(m_playerMain);
			}
			else
			{
				ResetLastInputTime(SELECT_TYPE.CHARA_MAIN);
				SetChangeBtnDelay(SELECT_TYPE.CHARA_MAIN);
				ChangePlayer(ref m_playerMain, ref m_playerSub, m_direction);
			}
		}
		m_pressBtnType = SELECT_TYPE.UNKNOWN;
	}

	private void OnPressPlayerSubUp()
	{
		if (!m_btnLock)
		{
			m_direction = -1;
			m_pressBtnType = SELECT_TYPE.CHARA_SUB;
			m_pressTime = 0f;
		}
	}

	private void OnPressPlayerSubDown()
	{
		if (!m_btnLock)
		{
			m_direction = 1;
			m_pressBtnType = SELECT_TYPE.CHARA_SUB;
			m_pressTime = 0f;
		}
	}

	private void OnReleasePlayerSub()
	{
		if (m_btnLock)
		{
			return;
		}
		if (m_init && m_pressBtnType == SELECT_TYPE.CHARA_SUB)
		{
			if (m_pressTime > 0.5f)
			{
				PlayerSetWindowUI.Create(m_playerSub);
			}
			else
			{
				ResetLastInputTime(SELECT_TYPE.CHARA_SUB);
				SetChangeBtnDelay(SELECT_TYPE.CHARA_SUB);
				ChangePlayer(ref m_playerSub, ref m_playerMain, m_direction);
			}
		}
		m_pressBtnType = SELECT_TYPE.UNKNOWN;
	}

	private void ChangeChaoSort(ChaoSort sort)
	{
		if (sort == ChaoSort.NUM || sort == ChaoSort.NONE)
		{
			return;
		}
		m_currentChaoSort = sort;
		DataTable.ChaoData[] dataTable = ChaoTable.GetDataTable();
		ChaoDataSorting chaoDataSorting = new ChaoDataSorting(m_currentChaoSort);
		if (chaoDataSorting == null)
		{
			return;
		}
		ChaoDataVisitorBase visitor = chaoDataSorting.visitor;
		if (visitor != null)
		{
			DataTable.ChaoData[] array = dataTable;
			foreach (DataTable.ChaoData chaoData in array)
			{
				chaoData.accept(ref visitor);
			}
			m_chaoList = chaoDataSorting.GetChaoList();
		}
	}

	private bool ChangeChao(SELECT_TYPE select, int type)
	{
		bool flag = false;
		SaveDataManager instance = SaveDataManager.Instance;
		if (instance != null && m_chaoList != null && m_chaoList.Count > 1)
		{
			switch (select)
			{
			case SELECT_TYPE.CHAO_MAIN:
				if (type != m_chaoMainId)
				{
					if (type == m_chaoSubId)
					{
						m_chaoSubId = m_chaoMainId;
					}
					m_chaoMainId = type;
					flag = true;
				}
				break;
			case SELECT_TYPE.CHAO_SUB:
				if (type != m_chaoSubId)
				{
					if (type == m_chaoMainId)
					{
						m_chaoMainId = m_chaoSubId;
					}
					m_chaoSubId = type;
					flag = true;
				}
				break;
			default:
				Debug.Log(string.Concat("ChangePlayer error select:", select, " !!!!!!"));
				break;
			}
			if (flag)
			{
				m_change = true;
				SetupChaoView();
				SetupBonusView();
				SoundManager.SePlay("sys_menu_decide");
			}
			else
			{
				SoundManager.SePlay("sys_window_close");
			}
		}
		else
		{
			SoundManager.SePlay("sys_window_close");
		}
		return flag;
	}

	private bool ChangeChao(ref int target, ref int other, int param = 1)
	{
		bool result = false;
		bool flag = false;
		if (target >= 0 && m_chaoList != null && m_chaoList.Count > 1)
		{
			flag = true;
		}
		else if (target == -1 && m_chaoList != null && m_chaoList.Count > 0 && ((other >= 0 && m_chaoList.Count > 1) || (other < 0 && m_chaoList.Count > 0)))
		{
			flag = true;
		}
		if (flag)
		{
			int count = m_chaoList.Count;
			int num = -1;
			if (target >= 0)
			{
				for (int i = 0; i < count; i++)
				{
					if (target == m_chaoList[i].id)
					{
						num = i;
						break;
					}
				}
			}
			else
			{
				num = 0;
				int num2 = (num + param + count) % count;
				if (m_chaoList.Count > num2 && m_chaoList[num2] != null)
				{
					if (other >= 0)
					{
						int id = m_chaoList[num2].id;
						if (id == other)
						{
							num2 = (num + param + param + count) % count;
							num = ((m_chaoList.Count > num2 && m_chaoList[num2] != null) ? 1 : (-1));
						}
					}
				}
				else
				{
					num = -1;
				}
			}
			if (num >= 0)
			{
				if (param != 0)
				{
					int num3 = target;
					int num4 = (num + param + count) % count;
					if (num4 >= 0 && num4 < m_chaoList.Count && m_chaoList[num4] != null)
					{
						if (m_chaoList[num4].id == other)
						{
							num4 = (num + param + param + count) % count;
							if (num4 < 0 || num4 >= m_chaoList.Count || m_chaoList[num4] == null)
							{
								return false;
							}
						}
						target = m_chaoList[num4].id;
						if (target != num3)
						{
							result = true;
						}
					}
				}
				else
				{
					result = true;
				}
			}
		}
		return result;
	}

	private void OnPressChaoMainUp()
	{
		if (!m_btnLock)
		{
			m_direction = -1;
			m_pressBtnType = SELECT_TYPE.CHAO_MAIN;
			m_pressTime = 0f;
		}
	}

	private void OnPressChaoMainDown()
	{
		if (!m_btnLock)
		{
			m_direction = 1;
			m_pressBtnType = SELECT_TYPE.CHAO_MAIN;
			m_pressTime = 0f;
		}
	}

	private void OnReleaseChaoMain()
	{
		if (m_btnLock)
		{
			return;
		}
		if (m_init && m_pressBtnType == SELECT_TYPE.CHAO_MAIN)
		{
			SoundManager.SePlay("sys_menu_decide");
			ResetLastInputTime(SELECT_TYPE.CHAO_MAIN);
			if (m_pressTime > 0.5f)
			{
				ChaoSetWindowUI window = ChaoSetWindowUI.GetWindow();
				if (window != null)
				{
					DataTable.ChaoData chaoData = ChaoTable.GetChaoData(m_chaoMainId);
					if (chaoData != null)
					{
						ChaoSetWindowUI.ChaoInfo chaoInfo = new ChaoSetWindowUI.ChaoInfo(chaoData);
						chaoInfo.level = chaoData.level;
						chaoInfo.detail = chaoData.GetDetailLevelPlusSP(chaoInfo.level);
						window.OpenWindow(chaoInfo, ChaoSetWindowUI.WindowType.WINDOW_ONLY);
					}
				}
			}
			else if (ChangeChao(ref m_chaoMainId, ref m_chaoSubId, m_direction))
			{
				m_change = true;
				SetupChaoView();
				SetupBonusView();
				SetChangeBtnDelay(SELECT_TYPE.CHAO_MAIN);
			}
		}
		m_pressBtnType = SELECT_TYPE.UNKNOWN;
	}

	private void OnPressChaoSubUp()
	{
		if (!m_btnLock)
		{
			m_direction = -1;
			m_pressBtnType = SELECT_TYPE.CHAO_SUB;
			m_pressTime = 0f;
		}
	}

	private void OnPressChaoSubDown()
	{
		if (!m_btnLock)
		{
			m_direction = 1;
			m_pressBtnType = SELECT_TYPE.CHAO_SUB;
			m_pressTime = 0f;
		}
	}

	private void OnReleaseChaoSub()
	{
		if (m_btnLock)
		{
			return;
		}
		if (m_init && m_pressBtnType == SELECT_TYPE.CHAO_SUB)
		{
			SoundManager.SePlay("sys_menu_decide");
			ResetLastInputTime(SELECT_TYPE.CHAO_SUB);
			if (m_pressTime > 0.5f)
			{
				ChaoSetWindowUI window = ChaoSetWindowUI.GetWindow();
				if (window != null)
				{
					DataTable.ChaoData chaoData = ChaoTable.GetChaoData(m_chaoSubId);
					if (chaoData != null)
					{
						ChaoSetWindowUI.ChaoInfo chaoInfo = new ChaoSetWindowUI.ChaoInfo(chaoData);
						chaoInfo.level = chaoData.level;
						chaoInfo.detail = chaoData.GetDetailLevelPlusSP(chaoInfo.level);
						window.OpenWindow(chaoInfo, ChaoSetWindowUI.WindowType.WINDOW_ONLY);
					}
				}
			}
			else if (ChangeChao(ref m_chaoSubId, ref m_chaoMainId, m_direction))
			{
				m_change = true;
				SetupChaoView();
				SetupBonusView();
				SetChangeBtnDelay(SELECT_TYPE.CHAO_SUB);
			}
		}
		m_pressBtnType = SELECT_TYPE.UNKNOWN;
	}

	private void OnClickChange()
	{
		if (!m_btnLock)
		{
			ResetLastInputTime();
			if (m_playerMain != CharaType.UNKNOWN && m_playerSub != CharaType.UNKNOWN)
			{
				SoundManager.SePlay("sys_menu_decide");
				CharaType playerMain = m_playerMain;
				CharaType charaType = m_playerMain = m_playerSub;
				m_playerSub = playerMain;
				m_change = true;
				SetupPlayerView();
				SetupBonusView();
			}
			else
			{
				SoundManager.SePlay("sys_window_close");
			}
		}
	}

	private void OnClickTab1()
	{
		if (m_currentChaoSetStock != 0 && !m_close)
		{
			DeckSetLoad(0);
			SoundManager.SePlay("sys_menu_decide");
		}
	}

	private void OnClickTab2()
	{
		if (m_currentChaoSetStock != 1 && !m_close)
		{
			DeckSetLoad(1);
			SoundManager.SePlay("sys_menu_decide");
		}
	}

	private void OnClickTab3()
	{
		if (m_currentChaoSetStock != 2 && !m_close)
		{
			DeckSetLoad(2);
			SoundManager.SePlay("sys_menu_decide");
		}
	}

	private void OnClickTab4()
	{
		if (m_currentChaoSetStock != 3 && !m_close)
		{
			DeckSetLoad(3);
			SoundManager.SePlay("sys_menu_decide");
		}
	}

	private void OnClickTab5()
	{
		if (m_currentChaoSetStock != 4 && !m_close)
		{
			DeckSetLoad(4);
			SoundManager.SePlay("sys_menu_decide");
		}
	}

	private void OnClickClose()
	{
		if (m_btnLock || !m_init || m_close)
		{
			return;
		}
		SoundManager.SePlay("sys_menu_decide");
		m_init = false;
		m_close = true;
		UIPlayAnimation[] componentsInChildren = base.gameObject.GetComponentsInChildren<UIPlayAnimation>();
		if (componentsInChildren != null && componentsInChildren.Length > 0)
		{
			UIPlayAnimation[] array = componentsInChildren;
			foreach (UIPlayAnimation uIPlayAnimation in array)
			{
				uIPlayAnimation.enabled = false;
			}
		}
		ResetRequestData(false);
		if (m_change && m_initPlayerMain == m_playerMain && m_initPlayerSub == m_playerSub && m_initChaoMain == m_chaoMainId && m_initChaoSub == m_chaoSubId)
		{
			m_change = false;
			if (m_initChaoSetStock != m_currentChaoSetStock)
			{
				DeckUtil.SetDeckCurrentStockIndex(m_currentChaoSetStock);
				m_parent.SendMessage("OnMsgReset", SendMessageOptions.DontRequireReceiver);
			}
		}
		if (m_change)
		{
			m_change = false;
			ServerInterface loggedInServerInterface = ServerInterface.LoggedInServerInterface;
			if (!GeneralUtil.IsNetwork() || loggedInServerInterface == null)
			{
				DeckUtil.SetDeckCurrentStockIndex(m_initChaoSetStock);
				DeckUtil.DeckSetSave(m_initChaoSetStock, m_initPlayerMain, m_initPlayerSub, m_initChaoMain, m_initChaoSub);
				GeneralUtil.SetCharasetBtnIcon(m_initPlayerMain, m_initPlayerSub, m_initChaoMain, m_initChaoSub, m_parent);
				GeneralUtil.ShowNoCommunication();
				ChaoTextureManager.Instance.RemoveChaoTextureForMainMenuEnd();
				if (m_parent != null)
				{
					m_parent.SendMessage("OnMsgDeckViewWindowNetworkError", SendMessageOptions.DontRequireReceiver);
				}
				CloseWindowAnim();
				return;
			}
			GeneralUtil.SetCharasetBtnIcon(m_playerMain, m_playerSub, m_chaoMainId, m_chaoSubId, m_parent);
			SetRequestData();
			DeckUtil.DeckSetSave(m_currentChaoSetStock, m_playerMain, m_playerSub, m_chaoMainId, m_chaoSubId);
			DeckUtil.SetDeckCurrentStockIndex(m_currentChaoSetStock);
			PlayerDataUpdate();
			if (loggedInServerInterface != null && m_playerMain != m_playerSub)
			{
				if (m_initChaoMain != m_chaoMainId || m_initChaoSub != m_chaoSubId)
				{
					int id = (int)ServerItem.CreateFromChaoId(m_chaoMainId).id;
					int id2 = (int)ServerItem.CreateFromChaoId(m_chaoSubId).id;
					loggedInServerInterface.RequestServerEquipChao(id, id2, base.gameObject);
				}
				else
				{
					ServerEquipChao_Dummy();
				}
			}
			m_parent.SendMessage("OnMsgReset", SendMessageOptions.DontRequireReceiver);
		}
		else
		{
			if (m_parent != null)
			{
				m_parent.SendMessage("OnMsgDeckViewWindowNotChange", SendMessageOptions.DontRequireReceiver);
			}
			CloseWindowAnim();
		}
		if (m_parent != null)
		{
			m_parent.SendMessage("OnMsgDeckViewWindowEnd", SendMessageOptions.DontRequireReceiver);
		}
	}

	private void OnClickSelect()
	{
		if (m_btnLock || !m_init || m_close)
		{
			return;
		}
		SoundManager.SePlay("sys_menu_decide");
		m_init = false;
		m_close = true;
		UIPlayAnimation[] componentsInChildren = base.gameObject.GetComponentsInChildren<UIPlayAnimation>();
		if (componentsInChildren != null && componentsInChildren.Length > 0)
		{
			UIPlayAnimation[] array = componentsInChildren;
			foreach (UIPlayAnimation uIPlayAnimation in array)
			{
				uIPlayAnimation.enabled = false;
			}
		}
		ResetRequestData(false);
		if (m_change && m_initPlayerMain == m_playerMain && m_initPlayerSub == m_playerSub && m_initChaoMain == m_chaoMainId && m_initChaoSub == m_chaoSubId)
		{
			m_change = false;
			if (m_initChaoSetStock != m_currentChaoSetStock)
			{
				DeckUtil.SetDeckCurrentStockIndex(m_currentChaoSetStock);
			}
		}
		if (m_change)
		{
			m_change = false;
			ServerInterface loggedInServerInterface = ServerInterface.LoggedInServerInterface;
			if (!GeneralUtil.IsNetwork() || loggedInServerInterface == null)
			{
				DeckUtil.SetDeckCurrentStockIndex(m_initChaoSetStock);
				DeckUtil.DeckSetSave(m_initChaoSetStock, m_initPlayerMain, m_initPlayerSub, m_initChaoMain, m_initChaoSub);
				GeneralUtil.ShowNoCommunication();
				ChaoTextureManager.Instance.RemoveChaoTextureForMainMenuEnd();
				HudMenuUtility.SendStartPlayerChaoPage();
				if (m_parent != null)
				{
					m_parent.SendMessage("OnMsgDeckViewWindowNetworkError", SendMessageOptions.DontRequireReceiver);
				}
				CloseWindowAnim();
				return;
			}
			SetRequestData();
			DeckUtil.DeckSetSave(m_currentChaoSetStock, m_playerMain, m_playerSub, m_chaoMainId, m_chaoSubId);
			DeckUtil.SetDeckCurrentStockIndex(m_currentChaoSetStock);
			PlayerDataUpdate();
			if (loggedInServerInterface != null && m_playerMain != m_playerSub)
			{
				if (m_initChaoMain != m_chaoMainId || m_initChaoSub != m_chaoSubId)
				{
					int id = (int)ServerItem.CreateFromChaoId(m_chaoMainId).id;
					int id2 = (int)ServerItem.CreateFromChaoId(m_chaoSubId).id;
					loggedInServerInterface.RequestServerEquipChao(id, id2, base.gameObject);
				}
				else
				{
					ServerEquipChao_Dummy();
				}
			}
		}
		else
		{
			if (m_parent != null)
			{
				m_parent.SendMessage("OnMsgDeckViewWindowNotChange", SendMessageOptions.DontRequireReceiver);
			}
			CloseWindowAnim();
		}
		if (m_parent != null)
		{
			m_parent.SendMessage("OnMsgDeckViewWindowEnd", SendMessageOptions.DontRequireReceiver);
		}
		HudMenuUtility.SendStartPlayerChaoPage();
	}

	private void UpdateChangeBtnDelay(float delteTime)
	{
		if (m_changeDelayCheckTime > 0f)
		{
			m_changeDelayCheckTime -= delteTime;
			if (m_changeDelayCheckTime <= 0f)
			{
				m_changeDelayCheckTime = 0f;
				SetAllChangeBtnEnabled(true);
			}
		}
	}

	private void SetAllChangeBtnEnabled(bool enabled)
	{
		if (m_changeBtnList == null || m_changeBtnList.Count <= 0)
		{
			return;
		}
		Dictionary<SELECT_TYPE, List<UIImageButton>>.KeyCollection keys = m_changeBtnList.Keys;
		foreach (SELECT_TYPE item in keys)
		{
			if (m_changeBtnList[item] == null || m_changeBtnList[item].Count <= 0)
			{
				continue;
			}
			foreach (UIImageButton item2 in m_changeBtnList[item])
			{
				if (item2 != null)
				{
					item2.isEnabled = enabled;
				}
			}
		}
	}

	private void UpdateLastInputTime(float delteTime)
	{
		if (!m_isLastInputTime)
		{
			return;
		}
		int num = 4;
		if (m_lastInputTime == null)
		{
			m_lastInputTime = new Dictionary<SELECT_TYPE, float>();
			for (int i = 0; i < num; i++)
			{
				SELECT_TYPE key = (SELECT_TYPE)i;
				m_lastInputTime.Add(key, delteTime);
			}
			return;
		}
		GameObject gameObject = null;
		GameObject gameObject2 = null;
		UITexture uITexture = null;
		int num2 = 0;
		for (int j = 0; j < num; j++)
		{
			SELECT_TYPE sELECT_TYPE = (SELECT_TYPE)j;
			if (!m_lastInputTime.ContainsKey(sELECT_TYPE))
			{
				continue;
			}
			if (m_lastInputTime[sELECT_TYPE] >= 0f)
			{
				Dictionary<SELECT_TYPE, float> lastInputTime;
				Dictionary<SELECT_TYPE, float> dictionary = lastInputTime = m_lastInputTime;
				SELECT_TYPE key2;
				SELECT_TYPE key3 = key2 = sELECT_TYPE;
				float num3 = lastInputTime[key2];
				dictionary[key3] = num3 + delteTime;
				if (!(m_lastInputTime[sELECT_TYPE] > 0.75f))
				{
					continue;
				}
				switch (sELECT_TYPE)
				{
				case SELECT_TYPE.CHAO_MAIN:
					gameObject = GameObjectUtil.FindChildGameObject(base.gameObject, "info_chao");
					gameObject2 = GameObjectUtil.FindChildGameObject(gameObject, "chao_main");
					if (gameObject2 != null && m_chaoMainId >= 0)
					{
						uITexture = GameObjectUtil.FindChildGameObjectComponent<UITexture>(gameObject2, "img_chao_text_main");
						if (!(uITexture != null))
						{
							break;
						}
						if (GeneralUtil.CheckChaoTexture(uITexture, m_chaoMainId))
						{
							m_lastInputTime[sELECT_TYPE] = -1f;
							break;
						}
						Texture loadedTexture2 = ChaoTextureManager.Instance.GetLoadedTexture(m_chaoMainId);
						if (loadedTexture2 != null)
						{
							uITexture.mainTexture = loadedTexture2;
						}
						else
						{
							ChaoTextureManager.CallbackInfo info2 = new ChaoTextureManager.CallbackInfo(uITexture, null, true);
							ChaoTextureManager.Instance.GetTexture(m_chaoMainId, info2);
						}
						uITexture.alpha = 1f;
						uITexture.enabled = true;
					}
					else
					{
						m_lastInputTime[sELECT_TYPE] = -1f;
					}
					break;
				case SELECT_TYPE.CHAO_SUB:
					gameObject = GameObjectUtil.FindChildGameObject(base.gameObject, "info_chao");
					gameObject2 = GameObjectUtil.FindChildGameObject(gameObject, "chao_sub");
					if (gameObject2 != null && m_chaoSubId >= 0)
					{
						uITexture = GameObjectUtil.FindChildGameObjectComponent<UITexture>(gameObject2, "img_chao_text_sub");
						if (!(uITexture != null))
						{
							break;
						}
						if (GeneralUtil.CheckChaoTexture(uITexture, m_chaoSubId))
						{
							m_lastInputTime[sELECT_TYPE] = -1f;
							break;
						}
						Texture loadedTexture = ChaoTextureManager.Instance.GetLoadedTexture(m_chaoSubId);
						if (loadedTexture != null)
						{
							uITexture.mainTexture = loadedTexture;
						}
						else
						{
							ChaoTextureManager.CallbackInfo info = new ChaoTextureManager.CallbackInfo(uITexture, null, true);
							ChaoTextureManager.Instance.GetTexture(m_chaoSubId, info);
						}
						uITexture.alpha = 1f;
						uITexture.enabled = true;
					}
					else
					{
						m_lastInputTime[sELECT_TYPE] = -1f;
					}
					break;
				default:
					m_lastInputTime[sELECT_TYPE] = -1f;
					break;
				}
			}
			else
			{
				num2++;
			}
		}
		if (num2 >= num)
		{
			m_isLastInputTime = false;
		}
	}

	private void ResetLastInputTime(SELECT_TYPE targetType = SELECT_TYPE.NUM)
	{
		m_isLastInputTime = true;
		if (m_lastInputTime == null)
		{
			m_lastInputTime = new Dictionary<SELECT_TYPE, float>();
			int num = 4;
			for (int i = 0; i < num; i++)
			{
				SELECT_TYPE key = (SELECT_TYPE)i;
				m_lastInputTime.Add(key, 0f);
			}
			return;
		}
		if (targetType != SELECT_TYPE.NUM)
		{
			if (m_lastInputTime.ContainsKey(targetType))
			{
				m_lastInputTime[targetType] = 0f;
			}
			return;
		}
		int num2 = 4;
		for (int j = 0; j < num2; j++)
		{
			SELECT_TYPE key2 = (SELECT_TYPE)j;
			if (m_lastInputTime.ContainsKey(key2))
			{
				m_lastInputTime[key2] = 0f;
			}
		}
	}

	public override void OnClickPlatformBackButton(BackButtonMessage msg)
	{
		if (m_close)
		{
			return;
		}
		if (msg != null)
		{
			msg.StaySequence();
		}
		if (PlayerSetWindowUI.isActive || ChaoSetWindowUI.isActive || m_btnLock)
		{
			return;
		}
		GameObject gameObject = GameObjectUtil.FindChildGameObject(base.gameObject, "Btn_ok");
		if (gameObject != null)
		{
			UIButtonMessage component = gameObject.GetComponent<UIButtonMessage>();
			if (component != null)
			{
				component.SendMessage("OnClick");
			}
		}
	}

	private void SetRequestData()
	{
		m_reqChaoSetStock = m_currentChaoSetStock;
		m_reqPlayerMain = m_playerMain;
		m_reqPlayerSub = m_playerSub;
		m_reqChaoMain = m_chaoMainId;
		m_reqChaoSub = m_chaoSubId;
	}

	private void PlayerDataUpdate()
	{
		if (m_reqChaoSetStock < 0)
		{
			return;
		}
		DeckUtil.SetDeckCurrentStockIndex(m_reqChaoSetStock);
		SystemSaveManager instance = SystemSaveManager.Instance;
		if (instance != null)
		{
			SystemData systemdata = instance.GetSystemdata();
			if (systemdata != null)
			{
				SaveDataManager instance2 = SaveDataManager.Instance;
				instance2.PlayerData.MainChara = m_reqPlayerMain;
				instance2.PlayerData.SubChara = m_reqPlayerSub;
				instance2.PlayerData.MainChaoID = m_reqChaoMain;
				instance2.PlayerData.SubChaoID = m_reqChaoSub;
			}
		}
	}

	private void ResetRequestData(bool isSavaDataUpdate)
	{
		if (isSavaDataUpdate && m_reqChaoSetStock >= 0)
		{
			SystemSaveManager instance = SystemSaveManager.Instance;
			if (instance != null)
			{
				SystemData systemdata = instance.GetSystemdata();
				if (systemdata != null)
				{
					SaveDataManager instance2 = SaveDataManager.Instance;
					instance2.PlayerData.MainChara = m_reqPlayerMain;
					instance2.PlayerData.SubChara = m_reqPlayerSub;
					instance2.PlayerData.MainChaoID = m_reqChaoMain;
					instance2.PlayerData.SubChaoID = m_reqChaoSub;
					HudMenuUtility.SendMsgUpdateSaveDataDisplay();
				}
			}
		}
		m_reqChaoSetStock = -1;
		m_reqPlayerMain = CharaType.UNKNOWN;
		m_reqPlayerSub = CharaType.UNKNOWN;
		m_reqChaoMain = -1;
		m_reqChaoSub = -1;
	}

	private void CloseWindowAnim()
	{
		if (m_windowAnimation != null)
		{
			ActiveAnimation activeAnimation = ActiveAnimation.Play(m_windowAnimation, Direction.Reverse);
			EventDelegate.Add(activeAnimation.onFinished, WindowAnimationFinishCallback, true);
		}
	}

	private void WindowAnimationFinishCallback()
	{
		if (m_close)
		{
			OnFinished();
		}
	}

	private void ServerChangeCharacter_Succeeded(MsgGetPlayerStateSucceed msg)
	{
		ResetRequestData(true);
		ItemSetMenu.UpdateBoostItemForCharacterDeck();
		if (m_parent != null)
		{
			m_parent.SendMessage("OnMsgDeckViewWindowChange", SendMessageOptions.DontRequireReceiver);
		}
		CloseWindowAnim();
	}

	private void ServerChangeCharacter_Dummy()
	{
		ResetRequestData(true);
		ItemSetMenu.UpdateBoostItemForCharacterDeck();
		if (m_parent != null)
		{
			m_parent.SendMessage("OnMsgDeckViewWindowChange", SendMessageOptions.DontRequireReceiver);
		}
		CloseWindowAnim();
	}

	private void ServerEquipChao_Succeeded(MsgGetPlayerStateSucceed msg)
	{
		ServerInterface loggedInServerInterface = ServerInterface.LoggedInServerInterface;
		bool flag = false;
		if (!(loggedInServerInterface != null))
		{
			return;
		}
		if (m_initPlayerMain != m_reqPlayerMain)
		{
			flag = true;
		}
		if (m_initPlayerSub != m_reqPlayerSub)
		{
			flag = true;
		}
		if (flag)
		{
			int mainCharaId = -1;
			int subCharaId = -1;
			ServerCharacterState serverCharacterState = ServerInterface.PlayerState.CharacterState(m_reqPlayerMain);
			ServerCharacterState serverCharacterState2 = ServerInterface.PlayerState.CharacterState(m_reqPlayerSub);
			if (serverCharacterState != null)
			{
				mainCharaId = serverCharacterState.Id;
			}
			if (serverCharacterState2 != null)
			{
				subCharaId = serverCharacterState2.Id;
			}
			loggedInServerInterface.RequestServerChangeCharacter(mainCharaId, subCharaId, base.gameObject);
		}
		else
		{
			ServerChangeCharacter_Dummy();
		}
	}

	private void ServerEquipChao_Dummy()
	{
		ServerInterface loggedInServerInterface = ServerInterface.LoggedInServerInterface;
		bool flag = false;
		if (!(loggedInServerInterface != null))
		{
			return;
		}
		if (m_initPlayerMain != m_reqPlayerMain)
		{
			flag = true;
		}
		if (m_initPlayerSub != m_reqPlayerSub)
		{
			flag = true;
		}
		if (flag)
		{
			int mainCharaId = -1;
			int subCharaId = -1;
			ServerCharacterState serverCharacterState = ServerInterface.PlayerState.CharacterState(m_reqPlayerMain);
			ServerCharacterState serverCharacterState2 = ServerInterface.PlayerState.CharacterState(m_reqPlayerSub);
			if (serverCharacterState != null)
			{
				mainCharaId = serverCharacterState.Id;
			}
			if (serverCharacterState2 != null)
			{
				subCharaId = serverCharacterState2.Id;
			}
			loggedInServerInterface.RequestServerChangeCharacter(mainCharaId, subCharaId, base.gameObject);
		}
		else
		{
			ServerChangeCharacter_Dummy();
		}
	}

	private void OnFinished()
	{
		if (m_bgCollider != null)
		{
			m_bgCollider.enabled = false;
		}
		GameObject gameObject = GameObjectUtil.FindChildGameObject(base.gameObject, "anime_blinder");
		if (gameObject != null)
		{
			gameObject.SetActive(false);
		}
		if (m_windowRoot == null)
		{
			m_windowRoot = GameObjectUtil.FindChildGameObject(base.gameObject, "window");
		}
		if (m_windowRoot != null)
		{
			m_windowRoot.SetActive(false);
		}
		base.gameObject.SetActive(false);
	}

	public void DeckSetLoad(int stock)
	{
		DeckUtil.DeckSetSave(m_currentChaoSetStock, m_playerMain, m_playerSub, m_chaoMainId, m_chaoSubId);
		if (DeckUtil.DeckSetLoad(stock, ref m_playerMain, ref m_playerSub, ref m_chaoMainId, ref m_chaoSubId))
		{
			m_change = true;
			SetupChaoView();
			SetupBonusView();
			SetupPlayerView();
			m_currentChaoSetStock = stock;
			SetupTabView();
			DeckUtil.SetDeckCurrentStockIndex(m_currentChaoSetStock);
		}
	}

	public bool ChaoSetName(int stock, out string main, out string sub)
	{
		main = string.Empty;
		sub = string.Empty;
		if (stock < 0 || stock >= 3)
		{
			return false;
		}
		bool result = false;
		SystemSaveManager instance = SystemSaveManager.Instance;
		if (instance != null)
		{
			SystemData systemdata = instance.GetSystemdata();
			if (systemdata != null)
			{
				int mainChaoId = -1;
				int subChaoId = -1;
				systemdata.GetDeckData(stock, out mainChaoId, out subChaoId);
				if (mainChaoId >= 0 || subChaoId >= 0)
				{
					if (mainChaoId >= 0)
					{
						DataTable.ChaoData chaoData = ChaoTable.GetChaoData(mainChaoId);
						main = chaoData.name;
					}
					if (subChaoId >= 0)
					{
						DataTable.ChaoData chaoData = ChaoTable.GetChaoData(subChaoId);
						sub = chaoData.name;
					}
					result = true;
				}
			}
		}
		return result;
	}

	private void SetChangeBtnDelay(SELECT_TYPE type)
	{
		SetAllChangeBtnEnabled(false);
		m_changeDelayCheckTime = 0.5f;
	}

	public static void Reset()
	{
		GameObject cameraUIObject = HudMenuUtility.GetCameraUIObject();
		if (!(cameraUIObject != null))
		{
			return;
		}
		GameObject gameObject = GameObjectUtil.FindChildGameObject(cameraUIObject, "DeckViewWindow");
		if (gameObject != null)
		{
			DeckViewWindow deckViewWindow = GameObjectUtil.FindChildGameObjectComponent<DeckViewWindow>(gameObject.gameObject, "DeckViewWindow");
			if (deckViewWindow != null)
			{
				deckViewWindow.Init();
			}
		}
	}

	public static DeckViewWindow Create(GameObject parent = null)
	{
		SaveDataManager instance = SaveDataManager.Instance;
		if (instance != null)
		{
			PlayerData playerData = instance.PlayerData;
			if (playerData != null)
			{
				Create(playerData.MainChaoID, playerData.SubChaoID, parent);
			}
		}
		return null;
	}

	public static DeckViewWindow Create(int mainChaoId, int subChaoId, GameObject parent = null)
	{
		GameObject cameraUIObject = HudMenuUtility.GetCameraUIObject();
		if (cameraUIObject != null)
		{
			GameObject gameObject = GameObjectUtil.FindChildGameObject(cameraUIObject, "DeckViewWindow");
			DeckViewWindow deckViewWindow = null;
			if (gameObject != null)
			{
				deckViewWindow = gameObject.GetComponent<DeckViewWindow>();
				if (deckViewWindow == null)
				{
					deckViewWindow = gameObject.AddComponent<DeckViewWindow>();
				}
				if (deckViewWindow != null)
				{
					deckViewWindow.Setup(mainChaoId, subChaoId, parent);
				}
			}
			return deckViewWindow;
		}
		return null;
	}
}
