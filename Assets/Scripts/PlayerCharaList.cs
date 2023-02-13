using AnimationOrTween;
using Message;
using SaveData;
using System.Collections.Generic;
using UnityEngine;

public class PlayerCharaList : MonoBehaviour
{
	public enum SET_CHARA_MODE
	{
		MAIN,
		SUB,
		CHANGE
	}

	private const int CHARA_DRAW_MAX = 4;

	private const float CHANGE_BTN_DELAY = 1.5f;

	[SerializeField]
	private Animation m_animation;

	[SerializeField]
	private UIRectItemStorage m_storage;

	[SerializeField]
	private GameObject m_charaDeckObject;

	[SerializeField]
	private GameObject m_chaoDeckObject;

	[SerializeField]
	private List<GameObject> m_gameObjectList;

	private static bool s_isActive;

	private static PlayerCharaList s_instance;

	private ServerPlayerState.CHARA_SORT m_sortType;

	private int m_sortOffset;

	private int m_page;

	private int m_pageMax;

	private float m_changeDelay;

	private UIImageButton m_changeBtn;

	private List<ui_player_set_scroll> m_charaObjectList;

	private Dictionary<CharaType, ServerCharacterState> m_charaStateList;

	private int m_currentDeck;

	private List<DeckUtil.DeckSet> m_deckList;

	private bool m_isEnd;

	private DeckUtil.DeckSet m_oldDeckSet;

	private bool m_tutorial;

	private bool m_pickup;

	private int m_reqDeck;

	private CharaType m_reqCharaMain = CharaType.UNKNOWN;

	private CharaType m_reqCharaSub = CharaType.UNKNOWN;

	private int m_reqChaoMain = -1;

	private int m_reqChaoSub = -1;

	private ServerPlayerState.CHARA_SORT sortType
	{
		get
		{
			return m_sortType;
		}
	}

	private int sortOffset
	{
		get
		{
			return m_sortOffset;
		}
	}

	private int currentDeck
	{
		get
		{
			return m_currentDeck;
		}
	}

	public bool isTutorial
	{
		get
		{
			return m_tutorial;
		}
	}

	public void SetTutorialEnd()
	{
		BackKeyManager.InvalidFlag = false;
		m_tutorial = false;
	}

	private void Update()
	{
		if (m_changeDelay > 0f)
		{
			m_changeDelay -= Time.deltaTime;
			if (m_changeDelay <= 0f)
			{
				m_changeDelay = 0f;
				if (m_changeBtn != null)
				{
					if (CheckSetMode(SET_CHARA_MODE.CHANGE))
					{
						m_changeBtn.isEnabled = true;
					}
					else
					{
						m_changeBtn.isEnabled = false;
						m_changeDelay = -1f;
					}
				}
			}
		}
		if (!m_pickup && m_pageMax > 0 && GeneralWindow.IsCreated("ShowNoCommunicationPicupCharaList") && GeneralWindow.IsOkButtonPressed)
		{
			if (GeneralUtil.IsNetwork())
			{
				RouletteManager.Instance.RequestPicupCharaList();
				m_pickup = true;
			}
			else
			{
				GeneralUtil.ShowNoCommunication("ShowNoCommunicationPicupCharaList");
			}
		}
	}

	public void Setup()
	{
		base.gameObject.SetActive(true);
		m_pickup = false;
		m_tutorial = HudMenuUtility.IsTutorial_CharaLevelUp();
		if (m_tutorial)
		{
			BackKeyManager.InvalidFlag = true;
			TutorialCursor.StartTutorialCursor(TutorialCursor.Type.CHARASELECT_CHARA);
			HudMenuUtility.SaveSystemDataFlagStatus(SystemData.FlagStatus.CHARA_LEVEL_UP_EXPLAINED);
		}
		if (RouletteManager.Instance != null && !RouletteManager.Instance.IsRequestPicupCharaList() && m_pageMax <= 0)
		{
			if (GeneralUtil.IsNetwork())
			{
				RouletteManager.Instance.RequestPicupCharaList();
				m_pickup = true;
			}
			else
			{
				GeneralUtil.ShowNoCommunication("ShowNoCommunicationPicupCharaList");
				m_pickup = false;
			}
		}
		m_currentDeck = DeckUtil.GetDeckCurrentStockIndex();
		m_deckList = DeckUtil.GetDeckList();
		Debug.Log("GetCurrentDeck " + m_deckList.Count + "   " + m_currentDeck);
		m_isEnd = false;
		m_page = 0;
		m_pageMax = 0;
		m_changeDelay = 0f;
		SetParam(true);
		SetObject(true);
		if (m_animation != null)
		{
			ActiveAnimation.Play(m_animation, "ui_mm_player_set_2_intro_Anim", Direction.Forward);
		}
		if (m_gameObjectList != null && m_gameObjectList.Count > 0)
		{
			foreach (GameObject gameObject in m_gameObjectList)
			{
				if (gameObject != null)
				{
					gameObject.SetActive(true);
				}
			}
		}
		SetSort(ServerPlayerState.CHARA_SORT.TEAM_ATTR);
	}

	public bool UpdateView(bool rest = false)
	{
		m_currentDeck = DeckUtil.GetDeckCurrentStockIndex();
		m_deckList = DeckUtil.GetDeckList();
		SetParam(rest);
		SetObject(rest);
		return true;
	}

	public bool CheckSetMode(SET_CHARA_MODE setMode, CharaType setCharaType = CharaType.UNKNOWN)
	{
		bool result = false;
		int deckCurrentStockIndex = DeckUtil.GetDeckCurrentStockIndex();
		List<DeckUtil.DeckSet> deckList = DeckUtil.GetDeckList();
		DeckUtil.DeckSet deckSet = deckList[deckCurrentStockIndex];
		switch (setMode)
		{
		case SET_CHARA_MODE.MAIN:
			if (deckSet.charaMain != setCharaType)
			{
				result = true;
			}
			break;
		case SET_CHARA_MODE.SUB:
			if (deckSet.charaSub != setCharaType)
			{
				result = ((deckSet.charaMain != setCharaType || deckSet.charaSub != CharaType.UNKNOWN) ? true : false);
			}
			break;
		case SET_CHARA_MODE.CHANGE:
			if (deckSet.charaSub != CharaType.UNKNOWN && deckSet.charaMain != CharaType.UNKNOWN)
			{
				result = true;
			}
			break;
		}
		return result;
	}

	public bool SetChara(SET_CHARA_MODE setMode, CharaType setCharaType = CharaType.UNKNOWN)
	{
		bool flag = false;
		if (GeneralUtil.IsNetwork())
		{
			if (setCharaType != CharaType.UNKNOWN || setMode == SET_CHARA_MODE.CHANGE)
			{
				ServerInterface loggedInServerInterface = ServerInterface.LoggedInServerInterface;
				if (loggedInServerInterface != null)
				{
					int deckCurrentStockIndex = DeckUtil.GetDeckCurrentStockIndex();
					List<DeckUtil.DeckSet> deckList = DeckUtil.GetDeckList();
					DeckUtil.DeckSet deckSet = deckList[deckCurrentStockIndex];
					int num = -1;
					int num2 = -1;
					int num3 = -1;
					int num4 = -1;
					int num5 = -1;
					ServerCharacterState serverCharacterState = null;
					ServerCharacterState serverCharacterState2 = ServerInterface.PlayerState.CharacterState(deckSet.charaMain);
					ServerCharacterState serverCharacterState3 = ServerInterface.PlayerState.CharacterState(deckSet.charaSub);
					if (setCharaType != CharaType.UNKNOWN)
					{
						serverCharacterState = ServerInterface.PlayerState.CharacterState(setCharaType);
					}
					if (serverCharacterState != null)
					{
						num = serverCharacterState.Id;
					}
					if (serverCharacterState2 != null)
					{
						num2 = serverCharacterState2.Id;
						num4 = num2;
					}
					if (serverCharacterState3 != null)
					{
						num3 = serverCharacterState3.Id;
						num5 = num3;
					}
					switch (setMode)
					{
					case SET_CHARA_MODE.MAIN:
						if (num >= 0)
						{
							if (num3 == num)
							{
								num5 = num4;
							}
							num4 = num;
							flag = true;
						}
						break;
					case SET_CHARA_MODE.SUB:
						if (num >= 0)
						{
							if (num2 == num)
							{
								num4 = num5;
							}
							num5 = num;
							flag = true;
						}
						break;
					case SET_CHARA_MODE.CHANGE:
						if (num3 >= 0)
						{
							num4 = num3;
							num5 = num2;
							flag = true;
						}
						break;
					}
					if (flag)
					{
						loggedInServerInterface.RequestServerChangeCharacter(num4, num5, base.gameObject);
					}
				}
			}
		}
		else
		{
			GeneralUtil.ShowNoCommunication();
		}
		return flag;
	}

	public bool SetSort(ServerPlayerState.CHARA_SORT sort)
	{
		bool result = false;
		if (m_sortType != sort)
		{
			m_sortType = sort;
			m_sortOffset = 0;
			result = true;
		}
		SetParam(false);
		SetObject(false);
		return result;
	}

	public bool SetSortDebug(ServerPlayerState.CHARA_SORT sort)
	{
		bool result = false;
		if (m_sortType == sort)
		{
			m_sortOffset++;
		}
		else
		{
			m_sortType = sort;
			m_sortOffset = 0;
			result = true;
		}
		SetParam(false);
		SetObject(false);
		return result;
	}

	public bool SetDeck(int stock)
	{
		bool result = false;
		if (GeneralUtil.IsNetwork())
		{
			if (stock >= 0 && m_currentDeck != stock && m_deckList != null && m_deckList.Count > stock)
			{
				DeckSetLoad(stock);
			}
			else
			{
				Debug.Log("SetDeck error   currentDeck:" + m_currentDeck + "  stock:" + stock);
			}
		}
		else
		{
			GeneralUtil.ShowNoCommunication();
		}
		return result;
	}

	private void DeckSetLoad(int stock)
	{
		m_oldDeckSet = GetCurrentDeck();
		m_reqCharaMain = m_deckList[stock].charaMain;
		m_reqCharaSub = m_deckList[stock].charaSub;
		m_reqChaoMain = m_deckList[stock].chaoMain;
		m_reqChaoSub = m_deckList[stock].chaoSub;
		m_reqDeck = stock;
		DeckUtil.SetDeckCurrentStockIndex(m_reqDeck);
		if (m_oldDeckSet.charaMain == m_reqCharaMain && m_oldDeckSet.charaSub == m_reqCharaSub && m_oldDeckSet.chaoMain == m_reqChaoMain && m_oldDeckSet.chaoSub == m_reqChaoSub)
		{
			m_oldDeckSet = null;
			m_reqDeck = -1;
			m_reqCharaMain = CharaType.UNKNOWN;
			m_reqCharaSub = CharaType.UNKNOWN;
			m_reqChaoMain = -1;
			m_reqChaoSub = -1;
			UpdateView(true);
		}
		else
		{
			ServerInterface loggedInServerInterface = ServerInterface.LoggedInServerInterface;
			if (m_oldDeckSet.chaoMain != m_reqChaoMain || m_oldDeckSet.chaoSub != m_reqChaoSub)
			{
				int id = (int)ServerItem.CreateFromChaoId(m_reqChaoMain).id;
				int id2 = (int)ServerItem.CreateFromChaoId(m_reqChaoSub).id;
				m_changeDelay = 0f;
				loggedInServerInterface.RequestServerEquipChao(id, id2, base.gameObject);
			}
			else
			{
				ServerEquipChao_Dummy();
			}
		}
	}

	private DeckUtil.DeckSet GetCurrentDeck()
	{
		DeckUtil.DeckSet result = null;
		if (m_deckList != null && m_deckList.Count > 0 && m_deckList.Count > m_currentDeck)
		{
			result = m_deckList[m_currentDeck];
		}
		return result;
	}

	private void SetParam(bool reset)
	{
		if (reset && m_charaObjectList != null)
		{
			m_charaObjectList.Clear();
		}
		if (m_charaStateList != null)
		{
			m_charaStateList.Clear();
		}
		ServerPlayerState playerState = ServerInterface.PlayerState;
		m_charaStateList = playerState.GetCharacterStateList(m_sortType, false, m_sortOffset);
		m_pageMax = Mathf.CeilToInt((float)m_charaStateList.Count / 4f);
		GeneralUtil.SetButtonFunc(base.gameObject, "player_set_grip_R", base.gameObject, "OnClickPageNext");
		GeneralUtil.SetButtonFunc(base.gameObject, "Btn_player_set_grip_R", base.gameObject, "OnClickPageNext");
		GeneralUtil.SetButtonFunc(base.gameObject, "player_set_grip_L", base.gameObject, "OnClickPagePrev");
		GeneralUtil.SetButtonFunc(base.gameObject, "Btn_player_set_grip_L", base.gameObject, "OnClickPagePrev");
	}

	private void SetObject(bool reset)
	{
		if (m_storage != null)
		{
			if (reset)
			{
				m_storage.maxItemCount = (m_storage.maxColumns = 0);
				m_storage.maxRows = 1;
				m_storage.Restart();
			}
			List<CharaType> list = new List<CharaType>();
			int num = 4 * m_page;
			int num2 = 0;
			if (m_charaStateList.Count > num)
			{
				num2 = m_charaStateList.Count - num;
				if (num2 > 4)
				{
					num2 = 4;
				}
			}
			m_storage.maxItemCount = (m_storage.maxColumns = num2);
			m_storage.Restart();
			m_charaObjectList = GameObjectUtil.FindChildGameObjectsComponents<ui_player_set_scroll>(m_storage.gameObject, "ui_player_set_scroll(Clone)");
			if (m_charaObjectList != null && m_charaObjectList.Count > 0)
			{
				ServerPlayerState playerState = ServerInterface.PlayerState;
				m_charaStateList = playerState.GetCharacterStateList(m_sortType, false, m_sortOffset);
				int num3 = 0;
				Dictionary<CharaType, ServerCharacterState>.KeyCollection keys = m_charaStateList.Keys;
				foreach (CharaType item in keys)
				{
					ServerCharacterState characterState = m_charaStateList[item];
					if (num3 >= num && num3 < num + num2 && m_charaObjectList.Count > num3 - num)
					{
						list.Add(item);
						m_charaObjectList[num3 - num].Setup(this, characterState);
					}
					num3++;
				}
			}
			if (list.Count > 0)
			{
				GeneralUtil.RemoveCharaTexture(list);
			}
		}
		DeckUtil.DeckSet currentDeck = GetCurrentDeck();
		if (m_charaDeckObject != null)
		{
			UISprite uISprite = GameObjectUtil.FindChildGameObjectComponent<UISprite>(m_charaDeckObject, "img_player_main");
			UISprite uISprite2 = GameObjectUtil.FindChildGameObjectComponent<UISprite>(m_charaDeckObject, "img_player_sub");
			UISprite uISprite3 = GameObjectUtil.FindChildGameObjectComponent<UISprite>(m_charaDeckObject, "img_decknum");
			if (uISprite != null)
			{
				uISprite.spriteName = "ui_tex_player_set_" + CharaTypeUtil.GetCharaSpriteNameSuffix(currentDeck.charaMain);
				uISprite.gameObject.SetActive(currentDeck.charaMain != CharaType.UNKNOWN);
			}
			if (uISprite2 != null)
			{
				uISprite2.spriteName = "ui_tex_player_set_" + CharaTypeUtil.GetCharaSpriteNameSuffix(currentDeck.charaSub);
				uISprite2.gameObject.SetActive(currentDeck.charaMain != CharaType.UNKNOWN);
			}
			if (uISprite3 != null)
			{
				uISprite3.spriteName = "ui_chao_set_deck_tab_" + (m_currentDeck + 1);
			}
			GeneralUtil.SetButtonFunc(m_charaDeckObject, "Btn_player_change", base.gameObject, "OnClickChange");
		}
		if (m_chaoDeckObject != null && reset)
		{
			UITexture uITexture = GameObjectUtil.FindChildGameObjectComponent<UITexture>(m_chaoDeckObject, "img_chao_main");
			UITexture uITexture2 = GameObjectUtil.FindChildGameObjectComponent<UITexture>(m_chaoDeckObject, "img_chao_sub");
			if (ChaoTextureManager.Instance != null)
			{
				if (uITexture != null)
				{
					uITexture.gameObject.SetActive(currentDeck.chaoMain >= 0);
					if (currentDeck.chaoMain >= 0)
					{
						ChaoTextureManager.CallbackInfo info = new ChaoTextureManager.CallbackInfo(uITexture, null, true);
						ChaoTextureManager.Instance.GetTexture(currentDeck.chaoMain, info);
					}
				}
				if (uITexture2 != null)
				{
					uITexture2.gameObject.SetActive(currentDeck.chaoSub >= 0);
					if (currentDeck.chaoSub >= 0)
					{
						ChaoTextureManager.CallbackInfo info2 = new ChaoTextureManager.CallbackInfo(uITexture2, null, true);
						ChaoTextureManager.Instance.GetTexture(currentDeck.chaoSub, info2);
					}
				}
			}
			else
			{
				if (uITexture != null)
				{
					uITexture.gameObject.SetActive(false);
				}
				if (uITexture2 != null)
				{
					uITexture2.gameObject.SetActive(false);
				}
			}
			GeneralUtil.SetButtonFunc(base.gameObject, m_chaoDeckObject.name, base.gameObject, "OnClickChao");
		}
		UILabel uILabel = GameObjectUtil.FindChildGameObjectComponent<UILabel>(base.gameObject, "Lbl_page");
		if (uILabel != null)
		{
			uILabel.text = m_page + 1 + "/" + m_pageMax;
		}
		if (m_changeDelay <= 0f && reset)
		{
			m_changeBtn = GameObjectUtil.FindChildGameObjectComponent<UIImageButton>(base.gameObject, "Btn_player_change");
			if (m_changeBtn != null)
			{
				if (CheckSetMode(SET_CHARA_MODE.CHANGE))
				{
					m_changeBtn.isEnabled = true;
					m_changeDelay = 1.5f;
				}
				else
				{
					m_changeBtn.isEnabled = false;
					m_changeDelay = -1f;
				}
			}
		}
		GeneralUtil.SetCharasetBtnIcon(base.gameObject);
		GeneralUtil.SetButtonFunc(base.gameObject, "Btn_charaset", base.gameObject, "OnClickDeck");
	}

	private void OnMsgDeckViewWindowChange()
	{
		UpdateView(true);
	}

	private void OnMsgDeckViewWindowNotChange()
	{
		UpdateView(true);
	}

	private void OnMsgDeckViewWindowNetworkError()
	{
		UpdateView();
	}

	private void OnClickBack()
	{
		if (base.gameObject.activeSelf && !m_isEnd)
		{
			SoundManager.SePlay("sys_menu_decide");
			m_isEnd = true;
		}
	}

	private void OnClickDeck()
	{
		SoundManager.SePlay("sys_menu_decide");
		DeckViewWindow.Create(base.gameObject);
	}

	private void OnClickChao()
	{
		HudMenuUtility.SendMenuButtonClicked(ButtonInfoTable.ButtonType.CHAO, true);
		SoundManager.SePlay("sys_menu_decide");
	}

	private void OnClickChange()
	{
		SetChara(SET_CHARA_MODE.CHANGE);
		if (m_changeBtn != null)
		{
			m_changeBtn.isEnabled = false;
			m_changeDelay = 1.5f;
		}
		SoundManager.SePlay("sys_menu_decide");
	}

	private void OnClickPageNext()
	{
		if (m_pageMax > 0)
		{
			m_page = (m_page + 1 + m_pageMax) % m_pageMax;
			SetObject(false);
			SoundManager.SePlay("sys_menu_decide");
		}
	}

	private void OnClickPagePrev()
	{
		if (m_pageMax > 0)
		{
			m_page = (m_page - 1 + m_pageMax) % m_pageMax;
			SetObject(false);
			SoundManager.SePlay("sys_menu_decide");
		}
	}

	public void OnClosedWindowAnim()
	{
		if (m_isEnd)
		{
			base.gameObject.SetActive(false);
		}
	}

	public void OnClickBackButton()
	{
		if (base.gameObject.activeSelf && !m_isEnd)
		{
			m_isEnd = true;
			SoundManager.SePlay("sys_menu_decide");
			ActiveAnimation activeAnimation = ActiveAnimation.Play(m_animation, "ui_mm_player_set_2_intro_Anim", Direction.Reverse);
			if (activeAnimation != null)
			{
				EventDelegate.Add(activeAnimation.onFinished, OnClosedWindowAnim, true);
			}
		}
	}

	private void ServerChangeCharacter_Succeeded(MsgGetPlayerStateSucceed msg)
	{
		m_oldDeckSet = null;
		m_reqDeck = -1;
		m_reqCharaMain = CharaType.UNKNOWN;
		m_reqCharaSub = CharaType.UNKNOWN;
		m_reqChaoMain = -1;
		m_reqChaoSub = -1;
		UpdateView(true);
		HudMenuUtility.SendMsgUpdateSaveDataDisplay();
	}

	private void ServerChangeCharacter_Dummy()
	{
		m_oldDeckSet = null;
		m_reqDeck = -1;
		m_reqCharaMain = CharaType.UNKNOWN;
		m_reqCharaSub = CharaType.UNKNOWN;
		m_reqChaoMain = -1;
		m_reqChaoSub = -1;
		UpdateView(true);
		HudMenuUtility.SendMsgUpdateSaveDataDisplay();
	}

	private void ServerEquipChao_Succeeded(MsgGetPlayerStateSucceed msg)
	{
		ServerInterface loggedInServerInterface = ServerInterface.LoggedInServerInterface;
		bool flag = false;
		if (!(loggedInServerInterface != null))
		{
			return;
		}
		if (m_oldDeckSet.charaMain != CharaType.UNKNOWN && m_oldDeckSet.charaMain != m_reqCharaMain)
		{
			flag = true;
		}
		if (m_oldDeckSet.charaSub != m_reqCharaSub)
		{
			flag = true;
		}
		if (flag)
		{
			int mainCharaId = -1;
			int subCharaId = -1;
			ServerCharacterState serverCharacterState = ServerInterface.PlayerState.CharacterState(m_reqCharaMain);
			ServerCharacterState serverCharacterState2 = ServerInterface.PlayerState.CharacterState(m_reqCharaSub);
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
		if (m_oldDeckSet.charaMain != CharaType.UNKNOWN && m_oldDeckSet.charaMain != m_reqCharaMain)
		{
			flag = true;
		}
		if (m_oldDeckSet.charaSub != m_reqCharaSub)
		{
			flag = true;
		}
		if (flag)
		{
			int mainCharaId = -1;
			int subCharaId = -1;
			ServerCharacterState serverCharacterState = ServerInterface.PlayerState.CharacterState(m_reqCharaMain);
			ServerCharacterState serverCharacterState2 = ServerInterface.PlayerState.CharacterState(m_reqCharaSub);
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

	public static void DebugShowPlayerCharaListGui()
	{
		if (!s_isActive || !(s_instance != null) || !s_instance.gameObject.activeSelf || PlayerLvupWindow.isActive || PlayerSetWindowUI.isActive || DeckViewWindow.isActive || GeneralWindow.Created)
		{
			return;
		}
		Rect rect = new Rect(170f, -5f, 150f, 90f);
		Rect rect2 = SingletonGameObject<DebugGameObject>.Instance.CreateGuiRect(rect, DebugGameObject.GUI_RECT_ANCHOR.CENTER_BOTTOM);
		GUI.Box(rect2, string.Concat("sort:", s_instance.sortType, "  [", s_instance.sortOffset, "]"));
		int num = 3;
		float num2 = 0.25f;
		float num3 = (1f - num2) / (float)num;
		for (int i = 0; i < num; i++)
		{
			Rect rect3 = new Rect(0f, num2 + num3 * (float)i, 0.95f, num3 * 0.95f);
			Rect position = SingletonGameObject<DebugGameObject>.Instance.CreateGuiRectInRate(rect2, rect3, DebugGameObject.GUI_RECT_ANCHOR.CENTER_TOP);
			ServerPlayerState.CHARA_SORT cHARA_SORT = (ServerPlayerState.CHARA_SORT)i;
			if (GUI.Button(position, string.Empty + cHARA_SORT))
			{
				s_instance.SetSortDebug(cHARA_SORT);
			}
		}
	}
}
