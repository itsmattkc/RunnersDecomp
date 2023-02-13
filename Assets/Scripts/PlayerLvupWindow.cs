using AnimationOrTween;
using Message;
using System.Collections;
using System.Collections.Generic;
using Text;
using UnityEngine;

public class PlayerLvupWindow : WindowBase
{
	private static bool s_isActive;

	[SerializeField]
	private Animation m_animation;

	[SerializeField]
	private UIPanel m_mainPanel;

	private bool m_isClickClose;

	private bool m_isEnd;

	private ui_player_set_scroll m_parent;

	private ServerCharacterState m_charaState;

	private CharaType m_charaType = CharaType.UNKNOWN;

	private AbilityType m_currentLevelUpAbility = AbilityType.NONE;

	private Dictionary<AbilityType, int> m_lvList;

	private Dictionary<AbilityType, float> m_paramList;

	private List<AbilityType> m_abilityList;

	private bool m_lock;

	private UIRectItemStorage m_storage;

	private Dictionary<AbilityType, MenuPlayerSetAbilityButton> m_btnObjList;

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

	private void OnDestroy()
	{
		Destroy();
	}

	public static bool Open(ui_player_set_scroll parent, CharaType charaType)
	{
		bool result = false;
		GameObject menuAnimUIObject = HudMenuUtility.GetMenuAnimUIObject();
		if (menuAnimUIObject != null)
		{
			PlayerLvupWindow playerLvupWindow = GameObjectUtil.FindChildGameObjectComponent<PlayerLvupWindow>(menuAnimUIObject, "PlayerLvupWindowUI");
			if (playerLvupWindow == null)
			{
				GameObject gameObject = GameObjectUtil.FindChildGameObject(menuAnimUIObject, "PlayerLvupWindowUI");
				if (gameObject != null)
				{
					playerLvupWindow = gameObject.AddComponent<PlayerLvupWindow>();
				}
			}
			if (playerLvupWindow != null)
			{
				playerLvupWindow.Setup(parent, charaType);
				result = true;
			}
		}
		return result;
	}

	private void Setup(ui_player_set_scroll parent, CharaType charaType)
	{
		s_isActive = true;
		base.gameObject.SetActive(true);
		m_charaType = charaType;
		m_parent = parent;
		m_lock = false;
		if (m_abilityList == null)
		{
			m_abilityList = new List<AbilityType>();
			m_abilityList.Add(AbilityType.INVINCIBLE);
			m_abilityList.Add(AbilityType.COMBO);
			m_abilityList.Add(AbilityType.MAGNET);
			m_abilityList.Add(AbilityType.TRAMPOLINE);
			m_abilityList.Add(AbilityType.ASTEROID);
			m_abilityList.Add(AbilityType.LASER);
			m_abilityList.Add(AbilityType.DRILL);
			m_abilityList.Add(AbilityType.DISTANCE_BONUS);
			m_abilityList.Add(AbilityType.RING_BONUS);
			m_abilityList.Add(AbilityType.ANIMAL);
		}
		SetParam();
		StartCoroutine(SetupObject(true));
	}

	private void SetParam()
	{
		ServerPlayerState playerState = ServerInterface.PlayerState;
		m_charaState = playerState.CharacterState(m_charaType);
		if (m_lvList == null)
		{
			m_lvList = new Dictionary<AbilityType, int>();
		}
		else
		{
			m_lvList.Clear();
		}
		if (m_paramList == null)
		{
			m_paramList = new Dictionary<AbilityType, float>();
		}
		else
		{
			m_paramList.Clear();
		}
		ImportAbilityTable instance = ImportAbilityTable.GetInstance();
		int num = 10;
		for (int i = 0; i < num; i++)
		{
			AbilityType abilityType = (AbilityType)i;
			ServerItem.Id id = ServerItem.ConvertAbilityId(abilityType);
			int num2 = m_charaState.AbilityLevel[(int)(id - 120000)];
			float abilityPotential = instance.GetAbilityPotential(abilityType, num2);
			m_lvList.Add(abilityType, num2);
			m_paramList.Add(abilityType, abilityPotential);
		}
	}

	private IEnumerator SetupObject(bool init)
	{
		yield return null;
		if (m_mainPanel != null)
		{
			m_mainPanel.alpha = 1f;
		}
		if (init && m_animation != null)
		{
			ActiveAnimation anim = ActiveAnimation.Play(m_animation, "ui_cmn_window_Anim2", Direction.Forward);
			if (anim != null)
			{
				EventDelegate.Add(anim.onFinished, OnFinishedInAnim, true);
			}
		}
		GeneralUtil.SetButtonFunc(base.gameObject, "Btn_close", base.gameObject, "OnClickClose");
		GeneralUtil.SetButtonFunc(base.gameObject, "Btn_lv_up", base.gameObject, "OnClickLvUp");
		UIPlayAnimation btnClose = GameObjectUtil.FindChildGameObjectComponent<UIPlayAnimation>(base.gameObject, "Btn_close");
		if (btnClose != null && !EventDelegate.IsValid(btnClose.onFinished))
		{
			EventDelegate.Add(btnClose.onFinished, OnFinished, true);
		}
		m_isEnd = false;
		m_isClickClose = false;
		GameObject saleObj = GameObjectUtil.FindChildGameObject(base.gameObject, "img_icon_sale");
		UILabel labCaption = GameObjectUtil.FindChildGameObjectComponent<UILabel>(base.gameObject, "Lbl_caption");
		UILabel labLv = GameObjectUtil.FindChildGameObjectComponent<UILabel>(base.gameObject, "Lbl_player_lv");
		UILabel labCost = GameObjectUtil.FindChildGameObjectComponent<UILabel>(base.gameObject, "Lbl_price_number");
		UITexture texChara = GameObjectUtil.FindChildGameObjectComponent<UITexture>(base.gameObject, "img_player_tex");
		UISlider sliExp = GameObjectUtil.FindChildGameObjectComponent<UISlider>(base.gameObject, "Pgb_exp");
		UIImageButton imgBtn = GameObjectUtil.FindChildGameObjectComponent<UIImageButton>(base.gameObject, "Btn_lv_up");
		m_storage = GameObjectUtil.FindChildGameObjectComponent<UIRectItemStorage>(base.gameObject, "slot");
		if (m_storage != null && init)
		{
			if (m_btnObjList == null)
			{
				m_btnObjList = new Dictionary<AbilityType, MenuPlayerSetAbilityButton>();
			}
			else
			{
				m_btnObjList.Clear();
			}
			m_btnObjList = new Dictionary<AbilityType, MenuPlayerSetAbilityButton>();
			int cnt = 0;
			List<GameObject> buttonList = GameObjectUtil.FindChildGameObjects(m_storage.gameObject, "ui_player_set_item_2_cell(Clone)");
			foreach (AbilityType key in m_abilityList)
			{
				if (buttonList.Count > cnt)
				{
					MenuPlayerSetAbilityButton button = buttonList[cnt].GetComponent<MenuPlayerSetAbilityButton>();
					if (button == null)
					{
						button = buttonList[cnt].AddComponent<MenuPlayerSetAbilityButton>();
					}
					if (button != null)
					{
						button.Setup(m_charaType, key);
						m_btnObjList.Add(key, button);
					}
				}
				cnt++;
			}
		}
		if (texChara != null)
		{
			TextureRequestChara textureRequest = new TextureRequestChara(m_charaType, texChara);
			TextureAsyncLoadManager.Instance.Request(textureRequest);
			if (m_charaState.IsUnlocked)
			{
				texChara.color = new Color(1f, 1f, 1f);
			}
			else
			{
				texChara.color = new Color(0f, 0f, 0f);
			}
		}
		int charaId = (int)new ServerItem(m_charaType).id;
		ServerCampaignData campaignData = ServerInterface.CampaignState.GetCampaignInSession(Constants.Campaign.emType.CharacterUpgradeCost, charaId);
		if (campaignData != null)
		{
			if (saleObj != null)
			{
				saleObj.SetActive(true);
			}
		}
		else if (saleObj != null)
		{
			saleObj.SetActive(false);
		}
		if (labCaption != null)
		{
			labCaption.text = TextManager.GetText(TextManager.TextType.TEXTTYPE_COMMON_TEXT, "WindowText", "chara_level_up_caption").text;
		}
		if (labLv != null)
		{
			int totalLevel = MenuPlayerSetUtil.GetTotalLevel(m_charaType);
			labLv.text = TextUtility.GetTextLevel(string.Format("{0:000}", totalLevel));
		}
		if (labCost != null)
		{
			int levelUpCost = MenuPlayerSetUtil.GetAbilityCost(m_charaType);
			int remainCost2 = levelUpCost - MenuPlayerSetUtil.GetCurrentExp(m_charaType);
			remainCost2 = Mathf.Max(0, remainCost2);
			labCost.text = HudUtility.GetFormatNumString(remainCost2);
		}
		if (imgBtn != null)
		{
			imgBtn.isEnabled = !MenuPlayerSetUtil.IsCharacterLevelMax(m_charaType);
		}
		if (init)
		{
			SoundManager.SePlay("sys_window_open");
		}
		if (sliExp != null)
		{
			float expRatio = sliExp.value = MenuPlayerSetUtil.GetCurrentExpRatio(m_charaType);
			GameObject barObj = GameObjectUtil.FindChildGameObject(base.gameObject, "img_bar_body");
			if (barObj != null)
			{
				barObj.SetActive(sliExp.value > 0f);
			}
		}
	}

	private void OnClickLvUp()
	{
		if (m_parent != null && m_parent.parent != null && m_parent.parent.isTutorial)
		{
			TutorialCursor.EndTutorialCursor(TutorialCursor.Type.CHARASELECT_LEVEL_UP);
			m_parent.parent.SetTutorialEnd();
		}
		if (GeneralUtil.IsNetwork())
		{
			SendLevelUp();
		}
		else
		{
			GeneralUtil.ShowNoCommunication();
		}
		Debug.Log("OnClickLvUp");
	}

	private bool SendLevelUp()
	{
		bool result = false;
		if (!m_lock && m_charaState != null && m_charaType != CharaType.UNKNOWN)
		{
			SaveDataManager instance = SaveDataManager.Instance;
			if (instance != null)
			{
				int num = 0;
				int abilityCost = MenuPlayerSetUtil.GetAbilityCost(m_charaType);
				int b = abilityCost - MenuPlayerSetUtil.GetCurrentExp(m_charaType);
				b = Mathf.Max(0, b);
				num = b;
				bool flag = true;
				if ((int)instance.ItemData.RingCount - num < 0)
				{
					flag = false;
				}
				if (flag)
				{
					BackKeyManager.InvalidFlag = true;
					m_currentLevelUpAbility = MenuPlayerSetUtil.GetNextLevelUpAbility(m_charaType);
					if (ServerInterface.LoggedInServerInterface != null && m_currentLevelUpAbility != AbilityType.NONE)
					{
						ServerInterface component = GameObject.Find("ServerInterface").GetComponent<ServerInterface>();
						int abilityId = MenuPlayerSetUtil.TransformServerAbilityID(m_currentLevelUpAbility);
						component.RequestServerUpgradeCharacter(m_charaState.Id, abilityId, base.gameObject);
						m_lock = true;
						result = true;
					}
				}
				else
				{
					GeneralWindow.CInfo info = default(GeneralWindow.CInfo);
					string text = TextManager.GetText(TextManager.TextType.TEXTTYPE_COMMON_TEXT, "MainMenu", "insufficient_ring").text;
					string text2 = TextManager.GetText(TextManager.TextType.TEXTTYPE_COMMON_TEXT, "ItemSet", "gw_buy_ring__error_text").text;
					info.caption = text;
					info.message = text2;
					info.buttonType = GeneralWindow.ButtonType.ShopCancel;
					info.isPlayErrorSe = true;
					info.finishedCloseDelegate = GeneralWindowCloseCallback;
					GeneralWindow.Create(info);
				}
			}
		}
		return result;
	}

	private void OnClickClose()
	{
		if (!m_isClickClose)
		{
			if (m_parent != null && m_parent.parent != null && m_parent.parent.isTutorial)
			{
				TutorialCursor.EndTutorialCursor(TutorialCursor.Type.CHARASELECT_LEVEL_UP);
				m_parent.parent.SetTutorialEnd();
			}
			m_isClickClose = true;
			s_isActive = false;
			ActiveAnimation activeAnimation = ActiveAnimation.Play(m_animation, "ui_cmn_window_Anim2", Direction.Reverse);
			if (activeAnimation != null)
			{
				EventDelegate.Add(activeAnimation.onFinished, OnFinished, true);
			}
			SoundManager.SePlay("sys_window_close");
		}
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
		if (m_parent != null && m_parent.parent != null && m_parent.parent.isTutorial)
		{
			TutorialCursor.EndTutorialCursor(TutorialCursor.Type.CHARASELECT_LEVEL_UP);
			m_parent.parent.SetTutorialEnd();
		}
		if (!m_isClickClose && !GeneralWindow.Created && !NetworkErrorWindow.Created)
		{
			s_isActive = false;
			m_isClickClose = true;
			ActiveAnimation activeAnimation = ActiveAnimation.Play(m_animation, "ui_cmn_window_Anim2", Direction.Reverse);
			if (activeAnimation != null)
			{
				EventDelegate.Add(activeAnimation.onFinished, OnFinished, true);
			}
			SoundManager.SePlay("sys_window_close");
		}
	}

	private void OnFinishedInAnim()
	{
		if (!(m_parent == null))
		{
			PlayerCharaList parent = m_parent.parent;
			if (!(parent == null) && !parent.isTutorial)
			{
				BackKeyManager.InvalidFlag = false;
			}
		}
	}

	private void ServerUpgradeCharacter_Succeeded(MsgGetPlayerStateSucceed msg)
	{
		m_lock = false;
		BackKeyManager.InvalidFlag = false;
		SoundManager.SePlay("sys_buy");
		HudMenuUtility.SendMsgUpdateSaveDataDisplay();
		StageAbilityManager instance = StageAbilityManager.Instance;
		if (instance != null)
		{
			instance.RecalcAbilityVaue();
		}
		ServerPlayerState playerState = ServerInterface.PlayerState;
		if (playerState != null)
		{
			m_charaState = playerState.CharacterState(m_charaType);
		}
		if (m_currentLevelUpAbility != AbilityType.NONE)
		{
			MenuPlayerSetAbilityButton menuPlayerSetAbilityButton = m_btnObjList[m_currentLevelUpAbility];
			if (menuPlayerSetAbilityButton != null)
			{
				menuPlayerSetAbilityButton.LevelUp(LevelUpAnimationEndCallback);
			}
		}
		m_currentLevelUpAbility = AbilityType.NONE;
		SetParam();
		StartCoroutine(SetupObject(false));
		if (m_parent != null)
		{
			m_parent.UpdateView();
		}
	}

	public void GeneralWindowCloseCallback()
	{
		Debug.Log("GeneralWindowCloseCallback IsOkButtonPressed:" + GeneralWindow.IsOkButtonPressed);
		if (GeneralWindow.IsYesButtonPressed)
		{
			s_isActive = false;
			m_isClickClose = true;
			ActiveAnimation activeAnimation = ActiveAnimation.Play(m_animation, "ui_cmn_window_Anim2", Direction.Reverse);
			if (activeAnimation != null)
			{
				EventDelegate.Add(activeAnimation.onFinished, OnFinished, true);
			}
			HudMenuUtility.SendMenuButtonClicked(ButtonInfoTable.ButtonType.RING_TO_SHOP);
			m_lock = false;
		}
		BackKeyManager.InvalidFlag = false;
	}

	private void LevelUpAnimationEndCallback()
	{
	}
}
