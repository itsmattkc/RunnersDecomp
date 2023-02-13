using System.Collections.Generic;
using UnityEngine;

public class HudItemPanel : MonoBehaviour
{
	private const string m_slot_obj_path = "Anchor_5_MC/mainmenu_contents/grid/page_1/slot/ui_mm_main2_page(Clone)/item_set/slot";

	private GameObject m_slot_obj;

	private List<ui_item_set_cell> m_item_cells = new List<ui_item_set_cell>();

	private UIRectItemStorage m_ui_rect_item_storage;

	private bool m_init_flag;

	private void Start()
	{
		HudMenuUtility.SetTagHudSaveItem(base.gameObject);
		base.enabled = false;
	}

	private void Initialize()
	{
		GameObject mainMenuUIObject = HudMenuUtility.GetMainMenuUIObject();
		if (mainMenuUIObject != null)
		{
			Transform transform = mainMenuUIObject.transform.FindChild("Anchor_5_MC/mainmenu_contents/grid/page_1/slot/ui_mm_main2_page(Clone)/item_set/slot");
			if (transform != null)
			{
				m_slot_obj = transform.gameObject;
				if (m_slot_obj != null)
				{
					m_ui_rect_item_storage = m_slot_obj.GetComponent<UIRectItemStorage>();
				}
			}
		}
		if (!(m_ui_rect_item_storage != null))
		{
			return;
		}
		m_init_flag = true;
		List<GameObject> list = GameObjectUtil.FindChildGameObjects(m_slot_obj, "ui_item_set_cell(Clone)");
		if (list.Count != m_ui_rect_item_storage.maxItemCount)
		{
			return;
		}
		for (int i = 0; i < list.Count; i++)
		{
			BoxCollider component = list[i].GetComponent<BoxCollider>();
			if (component != null)
			{
				component.enabled = false;
			}
			m_item_cells.Add(list[i].GetComponent<ui_item_set_cell>());
		}
	}

	public void OnUpdateSaveDataDisplay()
	{
		if (!m_init_flag)
		{
			Initialize();
		}
		SaveDataManager instance = SaveDataManager.Instance;
		if (instance != null && m_item_cells != null && m_item_cells.Count >= 3)
		{
			if ((bool)m_item_cells[0])
			{
				ItemType itemType = instance.PlayerData.EquippedItem[0];
				uint itemCount = instance.ItemData.GetItemCount(itemType);
				m_item_cells[0].UpdateView((int)itemType, (int)itemCount);
			}
			if ((bool)m_item_cells[1])
			{
				ItemType itemType2 = instance.PlayerData.EquippedItem[1];
				uint itemCount2 = instance.ItemData.GetItemCount(itemType2);
				m_item_cells[1].UpdateView((int)itemType2, (int)itemCount2);
			}
			if ((bool)m_item_cells[2])
			{
				ItemType itemType3 = instance.PlayerData.EquippedItem[2];
				uint itemCount3 = instance.ItemData.GetItemCount(itemType3);
				m_item_cells[2].UpdateView((int)itemType3, (int)itemCount3);
			}
		}
	}
}
