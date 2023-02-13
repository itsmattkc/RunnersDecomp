using UnityEngine;

public class ItemButton : MonoBehaviour
{
	public enum CursorColor
	{
		NONE = -1,
		BLUE,
		RED,
		GREEN,
		NUM
	}

	public delegate void ClickCallback(ItemType itemType, bool isEquiped);

	private ItemType m_itemType;

	private GameObject m_bgObject;

	private bool m_isEquiped;

	private ItemSetRingManagement m_ringManagement;

	private ClickCallback m_callback;

	private bool m_isActive = true;

	private bool m_isTutorialEnd;

	private int m_freeItemCount;

	private CursorColor m_cursorColor = CursorColor.NONE;

	public ItemType itemType
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
			bool result = false;
			if (m_bgObject != null)
			{
				bool flag = false;
				if (StageModeManager.Instance != null)
				{
					flag = StageModeManager.Instance.IsQuickMode();
				}
				switch (HudMenuUtility.itemSelectMode)
				{
				case HudMenuUtility.ITEM_SELECT_MODE.NORMAL:
					if (MileageMapUtility.IsBossStage() && !flag && (itemType == ItemType.COMBO || itemType == ItemType.TRAMPOLINE))
					{
						result = true;
					}
					break;
				case HudMenuUtility.ITEM_SELECT_MODE.EVENT_STAGE:
					result = false;
					break;
				case HudMenuUtility.ITEM_SELECT_MODE.EVENT_BOSS:
					if (!flag && (itemType == ItemType.COMBO || itemType == ItemType.TRAMPOLINE))
					{
						result = true;
					}
					break;
				}
			}
			return result;
		}
	}

	public void Setup(ItemType itemType, GameObject bgObject)
	{
		m_itemType = itemType;
		m_bgObject = bgObject;
		CheckItemLock();
		m_ringManagement = ItemSetUtility.GetItemSetRingManagement();
		if (m_bgObject == null)
		{
			return;
		}
		UISprite uISprite = GameObjectUtil.FindChildGameObjectComponent<UISprite>(base.gameObject, "img_item");
		if (uISprite != null)
		{
			int itemType2 = (int)m_itemType;
			uISprite.spriteName = "ui_cmn_icon_item_" + itemType2;
		}
		if (m_bgObject != null)
		{
			GameObject gameObject = GameObjectUtil.FindChildGameObject(m_bgObject, "Btn_toggle");
			if (gameObject != null)
			{
				UIButtonMessage uIButtonMessage = gameObject.GetComponent<UIButtonMessage>();
				if (uIButtonMessage == null)
				{
					uIButtonMessage = gameObject.AddComponent<UIButtonMessage>();
				}
				uIButtonMessage.target = base.gameObject;
				uIButtonMessage.functionName = "OnClickButton";
			}
		}
		OnEnable();
		RemoveCursor();
	}

	private void OnEnable()
	{
		CheckItemLock();
		GameObject gameObject = GameObjectUtil.FindChildGameObject(base.gameObject, "badge");
		GameObject gameObject2 = GameObjectUtil.FindChildGameObject(base.gameObject, "Lbl_ring_number");
		if (gameObject != null && gameObject2 != null)
		{
			int itemNum = ItemSetUtility.GetItemNum(m_itemType);
			if (itemNum > 0)
			{
				gameObject.SetActive(true);
				gameObject2.SetActive(false);
			}
			else
			{
				gameObject.SetActive(false);
				gameObject2.SetActive(true);
			}
		}
		UpdateItemCount();
		UpdateCampaignView();
		m_isTutorialEnd = true;
	}

	public void SetCallback(ClickCallback callback)
	{
		m_callback = callback;
	}

	public bool IsEquiped()
	{
		return m_isEquiped;
	}

	public void UpdateItemCount()
	{
		UILabel uILabel = GameObjectUtil.FindChildGameObjectComponent<UILabel>(base.gameObject, "Lbl_item_volume");
		if (uILabel != null)
		{
			uILabel.text = ItemSetUtility.GetItemNum(m_itemType).ToString();
		}
	}

	public void SetButtonActive(bool isActive)
	{
		if (!itemLock)
		{
			m_isActive = isActive;
			GameObject gameObject = GameObjectUtil.FindChildGameObject(base.gameObject, "disabled_filter");
			if (gameObject != null)
			{
				gameObject.SetActive(!m_isActive);
			}
			UIButtonScale uIButtonScale = GameObjectUtil.FindChildGameObjectComponent<UIButtonScale>(m_bgObject, "Btn_toggle");
			if (uIButtonScale != null)
			{
				uIButtonScale.enabled = isActive;
			}
			UIToggle uIToggle = GameObjectUtil.FindChildGameObjectComponent<UIToggle>(m_bgObject, "Btn_toggle");
			if (uIToggle != null)
			{
				uIToggle.enabled = m_isActive;
			}
		}
	}

	public void SetCursor(CursorColor cursorColor)
	{
		if (itemLock)
		{
			return;
		}
		m_isEquiped = true;
		m_cursorColor = cursorColor;
		GameObject gameObject = GameObjectUtil.FindChildGameObject(m_bgObject, "img_cursor");
		if (gameObject != null)
		{
			UISprite component = gameObject.GetComponent<UISprite>();
			if (component != null)
			{
				int num = (int)cursorColor;
				component.spriteName = "ui_itemset_3_cursor_" + num;
				component.alpha = 1f;
				component.color = Color.white;
			}
			gameObject.SetActive(true);
		}
		GameObject gameObject2 = GameObjectUtil.FindChildGameObject(m_bgObject, "Btn_toggle");
		if (gameObject2 != null)
		{
			UIToggle component2 = gameObject2.GetComponent<UIToggle>();
			if (component2 != null)
			{
				component2.value = true;
			}
		}
		UpdateButtonState();
	}

	public void RemoveCursor()
	{
		m_cursorColor = CursorColor.NONE;
		GameObject gameObject = GameObjectUtil.FindChildGameObject(m_bgObject, "img_cursor");
		if (gameObject != null)
		{
			UISprite component = gameObject.GetComponent<UISprite>();
			if (component != null)
			{
				component.spriteName = string.Empty;
			}
			gameObject.SetActive(false);
		}
		GameObject gameObject2 = GameObjectUtil.FindChildGameObject(m_bgObject, "Btn_toggle");
		if (gameObject2 != null)
		{
			UIToggle component2 = gameObject2.GetComponent<UIToggle>();
			if (component2 != null)
			{
				component2.value = false;
			}
		}
		m_isEquiped = false;
		UpdateButtonState();
	}

	public CursorColor GetCursorColor()
	{
		return m_cursorColor;
	}

	public bool IsButtonActive()
	{
		return m_isActive;
	}

	private void Start()
	{
	}

	private void Update()
	{
		UpdateCampaignView();
	}

	private void OnClickButton()
	{
		if (m_isActive)
		{
			m_isEquiped = !m_isEquiped;
		}
		string cueName = (!m_isEquiped) ? "sys_cancel" : "sys_menu_decide";
		SoundManager.SePlay(cueName);
		if (m_callback != null)
		{
			m_callback(m_itemType, m_isEquiped);
		}
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
		bool active = false;
		bool flag = IsFreeItem();
		ServerCampaignData campaignDataInSession = ItemSetUtility.GetCampaignDataInSession((int)new ServerItem(m_itemType).id);
		GameObject gameObject = GameObjectUtil.FindChildGameObject(base.gameObject, "img_free_icon");
		if (gameObject != null)
		{
			gameObject.SetActive(flag);
		}
		if (!flag && campaignDataInSession != null)
		{
			active = true;
		}
		GameObject gameObject2 = GameObjectUtil.FindChildGameObject(base.gameObject, "img_sale_icon");
		if (gameObject2 != null)
		{
			gameObject2.SetActive(active);
		}
		UILabel uILabel = GameObjectUtil.FindChildGameObjectComponent<UILabel>(base.gameObject, "Lbl_ring_number");
		if (uILabel != null)
		{
			uILabel.text = ItemSetUtility.GetOneItemCostString(m_itemType);
		}
	}

	private void UpdateButtonState()
	{
		bool flag = IsFreeItem();
		int itemNum = ItemSetUtility.GetItemNum(m_itemType);
		if (flag)
		{
			return;
		}
		if (itemNum > 0)
		{
			int num = 0;
			num = ((!m_isEquiped) ? itemNum : itemNum);
			UILabel uILabel = GameObjectUtil.FindChildGameObjectComponent<UILabel>(base.gameObject, "Lbl_item_volume");
			if (uILabel != null)
			{
				uILabel.text = num.ToString();
			}
			return;
		}
		int oneItemCost = ItemSetUtility.GetOneItemCost(m_itemType);
		if (m_isEquiped)
		{
			if (m_ringManagement != null)
			{
				m_ringManagement.AddOffset(-oneItemCost);
			}
		}
		else if (m_ringManagement != null)
		{
			m_ringManagement.AddOffset(oneItemCost);
		}
	}

	private bool CheckItemLock()
	{
		bool itemLock = this.itemLock;
		if (m_bgObject != null)
		{
			UISprite uISprite = GameObjectUtil.FindChildGameObjectComponent<UISprite>(m_bgObject.gameObject, "img_bg");
			BoxCollider boxCollider = GameObjectUtil.FindChildGameObjectComponent<BoxCollider>(m_bgObject.gameObject, "Btn_toggle");
			if (itemLock)
			{
				if (uISprite != null)
				{
					uISprite.spriteName = "ui_itemset_3_bost_4";
				}
				if (boxCollider != null)
				{
					boxCollider.enabled = false;
				}
			}
			else
			{
				if (uISprite != null)
				{
					uISprite.spriteName = "ui_itemset_3_bost_or_1";
				}
				if (boxCollider != null)
				{
					boxCollider.enabled = true;
				}
			}
		}
		return itemLock;
	}
}
