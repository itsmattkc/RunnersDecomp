using AnimationOrTween;
using Message;
using SaveData;
using System.Collections.Generic;
using Text;
using UnityEngine;

public class MenuPlayerSetUnlockedChara : MonoBehaviour
{
	private enum EventSignal
	{
		BUTTON_RING_BUTTON_PRESSED = 100,
		BUTTON_RS_RING_BUTTON_PRESSED
	}

	private TinyFsmBehavior m_fsm;

	private CharaType m_charaType;

	private GameObject m_pageRootObject;

	private int m_ringCost;

	private int m_redStartRingCost;

	private bool m_isWindowClose = true;

	private int m_currentDeckSetStock;

	private List<GameObject> m_deckObjects;

	public void Setup(CharaType charaType, GameObject pageRootObject)
	{
		m_charaType = charaType;
		m_pageRootObject = pageRootObject;
	}

	private void Start()
	{
		m_fsm = (base.gameObject.AddComponent(typeof(TinyFsmBehavior)) as TinyFsmBehavior);
		if (!(m_fsm == null))
		{
			TinyFsmBehavior.Description description = new TinyFsmBehavior.Description(this);
			description.initState = new TinyFsmState(StateIdle);
			description.onFixedUpdate = true;
			m_fsm.SetUp(description);
			m_ringCost = 0;
			m_redStartRingCost = 0;
			OnEnable();
			m_currentDeckSetStock = DeckUtil.GetDeckCurrentStockIndex();
			SetupTabView();
		}
	}

	private void LateUpdate()
	{
	}

	private void OnEnable()
	{
		if (m_pageRootObject == null)
		{
			return;
		}
		ServerPlayerState playerState = ServerInterface.PlayerState;
		ServerCharacterState serverCharacterState = playerState.CharacterState(m_charaType);
		if (serverCharacterState == null)
		{
			return;
		}
		bool flag = false;
		if (serverCharacterState.IsUnlocked)
		{
			flag = true;
		}
		ServerCharacterState.LockCondition lockCondition = ServerCharacterState.LockCondition.RING_OR_RED_STAR_RING;
		ServerInterface loggedInServerInterface = ServerInterface.LoggedInServerInterface;
		if (loggedInServerInterface != null)
		{
			ServerCharacterState serverCharacterState2 = ServerInterface.PlayerState.CharacterState(m_charaType);
			if (serverCharacterState2 != null)
			{
				lockCondition = serverCharacterState2.Condition;
			}
		}
		GameObject gameObject = GameObjectUtil.FindChildGameObject(m_pageRootObject, "pattern_lock");
		if (gameObject != null)
		{
			gameObject.SetActive(!flag);
		}
		GameObject gameObject2 = GameObjectUtil.FindChildGameObject(m_pageRootObject, "Btn_pay_ring");
		if (gameObject2 != null)
		{
			if (lockCondition == ServerCharacterState.LockCondition.RING_OR_RED_STAR_RING)
			{
				gameObject2.SetActive(true);
				UIButtonMessage uIButtonMessage = gameObject2.AddComponent<UIButtonMessage>();
				uIButtonMessage.target = base.gameObject;
				uIButtonMessage.functionName = "OnClickRingCostButton";
			}
			else
			{
				gameObject2.SetActive(false);
			}
		}
		GameObject gameObject3 = GameObjectUtil.FindChildGameObject(m_pageRootObject, "Btn_pay_rsring");
		if (gameObject3 != null)
		{
			if (lockCondition == ServerCharacterState.LockCondition.RING_OR_RED_STAR_RING)
			{
				gameObject3.SetActive(true);
				UIButtonMessage uIButtonMessage2 = gameObject3.AddComponent<UIButtonMessage>();
				uIButtonMessage2.target = base.gameObject;
				uIButtonMessage2.functionName = "OnClickRedStartRingCostButton";
			}
			else
			{
				gameObject3.SetActive(false);
			}
		}
		InitCost();
		UILabel uILabel = GameObjectUtil.FindChildGameObjectComponent<UILabel>(m_pageRootObject, "Lbl_lock_txt");
		if (uILabel != null)
		{
			string empty = string.Empty;
			string cellName = "recommend_chara_unlock_text_" + CharaName.Name[(int)m_charaType];
			empty = (uILabel.text = TextManager.GetText(TextManager.TextType.TEXTTYPE_COMMON_TEXT, "WindowText", cellName).text);
		}
	}

	private void SetupTabView()
	{
		GameObject gameObject = GameObjectUtil.FindChildGameObject(base.gameObject, "deck_tab");
		if (!(gameObject != null))
		{
			return;
		}
		if (m_deckObjects != null)
		{
			m_deckObjects.Clear();
		}
		m_deckObjects = new List<GameObject>();
		for (int i = 0; i < 10; i++)
		{
			GameObject gameObject2 = GameObjectUtil.FindChildGameObject(gameObject, "tab_" + (i + 1));
			if (gameObject2 != null)
			{
				m_deckObjects.Add(gameObject2);
				continue;
			}
			break;
		}
		if (m_deckObjects.Count > 0 && m_deckObjects.Count > m_currentDeckSetStock)
		{
			for (int j = 0; j < m_deckObjects.Count; j++)
			{
				m_deckObjects[j].SetActive(m_currentDeckSetStock == j);
			}
		}
	}

	private void InitCost()
	{
		ServerPlayerState playerState = ServerInterface.PlayerState;
		if (playerState == null)
		{
			return;
		}
		ServerCharacterState serverCharacterState = playerState.CharacterState(m_charaType);
		if (serverCharacterState == null)
		{
			return;
		}
		bool active = false;
		int num = 0;
		int num2 = 0;
		int id = (int)new ServerItem(m_charaType).id;
		ServerCampaignData campaignInSession = ServerInterface.CampaignState.GetCampaignInSession(Constants.Campaign.emType.CharacterUpgradeCost, id);
		if (campaignInSession != null)
		{
			int iContent = campaignInSession.iContent;
			int iSubContent = campaignInSession.iSubContent;
			num = iContent;
			num2 = iSubContent;
			active = true;
		}
		else
		{
			num = serverCharacterState.Cost;
			num2 = serverCharacterState.NumRedRings;
		}
		if (m_ringCost != num)
		{
			m_ringCost = num;
			GameObject gameObject = GameObjectUtil.FindChildGameObject(m_pageRootObject, "img_icon_sale_ring");
			if (gameObject != null)
			{
				gameObject.SetActive(active);
			}
			UILabel uILabel = GameObjectUtil.FindChildGameObjectComponent<UILabel>(m_pageRootObject, "Lbl_pay_ring");
			if (uILabel != null)
			{
				uILabel.text = m_ringCost.ToString();
			}
		}
		if (m_redStartRingCost != num2)
		{
			m_redStartRingCost = num2;
			GameObject gameObject2 = GameObjectUtil.FindChildGameObject(m_pageRootObject, "img_icon_sale_rsring");
			if (gameObject2 != null)
			{
				gameObject2.SetActive(active);
			}
			UILabel uILabel2 = GameObjectUtil.FindChildGameObjectComponent<UILabel>(m_pageRootObject, "Lbl_pay_rsring");
			if (uILabel2 != null)
			{
				uILabel2.text = m_redStartRingCost.ToString();
			}
		}
	}

	private TinyFsmState StateIdle(TinyFsmEvent e)
	{
		switch (e.Signal)
		{
		case -3:
			return TinyFsmState.End();
		case -4:
			return TinyFsmState.End();
		case 0:
		{
			InitCost();
			int deckCurrentStockIndex = DeckUtil.GetDeckCurrentStockIndex();
			if (m_currentDeckSetStock != deckCurrentStockIndex)
			{
				m_currentDeckSetStock = deckCurrentStockIndex;
				SetupTabView();
			}
			return TinyFsmState.End();
		}
		case 100:
			m_fsm.ChangeState(new TinyFsmState(StateWaitRingButtonPressed));
			return TinyFsmState.End();
		case 101:
			m_fsm.ChangeState(new TinyFsmState(StateWaitRedStartRingButtonPressed));
			return TinyFsmState.End();
		default:
			return TinyFsmState.End();
		}
	}

	private TinyFsmState StateWaitRingButtonPressed(TinyFsmEvent e)
	{
		switch (e.Signal)
		{
		case -3:
			return TinyFsmState.End();
		case -4:
			return TinyFsmState.End();
		case 0:
			if (GeneralWindow.IsYesButtonPressed)
			{
				GeneralWindow.Close();
				int ringCost = m_ringCost;
				int ringCount = (int)SaveDataManager.Instance.ItemData.RingCount;
				if (ringCost <= ringCount)
				{
					UnLock(new ServerItem(ServerItem.Id.RING));
					m_fsm.ChangeState(new TinyFsmState(StateIdle));
				}
				else
				{
					m_fsm.ChangeState(new TinyFsmState(StateFailedPurchaseRing));
				}
			}
			else if (GeneralWindow.IsNoButtonPressed)
			{
				GeneralWindow.Close();
				m_fsm.ChangeState(new TinyFsmState(StateIdle));
			}
			return TinyFsmState.End();
		default:
			return TinyFsmState.End();
		}
	}

	private TinyFsmState StateWaitRedStartRingButtonPressed(TinyFsmEvent e)
	{
		switch (e.Signal)
		{
		case -3:
			return TinyFsmState.End();
		case -4:
			return TinyFsmState.End();
		case 0:
			if (GeneralWindow.IsYesButtonPressed)
			{
				GeneralWindow.Close();
				int redStartRingCost = m_redStartRingCost;
				int redRingCount = (int)SaveDataManager.Instance.ItemData.RedRingCount;
				if (redStartRingCost <= redRingCount)
				{
					UnLock(new ServerItem(ServerItem.Id.RSRING));
					m_fsm.ChangeState(new TinyFsmState(StateIdle));
				}
				else
				{
					m_fsm.ChangeState(new TinyFsmState(StateFailedPurchaseRedRing));
				}
			}
			else if (GeneralWindow.IsNoButtonPressed)
			{
				GeneralWindow.Close();
				m_fsm.ChangeState(new TinyFsmState(StateIdle));
			}
			return TinyFsmState.End();
		default:
			return TinyFsmState.End();
		}
	}

	private TinyFsmState StateFailedPurchaseRing(TinyFsmEvent e)
	{
		switch (e.Signal)
		{
		case -3:
		{
			GeneralWindow.CInfo info = default(GeneralWindow.CInfo);
			info.name = "RingMissing";
			info.caption = TextManager.GetText(TextManager.TextType.TEXTTYPE_COMMON_TEXT, "ItemRoulette", "gw_cost_caption").text;
			info.message = TextManager.GetText(TextManager.TextType.TEXTTYPE_COMMON_TEXT, "ItemRoulette", "gw_cost_text").text;
			info.anchor_path = "Camera/menu_Anim/PlayerSet_2_UI/Anchor_5_MC";
			info.buttonType = GeneralWindow.ButtonType.ShopCancel;
			info.finishedCloseDelegate = GeneralWindowClosedCallback;
			GeneralWindow.Create(info);
			m_isWindowClose = false;
			return TinyFsmState.End();
		}
		case -4:
			return TinyFsmState.End();
		case 0:
			if (m_isWindowClose)
			{
				if (GeneralWindow.IsYesButtonPressed)
				{
					HudMenuUtility.SendMenuButtonClicked(ButtonInfoTable.ButtonType.RING_TO_SHOP);
				}
				GeneralWindow.Close();
				m_fsm.ChangeState(new TinyFsmState(StateIdle));
			}
			return TinyFsmState.End();
		default:
			return TinyFsmState.End();
		}
	}

	private TinyFsmState StateFailedPurchaseRedRing(TinyFsmEvent e)
	{
		switch (e.Signal)
		{
		case -3:
		{
			GeneralWindow.CInfo info = default(GeneralWindow.CInfo);
			info.name = "RingMissing";
			info.caption = TextManager.GetText(TextManager.TextType.TEXTTYPE_COMMON_TEXT, "ChaoRoulette", "gw_cost_caption").text;
			if (ServerInterface.IsRSREnable())
			{
				info.message = TextManager.GetText(TextManager.TextType.TEXTTYPE_COMMON_TEXT, "ChaoRoulette", "gw_cost_caption_text").text;
				info.buttonType = GeneralWindow.ButtonType.ShopCancel;
			}
			else
			{
				info.message = TextManager.GetText(TextManager.TextType.TEXTTYPE_COMMON_TEXT, "ChaoRoulette", "gw_cost_caption_text_2").text;
				info.buttonType = GeneralWindow.ButtonType.Ok;
			}
			info.anchor_path = "Camera/menu_Anim/PlayerSet_2_UI/Anchor_5_MC";
			info.finishedCloseDelegate = GeneralWindowClosedCallback;
			GeneralWindow.Create(info);
			m_isWindowClose = false;
			return TinyFsmState.End();
		}
		case -4:
			return TinyFsmState.End();
		case 0:
			if (m_isWindowClose)
			{
				if (GeneralWindow.IsYesButtonPressed)
				{
					HudMenuUtility.SendMenuButtonClicked(ButtonInfoTable.ButtonType.REDSTAR_TO_SHOP);
				}
				GeneralWindow.Close();
				m_fsm.ChangeState(new TinyFsmState(StateIdle));
			}
			return TinyFsmState.End();
		default:
			return TinyFsmState.End();
		}
	}

	private void UnLock(ServerItem serverItem)
	{
		ServerInterface loggedInServerInterface = ServerInterface.LoggedInServerInterface;
		if (loggedInServerInterface != null)
		{
			loggedInServerInterface.RequestServerUnlockedCharacter(m_charaType, serverItem, base.gameObject);
			return;
		}
		CharaData charaData = SaveDataManager.Instance.CharaData;
		charaData.Status[(int)m_charaType] = 1;
		SaveDataManager.Instance.ItemData.RingCount -= (uint)m_ringCost;
		ServerUnlockedCharacter_Succeeded(null);
	}

	private void OnClickRingCostButton()
	{
		GeneralWindow.CInfo info = default(GeneralWindow.CInfo);
		info.caption = TextManager.GetText(TextManager.TextType.TEXTTYPE_COMMON_TEXT, "WindowText", "chara_unlock_caption").text;
		TextObject text = TextManager.GetText(TextManager.TextType.TEXTTYPE_COMMON_TEXT, "WindowText", "chara_unlock_ring_text");
		int ringCost = m_ringCost;
		text.ReplaceTag("{RING_COST}", ringCost.ToString());
		info.message = text.text;
		info.anchor_path = "Camera/menu_Anim/PlayerSet_2_UI/Anchor_5_MC";
		info.buttonType = GeneralWindow.ButtonType.YesNo;
		GeneralWindow.Create(info);
		TinyFsmEvent signal = TinyFsmEvent.CreateUserEvent(100);
		if (m_fsm != null)
		{
			m_fsm.Dispatch(signal);
		}
	}

	private void OnClickRedStartRingCostButton()
	{
		GeneralWindow.CInfo info = default(GeneralWindow.CInfo);
		info.caption = TextManager.GetText(TextManager.TextType.TEXTTYPE_COMMON_TEXT, "WindowText", "chara_unlock_caption").text;
		TextObject text = TextManager.GetText(TextManager.TextType.TEXTTYPE_COMMON_TEXT, "WindowText", "chara_unlock_rsring_text");
		int redStartRingCost = m_redStartRingCost;
		text.ReplaceTag("{RING_COST}", redStartRingCost.ToString());
		info.message = text.text;
		info.anchor_path = "Camera/menu_Anim/PlayerSet_2_UI/Anchor_5_MC";
		info.buttonType = GeneralWindow.ButtonType.YesNo;
		GeneralWindow.Create(info);
		TinyFsmEvent signal = TinyFsmEvent.CreateUserEvent(101);
		if (m_fsm != null)
		{
			m_fsm.Dispatch(signal);
		}
	}

	private void ServerUnlockedCharacter_Succeeded(MsgGetPlayerStateSucceed msg)
	{
		Animation component = m_pageRootObject.GetComponent<Animation>();
		if (component != null)
		{
			ActiveAnimation activeAnimation = ActiveAnimation.Play(component, Direction.Forward);
			if (activeAnimation != null)
			{
				EventDelegate.Add(activeAnimation.onFinished, BuyAnimEndCallback, true);
			}
		}
		else
		{
			GameObject gameObject = GameObjectUtil.FindChildGameObject(m_pageRootObject, "pattern_lock");
			if (gameObject != null)
			{
				gameObject.SetActive(false);
			}
		}
		HudMenuUtility.SendMsgUpdateSaveDataDisplay();
		GameObject playerSetRoot = MenuPlayerSetUtil.GetPlayerSetRoot();
		MenuPlayerSetContents component2 = playerSetRoot.GetComponent<MenuPlayerSetContents>();
		if (component2 != null)
		{
			component2.UnlockedChara(m_charaType);
		}
		SoundManager.SePlay("sys_buy");
	}

	private void GeneralWindowClosedCallback()
	{
		m_isWindowClose = true;
	}

	private void BuyAnimEndCallback()
	{
		GameObject gameObject = GameObjectUtil.FindChildGameObject(m_pageRootObject, "pattern_lock");
		if (gameObject != null)
		{
			gameObject.SetActive(false);
		}
	}
}
