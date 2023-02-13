using Message;
using SaveData;
using System.Collections;
using System.Collections.Generic;
using Text;
using UnityEngine;

public class ItemSetMenu : MonoBehaviour
{
	private enum TutorialMode
	{
		Idle,
		AppolloStartWait,
		Window1,
		Laser,
		AppolloEndWait,
		Play,
		Window2,
		Window3,
		SubChara
	}

	private enum ButtonType
	{
		NORMAL_STAGE,
		CHALLENGE_BOSS,
		SP_STAGE,
		RAID_BOSS_ATTACK,
		NUM
	}

	private const float UPDATE_TIME = 0.25f;

	private const uint MAX_RAID_ATTACK_CHARGE = 3u;

	private ItemSet m_itemSet;

	private InstantItemSet m_instantItemSet;

	private ItemSetRingManagement m_ringManagement;

	private List<GameObject> m_hideGameObjects = new List<GameObject>();

	private MsgMenuItemSetStart m_msg;

	private float m_timer = 3f;

	private TutorialMode m_tutorialMode;

	private float m_targetFrameTime = 0.0166666675f;

	private float m_timeCounter = 0.25f;

	private bool m_isRaidMenu;

	private uint m_raidChargeCount = 1u;

	private uint m_raidChargeCountMax = 3u;

	private UILabel m_ChargeText;

	private UILabel m_AttackRateText;

	private UILabel m_bossTime;

	private UILabel m_ChallengeCountLabel;

	private GameObject m_playBetObject0;

	private GameObject m_playBetObject1;

	private GameObject m_playBetMaxSign;

	private UISlider m_bossLifeBar;

	private RaidBossData m_currentRaidBossData;

	private SendApollo m_sendApollo;

	private bool m_isRaidbossTimeOver;

	private bool m_isUpdateFreeItems;

	private bool m_UpdateCharaSettingFlag;

	private bool m_isEndSetup;

	public bool IsEndSetup
	{
		get
		{
			return m_isEndSetup;
		}
	}

	private void Start()
	{
		StartCoroutine(OnStart());
	}

	private void OnEnable()
	{
		DeckViewWindow.Reset();
		GameObject gameObject = GameObjectUtil.FindChildGameObject(base.gameObject, "Anchor_7_BL");
		if (gameObject != null)
		{
			GameObject gameObject2 = GameObjectUtil.FindChildGameObject(gameObject, "Btn_charaset");
			if (gameObject2 != null)
			{
				gameObject2.SetActive(true);
				UIButtonMessage uIButtonMessage = gameObject2.GetComponent<UIButtonMessage>();
				if (uIButtonMessage == null)
				{
					uIButtonMessage = gameObject2.AddComponent<UIButtonMessage>();
				}
				uIButtonMessage.target = base.gameObject;
				uIButtonMessage.functionName = "CharaSetButtonClickedCallback";
				GeneralUtil.SetCharasetBtnIcon(base.gameObject);
			}
			GameObject gameObject3 = GameObjectUtil.FindChildGameObject(gameObject, "Btn_cmn_back");
			if (gameObject3 != null)
			{
				gameObject3.SetActive(true);
			}
		}
		bool flag = false;
		if (StageModeManager.Instance != null)
		{
			flag = StageModeManager.Instance.IsQuickMode();
		}
		ButtonType buttonType = ButtonType.NORMAL_STAGE;
		m_isRaidMenu = false;
		m_isRaidbossTimeOver = false;
		if (!flag)
		{
			if (EventManager.Instance.IsSpecialStage())
			{
				buttonType = ButtonType.SP_STAGE;
			}
			else if (EventManager.Instance.IsRaidBossStage())
			{
				buttonType = ButtonType.RAID_BOSS_ATTACK;
				m_currentRaidBossData = RaidBossInfo.currentRaidData;
				m_isRaidMenu = true;
				m_timeCounter = 0.25f;
				m_targetFrameTime = 1f / (float)Application.targetFrameRate;
			}
		}
		GameObject gameObject4 = GameObjectUtil.FindChildGameObject(base.gameObject, "Anchor_9_BR");
		if (!(gameObject4 != null))
		{
			return;
		}
		for (int i = 0; i < 4; i++)
		{
			string name = "pattern_" + i;
			GameObject gameObject5 = GameObjectUtil.FindChildGameObject(gameObject4, name);
			if (!(gameObject5 == null))
			{
				gameObject5.SetActive(false);
			}
		}
		int num = (int)buttonType;
		string name2 = "pattern_" + num;
		GameObject gameObject6 = GameObjectUtil.FindChildGameObject(gameObject4, name2);
		if (gameObject6 != null)
		{
			gameObject6.SetActive(true);
			if (EventManager.Instance.EventStage)
			{
				UITexture uITexture = GameObjectUtil.FindChildGameObjectComponent<UITexture>(gameObject6, "img_stage_tex");
				if (uITexture != null)
				{
					uITexture.mainTexture = EventUtility.GetBGTexture();
					BoxCollider component = uITexture.gameObject.GetComponent<BoxCollider>();
					if (component != null)
					{
						component.enabled = false;
					}
				}
			}
			else if (flag && EventManager.Instance.Type == EventManager.EventType.QUICK)
			{
				int num2 = 1;
				if (StageModeManager.Instance != null)
				{
					num2 = StageModeManager.Instance.QuickStageIndex;
				}
				UISprite uISprite = GameObjectUtil.FindChildGameObjectComponent<UISprite>(gameObject6, "img_next_map_cache");
				if (uISprite != null)
				{
					uISprite.gameObject.SetActive(true);
					uISprite.spriteName = "ui_mm_map_thumb_w" + num2.ToString("D2") + "a";
				}
				GameObject gameObject7 = GameObjectUtil.FindChildGameObject(gameObject6, "img_next_map");
				if (gameObject7 != null && gameObject7.activeSelf)
				{
					gameObject7.SetActive(false);
				}
				for (int j = 0; j < 3; j++)
				{
					string name3 = "img_icon_type_" + (j + 1);
					UISprite uISprite2 = GameObjectUtil.FindChildGameObjectComponent<UISprite>(gameObject6, name3);
					if (uISprite2 != null)
					{
						uISprite2.enabled = true;
						uISprite2.gameObject.SetActive(true);
						switch (j)
						{
						case 0:
							uISprite2.spriteName = "ui_chao_set_type_icon_power";
							break;
						case 1:
							uISprite2.spriteName = "ui_chao_set_type_icon_fly";
							break;
						case 2:
							uISprite2.spriteName = "ui_chao_set_type_icon_speed";
							break;
						}
					}
				}
			}
			else if (buttonType == ButtonType.NORMAL_STAGE)
			{
				GameObject gameObject8 = GameObjectUtil.FindChildGameObject(gameObject6, "img_next_map_cache");
				if (gameObject8 != null && gameObject8.activeSelf)
				{
					gameObject8.SetActive(false);
				}
				int stageIndex = 1;
				if (MileageMapDataManager.Instance != null && StageModeManager.Instance != null)
				{
					stageIndex = (flag ? StageModeManager.Instance.QuickStageIndex : MileageMapDataManager.Instance.MileageStageIndex);
				}
				UISprite uISprite3 = GameObjectUtil.FindChildGameObjectComponent<UISprite>(gameObject6, "img_next_map");
				if (uISprite3 != null)
				{
					uISprite3.gameObject.SetActive(true);
					uISprite3.spriteName = "ui_mm_map_thumb_w" + stageIndex.ToString("00") + "a";
				}
				CharacterAttribute[] characterAttribute = MileageMapUtility.GetCharacterAttribute(stageIndex);
				if (characterAttribute != null)
				{
					for (int k = 0; k < 3; k++)
					{
						string name4 = "img_icon_type_" + (k + 1);
						UISprite uISprite4 = GameObjectUtil.FindChildGameObjectComponent<UISprite>(gameObject6, name4);
						if (!(uISprite4 != null))
						{
							continue;
						}
						if (k < characterAttribute.Length)
						{
							switch (characterAttribute[k])
							{
							case CharacterAttribute.SPEED:
								uISprite4.enabled = true;
								uISprite4.gameObject.SetActive(true);
								uISprite4.spriteName = "ui_chao_set_type_icon_speed";
								break;
							case CharacterAttribute.FLY:
								uISprite4.enabled = true;
								uISprite4.gameObject.SetActive(true);
								uISprite4.spriteName = "ui_chao_set_type_icon_fly";
								break;
							case CharacterAttribute.POWER:
								uISprite4.enabled = true;
								uISprite4.gameObject.SetActive(true);
								uISprite4.spriteName = "ui_chao_set_type_icon_power";
								break;
							default:
								uISprite4.gameObject.SetActive(false);
								uISprite4.enabled = false;
								break;
							}
						}
						else
						{
							uISprite4.gameObject.SetActive(false);
							uISprite4.enabled = false;
						}
					}
				}
			}
		}
		if (m_isRaidMenu)
		{
			int num3 = (int)buttonType;
			string name5 = "pattern_" + num3;
			GameObject parent = GameObjectUtil.FindChildGameObject(gameObject4, name5);
			m_raidChargeCount = 1u;
			m_raidChargeCountMax = 1u;
			m_ChallengeCountLabel = GameObjectUtil.FindChildGameObjectComponent<UILabel>(parent, "Lbl_EVchallenge");
			m_ChargeText = GameObjectUtil.FindChildGameObjectComponent<UILabel>(parent, "Lbl_bet_number");
			m_AttackRateText = GameObjectUtil.FindChildGameObjectComponent<UILabel>(parent, "Lbl_strike_power");
			m_playBetObject0 = GameObjectUtil.FindChildGameObject(parent, "bet_0");
			if (m_playBetObject0 != null)
			{
				m_playBetObject0.SetActive(true);
			}
			m_playBetObject1 = GameObjectUtil.FindChildGameObject(parent, "bet_1");
			if (m_playBetObject1 != null)
			{
				m_playBetObject1.SetActive(false);
			}
			m_playBetMaxSign = GameObjectUtil.FindChildGameObject(parent, "max_sign");
			if (m_playBetMaxSign != null)
			{
				m_playBetMaxSign.SetActive(false);
			}
			UpdateRaidbossChallangeCountView();
			if (m_currentRaidBossData == null)
			{
				return;
			}
			UILabel uILabel = GameObjectUtil.FindChildGameObjectComponent<UILabel>(parent, "Lbl_boss_name");
			if (uILabel != null)
			{
				uILabel.text = m_currentRaidBossData.name;
			}
			UILabel uILabel2 = GameObjectUtil.FindChildGameObjectComponent<UILabel>(parent, "Lbl_boss_name_sh");
			if (uILabel2 != null)
			{
				uILabel2.text = m_currentRaidBossData.name;
			}
			UILabel uILabel3 = GameObjectUtil.FindChildGameObjectComponent<UILabel>(parent, "Lbl_boss_lv");
			if (uILabel3 != null)
			{
				uILabel3.text = "Lv." + m_currentRaidBossData.lv;
			}
			GameObject gameObject9 = GameObjectUtil.FindChildGameObject(parent, "img_boss_icon");
			if (gameObject9 != null)
			{
				UISprite component2 = gameObject9.GetComponent<UISprite>();
				if (component2 != null)
				{
					component2.spriteName = "ui_gp_gauge_boss_icon_raid_" + m_currentRaidBossData.rarity;
				}
				UISprite uISprite5 = GameObjectUtil.FindChildGameObjectComponent<UISprite>(gameObject9, "img_boss_icon_bg");
				if (uISprite5 != null)
				{
					uISprite5.spriteName = "ui_event_raidboss_window_bosslevel_" + m_currentRaidBossData.rarity;
				}
			}
			UILabel uILabel4 = GameObjectUtil.FindChildGameObjectComponent<UILabel>(parent, "Lbl_boss_life");
			if (uILabel4 != null)
			{
				uILabel4.text = m_currentRaidBossData.hp + "/" + m_currentRaidBossData.hpMax;
			}
			m_bossTime = GameObjectUtil.FindChildGameObjectComponent<UILabel>(parent, "Lbl_boss_time");
			if (m_bossTime != null)
			{
				m_bossTime.text = m_currentRaidBossData.GetTimeLimitString(true);
			}
			m_bossLifeBar = GameObjectUtil.FindChildGameObjectComponent<UISlider>(parent, "Pgb_BossLife");
			if (m_bossLifeBar != null)
			{
				m_bossLifeBar.value = m_currentRaidBossData.GetHpRate();
				m_bossLifeBar.numberOfSteps = 1;
				m_bossLifeBar.ForceUpdate();
			}
			UISprite uISprite6 = GameObjectUtil.FindChildGameObjectComponent<UISprite>(parent, "img_boss_bg");
			if (uISprite6 != null)
			{
				if (m_currentRaidBossData.IsDiscoverer())
				{
					uISprite6.spriteName = "ui_event_raidboss_window_boss_bar_1";
				}
				else
				{
					uISprite6.spriteName = "ui_event_raidboss_window_boss_bar_0";
				}
			}
			return;
		}
		GameObject gameObject10 = GameObjectUtil.FindChildGameObject(gameObject4, "pattern_0");
		if (!(gameObject10 != null))
		{
			return;
		}
		string name6 = "img_word_play";
		GameObject gameObject11 = GameObjectUtil.FindChildGameObject(gameObject10, name6);
		if (!(gameObject11 != null))
		{
			return;
		}
		UISprite component3 = gameObject11.GetComponent<UISprite>();
		if (component3 != null)
		{
			if (MileageMapUtility.IsBossStage() && !flag)
			{
				component3.spriteName = "ui_mm_btn_word_play_boss";
			}
			else
			{
				component3.spriteName = "ui_mm_btn_word_play";
			}
		}
	}

	private void UpdateRaidbossChallangeCountView()
	{
		if (m_isRaidMenu)
		{
			m_raidChargeCountMax = (uint)EventManager.Instance.RaidbossChallengeCount;
			if (m_ChallengeCountLabel != null)
			{
				m_ChallengeCountLabel.text = "/" + m_raidChargeCountMax;
			}
			if (m_raidChargeCountMax > 3)
			{
				m_raidChargeCountMax = 3u;
			}
			if (m_ChargeText != null)
			{
				m_ChargeText.text = "x" + m_raidChargeCount;
			}
		}
	}

	private IEnumerator OnStart()
	{
		if (ServerInterface.FreeItemState != null)
		{
			ServerInterface.FreeItemState.SetExpiredFlag(true);
		}
		yield return null;
		GameObject anchor9 = GameObjectUtil.FindChildGameObject(base.gameObject, "Anchor_9_BR");
		if (anchor9 != null)
		{
			for (int index = 0; index < 4; index++)
			{
				string buttonParentName = "pattern_" + index;
				GameObject buttonParentObject = GameObjectUtil.FindChildGameObject(anchor9, buttonParentName);
				if (buttonParentObject == null)
				{
					continue;
				}
				GameObject ui_button = GameObjectUtil.FindChildGameObject(buttonParentObject, "Btn_play");
				if (ui_button == null)
				{
					continue;
				}
				UIButtonMessage button_msg = ui_button.GetComponent<UIButtonMessage>();
				if (button_msg == null)
				{
					ui_button.AddComponent<UIButtonMessage>();
					button_msg = ui_button.GetComponent<UIButtonMessage>();
				}
				if (button_msg != null)
				{
					button_msg.enabled = true;
					button_msg.trigger = UIButtonMessage.Trigger.OnClick;
					button_msg.target = base.gameObject;
					button_msg.functionName = "OnPlayButtonClicked";
				}
				if (index == 3)
				{
					GameObject ui_bet_button = GameObjectUtil.FindChildGameObject(buttonParentObject, "Btn_bet");
					if (ui_bet_button != null)
					{
						UIButtonMessage button_bet_msg = ui_bet_button.GetComponent<UIButtonMessage>();
						if (button_bet_msg == null)
						{
							ui_bet_button.AddComponent<UIButtonMessage>();
							button_bet_msg = ui_bet_button.GetComponent<UIButtonMessage>();
						}
						if (button_bet_msg != null)
						{
							button_bet_msg.enabled = true;
							button_bet_msg.trigger = UIButtonMessage.Trigger.OnClick;
							button_bet_msg.target = base.gameObject;
							button_bet_msg.functionName = "OnBetButtonClicked";
						}
					}
				}
				UIPlayAnimation[] anims = ui_button.GetComponents<UIPlayAnimation>();
				if (anims == null)
				{
					continue;
				}
				UIPlayAnimation[] array = anims;
				foreach (UIPlayAnimation anim in array)
				{
					if (!(anim == null))
					{
						anim.target = null;
					}
				}
			}
		}
		m_itemSet = GameObjectUtil.FindChildGameObjectComponent<ItemSet>(base.gameObject, "item_set_contents");
		m_instantItemSet = GameObjectUtil.FindChildGameObjectComponent<InstantItemSet>(base.gameObject, "item_boost");
		m_ringManagement = base.gameObject.GetComponent<ItemSetRingManagement>();
		m_hideGameObjects.Add(GameObjectUtil.FindChildGameObject(base.gameObject, "item_boost"));
		m_hideGameObjects.Add(GameObjectUtil.FindChildGameObject(base.gameObject, "item_set_contents_maintenance"));
		m_hideGameObjects.Add(GameObjectUtil.FindChildGameObject(base.gameObject, "item_info"));
		yield return null;
		foreach (GameObject o in m_hideGameObjects)
		{
			if (!(o == null))
			{
				o.SetActive(false);
			}
		}
		yield return null;
	}

	private void Update()
	{
		switch (m_tutorialMode)
		{
		case TutorialMode.AppolloStartWait:
			if (m_sendApollo != null && m_sendApollo.IsEnd())
			{
				Object.Destroy(m_sendApollo.gameObject);
				m_sendApollo = null;
				CreateTutorialWindow(0);
				m_tutorialMode = TutorialMode.Window1;
			}
			break;
		case TutorialMode.Window1:
			if (GeneralWindow.IsCreated("ItemTutorial") && GeneralWindow.IsButtonPressed)
			{
				HudMenuUtility.SetConnectAlertSimpleUI(false);
				GeneralWindow.Close();
				TutorialCursor.StartTutorialCursor(TutorialCursor.Type.ITEMSELECT_LASER);
				m_tutorialMode = TutorialMode.Laser;
			}
			break;
		case TutorialMode.Laser:
			if (IsEndTutorialLaser())
			{
				TutorialCursor.StartTutorialCursor(TutorialCursor.Type.MAINMENU_PLAY);
				SetEnablePlayButton(true);
				m_tutorialMode = TutorialMode.Play;
			}
			break;
		case TutorialMode.AppolloEndWait:
			if (m_sendApollo != null && m_sendApollo.IsEnd())
			{
				Object.Destroy(m_sendApollo.gameObject);
				m_sendApollo = null;
				m_tutorialMode = TutorialMode.Window2;
				CreateTutorialWindow(1);
			}
			break;
		case TutorialMode.Window2:
			if (GeneralWindow.IsCreated("ItemTutorial2") && GeneralWindow.IsButtonPressed)
			{
				HudMenuUtility.SendMenuButtonClicked(ButtonInfoTable.ButtonType.PLAY_ITEM);
				GeneralWindow.Close();
				m_tutorialMode = TutorialMode.Idle;
			}
			break;
		case TutorialMode.Window3:
			if (GeneralWindow.IsCreated("ItemTutorial3") && GeneralWindow.IsButtonPressed)
			{
				HudMenuUtility.SetConnectAlertSimpleUI(false);
				GeneralWindow.Close();
				TutorialCursor.StartTutorialCursor(TutorialCursor.Type.ITEMSELECT_SUBCHARA);
				m_timer = 3f;
				m_tutorialMode = TutorialMode.SubChara;
			}
			break;
		case TutorialMode.SubChara:
			m_timer -= Time.deltaTime;
			if (TutorialCursor.IsTouchScreen() || m_timer < 0f)
			{
				TutorialCursor.DestroyTutorialCursor();
				HudMenuUtility.SaveSystemDataFlagStatus(SystemData.FlagStatus.SUB_CHARA_ITEM_EXPLAINED);
				SetEnablePlayButton(true);
				m_tutorialMode = TutorialMode.Idle;
			}
			break;
		}
		if (m_isRaidMenu)
		{
			if (!m_isRaidbossTimeOver)
			{
				m_timeCounter -= m_targetFrameTime;
				if (m_timeCounter <= 0f)
				{
					if (m_bossTime != null && m_currentRaidBossData != null)
					{
						m_bossTime.text = m_currentRaidBossData.GetTimeLimitString(true);
					}
					m_timeCounter = 0.25f;
				}
			}
			else if (GeneralWindow.IsCreated("RaidbossTimeOver") && GeneralWindow.IsButtonPressed)
			{
				HudMenuUtility.SendMenuButtonClicked(ButtonInfoTable.ButtonType.ITEM_BACK);
				GeneralWindow.Close();
			}
		}
		if (GeneralWindow.IsCreated("RingMissing"))
		{
			if (GeneralWindow.IsYesButtonPressed)
			{
				HudMenuUtility.SendMenuButtonClicked(ButtonInfoTable.ButtonType.RING_TO_SHOP);
				OnMsgMenuBack();
				GeneralWindow.Close();
			}
			else if (GeneralWindow.IsNoButtonPressed)
			{
				GeneralWindow.Close();
			}
		}
	}

	private void ServerGetRingExchangeList_Succeeded(MsgGetRingExchangeListSucceed msg)
	{
		Setup();
	}

	private void GetFreeItemList()
	{
		ServerInterface loggedInServerInterface = ServerInterface.LoggedInServerInterface;
		m_isUpdateFreeItems = false;
		if (loggedInServerInterface != null)
		{
			ServerFreeItemState freeItemState = ServerInterface.FreeItemState;
			if (freeItemState.IsExpired())
			{
				loggedInServerInterface.RequestServerGetFreeItemList(base.gameObject);
				m_isUpdateFreeItems = true;
			}
		}
	}

	private void ServerGetFreeItemList_Succeeded(MsgGetFreeItemListSucceed msg)
	{
		if (m_instantItemSet != null)
		{
			m_instantItemSet.UpdateFreeItemList(msg.m_freeItemState);
			m_instantItemSet.SetupBoostedItem();
		}
		if (m_itemSet != null)
		{
			m_itemSet.UpdateFreeItemList(msg.m_freeItemState);
			m_itemSet.SetupEquipItem();
		}
	}

	private void OnMsgMenuItemSetStart(MsgMenuItemSetStart msg)
	{
		ServerGetRingExchangeList_Succeeded(null);
		switch (msg.m_setMode)
		{
		case MsgMenuItemSetStart.SetMode.NORMAL:
			m_tutorialMode = TutorialMode.Idle;
			CheckFreeItemAndEquip();
			break;
		case MsgMenuItemSetStart.SetMode.TUTORIAL:
		{
			BackKeyManager.TutorialFlag = true;
			HudMenuUtility.SetConnectAlertSimpleUI(true);
			SetEnablePlayButton(false);
			ClearEquipedItemForTutorial();
			string[] value = new string[1];
			SendApollo.GetTutorialValue(ApolloTutorialIndex.START_STEP4, ref value);
			m_sendApollo = SendApollo.CreateRequest(ApolloType.TUTORIAL_START, value);
			m_tutorialMode = TutorialMode.AppolloStartWait;
			break;
		}
		case MsgMenuItemSetStart.SetMode.TUTORIAL_SUBCHARA:
		{
			uint num = 0u;
			if (SaveDataManager.Instance != null)
			{
				num = SaveDataManager.Instance.PlayerData.ChallengeCount;
			}
			if (num == 0)
			{
				m_tutorialMode = TutorialMode.Idle;
			}
			else
			{
				HudMenuUtility.SetConnectAlertSimpleUI(true);
				SetEnablePlayButton(false);
				m_tutorialMode = TutorialMode.Window3;
				CreateTutorialWindow(2);
			}
			CheckFreeItemAndEquip();
			break;
		}
		}
		m_msg = msg;
		if (m_isRaidMenu)
		{
			UpdateRaidbossChallangeCountView();
		}
		OnEnable();
	}

	private void CheckFreeItemAndEquip()
	{
		GetFreeItemList();
		if (m_isUpdateFreeItems)
		{
			return;
		}
		ServerFreeItemState freeItemState = ServerInterface.FreeItemState;
		if (freeItemState != null)
		{
			if (m_instantItemSet != null)
			{
				m_instantItemSet.UpdateFreeItemList(freeItemState);
			}
			if (m_itemSet != null)
			{
				m_itemSet.UpdateFreeItemList(freeItemState);
			}
		}
		if (m_instantItemSet != null)
		{
			m_instantItemSet.SetupBoostedItem();
		}
		if (m_itemSet != null)
		{
			m_itemSet.SetupEquipItem();
		}
	}

	private void ClearEquipedItemForTutorial()
	{
		SaveDataManager instance = SaveDataManager.Instance;
		if (!(instance != null))
		{
			return;
		}
		PlayerData playerData = instance.PlayerData;
		if (playerData != null)
		{
			playerData.BoostedItem[0] = false;
			playerData.BoostedItem[2] = false;
			playerData.BoostedItem[1] = false;
			playerData.EquippedItem[0] = ItemType.UNKNOWN;
			playerData.EquippedItem[1] = ItemType.UNKNOWN;
			playerData.EquippedItem[2] = ItemType.UNKNOWN;
			if (m_instantItemSet != null)
			{
				m_instantItemSet.SetupBoostedItem();
			}
			if (m_itemSet != null)
			{
				m_itemSet.SetupEquipItem();
			}
		}
	}

	private void OnMsgMenuBack()
	{
		m_instantItemSet.ResetCheckMark();
		m_itemSet.ResetCheckMark();
		if (m_UpdateCharaSettingFlag)
		{
			HudMenuUtility.SendMsgUpdateSaveDataDisplay();
			m_UpdateCharaSettingFlag = false;
		}
	}

	private void OnPlayButtonClicked(GameObject obj)
	{
		SoundManager.SePlay("sys_menu_decide");
		if (m_msg != null && m_msg.m_setMode == MsgMenuItemSetStart.SetMode.TUTORIAL)
		{
			TutorialCursor.EndTutorialCursor(TutorialCursor.Type.MAINMENU_PLAY);
			m_tutorialMode = TutorialMode.AppolloEndWait;
			string[] value = new string[1];
			SendApollo.GetTutorialValue(ApolloTutorialIndex.START_STEP4, ref value);
			m_sendApollo = SendApollo.CreateRequest(ApolloType.TUTORIAL_END, value);
		}
		else
		{
			if (!(m_ringManagement != null))
			{
				return;
			}
			if (m_ringManagement.GetDisplayRingCount() >= 0)
			{
				if (m_isRaidMenu)
				{
					if (m_currentRaidBossData.IsLimit())
					{
						GeneralWindow.CInfo info = default(GeneralWindow.CInfo);
						info.name = "RaidbossTimeOver";
						info.buttonType = GeneralWindow.ButtonType.Ok;
						info.caption = TextUtility.GetText(TextManager.TextType.TEXTTYPE_EVENT_COMMON_TEXT, "Menu", "ui_Lbl_word_raid_boss_bye");
						info.message = TextUtility.GetText(TextManager.TextType.TEXTTYPE_EVENT_COMMON_TEXT, "Menu", "ui_Lbl_word_raid_boss_bye2");
						info.anchor_path = "Camera/menu_Anim/ItemSet_3_UI/Anchor_5_MC";
						GeneralWindow.Create(info);
						m_isRaidbossTimeOver = true;
					}
					else
					{
						uint raidbossChallengeCount = (uint)EventManager.Instance.RaidbossChallengeCount;
						if (m_raidChargeCount <= raidbossChallengeCount)
						{
							EventManager.Instance.UseRaidbossChallengeCount = (int)m_raidChargeCount;
							HudMenuUtility.SendMenuButtonClicked(ButtonInfoTable.ButtonType.PLAY_ITEM);
						}
					}
				}
				else
				{
					HudMenuUtility.SendMenuButtonClicked(ButtonInfoTable.ButtonType.PLAY_ITEM);
				}
			}
			else
			{
				GeneralWindow.CInfo info2 = default(GeneralWindow.CInfo);
				info2.name = "RingMissing";
				info2.caption = TextManager.GetText(TextManager.TextType.TEXTTYPE_COMMON_TEXT, "ItemRoulette", "gw_cost_caption").text;
				info2.message = TextManager.GetText(TextManager.TextType.TEXTTYPE_COMMON_TEXT, "ItemRoulette", "gw_cost_text").text;
				info2.anchor_path = "Camera/menu_Anim/ItemSet_3_UI/Anchor_5_MC";
				info2.buttonType = GeneralWindow.ButtonType.ShopCancel;
				GeneralWindow.Create(info2);
			}
		}
	}

	private void OnBetButtonClicked(GameObject obj)
	{
		SoundManager.SePlay("sys_menu_decide");
		uint raidChargeCount = m_raidChargeCount;
		if (m_raidChargeCountMax <= 1)
		{
			m_raidChargeCount = 1u;
		}
		else if (m_raidChargeCount < m_raidChargeCountMax)
		{
			m_raidChargeCount++;
		}
		else
		{
			m_raidChargeCount = 1u;
		}
		if (raidChargeCount != m_raidChargeCount)
		{
			m_ChargeText.text = "x" + m_raidChargeCount;
			if (m_raidChargeCount == 3)
			{
				m_playBetObject0.SetActive(false);
				m_playBetObject1.SetActive(true);
				m_playBetMaxSign.SetActive(true);
				SetRaidAttackRate();
			}
			else if (m_raidChargeCount > 1)
			{
				m_playBetObject0.SetActive(false);
				m_playBetObject1.SetActive(true);
				m_playBetMaxSign.SetActive(false);
				SetRaidAttackRate();
			}
			else
			{
				m_playBetObject0.SetActive(true);
				m_playBetObject1.SetActive(false);
				m_playBetMaxSign.SetActive(false);
			}
		}
	}

	private void SetRaidAttackRate()
	{
		if (m_raidChargeCount != 0)
		{
			float raidAttackRate = EventManager.Instance.GetRaidAttackRate((int)m_raidChargeCount);
			if (m_AttackRateText != null)
			{
				m_AttackRateText.text = "x" + raidAttackRate;
			}
		}
	}

	private void CreateTutorialWindow(int index)
	{
		string str = string.Empty;
		if (index > 0)
		{
			str = (index + 1).ToString();
		}
		GeneralWindow.CInfo info = default(GeneralWindow.CInfo);
		info.name = "ItemTutorial" + str;
		info.caption = TextManager.GetText(TextManager.TextType.TEXTTYPE_COMMON_TEXT, "ShopItem", "tutorial_comment_caption" + str).text;
		info.message = TextManager.GetText(TextManager.TextType.TEXTTYPE_COMMON_TEXT, "ShopItem", "tutorial_comment_text" + str).text;
		info.anchor_path = "Camera/menu_Anim/ItemSet_3_UI/Anchor_5_MC";
		info.buttonType = GeneralWindow.ButtonType.Ok;
		GeneralWindow.Create(info);
	}

	private void Setup()
	{
		foreach (GameObject hideGameObject in m_hideGameObjects)
		{
			if (!(hideGameObject == null))
			{
				hideGameObject.SetActive(true);
			}
		}
		if (m_instantItemSet != null)
		{
			m_instantItemSet.Setup();
		}
		if (m_itemSet != null)
		{
			m_itemSet.Setup();
		}
		m_UpdateCharaSettingFlag = false;
		m_isEndSetup = true;
	}

	private bool IsEndTutorialLaser()
	{
		if (m_itemSet != null)
		{
			ItemType[] item = m_itemSet.GetItem();
			if (item != null)
			{
				ItemType[] array = item;
				foreach (ItemType itemType in array)
				{
					if (itemType == ItemType.LASER)
					{
						return true;
					}
				}
			}
		}
		return false;
	}

	private void CharaSetButtonClickedCallback()
	{
		SoundManager.SePlay("sys_menu_decide");
		DeckViewWindow.Create(base.gameObject);
	}

	private void OnMsgReset()
	{
		if (StageAbilityManager.Instance != null)
		{
			StageAbilityManager.Instance.RecalcAbilityVaue();
			m_UpdateCharaSettingFlag = true;
		}
		if (m_itemSet != null)
		{
			m_itemSet.UpdateView();
		}
	}

	private void SetEnablePlayButton(bool enabledFlag)
	{
		GameObject gameObject = GameObjectUtil.FindChildGameObject(base.gameObject, "Anchor_7_BL");
		if (gameObject != null)
		{
			GameObject gameObject2 = GameObjectUtil.FindChildGameObject(gameObject, "Btn_cmn_back");
			if (gameObject2 != null)
			{
				BoxCollider component = gameObject2.GetComponent<BoxCollider>();
				if (component != null)
				{
					component.isTrigger = !enabledFlag;
				}
			}
			GameObject gameObject3 = GameObjectUtil.FindChildGameObject(gameObject, "Btn_charaset");
			if (gameObject3 != null)
			{
				BoxCollider component2 = gameObject3.GetComponent<BoxCollider>();
				if (component2 != null)
				{
					component2.isTrigger = !enabledFlag;
				}
			}
		}
		GameObject gameObject4 = GameObjectUtil.FindChildGameObject(base.gameObject, "Anchor_9_BR");
		if (!(gameObject4 != null))
		{
			return;
		}
		GameObject x = GameObjectUtil.FindChildGameObject(gameObject4, "pattern_0");
		if (!(x != null))
		{
			return;
		}
		GameObject gameObject5 = GameObjectUtil.FindChildGameObject(base.gameObject, "Btn_play");
		if (gameObject5 != null)
		{
			BoxCollider component3 = gameObject5.GetComponent<BoxCollider>();
			if (component3 != null)
			{
				component3.isTrigger = !enabledFlag;
			}
		}
	}

	public void UpdateBoostButton()
	{
		if (m_instantItemSet != null)
		{
			m_instantItemSet.ResetCheckMark();
			m_instantItemSet.Setup();
			m_instantItemSet.SetupBoostedItem();
		}
	}

	public static void UpdateBoostItemForCharacterDeck()
	{
		GameObject cameraUIObject = HudMenuUtility.GetCameraUIObject();
		if (cameraUIObject != null)
		{
			ItemSetMenu itemSetMenu = GameObjectUtil.FindChildGameObjectComponent<ItemSetMenu>(cameraUIObject, "ItemSet_3_UI");
			if (itemSetMenu != null)
			{
				itemSetMenu.UpdateBoostButton();
			}
		}
	}
}
