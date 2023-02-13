using AnimationOrTween;
using Message;
using System.Collections;
using System.Collections.Generic;
using Text;
using UnityEngine;

public class PlayerSetWindowUI : WindowBase
{
	public enum WINDOW_MODE
	{
		DEFAULT,
		BUY,
		SET
	}

	private enum BTN_TYPE
	{
		BUY_1,
		BUY_2,
		BUY_3,
		SET,
		NONE
	}

	private const float BTN_DELAY_TIME = 2f;

	private static bool s_isActive;

	private ui_player_set_scroll m_parent;

	private bool m_init;

	private bool m_setup;

	private bool m_costError;

	private ButtonInfoTable.ButtonType m_costErrorType = ButtonInfoTable.ButtonType.UNKNOWN;

	private ServerItem.Id m_buyId = ServerItem.Id.NONE;

	private WINDOW_MODE m_windMode;

	private BTN_TYPE m_btnType = BTN_TYPE.NONE;

	private CharaType m_charaType = CharaType.UNKNOWN;

	private ServerCharacterState m_charaState;

	private Dictionary<ServerItem.Id, int> m_buyCostList;

	private List<UIImageButton> m_btnObjectList;

	private float m_btnDelay;

	private Animation m_animation;

	private UIPlayAnimation m_playAnimation;

	private UIPanel m_panel;

	private int m_oldUnlockedCharacterNum;

	private UIButton m_btnClose;

	private List<GameObject> m_btnObjList;

	private static bool s_starTextDefaultInit;

	private static Color s_starTextDefault;

	public static bool isActive
	{
		get
		{
			return s_isActive;
		}
	}

	private void OnDestroy()
	{
		Destroy();
	}

	private void Update()
	{
		if (m_buyId != ServerItem.Id.NONE && GeneralWindow.IsCreated("BuyCharacter") && GeneralWindow.IsButtonPressed)
		{
			if (GeneralWindow.IsYesButtonPressed)
			{
				if (GeneralUtil.IsNetwork())
				{
					if (m_buyCostList != null && m_buyCostList.ContainsKey(m_buyId))
					{
						ServerInterface loggedInServerInterface = ServerInterface.LoggedInServerInterface;
						if (loggedInServerInterface != null)
						{
							SetBtnObjectCollider(false);
							m_oldUnlockedCharacterNum = 1;
							ServerPlayerState playerState = ServerInterface.PlayerState;
							if (playerState != null)
							{
								m_oldUnlockedCharacterNum = playerState.unlockedCharacterNum;
							}
							loggedInServerInterface.RequestServerUnlockedCharacter(item: new ServerItem(m_buyId), charaType: m_charaType, callbackObject: base.gameObject);
						}
					}
				}
				else
				{
					GeneralUtil.ShowNoCommunication();
				}
			}
			m_buyId = ServerItem.Id.NONE;
		}
		if (m_btnDelay > 0f)
		{
			m_btnDelay -= Time.deltaTime;
			if (m_btnDelay <= 0f)
			{
				SetBtnObjectEnabeld(true);
				m_btnDelay = 0f;
			}
		}
	}

	public void Init()
	{
		base.gameObject.SetActive(true);
		m_panel = GameObjectUtil.FindChildGameObjectComponent<UIPanel>(base.gameObject, "player_set_window");
		m_panel.alpha = 0f;
		m_animation = GameObjectUtil.FindChildGameObjectComponent<Animation>(base.gameObject, "player_set_window");
		if (m_animation != null)
		{
			GameObject gameObject = m_animation.gameObject;
			gameObject.SetActive(false);
			m_animation.Stop();
		}
		m_playAnimation = base.gameObject.AddComponent<UIPlayAnimation>();
		m_playAnimation.target = m_animation;
		m_btnClose = GameObjectUtil.FindChildGameObjectComponent<UIButton>(base.gameObject, "Btn_window_close");
		m_btnObjList = new List<GameObject>();
		for (int i = 0; i < 7; i++)
		{
			GameObject gameObject2 = GameObjectUtil.FindChildGameObject(base.gameObject, "Btn_" + i);
			if (gameObject2 != null)
			{
				m_btnObjList.Add(gameObject2);
			}
			else if (i > 0)
			{
				break;
			}
		}
		UIButtonMessage uIButtonMessage = m_btnClose.gameObject.GetComponent<UIButtonMessage>();
		if (uIButtonMessage == null)
		{
			uIButtonMessage = m_btnClose.gameObject.AddComponent<UIButtonMessage>();
		}
		if (uIButtonMessage != null)
		{
			uIButtonMessage.trigger = UIButtonMessage.Trigger.OnClick;
			uIButtonMessage.target = base.gameObject;
			uIButtonMessage.functionName = "OnClickBtn";
		}
		base.gameObject.SetActive(false);
		m_init = true;
	}

	private void OnClickBtn()
	{
		if (m_setup)
		{
			s_isActive = false;
			m_setup = false;
			m_costErrorType = ButtonInfoTable.ButtonType.UNKNOWN;
			m_buyId = ServerItem.Id.NONE;
			ActiveAnimation activeAnimation = ActiveAnimation.Play(m_animation, "ui_cmn_window_Anim", Direction.Reverse);
			if (activeAnimation != null)
			{
				EventDelegate.Add(activeAnimation.onFinished, OnAnimFinished, true);
			}
			SoundManager.SePlay("sys_window_close");
		}
	}

	private void OnAnimFinished()
	{
		base.gameObject.SetActive(false);
		m_windMode = WINDOW_MODE.DEFAULT;
		s_isActive = false;
	}

	private IEnumerator OnFinished()
	{
		yield return new WaitForSeconds(0.5f);
		m_windMode = WINDOW_MODE.DEFAULT;
		s_isActive = false;
		base.gameObject.SetActive(false);
	}

	private void ResetBtnObjectList()
	{
		if (m_btnObjectList != null)
		{
			m_btnObjectList.Clear();
		}
		else
		{
			m_btnObjectList = new List<UIImageButton>();
		}
	}

	private void AddBtnObjectList(UIImageButton btnObject)
	{
		if (m_btnObjectList == null)
		{
			m_btnObjectList = new List<UIImageButton>();
		}
		btnObject.isEnabled = (m_btnDelay <= 0f);
		m_btnObjectList.Add(btnObject);
	}

	private void SetBtnObjectCollider(bool enabeld)
	{
		if (m_btnObjectList == null || m_btnObjectList.Count <= 0)
		{
			return;
		}
		foreach (UIImageButton btnObject in m_btnObjectList)
		{
			if (btnObject != null)
			{
				BoxCollider component = btnObject.gameObject.GetComponent<BoxCollider>();
				if (component != null)
				{
					component.isTrigger = !enabeld;
				}
			}
		}
	}

	private void SetBtnObjectEnabeld(bool enabeld, float delay = 2f)
	{
		if (m_btnObjectList != null && m_btnObjectList.Count > 0)
		{
			foreach (UIImageButton btnObject in m_btnObjectList)
			{
				if (btnObject != null)
				{
					btnObject.isEnabled = enabeld;
				}
			}
		}
		if (enabeld)
		{
			m_btnDelay = 0f;
			SetBtnObjectCollider(true);
		}
		else
		{
			m_btnDelay = delay;
		}
	}

	private void SetupCharaSetBtnView(GameObject parent, string mainObjName, string subObjName)
	{
		GeneralUtil.SetButtonFunc(parent, mainObjName, base.gameObject, "OnClickBtnMain");
		GeneralUtil.SetButtonFunc(parent, subObjName, base.gameObject, "OnClickBtnSub");
		UIImageButton uIImageButton = GameObjectUtil.FindChildGameObjectComponent<UIImageButton>(parent, mainObjName);
		UIImageButton uIImageButton2 = GameObjectUtil.FindChildGameObjectComponent<UIImageButton>(parent, subObjName);
		if (uIImageButton != null && uIImageButton2 != null)
		{
			if (m_parent != null && m_parent.parent != null)
			{
				uIImageButton.isEnabled = m_parent.parent.CheckSetMode(PlayerCharaList.SET_CHARA_MODE.MAIN, m_charaType);
				uIImageButton2.isEnabled = m_parent.parent.CheckSetMode(PlayerCharaList.SET_CHARA_MODE.SUB, m_charaType);
			}
			else
			{
				uIImageButton.isEnabled = true;
				uIImageButton2.isEnabled = true;
			}
		}
	}

	private void SetupRouletteBtnView(GameObject parent, string objName)
	{
		GeneralUtil.SetButtonFunc(parent, objName, base.gameObject, "OnClickBtnRoulette");
		UIImageButton uIImageButton = GameObjectUtil.FindChildGameObjectComponent<UIImageButton>(parent, objName);
		GameObject gameObject = GameObjectUtil.FindChildGameObject(parent, objName + "_1");
		GameObject gameObject2 = GameObjectUtil.FindChildGameObject(parent, objName + "_2");
		GameObject gameObject3 = GameObjectUtil.FindChildGameObject(parent, "img_sale_icon");
		if (uIImageButton != null)
		{
			AddBtnObjectList(uIImageButton);
		}
		if (gameObject != null && gameObject2 != null)
		{
			gameObject.SetActive(false);
			gameObject2.SetActive(true);
		}
		if (gameObject3 != null)
		{
			if (HudMenuUtility.IsSale(Constants.Campaign.emType.ChaoRouletteCost))
			{
				gameObject3.SetActive(true);
			}
			else if (HudMenuUtility.IsSale(Constants.Campaign.emType.PremiumRouletteOdds))
			{
				gameObject3.SetActive(true);
			}
			else
			{
				gameObject3.SetActive(false);
			}
		}
	}

	private void SetupBuyBtnView(GameObject parent, string objName, ServerItem.Id costItem, int costValue)
	{
		GeneralUtil.SetButtonFunc(parent, objName, base.gameObject, "OnClickBtn_" + costItem);
		UIImageButton uIImageButton = GameObjectUtil.FindChildGameObjectComponent<UIImageButton>(parent, objName);
		GameObject gameObject = GameObjectUtil.FindChildGameObject(parent, objName + "_1");
		GameObject gameObject2 = GameObjectUtil.FindChildGameObject(parent, objName + "_2");
		GameObject gameObject3 = GameObjectUtil.FindChildGameObject(parent, "img_sale_icon");
		if (uIImageButton != null)
		{
			AddBtnObjectList(uIImageButton);
		}
		if (gameObject != null && gameObject2 != null)
		{
			gameObject.SetActive(true);
			gameObject2.SetActive(false);
			UISprite uISprite = GameObjectUtil.FindChildGameObjectComponent<UISprite>(gameObject, "img_icon_ring");
			UILabel uILabel = GameObjectUtil.FindChildGameObjectComponent<UILabel>(gameObject, "Lbl_cost");
			if (uISprite != null && uILabel != null)
			{
				switch (costItem)
				{
				case ServerItem.Id.RING:
					uISprite.spriteName = "ui_test_icon_ring00";
					break;
				case ServerItem.Id.RSRING:
					uISprite.spriteName = "ui_test_icon_rsring";
					break;
				case ServerItem.Id.RAIDRING:
					uISprite.spriteName = "ui_event_ring_icon";
					break;
				case ServerItem.Id.SPECIAL_EGG:
					uISprite.spriteName = "ui_roulette_pager_icon_8";
					break;
				default:
					uISprite.spriteName = string.Empty;
					break;
				}
				uILabel.text = HudUtility.GetFormatNumString(costValue);
			}
		}
		else
		{
			UISprite uISprite2 = GameObjectUtil.FindChildGameObjectComponent<UISprite>(parent, "img_icon_ring");
			UILabel uILabel2 = GameObjectUtil.FindChildGameObjectComponent<UILabel>(parent, "Lbl_rs_cost_1");
			if (uILabel2 == null)
			{
				uILabel2 = GameObjectUtil.FindChildGameObjectComponent<UILabel>(parent, "Lbl_rs_cost_2");
			}
			if (uISprite2 != null && uILabel2 != null)
			{
				switch (costItem)
				{
				case ServerItem.Id.RING:
					uISprite2.spriteName = "ui_test_icon_ring00";
					break;
				case ServerItem.Id.RSRING:
					uISprite2.spriteName = "ui_test_icon_rsring";
					break;
				case ServerItem.Id.RAIDRING:
					uISprite2.spriteName = "ui_event_ring_icon";
					break;
				case ServerItem.Id.SPECIAL_EGG:
					uISprite2.spriteName = "ui_roulette_pager_icon_8";
					break;
				default:
					uISprite2.spriteName = string.Empty;
					break;
				}
				uILabel2.text = HudUtility.GetFormatNumString(costValue);
			}
		}
		if (gameObject3 != null)
		{
			int id = (int)new ServerItem(m_charaType).id;
			ServerCampaignData campaignInSession = ServerInterface.CampaignState.GetCampaignInSession(Constants.Campaign.emType.CharacterUpgradeCost, id);
			gameObject3.SetActive(campaignInSession != null);
		}
	}

	private void SetupBtn()
	{
		ResetBtnObjectList();
		ServerPlayerState playerState = ServerInterface.PlayerState;
		if (playerState != null)
		{
			m_charaState = playerState.CharacterState(m_charaType);
		}
		else
		{
			m_charaState = null;
		}
		m_buyCostList = null;
		switch (m_windMode)
		{
		case WINDOW_MODE.BUY:
		{
			m_btnType = BTN_TYPE.BUY_1;
			if (m_charaState == null)
			{
				break;
			}
			int num = 0;
			if (m_charaState.IsBuy)
			{
				m_buyCostList = m_charaState.GetBuyCostItemList();
				if (m_buyCostList != null)
				{
					num = m_buyCostList.Count;
				}
			}
			if (m_charaState.IsRoulette)
			{
				num++;
			}
			switch (num)
			{
			case 1:
				m_btnType = BTN_TYPE.BUY_1;
				break;
			case 2:
				m_btnType = BTN_TYPE.BUY_2;
				break;
			case 3:
				m_btnType = BTN_TYPE.BUY_3;
				break;
			default:
				m_btnType = BTN_TYPE.NONE;
				break;
			}
			break;
		}
		case WINDOW_MODE.SET:
			m_btnType = BTN_TYPE.SET;
			break;
		default:
			m_btnType = BTN_TYPE.NONE;
			break;
		}
		List<ServerItem.Id> list = null;
		if (m_buyCostList != null && m_buyCostList.Count > 0)
		{
			list = new List<ServerItem.Id>();
			Dictionary<ServerItem.Id, int>.KeyCollection keys = m_buyCostList.Keys;
			foreach (ServerItem.Id item in keys)
			{
				list.Add(item);
			}
		}
		int num2 = -1;
		switch (m_btnType)
		{
		case BTN_TYPE.BUY_1:
			num2 = 0;
			break;
		case BTN_TYPE.BUY_2:
			num2 = 1;
			break;
		case BTN_TYPE.BUY_3:
			num2 = 2;
			break;
		case BTN_TYPE.SET:
			num2 = 3;
			break;
		default:
			num2 = -1;
			break;
		}
		for (int i = 0; i < m_btnObjList.Count; i++)
		{
			if (i == num2)
			{
				m_btnObjList[i].SetActive(true);
				switch (m_btnType)
				{
				case BTN_TYPE.BUY_1:
					if (m_charaState.IsRoulette)
					{
						SetupRouletteBtnView(m_btnObjList[i], "Btn_c");
					}
					else
					{
						SetupBuyBtnView(m_btnObjList[i], "Btn_c", list[0], m_buyCostList[list[0]]);
					}
					break;
				case BTN_TYPE.BUY_2:
					if (m_charaState.IsRoulette)
					{
						SetupRouletteBtnView(m_btnObjList[i], "Btn_l");
						SetupBuyBtnView(m_btnObjList[i], "Btn_r", list[0], m_buyCostList[list[0]]);
					}
					else
					{
						SetupBuyBtnView(m_btnObjList[i], "Btn_l", list[0], m_buyCostList[list[0]]);
						SetupBuyBtnView(m_btnObjList[i], "Btn_r", list[1], m_buyCostList[list[1]]);
					}
					break;
				case BTN_TYPE.BUY_3:
					if (m_charaState.IsRoulette)
					{
						SetupRouletteBtnView(m_btnObjList[i], "Btn_l");
						SetupBuyBtnView(m_btnObjList[i], "Btn_c", list[0], m_buyCostList[list[0]]);
						SetupBuyBtnView(m_btnObjList[i], "Btn_r", list[1], m_buyCostList[list[1]]);
					}
					else
					{
						SetupBuyBtnView(m_btnObjList[i], "Btn_l", list[0], m_buyCostList[list[0]]);
						SetupBuyBtnView(m_btnObjList[i], "Btn_c", list[1], m_buyCostList[list[1]]);
						SetupBuyBtnView(m_btnObjList[i], "Btn_r", list[2], m_buyCostList[list[2]]);
					}
					break;
				case BTN_TYPE.SET:
					SetupCharaSetBtnView(m_btnObjList[i], "Btn_main", "Btn_sub");
					break;
				}
			}
			else
			{
				m_btnObjList[i].SetActive(false);
			}
		}
	}

	private void SetupObject()
	{
		if (m_panel != null)
		{
			m_panel.depth = 54;
			m_panel.alpha = 1f;
		}
		GameObject gameObject = GameObjectUtil.FindChildGameObject(base.gameObject, "img_icon_key");
		if (gameObject != null)
		{
			gameObject.SetActive(!m_charaState.IsUnlocked);
		}
		string commonText = TextUtility.GetCommonText("CharaName", CharaName.Name[(int)m_charaType]);
		string charaAttributeSpriteName = HudUtility.GetCharaAttributeSpriteName(m_charaType);
		string teamAttributeSpriteName = HudUtility.GetTeamAttributeSpriteName(m_charaType);
		string charaAttributeText = m_charaState.GetCharaAttributeText();
		int totalLevel = MenuPlayerSetUtil.GetTotalLevel(m_charaType);
		UISprite uISprite = GameObjectUtil.FindChildGameObjectComponent<UISprite>(base.gameObject, "img_player_speacies");
		UISprite uISprite2 = GameObjectUtil.FindChildGameObjectComponent<UISprite>(base.gameObject, "img_player_genus");
		UILabel uILabel = GameObjectUtil.FindChildGameObjectComponent<UILabel>(base.gameObject, "Lbl_player_lv");
		UILabel uILabel2 = GameObjectUtil.FindChildGameObjectComponent<UILabel>(base.gameObject, "Lbl_player_name");
		UILabel uILabel3 = GameObjectUtil.FindChildGameObjectComponent<UILabel>(base.gameObject, "Lbl_player_attribute");
		UILabel uILabel4 = GameObjectUtil.FindChildGameObjectComponent<UILabel>(base.gameObject, "Lbl_player_star_lv");
		UILabel uILabel5 = GameObjectUtil.FindChildGameObjectComponent<UILabel>(base.gameObject, "Lbt_caption");
		if (uILabel5 != null)
		{
			switch (m_btnType)
			{
			case BTN_TYPE.BUY_1:
			case BTN_TYPE.BUY_2:
			case BTN_TYPE.BUY_3:
				uILabel5.text = TextManager.GetText(TextManager.TextType.TEXTTYPE_COMMON_TEXT, "PlayerSet", "ui_Lbt_buy_caption").text;
				break;
			case BTN_TYPE.SET:
				uILabel5.text = TextManager.GetText(TextManager.TextType.TEXTTYPE_COMMON_TEXT, "PlayerSet", "ui_Lbt_caption").text;
				break;
			case BTN_TYPE.NONE:
				uILabel5.text = TextManager.GetText(TextManager.TextType.TEXTTYPE_COMMON_TEXT, "PlayerSet", "ui_Lbt_info_caption").text;
				break;
			}
		}
		if (uILabel4 != null)
		{
			uILabel4.text = m_charaState.star.ToString();
			if (!s_starTextDefaultInit)
			{
				Color color = uILabel4.color;
				float r = color.r;
				Color color2 = uILabel4.color;
				float g = color2.g;
				Color color3 = uILabel4.color;
				float b = color3.b;
				Color color4 = uILabel4.color;
				s_starTextDefault = new Color(r, g, b, color4.a);
				s_starTextDefaultInit = true;
			}
			if (m_charaState.star >= m_charaState.starMax)
			{
				uILabel4.color = new Color(82f / 85f, 116f / 255f, 0f);
			}
			else
			{
				uILabel4.color = s_starTextDefault;
			}
		}
		UITexture uITexture = GameObjectUtil.FindChildGameObjectComponent<UITexture>(base.gameObject, "img_player_tex");
		if (uITexture != null)
		{
			TextureRequestChara request = new TextureRequestChara(m_charaType, uITexture);
			TextureAsyncLoadManager.Instance.Request(request);
			if (m_charaState.IsUnlocked)
			{
				uITexture.color = new Color(1f, 1f, 1f);
			}
			else
			{
				uITexture.color = new Color(0f, 0f, 0f);
			}
		}
		uISprite.spriteName = charaAttributeSpriteName;
		uISprite2.spriteName = teamAttributeSpriteName;
		uILabel.text = TextUtility.GetTextLevel(string.Format("{0:000}", totalLevel));
		uILabel2.text = commonText;
		uILabel3.text = charaAttributeText;
	}

	public void Setup(CharaType charaType, ui_player_set_scroll parent = null, WINDOW_MODE mode = WINDOW_MODE.DEFAULT)
	{
		s_isActive = true;
		m_setup = true;
		m_costError = false;
		m_costErrorType = ButtonInfoTable.ButtonType.UNKNOWN;
		m_buyId = ServerItem.Id.NONE;
		if (!m_init)
		{
			Init();
		}
		m_parent = parent;
		m_charaType = charaType;
		m_windMode = mode;
		m_btnDelay = 0f;
		SetupBtn();
		if (m_charaState != null)
		{
			base.gameObject.SetActive(true);
			SetupObject();
			if (m_animation != null)
			{
				GameObject gameObject = m_animation.gameObject;
				gameObject.SetActive(true);
				m_playAnimation.Play(true);
			}
			UIDraggablePanel uIDraggablePanel = GameObjectUtil.FindChildGameObjectComponent<UIDraggablePanel>(base.gameObject, "ScrollView");
			if (uIDraggablePanel != null)
			{
				uIDraggablePanel.ResetPosition();
			}
			SoundManager.SePlay("sys_window_open");
		}
		GameObject gameObject2 = GameObjectUtil.FindChildGameObject(base.gameObject, "eff_set1");
		GameObject gameObject3 = GameObjectUtil.FindChildGameObject(base.gameObject, "eff_set2");
		if (gameObject2 != null && gameObject3 != null)
		{
			gameObject2.SetActive(false);
			gameObject3.SetActive(false);
		}
		SetBtnObjectCollider(true);
	}

	public static PlayerSetWindowUI Create(CharaType charaType, ui_player_set_scroll parent = null, WINDOW_MODE mode = WINDOW_MODE.DEFAULT)
	{
		GameObject cameraUIObject = HudMenuUtility.GetCameraUIObject();
		if (cameraUIObject != null)
		{
			GameObject gameObject = GameObjectUtil.FindChildGameObject(cameraUIObject, "PlayerSetWindowUI");
			PlayerSetWindowUI playerSetWindowUI = null;
			if (gameObject != null)
			{
				playerSetWindowUI = gameObject.GetComponent<PlayerSetWindowUI>();
				if (playerSetWindowUI == null)
				{
					playerSetWindowUI = gameObject.AddComponent<PlayerSetWindowUI>();
				}
				if (playerSetWindowUI != null)
				{
					playerSetWindowUI.Setup(charaType, parent, mode);
				}
			}
			return playerSetWindowUI;
		}
		return null;
	}

	private void OnClickBtnRoulette()
	{
		s_isActive = false;
		m_setup = false;
		m_buyId = ServerItem.Id.NONE;
		ActiveAnimation activeAnimation = ActiveAnimation.Play(m_animation, "ui_cmn_window_Anim", Direction.Reverse);
		if (activeAnimation != null)
		{
			EventDelegate.Add(activeAnimation.onFinished, OnAnimFinished, true);
		}
		HudMenuUtility.SendMenuButtonClicked(ButtonInfoTable.ButtonType.ROULETTE);
		SoundManager.SePlay("sys_menu_decide");
	}

	private void OnClickBtn_RING()
	{
		if (m_buyCostList != null && m_buyCostList.Count > 0 && m_buyCostList.ContainsKey(ServerItem.Id.RING))
		{
			int num = m_buyCostList[ServerItem.Id.RING];
			if (SendBuyChara(ServerItem.Id.RING, num))
			{
				Debug.Log("OnClickBtn_RING value:" + num);
			}
		}
	}

	private void OnClickBtn_RSRING()
	{
		if (m_buyCostList != null && m_buyCostList.Count > 0 && m_buyCostList.ContainsKey(ServerItem.Id.RSRING))
		{
			int num = m_buyCostList[ServerItem.Id.RSRING];
			if (SendBuyChara(ServerItem.Id.RSRING, num))
			{
				Debug.Log("OnClickBtn_RSRING value:" + num);
			}
		}
	}

	private void OnClickBtnMain()
	{
		if (m_parent != null && m_parent.parent != null)
		{
			m_parent.parent.SetChara(PlayerCharaList.SET_CHARA_MODE.MAIN, m_charaType);
		}
		s_isActive = false;
		ActiveAnimation activeAnimation = ActiveAnimation.Play(m_animation, "ui_cmn_window_Anim", Direction.Reverse);
		if (activeAnimation != null)
		{
			EventDelegate.Add(activeAnimation.onFinished, OnAnimFinished, true);
		}
		SoundManager.SePlay("sys_menu_decide");
	}

	private void OnClickBtnSub()
	{
		if (m_parent != null && m_parent.parent != null)
		{
			m_parent.parent.SetChara(PlayerCharaList.SET_CHARA_MODE.SUB, m_charaType);
		}
		s_isActive = false;
		ActiveAnimation activeAnimation = ActiveAnimation.Play(m_animation, "ui_cmn_window_Anim", Direction.Reverse);
		if (activeAnimation != null)
		{
			EventDelegate.Add(activeAnimation.onFinished, OnAnimFinished, true);
		}
		SoundManager.SePlay("sys_menu_decide");
	}

	private bool SendBuyChara(ServerItem.Id itemId, int cost)
	{
		bool result = false;
		if (GeneralUtil.IsNetwork())
		{
			if (m_charaState != null)
			{
				SetBtnObjectCollider(false);
				long itemCount = GeneralUtil.GetItemCount(itemId);
				if (itemCount >= cost)
				{
					m_buyId = itemId;
					string text = string.Empty;
					string empty = string.Empty;
					switch (itemId)
					{
					case ServerItem.Id.RING:
						text = ((!m_charaState.IsUnlocked) ? TextUtility.GetText(TextManager.TextType.TEXTTYPE_COMMON_TEXT, "WindowText", "chara_unlock_ring_text") : TextUtility.GetText(TextManager.TextType.TEXTTYPE_COMMON_TEXT, "WindowText", "chara_unlock_ring_text_2"));
						break;
					case ServerItem.Id.RSRING:
						text = ((!m_charaState.IsUnlocked) ? TextUtility.GetText(TextManager.TextType.TEXTTYPE_COMMON_TEXT, "WindowText", "chara_unlock_rsring_text") : TextUtility.GetText(TextManager.TextType.TEXTTYPE_COMMON_TEXT, "WindowText", "chara_unlock_rsring_text_2"));
						break;
					}
					text = text.Replace("{RING_COST}", HudUtility.GetFormatNumString(cost));
					empty = ((!m_charaState.IsUnlocked) ? TextUtility.GetText(TextManager.TextType.TEXTTYPE_COMMON_TEXT, "WindowText", "chara_unlock_caption") : TextUtility.GetText(TextManager.TextType.TEXTTYPE_COMMON_TEXT, "WindowText", "chara_unlock_caption_2"));
					GeneralWindow.CInfo info = default(GeneralWindow.CInfo);
					info.name = "BuyCharacter";
					info.buttonType = GeneralWindow.ButtonType.YesNo;
					info.finishedCloseDelegate = GeneralWindowBuyCharacterClosedCallback;
					info.caption = empty;
					info.message = text;
					GeneralWindow.Create(info);
					result = true;
				}
				else
				{
					string message = string.Empty;
					string caption = string.Empty;
					string name = string.Empty;
					m_costError = false;
					GeneralWindow.ButtonType buttonType = GeneralWindow.ButtonType.ShopCancel;
					bool flag = ServerInterface.IsRSREnable();
					switch (itemId)
					{
					case ServerItem.Id.RING:
						name = "SpinCostErrorRing";
						caption = TextUtility.GetText(TextManager.TextType.TEXTTYPE_COMMON_TEXT, "ItemRoulette", "gw_cost_caption");
						message = TextUtility.GetText(TextManager.TextType.TEXTTYPE_COMMON_TEXT, "ItemRoulette", "gw_cost_text");
						m_costErrorType = ButtonInfoTable.ButtonType.RING_TO_SHOP;
						break;
					case ServerItem.Id.RSRING:
						name = "SpinCostErrorRSRing";
						caption = TextUtility.GetText(TextManager.TextType.TEXTTYPE_COMMON_TEXT, "ChaoRoulette", "gw_cost_caption");
						message = ((!flag) ? TextUtility.GetCommonText("ChaoRoulette", "gw_cost_caption_text_2") : TextUtility.GetCommonText("ChaoRoulette", "gw_cost_caption_text"));
						buttonType = ((!flag) ? GeneralWindow.ButtonType.Ok : GeneralWindow.ButtonType.ShopCancel);
						m_costErrorType = ButtonInfoTable.ButtonType.REDSTAR_TO_SHOP;
						break;
					}
					GeneralWindow.CInfo info2 = default(GeneralWindow.CInfo);
					info2.name = name;
					info2.buttonType = buttonType;
					info2.finishedCloseDelegate = GeneralWindowClosedCallback;
					info2.caption = caption;
					info2.message = message;
					info2.isPlayErrorSe = true;
					GeneralWindow.Create(info2);
				}
			}
		}
		else
		{
			GeneralUtil.ShowNoCommunication();
		}
		return result;
	}

	private void GeneralWindowBuyCharacterClosedCallback()
	{
		if (!GeneralWindow.IsYesButtonPressed)
		{
			GeneralWindow.Close();
			SetBtnObjectEnabeld(true);
		}
	}

	private void GeneralWindowClosedCallback()
	{
		HudMenuUtility.SetConnectAlertSimpleUI(false);
		if (m_costErrorType != ButtonInfoTable.ButtonType.UNKNOWN)
		{
			if (GeneralWindow.IsYesButtonPressed)
			{
				s_isActive = false;
				m_setup = false;
				m_buyId = ServerItem.Id.NONE;
				ActiveAnimation activeAnimation = ActiveAnimation.Play(m_animation, "ui_cmn_window_Anim", Direction.Reverse);
				if (activeAnimation != null)
				{
					EventDelegate.Add(activeAnimation.onFinished, OnAnimFinished, true);
				}
				HudMenuUtility.SendMenuButtonClicked(m_costErrorType);
				m_costErrorType = ButtonInfoTable.ButtonType.UNKNOWN;
			}
			else
			{
				SetBtnObjectEnabeld(true);
				m_costErrorType = ButtonInfoTable.ButtonType.UNKNOWN;
			}
		}
		GeneralWindow.Close();
	}

	private void ServerUnlockedCharacter_Succeeded(MsgGetPlayerStateSucceed msg)
	{
		Animation animation = GameObjectUtil.FindChildGameObjectComponent<Animation>(base.gameObject, "window");
		if (animation != null)
		{
			ActiveAnimation.Play(animation, "ui_menu_player_open_Anim", Direction.Forward);
		}
		if (m_parent != null)
		{
			m_parent.UpdateView();
		}
		SetupBtn();
		SetupObject();
		ServerPlayerState playerState = ServerInterface.PlayerState;
		if (playerState != null && playerState.unlockedCharacterNum == 2 && m_oldUnlockedCharacterNum == 1 && m_parent != null && m_parent.parent != null)
		{
			m_parent.parent.SetChara(PlayerCharaList.SET_CHARA_MODE.SUB, m_charaType);
		}
		Debug.Log("ServerUnlockedCharacter_Succeeded oldUnlockedCharacterNum:" + m_oldUnlockedCharacterNum + "  currentUnlockedCharacterNum:" + playerState.unlockedCharacterNum);
		SoundManager.SePlay("sys_buy");
		SetBtnObjectEnabeld(false);
		HudMenuUtility.SendMsgUpdateSaveDataDisplay();
	}

	public override void OnClickPlatformBackButton(BackButtonMessage msg)
	{
		if (msg != null)
		{
			msg.StaySequence();
		}
		if (!GeneralWindow.Created && !NetworkErrorWindow.Created)
		{
			s_isActive = false;
			m_setup = false;
			m_costErrorType = ButtonInfoTable.ButtonType.UNKNOWN;
			m_buyId = ServerItem.Id.NONE;
			ActiveAnimation activeAnimation = ActiveAnimation.Play(m_animation, "ui_cmn_window_Anim", Direction.Reverse);
			if (activeAnimation != null)
			{
				EventDelegate.Add(activeAnimation.onFinished, OnAnimFinished, true);
			}
			SoundManager.SePlay("sys_window_close");
		}
	}
}
