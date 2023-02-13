using AnimationOrTween;
using Message;
using System.Collections.Generic;
using Text;
using UnityEngine;

public class ItemSetWindowEquipUI : MonoBehaviour
{
	private int m_id;

	private int m_count;

	public void OpenWindow(int id, int count)
	{
		m_id = id;
		m_count = count;
		SoundManager.SePlay("sys_window_open");
		UpdateView();
		Animation animation = GameObjectUtil.FindGameObjectComponent<Animation>("item_set_window_equip");
		if (animation != null)
		{
			ActiveAnimation.Play(animation, "ui_cmn_window_Anim", Direction.Forward, EnableCondition.DoNothing, DisableCondition.DoNotDisable);
		}
	}

	private void OnClickClose()
	{
		SoundManager.SePlay("sys_window_close");
	}

	private static int GetMainCharaItemAbilityLevel(ItemType itemType)
	{
		AbilityType abilityType = StageItemManager.s_dicItemTypeToCharAbilityType[itemType];
		SaveDataManager instance = SaveDataManager.Instance;
		CharaType mainChara = instance.PlayerData.MainChara;
		CharaAbility charaAbility = instance.CharaData.AbilityArray[(int)mainChara];
		return (int)(((uint)abilityType < charaAbility.Ability.Length) ? charaAbility.Ability[(int)abilityType] : 0);
	}

	private void UpdateView()
	{
		ShopItemData shopItemData = ShopItemTable.GetShopItemData(m_id);
		if (shopItemData != null)
		{
			UILabel uILabel = GameObjectUtil.FindChildGameObjectComponent<UILabel>(base.gameObject, "Lbt_item_name");
			if (uILabel != null)
			{
				uILabel.text = shopItemData.name;
			}
			UILabel uILabel2 = GameObjectUtil.FindChildGameObjectComponent<UILabel>(base.gameObject, "Lbt_item_effect");
			if (uILabel2 != null)
			{
				uILabel2.text = GetItemDetailsText(shopItemData);
			}
		}
		ui_item_set_cell ui_item_set_cell = GameObjectUtil.FindChildGameObjectComponent<ui_item_set_cell>(base.gameObject, "ui_item_set_cell(Clone)");
		if (ui_item_set_cell != null)
		{
			ui_item_set_cell.UpdateView(m_id, m_count);
		}
		for (EquippedType equippedType = EquippedType.EQUIPPED_01; equippedType < EquippedType.NUM; equippedType++)
		{
			GameObject gameObject = GameObjectUtil.FindChildGameObject(base.gameObject, "Btn_off_" + (int)(equippedType + 1));
			if (gameObject != null)
			{
				gameObject.SetActive(ItemSetUI.SaveDataInterface.GetEquipedItemType(equippedType) == (ItemType)m_id);
			}
		}
	}

	private void OnClickEquipSlot0()
	{
		EquipItem(EquippedType.EQUIPPED_01);
		SoundManager.SePlay("sys_menu_decide");
	}

	private void OnClickEquipSlot1()
	{
		EquipItem(EquippedType.EQUIPPED_02);
		SoundManager.SePlay("sys_menu_decide");
	}

	private void OnClickEquipSlot2()
	{
		EquipItem(EquippedType.EQUIPPED_03);
		SoundManager.SePlay("sys_menu_decide");
	}

	private void OnClickUnequipSlot0()
	{
		UnequipItem(EquippedType.EQUIPPED_01);
		SoundManager.SePlay("sys_window_close");
	}

	private void OnClickUnequipSlot1()
	{
		UnequipItem(EquippedType.EQUIPPED_02);
		SoundManager.SePlay("sys_window_close");
	}

	private void OnClickUnequipSlot2()
	{
		UnequipItem(EquippedType.EQUIPPED_03);
		SoundManager.SePlay("sys_window_close");
	}

	private void OnClickToBuy()
	{
		ItemSetWindowBuyUI itemSetWindowBuyUI = GameObjectUtil.FindGameObjectComponent<ItemSetWindowBuyUI>("ItemSetWindowBuyUI");
		if (itemSetWindowBuyUI != null)
		{
			itemSetWindowBuyUI.OpenWindow(m_id, m_count);
			SoundManager.SePlay("sys_menu_decide");
		}
	}

	private void EquipItem(EquippedType slot)
	{
		ItemType[] equipItems = GetEquipItems();
		EquippedType equipedSlot = ItemSetUI.SaveDataInterface.GetEquipedSlot(m_id);
		if (equipedSlot < EquippedType.NUM)
		{
			equipItems[(int)equipedSlot] = ItemSetUI.SaveDataInterface.GetEquipedItemType(slot);
		}
		equipItems[(int)slot] = (ItemType)m_id;
		UpdateItems(equipItems);
	}

	private void UnequipItem(EquippedType removeSlot)
	{
		ItemType[] equipItems = GetEquipItems();
		equipItems[(int)removeSlot] = ItemType.UNKNOWN;
		UpdateItems(equipItems);
	}

	private void UpdateItems(ItemType[] equipItems)
	{
		ServerInterface loggedInServerInterface = ServerInterface.LoggedInServerInterface;
		if (loggedInServerInterface != null)
		{
			List<ItemType> list = new List<ItemType>();
			for (EquippedType equippedType = EquippedType.EQUIPPED_01; equippedType < EquippedType.NUM; equippedType++)
			{
				list.Add(equipItems[(int)equippedType]);
			}
			loggedInServerInterface.RequestServerEquipItem(list, base.gameObject);
		}
		else
		{
			SetEquipItems(equipItems);
			UpdateEquipedItemView();
		}
	}

	private void ServerEquipItem_Succeeded(MsgGetPlayerStateSucceed msg)
	{
		UpdateEquipedItemView();
	}

	private void UpdateEquipedItemView()
	{
		ItemSetUI itemSetUI = GameObjectUtil.FindGameObjectComponent<ItemSetUI>("ItemSetUI");
		if (itemSetUI != null)
		{
			itemSetUI.UpdateView();
		}
		HudMenuUtility.SendMsgUpdateSaveDataDisplay();
	}

	private ItemType[] GetEquipItems()
	{
		ItemType[] array = new ItemType[3];
		for (EquippedType equippedType = EquippedType.EQUIPPED_01; equippedType < EquippedType.NUM; equippedType++)
		{
			array[(int)equippedType] = ItemSetUI.SaveDataInterface.GetEquipedItemType(equippedType);
		}
		return array;
	}

	private void SetEquipItems(ItemType[] equipItems)
	{
		for (EquippedType equippedType = EquippedType.EQUIPPED_01; equippedType < EquippedType.NUM; equippedType++)
		{
			ItemSetUI.SaveDataInterface.SetEquipedItemType(equippedType, equipItems[(int)equippedType]);
		}
	}

	public static string GetItemDetailsText(ShopItemData shopItemData)
	{
		ItemType id = (ItemType)shopItemData.id;
		return TextUtility.Replaces(shopItemData.details, new Dictionary<string, string>
		{
			{
				"{LEVEL}",
				GetMainCharaItemAbilityLevel(id).ToString()
			},
			{
				"{TIME}",
				((int)StageItemManager.GetItemTimeFromChara(id)).ToString("0.0")
			}
		});
	}
}
