using DataTable;
using Message;
using SaveData;
using System;
using System.Collections;
using System.Collections.Generic;
using Text;
using UnityEngine;

public class ChaoSetUI : MonoBehaviour
{
	public class SaveDataInterface
	{
		public static int MainChaoId
		{
			get
			{
				SaveDataManager instance = SaveDataManager.Instance;
				if (instance != null)
				{
					return instance.PlayerData.MainChaoID;
				}
				return -1;
			}
			set
			{
				SaveDataManager instance = SaveDataManager.Instance;
				if (instance != null)
				{
					instance.PlayerData.MainChaoID = value;
				}
			}
		}

		public static int SubChaoId
		{
			get
			{
				SaveDataManager instance = SaveDataManager.Instance;
				if (instance != null)
				{
					return instance.PlayerData.SubChaoID;
				}
				return -1;
			}
			set
			{
				SaveDataManager instance = SaveDataManager.Instance;
				if (instance != null)
				{
					instance.PlayerData.SubChaoID = value;
				}
			}
		}
	}

	[Serializable]
	private class ChaoSerializeFields
	{
		[SerializeField]
		public UISprite m_chaoSprite;

		[SerializeField]
		public UITexture m_chaoTexture;

		[SerializeField]
		public UISprite m_chaoRankSprite;

		[SerializeField]
		public UILabel m_chaoNameLabel;

		[SerializeField]
		public UILabel m_chaoLevelLabel;

		[SerializeField]
		public UISprite m_chaoTypeSprite;

		[SerializeField]
		public UISprite m_bonusTypeSprite;

		[SerializeField]
		public UILabel m_bonusLabel;
	}

	[Serializable]
	private class RouletteButtonUI
	{
		[SerializeField]
		public GameObject m_alertBadgeGameObject;

		[SerializeField]
		public GameObject m_eqqBadgeGameObject;

		[SerializeField]
		public GameObject m_spinCountBadgeGameObject;

		[SerializeField]
		public UILabel m_spinCountLabel;

		public void Setup()
		{
			ServerInterface loggedInServerInterface = ServerInterface.LoggedInServerInterface;
			if (loggedInServerInterface != null)
			{
				m_alertBadgeGameObject.SetActive(ServerInterface.CampaignState.InAnyIdSession(Constants.Campaign.emType.ChaoRouletteCost));
				int num = 0;
				if (RouletteManager.Instance != null)
				{
					num = RouletteManager.Instance.specialEgg;
				}
				m_eqqBadgeGameObject.SetActive(num >= 10);
				m_spinCountBadgeGameObject.SetActive(ServerInterface.WheelOptions.m_numRemaining > 0);
				m_spinCountLabel.text = ServerInterface.WheelOptions.m_numRemaining.ToString();
				m_spinCountBadgeGameObject.SetActive(false);
			}
		}
	}

	private const int MAX_COLUMNS = 4;

	private const float SORT_DELAY = 0.4f;

	private const float INIT_DELAY = 0.2f;

	[SerializeField]
	private bool isDebugRondomSetChao;

	[SerializeField]
	private int[] m_chaoCountNumber = new int[3];

	[SerializeField]
	private UILabel m_getChaoCountLabel;

	[SerializeField]
	private UILabel m_getChaoCountShadowLabel;

	[SerializeField]
	private UISprite m_getChaoSprite;

	[SerializeField]
	private UILabel m_getChaoBonusLabel;

	[SerializeField]
	private ChaoSerializeFields[] m_chaosSerializeFields = new ChaoSerializeFields[2];

	[SerializeField]
	private RouletteButtonUI m_rouletteButtonUI;

	[SerializeField]
	private UISprite m_sortLeveUp;

	[SerializeField]
	private UISprite m_sortRareUp;

	[SerializeField]
	private GameObject m_specialEggIconObj;

	[SerializeField]
	private GameObject m_freeSpinIconObj;

	[SerializeField]
	private UILabel m_freeSpinCountLabel;

	[SerializeField]
	private GameObject m_saleIconObj;

	[SerializeField]
	private GameObject m_eventIconObj;

	public static readonly string[] s_chaoBonusTypeSpriteNameSuffixs = new string[5]
	{
		"score",
		"ring",
		"rsr",
		"animal",
		"range"
	};

	private GameObject m_slot;

	private UIRectItemStorage m_uiRectItemStorage;

	private UIDraggablePanel m_uiDraggablePanel;

	private List<GameObject> m_cells;

	private StageAbilityManager m_stageAbilityManager;

	private ChaoSort m_lastSort;

	private int m_currentDeckSetStock;

	private int m_sortCount;

	private List<GameObject> m_deckObjects;

	private List<DataTable.ChaoData> m_chaoDatas;

	private float m_sortDelay;

	private BoxCollider m_sortLeveUpBC;

	private BoxCollider m_sortRareUpBC;

	private UISprite m_mask0;

	private UISprite m_mask1;

	private ChaoSort m_chaoSort;

	private bool m_tutorial;

	private bool m_descendingOrder;

	private bool m_initEnd;

	private bool m_endSetUp;

	private float m_initDelay;

	public bool IsEndSetup
	{
		get
		{
			return m_endSetUp;
		}
	}

	private void Update()
	{
		if (m_tutorial && GeneralWindow.IsCreated("ChaoTutorial") && GeneralWindow.IsButtonPressed)
		{
			GeneralWindow.Close();
			TutorialCursor.StartTutorialCursor(TutorialCursor.Type.BACK);
		}
		if (GeneralWindow.IsCreated("ChaoCantSet") && GeneralWindow.IsButtonPressed)
		{
			GeneralWindow.Close();
		}
		if (m_sortDelay > 0f)
		{
			m_sortDelay -= Time.deltaTime;
			if (m_sortDelay <= 0f)
			{
				m_sortDelay = 0f;
				if (m_sortLeveUpBC != null)
				{
					m_sortLeveUpBC.enabled = true;
				}
				if (m_sortRareUpBC != null)
				{
					m_sortRareUpBC.enabled = true;
				}
			}
		}
		if (m_initDelay > 0f)
		{
			m_initDelay -= Time.deltaTime;
			if (m_initDelay < 0.5f)
			{
				m_endSetUp = true;
			}
			if (m_initDelay <= 0f)
			{
				m_endSetUp = true;
				m_initDelay = 0f;
				HudMenuUtility.SetConnectAlertSimpleUI(false);
			}
		}
	}

	private void OnStartChaoSet()
	{
		HudMenuUtility.SetConnectAlertSimpleUI(true);
		m_initDelay = 0f;
		m_currentDeckSetStock = DeckUtil.GetDeckCurrentStockIndex();
		m_endSetUp = false;
		m_tutorial = IsChaoTutorial();
		if (m_tutorial)
		{
			SaveDataInterface.MainChaoId = -1;
			SaveDataInterface.SubChaoId = -1;
		}
		if (isDebugRondomSetChao)
		{
			SaveDataManager instance = SaveDataManager.Instance;
			if (instance != null)
			{
				DataTable.ChaoData[] dataTable = ChaoTable.GetDataTable();
				if (dataTable != null)
				{
					instance.ChaoData.Info = new SaveData.ChaoData.ChaoDataInfo[dataTable.Length];
					instance.PlayerData.MainChaoID = -1;
					instance.PlayerData.SubChaoID = -1;
					for (int i = 0; i < dataTable.Length; i++)
					{
						int id = dataTable[i].id;
						int num = UnityEngine.Random.Range(-1, 11);
						instance.ChaoData.Info[i].chao_id = id;
						instance.ChaoData.Info[i].level = num;
						if (instance.PlayerData.MainChaoID == -1 && num != -1)
						{
							instance.PlayerData.MainChaoID = id;
						}
					}
				}
			}
		}
		m_rouletteButtonUI.Setup();
		m_stageAbilityManager = GameObjectUtil.FindGameObjectComponent<StageAbilityManager>("StageAbilityManager");
		if (m_stageAbilityManager != null)
		{
			m_stageAbilityManager.RecalcAbilityVaue();
		}
		m_slot = GameObjectUtil.FindChildGameObject(base.gameObject, "slot");
		m_uiRectItemStorage = m_slot.GetComponent<UIRectItemStorage>();
		m_uiDraggablePanel = GameObjectUtil.FindChildGameObjectComponent<UIDraggablePanel>(base.gameObject, "chao_set_contents");
		SetRouletteButton();
		GeneralUtil.SetCharasetBtnIcon(base.gameObject);
		GeneralUtil.SetButtonFunc(base.gameObject, "Btn_charaset", base.gameObject, "OnClickDeck");
		UIButtonMessage uIButtonMessage = GameObjectUtil.FindChildGameObjectComponent<UIButtonMessage>(base.gameObject, "Btn_player");
		if (uIButtonMessage != null)
		{
			uIButtonMessage.target = base.gameObject;
			uIButtonMessage.functionName = "GoToCharacterButtonClicked";
		}
		StartCoroutine(InitView());
	}

	private IEnumerator InitView()
	{
		while (GameObjectUtil.FindChildGameObjects(m_slot, "ui_chao_set_cell(Clone)").Count == 0)
		{
			yield return null;
		}
		m_currentDeckSetStock = DeckUtil.GetDeckCurrentStockIndex();
		SetupTabView();
		m_uiRectItemStorage.maxColumns = 4;
		int maxChaoCount = 0;
		if (ServerInterface.PlayerState != null && ServerInterface.PlayerState.ChaoStates != null)
		{
			maxChaoCount = ServerInterface.PlayerState.ChaoStates.Count;
		}
		if (maxChaoCount == 0)
		{
			maxChaoCount = ChaoTable.GetDataTable().Length;
		}
		m_uiRectItemStorage.maxRows = (maxChaoCount + m_uiRectItemStorage.maxColumns - 1) / m_uiRectItemStorage.maxColumns;
		m_uiRectItemStorage.maxItemCount = maxChaoCount;
		m_uiRectItemStorage.Restart();
		m_uiRectItemStorage.Strip();
		if (m_initEnd)
		{
			ChaoSortUpadate(m_chaoSort, m_descendingOrder);
		}
		else
		{
			m_mask0 = GameObjectUtil.FindChildGameObjectComponent<UISprite>(base.gameObject, "img_mask_0_bg");
			m_mask1 = GameObjectUtil.FindChildGameObjectComponent<UISprite>(base.gameObject, "img_mask_1_bg");
			m_sortLeveUpBC = GameObjectUtil.FindChildGameObjectComponent<BoxCollider>(base.gameObject, "sort_1");
			m_sortRareUpBC = GameObjectUtil.FindChildGameObjectComponent<BoxCollider>(base.gameObject, "sort_0");
			m_mask0.alpha = 0f;
			m_mask1.alpha = 1f;
			m_sortLeveUp.alpha = 0f;
			m_sortRareUp.alpha = 0f;
			m_chaoSort = ChaoSort.RARE;
			m_descendingOrder = false;
			if (m_tutorial)
			{
				m_mask0.alpha = 1f;
				m_mask1.alpha = 0f;
				m_sortLeveUp.alpha = 1f;
				m_sortRareUp.alpha = 1f;
				m_chaoSort = ChaoSort.LEVEL;
				m_descendingOrder = true;
				ChaoSortUpadate(m_chaoSort, m_descendingOrder);
			}
			else
			{
				SystemSaveManager systemSaveManager = SystemSaveManager.Instance;
				if (systemSaveManager != null)
				{
					SystemData saveData = systemSaveManager.GetSystemdata();
					if (saveData != null)
					{
						m_chaoSort = (ChaoSort)saveData.chaoSortType01;
						m_descendingOrder = (saveData.chaoSortType02 > 0);
						if (m_descendingOrder)
						{
							m_sortLeveUp.alpha = 1f;
							m_sortRareUp.alpha = 1f;
						}
						if (m_chaoSort == ChaoSort.LEVEL)
						{
							m_mask0.alpha = 1f;
							m_mask1.alpha = 0f;
						}
					}
				}
				ChaoSortUpadate(m_chaoSort, m_descendingOrder);
			}
			m_initEnd = true;
		}
		if (m_sortLeveUpBC != null)
		{
			m_sortLeveUpBC.enabled = true;
		}
		if (m_sortRareUpBC != null)
		{
			m_sortRareUpBC.enabled = true;
		}
		m_sortDelay = 0f;
		m_initDelay = 0.2f;
		UpdateView();
	}

	private void SetRouletteButton()
	{
		HudRouletteButtonUtil.SetSpecialEggIcon(m_specialEggIconObj);
		HudRouletteButtonUtil.SetFreeSpin(m_freeSpinIconObj, m_freeSpinCountLabel);
		HudRouletteButtonUtil.SetSaleIcon(m_saleIconObj);
		HudRouletteButtonUtil.SetEventIcon(m_eventIconObj);
	}

	private void SetupTabView()
	{
		GameObject gameObject = GameObjectUtil.FindChildGameObject(base.gameObject, "window_chao_tab");
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

	private void UpdateView()
	{
		StartCoroutine(UpdateViewCoroutine());
	}

	private IEnumerator UpdateViewCoroutine()
	{
		m_cells = GameObjectUtil.FindChildGameObjects(m_slot, "ui_chao_set_cell(Clone)");
		if (m_cells.Count != m_uiRectItemStorage.maxItemCount)
		{
			yield return null;
		}
		if (m_stageAbilityManager != null)
		{
			m_getChaoCountLabel.text = m_stageAbilityManager.GetChaoCount().ToString();
			m_getChaoCountShadowLabel.text = m_stageAbilityManager.GetChaoCount().ToString();
			int chaoCountIndex;
			for (chaoCountIndex = 0; chaoCountIndex < m_chaoCountNumber.Length && m_stageAbilityManager.GetChaoCount() >= m_chaoCountNumber[chaoCountIndex]; chaoCountIndex++)
			{
			}
			m_getChaoSprite.spriteName = "ui_chao_set_dec_" + chaoCountIndex;
			m_getChaoBonusLabel.text = HudUtility.GetChaoCountBonusText(m_stageAbilityManager.GetChaoCountBonusValue());
		}
		RegistChao(0, SaveDataInterface.MainChaoId);
		RegistChao(1, SaveDataInterface.SubChaoId);
		UpdateRegistedChaoView();
		UpdateGotChaoView();
		if (m_tutorial)
		{
			TutorialCursor.StartTutorialCursor(TutorialCursor.Type.CHAOSELECT_CHAO);
		}
	}

	private void SetCellChao(int cellIndex, DataTable.ChaoData chaoData, int mainChaoId, int subChaoId)
	{
		if (m_chaoDatas == null)
		{
			return;
		}
		ui_chao_set_cell[] componentsInChildren = base.gameObject.GetComponentsInChildren<ui_chao_set_cell>(true);
		if (cellIndex >= 0 && cellIndex < componentsInChildren.Length)
		{
			if (m_uiDraggablePanel == null)
			{
				m_uiDraggablePanel = GameObjectUtil.FindChildGameObjectComponent<UIDraggablePanel>(base.gameObject, "chao_set_contents");
			}
			if (m_uiDraggablePanel != null)
			{
				componentsInChildren[cellIndex].UpdateView(chaoData.id, mainChaoId, subChaoId, m_uiDraggablePanel.panel);
			}
		}
	}

	public void RegistChao(int chaoMainSubIndex, int chaoId)
	{
		base.enabled = true;
		int num = -1;
		int num2 = -1;
		bool flag = false;
		bool flag2 = false;
		if (chaoMainSubIndex == 0)
		{
			if (SaveDataInterface.SubChaoId == chaoId)
			{
				num2 = SaveDataInterface.MainChaoId;
				flag2 = true;
			}
			if (SaveDataInterface.MainChaoId != chaoId)
			{
				num = chaoId;
				flag = true;
			}
		}
		else
		{
			if (SaveDataInterface.MainChaoId == chaoId)
			{
				num = SaveDataInterface.SubChaoId;
				flag = true;
			}
			if (SaveDataInterface.SubChaoId != chaoId)
			{
				num2 = chaoId;
				flag2 = true;
			}
		}
		ServerInterface loggedInServerInterface = ServerInterface.LoggedInServerInterface;
		if (loggedInServerInterface != null && m_initDelay <= 0f)
		{
			if (flag || flag2)
			{
				loggedInServerInterface.RequestServerEquipChao((int)ServerItem.CreateFromChaoId((!flag) ? SaveDataInterface.MainChaoId : num).id, (int)ServerItem.CreateFromChaoId((!flag2) ? SaveDataInterface.SubChaoId : num2).id, base.gameObject);
			}
			return;
		}
		if (flag)
		{
			SaveDataInterface.MainChaoId = num;
		}
		if (flag2)
		{
			SaveDataInterface.SubChaoId = num2;
		}
		if (flag || flag2)
		{
			UpdateRegistedChaoView();
			UpdateGotChaoView();
		}
	}

	private void ServerEquipChao_Succeeded(MsgGetPlayerStateSucceed msg)
	{
		Debug.Log("ServerEquipChao_Succeeded  mainCharaId:" + msg.m_playerState.m_mainCharaId + "  subCharaId:" + msg.m_playerState.m_subCharaId);
		NetUtil.SyncSaveDataAndDataBase(msg.m_playerState);
		UpdateRegistedChaoView();
		UpdateGotChaoView();
		HudMenuUtility.SendMsgUpdateSaveDataDisplay();
	}

	private void ServerEquipChao_Failed(MsgServerConnctFailed msg)
	{
	}

	private void ServerEquipChao_Dummy()
	{
		UpdateRegistedChaoView();
		UpdateGotChaoView();
		HudMenuUtility.SendMsgUpdateSaveDataDisplay();
	}

	private void UpdateRegistedChaoView()
	{
		GeneralUtil.SetCharasetBtnIcon(base.gameObject);
		UpdateRegistedChaoView(0, ChaoTable.GetChaoData(SaveDataInterface.MainChaoId));
		UpdateRegistedChaoView(1, ChaoTable.GetChaoData(SaveDataInterface.SubChaoId));
	}

	public void UpdateRegistedChaoView(int chaoMainSubIndex, DataTable.ChaoData chaoData)
	{
		ChaoSerializeFields chaoSerializeFields = m_chaosSerializeFields[chaoMainSubIndex];
		if (chaoData != null && chaoData.IsValidate)
		{
			ChaoTextureManager.CallbackInfo info = new ChaoTextureManager.CallbackInfo(chaoSerializeFields.m_chaoTexture, null, true);
			ChaoTextureManager.Instance.GetTexture(chaoData.id, info);
			chaoSerializeFields.m_chaoTexture.enabled = true;
			chaoSerializeFields.m_chaoSprite.enabled = false;
			chaoSerializeFields.m_chaoRankSprite.spriteName = "ui_chao_set_bg_l_" + (int)chaoData.rarity;
			chaoSerializeFields.m_chaoNameLabel.text = chaoData.nameTwolines;
			chaoSerializeFields.m_chaoLevelLabel.text = TextUtility.GetTextLevel(chaoData.level.ToString());
			string str = chaoData.charaAtribute.ToString().ToLower();
			chaoSerializeFields.m_chaoTypeSprite.spriteName = "ui_chao_set_type_icon_" + str;
			chaoSerializeFields.m_bonusLabel.gameObject.SetActive(false);
			chaoSerializeFields.m_bonusTypeSprite.gameObject.SetActive(false);
		}
		else
		{
			chaoSerializeFields.m_chaoTexture.enabled = false;
			chaoSerializeFields.m_chaoSprite.enabled = true;
			chaoSerializeFields.m_chaoRankSprite.spriteName = "ui_chao_set_bg_l_3";
			chaoSerializeFields.m_chaoNameLabel.text = string.Empty;
			chaoSerializeFields.m_chaoLevelLabel.text = string.Empty;
			chaoSerializeFields.m_chaoTypeSprite.spriteName = null;
			chaoSerializeFields.m_bonusTypeSprite.spriteName = null;
			chaoSerializeFields.m_bonusLabel.text = string.Empty;
		}
	}

	private void UpdateGotChaoView()
	{
		int num = 0;
		if (m_chaoDatas != null && m_chaoDatas.Count > 0 && ServerInterface.PlayerState != null && ServerInterface.PlayerState.ChaoStates != null && ServerInterface.PlayerState.ChaoStates.Count > 0)
		{
			foreach (DataTable.ChaoData chaoData in m_chaoDatas)
			{
				int num2 = chaoData.id + 400000;
				foreach (ServerChaoState chaoState in ServerInterface.PlayerState.ChaoStates)
				{
					if (num2 == chaoState.Id)
					{
						SetCellChao(num, chaoData, SaveDataInterface.MainChaoId, SaveDataInterface.SubChaoId);
						num++;
						break;
					}
				}
			}
			return;
		}
		ChaoSortUpadate(ChaoSort.RARE);
		if (m_chaoDatas == null)
		{
			return;
		}
		foreach (DataTable.ChaoData chaoData2 in m_chaoDatas)
		{
			SetCellChao(num, chaoData2, SaveDataInterface.MainChaoId, SaveDataInterface.SubChaoId);
			num++;
		}
	}

	private void ChaoSortUpadate(ChaoSort sort, bool down = false, DataTable.ChaoData.Rarity exclusion = DataTable.ChaoData.Rarity.NONE)
	{
		DataTable.ChaoData[] dataTable = ChaoTable.GetDataTable();
		ChaoDataSorting chaoDataSorting = new ChaoDataSorting(sort);
		if (chaoDataSorting == null)
		{
			return;
		}
		ChaoDataVisitorBase visitor = chaoDataSorting.visitor;
		if (visitor != null)
		{
			if (m_lastSort != sort)
			{
				m_lastSort = sort;
				m_sortCount = 0;
			}
			else
			{
				m_sortCount++;
			}
			DataTable.ChaoData[] array = dataTable;
			foreach (DataTable.ChaoData chaoData in array)
			{
				chaoData.accept(ref visitor);
			}
			switch (sort)
			{
			case ChaoSort.RARE:
			case ChaoSort.LEVEL:
				m_chaoDatas = chaoDataSorting.GetChaoListAll(down, exclusion);
				break;
			case ChaoSort.ATTRIBUTE:
			case ChaoSort.ABILITY:
			case ChaoSort.EVENT:
				m_chaoDatas = chaoDataSorting.GetChaoListAllOffset(m_sortCount);
				break;
			}
		}
	}

	private void OnClickDeck()
	{
		SoundManager.SePlay("sys_menu_decide");
		DeckViewWindow.Create(base.gameObject);
	}

	private void OnMsgDeckViewWindowChange()
	{
		OnStartChaoSet();
		Debug.Log("ChaoSetUI OnMsgDeckViewWindowChange!");
	}

	private void OnMsgDeckViewWindowNotChange()
	{
		OnStartChaoSet();
		Debug.Log("ChaoSetUI OnMsgDeckViewWindowNotChange!");
	}

	private void OnMsgDeckViewWindowNetworkError()
	{
		OnStartChaoSet();
		Debug.Log("ChaoSetUI OnMsgDeckViewWindowNetworkError!");
	}

	public void OnPressSortLevel()
	{
		if (m_sortLeveUpBC != null)
		{
			m_sortLeveUpBC.enabled = false;
		}
		if (m_sortRareUpBC != null)
		{
			m_sortRareUpBC.enabled = false;
		}
		m_sortDelay = 0.4f;
		if (m_chaoSort != ChaoSort.LEVEL)
		{
			m_descendingOrder = (m_sortLeveUp.alpha >= 0.9f);
			m_chaoSort = ChaoSort.LEVEL;
		}
		else
		{
			if (m_sortLeveUp.alpha >= 0.9f)
			{
				m_sortLeveUp.alpha = 0f;
			}
			else
			{
				m_sortLeveUp.alpha = 1f;
			}
			m_descendingOrder = !m_descendingOrder;
		}
		m_mask0.alpha = 1f;
		m_mask1.alpha = 0f;
		ChaoSortUpadate(m_chaoSort, m_descendingOrder);
		UpdateGotChaoView();
		SoundManager.SePlay("sys_menu_decide");
	}

	public void OnPressSortRare()
	{
		if (m_sortLeveUpBC != null)
		{
			m_sortLeveUpBC.enabled = false;
		}
		if (m_sortRareUpBC != null)
		{
			m_sortRareUpBC.enabled = false;
		}
		m_sortDelay = 0.4f;
		if (m_chaoSort != 0)
		{
			m_descendingOrder = (m_sortRareUp.alpha >= 0.9f);
			m_chaoSort = ChaoSort.RARE;
		}
		else
		{
			if (m_sortRareUp.alpha >= 0.9f)
			{
				m_sortRareUp.alpha = 0f;
			}
			else
			{
				m_sortRareUp.alpha = 1f;
			}
			m_descendingOrder = !m_descendingOrder;
		}
		m_mask0.alpha = 0f;
		m_mask1.alpha = 1f;
		ChaoSortUpadate(m_chaoSort, m_descendingOrder);
		UpdateGotChaoView();
		SoundManager.SePlay("sys_menu_decide");
	}

	private void OnSetChaoTexture(ChaoTextureManager.TextureData data)
	{
		if (SaveDataInterface.MainChaoId == data.chao_id)
		{
			m_chaosSerializeFields[0].m_chaoTexture.enabled = true;
			m_chaosSerializeFields[0].m_chaoTexture.mainTexture = data.tex;
		}
		else if (SaveDataInterface.SubChaoId == data.chao_id)
		{
			m_chaosSerializeFields[1].m_chaoTexture.enabled = true;
			m_chaosSerializeFields[1].m_chaoTexture.mainTexture = data.tex;
		}
	}

	private void OnClickChaoMain()
	{
		SoundManager.SePlay("sys_menu_decide");
		ChaoSetWindowUI window = ChaoSetWindowUI.GetWindow();
		if (window != null)
		{
			DataTable.ChaoData chaoData = ChaoTable.GetChaoData(SaveDataInterface.MainChaoId);
			if (chaoData != null)
			{
				ChaoSetWindowUI.ChaoInfo chaoInfo = new ChaoSetWindowUI.ChaoInfo(chaoData);
				chaoInfo.level = chaoData.level;
				chaoInfo.detail = chaoData.GetDetailLevelPlusSP(chaoInfo.level);
				window.OpenWindow(chaoInfo, ChaoSetWindowUI.WindowType.WINDOW_ONLY);
			}
		}
	}

	private void OnClickChaoSub()
	{
		SoundManager.SePlay("sys_menu_decide");
		ChaoSetWindowUI window = ChaoSetWindowUI.GetWindow();
		if (window != null)
		{
			DataTable.ChaoData chaoData = ChaoTable.GetChaoData(SaveDataInterface.SubChaoId);
			if (chaoData != null)
			{
				ChaoSetWindowUI.ChaoInfo chaoInfo = new ChaoSetWindowUI.ChaoInfo(chaoData);
				chaoInfo.level = chaoData.level;
				chaoInfo.detail = chaoData.GetDetailLevelPlusSP(chaoInfo.level);
				window.OpenWindow(chaoInfo, ChaoSetWindowUI.WindowType.WINDOW_ONLY);
			}
		}
	}

	private void GoToCharacterButtonClicked()
	{
		SoundManager.SePlay("sys_menu_decide");
		HudMenuUtility.SendMenuButtonClicked(ButtonInfoTable.ButtonType.CHARA_MAIN, true);
	}

	private void OnMsgMenuBack()
	{
		ui_chao_set_cell.ResetLastLoadTime();
		if (m_tutorial)
		{
			TutorialCursor.EndTutorialCursor(TutorialCursor.Type.BACK);
			HudMenuUtility.SendMsgUpdateSaveDataDisplay();
			m_tutorial = false;
		}
		ChaoTextureManager.Instance.RemoveChaoTexture();
		SystemSaveManager instance = SystemSaveManager.Instance;
		if (instance != null)
		{
			SystemData systemdata = instance.GetSystemdata();
			if (systemdata != null)
			{
				int num = m_descendingOrder ? 1 : 0;
				if (m_chaoSort != (ChaoSort)systemdata.chaoSortType01 || num != systemdata.chaoSortType02)
				{
					systemdata.chaoSortType01 = (int)m_chaoSort;
					systemdata.chaoSortType02 = num;
					instance.SaveSystemData();
				}
			}
		}
		m_endSetUp = false;
	}

	public void ChaoSetLoad(int stock)
	{
	}

	public static bool IsChaoTutorial()
	{
		return false;
	}
}
