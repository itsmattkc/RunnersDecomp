using DataTable;
using Message;
using System.Collections;
using System.Collections.Generic;
using Text;
using UnityEngine;

public class PresentBoxUI : MonoBehaviour
{
	public class PresentInfo
	{
		public int messageId = -1;

		public int itemId = -1;

		public int itemNum;

		public int expireTime;

		public ServerMessageEntry.MessageType messageType = ServerMessageEntry.MessageType.Unknown;

		public string name = string.Empty;

		public string infoText = string.Empty;

		public string fromId = string.Empty;

		public ServerItem serverItem;

		public Texture charaTex;

		public bool operatorFlag;

		public bool checkFlag = true;

		public PresentInfo(ServerMessageEntry msg)
		{
			if (msg != null)
			{
				messageId = msg.m_messageId;
				messageType = msg.m_messageType;
				itemId = msg.m_presentState.m_itemId;
				itemNum = msg.m_presentState.m_numItem;
				expireTime = msg.m_expireTiem;
				name = msg.m_name;
				fromId = msg.m_fromId;
				serverItem = new ServerItem((ServerItem.Id)itemId);
			}
		}

		public PresentInfo(ServerOperatorMessageEntry msg)
		{
			if (msg != null)
			{
				messageId = msg.m_messageId;
				itemId = msg.m_presentState.m_itemId;
				itemNum = msg.m_presentState.m_numItem;
				expireTime = msg.m_expireTiem;
				infoText = msg.m_content;
				serverItem = new ServerItem((ServerItem.Id)itemId);
				operatorFlag = true;
			}
		}
	}

	private const int DISPLAY_MAX_ITEM_COUNT = 10;

	[SerializeField]
	private UIRectItemStorage m_itemStorage;

	[SerializeField]
	private UIScrollBar m_scrollBar;

	[SerializeField]
	private UILabel m_recieveAllLabel;

	[SerializeField]
	private UILabel m_recieveSelectLabel;

	[SerializeField]
	private UILabel m_infoLabel;

	[SerializeField]
	private UILabel m_nextPageLabel;

	[SerializeField]
	private UILabel m_prevPageLabel;

	[SerializeField]
	private UILabel m_numPageLabel;

	[SerializeField]
	private GameObject m_recieveAllBtnObj;

	[SerializeField]
	private GameObject m_recieveSelectBtnObj;

	[SerializeField]
	private GameObject m_nextPageBtnObj;

	[SerializeField]
	private GameObject m_prevPageBtnObj;

	private int m_currentPageNum;

	private int m_totalPageCount;

	private bool m_init_flag;

	private bool m_outOfDataFlag;

	private bool m_setUp;

	private List<PresentInfo> m_presentInfoList = new List<PresentInfo>();

	private Dictionary<int, int> m_pageItemCountDic = new Dictionary<int, int>();

	private ChaoGetWindow m_charaGetWindow;

	private ChaoGetWindow m_chaoGetWindow;

	private ChaoMergeWindow m_chaoMergeWindow;

	private PlayerMergeWindow m_playerMergeWindow;

	public bool IsEndSetup
	{
		get
		{
			return m_setUp;
		}
	}

	private void Start()
	{
		ServerInterface loggedInServerInterface = ServerInterface.LoggedInServerInterface;
		if (loggedInServerInterface == null)
		{
			ServerInterface.DebugInit();
		}
		Initialize();
		base.enabled = false;
	}

	private void Update()
	{
	}

	private void Initialize()
	{
		if (!m_init_flag)
		{
			SetUIButtonMessage(m_recieveAllBtnObj, "OnClickedRecieveAllBtn");
			SetUIButtonMessage(m_recieveSelectBtnObj, "OnClickedRecieveSelectBtn");
			SetUIButtonMessage(m_nextPageBtnObj, "OnClickedNextPageBtn");
			SetUIButtonMessage(m_prevPageBtnObj, "OnClickedPrevPageBtnBtn");
			SetTextLabel(m_infoLabel, "information", null, null);
			SetTextLabel(m_recieveAllLabel, "recieve_all_item", null, null);
			SetTextLabel(m_recieveSelectLabel, "recieve_select_item", null, null);
			SetTextLabel(m_prevPageLabel, "prev_page", "{MAILE_COUNT}", 10.ToString());
			SetTextLabel(m_nextPageLabel, "next_page", "{MAILE_COUNT}", 10.ToString());
			m_init_flag = true;
		}
	}

	private void SetTextLabel(UILabel uiLabel, string cellId, string tagString, string replaceString)
	{
		if (uiLabel != null)
		{
			TextObject text = TextManager.GetText(TextManager.TextType.TEXTTYPE_COMMON_TEXT, "PresentBox", cellId);
			if (text != null)
			{
				text.ReplaceTag(tagString, replaceString);
				uiLabel.text = text.text;
			}
		}
	}

	private void SetUIButtonMessage(GameObject obj, string callBackName)
	{
		if (obj != null)
		{
			UIButtonMessage component = obj.GetComponent<UIButtonMessage>();
			if (component == null)
			{
				obj.AddComponent<UIButtonMessage>();
				component = obj.GetComponent<UIButtonMessage>();
			}
			if (component != null)
			{
				component.enabled = true;
				component.trigger = UIButtonMessage.Trigger.OnClick;
				component.target = base.gameObject;
				component.functionName = callBackName;
			}
		}
	}

	private void UpdatePage()
	{
		UpdateRectItemStorage();
		UpdateText();
		bool flag = m_totalPageCount > 1;
		SetEnableImageButton(m_nextPageBtnObj, flag);
		SetEnableImageButton(m_prevPageBtnObj, flag);
		bool flag2 = m_totalPageCount > 0;
		SetEnableImageButton(m_recieveAllBtnObj, flag2);
		SetEnableImageButton(m_recieveSelectBtnObj, flag2);
		if (m_scrollBar != null)
		{
			m_scrollBar.value = 0f;
		}
	}

	private void SetEnableImageButton(GameObject obj, bool flag)
	{
		if (obj != null)
		{
			UIImageButton component = obj.GetComponent<UIImageButton>();
			if (component != null)
			{
				component.isEnabled = flag;
			}
		}
	}

	private void UpdateText()
	{
		if (m_numPageLabel != null)
		{
			if (m_totalPageCount == 0)
			{
				m_numPageLabel.text = "0/0";
			}
			else
			{
				m_numPageLabel.text = 1 + m_currentPageNum + "/" + m_totalPageCount;
			}
		}
	}

	private void UpdateRectItemStorage()
	{
		if (!(m_itemStorage != null) || m_presentInfoList == null)
		{
			return;
		}
		if (m_totalPageCount == 0)
		{
			m_itemStorage.maxItemCount = 0;
			m_itemStorage.maxRows = 0;
			m_currentPageNum = 0;
			m_itemStorage.Restart();
			return;
		}
		m_currentPageNum = Mathf.Min(m_currentPageNum, m_totalPageCount - 1);
		m_currentPageNum = Mathf.Max(m_currentPageNum, 0);
		int num = Mathf.Clamp(m_pageItemCountDic[m_currentPageNum], 0, 10);
		m_itemStorage.maxItemCount = num;
		m_itemStorage.maxRows = num;
		m_itemStorage.Restart();
		ui_presentbox_scroll[] componentsInChildren = m_itemStorage.GetComponentsInChildren<ui_presentbox_scroll>(true);
		int num2 = componentsInChildren.Length;
		for (int i = 0; i < num; i++)
		{
			if (i < num2)
			{
				int index = i + m_currentPageNum * 10;
				componentsInChildren[i].UpdateView(m_presentInfoList[index]);
			}
		}
	}

	private void SetPresentBoxInfo()
	{
		if (m_presentInfoList == null)
		{
			return;
		}
		m_presentInfoList.Clear();
		m_pageItemCountDic.Clear();
		if (ServerInterface.MessageList != null)
		{
			List<ServerMessageEntry> messageList = ServerInterface.MessageList;
			foreach (ServerMessageEntry item3 in messageList)
			{
				if (PresentBoxUtility.IsWithinTimeLimit(item3.m_expireTiem))
				{
					PresentInfo item = new PresentInfo(item3);
					m_presentInfoList.Add(item);
				}
			}
		}
		if (ServerInterface.OperatorMessageList != null)
		{
			List<ServerOperatorMessageEntry> operatorMessageList = ServerInterface.OperatorMessageList;
			foreach (ServerOperatorMessageEntry item4 in operatorMessageList)
			{
				if (PresentBoxUtility.IsWithinTimeLimit(item4.m_expireTiem))
				{
					PresentInfo item2 = new PresentInfo(item4);
					m_presentInfoList.Add(item2);
				}
			}
		}
		m_totalPageCount = m_presentInfoList.Count / 10;
		int num = m_presentInfoList.Count % 10;
		if (num > 0)
		{
			m_totalPageCount++;
		}
		for (int i = 0; i < m_totalPageCount; i++)
		{
			if (i == m_totalPageCount - 1)
			{
				if (num == 0)
				{
					m_pageItemCountDic.Add(i, 10);
				}
				else
				{
					m_pageItemCountDic.Add(i, num);
				}
			}
			else
			{
				m_pageItemCountDic.Add(i, 10);
			}
		}
	}

	private void OnStartPresentBox()
	{
		m_outOfDataFlag = false;
		Initialize();
		SetPresentBoxInfo();
		UpdatePage();
		m_setUp = true;
	}

	private void OnEndPresentBox()
	{
		if (m_itemStorage != null)
		{
			m_itemStorage.maxItemCount = 0;
			m_itemStorage.maxRows = 0;
			m_itemStorage.Restart();
			m_presentInfoList.Clear();
			m_pageItemCountDic.Clear();
			GameObject gameObject = GameObject.Find("PresentBoxTextures");
			if (gameObject != null)
			{
				Object.Destroy(gameObject);
			}
		}
		m_setUp = false;
	}

	private void OnClickedRecieveAllBtn()
	{
		if (m_totalPageCount <= 0)
		{
			return;
		}
		List<int> list = new List<int>();
		List<int> list2 = new List<int>();
		int num = 0;
		if (ServerInterface.MessageList != null)
		{
			foreach (PresentInfo presentInfo in m_presentInfoList)
			{
				if (PresentBoxUtility.IsWithinTimeLimit(presentInfo.expireTime))
				{
					if (presentInfo.operatorFlag)
					{
						list2.Add(presentInfo.messageId);
					}
					else
					{
						list.Add(presentInfo.messageId);
					}
				}
				else
				{
					m_outOfDataFlag = true;
				}
			}
		}
		int num2 = list.Count + list2.Count;
		if (num2 > 0)
		{
			ServerInterface loggedInServerInterface = ServerInterface.LoggedInServerInterface;
			if (loggedInServerInterface != null)
			{
				loggedInServerInterface.RequestServerUpdateMessage(list, list2, base.gameObject);
			}
			BackKeyManager.InvalidFlag = true;
		}
		else if (m_outOfDataFlag)
		{
			StartCoroutine(ShowOnlyOutOfDataResult());
		}
		SoundManager.SePlay("sys_roulette_itemget");
	}

	private void OnClickedRecieveSelectBtn()
	{
		if (m_totalPageCount <= 0)
		{
			return;
		}
		List<int> list = new List<int>();
		List<int> list2 = new List<int>();
		if (m_itemStorage != null && list != null && list2 != null)
		{
			ui_presentbox_scroll[] componentsInChildren = m_itemStorage.GetComponentsInChildren<ui_presentbox_scroll>(true);
			int num = componentsInChildren.Length;
			for (int i = 0; i < num; i++)
			{
				if (!componentsInChildren[i].IsCheck())
				{
					continue;
				}
				int index = i + m_currentPageNum * 10;
				int messageId = m_presentInfoList[index].messageId;
				if (PresentBoxUtility.IsWithinTimeLimit(m_presentInfoList[index].expireTime))
				{
					if (m_presentInfoList[index].operatorFlag)
					{
						list2.Add(messageId);
					}
					else
					{
						list.Add(messageId);
					}
				}
				else
				{
					m_outOfDataFlag = true;
				}
			}
			int num2 = list.Count + list2.Count;
			if (num2 > 0)
			{
				ServerInterface loggedInServerInterface = ServerInterface.LoggedInServerInterface;
				if (loggedInServerInterface != null)
				{
					loggedInServerInterface.RequestServerUpdateMessage(list, list2, base.gameObject);
				}
				BackKeyManager.InvalidFlag = true;
			}
			else if (m_outOfDataFlag)
			{
				StartCoroutine(ShowOnlyOutOfDataResult());
			}
		}
		SoundManager.SePlay("sys_roulette_itemget");
	}

	private void OnClickedNextPageBtn()
	{
		SoundManager.SePlay("sys_page_skip");
		if (m_totalPageCount > 1)
		{
			m_currentPageNum++;
			if (m_totalPageCount == m_currentPageNum)
			{
				m_currentPageNum = 0;
			}
			UpdatePage();
		}
	}

	private void OnClickedPrevPageBtnBtn()
	{
		SoundManager.SePlay("sys_page_skip");
		if (m_totalPageCount > 1)
		{
			if (m_currentPageNum > 0)
			{
				m_currentPageNum--;
			}
			else
			{
				m_currentPageNum = m_totalPageCount - 1;
			}
			UpdatePage();
		}
	}

	private void ServerUpdateMessage_Succeeded(MsgUpdateMesseageSucceed msg)
	{
		BackKeyManager.InvalidFlag = false;
		StartCoroutine(ShowResult(msg));
		SetPresentBoxInfo();
		UpdatePage();
		HudMenuUtility.SendMsgUpdateSaveDataDisplay();
	}

	private void ServerUpdateMessage_Failed()
	{
		BackKeyManager.InvalidFlag = false;
	}

	public IEnumerator ShowOnlyOutOfDataResult()
	{
		GeneralWindow.CInfo info = default(GeneralWindow.CInfo);
		info.name = "presentbox";
		info.buttonType = GeneralWindow.ButtonType.Ok;
		info.caption = TextUtility.GetCommonText("PresentBox", "out_of_date_caption");
		info.message = TextUtility.GetCommonText("PresentBox", "out_of_date_text");
		info.parentGameObject = base.gameObject;
		GeneralWindow.Create(info);
		while (!GeneralWindow.IsButtonPressed)
		{
			yield return null;
		}
		GeneralWindow.Close();
		m_outOfDataFlag = false;
	}

	public IEnumerator ShowResult(MsgUpdateMesseageSucceed msg)
	{
		CharaType charaType = CharaType.UNKNOWN;
		bool charaFlag = false;
		bool chaoFlag = false;
		bool missFlag = false;
		if (msg != null)
		{
			missFlag = (msg.m_notRecvMessageList.Count + msg.m_notRecvOperatorMessageList.Count > 0);
			GeneralWindow.CInfo info = default(GeneralWindow.CInfo);
			info.name = "presentbox";
			info.buttonType = GeneralWindow.ButtonType.Ok;
			info.caption = TextUtility.GetCommonText("PresentBox", "present_box");
			info.message = PresentBoxUtility.GetPresetTextList(msg.m_presentStateList);
			info.parentGameObject = base.gameObject;
			GeneralWindow.Create(info);
		}
		else
		{
			GeneralWindow.CInfo info2 = default(GeneralWindow.CInfo);
			info2.name = "presentbox";
			info2.buttonType = GeneralWindow.ButtonType.Ok;
			info2.caption = TextUtility.GetCommonText("PresentBox", "present_box");
			info2.message = TextUtility.GetCommonText("PresentBox", "not_receive_message");
			info2.parentGameObject = base.gameObject;
			GeneralWindow.Create(info2);
		}
		while (!GeneralWindow.IsButtonPressed)
		{
			yield return null;
		}
		GeneralWindow.Close();
		if (missFlag)
		{
			GeneralWindow.CInfo info3 = default(GeneralWindow.CInfo);
			info3.name = "presentbox";
			info3.buttonType = GeneralWindow.ButtonType.Ok;
			info3.caption = TextUtility.GetCommonText("PresentBox", "present_box");
			info3.message = TextUtility.GetCommonText("PresentBox", "miss_message");
			info3.parentGameObject = base.gameObject;
			GeneralWindow.Create(info3);
			while (!GeneralWindow.IsButtonPressed)
			{
				yield return null;
			}
			GeneralWindow.Close();
		}
		if (m_outOfDataFlag)
		{
			GeneralWindow.CInfo info4 = default(GeneralWindow.CInfo);
			info4.name = "presentbox";
			info4.buttonType = GeneralWindow.ButtonType.Ok;
			info4.caption = TextUtility.GetCommonText("PresentBox", "out_of_date_caption");
			info4.message = TextUtility.GetCommonText("PresentBox", "out_of_date_text");
			info4.parentGameObject = base.gameObject;
			GeneralWindow.Create(info4);
			while (!GeneralWindow.IsButtonPressed)
			{
				yield return null;
			}
			GeneralWindow.Close();
		}
		if (msg != null)
		{
			foreach (ServerPresentState state2 in msg.m_presentStateList)
			{
				ServerItem serverItem2 = new ServerItem((ServerItem.Id)state2.m_itemId);
				if (serverItem2.idType != ServerItem.IdType.CHARA || serverItem2.charaType == CharaType.UNKNOWN)
				{
					continue;
				}
				charaFlag = true;
				charaType = serverItem2.charaType;
				GameObject uiRoot2 = GameObject.Find("UI Root (2D)");
				ServerPlayerState playerState = ServerInterface.PlayerState;
				if (playerState == null)
				{
					continue;
				}
				ServerCharacterState charaState = playerState.CharacterState(charaType);
				if (charaState != null && charaState.star > 0)
				{
					if (m_playerMergeWindow == null)
					{
						m_playerMergeWindow = GameObjectUtil.FindChildGameObjectComponent<PlayerMergeWindow>(uiRoot2, "player_merge_Window");
					}
					if (m_playerMergeWindow != null)
					{
						m_playerMergeWindow.PlayStart(state2.m_itemId, RouletteUtility.AchievementType.PlayerGet);
						while (!m_playerMergeWindow.IsPlayEnd)
						{
							yield return null;
						}
						m_playerMergeWindow = null;
					}
					continue;
				}
				if (m_charaGetWindow == null)
				{
					m_charaGetWindow = GameObjectUtil.FindChildGameObjectComponent<ChaoGetWindow>(uiRoot2, "ro_PlayerGetWindowUI");
				}
				if (m_charaGetWindow != null)
				{
					PlayerGetPartsOverlap playerGet = m_charaGetWindow.gameObject.GetComponent<PlayerGetPartsOverlap>();
					if (playerGet == null)
					{
						playerGet = m_charaGetWindow.gameObject.AddComponent<PlayerGetPartsOverlap>();
					}
					playerGet.Init(state2.m_itemId, 100, 0, null, PlayerGetPartsOverlap.IntroType.NO_EGG);
					ChaoGetPartsBase partsBase3 = playerGet;
					bool tutorial2 = false;
					bool disabledEqip2 = true;
					m_charaGetWindow.PlayStart(partsBase3, tutorial2, disabledEqip2);
					while (!m_charaGetWindow.IsPlayEnd)
					{
						yield return null;
					}
					m_charaGetWindow = null;
				}
			}
			List<int> chaoIdList = new List<int>();
			foreach (ServerPresentState state in msg.m_presentStateList)
			{
				ServerItem serverItem = new ServerItem((ServerItem.Id)state.m_itemId);
				if (serverItem.idType != ServerItem.IdType.CHAO || chaoIdList.Contains(serverItem.chaoId))
				{
					continue;
				}
				chaoIdList.Add(serverItem.chaoId);
				GameObject uiRoot = GameObject.Find("UI Root (2D)");
				ChaoData data = ChaoTable.GetChaoData(serverItem.chaoId);
				if (data == null)
				{
					continue;
				}
				chaoFlag = true;
				ChaoGetPartsBase partsBase2 = null;
				if (data.level > 0)
				{
					if (m_chaoMergeWindow == null)
					{
						m_chaoMergeWindow = GameObjectUtil.FindChildGameObjectComponent<ChaoMergeWindow>(uiRoot, "chao_merge_Window");
					}
					m_chaoMergeWindow.PlayStart(state.m_itemId, data.level, (int)data.rarity);
					while (!m_chaoMergeWindow.IsPlayEnd)
					{
						yield return null;
					}
					m_chaoMergeWindow = null;
					continue;
				}
				if (data.rarity == ChaoData.Rarity.NORMAL || data.rarity == ChaoData.Rarity.SRARE)
				{
					if (m_chaoGetWindow == null)
					{
						m_chaoGetWindow = GameObjectUtil.FindChildGameObjectComponent<ChaoGetWindow>(uiRoot, "chao_get_Window");
					}
					ChaoGetPartsNormal normal = m_chaoGetWindow.gameObject.GetComponent<ChaoGetPartsNormal>();
					if (normal == null)
					{
						normal = m_chaoGetWindow.gameObject.AddComponent<ChaoGetPartsNormal>();
					}
					normal.Init(state.m_itemId, (int)data.rarity);
					partsBase2 = normal;
				}
				else
				{
					if (m_chaoGetWindow == null)
					{
						m_chaoGetWindow = GameObjectUtil.FindChildGameObjectComponent<ChaoGetWindow>(uiRoot, "chao_rare_get_Window");
					}
					ChaoGetPartsRare rare = m_chaoGetWindow.gameObject.GetComponent<ChaoGetPartsRare>();
					if (rare == null)
					{
						rare = m_chaoGetWindow.gameObject.AddComponent<ChaoGetPartsRare>();
					}
					rare.Init(state.m_itemId, (int)data.rarity);
					partsBase2 = rare;
				}
				bool tutorial = false;
				bool disabledEqip = true;
				m_chaoGetWindow.PlayStart(partsBase2, tutorial, disabledEqip);
				while (!m_chaoGetWindow.IsPlayEnd)
				{
					yield return null;
				}
				m_chaoGetWindow = null;
			}
		}
		if (charaFlag || chaoFlag)
		{
			AchievementManager.RequestUpdate();
			while (!AchievementManager.IsRequestEnd())
			{
				yield return null;
			}
		}
		if (charaFlag)
		{
			GeneralWindow.CInfo info5 = default(GeneralWindow.CInfo);
			info5.name = "presentbox";
			info5.buttonType = GeneralWindow.ButtonType.YesNo;
			info5.caption = TextUtility.GetText(TextManager.TextType.TEXTTYPE_FIXATION_TEXT, "FaceBook", "ui_Lbl_verification");
			info5.message = TextUtility.GetCommonText("PresentBox", "player_set_text");
			info5.parentGameObject = base.gameObject;
			GeneralWindow.Create(info5);
			while (!GeneralWindow.IsButtonPressed)
			{
				yield return null;
			}
			if (GeneralWindow.IsYesButtonPressed)
			{
				chaoFlag = false;
				MenuPlayerSetUtil.SetMarkCharaPage(charaType);
				HudMenuUtility.SendMenuButtonClicked(ButtonInfoTable.ButtonType.CHARA_MAIN, true);
			}
			GeneralWindow.Close();
		}
		if (chaoFlag)
		{
			GeneralWindow.CInfo info6 = default(GeneralWindow.CInfo);
			info6.name = "presentbox";
			info6.buttonType = GeneralWindow.ButtonType.YesNo;
			info6.caption = TextUtility.GetText(TextManager.TextType.TEXTTYPE_FIXATION_TEXT, "FaceBook", "ui_Lbl_verification");
			info6.message = TextUtility.GetCommonText("PresentBox", "chao_set_text");
			info6.parentGameObject = base.gameObject;
			GeneralWindow.Create(info6);
			while (!GeneralWindow.IsButtonPressed)
			{
				yield return null;
			}
			if (GeneralWindow.IsYesButtonPressed)
			{
				HudMenuUtility.SendMenuButtonClicked(ButtonInfoTable.ButtonType.CHAO, true);
			}
			GeneralWindow.Close();
		}
		m_outOfDataFlag = false;
	}
}
