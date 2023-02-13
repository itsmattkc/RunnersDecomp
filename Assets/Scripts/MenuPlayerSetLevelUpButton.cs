using Message;
using SaveData;
using Text;
using UnityEngine;

public class MenuPlayerSetLevelUpButton : MonoBehaviour
{
	private enum EventSignal
	{
		BUTTON_PRESSED = 100,
		LEVEL_UP_END
	}

	public delegate void LevelUpCallback(AbilityType abilityType);

	private TinyFsmBehavior m_fsm;

	private CharaType m_charaType;

	private GameObject m_pageRootObject;

	private bool m_is_end_connect;

	private LevelUpCallback m_callback;

	private GameObject m_saleIcon;

	private AbilityType m_currentLevelUpAbility;

	private void OnEnable()
	{
	}

	public void Setup(CharaType charaType, GameObject pageRootObject)
	{
		m_charaType = charaType;
		m_pageRootObject = pageRootObject;
		GameObject gameObject = GameObjectUtil.FindChildGameObject(m_pageRootObject, "Btn_lv_up");
		if (gameObject != null)
		{
			UIButtonMessage uIButtonMessage = gameObject.GetComponent<UIButtonMessage>();
			if (uIButtonMessage == null)
			{
				uIButtonMessage = gameObject.AddComponent<UIButtonMessage>();
			}
			uIButtonMessage.target = base.gameObject;
			uIButtonMessage.functionName = "LevelUpButtonClickedCallback";
			m_saleIcon = GameObjectUtil.FindChildGameObject(gameObject, "img_icon_sale");
		}
		InitCostLabel();
	}

	public void InitCostLabel()
	{
		int id = (int)new ServerItem(m_charaType).id;
		ServerCampaignData campaignInSession = ServerInterface.CampaignState.GetCampaignInSession(Constants.Campaign.emType.CharacterUpgradeCost, id);
		if (campaignInSession != null)
		{
			if (m_saleIcon != null)
			{
				m_saleIcon.SetActive(true);
			}
		}
		else if (m_saleIcon != null)
		{
			m_saleIcon.SetActive(false);
		}
		UILabel uILabel = GameObjectUtil.FindChildGameObjectComponent<UILabel>(base.gameObject, "Lbl_price_number");
		if (uILabel != null)
		{
			int abilityCost = MenuPlayerSetUtil.GetAbilityCost(m_charaType);
			int b = abilityCost - MenuPlayerSetUtil.GetCurrentExp(m_charaType);
			b = Mathf.Max(0, b);
			uILabel.text = HudUtility.GetFormatNumString(b);
		}
		UISlider uISlider = GameObjectUtil.FindChildGameObjectComponent<UISlider>(base.gameObject, "Pgb_exp");
		if (uISlider != null)
		{
			float num = uISlider.value = MenuPlayerSetUtil.GetCurrentExpRatio(m_charaType);
		}
	}

	public void SetCallback(LevelUpCallback callback)
	{
		m_callback = callback;
	}

	public void OnLevelUpEnd()
	{
		TinyFsmEvent signal = TinyFsmEvent.CreateUserEvent(101);
		if (m_fsm != null)
		{
			m_fsm.Dispatch(signal);
		}
	}

	private void Start()
	{
		m_fsm = base.gameObject.AddComponent<TinyFsmBehavior>();
		TinyFsmBehavior.Description description = new TinyFsmBehavior.Description(this);
		description.initState = new TinyFsmState(StateWaitLevelUpButtonPressed);
		description.onFixedUpdate = true;
		m_fsm.SetUp(description);
	}

	private void Update()
	{
		InitCostLabel();
	}

	private TinyFsmState StateWaitLevelUpButtonPressed(TinyFsmEvent e)
	{
		switch (e.Signal)
		{
		case -3:
			m_is_end_connect = false;
			if (MenuPlayerSetUtil.IsCharacterLevelMax(m_charaType))
			{
				UIImageButton uIImageButton = GameObjectUtil.FindChildGameObjectComponent<UIImageButton>(m_pageRootObject, "Btn_lv_up");
				if (uIImageButton != null)
				{
					uIImageButton.isEnabled = false;
				}
			}
			return TinyFsmState.End();
		case -4:
			return TinyFsmState.End();
		case 0:
			return TinyFsmState.End();
		case 100:
			m_currentLevelUpAbility = MenuPlayerSetUtil.GetNextLevelUpAbility(m_charaType);
			m_fsm.ChangeState(new TinyFsmState(StateAskLevelUp));
			return TinyFsmState.End();
		default:
			return TinyFsmState.End();
		}
	}

	private TinyFsmState StateAskLevelUp(TinyFsmEvent e)
	{
		switch (e.Signal)
		{
		case -3:
			return TinyFsmState.End();
		case -4:
			return TinyFsmState.End();
		case 0:
		{
			GeneralWindow.Close();
			SaveDataManager instance = SaveDataManager.Instance;
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
				if (ServerInterface.LoggedInServerInterface != null)
				{
					ServerInterface component = GameObject.Find("ServerInterface").GetComponent<ServerInterface>();
					ServerPlayerState playerState = ServerInterface.PlayerState;
					if (playerState != null)
					{
						ServerCharacterState serverCharacterState = playerState.CharacterState(m_charaType);
						if (serverCharacterState != null)
						{
							int abilityId = MenuPlayerSetUtil.TransformServerAbilityID(m_currentLevelUpAbility);
							component.RequestServerUpgradeCharacter(serverCharacterState.Id, abilityId, base.gameObject);
						}
					}
				}
				else
				{
					CharaData charaData = instance.CharaData;
					CharaAbility charaAbility = charaData.AbilityArray[(int)m_charaType];
					charaAbility.Ability[(int)m_currentLevelUpAbility]++;
					ServerUpgradeCharacter_Succeeded(null);
				}
				m_fsm.ChangeState(new TinyFsmState(StateWaitServerConnectEnd));
			}
			else
			{
				GeneralWindow.CInfo info = default(GeneralWindow.CInfo);
				string text = TextManager.GetText(TextManager.TextType.TEXTTYPE_COMMON_TEXT, "MainMenu", "insufficient_ring").text;
				string text2 = TextManager.GetText(TextManager.TextType.TEXTTYPE_COMMON_TEXT, "ItemSet", "gw_buy_ring__error_text").text;
				info.caption = text;
				info.message = text2;
				info.anchor_path = "Camera/menu_Anim/PlayerSet_2_UI/Anchor_5_MC";
				info.buttonType = GeneralWindow.ButtonType.Ok;
				info.isPlayErrorSe = true;
				info.finishedCloseDelegate = GeneralWindowCloseCallback;
				GeneralWindow.Create(info);
				m_fsm.ChangeState(new TinyFsmState(StateFailedLevelUp));
			}
			return TinyFsmState.End();
		}
		default:
			return TinyFsmState.End();
		}
	}

	private TinyFsmState StateWaitServerConnectEnd(TinyFsmEvent e)
	{
		switch (e.Signal)
		{
		case -3:
			return TinyFsmState.End();
		case -4:
			return TinyFsmState.End();
		case 0:
			if (m_is_end_connect)
			{
				m_callback(m_currentLevelUpAbility);
				m_fsm.ChangeState(new TinyFsmState(StateWaitLevelUpAnimation));
			}
			return TinyFsmState.End();
		default:
			return TinyFsmState.End();
		}
	}

	private TinyFsmState StateFailedLevelUp(TinyFsmEvent e)
	{
		switch (e.Signal)
		{
		case -3:
			return TinyFsmState.End();
		case -4:
			return TinyFsmState.End();
		case 0:
			return TinyFsmState.End();
		default:
			return TinyFsmState.End();
		}
	}

	private TinyFsmState StateWaitLevelUpAnimation(TinyFsmEvent e)
	{
		switch (e.Signal)
		{
		case -3:
			return TinyFsmState.End();
		case -4:
			return TinyFsmState.End();
		case 0:
			return TinyFsmState.End();
		case 101:
			BackKeyManager.InvalidFlag = false;
			m_fsm.ChangeState(new TinyFsmState(StateWaitLevelUpButtonPressed));
			return TinyFsmState.End();
		default:
			return TinyFsmState.End();
		}
	}

	private TinyFsmState StateLevelUpAbilityExplain(TinyFsmEvent e)
	{
		switch (e.Signal)
		{
		case -3:
		{
			int level = MenuPlayerSetUtil.GetLevel(m_charaType, m_currentLevelUpAbility);
			int abilityCost = MenuPlayerSetUtil.GetAbilityCost(m_charaType);
			GeneralWindow.CInfo info = default(GeneralWindow.CInfo);
			info.caption = TextManager.GetText(TextManager.TextType.TEXTTYPE_COMMON_TEXT, "WindowText", "level_up_caption").text;
			TextObject text = TextManager.GetText(TextManager.TextType.TEXTTYPE_COMMON_TEXT, "WindowText", "level_up_text");
			text.ReplaceTag("{RING_COST}", abilityCost.ToString());
			string text2 = TextManager.GetText(TextManager.TextType.TEXTTYPE_COMMON_TEXT, "CharaStatus", "abilityname" + (int)(m_currentLevelUpAbility + 1)).text;
			text.ReplaceTag("{ABILITY_NAME}", text2);
			string text3 = TextManager.GetText(TextManager.TextType.TEXTTYPE_COMMON_TEXT, "CharaStatus", "abilitycaption" + (int)(m_currentLevelUpAbility + 1)).text;
			text.ReplaceTag("{ABILITY_CAPTION}", text3);
			text.ReplaceTag("{ABILITY_CAPTION2}", text3);
			TextObject text4 = TextManager.GetText(TextManager.TextType.TEXTTYPE_COMMON_TEXT, "CharaStatus", "abilitypotential" + (int)(m_currentLevelUpAbility + 1));
			TextObject text5 = TextManager.GetText(TextManager.TextType.TEXTTYPE_COMMON_TEXT, "CharaStatus", "abilitypotential" + (int)(m_currentLevelUpAbility + 1));
			float levelAbility = MenuPlayerSetUtil.GetLevelAbility(m_charaType, m_currentLevelUpAbility, level - 1);
			float levelAbility2 = MenuPlayerSetUtil.GetLevelAbility(m_charaType, m_currentLevelUpAbility, level);
			text4.ReplaceTag("{ABILITY_POTENTIAL}", levelAbility.ToString());
			text5.ReplaceTag("{ABILITY_POTENTIAL}", levelAbility2.ToString());
			text.ReplaceTag("{ABILITY_POTENTIAL}", text4.text);
			text.ReplaceTag("{ABILITY_POTENTIAL2}", text5.text);
			info.message = text.text;
			info.anchor_path = "Camera/menu_Anim/PlayerSet_2_UI/Anchor_5_MC";
			info.buttonType = GeneralWindow.ButtonType.Ok;
			info.finishedCloseDelegate = GeneralWindowCloseCallback;
			GeneralWindow.Create(info);
			return TinyFsmState.End();
		}
		case -4:
			return TinyFsmState.End();
		case 0:
			return TinyFsmState.End();
		default:
			return TinyFsmState.End();
		}
	}

	private void LevelUpButtonClickedCallback()
	{
		TinyFsmEvent signal = TinyFsmEvent.CreateUserEvent(100);
		if (m_fsm != null)
		{
			m_fsm.Dispatch(signal);
		}
		if (HudMenuUtility.IsTutorial_CharaLevelUp())
		{
			GameObjectUtil.SendMessageFindGameObject("PlayerSet_2_UI", "OnClickedLevelUpButton", null, SendMessageOptions.DontRequireReceiver);
		}
	}

	private void ServerUpgradeCharacter_Succeeded(MsgGetPlayerStateSucceed msg)
	{
		m_is_end_connect = true;
		SoundManager.SePlay("sys_buy");
		HudMenuUtility.SendMsgUpdateSaveDataDisplay();
		StageAbilityManager instance = StageAbilityManager.Instance;
		if (instance != null)
		{
			instance.RecalcAbilityVaue();
		}
	}

	public void GeneralWindowCloseCallback()
	{
		Debug.Log("GeneralWindowCloseCallback IsOkButtonPressed:" + GeneralWindow.IsOkButtonPressed);
		if (m_fsm != null && GeneralWindow.IsOkButtonPressed)
		{
			BackKeyManager.InvalidFlag = false;
			m_fsm.ChangeState(new TinyFsmState(StateWaitLevelUpButtonPressed));
		}
	}
}
