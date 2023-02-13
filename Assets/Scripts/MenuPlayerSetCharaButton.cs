using AnimationOrTween;
using Message;
using SaveData;
using System.Collections.Generic;
using Text;
using UnityEngine;

public class MenuPlayerSetCharaButton : MonoBehaviour
{
	private enum EventSignal
	{
		CHARA_CHANGE = 100
	}

	public delegate void AnimEndCallback();

	private TinyFsmBehavior m_fsm;

	private GameObject m_pageRootObject;

	private CharaType m_charaType;

	private UISprite m_charaIcon;

	private UILabel m_charaName;

	private UILabel m_charaLevel;

	private UISprite m_ribbon;

	[SerializeField]
	private UIObjectContainer m_objectContainer;

	private PlayerSetWindowUIWithButton m_charaChangeWindow;

	private AnimEndCallback m_animEndCallback;

	private int m_currentDeckSetStock;

	private List<GameObject> m_deckObjects;

	private bool m_animEnd = true;

	public bool AnimEnd
	{
		get
		{
			return m_animEnd;
		}
		private set
		{
		}
	}

	public void Setup(CharaType charaType, GameObject pageRootObject)
	{
		m_charaType = charaType;
		m_pageRootObject = pageRootObject;
		GameObject gameObject = GameObjectUtil.FindChildGameObject(m_pageRootObject, "Btn_player_main");
		if (gameObject != null)
		{
			UIButtonMessage uIButtonMessage = gameObject.AddComponent<UIButtonMessage>();
			uIButtonMessage.target = base.gameObject;
			uIButtonMessage.functionName = "OnSelected";
		}
		UISprite uISprite = GameObjectUtil.FindChildGameObjectComponent<UISprite>(gameObject, "img_player_speacies");
		if (uISprite != null)
		{
			uISprite.spriteName = HudUtility.GetCharaAttributeSpriteName(m_charaType);
		}
		UISprite uISprite2 = GameObjectUtil.FindChildGameObjectComponent<UISprite>(gameObject, "img_player_genus");
		if (uISprite2 != null)
		{
			uISprite2.spriteName = HudUtility.GetTeamAttributeSpriteName(m_charaType);
		}
		m_currentDeckSetStock = DeckUtil.GetDeckCurrentStockIndex();
		SetupTabView();
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

	public void UpdateRibbon()
	{
		m_ribbon = GameObjectUtil.FindChildGameObjectComponent<UISprite>(m_pageRootObject, "img_ribbon");
		if (m_ribbon != null)
		{
			PlayerData playerData = SaveDataManager.Instance.PlayerData;
			if (m_charaType == playerData.MainChara)
			{
				m_ribbon.gameObject.SetActive(true);
				m_ribbon.spriteName = "ui_mm_player_ribbon_0";
			}
			else if (m_charaType == playerData.SubChara)
			{
				m_ribbon.gameObject.SetActive(true);
				m_ribbon.spriteName = "ui_mm_player_ribbon_1";
			}
			else
			{
				m_ribbon.gameObject.SetActive(false);
			}
		}
	}

	private void Start()
	{
		UITexture uITexture = GameObjectUtil.FindChildGameObjectComponent<UITexture>(m_pageRootObject, "img_player_tex");
		if (uITexture != null)
		{
			TextureRequestChara request = new TextureRequestChara(m_charaType, uITexture);
			TextureAsyncLoadManager.Instance.Request(request);
		}
		m_charaName = GameObjectUtil.FindChildGameObjectComponent<UILabel>(m_pageRootObject, "Lbl_player_name");
		if (m_charaName != null)
		{
			string text = TextManager.GetText(TextManager.TextType.TEXTTYPE_COMMON_TEXT, "CharaName", CharaName.Name[(int)m_charaType]).text;
			m_charaName.text = text;
		}
		m_charaLevel = GameObjectUtil.FindChildGameObjectComponent<UILabel>(m_pageRootObject, "Lbl_player_lv");
		if (m_charaLevel != null)
		{
			int totalLevel = MenuPlayerSetUtil.GetTotalLevel(m_charaType);
			m_charaLevel.text = TextUtility.GetTextLevel(string.Format("{0:000}", totalLevel));
		}
		UpdateRibbon();
		m_fsm = base.gameObject.AddComponent<TinyFsmBehavior>();
		TinyFsmBehavior.Description description = new TinyFsmBehavior.Description(this);
		description.initState = new TinyFsmState(StateIdle);
		description.onFixedUpdate = true;
		m_fsm.SetUp(description);
		m_objectContainer = base.gameObject.AddComponent<UIObjectContainer>();
		List<GameObject> list = new List<GameObject>();
		GameObject parent = GameObjectUtil.FindChildGameObject(base.gameObject, "Btn_player_main");
		list.Add(GameObjectUtil.FindChildGameObject(parent, "eff_0"));
		list.Add(GameObjectUtil.FindChildGameObject(parent, "eff_1"));
		list.Add(GameObjectUtil.FindChildGameObject(parent, "img_player_main_sale_word"));
		m_objectContainer.Objects = list.ToArray();
	}

	public void LevelUp(AnimEndCallback callback)
	{
		m_animEndCallback = callback;
		m_animEnd = false;
		int totalLevel = MenuPlayerSetUtil.GetTotalLevel(m_charaType);
		m_charaLevel.text = TextUtility.GetTextLevel(string.Format("{0:000}", totalLevel));
		Animation animation = GameObjectUtil.FindChildGameObjectComponent<Animation>(base.gameObject, "Btn_player_main");
		if (animation != null)
		{
			ActiveAnimation activeAnimation = ActiveAnimation.Play(animation, Direction.Forward);
			if (activeAnimation != null)
			{
				EventDelegate.Add(activeAnimation.onFinished, LevelUpAnimEndCallback, true);
			}
			if (m_objectContainer != null)
			{
				m_objectContainer.SetActive(true);
			}
		}
		AchievementManager.RequestUpdate();
	}

	public void SkipLevelUp()
	{
		Animation animation = GameObjectUtil.FindChildGameObjectComponent<Animation>(base.gameObject, "Btn_player_main");
		if (!(animation != null))
		{
			return;
		}
		foreach (AnimationState item in animation)
		{
			if (!(item == null))
			{
				item.time = item.length * 0.99f;
			}
		}
	}

	public void OnSelected()
	{
		TinyFsmEvent signal = TinyFsmEvent.CreateUserEvent(100);
		if (m_fsm != null)
		{
			m_fsm.Dispatch(signal);
		}
		if (HudMenuUtility.IsTutorial_11())
		{
			HudMenuUtility.SendMsgMenuSequenceToMainMenu(MsgMenuSequence.SequeneceType.CHARA_MAIN);
		}
	}

	private void LateUpdate()
	{
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
			int deckCurrentStockIndex = DeckUtil.GetDeckCurrentStockIndex();
			if (m_currentDeckSetStock != deckCurrentStockIndex)
			{
				m_currentDeckSetStock = deckCurrentStockIndex;
				SetupTabView();
			}
			return TinyFsmState.End();
		}
		case 100:
		{
			int playableCharaCount = MenuPlayerSetUtil.GetPlayableCharaCount();
			if (playableCharaCount <= 1)
			{
				GeneralWindow.CInfo info = default(GeneralWindow.CInfo);
				info.caption = TextManager.GetText(TextManager.TextType.TEXTTYPE_COMMON_TEXT, "WindowText", "recommend_chara_unlock_caption").text;
				string text = info.message = TextManager.GetText(TextManager.TextType.TEXTTYPE_COMMON_TEXT, "WindowText", "recommend_chara_unlock_text").text;
				info.anchor_path = "Camera/menu_Anim/PlayerSet_2_UI/Anchor_5_MC";
				info.buttonType = GeneralWindow.ButtonType.Ok;
				GeneralWindow.Create(info);
			}
			else
			{
				m_charaChangeWindow = PlayerSetWindowUIWithButton.Create(m_charaType);
				if (m_fsm != null)
				{
					m_fsm.ChangeState(new TinyFsmState(StateCharaChangeWindow));
				}
			}
			return TinyFsmState.End();
		}
		default:
			return TinyFsmState.End();
		}
	}

	private TinyFsmState StateRecommendUnlockChara(TinyFsmEvent e)
	{
		switch (e.Signal)
		{
		case -3:
			return TinyFsmState.End();
		case -4:
			return TinyFsmState.End();
		case 0:
			if (GeneralWindow.IsOkButtonPressed && m_fsm != null)
			{
				GeneralWindow.Close();
				m_fsm.ChangeState(new TinyFsmState(StateIdle));
			}
			return TinyFsmState.End();
		default:
			return TinyFsmState.End();
		}
	}

	private TinyFsmState StateCharaChangeWindow(TinyFsmEvent e)
	{
		switch (e.Signal)
		{
		case -3:
			return TinyFsmState.End();
		case -4:
			return TinyFsmState.End();
		case 0:
			if (m_charaChangeWindow.isClickMain)
			{
				if (m_fsm != null)
				{
					m_fsm.ChangeState(new TinyFsmState(StateIdle));
				}
				GameObject playerSetRoot = MenuPlayerSetUtil.GetPlayerSetRoot();
				MenuPlayerSetContents component = playerSetRoot.GetComponent<MenuPlayerSetContents>();
				if (component != null)
				{
					component.ChangeMainChara(m_charaType);
				}
			}
			if (m_charaChangeWindow.isClickSub)
			{
				if (m_fsm != null)
				{
					m_fsm.ChangeState(new TinyFsmState(StateIdle));
				}
				GameObject playerSetRoot2 = MenuPlayerSetUtil.GetPlayerSetRoot();
				MenuPlayerSetContents component2 = playerSetRoot2.GetComponent<MenuPlayerSetContents>();
				if (component2 != null)
				{
					component2.ChangeSubChara(m_charaType);
				}
			}
			if (m_charaChangeWindow.isClickCancel && m_fsm != null)
			{
				m_fsm.ChangeState(new TinyFsmState(StateIdle));
			}
			return TinyFsmState.End();
		default:
			return TinyFsmState.End();
		}
	}

	private void LevelUpAnimEndCallback()
	{
		m_animEnd = true;
		if (m_objectContainer != null)
		{
			m_objectContainer.SetActive(false);
		}
		if (m_animEndCallback != null)
		{
			m_animEndCallback();
			m_animEndCallback = null;
		}
	}
}
