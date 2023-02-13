using AnimationOrTween;
using System.Collections.Generic;
using UnityEngine;

public class RouletteStandardPart : RoulettePartsBase
{
	private const string FADE_ANIM_INTRO = "ui_simple_load_intro_Anim2";

	private const string FADE_ANIM_OUTRO = "ui_simple_load_outro_Anim2";

	private const float FRONT_COLLIDER_DELAY = 0.25f;

	private const float EVENT_UI_UPDATE_TIME = 10f;

	[SerializeField]
	private Animation m_wordAnim;

	[SerializeField]
	private GameObject m_wordGet;

	[SerializeField]
	private GameObject m_wordRankup;

	[SerializeField]
	private GameObject m_wordJackpot;

	[SerializeField]
	private GameObject m_wordLavel;

	[SerializeField]
	private GameObject m_spEgg;

	[SerializeField]
	private List<GameObject> m_Eggs;

	[SerializeField]
	private GameObject m_backButton;

	[SerializeField]
	private GameObject m_oddsButton;

	[SerializeField]
	private List<GameObject> m_spinButtons;

	[SerializeField]
	private GameObject m_costBase;

	[SerializeField]
	private GameObject m_eventUI;

	[SerializeField]
	private GameObject m_frontCollider;

	[SerializeField]
	private Animation m_fadeAnime;

	private float m_frontColliderDelay;

	private int m_remainingNum;

	private int m_remainingOffset;

	private float m_animeTime;

	private ServerWheelOptionsData.SPIN_BUTTON m_spinBtn = ServerWheelOptionsData.SPIN_BUTTON.NONE;

	private bool m_spinBtnActive;

	private string m_spinErrorWindow;

	private UIImageButton m_backButtonImg;

	private List<int> m_spinCostList;

	private List<ServerItem> m_attentionItemList;

	private bool m_isJackpot;

	private GameObject m_spinMultiButton;

	private List<GameObject> m_costList;

	private List<Constants.Campaign.emType> m_campaign;

	private RouletteCategory m_currentCategory;

	private int m_eventUiCount;

	private float m_eventUiNextUpdate = -1f;

	protected override void UpdateParts()
	{
		if (m_backButtonImg != null)
		{
			if (base.isSpin && base.spinDecisionIndex == -1)
			{
				m_backButtonImg.gameObject.SetActive(true);
				if (m_parent.spinTime > 10f)
				{
					m_backButtonImg.isEnabled = true;
				}
			}
			else
			{
				m_backButtonImg.gameObject.SetActive(false);
			}
		}
		if (!string.IsNullOrEmpty(m_spinErrorWindow))
		{
			if (GeneralWindow.IsCreated(m_spinErrorWindow))
			{
				if (GeneralWindow.IsButtonPressed)
				{
					if (GeneralWindow.IsYesButtonPressed)
					{
						switch (m_spinBtn)
						{
						case ServerWheelOptionsData.SPIN_BUTTON.RING:
							HudMenuUtility.SendMenuButtonClicked(ButtonInfoTable.ButtonType.RING_TO_SHOP);
							break;
						case ServerWheelOptionsData.SPIN_BUTTON.RSRING:
							HudMenuUtility.SendMenuButtonClicked(ButtonInfoTable.ButtonType.REDSTAR_TO_SHOP);
							break;
						}
					}
					GeneralWindow.Close();
					m_spinErrorWindow = null;
				}
			}
			else
			{
				GeneralWindow.Close();
				m_spinErrorWindow = null;
			}
		}
		if (RouletteManager.GetCurrentLoading() != null && RouletteManager.GetCurrentLoading().Count > 0)
		{
			if (m_frontCollider != null && !m_frontCollider.activeSelf)
			{
				m_frontCollider.SetActive(true);
			}
			m_frontColliderDelay = 0.25f;
		}
		else if (m_parent != null && (m_parent.isSpinGetWindow || m_parent.isWordAnime))
		{
			if (m_frontCollider != null && !m_frontCollider.activeSelf)
			{
				m_frontCollider.SetActive(true);
			}
			m_frontColliderDelay = 0.25f;
		}
		else if (base.isDelay)
		{
			if (m_frontCollider != null && !m_frontCollider.activeSelf)
			{
				m_frontCollider.SetActive(true);
			}
			if (m_frontColliderDelay < 0.0625f)
			{
				m_frontColliderDelay = 0.0625f;
			}
		}
		if (m_frontColliderDelay > 0f)
		{
			m_frontColliderDelay -= Time.deltaTime / Time.timeScale;
			if (m_frontColliderDelay <= 0f)
			{
				if (m_frontCollider != null)
				{
					m_frontCollider.SetActive(false);
				}
				m_frontColliderDelay = 0f;
			}
		}
		if (m_animeTime > 0f)
		{
			m_animeTime -= Time.deltaTime;
			if (m_animeTime <= 0f)
			{
				AnimationFinishCallback();
				m_animeTime = 0f;
				if (m_wordGet != null)
				{
					m_wordGet.SetActive(false);
				}
				if (m_wordJackpot != null)
				{
					m_wordJackpot.SetActive(false);
				}
				if (m_wordLavel != null)
				{
					m_wordLavel.SetActive(false);
				}
				if (m_wordRankup != null)
				{
					m_wordRankup.SetActive(false);
				}
				if (m_isJackpot)
				{
					m_isJackpot = false;
				}
			}
		}
		if (m_currentCategory != 0 && m_currentCategory != RouletteCategory.ITEM && m_attentionItemList == null)
		{
			if (RouletteManager.Instance != null && !RouletteManager.Instance.isCurrentPrizeLoading)
			{
				m_attentionItemList = base.wheel.GetAttentionItemList();
				if (m_attentionItemList != null)
				{
					m_eventUiNextUpdate = 0f;
					SetEventUI();
				}
			}
		}
		else if (m_eventUiNextUpdate > 0f)
		{
			m_eventUiNextUpdate -= Time.deltaTime;
			if (m_eventUiNextUpdate <= 0f)
			{
				m_eventUiNextUpdate = 0f;
				SetEventUI();
			}
		}
	}

	public override void UpdateEffectSetting()
	{
		m_isEffectLock = !base.parent.IsEffect(RouletteTop.ROULETTE_EFFECT_TYPE.BG_PARTICLE);
		bool enabled = base.parent.IsEffect(RouletteTop.ROULETTE_EFFECT_TYPE.SPIN);
		if (m_spinButtons == null || m_spinButtons.Count <= 0)
		{
			return;
		}
		int num = 0;
		foreach (GameObject spinButton in m_spinButtons)
		{
			UIPlayAnimation[] components = spinButton.GetComponents<UIPlayAnimation>();
			if (components != null)
			{
				UIPlayAnimation[] array = components;
				foreach (UIPlayAnimation uIPlayAnimation in array)
				{
					uIPlayAnimation.enabled = enabled;
				}
			}
			num++;
		}
	}

	public override void Setup(RouletteTop parent)
	{
		base.Setup(parent);
		m_eventUiCount = 0;
		m_eventUiNextUpdate = -1f;
		m_isEffectLock = false;
		if (m_backButton != null)
		{
			m_backButton.SetActive(true);
			m_backButtonImg = m_backButton.GetComponent<UIImageButton>();
			if (m_backButtonImg != null)
			{
				m_backButtonImg.isEnabled = false;
			}
		}
		m_isJackpot = false;
		m_spinErrorWindow = null;
		m_frontColliderDelay = 0f;
		if (m_frontCollider != null)
		{
			m_frontCollider.SetActive(false);
		}
		m_animeTime = 0f;
		if (m_attentionItemList != null)
		{
			m_attentionItemList.Clear();
			m_attentionItemList = null;
		}
		if (base.wheel != null)
		{
			base.wheel.ChangeSpinCost(0);
			m_attentionItemList = base.wheel.GetAttentionItemList();
			m_currentCategory = base.wheel.category;
			if (m_attentionItemList != null)
			{
				m_eventUiNextUpdate = 0f;
			}
		}
		if (m_wordGet != null)
		{
			m_wordGet.SetActive(false);
		}
		if (m_wordJackpot != null)
		{
			m_wordJackpot.SetActive(false);
		}
		if (m_wordLavel != null)
		{
			m_wordLavel.SetActive(false);
		}
		if (m_wordRankup != null)
		{
			m_wordRankup.SetActive(false);
		}
		SetEventUI();
		SetButton();
		SetEgg();
		UpdateEffectSetting();
	}

	public override void OnUpdateWheelData(ServerWheelOptionsData data)
	{
		base.OnUpdateWheelData(data);
		m_isJackpot = false;
		m_spinErrorWindow = null;
		m_frontColliderDelay = 0.125f;
		if (m_frontCollider != null)
		{
			m_frontCollider.SetActive(true);
		}
		m_animeTime = 0f;
		if (m_attentionItemList != null)
		{
			m_attentionItemList.Clear();
			m_attentionItemList = null;
		}
		if (base.wheel != null)
		{
			m_attentionItemList = base.wheel.GetAttentionItemList();
			m_currentCategory = base.wheel.category;
		}
		SetEventUI();
		SetButton();
		SetEgg();
		UpdateEffectSetting();
	}

	private void SetEventAttention()
	{
		if (!(m_eventUI != null) || m_attentionItemList == null || m_eventUiCount < 0)
		{
			return;
		}
		GameObject gameObject = GameObjectUtil.FindChildGameObject(m_eventUI, "add_space");
		if (!(gameObject != null))
		{
			return;
		}
		GameObject gameObject2 = GameObjectUtil.FindChildGameObject(gameObject, "chao_set");
		GameObject gameObject3 = GameObjectUtil.FindChildGameObject(gameObject, "item_set");
		GameObject gameObject4 = GameObjectUtil.FindChildGameObject(gameObject, "player_set");
		ServerItem serverItem = m_attentionItemList[m_eventUiCount % m_attentionItemList.Count];
		switch (serverItem.idType)
		{
		case ServerItem.IdType.CHAO:
			if (gameObject2 != null)
			{
				gameObject2.SetActive(true);
				UITexture uITexture = GameObjectUtil.FindChildGameObjectComponent<UITexture>(gameObject2, "img_tex_chao");
				UISprite uISprite2 = GameObjectUtil.FindChildGameObjectComponent<UISprite>(gameObject2, "img_bord_bg");
				if (uITexture != null && ChaoTextureManager.Instance != null)
				{
					ChaoTextureManager.CallbackInfo info = new ChaoTextureManager.CallbackInfo(uITexture, null, true);
					ChaoTextureManager.Instance.GetTexture(serverItem.chaoId, info);
				}
				if (uISprite2 != null)
				{
					if (serverItem.id >= ServerItem.Id.CHAO_BEGIN_SRARE)
					{
						uISprite2.spriteName = "ui_chao_set_bg_m_2";
					}
					else if (serverItem.id >= ServerItem.Id.CHAO_BEGIN_RARE)
					{
						uISprite2.spriteName = "ui_chao_set_bg_m_1";
					}
					else
					{
						uISprite2.spriteName = "ui_chao_set_bg_m_0";
					}
				}
			}
			if (gameObject3 != null)
			{
				gameObject3.SetActive(false);
			}
			if (gameObject4 != null)
			{
				gameObject4.SetActive(false);
			}
			return;
		case ServerItem.IdType.CHARA:
			if (gameObject4 != null)
			{
				gameObject4.SetActive(true);
				UITexture uITexture2 = GameObjectUtil.FindChildGameObjectComponent<UITexture>(gameObject4, "img_tex_player");
				if (uITexture2 != null)
				{
					TextureRequestChara request = new TextureRequestChara(serverItem.charaType, uITexture2);
					TextureAsyncLoadManager.Instance.Request(request);
				}
			}
			if (gameObject2 != null)
			{
				gameObject2.SetActive(false);
			}
			if (gameObject3 != null)
			{
				gameObject3.SetActive(false);
			}
			return;
		case ServerItem.IdType.EQUIP_ITEM:
			if (gameObject3 != null)
			{
				gameObject3.SetActive(true);
				UISprite uISprite = GameObjectUtil.FindChildGameObjectComponent<UISprite>(gameObject3, "img_item");
				if (uISprite != null)
				{
					int num = 0;
					num = (int)(serverItem.id - 120000);
					uISprite.spriteName = "ui_cmn_icon_item_" + num;
				}
			}
			if (gameObject2 != null)
			{
				gameObject2.SetActive(false);
			}
			if (gameObject4 != null)
			{
				gameObject4.SetActive(false);
			}
			return;
		}
		if (gameObject3 != null)
		{
			gameObject3.SetActive(true);
			UISprite uISprite3 = GameObjectUtil.FindChildGameObjectComponent<UISprite>(gameObject3, "img_item");
			if (uISprite3 != null)
			{
				int num2 = 0;
				num2 = (int)serverItem.id;
				uISprite3.spriteName = "ui_cmn_icon_item_" + num2;
			}
		}
		if (gameObject2 != null)
		{
			gameObject2.SetActive(false);
		}
		if (gameObject4 != null)
		{
			gameObject4.SetActive(false);
		}
	}

	private void SetEventUI()
	{
		if (!(m_eventUI != null))
		{
			return;
		}
		if (!RouletteUtility.isTutorial || m_parent.wheelData.category != RouletteCategory.PREMIUM)
		{
			if (EventUtility.IsEnableRouletteUI())
			{
				bool flag = m_attentionItemList != null;
				if (flag && m_eventUiNextUpdate >= 0f)
				{
					SetEventAttention();
					m_eventUiNextUpdate = 10f;
					m_eventUiCount++;
				}
				m_eventUI.SetActive(flag);
			}
			else
			{
				m_eventUI.SetActive(false);
			}
		}
		else
		{
			m_eventUI.SetActive(false);
		}
	}

	private void SetEgg()
	{
		if (m_parent == null || m_parent.wheelData == null)
		{
			return;
		}
		int count = 0;
		bool eggSeting = m_parent.wheelData.GetEggSeting(out count);
		if (m_Eggs != null && m_Eggs.Count > 0)
		{
			bool flag = true;
			if (RouletteUtility.isTutorial && m_parent.wheelData.category == RouletteCategory.PREMIUM && m_parent.addSpecialEgg)
			{
				flag = false;
			}
			if (flag)
			{
				for (int i = 0; i < m_Eggs.Count; i++)
				{
					if (m_Eggs[i] != null)
					{
						m_Eggs[i].SetActive(count > i);
					}
				}
			}
		}
		if (m_spEgg != null)
		{
			m_spEgg.SetActive(eggSeting);
		}
	}

	private void UpdateButtonCount(int offset)
	{
		if (m_spinBtn != 0)
		{
			return;
		}
		m_remainingOffset = offset;
		GameObject gameObject = GameObjectUtil.FindChildGameObject(m_spinButtons[(int)m_spinBtn], "img_free_counter_bg");
		if (m_remainingNum - m_remainingOffset < 0 && gameObject != null)
		{
			gameObject.SetActive(false);
		}
		else
		{
			if (!(gameObject != null))
			{
				return;
			}
			gameObject.SetActive(true);
			UISprite uISprite = GameObjectUtil.FindChildGameObjectComponent<UISprite>(gameObject, "img_number_00");
			UISprite uISprite2 = GameObjectUtil.FindChildGameObjectComponent<UISprite>(gameObject, "img_number_0");
			if (uISprite != null && uISprite2 != null)
			{
				if (m_remainingNum - m_remainingOffset >= 100)
				{
					uISprite.spriteName = "ui_roulette_free_counter_number_9";
					uISprite2.spriteName = "ui_roulette_free_counter_number_9";
				}
				else if (m_remainingNum - m_remainingOffset <= 0)
				{
					uISprite.spriteName = "ui_roulette_free_counter_number_0";
					uISprite2.spriteName = "ui_roulette_free_counter_number_0";
				}
				else
				{
					uISprite.spriteName = "ui_roulette_free_counter_number_" + (m_remainingNum - m_remainingOffset) / 10 % 10;
					uISprite2.spriteName = "ui_roulette_free_counter_number_" + (m_remainingNum - m_remainingOffset) % 10;
				}
			}
		}
	}

	private void SetButton()
	{
		if (m_parent == null || m_parent.wheelData == null)
		{
			return;
		}
		m_campaign = m_parent.wheelData.GetCampaign();
		if (m_costBase != null && m_costList == null)
		{
			m_costList = new List<GameObject>();
			for (int i = 0; i < 5; i++)
			{
				GameObject gameObject = GameObjectUtil.FindChildGameObject(m_costBase, "roulette_cost_" + i);
				UIButtonMessage uIButtonMessage = GameObjectUtil.FindChildGameObjectComponent<UIButtonMessage>(m_costBase, "roulette_cost_" + i);
				if (gameObject != null)
				{
					gameObject.SetActive(false);
					m_costList.Add(gameObject);
					if (uIButtonMessage != null)
					{
						uIButtonMessage.functionName = "OnClickSpinCost" + i;
					}
					continue;
				}
				break;
			}
		}
		SetCostItem();
		int count = 0;
		bool btnActive = false;
		m_spinBtn = m_parent.wheelData.GetSpinButtonSeting(out count, out btnActive);
		m_spinBtnActive = btnActive;
		btnActive = true;
		m_remainingOffset = 0;
		m_remainingNum = count;
		if (m_oddsButton != null)
		{
			m_oddsButton.SetActive(true);
		}
		SetButtonMulti();
		SetButtonSpin(count, btnActive);
		if (m_parent != null && m_parent.wheelData.category != RouletteCategory.ITEM)
		{
			if (RouletteUtility.isTutorial && m_parent.wheelData.category == RouletteCategory.PREMIUM)
			{
				GeneralUtil.SetRouletteBannerBtn(base.gameObject, "Btn_ad", base.gameObject, "OnClickBanner", m_parent.wheelData.category, false);
			}
			else
			{
				GeneralUtil.SetRouletteBannerBtn(base.gameObject, "Btn_ad", base.gameObject, "OnClickBanner", m_parent.wheelData.category, true);
			}
		}
		else
		{
			GeneralUtil.SetRouletteBannerBtn(base.gameObject, "Btn_ad", base.gameObject, "OnClickBanner", m_parent.wheelData.category, false);
		}
	}

	private void SetCostItem(int costItemId = -1, int offset = 0)
	{
		m_spinCostList = base.wheel.GetSpinCostItemIdList();
		if (m_spinCostList != null && m_spinCostList.Count > 0)
		{
			if (m_costList != null && m_costList.Count > 0)
			{
				for (int i = 0; i < m_costList.Count; i++)
				{
					GameObject gameObject = m_costList[i];
					if (!(gameObject != null))
					{
						continue;
					}
					if (i < m_spinCostList.Count && m_spinCostList[i] != 910000 && m_spinCostList[i] != 900000 && m_spinCostList[i] > 0)
					{
						gameObject.SetActive(true);
						UISprite uISprite = GameObjectUtil.FindChildGameObjectComponent<UISprite>(gameObject, "img_icon");
						UILabel uILabel = GameObjectUtil.FindChildGameObjectComponent<UILabel>(gameObject, "Lbl_num");
						UILabel uILabel2 = GameObjectUtil.FindChildGameObjectComponent<UILabel>(gameObject, "Lbl_num_sdw");
						if (uISprite != null && uILabel != null && uILabel != null)
						{
							uISprite.spriteName = "ui_cmn_icon_item_" + m_spinCostList[i];
							int num = base.wheel.GetSpinCostItemNum(m_spinCostList[i]);
							if (costItemId == m_spinCostList[i])
							{
								num += offset;
							}
							uILabel.text = HudUtility.GetFormatNumString(num);
							uILabel2.text = HudUtility.GetFormatNumString(num);
						}
					}
					else
					{
						gameObject.SetActive(false);
					}
				}
			}
			else if (m_costList != null && m_costList.Count > 0)
			{
				for (int j = 0; j < m_costList.Count; j++)
				{
					m_costList[j].SetActive(false);
				}
			}
		}
		else if (m_costList != null && m_costList.Count > 0)
		{
			for (int k = 0; k < m_costList.Count; k++)
			{
				m_costList[k].SetActive(false);
			}
		}
	}

	private void SetButtonSpin(int count, bool btnAct)
	{
		if (m_spinButtons == null || m_spinButtons.Count <= 0)
		{
			return;
		}
		for (int i = 0; i < m_spinButtons.Count; i++)
		{
			if (!(m_spinButtons[i] != null))
			{
				continue;
			}
			if (m_spinBtn == (ServerWheelOptionsData.SPIN_BUTTON)i)
			{
				m_spinButtons[i].SetActive(true);
				UIPlayAnimation[] componentsInChildren = m_spinButtons[i].GetComponentsInChildren<UIPlayAnimation>();
				if (componentsInChildren != null && componentsInChildren.Length > 0)
				{
					UIPlayAnimation[] array = componentsInChildren;
					foreach (UIPlayAnimation uIPlayAnimation in array)
					{
						uIPlayAnimation.enabled = m_spinBtnActive;
					}
				}
				UIImageButton uIImageButton = GameObjectUtil.FindChildGameObjectComponent<UIImageButton>(base.gameObject, m_spinButtons[i].name);
				if (uIImageButton != null)
				{
					uIImageButton.isEnabled = btnAct;
				}
				UISprite uISprite = GameObjectUtil.FindChildGameObjectComponent<UISprite>(m_spinButtons[i], "img_sale_icon");
				switch (m_spinBtn)
				{
				case ServerWheelOptionsData.SPIN_BUTTON.FREE:
				{
					GameObject gameObject = GameObjectUtil.FindChildGameObject(m_spinButtons[i], "img_free_counter_bg");
					if (m_remainingNum - m_remainingOffset < 0 && gameObject != null)
					{
						gameObject.SetActive(false);
					}
					else if (gameObject != null)
					{
						gameObject.SetActive(true);
						UISprite uISprite2 = GameObjectUtil.FindChildGameObjectComponent<UISprite>(gameObject, "img_number_00");
						UISprite uISprite3 = GameObjectUtil.FindChildGameObjectComponent<UISprite>(gameObject, "img_number_0");
						if (uISprite2 != null && uISprite3 != null)
						{
							if (m_remainingNum - m_remainingOffset >= 100)
							{
								uISprite2.spriteName = "ui_roulette_free_counter_number_9";
								uISprite3.spriteName = "ui_roulette_free_counter_number_9";
							}
							else if (m_remainingNum - m_remainingOffset <= 0)
							{
								uISprite2.spriteName = "ui_roulette_free_counter_number_0";
								uISprite3.spriteName = "ui_roulette_free_counter_number_0";
							}
							else
							{
								uISprite2.spriteName = "ui_roulette_free_counter_number_" + (m_remainingNum - m_remainingOffset) / 10 % 10;
								uISprite3.spriteName = "ui_roulette_free_counter_number_" + (m_remainingNum - m_remainingOffset) % 10;
							}
						}
					}
					if (uISprite != null)
					{
						if (m_campaign != null && m_campaign.Contains(Constants.Campaign.emType.FreeWheelSpinCount))
						{
							uISprite.gameObject.SetActive(true);
						}
						else
						{
							uISprite.gameObject.SetActive(false);
						}
					}
					break;
				}
				case ServerWheelOptionsData.SPIN_BUTTON.TICKET:
				{
					UISprite uISprite4 = GameObjectUtil.FindChildGameObjectComponent<UISprite>(m_spinButtons[i], "img_btn_" + i + "_icon");
					UILabel uILabel3 = GameObjectUtil.FindChildGameObjectComponent<UILabel>(m_spinButtons[i], "Lbl_btn_" + i);
					UILabel uILabel4 = GameObjectUtil.FindChildGameObjectComponent<UILabel>(m_spinButtons[i], "Lbl_btn_" + i + "_sdw");
					if (uISprite4 != null)
					{
						uISprite4.spriteName = base.wheel.GetRouletteTicketSprite();
					}
					if (uILabel3 != null && uILabel4 != null)
					{
						uILabel3.text = string.Empty + HudUtility.GetFormatNumString(count);
						uILabel4.text = string.Empty + HudUtility.GetFormatNumString(count);
					}
					if (uISprite != null)
					{
						uISprite.gameObject.SetActive(false);
					}
					break;
				}
				case ServerWheelOptionsData.SPIN_BUTTON.RING:
				case ServerWheelOptionsData.SPIN_BUTTON.RSRING:
				case ServerWheelOptionsData.SPIN_BUTTON.RAID:
				{
					UILabel uILabel = GameObjectUtil.FindChildGameObjectComponent<UILabel>(m_spinButtons[i], "Lbl_btn_" + i);
					UILabel uILabel2 = GameObjectUtil.FindChildGameObjectComponent<UILabel>(m_spinButtons[i], "Lbl_btn_" + i + "_sdw");
					if (uILabel != null && uILabel2 != null)
					{
						uILabel.text = HudUtility.GetFormatNumString(count);
						uILabel2.text = HudUtility.GetFormatNumString(count);
					}
					if (uISprite != null)
					{
						if (m_campaign != null && m_campaign.Contains(Constants.Campaign.emType.ChaoRouletteCost))
						{
							uISprite.gameObject.SetActive(true);
						}
						else
						{
							uISprite.gameObject.SetActive(false);
						}
					}
					break;
				}
				default:
					m_spinButtons[i].SetActive(false);
					break;
				}
			}
			else
			{
				m_spinButtons[i].SetActive(false);
			}
		}
	}

	private void SetButtonMulti()
	{
		if (m_spinMultiButton == null)
		{
			m_spinMultiButton = GameObjectUtil.FindChildGameObject(base.gameObject, "multiple_switch");
		}
		if (!(m_spinMultiButton != null))
		{
			return;
		}
		bool flag = false;
		if (base.wheel != null && base.wheel.isGeneral && base.wheel.GetRouletteRank() == RouletteUtility.WheelRank.Normal && (!RouletteUtility.isTutorial || m_parent.wheelData.category != RouletteCategory.PREMIUM) && m_spinBtn != 0)
		{
			flag = true;
		}
		else if (base.wheel != null && base.wheel.category == RouletteCategory.PREMIUM && (!RouletteUtility.isTutorial || m_parent.wheelData.category != RouletteCategory.PREMIUM) && m_spinBtn != 0)
		{
			flag = true;
		}
		if (flag)
		{
			m_spinMultiButton.SetActive(true);
			UIButtonMessage uIButtonMessage = GameObjectUtil.FindChildGameObjectComponent<UIButtonMessage>(m_spinMultiButton.gameObject, "Tgl_multi_0");
			UIButtonMessage uIButtonMessage2 = GameObjectUtil.FindChildGameObjectComponent<UIButtonMessage>(m_spinMultiButton.gameObject, "Tgl_multi_1");
			UIButtonMessage uIButtonMessage3 = GameObjectUtil.FindChildGameObjectComponent<UIButtonMessage>(m_spinMultiButton.gameObject, "Tgl_multi_2");
			if (uIButtonMessage == null)
			{
				uIButtonMessage = GameObjectUtil.FindChildGameObjectComponent<UIButtonMessage>(m_spinMultiButton.gameObject, "Tgl_single");
			}
			if (uIButtonMessage != null)
			{
				uIButtonMessage.functionName = "OnClickSpinMulti0";
				UIImageButton componentInChildren = uIButtonMessage.gameObject.GetComponentInChildren<UIImageButton>();
				if (componentInChildren != null)
				{
					componentInChildren.isEnabled = base.wheel.IsMulti(1);
				}
			}
			if (uIButtonMessage2 != null)
			{
				uIButtonMessage2.functionName = "OnClickSpinMulti1";
				UILabel componentInChildren2 = uIButtonMessage2.gameObject.GetComponentInChildren<UILabel>();
				UIImageButton componentInChildren3 = uIButtonMessage2.gameObject.GetComponentInChildren<UIImageButton>();
				if (componentInChildren2 != null)
				{
					componentInChildren2.text = string.Empty + 3;
				}
				if (componentInChildren3 != null)
				{
					componentInChildren3.isEnabled = base.wheel.IsMulti(3);
				}
			}
			if (uIButtonMessage3 != null)
			{
				uIButtonMessage3.functionName = "OnClickSpinMulti2";
				UILabel componentInChildren4 = uIButtonMessage3.gameObject.GetComponentInChildren<UILabel>();
				UIImageButton componentInChildren5 = uIButtonMessage3.gameObject.GetComponentInChildren<UIImageButton>();
				if (componentInChildren4 != null)
				{
					componentInChildren4.text = string.Empty + 5;
				}
				if (componentInChildren5 != null)
				{
					componentInChildren5.isEnabled = base.wheel.IsMulti(5);
				}
			}
			if (uIButtonMessage != null)
			{
				UIToggle componentInChildren6 = uIButtonMessage.gameObject.GetComponentInChildren<UIToggle>();
				if (componentInChildren6 != null)
				{
					if (base.wheel.multi == 1)
					{
						componentInChildren6.startsActive = true;
						componentInChildren6.SendMessage("Start");
					}
					else
					{
						componentInChildren6.startsActive = false;
					}
				}
			}
			if (uIButtonMessage2 != null)
			{
				UIToggle componentInChildren7 = uIButtonMessage2.gameObject.GetComponentInChildren<UIToggle>();
				if (componentInChildren7 != null)
				{
					if (base.wheel.multi == 3)
					{
						componentInChildren7.startsActive = true;
						componentInChildren7.SendMessage("Start");
					}
					else
					{
						componentInChildren7.startsActive = false;
					}
				}
			}
			if (!(uIButtonMessage3 != null))
			{
				return;
			}
			UIToggle componentInChildren8 = uIButtonMessage3.gameObject.GetComponentInChildren<UIToggle>();
			if (componentInChildren8 != null)
			{
				if (base.wheel.multi == 5)
				{
					componentInChildren8.startsActive = true;
					componentInChildren8.SendMessage("Start");
				}
				else
				{
					componentInChildren8.startsActive = false;
				}
			}
		}
		else
		{
			m_spinMultiButton.SetActive(false);
		}
	}

	private void OnClickFront()
	{
		if (!(m_parent == null) && !base.isDelay && base.isSpin && base.spinDecisionIndex != -1)
		{
			m_parent.OnRouletteSpinSkip();
		}
	}

	private void OnClickOdds()
	{
		if (!(m_parent == null) && !base.isDelay && (!RouletteUtility.isTutorial || m_parent.wheelData.category != RouletteCategory.PREMIUM) && !RouletteManager.Instance.isCurrentPrizeLoading && !RouletteManager.IsPrizeLoading(m_parent.wheelData.category))
		{
			m_parent.SetDelayTime();
			RouletteManager.RequestRoulettePrize(m_parent.wheelData.category, base.gameObject);
		}
	}

	private void OnClickBack()
	{
		if (RouletteManager.IsRouletteEnabled() && base.isSpin && base.spinDecisionIndex == -1)
		{
			RouletteManager.RouletteClose();
		}
	}

	private void OnClickSpin()
	{
		if (base.isSpin || m_parent == null || base.wheel == null || base.isDelay)
		{
			return;
		}
		if (!GeneralUtil.IsNetwork())
		{
			GeneralUtil.ShowNoCommunication("SpinNoCommunication");
		}
		else if (m_spinBtnActive || (RouletteUtility.isTutorial && m_parent.wheelData.category == RouletteCategory.PREMIUM))
		{
			if (m_backButtonImg != null)
			{
				m_backButtonImg.isEnabled = false;
			}
			int spinCostItemId = base.wheel.GetSpinCostItemId();
			int spinCostItemCost = base.wheel.GetSpinCostItemCost(spinCostItemId);
			SetCostItem(spinCostItemId, spinCostItemCost * -1 * base.wheel.multi);
			m_isJackpot = false;
			base.wheel.PlaySe(ServerWheelOptionsData.SE_TYPE.Spin, 0f);
			m_parent.OnRouletteSpinStart(m_parent.wheelData, base.wheel.multi);
		}
		else
		{
			base.wheel.PlaySe(ServerWheelOptionsData.SE_TYPE.SpinError, 0f);
			if (m_spinBtn != 0)
			{
				m_spinErrorWindow = m_parent.wheelData.ShowSpinErrorWindow();
			}
			else
			{
				m_spinErrorWindow = m_parent.wheelData.ShowSpinErrorWindow();
			}
		}
	}

	private void OnClickSpin0()
	{
		OnClickSpin();
	}

	private void OnClickSpin1()
	{
		OnClickSpin();
	}

	private void OnClickSpin2()
	{
		OnClickSpin();
	}

	private void OnClickSpin3()
	{
		OnClickSpin();
	}

	private void OnClickSpin4()
	{
		OnClickSpin();
	}

	private void OnClickSpin5()
	{
		OnClickSpin();
	}

	private void OnClickSpinCost(int index)
	{
		if (base.wheel != null && m_spinCostList != null && m_spinCostList.Count > 1 && (!RouletteUtility.isTutorial || m_parent.wheelData.category != RouletteCategory.PREMIUM))
		{
			int spinCostItemId = base.wheel.GetSpinCostItemId();
			if (spinCostItemId != m_spinCostList[index] && base.wheel.ChangeSpinCost(index + 1))
			{
				base.wheel.ChangeMulti(base.wheel.multi);
				base.wheel.PlaySe(ServerWheelOptionsData.SE_TYPE.Click, 0f);
				SetButton();
			}
		}
	}

	private void OnClickSpinCost0()
	{
		OnClickSpinCost(0);
	}

	private void OnClickSpinCost1()
	{
		OnClickSpinCost(1);
	}

	private void OnClickSpinCost2()
	{
		OnClickSpinCost(2);
	}

	private void OnClickSpinCost3()
	{
		OnClickSpinCost(3);
	}

	private void onClickInfoButton()
	{
		if (RouletteManager.Instance.isCurrentPrizeLoading)
		{
			return;
		}
		if (m_attentionItemList == null)
		{
			m_attentionItemList = base.wheel.GetAttentionItemList();
		}
		if (m_attentionItemList != null)
		{
			EventBestChaoWindow window = EventBestChaoWindow.GetWindow();
			if (window != null)
			{
				window.OpenWindow(m_attentionItemList);
			}
		}
	}

	private void OnClickBanner()
	{
		if (m_parent != null && m_parent.wheelData != null)
		{
			m_parent.OnClickCurrentRouletteBanner();
		}
		Debug.Log("OnClickBanner !");
	}

	private void OnClickSpinMulti0()
	{
		if (base.wheel.ChangeMulti(1))
		{
			SetButton();
			base.wheel.PlaySe(ServerWheelOptionsData.SE_TYPE.Click, 0f);
		}
	}

	private void OnClickSpinMulti1()
	{
		if (base.wheel.ChangeMulti(3))
		{
			SetButton();
			base.wheel.PlaySe(ServerWheelOptionsData.SE_TYPE.Click, 0f);
		}
	}

	private void OnClickSpinMulti2()
	{
		if (base.wheel.ChangeMulti(5))
		{
			SetButton();
			base.wheel.PlaySe(ServerWheelOptionsData.SE_TYPE.Click, 0f);
		}
	}

	private void AnimationFinishCallback()
	{
		if (m_parent != null)
		{
			m_parent.OnRouletteWordAnimeEnd();
		}
	}

	private void FadeAnimationFinishCallback()
	{
		if (m_parent != null)
		{
			m_parent.OnRouletteSpinEnd();
			ActiveAnimation activeAnimation = ActiveAnimation.Play(m_fadeAnime, "ui_simple_load_outro_Anim2", Direction.Forward);
			EventDelegate.Add(activeAnimation.onFinished, FadeOut, true);
		}
	}

	private void FadeOut()
	{
	}

	private void RequestRoulettePrize_Succeeded(ServerPrizeState prize)
	{
		if (!(m_parent == null))
		{
			m_parent.OpenOdds(prize);
		}
	}

	private void RequestRoulettePrize_Failed()
	{
		Debug.Log("RequestRoulettePrize_Failed !!!");
	}

	public override void OnSpinStart()
	{
		m_frontColliderDelay = 5f;
		if (m_frontCollider != null)
		{
			m_frontCollider.SetActive(true);
		}
		if (m_spinBtn == ServerWheelOptionsData.SPIN_BUTTON.FREE || m_spinBtn == ServerWheelOptionsData.SPIN_BUTTON.TICKET)
		{
			UpdateButtonCount(1);
		}
	}

	public override void OnSpinSkip()
	{
		if (m_frontCollider != null)
		{
			m_frontColliderDelay = 5f;
			m_frontCollider.SetActive(true);
		}
	}

	public override void OnSpinDecision()
	{
		if (m_frontCollider != null)
		{
			m_frontColliderDelay = 5f;
			m_frontCollider.SetActive(true);
		}
	}

	public override void OnSpinDecisionMulti()
	{
		if (m_fadeAnime != null)
		{
			ActiveAnimation activeAnimation = ActiveAnimation.Play(m_fadeAnime, "ui_simple_load_intro_Anim2", Direction.Forward);
			EventDelegate.Add(activeAnimation.onFinished, FadeAnimationFinishCallback, true);
			base.wheel.PlaySe(ServerWheelOptionsData.SE_TYPE.Multi, 0f);
		}
		else
		{
			m_parent.OnRouletteSpinEnd();
		}
	}

	public override void OnSpinEnd()
	{
		Debug.Log("RouletteStandardPart OnSpinEnd !!!!!");
		if (m_frontCollider != null)
		{
			m_frontColliderDelay = 0.5f;
			m_frontCollider.SetActive(true);
		}
		if (m_spinBtn == ServerWheelOptionsData.SPIN_BUTTON.FREE && m_parent.wheelData.isRemainingRefresh)
		{
			UpdateButtonCount(0);
		}
		m_isJackpot = false;
		if (!(m_wordAnim != null) || !(m_parent != null))
		{
			return;
		}
		RouletteManager instance = RouletteManager.Instance;
		ServerWheelOptionsData wheelData = m_parent.wheelData;
		bool flag = true;
		if (wheelData != null && instance != null)
		{
			ServerSpinResultGeneral result = instance.GetResult();
			ServerChaoSpinResult resultChao = instance.GetResultChao();
			if (m_wordGet != null)
			{
				m_wordGet.SetActive(false);
			}
			if (m_wordJackpot != null)
			{
				m_wordJackpot.SetActive(false);
			}
			if (m_wordLavel != null)
			{
				m_wordLavel.SetActive(false);
			}
			if (m_wordRankup != null)
			{
				m_wordRankup.SetActive(false);
			}
			if (result != null)
			{
				if (result != null)
				{
					if (result.ItemWon >= 0)
					{
						int num = 0;
						ServerItem cellItem = wheelData.GetCellItem(result.ItemWon, out num);
						if (cellItem.idType == ServerItem.IdType.ITEM_ROULLETE_WIN)
						{
							if (wheelData.GetRouletteRank() != RouletteUtility.WheelRank.Super)
							{
								if (m_wordRankup != null)
								{
									m_wordRankup.SetActive(true);
								}
								base.wheel.PlaySe(ServerWheelOptionsData.SE_TYPE.GetRankup, 0.3f);
								m_frontColliderDelay = 0.125f;
							}
							else
							{
								if (m_wordJackpot != null)
								{
									m_wordJackpot.SetActive(true);
									m_isJackpot = true;
								}
								base.wheel.PlaySe(ServerWheelOptionsData.SE_TYPE.GetJackpot, 0.3f);
								m_frontColliderDelay = 0.125f;
							}
						}
						else
						{
							if (m_wordGet != null)
							{
								m_wordGet.SetActive(true);
							}
							bool flag2 = false;
							if (cellItem.idType == ServerItem.IdType.CHARA)
							{
								flag2 = true;
							}
							else if (cellItem.idType == ServerItem.IdType.CHAO && cellItem.chaoId >= 1000)
							{
								flag2 = true;
							}
							if (flag2)
							{
								base.wheel.PlaySe(ServerWheelOptionsData.SE_TYPE.GetRare, 0.3f);
							}
							else
							{
								base.wheel.PlaySe(ServerWheelOptionsData.SE_TYPE.GetItem, 0.3f);
							}
						}
					}
					else
					{
						flag = false;
					}
				}
			}
			else if (wheelData.wheelType == RouletteUtility.WheelType.Normal)
			{
				if (m_wordGet != null)
				{
					m_wordGet.SetActive(true);
				}
				bool flag3 = false;
				if (resultChao != null)
				{
					Dictionary<int, ServerItemState>.KeyCollection keys = resultChao.ItemState.Keys;
					foreach (int item2 in keys)
					{
						ServerItem item = resultChao.ItemState[item2].GetItem();
						if (item.idType == ServerItem.IdType.CHARA)
						{
							flag3 = true;
							break;
						}
						if (item.idType == ServerItem.IdType.CHAO && item.chaoId >= 1000)
						{
							flag3 = true;
							break;
						}
					}
				}
				if (flag3)
				{
					base.wheel.PlaySe(ServerWheelOptionsData.SE_TYPE.GetRare, 0.3f);
				}
				else
				{
					base.wheel.PlaySe(ServerWheelOptionsData.SE_TYPE.GetItem, 0.3f);
				}
			}
			else
			{
				Debug.Log("RouletteStandardPart OnSpinEnd error?");
				if (wheelData.itemWonData.idType == ServerItem.IdType.ITEM_ROULLETE_WIN)
				{
					if (wheelData.GetRouletteRank() != RouletteUtility.WheelRank.Super)
					{
						if (m_wordRankup != null)
						{
							m_wordRankup.SetActive(true);
						}
						base.wheel.PlaySe(ServerWheelOptionsData.SE_TYPE.GetRankup, 0.3f);
					}
					else
					{
						if (m_wordJackpot != null)
						{
							m_wordJackpot.SetActive(true);
							m_isJackpot = true;
						}
						base.wheel.PlaySe(ServerWheelOptionsData.SE_TYPE.GetJackpot, 0.3f);
					}
				}
				else
				{
					if (m_wordGet != null)
					{
						m_wordGet.SetActive(true);
					}
					base.wheel.PlaySe(ServerWheelOptionsData.SE_TYPE.GetItem, 0.3f);
				}
			}
		}
		m_wordAnim.Stop("ui_menu_roulette_word_Anim");
		if (flag)
		{
			if (m_isJackpot)
			{
				m_animeTime = 3f;
			}
			else
			{
				m_animeTime = 1.1f;
			}
			ActiveAnimation.Play(m_wordAnim, "ui_menu_roulette_word_Anim", Direction.Forward);
		}
		else
		{
			m_animeTime = 0f;
			AnimationFinishCallback();
		}
	}

	public override void OnSpinError()
	{
		m_frontColliderDelay = 0f;
		if (m_frontCollider != null)
		{
			m_frontCollider.SetActive(false);
		}
	}

	public override void windowClose()
	{
		base.windowClose();
		if (m_frontCollider != null && !m_frontCollider.activeSelf)
		{
			m_frontCollider.SetActive(true);
		}
		m_frontColliderDelay = 0.25f;
	}

	public override void PartsSendMessage(string mesage)
	{
		if (!string.IsNullOrEmpty(mesage) && mesage.IndexOf("CostItemUpdate") != -1)
		{
			SetEgg();
			SetCostItem();
			SetButton();
		}
	}
}
