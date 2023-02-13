using UnityEngine;

public class InstantItemButton : MonoBehaviour
{
	public delegate void ClickCallback(BoostItemType itemType, bool isChecked);

	private bool m_isChecked;

	private UIToggle m_uiToggle;

	private BoostItemType m_itemType;

	private ItemSetRingManagement m_ringManagement;

	private ClickCallback m_callback;

	private bool m_isEnableCheck = true;

	private bool m_isTutorialEnd;

	private int m_freeItemCount;

	public BoostItemType boostItemType
	{
		get
		{
			return m_itemType;
		}
	}

	public bool itemLock
	{
		get
		{
			bool flag = false;
			SaveDataManager instance = SaveDataManager.Instance;
			if (instance != null)
			{
				flag = ((m_itemType == BoostItemType.SUB_CHARACTER && instance.PlayerData.SubChara == CharaType.UNKNOWN) ? true : false);
			}
			bool flag2 = false;
			if (StageModeManager.Instance != null)
			{
				flag2 = StageModeManager.Instance.IsQuickMode();
			}
			if (!flag && m_itemType != BoostItemType.SUB_CHARACTER)
			{
				switch (HudMenuUtility.itemSelectMode)
				{
				case HudMenuUtility.ITEM_SELECT_MODE.NORMAL:
					if (MileageMapUtility.IsBossStage() && !flag2 && (m_itemType == BoostItemType.SCORE_BONUS || m_itemType == BoostItemType.ASSIST_TRAMPOLINE))
					{
						flag = true;
					}
					break;
				case HudMenuUtility.ITEM_SELECT_MODE.EVENT_STAGE:
					if (m_itemType == BoostItemType.SCORE_BONUS)
					{
						flag = true;
					}
					break;
				case HudMenuUtility.ITEM_SELECT_MODE.EVENT_BOSS:
					if (!flag2 && (m_itemType == BoostItemType.SCORE_BONUS || m_itemType == BoostItemType.ASSIST_TRAMPOLINE))
					{
						flag = true;
					}
					break;
				}
			}
			return flag;
		}
	}

	public bool IsChecked()
	{
		return m_isChecked;
	}

	public void Setup(BoostItemType itemType, ClickCallback callback)
	{
		m_itemType = itemType;
		m_callback = callback;
		string name = "Btn_toggle_" + (int)(itemType + 1);
		GameObject gameObject = GameObjectUtil.FindChildGameObject(base.gameObject, name);
		if (!(gameObject == null))
		{
			m_uiToggle = gameObject.GetComponent<UIToggle>();
			UIButtonMessage uIButtonMessage = gameObject.GetComponent<UIButtonMessage>();
			if (uIButtonMessage == null)
			{
				uIButtonMessage = gameObject.AddComponent<UIButtonMessage>();
			}
			uIButtonMessage.target = base.gameObject;
			uIButtonMessage.functionName = "OnClickButton";
			m_ringManagement = ItemSetUtility.GetItemSetRingManagement();
			OnEnable();
		}
	}

	public void ResetCheckMark()
	{
		if (m_isChecked)
		{
			if (!IsFreeItem() && m_ringManagement != null)
			{
				int instantItemCost = ItemSetUtility.GetInstantItemCost(m_itemType);
				m_ringManagement.AddOffset(instantItemCost);
			}
			if (m_uiToggle != null)
			{
				m_uiToggle.value = false;
			}
			m_isChecked = false;
		}
	}

	private void Start()
	{
	}

	private void Update()
	{
		UpdateCampaignView();
	}

	private void OnEnable()
	{
		m_isEnableCheck = !itemLock;
		m_isTutorialEnd = true;
		string name = "Btn_toggle_" + (int)(m_itemType + 1);
		UIImageButton uIImageButton = GameObjectUtil.FindChildGameObjectComponent<UIImageButton>(base.gameObject, name);
		if ((bool)uIImageButton)
		{
			if (m_isEnableCheck)
			{
				uIImageButton.isEnabled = true;
			}
			else
			{
				uIImageButton.isEnabled = false;
			}
		}
		UpdateCampaignView();
	}

	private bool IsFreeItem()
	{
		bool result = false;
		if (m_isTutorialEnd && m_freeItemCount > 0)
		{
			result = true;
		}
		return result;
	}

	public void UpdateFreeItemCount(int count)
	{
		m_freeItemCount = count;
		UpdateCampaignView();
	}

	private void UpdateCampaignView()
	{
		int num = (int)(m_itemType + 1);
		string name = "Lbl_ring_number_" + num;
		UILabel uILabel = GameObjectUtil.FindChildGameObjectComponent<UILabel>(base.gameObject, name);
		if (uILabel != null)
		{
			uILabel.text = ItemSetUtility.GetInstantItemCostString(m_itemType);
		}
		bool active = false;
		bool flag = IsFreeItem();
		ServerItem[] serverItemTable = ServerItem.GetServerItemTable(ServerItem.IdType.BOOST_ITEM);
		int id = (int)serverItemTable[(int)m_itemType].id;
		ServerCampaignData campaignDataInSession = ItemSetUtility.GetCampaignDataInSession(id);
		if (campaignDataInSession != null)
		{
			active = true;
		}
		string name2 = "img_free_icon_" + num;
		GameObject gameObject = GameObjectUtil.FindChildGameObject(base.gameObject, name2);
		if (gameObject != null)
		{
			gameObject.SetActive(flag);
		}
		if (flag)
		{
			active = false;
		}
		string name3 = "img_sale_icon_" + num;
		GameObject gameObject2 = GameObjectUtil.FindChildGameObject(base.gameObject, name3);
		if (gameObject2 != null)
		{
			gameObject2.SetActive(active);
		}
	}

	private void OnClickButton()
	{
		m_isChecked = !m_isChecked;
		Debug.Log("InstantItemButton:OnClickButton   this button >>" + m_itemType);
		bool flag = IsFreeItem();
		bool flag2 = true;
		int instantItemCost = ItemSetUtility.GetInstantItemCost(m_itemType);
		SaveDataManager instance = SaveDataManager.Instance;
		if (instance == null)
		{
			return;
		}
		if (!m_isEnableCheck)
		{
			m_isChecked = false;
			if ((bool)m_uiToggle)
			{
				m_uiToggle.value = false;
				flag2 = false;
			}
		}
		else if (!flag)
		{
			if (m_isChecked)
			{
				if (m_ringManagement != null)
				{
					m_ringManagement.AddOffset(-instantItemCost);
				}
			}
			else if (m_ringManagement != null)
			{
				m_ringManagement.AddOffset(instantItemCost);
			}
		}
		string empty = string.Empty;
		string empty2 = string.Empty;
		if (m_isChecked)
		{
			empty = "sys_menu_decide";
			empty2 = "ui_itemset_3_bost_1";
		}
		else if (flag2)
		{
			empty = "sys_cancel";
			empty2 = "ui_itemset_3_bost_0";
		}
		else if (!m_isEnableCheck)
		{
			empty = "sys_error";
			empty2 = "ui_itemset_3_bost_4";
		}
		else
		{
			empty = "sys_error";
			empty2 = "ui_itemset_3_bost_0";
		}
		SoundManager.SePlay(empty);
		UISprite uISprite = GameObjectUtil.FindChildGameObjectComponent<UISprite>(base.gameObject, "img_bg");
		if (uISprite != null)
		{
			uISprite.spriteName = empty2;
		}
		if (m_callback != null)
		{
			m_callback(m_itemType, m_isChecked);
		}
	}

	public void SetupBoostedItemButton(bool isChecked)
	{
		if (!m_isEnableCheck)
		{
			return;
		}
		m_isChecked = isChecked;
		bool flag = IsFreeItem();
		string empty = string.Empty;
		m_uiToggle.value = isChecked;
		if (!m_isChecked)
		{
			empty = (m_isEnableCheck ? "ui_itemset_3_bost_0" : "ui_itemset_3_bost_4");
		}
		else if (m_isEnableCheck)
		{
			if (!flag)
			{
				int instantItemCost = ItemSetUtility.GetInstantItemCost(m_itemType);
				m_ringManagement.AddOffset(-instantItemCost);
			}
			empty = "ui_itemset_3_bost_1";
		}
		else
		{
			m_isChecked = false;
			m_uiToggle.value = false;
			empty = "ui_itemset_3_bost_4";
		}
		UISprite uISprite = GameObjectUtil.FindChildGameObjectComponent<UISprite>(base.gameObject, "img_bg");
		if (uISprite != null)
		{
			uISprite.spriteName = empty;
		}
		if (m_callback != null)
		{
			m_callback(m_itemType, m_isChecked);
		}
	}
}
