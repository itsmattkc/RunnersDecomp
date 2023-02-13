using Message;
using System.Collections.Generic;
using UnityEngine;

public class ItemSetUtility
{
	public enum ItemSetType
	{
		SET_ONE,
		SET_TEN
	}

	public static readonly string[] ButtonObjectName = new string[3]
	{
		"boost_switch_1_lt",
		"boost_switch_2_ct",
		"boost_switch_3_rt"
	};

	private static readonly int TEN_PACK_OFFSET = 2;

	public static GameObject GetItemSetRootObject()
	{
		GameObject gameObject = GameObject.Find("UI Root (2D)");
		if (gameObject == null)
		{
			return null;
		}
		return GameObjectUtil.FindChildGameObject(gameObject, "ItemSet_3_UI");
	}

	public static ItemSetRingManagement GetItemSetRingManagement()
	{
		GameObject itemSetRootObject = GetItemSetRootObject();
		if (itemSetRootObject != null)
		{
			return itemSetRootObject.GetComponent<ItemSetRingManagement>();
		}
		return null;
	}

	public static int GetInstantItemCost(BoostItemType itemType)
	{
		ServerItem[] serverItemTable = ServerItem.GetServerItemTable(ServerItem.IdType.BOOST_ITEM);
		int id = (int)serverItemTable[(int)itemType].id;
		int result = 100;
		List<ServerConsumedCostData> costList = ServerInterface.CostList;
		if (costList == null)
		{
			return result;
		}
		foreach (ServerConsumedCostData item in costList)
		{
			if (item == null || item.consumedItemId != id)
			{
				continue;
			}
			result = item.numItem;
			ServerCampaignData campaignDataInSession = GetCampaignDataInSession(id);
			if (campaignDataInSession != null)
			{
				return campaignDataInSession.iContent;
			}
			return result;
		}
		return result;
	}

	public static string GetInstantItemCostString(BoostItemType itemType)
	{
		int instantItemCost = GetInstantItemCost(itemType);
		return HudUtility.GetFormatNumString(instantItemCost);
	}

	public static int GetItemNum(ItemType itemType)
	{
		int num = 0;
		SaveDataManager instance = SaveDataManager.Instance;
		if (instance != null)
		{
			num = (int)instance.ItemData.GetItemCount(itemType);
			if ((long)num > 99L)
			{
				num = 99;
			}
		}
		if (itemType == ItemType.LASER)
		{
			MsgMenuItemSetStart.SetMode msgMenuItemSetStartMode = GetMsgMenuItemSetStartMode();
			if (msgMenuItemSetStartMode == MsgMenuItemSetStart.SetMode.TUTORIAL && num == 0)
			{
				num = 1;
			}
		}
		return num;
	}

	public static int GetFreeItemNum(ItemType itemType)
	{
		int result = 0;
		ServerFreeItemState freeItemState = ServerInterface.FreeItemState;
		if (freeItemState != null)
		{
			List<ServerItemState> itemList = freeItemState.itemList;
			for (int i = 0; i < itemList.Count; i++)
			{
				if (itemType == itemList[i].GetItem().itemType)
				{
					result = itemList[i].m_num;
					break;
				}
			}
		}
		return result;
	}

	public static int GetFreeBoostItemNum(BoostItemType boostItemType)
	{
		int result = 0;
		ServerFreeItemState freeItemState = ServerInterface.FreeItemState;
		if (freeItemState != null)
		{
			List<ServerItemState> itemList = freeItemState.itemList;
			for (int i = 0; i < itemList.Count; i++)
			{
				if (boostItemType == itemList[i].GetItem().boostItemType)
				{
					result = itemList[i].m_num;
					break;
				}
			}
		}
		return result;
	}

	public static int GetOneRingItemId(ItemType itemType)
	{
		ServerItem[] serverItemTable = ServerItem.GetServerItemTable(ServerItem.IdType.EQUIP_ITEM);
		return (int)serverItemTable[(int)itemType].id;
	}

	public static int GetTenRingItemId(ItemType itemType)
	{
		ServerItem[] serverItemTable = ServerItem.GetServerItemTable(ServerItem.IdType.EQUIP_ITEM);
		int num = 11;
		return (int)serverItemTable[(int)(num * TEN_PACK_OFFSET + itemType)].id;
	}

	public static int GetOneRingItemContent(ItemType itemType)
	{
		int oneRingItemId = GetOneRingItemId(itemType);
		return GetRingItemContent(oneRingItemId);
	}

	public static int GetTenRingTenContent(ItemType itemType)
	{
		int tenRingItemId = GetTenRingItemId(itemType);
		return GetRingItemContent(tenRingItemId);
	}

	private static int GetRingItemContent(int serverItemId)
	{
		int result = 1;
		List<ServerConsumedCostData> costList = ServerInterface.CostList;
		if (costList == null)
		{
			return result;
		}
		foreach (ServerConsumedCostData item in costList)
		{
			if (item == null || item.consumedItemId != serverItemId)
			{
				continue;
			}
			return item.numItem;
		}
		return result;
	}

	public static ServerCampaignData GetCampaignDataInSession(int serverItemId)
	{
		if (ServerInterface.LoggedInServerInterface != null)
		{
			return ServerInterface.CampaignState.GetCampaignInSession(Constants.Campaign.emType.GameItemCost, serverItemId);
		}
		return null;
	}

	public static int GetOneItemCost(ItemType itemType)
	{
		int result = 200;
		int oneRingItemId = GetOneRingItemId(itemType);
		List<ServerConsumedCostData> costList = ServerInterface.CostList;
		if (costList == null)
		{
			return result;
		}
		foreach (ServerConsumedCostData item in costList)
		{
			if (item == null || item.consumedItemId != oneRingItemId)
			{
				continue;
			}
			result = item.numItem;
			ServerCampaignData campaignDataInSession = GetCampaignDataInSession(item.consumedItemId);
			if (campaignDataInSession != null)
			{
				return campaignDataInSession.iContent;
			}
			return result;
		}
		return result;
	}

	public static int GetTenItemCost(ItemType itemType)
	{
		int result = 200;
		int tenRingItemId = GetTenRingItemId(itemType);
		List<ServerConsumedCostData> costList = ServerInterface.CostList;
		foreach (ServerConsumedCostData item in costList)
		{
			if (item == null || item.consumedItemId != tenRingItemId)
			{
				continue;
			}
			return item.numItem;
		}
		return result;
	}

	public static string GetOneItemCostString(ItemType itemType)
	{
		int oneItemCost = GetOneItemCost(itemType);
		return HudUtility.GetFormatNumString(oneItemCost);
	}

	public static string GetTenItemCostString(ItemType itemType)
	{
		int tenItemCost = GetTenItemCost(itemType);
		return HudUtility.GetFormatNumString(tenItemCost);
	}

	public static MsgMenuItemSetStart.SetMode GetMsgMenuItemSetStartMode()
	{
		MsgMenuItemSetStart.SetMode result = MsgMenuItemSetStart.SetMode.NORMAL;
		if (HudMenuUtility.IsItemTutorial())
		{
			if (HudMenuUtility.itemSelectMode != HudMenuUtility.ITEM_SELECT_MODE.EVENT_STAGE && HudMenuUtility.itemSelectMode != HudMenuUtility.ITEM_SELECT_MODE.EVENT_BOSS)
			{
				result = MsgMenuItemSetStart.SetMode.TUTORIAL;
			}
		}
		else if (HudMenuUtility.IsTutorial_SubCharaItem())
		{
			result = MsgMenuItemSetStart.SetMode.TUTORIAL_SUBCHARA;
		}
		return result;
	}
}
