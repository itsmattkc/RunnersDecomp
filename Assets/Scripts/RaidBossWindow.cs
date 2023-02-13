using AnimationOrTween;
using Message;
using SaveData;
using System;
using System.Collections.Generic;
using Text;
using UnityEngine;

public class RaidBossWindow : EventWindowBase
{
	private enum BUTTON_ACT
	{
		CLOSE,
		BOSS_PLAY,
		BOSS_INFO,
		BOSS_REWARD,
		INFO,
		ROULETTE,
		CHALLENGE,
		SHOP_RSRING,
		SHOP_RING,
		SHOP_CHALLENGE,
		NONE
	}

	public enum WINDOW_OPEN_MODE
	{
		NONE,
		ADVENT_BOSS_NORMAL,
		ADVENT_BOSS_RARE,
		ADVENT_BOSS_S_RARE
	}

	public const float RELOAD_TIME = 5f;

	public const float WAIT_LIMIT_TIME = 300f;

	[SerializeField]
	private UIPanel mainPanel;

	[SerializeField]
	private Animation m_animation;

	[SerializeField]
	private UIDraggablePanel m_listPanel;

	[SerializeField]
	private UILabel m_crushCount;

	[SerializeField]
	private UILabel m_raidRingCount;

	[SerializeField]
	private RaidEnergyStorage m_energy;

	[SerializeField]
	private GameObject m_advent;

	[SerializeField]
	private UITexture m_bgTexture;

	[SerializeField]
	private GameObject eventEndObject;

	private RaidBossInfo m_rbInfoData;

	private bool m_isLoading;

	private float m_time;

	private bool m_opened;

	private bool m_close;

	private bool m_alertCollider;

	private BUTTON_ACT m_btnAct = BUTTON_ACT.NONE;

	private UIRectItemStorage m_storage;

	private UIButton[] m_btnReload;

	private bool m_isBossAttention;

	private bool m_isMyBoss;

	private WINDOW_OPEN_MODE m_openMode;

	private Animation m_bossAnim;

	private static RaidBossWindow s_instance;

	public bool isLoading
	{
		get
		{
			return m_isLoading;
		}
	}

	public float time
	{
		get
		{
			return m_time;
		}
	}

	private static RaidBossWindow Instance
	{
		get
		{
			return s_instance;
		}
	}

	private void Update()
	{
		m_time += Time.deltaTime;
		if (m_time >= float.MaxValue)
		{
			m_time = 1000f;
		}
		CheckReloadBtn(m_time);
		if (GeneralWindow.IsCreated("RaidbossChallengeMissing"))
		{
			if (GeneralWindow.IsYesButtonPressed)
			{
				GeneralWindow.Close();
				OnClickEvChallenge();
			}
			else if (GeneralWindow.IsNoButtonPressed)
			{
				GeneralWindow.Close();
			}
		}
	}

	private void CheckReloadBtn(float time)
	{
		if (time <= 5f && m_btnReload != null && m_btnReload.Length > 0)
		{
			UIButton[] btnReload = m_btnReload;
			foreach (UIButton uIButton in btnReload)
			{
				uIButton.isEnabled = false;
			}
		}
		else if (time > 5f && m_btnReload != null && m_btnReload.Length > 0)
		{
			UIButton[] btnReload2 = m_btnReload;
			foreach (UIButton uIButton2 in btnReload2)
			{
				uIButton2.isEnabled = true;
			}
		}
	}

	private void UpdateList()
	{
		m_time = 0f;
		m_isMyBoss = false;
		if (m_energy != null)
		{
			m_energy.Init();
		}
		GameObject gameObject = GameObjectUtil.FindChildGameObject(m_energy.gameObject, "img_sale_icon_challenge");
		if (gameObject != null)
		{
			bool flag = true;
			if (EventManager.Instance != null)
			{
				flag = EventManager.Instance.IsChallengeEvent();
			}
			if (HudMenuUtility.IsSale(Constants.Campaign.emType.PurchaseAddRaidEnergys) && flag)
			{
				gameObject.SetActive(true);
			}
			else
			{
				gameObject.SetActive(false);
			}
		}
		m_rbInfoData = null;
		if (EventManager.Instance != null)
		{
			m_rbInfoData = EventManager.Instance.RaidBossInfo;
		}
		if (m_rbInfoData == null)
		{
			return;
		}
		if (m_crushCount != null)
		{
			m_crushCount.text = HudUtility.GetFormatNumString(m_rbInfoData.totalDestroyCount);
		}
		if (m_raidRingCount != null)
		{
			m_raidRingCount.text = HudUtility.GetFormatNumString(GeneralUtil.GetItemCount(ServerItem.Id.RAIDRING));
		}
		if (!(m_listPanel != null))
		{
			return;
		}
		List<RaidBossData> raidData = m_rbInfoData.raidData;
		List<RaidBossData> list = new List<RaidBossData>();
		List<RaidBossData> list2 = new List<RaidBossData>();
		List<RaidBossData> list3 = new List<RaidBossData>();
		List<RaidBossData> list4 = new List<RaidBossData>();
		List<RaidBossData> list5 = new List<RaidBossData>();
		if (raidData != null)
		{
			for (int i = 0; i < raidData.Count; i++)
			{
				if (raidData[i] == null)
				{
					continue;
				}
				if (raidData[i].end && !raidData[i].IsDiscoverer())
				{
					if (!raidData[i].participation)
					{
						list5.Add(raidData[i]);
					}
					else if (raidData[i].clear)
					{
						list3.Add(raidData[i]);
					}
					else
					{
						list4.Add(raidData[i]);
					}
				}
				else if (raidData[i].IsDiscoverer())
				{
					list.Add(raidData[i]);
					m_isMyBoss = true;
				}
				else
				{
					list2.Add(raidData[i]);
				}
			}
		}
		if (list2.Count > 0)
		{
			foreach (RaidBossData item in list2)
			{
				list.Add(item);
			}
		}
		if (list3.Count > 0)
		{
			foreach (RaidBossData item2 in list3)
			{
				list.Add(item2);
			}
		}
		if (list4.Count > 0)
		{
			foreach (RaidBossData item3 in list4)
			{
				list.Add(item3);
			}
		}
		if (list5.Count > 0)
		{
			ServerInterface loggedInServerInterface = ServerInterface.LoggedInServerInterface;
			int id = EventManager.Instance.Id;
			if (loggedInServerInterface != null)
			{
				foreach (RaidBossData item4 in list5)
				{
					loggedInServerInterface.RequestServerGetEventRaidBossUserList(id, item4.id, base.gameObject);
				}
			}
		}
		m_storage = GameObjectUtil.FindChildGameObjectComponent<UIRectItemStorage>(m_listPanel.gameObject, "slot");
		if (m_storage != null)
		{
			m_storage.maxRows = list.Count;
			m_storage.Restart();
			if (list.Count > 0)
			{
				List<ui_event_raid_scroll> list6 = GameObjectUtil.FindChildGameObjectsComponents<ui_event_raid_scroll>(m_listPanel.gameObject, "ui_event_raid_scroll(Clone)");
				if (list6 != null && list6.Count > 0)
				{
					for (int j = 0; j < list6.Count && j < list.Count; j++)
					{
						list6[j].UpdateView(list[j], this, true);
					}
				}
				m_listPanel.Scroll(1f);
				m_listPanel.ResetPosition();
			}
		}
		if (!(eventEndObject != null))
		{
			return;
		}
		GameObject gameObject2 = GameObjectUtil.FindChildGameObject(base.gameObject, "Btn_charge_challenge");
		GameObject gameObject3 = GameObjectUtil.FindChildGameObject(base.gameObject, "Btn_roulette");
		if (!EventManager.Instance.IsChallengeEvent() && list.Count <= 0)
		{
			eventEndObject.SetActive(true);
			UIButtonMessage componentInChildren = eventEndObject.GetComponentInChildren<UIButtonMessage>();
			if (componentInChildren != null)
			{
				componentInChildren.target = base.gameObject;
				componentInChildren.functionName = "OnClickRouletteRaid";
			}
			UILabel uILabel = GameObjectUtil.FindChildGameObjectComponent<UILabel>(eventEndObject, "Lbl_expdate");
			if (uILabel != null)
			{
				DateTime eventCloseTime = EventManager.Instance.EventCloseTime;
				string text = TextManager.GetText(TextManager.TextType.TEXTTYPE_EVENT_COMMON_TEXT, "Common", "event_finished_guidance_2").text;
				if (!string.IsNullOrEmpty(text))
				{
					uILabel.text = text.Replace("{DATE}", eventCloseTime.ToString());
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
		}
		else
		{
			eventEndObject.SetActive(false);
			if (gameObject2 != null)
			{
				gameObject2.SetActive(true);
			}
			if (gameObject3 != null)
			{
				gameObject3.SetActive(true);
			}
		}
	}

	public void Setup(RaidBossInfo info, WINDOW_OPEN_MODE mode)
	{
		RaidBossInfo.currentRaidData = null;
		m_isLoading = false;
		m_isBossAttention = false;
		m_isMyBoss = false;
		m_opened = false;
		m_openMode = mode;
		mainPanel.alpha = 1f;
		SetObject();
		SetAlertSimpleUI(true);
		HudMenuUtility.SendEnableShopButton(true);
		if (m_animation != null)
		{
			ActiveAnimation activeAnimation = ActiveAnimation.Play(m_animation, "ui_eventmenu_intro", Direction.Forward);
			EventDelegate.Add(activeAnimation.onFinished, WindowAnimationFinishCallback, true);
			SoundManager.SePlay("sys_window_open");
		}
		Texture bGTexture = EventUtility.GetBGTexture();
		if (bGTexture != null && m_bgTexture != null)
		{
			m_bgTexture.mainTexture = bGTexture;
		}
		base.enabledAnchorObjects = true;
		info.callback = CallbackInfoUpdate;
		UpdateList();
		m_btnAct = BUTTON_ACT.NONE;
		m_close = false;
		BackKeyManager.AddWindowCallBack(base.gameObject);
	}

	protected override void SetObject()
	{
		GameObject gameObject = GameObjectUtil.FindChildGameObject(base.gameObject, "Btn_reload");
		if (gameObject != null)
		{
			m_btnReload = gameObject.GetComponents<UIButton>();
		}
		ShowBossAdvent(m_openMode != WINDOW_OPEN_MODE.NONE);
		GeneralUtil.SetRouletteBtnIcon(base.gameObject);
	}

	private void ShowBossAdvent(bool adventActive)
	{
		if (m_advent != null)
		{
			m_advent.SetActive(adventActive);
			if (adventActive)
			{
				RaidBossInfo raidBossInfo = EventManager.Instance.RaidBossInfo;
				RaidBossData raidBossData = null;
				if (raidBossInfo != null)
				{
					List<RaidBossData> raidData = raidBossInfo.raidData;
					if (raidData != null && raidData.Count > 0)
					{
						foreach (RaidBossData item in raidData)
						{
							if (item.IsDiscoverer())
							{
								raidBossData = item;
								break;
							}
						}
					}
				}
				m_bossAnim = GameObjectUtil.FindChildGameObjectComponent<Animation>(m_advent.gameObject, "bit_Anim");
				UILabel uILabel = GameObjectUtil.FindChildGameObjectComponent<UILabel>(m_advent.gameObject, "Lbl_boss_level");
				List<UISprite> list = null;
				for (int i = 0; i < 5; i++)
				{
					UISprite uISprite = GameObjectUtil.FindChildGameObjectComponent<UISprite>(m_advent.gameObject, "boss_icon" + i);
					if (uISprite != null)
					{
						if (list == null)
						{
							list = new List<UISprite>();
						}
						list.Add(uISprite);
						continue;
					}
					break;
				}
				if (raidBossData != null)
				{
					if (uILabel != null)
					{
						string text = TextManager.GetText(TextManager.TextType.TEXTTYPE_COMMON_TEXT, "MainMenu", "ui_LevelNumber").text;
						uILabel.text = text.Replace("{PARAM}", raidBossData.lv.ToString());
					}
					if (list != null)
					{
						foreach (UISprite item2 in list)
						{
							item2.spriteName = "ui_event_raidboss_icon_silhouette_" + raidBossData.rarity;
							item2.color = new Color(1f, 1f, 1f, item2.alpha);
						}
					}
				}
				else
				{
					m_advent.SetActive(false);
				}
			}
		}
		if (m_openMode == WINDOW_OPEN_MODE.NONE && adventActive)
		{
			StartRaidbossAttentionAnim();
		}
	}

	private void StartRaidbossAttentionAnim()
	{
		ActiveAnimation activeAnimation = ActiveAnimation.Play(m_bossAnim, "ui_EventResult_raidboss_attention_intro_Anim", Direction.Forward);
		EventDelegate.Add(activeAnimation.onFinished, BossAnimationIntroCallback, true);
		SoundManager.SePlay("sys_boss_warning");
	}

	public void OnClickEvChallenge()
	{
		m_btnAct = BUTTON_ACT.CHALLENGE;
		m_close = true;
		SoundManager.SePlay("sys_window_close");
		GameObject gameObject = GameObjectUtil.FindChildGameObject(base.gameObject, "Btn_cmn_back");
		UIPlayAnimation component = gameObject.GetComponent<UIPlayAnimation>();
		if (component != null)
		{
			EventDelegate.Add(component.onFinished, WindowAnimationFinishCallback, true);
			component.Play(true);
		}
		HudMenuUtility.SendMenuButtonClicked(ButtonInfoTable.ButtonType.RAIDENERGY_TO_SHOP);
	}

	public void OnClickNo()
	{
		m_btnAct = BUTTON_ACT.CLOSE;
		m_close = true;
		SoundManager.SePlay("sys_window_close");
		GameObject gameObject = GameObjectUtil.FindChildGameObject(base.gameObject, "Btn_cmn_back");
		UIPlayAnimation component = gameObject.GetComponent<UIPlayAnimation>();
		if (component != null)
		{
			EventDelegate.Add(component.onFinished, WindowAnimationFinishCallback, true);
			component.Play(true);
		}
	}

	public void OnClickNoBg()
	{
	}

	public void OnClickReload()
	{
		if (GeneralUtil.IsNetwork())
		{
			if (m_time > 5f)
			{
				RaidBossInfo.currentRaidData = null;
				RequestServerGetEventUserRaidBossList();
				SoundManager.SePlay("sys_menu_decide");
			}
		}
		else
		{
			ShowNoCommunication();
		}
	}

	public void RequestServerGetEventUserRaidBossList()
	{
		ServerInterface loggedInServerInterface = ServerInterface.LoggedInServerInterface;
		if (loggedInServerInterface != null)
		{
			int eventId = 0;
			if (EventManager.Instance != null)
			{
				eventId = EventManager.Instance.Id;
			}
			m_isLoading = true;
			SetAlertSimpleUI(true);
			loggedInServerInterface.RequestServerGetEventUserRaidBossList(eventId, base.gameObject);
		}
		else
		{
			ServerGetEventUserRaidBossList_Succeeded(null);
		}
	}

	private void ServerGetEventUserRaidBossList_Succeeded(MsgGetEventUserRaidBossListSucceed msg)
	{
		m_isLoading = false;
		if (RaidBossInfo.currentRaidData == null)
		{
			SetAlertSimpleUI(false);
			listReload();
			return;
		}
		if (RaidBossInfo.currentRaidData.end)
		{
			SetAlertSimpleUI(false);
			GeneralWindow.CInfo info = default(GeneralWindow.CInfo);
			info.name = "BossEnd";
			info.buttonType = GeneralWindow.ButtonType.Ok;
			info.caption = TextUtility.GetText(TextManager.TextType.TEXTTYPE_EVENT_COMMON_TEXT, "Menu", "ui_Lbl_word_raid_boss_bye");
			info.message = TextUtility.GetText(TextManager.TextType.TEXTTYPE_EVENT_COMMON_TEXT, "Menu", "ui_Lbl_word_raid_boss_bye2");
			GeneralWindow.Create(info);
			listReload();
			return;
		}
		SetAlertSimpleUI(true);
		m_btnAct = BUTTON_ACT.BOSS_PLAY;
		m_close = true;
		SoundManager.SePlay("sys_window_close");
		GameObject gameObject = GameObjectUtil.FindChildGameObject(base.gameObject, "Btn_cmn_back");
		UIPlayAnimation component = gameObject.GetComponent<UIPlayAnimation>();
		if (component != null)
		{
			EventDelegate.Add(component.onFinished, WindowAnimationFinishCallback, true);
			component.Play(true);
		}
	}

	private void ServerGetEventUserRaidBossList_Failed(MsgServerConnctFailed msg)
	{
		m_isLoading = false;
		SetAlertSimpleUI(false);
	}

	public void listReload()
	{
		m_time = 0f;
		UpdateList();
	}

	private void CallbackInfoUpdate(RaidBossInfo info)
	{
		UpdateList();
	}

	public void OnClickBossPlayButton(RaidBossData bossData)
	{
		if (m_energy != null && m_energy.energyCount == 0)
		{
			GeneralWindow.CInfo info = default(GeneralWindow.CInfo);
			info.name = "RaidbossChallengeMissing";
			info.caption = TextManager.GetText(TextManager.TextType.TEXTTYPE_COMMON_TEXT, "MainMenu", "no_challenge_raid_count").text;
			info.message = TextManager.GetText(TextManager.TextType.TEXTTYPE_COMMON_TEXT, "MainMenu", "no_challenge_raid_count_info").text;
			info.anchor_path = "Camera/RaidBossWindowUI/Anchor_5_MC";
			info.buttonType = GeneralWindow.ButtonType.ShopCancel;
			GeneralWindow.Create(info);
			return;
		}
		SetAlertSimpleUI(true);
		RaidBossInfo.currentRaidData = bossData;
		TimeSpan timeLimit = bossData.GetTimeLimit();
		if (time >= 300f || timeLimit.Ticks <= 0)
		{
			RequestServerGetEventUserRaidBossList();
		}
		m_energy.ReflectChallengeCount();
		m_btnAct = BUTTON_ACT.BOSS_PLAY;
		m_close = true;
		SoundManager.SePlay("sys_window_close");
		GameObject gameObject = GameObjectUtil.FindChildGameObject(base.gameObject, "Btn_cmn_back");
		UIPlayAnimation component = gameObject.GetComponent<UIPlayAnimation>();
		if (component != null)
		{
			EventDelegate.Add(component.onFinished, WindowAnimationFinishCallback, true);
			component.Play(true);
		}
	}

	public void OnClickBossInfoButton(RaidBossData bossData)
	{
		RaidBossInfo.currentRaidData = bossData;
		m_btnAct = BUTTON_ACT.BOSS_INFO;
		SoundManager.SePlay("sys_menu_decide");
		RaidBossDamageRewardWindow.Create(bossData, this);
	}

	public void OnClickBossRewardButton(RaidBossData bossData)
	{
		RaidBossInfo.currentRaidData = bossData;
		m_btnAct = BUTTON_ACT.BOSS_REWARD;
		SoundManager.SePlay("sys_menu_decide");
		RaidBossDamageRewardWindow.Create(bossData, this);
	}

	public void OnClickBossInfoBackButton(RaidBossData bossData)
	{
		m_btnAct = BUTTON_ACT.NONE;
	}

	public void OnClickReward()
	{
		m_btnAct = BUTTON_ACT.INFO;
		if (EventManager.Instance != null)
		{
			EventRewardWindow.Create(EventManager.Instance.RaidBossInfo);
		}
	}

	public void OnClickRoulette()
	{
		SetAlertSimpleUI(true);
		m_btnAct = BUTTON_ACT.ROULETTE;
		m_close = true;
		SoundManager.SePlay("sys_window_close");
		GameObject gameObject = GameObjectUtil.FindChildGameObject(base.gameObject, "Btn_roulette");
		UIPlayAnimation component = gameObject.GetComponent<UIPlayAnimation>();
		if (component != null)
		{
			EventDelegate.Add(component.onFinished, WindowAnimationFinishCallback, true);
			component.Play(true);
		}
	}

	public void OnClickRouletteRaid()
	{
		if (EventManager.Instance != null && EventManager.Instance.TypeInTime != EventManager.EventType.RAID_BOSS)
		{
			GeneralUtil.ShowEventEnd();
			return;
		}
		SetAlertSimpleUI(true);
		RouletteUtility.rouletteDefault = RouletteCategory.RAID;
		m_btnAct = BUTTON_ACT.ROULETTE;
		m_close = true;
		SoundManager.SePlay("sys_window_close");
		GameObject gameObject = GameObjectUtil.FindChildGameObject(base.gameObject, "Btn_raid_roulette");
		UIPlayAnimation component = gameObject.GetComponent<UIPlayAnimation>();
		if (component != null)
		{
			EventDelegate.Add(component.onFinished, WindowAnimationFinishCallback, true);
			component.Play(true);
		}
	}

	public void OnClickAdventBg()
	{
		if (m_advent != null)
		{
			m_advent.SetActive(false);
			SoundManager.SePlay("sys_window_close");
		}
	}

	public void OnClickEndButton(ButtonInfoTable.ButtonType btnType)
	{
		SetAlertSimpleUI(true);
		switch (btnType)
		{
		case ButtonInfoTable.ButtonType.REDSTAR_TO_SHOP:
			m_btnAct = BUTTON_ACT.SHOP_RSRING;
			break;
		case ButtonInfoTable.ButtonType.RING_TO_SHOP:
			m_btnAct = BUTTON_ACT.SHOP_RING;
			break;
		case ButtonInfoTable.ButtonType.CHALLENGE_TO_SHOP:
			m_btnAct = BUTTON_ACT.SHOP_CHALLENGE;
			break;
		case ButtonInfoTable.ButtonType.REWARDLIST_TO_CHAO_ROULETTE:
			m_btnAct = BUTTON_ACT.ROULETTE;
			break;
		case ButtonInfoTable.ButtonType.EVENT_BACK:
			m_btnAct = BUTTON_ACT.CLOSE;
			break;
		}
		m_close = true;
		if (m_animation != null)
		{
			ActiveAnimation activeAnimation = ActiveAnimation.Play(m_animation, "ui_eventmenu_outro", Direction.Forward);
			EventDelegate.Add(activeAnimation.onFinished, WindowAnimationFinishCallback, true);
		}
	}

	public void OnClickBossAttention()
	{
		if (m_isBossAttention)
		{
			ActiveAnimation activeAnimation = ActiveAnimation.Play(m_bossAnim, "ui_EventResult_raidboss_attention_outro_Anim", Direction.Forward);
			EventDelegate.Add(activeAnimation.onFinished, BossAnimationFinishCallback, true);
			m_isBossAttention = false;
			SoundManager.SePlay("sys_window_close");
		}
	}

	private void OnClickPlatformBackButton(WindowBase.BackButtonMessage msg)
	{
		if (msg != null && m_alertCollider)
		{
			msg.StaySequence();
		}
	}

	private void BossAnimationIntroCallback()
	{
		m_isBossAttention = true;
	}

	private void BossAnimationFinishCallback()
	{
		m_advent.SetActive(false);
	}

	private bool IsActiveAdventData()
	{
		if (m_advent != null)
		{
			return m_advent.activeSelf;
		}
		return false;
	}

	private void WindowAnimationFinishCallback()
	{
		if (m_close)
		{
			switch (m_btnAct)
			{
			case BUTTON_ACT.BOSS_PLAY:
				HudMenuUtility.SendVirtualNewItemSelectClicked(HudMenuUtility.ITEM_SELECT_MODE.EVENT_BOSS);
				base.enabledAnchorObjects = false;
				HudMenuUtility.SendEnableShopButton(true);
				break;
			case BUTTON_ACT.ROULETTE:
				base.enabledAnchorObjects = false;
				HudMenuUtility.SendEnableShopButton(true);
				HudMenuUtility.SendChaoRouletteButtonClicked();
				break;
			case BUTTON_ACT.CHALLENGE:
				base.enabledAnchorObjects = false;
				break;
			case BUTTON_ACT.SHOP_RSRING:
				HudMenuUtility.SendMenuButtonClicked(ButtonInfoTable.ButtonType.REDSTAR_TO_SHOP);
				base.enabledAnchorObjects = false;
				break;
			case BUTTON_ACT.SHOP_RING:
				HudMenuUtility.SendMenuButtonClicked(ButtonInfoTable.ButtonType.RING_TO_SHOP);
				base.enabledAnchorObjects = false;
				break;
			case BUTTON_ACT.SHOP_CHALLENGE:
				HudMenuUtility.SendMenuButtonClicked(ButtonInfoTable.ButtonType.CHALLENGE_TO_SHOP);
				base.enabledAnchorObjects = false;
				break;
			default:
				HudMenuUtility.SendMenuButtonClicked(ButtonInfoTable.ButtonType.EVENT_BACK);
				base.enabledAnchorObjects = false;
				HudMenuUtility.SendEnableShopButton(true);
				break;
			case BUTTON_ACT.BOSS_INFO:
			case BUTTON_ACT.BOSS_REWARD:
			case BUTTON_ACT.INFO:
				break;
			}
			SetAlertSimpleUI(false);
			BackKeyManager.RemoveWindowCallBack(base.gameObject);
			m_opened = false;
			m_close = false;
		}
		else
		{
			if (m_openMode != 0)
			{
				StartRaidbossAttentionAnim();
			}
			m_opened = true;
			SetAlertSimpleUI(false);
		}
	}

	private void SetAlertSimpleUI(bool flag)
	{
		if (m_alertCollider)
		{
			if (!flag)
			{
				HudMenuUtility.SetConnectAlertSimpleUI(false);
				m_alertCollider = false;
			}
		}
		else if (flag)
		{
			HudMenuUtility.SetConnectAlertSimpleUI(true);
			m_alertCollider = true;
		}
	}

	public static bool IsEnabled()
	{
		bool result = false;
		if (s_instance != null)
		{
			result = s_instance.enabledAnchorObjects;
		}
		return result;
	}

	public static bool IsDataReload()
	{
		bool result = true;
		if (s_instance != null)
		{
			result = s_instance.IsReload();
		}
		return result;
	}

	private bool IsReload()
	{
		bool result = false;
		if (m_rbInfoData == null || m_time > 5f)
		{
			result = true;
		}
		return result;
	}

	public static bool IsOpend()
	{
		if (s_instance != null)
		{
			return s_instance.m_opened;
		}
		return false;
	}

	public static bool IsOpenAdvent()
	{
		if (s_instance != null)
		{
			return s_instance.IsActiveAdventData();
		}
		return false;
	}

	public static RaidBossWindow Create(RaidBossInfo info, WINDOW_OPEN_MODE mode)
	{
		if (s_instance != null)
		{
			if (s_instance.gameObject.transform.parent != null && s_instance.gameObject.transform.parent.name != "Camera")
			{
				return null;
			}
			s_instance.gameObject.SetActive(true);
			s_instance.Setup(info, mode);
			return s_instance;
		}
		return null;
	}

	private static void RaidbossDiscoverSaveDataUpdate()
	{
		if (EventManager.Instance == null || EventManager.Instance.Type != EventManager.EventType.RAID_BOSS)
		{
			return;
		}
		SystemSaveManager instance = SystemSaveManager.Instance;
		if (!(instance != null))
		{
			return;
		}
		SystemData systemdata = instance.GetSystemdata();
		if (systemdata == null || EventManager.Instance.RaidBossInfo == null)
		{
			return;
		}
		List<ServerEventRaidBossState> userRaidBossList = EventManager.Instance.UserRaidBossList;
		if (userRaidBossList == null)
		{
			return;
		}
		bool flag = false;
		foreach (ServerEventRaidBossState item in userRaidBossList)
		{
			if (item.Encounter && systemdata.currentRaidDrawIndex != item.Id)
			{
				systemdata.currentRaidDrawIndex = item.Id;
				systemdata.raidEntryFlag = false;
				flag = true;
				break;
			}
		}
		if (flag)
		{
			instance.SaveSystemData();
		}
	}

	public void ShowNoCommunication()
	{
		GeneralUtil.ShowNoCommunication();
	}

	private void Awake()
	{
		SetInstance();
		base.enabledAnchorObjects = false;
	}

	private void OnDestroy()
	{
		if (s_instance == this)
		{
			s_instance = null;
		}
	}

	private void SetInstance()
	{
		if (s_instance == null)
		{
			s_instance = this;
		}
		else
		{
			UnityEngine.Object.Destroy(base.gameObject);
		}
	}
}
