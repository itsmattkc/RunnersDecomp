using SaveData;
using System.Collections.Generic;
using UnityEngine;

public class ItemSet : MonoBehaviour
{
	private List<ItemButton> m_buttons = new List<ItemButton>();

	private ItemWindow m_window;

	private bool[] m_enableColor = new bool[3];

	private ItemType[] m_itemType = new ItemType[3];

	private void Awake()
	{
		for (int i = 0; i < m_itemType.Length; i++)
		{
			m_itemType[i] = ItemType.UNKNOWN;
		}
	}

	private void Start()
	{
	}

	private void Update()
	{
	}

	private void OnEnable()
	{
		if (m_window != null)
		{
			if (StageAbilityManager.Instance != null)
			{
				StageAbilityManager.Instance.RecalcAbilityVaue();
			}
			m_window.SetItemType(ItemType.INVINCIBLE);
		}
	}

	private void OnDestroy()
	{
		StageInfo stageInfo = GameObjectUtil.FindGameObjectComponent<StageInfo>("StageInfo");
		if (stageInfo != null)
		{
			for (int i = 0; i < 3; i++)
			{
				stageInfo.EquippedItems[i] = m_itemType[i];
			}
		}
	}

	public void Setup()
	{
		SetupItem();
	}

	public void ResetCheckMark()
	{
		for (int i = 0; i < 3; i++)
		{
			m_itemType[i] = ItemType.UNKNOWN;
			m_enableColor[i] = true;
		}
		foreach (ItemButton button in m_buttons)
		{
			if (!(button == null))
			{
				ItemButton.CursorColor cursorColor = button.GetCursorColor();
				if (cursorColor != ItemButton.CursorColor.NONE)
				{
					button.RemoveCursor();
				}
				button.SetButtonActive(true);
			}
		}
		if (m_window != null)
		{
			m_window.SetWindowActive();
			m_window.SetEquipMark(false);
		}
	}

	public void SetupEquipItem()
	{
		if (m_window != null)
		{
			m_window.SetItemType(ItemType.INVINCIBLE);
			m_window.SetWindowActive();
		}
		SaveDataManager instance = SaveDataManager.Instance;
		PlayerData playerData = null;
		if (!(instance != null))
		{
			return;
		}
		playerData = instance.PlayerData;
		if (playerData != null)
		{
			for (int i = 0; i < 3; i++)
			{
				ItemType item = playerData.EquippedItem[i];
				SetupEquipItemOne(i, item);
			}
		}
	}

	private void SetupEquipItemOne(int equipIndex, ItemType item)
	{
		if (item == ItemType.UNKNOWN)
		{
			return;
		}
		if (m_window != null && !m_buttons[(int)item].itemLock)
		{
			m_window.SetWindowActive();
			m_window.SetEquipMark(true);
			m_window.SetItemType(item);
		}
		m_buttons[(int)item].SetCursor((ItemButton.CursorColor)equipIndex);
		if (m_buttons[(int)item].IsEquiped())
		{
			if (m_window != null)
			{
				m_window.SetEquipMarkColor((ItemButton.CursorColor)equipIndex);
			}
			m_itemType[equipIndex] = item;
			m_enableColor[equipIndex] = false;
		}
		SetButtonActive();
	}

	public void UpdateView()
	{
		if (m_window != null)
		{
			m_window.UpdateView();
		}
	}

	public void UpdateFreeItemList(ServerFreeItemState freeItemState)
	{
		List<ServerItemState> itemList = freeItemState.itemList;
		foreach (ItemButton button in m_buttons)
		{
			if (button == null)
			{
				continue;
			}
			for (int i = 0; i < itemList.Count; i++)
			{
				if (button.itemType == itemList[i].GetItem().itemType)
				{
					button.UpdateFreeItemCount(itemList[i].m_num);
				}
			}
		}
	}

	private void SetupItem()
	{
		for (int i = 0; i < 3; i++)
		{
			m_itemType[i] = ItemType.UNKNOWN;
			m_enableColor[i] = true;
		}
		GameObject itemSetRootObject = ItemSetUtility.GetItemSetRootObject();
		m_window = GameObjectUtil.FindChildGameObjectComponent<ItemWindow>(itemSetRootObject, "info_pla");
		m_window.SetItemBuyCallback(ItemBuyCallback);
		GameObject gameObject = GameObjectUtil.FindChildGameObject(base.gameObject, "slot_bg");
		GameObject gameObject2 = GameObjectUtil.FindChildGameObject(base.gameObject, "slot_item");
		if (!(gameObject != null) || !(gameObject2 != null))
		{
			return;
		}
		int childCount = gameObject.transform.childCount;
		int childCount2 = gameObject2.transform.childCount;
		if (childCount != childCount2)
		{
			return;
		}
		int num = 8;
		m_buttons.Clear();
		for (int j = 0; j < num; j++)
		{
			Transform child = gameObject2.transform.GetChild(j);
			if (child == null)
			{
				continue;
			}
			GameObject gameObject3 = child.gameObject;
			if (!(gameObject3 == null))
			{
				ItemButton itemButton = gameObject3.GetComponent<ItemButton>();
				if (itemButton == null)
				{
					itemButton = gameObject3.AddComponent<ItemButton>();
				}
				itemButton.Setup((ItemType)j, gameObject.transform.GetChild(j).gameObject);
				itemButton.SetCallback(ClickButtonCallback);
				m_buttons.Add(itemButton);
			}
		}
		int num2 = childCount;
		if (num >= num2)
		{
			return;
		}
		for (int k = num; k < num2; k++)
		{
			Transform child2 = gameObject2.transform.GetChild(k);
			if (child2 != null)
			{
				GameObject gameObject4 = child2.gameObject;
				if (gameObject4 != null)
				{
					gameObject4.SetActive(false);
				}
			}
			Transform child3 = gameObject.transform.GetChild(k);
			if (child3 != null)
			{
				GameObject gameObject5 = child3.gameObject;
				if (gameObject5 != null)
				{
					gameObject5.SetActive(false);
				}
			}
		}
	}

	private void ClickButtonCallback(ItemType itemType, bool isEquiped)
	{
		if (m_window != null)
		{
			m_window.SetWindowActive();
			m_window.SetEquipMark(isEquiped);
			m_window.SetItemType(itemType);
		}
		if (isEquiped)
		{
			int num = 0;
			for (int i = 0; i < 3; i++)
			{
				if (m_enableColor[i])
				{
					num = i;
					m_enableColor[i] = false;
					break;
				}
			}
			ItemButton.CursorColor cursorColor = (ItemButton.CursorColor)num;
			m_buttons[(int)itemType].SetCursor(cursorColor);
			if (m_window != null)
			{
				m_window.SetEquipMarkColor(cursorColor);
			}
			m_itemType[num] = itemType;
			SaveDataManager instance = SaveDataManager.Instance;
			if (instance != null)
			{
				PlayerData playerData = instance.PlayerData;
				if (playerData != null)
				{
					playerData.EquippedItem[num] = itemType;
				}
			}
		}
		else
		{
			ItemButton.CursorColor cursorColor2 = m_buttons[(int)itemType].GetCursorColor();
			if (cursorColor2 != ItemButton.CursorColor.NONE)
			{
				m_enableColor[(int)cursorColor2] = true;
				m_itemType[(int)cursorColor2] = ItemType.UNKNOWN;
				m_buttons[(int)itemType].RemoveCursor();
				SaveDataManager instance2 = SaveDataManager.Instance;
				if (instance2 != null)
				{
					PlayerData playerData2 = instance2.PlayerData;
					if (playerData2 != null)
					{
						playerData2.EquippedItem[(int)cursorColor2] = ItemType.UNKNOWN;
					}
				}
			}
		}
		SetButtonActive();
	}

	private void SetButtonActive()
	{
		int num = 0;
		foreach (ItemButton button in m_buttons)
		{
			if (!(button == null) && button.IsEquiped())
			{
				num++;
			}
		}
		if (num >= 3)
		{
			foreach (ItemButton button2 in m_buttons)
			{
				if (!(button2 == null) && !button2.IsEquiped())
				{
					button2.SetButtonActive(false);
				}
			}
			return;
		}
		foreach (ItemButton button3 in m_buttons)
		{
			if (!(button3 == null))
			{
				button3.SetButtonActive(true);
			}
		}
	}

	private void ItemBuyCallback(ItemType itemType)
	{
		ItemButton itemButton = m_buttons[(int)itemType];
		if (!(itemButton == null))
		{
			itemButton.UpdateItemCount();
		}
	}

	public ItemType[] GetItem()
	{
		return m_itemType;
	}
}
